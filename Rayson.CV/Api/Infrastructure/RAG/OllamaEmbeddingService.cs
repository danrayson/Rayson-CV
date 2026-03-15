using Microsoft.Extensions.Logging;
using Rayson.Ollama;

namespace Infrastructure.RAG;

public class OllamaEmbeddingService(
    IOllamaService ollamaService,
    ILogger<OllamaEmbeddingService> logger) : IEmbeddingService
{
    private const string EmbeddingModel = "nomic-embed-text:latest";

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        var response = await ollamaService.EmbedAsync(text, EmbeddingModel);

        if (response?.Embedding == null)
        {
            throw new Exception("Invalid response from Ollama embedding endpoint");
        }

        return response.Embedding.Select(x => (float)x).ToArray();
    }

    public async Task EnsureModelAvailableAsync()
    {
        logger.LogInformation("Checking if embedding model {Model} is available", EmbeddingModel);

        var models = await ollamaService.ListModelsAsync();
        var modelExists = models.Any(m => m.Name == EmbeddingModel);

        if (!modelExists)
        {
            logger.LogInformation("Pulling embedding model {Model}...", EmbeddingModel);
            await ollamaService.PullModelAsync(EmbeddingModel);
        }
        else
        {
            logger.LogInformation("Embedding model {Model} is already available", EmbeddingModel);
        }
    }
}
