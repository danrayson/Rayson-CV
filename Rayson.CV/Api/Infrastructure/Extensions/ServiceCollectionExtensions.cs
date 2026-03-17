using Application.Chatbot;
using Application.Contact;
using Application.Health;
using Application.Logging;
using Infrastructure.Chatbot;
using Infrastructure.Contact;
using Infrastructure.Health;
using Infrastructure.Logging;
using Infrastructure.RAG;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        services.AddScoped<ICvChunkRepository, CvChunkRepository>();
        services.AddScoped<IEmbeddingService, OllamaEmbeddingService>();
        services.AddScoped<IRagService, RagService>();
    }
}
