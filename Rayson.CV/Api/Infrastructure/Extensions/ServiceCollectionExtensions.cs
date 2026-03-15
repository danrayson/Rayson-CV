using Application.Chatbot;
using Application.Health;
using Application.Logging;
using Infrastructure.Chatbot;
using Infrastructure.Health;
using Infrastructure.Logging;
using Infrastructure.RAG;
using Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rayson.Ollama.Extensions;

namespace Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructureServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IHealthService, HealthService>();
        services.AddScoped<ILoggingService, LoggingService>();

        services.Configure<BlobSettings>(configuration.GetSection("BlobStorage"));
        services.AddHttpClient("BlobStorage");

        services.AddScoped<ICvProvider, CvProvider>();

        services.AddOllama();
        services.AddScoped<IChatbotService, OllamaChatbotService>();

        services.AddScoped<ICvChunkRepository, CvChunkRepository>();
        services.AddScoped<IEmbeddingService, OllamaEmbeddingService>();
        services.AddScoped<IRagService, RagService>();
    }
}
