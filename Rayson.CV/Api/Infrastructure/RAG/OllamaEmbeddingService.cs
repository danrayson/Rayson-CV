using System.Text;
using System.Text.Json;
using Infrastructure.Chatbot;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.RAG;

public class OllamaEmbeddingService(
    IHttpClientFactory httpClientFactory,
    IOptions<OllamaSettings> options,
    ILogger<OllamaEmbeddingService> logger) : IEmbeddingService
{
    private const string EmbeddingModel = "nomic-embed-text:latest";
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Ollama");
    private readonly OllamaSettings _settings = options.Value;

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        var request = new OllamaEmbeddingRequest
        {
            Model = EmbeddingModel,
            Prompt = text
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync("/api/embeddings", jsonContent);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            logger.LogError("Ollama embedding request failed with status {StatusCode}: {ErrorContent}", response.StatusCode, errorContent);
            throw new Exception($"Ollama embedding failed with status code: {response.StatusCode}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var ollamaResponse = JsonSerializer.Deserialize<OllamaEmbeddingResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (ollamaResponse?.Embedding == null)
        {
            throw new Exception("Invalid response from Ollama embedding endpoint");
        }

        return ollamaResponse.Embedding;
    }

    public async Task EnsureModelAvailableAsync()
    {
        logger.LogInformation("Checking if embedding model {Model} is available", EmbeddingModel);

        var response = await _httpClient.GetAsync("/api/tags");

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("Failed to get models from Ollama: {StatusCode}", response.StatusCode);
            return;
        }

        var content = await response.Content.ReadAsStringAsync();
        var tagsResponse = JsonSerializer.Deserialize<OllamaModelsResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var modelExists = tagsResponse?.Models?.Any(m => m.Name == EmbeddingModel) ?? false;

        if (!modelExists)
        {
            logger.LogInformation("Pulling embedding model {Model}...", EmbeddingModel);
            await PullModelAsync(EmbeddingModel);
        }
        else
        {
            logger.LogInformation("Embedding model {Model} is already available", EmbeddingModel);
        }
    }

    private async Task PullModelAsync(string model)
    {
        var request = new { name = model };
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        var pullRequest = new HttpRequestMessage(HttpMethod.Post, "/api/pull")
        {
            Content = jsonContent
        };

        var response = await _httpClient.SendAsync(pullRequest, HttpCompletionOption.ResponseHeadersRead);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to pull model {model}: {response.StatusCode} - {errorContent}");
        }

        logger.LogInformation("Successfully pulled embedding model {Model}", model);
    }

    private class OllamaEmbeddingRequest
    {
        public required string Model { get; set; }
        public required string Prompt { get; set; }
    }

    private class OllamaEmbeddingResponse
    {
        public float[]? Embedding { get; set; }
    }

    private class OllamaModelsResponse
    {
        public List<OllamaModel>? Models { get; set; }
    }

    private class OllamaModel
    {
        public string? Name { get; set; }
    }
}
