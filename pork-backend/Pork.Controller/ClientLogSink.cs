using Pork.Shared;
using Pork.Shared.Entities;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;

namespace Pork.Controller;

public class ClientLogSink : IBatchedLogEventSink {
    private readonly DataContext dataContext;

    public ClientLogSink(DataContext dataContext) {
        this.dataContext = dataContext;
    }

    public async Task EmitBatchAsync(IEnumerable<LogEvent> batch) {
        await dataContext.ClientLogs.InsertManyAsync(batch.Select(ev => {
            var msg = ev.RenderMessage();
            if (ev.Exception is not null) {
                msg += $"\n{ev.Exception}";
            }

            return new ClientLog()
            {
                ClientId = ev.Properties["ClientId"].ToString()[1..^1],
                Level = ev.Level.ToString(),
                Message = msg,
                Timestamp = ev.Timestamp
            };
        }));
    }

    public Task OnEmptyBatchAsync() => Task.CompletedTask;
}