using Database;
using Database.SeedData;
using Infrastructure.RAG;
using Microsoft.EntityFrameworkCore;

namespace Presentation.Extensions;

public static class WebApplicationExtensions
{
    public static async Task RunMigrationsAsync(this WebApplication webApplication)
    {
        var scope = webApplication.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RaysonCVDbContext>();
        await context.Database.MigrateAsync();
        var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        await seeder.SeedDatabaseAsync();
    }

    public static async Task InitializeRagAsync(this WebApplication webApplication)
    {
        var scope = webApplication.Services.CreateScope();
        var ragService = scope.ServiceProvider.GetRequiredService<IRagService>();
        await ragService.InitializeAsync();
    }
}
