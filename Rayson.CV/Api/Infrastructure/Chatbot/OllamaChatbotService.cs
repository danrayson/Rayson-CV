using System.Text;
using System.Text.Json;
using Application.Chatbot;
using Application.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Chatbot;

public class OllamaChatbotService(
    IHttpClientFactory httpClientFactory,
    IOptions<OllamaSettings> options,
    ILogger<OllamaChatbotService> logger) : IChatbotService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Ollama");
    private readonly OllamaSettings _settings = options.Value;

    public async Task<ServiceResponse<ChatbotResponse>> GetChatResponseAsync(ChatbotRequest request, ICvProvider cvProvider)
    {
        try
        {
            var cvContent = cvProvider.GetCvContent();
            var systemPrompt = $"""
                You are a helpful assistant answering questions about Daniel Rayson's CV.
                
                Only answer questions related to Daniel's professional background, skills, experience, and education.
                Keep your answers brief and concise, preferably 1-2 sentences maximum.
                If asked about something not in the CV, politely say you don't have that information.
                
                ---
                
                {cvContent}
                """;

            var messages = new List<OllamaMessage>
            {
                new() { Role = "system", Content = systemPrompt }
            };

            if (request.History != null)
            {
                messages.AddRange(request.History.Select(h => new OllamaMessage
                {
                    Role = h.Role,
                    Content = h.Content
                }));
            }

            messages.Add(new OllamaMessage { Role = "user", Content = request.Message });

            var ollamaRequest = new OllamaChatRequest
            {
                Model = "smollm2:135m",
                Messages = messages,
                Stream = false,
                NumPredict = 128
            };

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

    public async Task StreamChatResponseAsync(ChatbotRequest request, ICvProvider cvProvider, Func<string, Task> onChunk, CancellationToken cancellationToken)
    {
        var cvContent = cvProvider.GetCvContent();
        var systemPrompt = $"""
            You are a helpful assistant answering questions about Daniel Rayson's CV.
            
            Only answer questions related to Daniel's professional background, skills, experience, and education.
            Keep your answers brief and concise, preferably 1-2 sentences maximum.
            If asked about something not in the CV, politely say you don't have that information.
            
            ---
            
            {cvContent}
            """;

        var messages = new List<OllamaMessage>
        {
            new() { Role = "system", Content = systemPrompt }
        };

        if (request.History != null)
        {
            messages.AddRange(request.History.Select(h => new OllamaMessage
            {
                Role = h.Role,
                Content = h.Content
            }));
        }

        messages.Add(new OllamaMessage { Role = "user", Content = request.Message });

        var ollamaRequest = new OllamaChatRequest
        {
            Model = "smollm2:135m",
            Messages = messages,
            Stream = true,
            NumPredict = 128
        };

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
}
