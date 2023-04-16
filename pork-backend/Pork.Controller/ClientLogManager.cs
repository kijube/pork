using Pork.Shared;
using Pork.Shared.Entities;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;
using ILogger = Serilog.ILogger;

namespace Pork.Controller;

public static class ClientLogManager {
    private static readonly object InitLock = new();

    private static bool initialized;
    private static ILogger? logger;

    public static void Init(DataContext dataContext) {
        lock (InitLock) {
            if (initialized) return;
            logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(
                    outputTemplate:
                    "{Timestamp:dd.MM.yyyy HH:mm:ss} | {LocalClientId} | {Level:u3} | {Message:lj}{NewLine}{Exception}")
                .WriteTo.Sink(new PeriodicBatchingSink(new ClientLogSink(dataContext),
                    new PeriodicBatchingSinkOptions()))
                .CreateLogger();
            initialized = true;
        }
    }

    public static ILogger GetClientLogger(int clientId, string remoteIp) {
        if (logger is null) {
            throw new InvalidOperationException("ClientLogManager not initialized");
        }

        return new LoggerConfiguration()
            .Enrich.WithProperty("LocalClientId", clientId)
            .Enrich.WithProperty("RemoteIp", remoteIp)
            .WriteTo.Logger(logger)
            .CreateLogger();
    }
}

public class ClientLogSink : IBatchedLogEventSink {
    private readonly DataContext dataContext;

    public ClientLogSink(DataContext dataContext) {
        this.dataContext = dataContext;
    }

    public async Task EmitBatchAsync(IEnumerable<LogEvent> batch) {
        try {
            await dataContext.ClientLogs.AddRangeAsync(batch.Select(ev => {
                var msg = ev.RenderMessage();
                if (ev.Exception is not null) {
                    msg += $"\n{ev.Exception}";
                }

                return new ClientLog()
                {
                    LocalClientId = int.Parse(ev.Properties["LocalClientId"].ToString()),
                    Level = ev.Level.ToString(),
                    Message = msg,
                    Timestamp = ev.Timestamp.ToUniversalTime()
                };
            }));

            await dataContext.SaveChangesAsync();
        }
        catch (Exception e) {
            Log.Error(e, "Error while saving client logs");
        }
    }

    public Task OnEmptyBatchAsync() => Task.CompletedTask;
}