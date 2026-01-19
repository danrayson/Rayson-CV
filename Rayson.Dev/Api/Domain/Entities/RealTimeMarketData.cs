namespace Domain.Entities;

public class RealTimeMarketData : Entity
{
    public decimal Ask { get; set; }
    public decimal Bid { get; set; }
    public decimal Last { get; set; }
    public decimal PreviousClose { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public DateTime LastTradeTime { get; set; }
    public long Volume { get; set; }
    public required string  Symbol { get; set; }
    public required string Name { get; set; }
}
