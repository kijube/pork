using System.Net.WebSockets;
using MongoDB.Driver;
using Pork.Shared;
using Pork.Shared.Entities;
using Serilog;
using Serilog.Sinks.PeriodicBatching;
using ILogger = Serilog.ILogger;

namespace Pork.Controller;

public class ClientConnector {
    private static readonly SemaphoreSlim ConnectionLock = new(1, 1);

    private readonly DataContext dataContext;

    public ClientConnector(DataContext dataContext) {
        this.dataContext = dataContext;
    }

    private async Task<Client> GetOrCreateClientAsync(HttpContext context, string clientId) {
        var client = await dataContext.Clients.Find(c => c.ClientId == clientId).FirstOrDefaultAsync();
        if (client is null) {
            client = new Client {ClientId = clientId};
            await dataContext.Clients.InsertOneAsync(client);
        }

        client.RemoteIp = context.Connection.RemoteIpAddress?.ToString();
        await dataContext.Clients.UpdateOneAsync(c => c.ClientId == clientId,
            Builders<Client>.Update.Set(c => c.RemoteIp, client.RemoteIp));
        return client;
    }

    public async Task ConnectAsync(HttpContext context) {
        var hasClientId = context.Request.Cookies.TryGetValue("clientId", out var clientId);
        if (!hasClientId || clientId is null) {
            clientId = Guid.NewGuid().ToString();
            context.Response.Cookies.Append("clientId", clientId, new CookieOptions
            {
                Expires = null, SameSite = SameSiteMode.Strict, HttpOnly = true
            });
        }

        ClientController controller;
        Client client;
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();

        try {
            await ConnectionLock.WaitAsync();
            client = await GetOrCreateClientAsync(context, clientId);
            controller = BuildController(webSocket, client);

            if (ClientManager.TryGetController(clientId, out _)) {
                controller.Logger.Warning("A second connection was tried to be opened for the same client");
                return;
            }

            ClientManager.Controllers.TryAdd(clientId, controller);
        }
        finally {
            ConnectionLock.Release();
        }

        try {
            controller.Logger.Information("Client connected from {RemoteIp}", client.RemoteIp);

            await UpdateOnlineStatusAsync(clientId, true);
            await UpdateLastSeenAsync(clientId);
            await controller.RunAsync();
            await UpdateOnlineStatusAsync(clientId, false);
        }
        catch (Exception e) {
            Log.Error(e, "An error occurred while handling a client connection from {RemoteIp}/{ClientId}",
                client.RemoteIp, client.Id);
        }
        finally {
            ClientManager.Controllers.Remove(clientId, out _);
        }
    }

    private async Task UpdateOnlineStatusAsync(string clientId, bool isOnline) {
        await dataContext.Clients.UpdateOneAsync(c => c.ClientId == clientId,
            Builders<Client>.Update.Set(c => c.IsOnline, isOnline));
    }

    private async Task UpdateLastSeenAsync(string clientId) {
        await dataContext.Clients.UpdateOneAsync(c => c.ClientId == clientId,
            Builders<Client>.Update.Set(c => c.LastSeen, DateTimeOffset.Now));
    }

    private ClientController BuildController(WebSocket webSocket, Client client) {
        var logger = CreateLogger(client);
        return new ClientController(dataContext, logger, webSocket, client);
    }

    private ILogger CreateLogger(Client client) {
        var logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.WithProperty("ClientId", client.ClientId)
            .Enrich.WithProperty("RemoteIp", client.RemoteIp ??
                                             "[unknown]")
            .WriteTo.Console(
                outputTemplate:
                "{Timestamp:dd.MM.yyyy HH:mm:ss} | {ClientId} | {Level:u3} | {Message:lj}{NewLine}{Exception}")
            .WriteTo.Sink(new PeriodicBatchingSink(new ClientLogSink(dataContext),
                new PeriodicBatchingSinkOptions()));
        return logger.CreateLogger();
    }
}