using Application.Logging;
using Serilog;

namespace Infrastructure.Logging;

public class LoggingService : ILoggingService
{
    public Task LogClientEventAsync(ClientLogEvent logEvent, string? userId = null, string? userCorrelationId = null)
    {
        var logger = Log.ForContext("SourceContext", "Client")
            .ForContext("EventType", logEvent.EventType);

        if (userId != null)
            logger = logger.ForContext("UserId", userId);

        if (userCorrelationId != null)
            logger = logger.ForContext("UserCorrelationId", userCorrelationId);

        Action<ILogger> logCall = logEvent.EventType switch
        {
            "PageView" => l => l.Information("PageView - {PageName}", logEvent.Path),
            "SectionVisible" or "SectionHidden" => l => l.Information("{EventType} - {SectionId}", logEvent.EventType, logEvent.SectionId),
            "Click" => l => l.Information("Click - {ElementText}", logEvent.ElementText ?? logEvent.ElementId),
            "ApiCall" => l => l.Information("ApiCall - {Path}", logEvent.Path),
            "Error" => l => l.Error("Error - {ErrorMessage}", logEvent.Message ?? "Unknown error"),
            _ => l => l.Information("Client event: {EventType} - {Message}", logEvent.EventType, logEvent.Message)
        };

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
            "Click" => logger
                .ForContext("ElementId", logEvent.ElementId)
                .ForContext("ElementText", logEvent.ElementText)
                .ForContext("CorrelationId", logEvent.CorrelationId),
            "ApiCall" => logger
                .ForContext("Path", logEvent.Path)
                .ForContext("CorrelationId", logEvent.CorrelationId),
            "Error" => logger
                .ForContext("BrowserInfo", logEvent.BrowserInfo)
                .ForContext("StackTrace", logEvent.StackTrace)
                .ForContext("AdditionalData", logEvent.AdditionalData)
                .ForContext("CorrelationId", logEvent.CorrelationId),
            _ => logger
                .ForContext("BrowserInfo", logEvent.BrowserInfo)
                .ForContext("StackTrace", logEvent.StackTrace)
                .ForContext("AdditionalData", logEvent.AdditionalData)
        };

        if (logEvent is { EventType: "ApiCall", Path: "/logs" })
        {
            return Task.CompletedTask;
        }

        logCall(logger);

        return Task.CompletedTask;
    }
}
