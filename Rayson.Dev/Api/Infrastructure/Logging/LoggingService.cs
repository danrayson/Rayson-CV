using Application.Logging;
using Serilog;
using Serilog.Events;

namespace Infrastructure.Logging;

public class LoggingService : ILoggingService
{
    public Task LogClientEventAsync(ClientLogEvent logEvent, string? userId = null)
    {
        var logger = Log.ForContext("SourceContext", logEvent.Source ?? "Client");

        if (userId is not null)
        {
            logger = logger.ForContext("UserId", userId);
        }
        if (logEvent.BrowserInfo is not null)
        {
            logger = logger.ForContext("BrowserInfo", logEvent.BrowserInfo);
        }
        if (logEvent.StackTrace is not null)
        {
            logger = logger.ForContext("StackTrace", logEvent.StackTrace);
        }
        if (logEvent.AdditionalData is not null)
        {
            logger = logger.ForContext("AdditionalData", logEvent.AdditionalData);
        }

        var level = Enum.Parse<LogEventLevel>(logEvent.Level, ignoreCase: true);
        logger.Write(level, logEvent.Message);

        return Task.CompletedTask;
    }
}
