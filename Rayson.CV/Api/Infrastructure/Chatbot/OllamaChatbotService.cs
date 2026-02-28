using System.Text;
using System.Text.Json;
using Application.Chatbot;
using Application.Core;
using Infrastructure.RAG;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Chatbot;

public class OllamaChatbotService(
    IHttpClientFactory httpClientFactory,
    IOptions<OllamaSettings> options,
    IRagService ragService,
    ILogger<OllamaChatbotService> logger) : IChatbotService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Ollama");
    private readonly OllamaSettings _settings = options.Value;

    public async Task<ServiceResponse<ChatbotResponse>> GetChatResponseAsync(ChatbotRequest request)
    {
        try
        {
            var messages = await BuildMessagesAsync(request);
            var ollamaRequest = CreateChatRequest(messages, stream: false);

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(ollamaRequest),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("/api/chat", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                return ServiceResponse<ChatbotResponse>.Fail($"Ollama returned status code: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var ollamaResponse = JsonSerializer.Deserialize<OllamaChatResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (ollamaResponse?.Message?.Content == null)
            {
                return ServiceResponse<ChatbotResponse>.Fail("Invalid response from Ollama");
            }

            return ServiceResponse<ChatbotResponse>.Succeed(new ChatbotResponse
            {
                Message = ollamaResponse.Message.Content
            });
        }
        catch (Exception ex)
        {
            return ServiceResponse<ChatbotResponse>.Fail($"Error calling Ollama: {ex.Message}");
        }
    }

    public async Task StreamChatResponseAsync(ChatbotRequest request, Func<string, Task> onChunk, CancellationToken cancellationToken)
    {
        var messages = await BuildMessagesAsync(request);
        var ollamaRequest = CreateChatRequest(messages, stream: true);

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(ollamaRequest),
            Encoding.UTF8,
            "application/json");

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/chat")
        {
            Content = jsonContent
        };

        var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Ollama request failed with status {StatusCode}: {ErrorContent}", response.StatusCode, errorContent);
            throw new Exception($"Ollama returned status code: {response.StatusCode}");
        }

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream, Encoding.UTF8);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            
            if (string.IsNullOrWhiteSpace(line)) continue;

            try
            {
                var chunk = JsonSerializer.Deserialize<OllamaChatResponse>(line, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (chunk?.Done == true)
                {
                    break;
                }

                if (chunk?.Message?.Content != null)
                {
                    await onChunk(chunk.Message.Content);
                }
            }
            catch (JsonException ex)
            {
                logger.LogWarning(ex, "Failed to parse JSON chunk: {Json}", line);
            }
        }
    }

    private async Task<List<OllamaMessage>> BuildMessagesAsync(ChatbotRequest request)
    {
        var systemPrompt = await BuildSystemPromptAsync(request.Message);

        var messages = new List<OllamaMessage>
        {
            new() { Role = "system", Content = systemPrompt }
        };

        messages.Add(new OllamaMessage { Role = "user", Content = request.Message });

        return messages;
    }

    private async Task<string> BuildSystemPromptAsync(string message)
    {
        var relevantChunks = await ragService.SearchAsync(message, topK: 5);
        var chunksText = string.Join("\n---\n", relevantChunks);

        return $"""
            You are a helpful assistant answering questions about Daniel Rayson's CV.
            ---
            Relevant sections from Daniel's CV:
            ---
            {chunksText}
            """;
    }

    private static OllamaChatRequest CreateChatRequest(List<OllamaMessage> messages, bool stream)
    {
        return new OllamaChatRequest
        {
            Model = "smollm2:135m",
            Messages = messages,
            Stream = stream,
            NumPredict = 128
        };
    }
}
