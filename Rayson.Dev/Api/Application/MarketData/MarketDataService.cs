using Domain.Entities;
using Domain.Repositories;

namespace Application.MarketData;

public class MarketDataService(
    IRepository<MarketPricePoint> marketDataRepo,
    IRepository<Exchange> exchangeRepo,
    IRepository<Symbol> symbolRepo) : IMarketDataService
{
    private readonly IRepository<MarketPricePoint> marketDataRepo = marketDataRepo;
    private readonly IRepository<Exchange> exchangeRepo = exchangeRepo;
    private readonly IRepository<Symbol> symbolRepo = symbolRepo;

    public async Task<List<Exchange>> GetExchanges(string? symbol = null)
    {
        var query = exchangeRepo.Query().Where(e => symbol == null || e.Symbols.Select(s => s.Name).Contains(symbol));
        return await exchangeRepo.GetListAsync(query);
    }

    public async Task<List<MarketPricePoint>> GetMarketData(string symbol, string exchangeIdentifier, DateOnly startDateInclusive, DateOnly endDateInclusive)
    {
        var query = marketDataRepo.Query().Where(d =>
            d.Exchange.Name == exchangeIdentifier
            && d.Symbol.Name == symbol
            && d.DateUtc >= startDateInclusive
            && d.DateUtc <= endDateInclusive);
        return await marketDataRepo.GetListAsync(query);
    }

    public async Task<List<Symbol>> GetSymbols(string? exchanges)
    {
        var query = symbolRepo.Query();
        return await symbolRepo.GetListAsync(query);
    }
}
