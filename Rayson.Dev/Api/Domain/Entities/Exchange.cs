namespace Domain.Entities;

public class Exchange : Entity
{
    public required string Name { get; set; }
    public required string Code { get; set; }

    // Navigation properties
    public ICollection<Symbol> Symbols { get; set; } = new HashSet<Symbol>();
    public ICollection<MarketPricePoint> DailyMarketDatas { get; set; } = new HashSet<MarketPricePoint>();
}