using System.Security.Claims;
using Application.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Endpoints.Logging;

public static class LoggingEndpoints
{
    public static void MapLoggingEndpoints(this WebApplication webApplication)
    {
        var group = webApplication.MapGroup("logs");
        group.MapPost("", LogClientEvent).AllowAnonymous();
    }

    private static async Task<IResult> LogClientEvent(
        [FromServices] ILoggingService loggingService,
        HttpContext httpContext,
        [FromBody] ClientLogEvent logEvent)
    {
        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await loggingService.LogClientEventAsync(logEvent, userId);
        return Results.Ok();
    }
}
