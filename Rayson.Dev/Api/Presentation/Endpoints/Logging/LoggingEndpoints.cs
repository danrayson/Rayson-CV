using Application.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Endpoints.Logging;

public static class LoggingEndpoints
{
    public static void MapLoggingEndpoints(this WebApplication webApplication)
    {
        var group = webApplication.MapGroup("logs");
        group.MapPost("", LogClientEvent);
    }

    private static async Task<IResult> LogClientEvent(
        [FromServices] ILoggingService loggingService,
        [FromBody] ClientLogEvent logEvent)
    {
        await loggingService.LogClientEventAsync(logEvent);
        return Results.Ok();
    }
}
