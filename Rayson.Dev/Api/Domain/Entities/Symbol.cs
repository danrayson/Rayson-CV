namespace Domain.Entities;

public class Symbol : Entity
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }

    // Navigation properties
    public ICollection<Exchange> Exchanges { get; set; } = new HashSet<Exchange>();
    public ICollection<MarketPricePoint> DailyMarketDatas { get; set; } = new HashSet<MarketPricePoint>();
}