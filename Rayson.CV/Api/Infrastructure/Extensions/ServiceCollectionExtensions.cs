using Application.Health;
using Application.Logging;
using Infrastructure.Health;
using Infrastructure.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IHealthService, HealthService>();
        services.AddScoped<ILoggingService, LoggingService>();
    }
}
