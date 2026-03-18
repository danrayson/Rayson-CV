using Microsoft.Extensions.Diagnostics.HealthChecks;
using Rayson.Ollama;

namespace Infrastructure.Health;

public class OllamaHealthCheck(IOllamaService ollamaService) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var models = await ollamaService.ListModelsAsync();
            return HealthCheckResult.Healthy($"Ollama is running with {models.Count()} models available");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Ollama connection failed", ex);
        }
    }
}
