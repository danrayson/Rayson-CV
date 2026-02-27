using Application.Core;

namespace Application.Chatbot;

public interface IChatbotService
{
    Task<ServiceResponse<ChatbotResponse>> GetChatResponseAsync(ChatbotRequest request, ICvProvider cvProvider);
    Task StreamChatResponseAsync(ChatbotRequest request, ICvProvider cvProvider, Func<string, Task> onChunk, CancellationToken cancellationToken);
}
