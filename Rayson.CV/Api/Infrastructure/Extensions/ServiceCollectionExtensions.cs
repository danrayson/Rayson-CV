using Application.Chatbot;
using Application.Contact;
using Application.Health;
using Application.Logging;
using Infrastructure.Chatbot;
using Infrastructure.Contact;
using Infrastructure.Health;
using Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Rayson.Ollama.Extensions;

namespace Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructureServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddOptions<GraphOptions>().Bind(configuration.GetSection("Graph"));

        services.AddHttpContextAccessor();
        services.AddScoped<IHealthService, HealthService>();
        services.AddScoped<ILoggingService, LoggingService>();
        services.AddScoped<IContactService, GraphEmailService>();

        services.AddScoped<ICvProvider, CvProvider>();

        services.AddOllama();
        services.AddScoped<IChatbotService, OllamaChatbotService>();

        services.AddHealthChecks()
            .AddCheck<OllamaHealthCheck>("ollama", tags: new[] { "ready" });
    }
}
