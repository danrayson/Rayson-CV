using Application.MarketData;
using FluentValidation;
using Microsoft.AspNetCore.Identity.Data;
using Presentation.Endpoints.Auth;
using Presentation.Endpoints.DailyMarketData;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPresentationServices(this IServiceCollection services)
    {
        services.AddScoped<IValidator<DailyMarketDataRequest>, DailyMarketDataRequestValidator>();
        services.AddScoped<IValidator<SignUpEmailPasswordRequest>, SignUpEmailPasswordRequestValidator>();
        services.AddScoped<IValidator<SignInEmailPasswordRequest>, SignInEmailPasswordRequestValidator>();
        services.AddScoped<IValidator<ChangePasswordRequest>, ChangePasswordRequestValidator>();
        services.AddScoped<IValidator<RequestPasswordResetRequest>, RequestPasswordResetRequestValidator>();
        services.AddScoped<IMarketDataService, MarketDataService>();
        services.AddAutoMapper(typeof(Program));
        services.AddFluentValidationAutoValidation();
    }
}
