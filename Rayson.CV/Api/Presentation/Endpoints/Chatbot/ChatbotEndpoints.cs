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
        group.MapPost("/stream", StreamChatResponse).AllowAnonymous();
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

    private static async Task StreamChatResponse(
        [FromServices] IChatbotService chatbotService,
        [FromServices] ICvProvider cvProvider,
        [FromBody] ChatbotRequest request,
        HttpContext httpContext,
        [FromServices] ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("ChatbotEndpoints.StreamChatResponse");

        if (string.IsNullOrWhiteSpace(request.Message))
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("Message is required");
            return;
        }

        httpContext.Response.ContentType = "text/event-stream";
        httpContext.Response.Headers.CacheControl = "no-cache";
        httpContext.Response.Headers.Connection = "keep-alive";

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(httpContext.RequestAborted);

        try
        {
            await chatbotService.StreamChatResponseAsync(
                request,
                cvProvider,
                async chunk =>
                {
                    var data = $"data: {System.Text.Json.JsonSerializer.Serialize(new { message = new { content = chunk } })}\n\n";
                    await httpContext.Response.WriteAsync(data, cts.Token);
                    await httpContext.Response.Body.FlushAsync(cts.Token);
                },
                cts.Token);

            await httpContext.Response.WriteAsync("data: [DONE]\n\n", cts.Token);
            await httpContext.Response.Body.FlushAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Streaming cancelled - client disconnected");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during streaming: {Message}", ex.Message);
            var error = $"data: {System.Text.Json.JsonSerializer.Serialize(new { error = ex.Message })}\n\n";
            await httpContext.Response.WriteAsync(error, cts.Token);
            await httpContext.Response.Body.FlushAsync(cts.Token);
        }
    }
}
