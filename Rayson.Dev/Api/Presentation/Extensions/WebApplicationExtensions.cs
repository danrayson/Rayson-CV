using Database;
using Database.SeedData;
using Microsoft.EntityFrameworkCore;

namespace Presentation.Extensions;

public static class WebApplicationExtensions
{
    public static async Task RunMigrations(this WebApplication webApplication)
    {
        var scope = webApplication.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RaysonDevDbContext>();
        await context.Database.MigrateAsync();
        var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        await seeder.SeedDatabase();
    }
}
