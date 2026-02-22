namespace Application.Health;

public class HealthCheckResultEntry
{
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; }
    public double Duration { get; set; }
}
