using Application.Chatbot;
using Application.Core;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Endpoints.Chatbot;

public static class ChatbotEndpoints
{
    public static void MapChatbotEndpoints(this WebApplication webApplication)
    {
        var group = webApplication.MapGroup("chatbot");
        group.MapPost("", GetChatResponse).AllowAnonymous();
    }

    private static async Task<IResult> GetChatResponse(
        [FromServices] IChatbotService chatbotService,
        [FromServices] ICvProvider cvProvider,
        [FromBody] ChatbotRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return ServiceResponse<ChatbotResponse>.Invalid("Message is required").ToHttpResult();
        }

        var response = await chatbotService.GetChatResponseAsync(request, cvProvider);
        return response.ToHttpResult();
    }
}
