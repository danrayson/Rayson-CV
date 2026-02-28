using System.Globalization;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CvChunk>(entity =>
        {
            entity.Property(e => e.Embedding)
                .HasColumnType("vector(768)")
                .HasConversion(
                    v => "[" + string.Join(",", v.Select(e => e.ToString("G10", CultureInfo.InvariantCulture))) + "]",
                    v => v.Trim('[', ']').Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                          .Select(double.Parse)
                          .ToArray()
                );
        });
    }
}