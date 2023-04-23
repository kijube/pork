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

    private static async Task<LocalClient> GetOrCreateClientAsync(DataContext dataContext, HttpContext context,
        Guid clientId) {
        // get existing global client or create a new one
        var globalClient = await dataContext.GlobalClients.FirstOrDefaultAsync(c => c.Id == clientId);
        if (globalClient is null) {
            globalClient = new GlobalClient
            {
                Id = clientId
            };
            await dataContext.GlobalClients.AddAsync(globalClient);
        }

        globalClient.RemoteIp = context.Connection.RemoteIpAddress?.ToString();

        // get existing site or create a new one
        var origin = new Uri(context.Request.Headers["Origin"].ToString());
        var site = await dataContext.Sites.FirstOrDefaultAsync(s => s.Key == Site.NormalizeKey(origin.Host));

        if (site is null) {
            site = new Site
            {
                Key = Site.NormalizeKey(origin.Host),
                LocalClients = new List<LocalClient>()
            };
            await dataContext.Sites.AddAsync(site);
        }

        // get existing local client or create a new one
        var localClient = await dataContext.LocalClients.FirstOrDefaultAsync(c =>
            c.GlobalClientId == globalClient.Id && c.SiteId == site.Id);

        if (localClient is null) {
            localClient = new LocalClient
            {
                GlobalClient = globalClient,
                Site = site
            };
            await dataContext.LocalClients.AddAsync(localClient);
        }


        await dataContext.SaveChangesAsync();
        return localClient;
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
                Expires = DateTimeOffset.MaxValue, SameSite = SameSiteMode.Strict, HttpOnly = true
            });
        }

        if (!Guid.TryParse(clientIdStr, out var clientId)) {
            context.Response.StatusCode = 404;
            return;
        }

        ClientController controller;
        LocalClient localClient;
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();

        try {
            await ConnectionLock.WaitAsync();
            localClient = await GetOrCreateClientAsync(readContext, context, clientId);
            controller = new ClientController(readContext, writeContext,
                ClientLogManager.GetClientLogger(localClient.Id,
                    localClient.GlobalClient.RemoteIp ?? "[unknown]"),
                webSocket,
                localClient);

            if (ClientManager.TryGetController(clientId, out _)) {
                controller.Logger.Warning("A second connection was tried to be opened for the same client");
                return;
            }

            ClientManager.Controllers.TryAdd(clientId, controller);
        }
        catch (Exception e) {
            Log.Error(e, "An error occurred while handling a client connection");
            return;
        }
        finally {
            ConnectionLock.Release();
        }

        try {
            controller.Logger.Information("Client connected from {RemoteIp}", localClient.GlobalClient.RemoteIp);
            localClient.IsOnline = true;
            localClient.LastSeen = DateTimeOffset.UtcNow;
            await readContext.SaveChangesAsync();
            await controller.RunAsync();
            localClient.IsOnline = false;
            await readContext.SaveChangesAsync();
            controller.Logger.Information("Client disconnected");
        }
        catch (Exception e) {
            Log.Error(e, "An error occurred while handling a client connection from {RemoteIp}/{ClientId}",
                localClient.GlobalClient.RemoteIp, localClient.Id);
        }
        finally {
            ClientManager.Controllers.Remove(clientId, out _);
        }
    }
}