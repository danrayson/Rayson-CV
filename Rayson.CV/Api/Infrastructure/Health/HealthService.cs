using Application.Core;
using Application.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Infrastructure.Health;

public class HealthService : IHealthService
{
    private readonly HealthCheckService _healthCheckService;

    public HealthService(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    public async Task<ServiceResponse<HealthResult>> GetHealthAsync()
    {
        var report = await _healthCheckService.CheckHealthAsync();
        return MapReportToResponse(report);
    }

    public async Task<ServiceResponse<HealthResult>> GetLivenessAsync()
    {
        var report = await _healthCheckService.CheckHealthAsync(_ => false);
        return MapReportToResponse(report);
    }

    public async Task<ServiceResponse<HealthResult>> GetReadinessAsync()
    {
        var report = await _healthCheckService.CheckHealthAsync(
            check => check.Tags.Contains("ready"));
        return MapReportToResponse(report);
    }

    private static ServiceResponse<HealthResult> MapReportToResponse(HealthReport report)
    {
        var result = new HealthResult
        {
            Status = report.Status.ToString(),
            Checks = report.Entries.ToDictionary(
                e => e.Key,
                e => new HealthCheckResultEntry
                {
                    Status = e.Value.Status.ToString(),
                    Description = e.Value.Description,
                    Duration = e.Value.Duration.TotalMilliseconds
                }
            ),
            TotalDuration = report.TotalDuration.TotalMilliseconds
        };

        return report.Status == HealthStatus.Healthy
            ? ServiceResponse.Succeed(result)
            : ServiceResponse<HealthResult>.ServiceUnavailable(result);
    }
}
