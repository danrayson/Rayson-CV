namespace Application.Health;

public class HealthResult
{
    public string Status { get; set; } = string.Empty;
    public Dictionary<string, HealthCheckResultEntry> Checks { get; set; } = new();
    public double TotalDuration { get; set; }
}
