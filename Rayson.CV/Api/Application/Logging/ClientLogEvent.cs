namespace Application.Logging;

public class ClientLogEvent
{
    public string? Level { get; set; } = "Information";
    public string Message { get; set; } = string.Empty;
    public string? Source { get; set; }
    public string EventType { get; set; } = "ClientLog";

    public string? BrowserInfo { get; set; }
    public string? StackTrace { get; set; }
    public Dictionary<string, object>? AdditionalData { get; set; }

    public string? Path { get; set; }
    public string? Referrer { get; set; }
    public string? CorrelationId { get; set; }
    public string? UserAgent { get; set; }
    public string? Language { get; set; }
    public int? ScreenWidth { get; set; }
    public int? ScreenHeight { get; set; }
    public string? Timezone { get; set; }

    public string? SectionId { get; set; }
    public double? Duration { get; set; }

    public string? ElementId { get; set; }
    public string? ElementText { get; set; }
}
