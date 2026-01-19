using AutoMapper;
using Domain.Entities;

namespace Presentation.Endpoints.DailyMarketData;

public class DailyMarketDataProfiles : Profile
{
    public DailyMarketDataProfiles()
    {
        CreateMap<Exchange, GetExchangesResponse>();
    }
}
