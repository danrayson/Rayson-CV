using Database.SeedData;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Database.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDatabaseServices(this IServiceCollection services, string connectionString)
    {

        services.AddDbContext<TradePulseDbContext>((options) =>
        {
            options.UseNpgsql(connectionString);
        });
        
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IDataSeeder, DataSeeder>();
    }
}
