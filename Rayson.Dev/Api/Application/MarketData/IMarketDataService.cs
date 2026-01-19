using Domain.Entities;

namespace Application.MarketData;

public interface IMarketDataService
{
    Task<List<MarketPricePoint>> GetMarketData(string symbol, string exchangeIdentifier, DateOnly startDateInclusive, DateOnly endDateInclusive);
    Task<List<Exchange>> GetExchanges(string? symbol = null);
    Task<List<Symbol>> GetSymbols(string? exchanges = null);
}
