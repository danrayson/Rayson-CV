using Application.Core;

namespace Application.Chatbot;

public interface IChatbotService
{
    Task<ServiceResponse<ChatbotResponse>> GetChatResponseAsync(ChatbotRequest request);
    Task StreamChatResponseAsync(ChatbotRequest request, Func<string, Task> onChunk, CancellationToken cancellationToken);
}
