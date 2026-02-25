using Microsoft.EntityFrameworkCore;

namespace Database;

public class RaysonCVDbContext : DbContext
{
    public RaysonCVDbContext()
        : base() { }

    public RaysonCVDbContext(DbContextOptions<RaysonCVDbContext> options)
        : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql();
    }
}