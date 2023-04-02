using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using MongoDB.Driver;
using Pork.Controller.Dtos;
using Pork.Shared;
using Pork.Shared.Entities;
using Pork.Shared.Entities.Messages.Requests;
using ILogger = Serilog.ILogger;

namespace Pork.Controller;

public class ClientController : IAsyncDisposable {
    private readonly WebSocket webSocket;
    private readonly Client client;
    private readonly DataContext dataContext;

    public ILogger Logger { get; }


    public ClientController(DataContext dataContext, ILogger logger, WebSocket webSocket, Client client) {
        this.dataContext = dataContext;
        this.webSocket = webSocket;
        this.client = client;
        Logger = logger;
    }


    private async Task HandleAsync() {
        var buffer = new byte[1024 * 4];

        while (true) {
            var messageBuilder = new StringBuilder();
            WebSocketReceiveResult result;
            do {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                switch (result.MessageType) {
                    case WebSocketMessageType.Close:
                        return;
                    case WebSocketMessageType.Text:
                        messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                        break;
                }
            } while (!result.EndOfMessage);

            // update last seen, refactor to method todo
            await dataContext.Clients.UpdateOneAsync(c => c.Id == client.Id,
                Builders<Client>.Update.Set(c => c.LastSeen, DateTimeOffset.Now));


            var message = messageBuilder.ToString();
            _ = OnMessageAsync(message); // we don't want to wait for the message to be handled
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

            var clientResponse = DtoMapper.MapExternalResponse(client.ClientId, response);
            await dataContext.ClientMessages.InsertOneAsync(clientResponse);
        }
        catch (Exception e) {
            Logger.Error(e, "An exception occured while handling message {Message}", message);
        }
    }

    private async Task<bool> SendAsync(ClientRequest request) {
        try {
            var success = DtoSerializer.TrySerializeRequest(request, out var json);
            if (!success) {
                throw new Exception($"Failed to serialize request with id {request.Id}");
            }

            var bytes = Encoding.UTF8.GetBytes(json);
            await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true,
                CancellationToken.None);

            // set sent boolean
            await dataContext.ClientMessages.OfType<ClientRequest>()
                .UpdateOneAsync(r => r.Id == request.Id,
                    Builders<ClientRequest>.Update.Set(r => r.Sent, true).Set(r => r.SentAt, DateTimeOffset.Now));
            return true;
        }
        catch (Exception e) {
            Logger.Error(e, "An exception occured while sending message {@Message}", request);
            return false;
        }
    }

    public async Task RunAsync() {
        try {
            await HandleAsync();
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