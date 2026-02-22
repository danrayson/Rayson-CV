using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Logging;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private const int MaxBodyLength = 4096;

    private static readonly string[] ExcludedPaths =
    {
        "/health",
        "/health/live",
        "/health/ready",
        "/logs"
    };

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (ShouldExcludePath(context.Request.Path.Value))
        {
            await _next(context);
            return;
        }

        var requestBody = await ReadRequestBodyAsync(context.Request);

        if (!string.IsNullOrEmpty(requestBody))
        {
            requestBody = SensitiveDataRedactor.Redact(requestBody);
        }

        _logger.LogDebug(
            "HTTP {Method} {Path}{RequestBody}",
            context.Request.Method,
            context.Request.Path.Value,
            FormatBodyForLog(requestBody));

        await _next(context);
    }

    private static bool ShouldExcludePath(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        return ExcludedPaths.Any(excluded =>
            path.StartsWith(excluded, StringComparison.OrdinalIgnoreCase));
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        if (!IsJsonContentType(request.ContentType))
        {
            return string.Empty;
        }

        request.EnableBuffering();

        try
        {
            request.Body.Seek(0, SeekOrigin.Begin);

            var contentLength = request.ContentLength ?? 0;
            if (contentLength == 0)
            {
                return string.Empty;
            }

            var buffer = new byte[Math.Min(contentLength, MaxBodyLength)];
            var bytesRead = await request.Body.ReadAsync(buffer.AsMemory(0, buffer.Length));
            var body = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            if (contentLength > MaxBodyLength)
            {
                body += "\n[TRUNCATED]";
            }

            return body;
        }
        finally
        {
            request.Body.Seek(0, SeekOrigin.Begin);
        }
    }

    private static bool IsJsonContentType(string? contentType)
    {
        return !string.IsNullOrEmpty(contentType) &&
               contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase);
    }

    private static string FormatBodyForLog(string? body)
    {
        if (string.IsNullOrEmpty(body))
        {
            return string.Empty;
        }

        return $"\n{body}";
    }
}
