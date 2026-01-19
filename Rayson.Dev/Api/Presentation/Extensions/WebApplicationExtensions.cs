using Database;
using Database.SeedData;
using Microsoft.EntityFrameworkCore;

namespace Presentation.Extensions;

public static class WebApplicationExtensions
{
    public static void RunMigrations(this WebApplication webApplication)
    {
        var scope = webApplication.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TradePulseDbContext>();
        context.Database.Migrate();
        var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        seeder.SeedDatabase();
    }
}
