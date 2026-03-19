using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Logging;

public class ApiLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiLoggingMiddleware> _logger;
    private const int MaxBodyLength = 1000;

    private static readonly string[] ExcludedPaths =
    {
        "/health",
        "/health/live",
        "/health/ready",
        "/logs",
        "/chatbot"
    };

    public ApiLoggingMiddleware(RequestDelegate next, ILogger<ApiLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IGeoIpService geoIpService)
    {
        if (ShouldExclude(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var userCorrelationId = context.Request.Headers["X-User-Correlation-Id"].FirstOrDefault() ?? Guid.NewGuid().ToString();
        var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault() ?? Guid.NewGuid().ToString();
        var userIp = GetClientIp(context);
        var userAgent = context.Request.Headers["User-Agent"].FirstOrDefault() ?? "";
        var country = geoIpService.GetCountryCode(userIp);
        var culture = context.Request.Headers["Accept-Language"].FirstOrDefault()?.Split(',').FirstOrDefault();

        context.Items["CorrelationId"] = correlationId;
        context.Items["UserCorrelationId"] = userCorrelationId;
        context.Items["Country"] = country ?? "Unknown";

        var startTime = Stopwatch.GetTimestamp();
        var requestBody = await ReadRequestBodyAsync(context.Request);
        if (!string.IsNullOrEmpty(requestBody))
            requestBody = SensitiveDataRedactor.Redact(requestBody);

        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
        }
        finally
        {
            var elapsedMs = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;
            responseBody.Position = 0;
            var responseBodyContent = await new StreamReader(responseBody).ReadToEndAsync();
            if (responseBodyContent.Length > MaxBodyLength)
                responseBodyContent = responseBodyContent[^MaxBodyLength..] + "\n[TRUNCATED]";
            responseBody.Position = 0;
            await responseBody.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;

            if (!context.Response.HasStarted)
            {
                context.Response.Headers["X-User-Correlation-Id"] = userCorrelationId;
                context.Response.Headers["X-Correlation-Id"] = correlationId;
            }

            _logger.LogInformation(
                "HTTP {Method} {Path} responded {StatusCode} in {DurationMs}ms | " +
                "UserCorrelationId: {UserCorrelationId} | CorrelationId: {CorrelationId} | " +
                "Country: {Country} | IP: {UserIp} | UserAgent: {UserAgent} | Culture: {Culture}",
                context.Request.Method,
                context.Request.Path.Value,
                context.Response.StatusCode,
                elapsedMs,
                userCorrelationId,
                correlationId,
                country ?? "Unknown",
                userIp,
                userAgent,
                culture ?? "Unknown");

            if (!string.IsNullOrEmpty(requestBody) || !string.IsNullOrEmpty(responseBodyContent))
            {
                _logger.LogDebug(
                    "Request: {RequestBody}\nResponse: {ResponseBody}",
                    requestBody ?? "",
                    SensitiveDataRedactor.Redact(responseBodyContent));
            }
        }
    }

    private static bool ShouldExclude(PathString path)
    {
        return ExcludedPaths.Any(excluded =>
            path.StartsWithSegments(excluded, StringComparison.OrdinalIgnoreCase));
    }

    private static string GetClientIp(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
            return forwardedFor.Split(',')[0].Trim();

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
            return realIp;

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        if (!IsJsonContentType(request.ContentType) || request.ContentLength == 0)
            return "";

        request.EnableBuffering();
        request.Body.Position = 0;

        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();

        request.Body.Position = 0;
        return body.Length > MaxBodyLength ? body[^MaxBodyLength..] + "\n[TRUNCATED]" : body;
    }

    private static bool IsJsonContentType(string? contentType)
        => !string.IsNullOrEmpty(contentType) &&
           contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase);
}
