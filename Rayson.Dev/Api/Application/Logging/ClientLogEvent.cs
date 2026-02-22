namespace Application.Logging;

public class ClientLogEvent
{
    public string Level { get; set; } = "Information";
    public string Message { get; set; } = string.Empty;
    public string? Source { get; set; }
    public string? UserId { get; set; }
    public string? BrowserInfo { get; set; }
    public string? StackTrace { get; set; }
    public Dictionary<string, object>? AdditionalData { get; set; }
}
