using Application.Auth;
using Application.Health;
using Application.Logging;
using Database;
using Infrastructure.Auth;
using Infrastructure.Health;
using Infrastructure.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Database.Auth;

namespace Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IHealthService, HealthService>();
        services.AddScoped<ILoggingService, LoggingService>();
        services.AddDataProtection().PersistKeysToDbContext<RaysonDevDbContext>();
        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<RaysonDevDbContext>()
            .AddDefaultTokenProviders();
    }
}
