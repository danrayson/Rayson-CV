using Application.Chatbot;
using Application.Core;
using Infrastructure.RAG;
using Microsoft.Extensions.Logging;
using Rayson.Ollama;

namespace Infrastructure.Chatbot;

public class OllamaChatbotService(
    IOllamaService ollamaService,
    IRagService ragService,
    ILogger<OllamaChatbotService> logger) : IChatbotService
{
    private const string Model = "llama3.2:latest";

    public async Task<ServiceResponse<ChatbotResponse>> GetChatResponseAsync(ChatbotRequest request)
    {
        try
        {
            var messages = await BuildMessagesAsync(request);
            var ollamaResponse = await ollamaService.ChatAsync(messages, Model);

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

        await foreach (var chunk in ollamaService.ChatStreamAsync(messages, Model, ct: cancellationToken))
        {
            if (chunk.Done)
            {
                break;
            }

            if (chunk.Message?.Content != null)
            {
                await onChunk(chunk.Message.Content);
            }
        }
    }

    private async Task<IList<OllamaMessage>> BuildMessagesAsync(ChatbotRequest request)
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
}
