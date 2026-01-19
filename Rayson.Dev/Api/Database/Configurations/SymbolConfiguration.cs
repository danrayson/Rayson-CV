using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations;

public class SymbolConfiguration : IEntityTypeConfiguration<Symbol>
{
    public void Configure(EntityTypeBuilder<Symbol> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Description).IsRequired().HasMaxLength(255);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(30);
        builder.Property(e => e.Code).IsRequired().HasMaxLength(32);

        builder.HasMany(s => s.Exchanges)
            .WithMany(e => e.Symbols)
            .UsingEntity("SymbolExchange");

        builder.HasMany(s => s.DailyMarketDatas)
            .WithOne(dmd => dmd.Symbol)
            .HasForeignKey(dmd => dmd.SymbolId);
    }

}