using Application.MarketData;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Presentation.Endpoints.DailyMarketData;

public static class DailyMarketDataEndpoints
{
    public static void MapDailyMarketDataEndpoints(this WebApplication webApplication)
    {
        var group = webApplication.MapGroup("markets")
            .AddFluentValidationAutoValidation();
        group.MapGet("marketdata/{symbol}/{exchange}/{startDate}/{endDate}", GetMarketDataEndpoint);
        group.MapGet("symbols", GetSymbolsEndpoint);
        group.MapGet("exchanges", GetExchangesEndpoint);
    }

    private static async Task<IResult> GetMarketDataEndpoint(
        [FromServices] IMarketDataService marketDataService,
        [FromServices] IMapper mapper,
        [AsParameters] DailyMarketDataRequest request)
    {
        var domainEntities = await marketDataService.GetMarketData(request.Symbol, request.Exchange, request.StartDate, request.EndDate);
        var dtos = mapper.Map<GetMarketDataResponse[]>(domainEntities);
        return Results.Ok(dtos);
    }

    private static async Task<IResult> GetSymbolsEndpoint(
        [FromServices] IMarketDataService marketDataService,
        [FromServices] IMapper mapper)
    {
        var domainEntities = await marketDataService.GetSymbols();
        var dtos = mapper.Map<GetSymbolsResponse[]>(domainEntities);
        return Results.Ok(dtos);
    }

    private static async Task<IResult> GetExchangesEndpoint(
        [FromServices] IMarketDataService marketDataService,
        [FromServices] IMapper mapper)
    {
        var domainEntities = await marketDataService.GetExchanges();
        var dtos = mapper.Map<GetExchangesResponse[]>(domainEntities.OrderBy(e => e.Name));
        return Results.Ok(dtos);
    }
}
