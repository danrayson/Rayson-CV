namespace Application.Logging;

public interface ILoggingService
{
    Task LogClientEventAsync(ClientLogEvent logEvent, string? userId = null);
}
