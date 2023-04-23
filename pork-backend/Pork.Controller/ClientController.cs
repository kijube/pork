using System.Net.WebSockets;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Pork.Controller.Dtos;
using Pork.Shared;
using Pork.Shared.Entities;
using Pork.Shared.Entities.Messages.Requests;
using Pork.Shared.Entities.Messages.Responses;
using ILogger = Serilog.ILogger;

namespace Pork.Controller;

public class ClientController : IAsyncDisposable {
    private readonly WebSocket webSocket;
    private readonly LocalClient localClient;
    private readonly DataContext readContext;
    private readonly DataContext writeContext;

    public ILogger Logger { get; }


    public ClientController(DataContext readContext, DataContext writeContext, ILogger logger, WebSocket webSocket,
        LocalClient localClient) {
        this.readContext = readContext;
        this.webSocket = webSocket;
        this.localClient = localClient;
        this.writeContext = writeContext;
        Logger = logger;
    }


    private async Task HandleAsync(CancellationTokenSource cts) {
        var buffer = new byte[1024 * 4];

        while (true) {
            var messageBuilder = new StringBuilder();
            WebSocketReceiveResult result;
            do {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                switch (result.MessageType) {
                    case WebSocketMessageType.Close:
                        cts.Cancel();
                        return;
                    case WebSocketMessageType.Text:
                        messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                        break;
                }
            } while (!result.EndOfMessage);

            localClient.LastSeen = DateTimeOffset.UtcNow;
            await readContext.SaveChangesAsync();


            var message = messageBuilder.ToString();
            await OnMessageAsync(message); // find way to not wait for handling
        }
    }

    private async Task OnMessageAsync(string message) {
        try {
            if (webSocket.State == WebSocketState.Closed) {
                return;
            }

            var success = DtoSerializer.TryMapResponse(message, out var response);
            if (!success) {
                Logger.Warning("Received invalid message {Message}", message);
                return;
            }

            if (!response.GetType().Name.StartsWith("External")) {
                // security feature: only allow External types
                Logger.Warning("Client tried to send non-external response {@Response}", response);
                return;
            }

            var clientResponse = DtoMapper.MapExternalResponse(localClient.Id, response);

            switch (clientResponse) {
                case ClientEvalResponse {FlowId: not null} eval:
                    // handle eval differently as we save it together with the request
                    await HandleEvalResponseAsync(eval);
                    return;
                case ClientHookResponse hookResponse:
                    if (hookResponse.Method.StartsWith("console.")) {
                        // log console messages to console
                        hookResponse.ShowInConsole = true;
                    }

                    break;
            }

            await HandleResponseAsync(clientResponse);
        }
        catch (Exception e) {
            Logger.Error(e, "An exception occured while handling message {Message}", message);
        }
    }

    private async Task HandleResponseAsync(ClientResponse response) {
        await readContext.ClientMessages.AddAsync(response);
        await readContext.SaveChangesAsync();
    }

    private async Task HandleEvalResponseAsync(ClientEvalResponse eval) {
        // get request
        var request = await readContext.ClientEvalRequests
            .FirstOrDefaultAsync(r => r.FlowId == eval.FlowId);

        if (request is null) {
            // no request found, just save the response
            await HandleResponseAsync(eval);
            return;
        }

        // update request with response
        request.Response = eval;
        await readContext.SaveChangesAsync();
    }

    private async Task<bool> SendAsync(ClientRequest request) {
        try {
            var success = DtoSerializer.TrySerializeRequest(request, out var json);
            if (!success) {
                return false;
            }

            var bytes = Encoding.UTF8.GetBytes(json);
            await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true,
                CancellationToken.None);

            // set sent boolean
            request.Sent = true;
            request.SentAt = DateTimeOffset.UtcNow;
            await writeContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e) {
            Logger.Error(e, "An exception occured while sending message {@Message}", request);
            return false;
        }
    }

    private async Task RunSenderAsync(CancellationToken ct) {
        while (webSocket.CloseStatus is null && !ct.IsCancellationRequested) {
            var request = await writeContext.ClientMessages.OfType<ClientRequest>()
                .FirstOrDefaultAsync(r => r.LocalClientId == localClient.Id && !r.Sent, cancellationToken: ct);

            if (request is not null) {
                var ok = await SendAsync(request);
                if (ok) {
                    request.SentAt = DateTimeOffset.UtcNow;
                    request.Sent = true;
                    await writeContext.SaveChangesAsync(ct);
                }
                else {
                    Logger.Error("Failed to send message {@Message}", request);
                    await Task.Delay(TimeSpan.FromSeconds(5), ct);
                }
            }
            else {
                await Task.Delay(TimeSpan.FromSeconds(2), ct);
            }
        }
    }

    private async Task SendIdAsync(CancellationToken ct) {
        var bytes = Encoding.UTF8.GetBytes(localClient.GlobalClientId.ToString());
        await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true,
            ct);
    }

    public async Task RunAsync() {
        using var cts = new CancellationTokenSource();

        try {
            await SendIdAsync(cts.Token);
            _ = RunSenderAsync(cts.Token);
            await HandleAsync(cts);
        }
        catch (Exception e) {
            Logger.Error(e, "An exception occured");
        }

        await DisposeAsync();
    }

    public async ValueTask DisposeAsync() {
        if (!webSocket.CloseStatus.HasValue)
            await webSocket.CloseAsync(WebSocketCloseStatus.Empty, "", CancellationToken.None);
        webSocket.Dispose();
    }
}