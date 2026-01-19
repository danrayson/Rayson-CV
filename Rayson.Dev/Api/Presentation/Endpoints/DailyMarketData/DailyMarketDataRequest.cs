namespace Presentation.Endpoints.DailyMarketData;

public class DailyMarketDataRequest
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public required string Symbol { get; set; }
    public required string Exchange { get; set; }
}
