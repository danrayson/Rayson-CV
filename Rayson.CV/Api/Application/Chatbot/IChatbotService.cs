using Application.Core;

namespace Application.Chatbot;

public interface IChatbotService
{
    Task<ServiceResponse<ChatbotResponse>> GetChatResponseAsync(ChatbotRequest request, ICvProvider cvProvider);
}
