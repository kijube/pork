using Serilog;
using Serilog.Events;
using ILogger = Serilog.ILogger;

namespace Pork.Shared;

public static class LoggerUtils {
    private const string LogFormat =
        "{Timestamp:dd.MM.yyyy HH:mm:ss} | {Level:u3} | {Message:lj}{NewLine}{Exception}";

    public static ILogger CreateLogger() {
        
        var config = new LoggerConfiguration().MinimumLevel
            .Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Extensions.Http", LogEventLevel.Warning)
            .Filter.ByExcluding(
                logEvent =>
                    logEvent.Exception is not null && logEvent.Exception.GetType().Name.Contains("ServiceException")
            )
            .Filter.ByExcluding(
                ev => ev.MessageTemplate.Text.Contains("HttpMessageHandler")
            )
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: LogFormat);


        return config.CreateLogger();
    }
}