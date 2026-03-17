using Application.Chatbot;
using Application.Core;
using Microsoft.Extensions.Logging;
using Rayson.Ollama;

namespace Infrastructure.Chatbot;

public class OllamaChatbotService(
    IOllamaService ollamaService,
    ICvProvider cvProvider,
    ILogger<OllamaChatbotService> logger) : IChatbotService
{
    private const string Model = "llama3.2:latest";
    private const int MaxTokens = 35;  // ~100 words

    public async Task<ServiceResponse<ChatbotResponse>> GetChatResponseAsync(ChatbotRequest request)
    {
        try
        {
            var messages = await BuildMessagesAsync(request);
            var options = new ChatOptions { NumPredict = MaxTokens };
            var ollamaResponse = await ollamaService.ChatAsync(messages, Model, options);

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
        var options = new ChatOptions { NumPredict = MaxTokens };

        await foreach (var chunk in ollamaService.ChatStreamAsync(messages, Model, options, ct: cancellationToken))
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

        if (request.History != null)
        {
            foreach (var historyMessage in request.History)
            {
                messages.Add(new OllamaMessage
                {
                    Role = historyMessage.Role,
                    Content = historyMessage.Content
                });
            }
        }

        messages.Add(new OllamaMessage { Role = "user", Content = request.Message });

        return messages;
    }

    private Task<string> BuildSystemPromptAsync(string message)
    {
        var cvContent = cvProvider.GetCvContent();

        return Task.FromResult("""
            You are Daniel Rayson's AI-powered professional representative. Your role is to help visitors understand his background, skills, and value as a developer by providing factual, informative answers based on the CV content provided.

            OUTPUT FORMAT:
            - Always use clean, well-formatted markdown
            - Use ## for section headings
            - Use bullet points (-) for lists
            - Use **bold** for emphasis on key terms
            - Use code blocks (```) for technical terms, languages, or tools
            - Use paragraphs for longer explanations
            - NEVER use tables - use bullet points or structured lists instead
            - Keep your response well-structured and easy to read

            TONE AND STYLE:
            - Let the facts speak - present information objectively without hype
            - Be professional, knowledgeable, and helpful
            - Use specific examples from the CV when available
            
            RESPONSE LENGTH:
            - CRITICAL:  Keep responses between 75-125 words
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
            
            CV CONTENT:
            ---
            """ + cvContent);
    }
}
