using Microsoft.EntityFrameworkCore;
using Domain;

namespace Database;

public class RaysonCVDbContext : DbContext
{
    public RaysonCVDbContext()
        : base() { }

    public RaysonCVDbContext(DbContextOptions<RaysonCVDbContext> options)
        : base(options) { }

    public DbSet<CvChunk> CvChunks { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql();
    }
}