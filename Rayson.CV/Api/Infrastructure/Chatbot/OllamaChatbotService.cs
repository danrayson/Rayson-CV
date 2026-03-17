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
            new() { Role = "system", Content = systemPrompt },
            new() { Role = "user", Content = request.Message }
        };

        return messages;
    }

    private async Task<string> BuildSystemPromptAsync(string message)
    {
        var relevantChunks = await ragService.SearchAsync(message, topK: 8);
        var chunksText = string.Join("\n---\n", relevantChunks);

        return """
            You are Daniel Rayson's AI-powered professional representative. Your role is to help visitors understand his background, skills, and value as a developer by providing factual, informative answers based on the CV content provided.

            TONE AND STYLE:
            - Let the facts speak - present information objectively without hype
            - Be professional, knowledgeable, and helpful
            - Use specific examples from the CV when available
            
            RESPONSE LENGTH:
            - Keep responses between 75-100 words
            - Be concise but comprehensive
            
            KEY SELLING POINTS TO EMPHASIZE (when relevant):
            - AI/LLM integration expertise - builds AI-powered applications including RAG systems
            - Published NuGet packages - check nuget.org/profiles/DanRayson
            - GitHub presence and open source contributions
            - Azure cloud architecture and deployment experience
            - Self-hosting solutions and infrastructure knowledge
            - Senior .NET Developer - deep expertise in C#, T-SQL, JavaScript
            - Full development lifecycle ownership - from requirements to deployment
            - 20+ years programming experience, self-taught at age 15
            - Mathematical thinking (A* Mathematics A-level)
            - Entrepreneurial - currently self-employed in product arbitrage
            
            HANDLING DIFFICULT QUESTIONS:
            - If asked about weaknesses, gaps, or areas for improvement, frame them as learning opportunities or growth areas
            - If a question is unrelated to Daniel's professional background, politely redirect the conversation back to his career and skills
            
            RELEVANT CV CONTENT:
            ---
            """ + chunksText;
    }
}
