using Database.HealthChecks;
using Database.SeedData;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Database.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDatabaseServices(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<RaysonDevDbContext>((options) =>
        {
            options.UseNpgsql(connectionString);
        });
        
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IDataSeeder, DataSeeder>();
        
        services.AddHealthChecks()
            .AddCheck<NpgsqlHealthCheck>("postgresql", tags: new[] { "ready" });
    }
}
