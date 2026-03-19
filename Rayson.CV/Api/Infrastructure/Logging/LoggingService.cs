using Application.Logging;
using Serilog;
using Serilog.Events;

namespace Infrastructure.Logging;

public class LoggingService : ILoggingService
{
    public Task LogClientEventAsync(ClientLogEvent logEvent, string? userId = null)
    {
        var logger = Log.ForContext("SourceContext", "Client")
            .ForContext("EventType", logEvent.EventType);

        if (userId != null)
            logger = logger.ForContext("UserId", userId);

        logger = logEvent.EventType switch
        {
            "PageView" => logger
                .ForContext("PagePath", logEvent.Path)
                .ForContext("Referrer", logEvent.Referrer)
                .ForContext("CorrelationId", logEvent.CorrelationId)
                .ForContext("BrowserInfo", new
                {
                    logEvent.UserAgent,
                    logEvent.Language,
                    logEvent.ScreenWidth,
                    logEvent.ScreenHeight,
                    logEvent.Timezone
                }),
            "SectionVisible" or "SectionHidden" => logger
                .ForContext("SectionId", logEvent.SectionId)
                .ForContext("Duration", logEvent.Duration)
                .ForContext("CorrelationId", logEvent.CorrelationId),
            _ => logger
                .ForContext("BrowserInfo", logEvent.BrowserInfo)
                .ForContext("StackTrace", logEvent.StackTrace)
                .ForContext("AdditionalData", logEvent.AdditionalData)
        };

        var level = Enum.Parse<LogEventLevel>(logEvent.Level ?? "Information", ignoreCase: true);

        // Skip ApiCall events to /logs - would cause infinite loop
        // All other ApiCall events are logged to track UI→API response times
        if (logEvent is { EventType: "ApiCall", Path: "/logs" })
        {
            return Task.CompletedTask;
        }

        logger.Write(level, "Client event: {EventType} - {Message}", logEvent.EventType, logEvent.Message);

        return Task.CompletedTask;
    }
}
