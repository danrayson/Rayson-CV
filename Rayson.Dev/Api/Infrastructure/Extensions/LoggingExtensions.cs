using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;

namespace Infrastructure.Extensions;

public static class LoggingExtensions
{
    public static void AddLoggingConfiguration(this WebApplicationBuilder builder)
    {
        var logLevelStr = builder.Configuration["LOG_LEVEL"] ?? "Information";
        var logLevel = Enum.Parse<LogEventLevel>(logLevelStr, ignoreCase: true);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(logLevel)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .CreateBootstrapLogger();

        builder.Host.UseSerilog((context, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithCorrelationIdHeader("X-Correlation-ID")
                .MinimumLevel.Is(logLevel)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");

            var lokiUrl = context.Configuration["LOKI_URL"];
            if (!string.IsNullOrEmpty(lokiUrl))
            {
                loggerConfiguration.WriteTo.GrafanaLoki(lokiUrl);
            }
        });
    }
}
