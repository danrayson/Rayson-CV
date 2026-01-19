using FluentValidation;

namespace Presentation.Endpoints.DailyMarketData;

public class DailyMarketDataRequestValidator : AbstractValidator<DailyMarketDataRequest>
{
    public DailyMarketDataRequestValidator()
    {
        RuleFor(x => x.StartDate).LessThanOrEqualTo(x => x.EndDate)
            .WithMessage("Start date must be on or before end date.");
        
        RuleFor(x => x.Symbol).NotEmpty().WithMessage("Symbol is required.");
        
        RuleFor(x => x.Exchange).NotEmpty().WithMessage("Exchange is required.");
    }
}
