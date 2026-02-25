using Database.Exceptions;
using Database.HealthChecks;
using Database.SeedData;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Database.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = BuildConnectionString(configuration);
        
        services.AddDbContext<RaysonCVDbContext>((options) =>
        {
            options.UseNpgsql(connectionString);
        });
        
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IDataSeeder, DataSeeder>();
        
        services.AddHealthChecks()
            .AddCheck<NpgsqlHealthCheck>("postgresql", tags: new[] { "ready" });
    }

    private static string BuildConnectionString(IConfiguration configuration)
    {
        var host = configuration["POSTGRES_HOST"];
        var port = configuration["POSTGRES_PORT"];
        var username = configuration["POSTGRES_USERNAME"];
        var password = configuration["POSTGRES_PASSWORD"];
        var database = configuration["POSTGRES_DATABASE"];
        
        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(port) || 
            string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || 
            string.IsNullOrEmpty(database))
        {
            throw new DatabaseConfigurationException("Database connection string is missing. Please provide all PostgreSQL environment variables (POSTGRES_HOST, POSTGRES_PORT, POSTGRES_USERNAME, POSTGRES_PASSWORD, POSTGRES_DATABASE).");
        }
        
        return $"Server={host};Port={port};Database={database};User Id={username};Password={password};TrustServerCertificate=True";
    }
}
