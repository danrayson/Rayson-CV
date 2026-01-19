using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations;

public class ExchangeConfiguration : IEntityTypeConfiguration<Exchange>
    {
        public void Configure(EntityTypeBuilder<Exchange> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(255);
            builder.Property(e => e.Code).IsRequired().HasMaxLength(32);

            // Navigation properties for many-to-many relationship with Symbol
            // See Symbol config

            // Navigation properties for one-to-many relationship with DailyMarketData
            builder.HasMany(e => e.DailyMarketDatas)
                .WithOne(dmd => dmd.Exchange)
                .HasForeignKey(dmd => dmd.ExchangeId);
        }
    }