using Application.Chatbot;
using Application.Health;
using Application.Logging;
using Infrastructure.Chatbot;
using Infrastructure.Health;
using Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructureServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IHealthService, HealthService>();
        services.AddScoped<ILoggingService, LoggingService>();

        services.AddScoped<ICvProvider, CvProvider>();

        services.Configure<OllamaSettings>(configuration.GetSection("Ollama"));
        services.AddHttpClient("Ollama")
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<OllamaSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
                client.Timeout = TimeSpan.FromMinutes(5);
            });
        services.AddScoped<IChatbotService, OllamaChatbotService>();
    }
}
