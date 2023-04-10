using System.Net.WebSockets;
using Microsoft.EntityFrameworkCore;
using Pork.Shared;
using Pork.Shared.Entities;
using Serilog;

namespace Pork.Controller;

public class ClientConnector {
    private static readonly SemaphoreSlim ConnectionLock = new(1, 1);

    private readonly IServiceProvider serviceProvider;

    public ClientConnector(IServiceProvider serviceProvider) {
        this.serviceProvider = serviceProvider;
    }

    private async Task<Client> GetOrCreateClientAsync(DataContext dataContext, HttpContext context, Guid clientId) {
        var client = await dataContext.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId);
        if (client is null) {
            client = new Client {ClientId = clientId};
            await dataContext.Clients.AddAsync(client);
        }

        client.RemoteIp = context.Connection.RemoteIpAddress?.ToString();
        await dataContext.SaveChangesAsync();
        return client;
    }

    public async Task ConnectAsync(HttpContext context) {
        using var readScope = serviceProvider.CreateScope();
        using var writeScope = serviceProvider.CreateScope();
        await using var readContext = readScope.ServiceProvider.GetRequiredService<DataContext>();
        await using var writeContext = writeScope.ServiceProvider.GetRequiredService<DataContext>();

        var hasClientId = context.Request.Cookies.TryGetValue("clientId", out var clientIdStr);
        if (!hasClientId || clientIdStr is null) {
            clientIdStr = Guid.NewGuid().ToString();
            context.Response.Cookies.Append("clientId", clientIdStr, new CookieOptions
            {
                Expires = null, SameSite = SameSiteMode.Strict, HttpOnly = true
            });
        }

        if (!Guid.TryParse(clientIdStr, out var clientId)) {
            context.Response.StatusCode = 404;
            return;
        }

        ClientController controller;
        Client client;
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();

        try {
            await ConnectionLock.WaitAsync();
            client = await GetOrCreateClientAsync(writeContext, context, clientId);
            controller = new ClientController(readContext, writeContext,
                ClientLogManager.GetClientLogger(client.ClientId, client.RemoteIp ?? "[unknown]"), webSocket,
                client);

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
            client.IsOnline = true;
            client.LastSeen = DateTimeOffset.UtcNow;
            await writeContext.SaveChangesAsync();
            await controller.RunAsync();
            client.IsOnline = false;
            await writeContext.SaveChangesAsync();
            controller.Logger.Information("Client disconnected");
        }
        catch (Exception e) {
            Log.Error(e, "An error occurred while handling a client connection from {RemoteIp}/{ClientId}",
                client.RemoteIp, client.Id);
        }
        finally {
            ClientManager.Controllers.Remove(clientId, out _);
        }
    }
}