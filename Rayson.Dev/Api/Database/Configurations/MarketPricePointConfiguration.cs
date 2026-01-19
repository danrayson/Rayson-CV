using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations;

public class MarketPricePointConfiguration : IEntityTypeConfiguration<MarketPricePoint>
{
    public void Configure(EntityTypeBuilder<MarketPricePoint> builder)
    {
        builder.HasKey(dmd => dmd.Id);
        builder.Property(dmd => dmd.DateUtc).IsRequired();
        builder.Property(dmd => dmd.Timeframe).IsRequired();
        builder.Property(dmd => dmd.OpenPrice).IsRequired().HasColumnType("decimal(19,9)");
        builder.Property(dmd => dmd.HighPrice).IsRequired().HasColumnType("decimal(19,9)");
        builder.Property(dmd => dmd.LowPrice).IsRequired().HasColumnType("decimal(19,9)");
        builder.Property(dmd => dmd.ClosePrice).IsRequired().HasColumnType("decimal(19,9)");
        builder.Property(dmd => dmd.AdjustedPrice).IsRequired().HasColumnType("decimal(19,9)");

        // Navigation properties
        builder.HasOne(dmd => dmd.Symbol)
            .WithMany(s => s.DailyMarketDatas)
            .HasForeignKey(dmd => dmd.SymbolId);

        builder.HasOne(dmd => dmd.Exchange)
            .WithMany(e => e.DailyMarketDatas)
            .HasForeignKey(dmd => dmd.ExchangeId);
    }
}