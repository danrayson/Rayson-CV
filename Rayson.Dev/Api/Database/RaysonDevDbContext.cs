using Database.Auth;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class RaysonDevDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>, IDataProtectionKeyContext
{
    //Need twin constructurs because IdentityDbContext won't work in migrations without a zero parameter contructor.
    public RaysonDevDbContext()
        : base() { }
    //Need to keep this one because it's the one we actually use during runtime.
    public RaysonDevDbContext(DbContextOptions<RaysonDevDbContext> options)
        : base(options) { }

    //Used in the identity stuff for generating tokens or something
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}