using Application.Health;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Endpoints.Health;

public static class HealthEndpoints
{
    public static void MapHealthEndpoints(this WebApplication webApplication)
    {
        var group = webApplication.MapGroup("health");
        
        group.MapGet("", GetHealth);
        group.MapGet("/live", GetLiveness);
        group.MapGet("/ready", GetReadiness);
    }

    private static async Task<IResult> GetHealth([FromServices] IHealthService healthService)
    {
        var response = await healthService.GetHealthAsync();
        return response.ToHealthHttpResult();
    }

    private static async Task<IResult> GetLiveness([FromServices] IHealthService healthService)
    {
        var response = await healthService.GetLivenessAsync();
        return response.ToHealthHttpResult();
    }

    private static async Task<IResult> GetReadiness([FromServices] IHealthService healthService)
    {
        var response = await healthService.GetReadinessAsync();
        return response.ToHealthHttpResult();
    }
}
