using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Events;

namespace Infrastructure.Extensions;

public static class LoggingExtensions
{
    public static void AddLoggingConfiguration(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
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
                .Enrich.WithCorrelationId()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");

            var seqUrl = context.Configuration["SEQ_URL"];
            if (!string.IsNullOrEmpty(seqUrl))
            {
                loggerConfiguration.WriteTo.Seq(seqUrl);
            }

            var appInsightsConnStr = context.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
            if (!string.IsNullOrEmpty(appInsightsConnStr))
            {
                loggerConfiguration.WriteTo.ApplicationInsights(
                    connectionString: appInsightsConnStr,
                    telemetryConverter: new Serilog.Sinks.ApplicationInsights.TelemetryConverters.TraceTelemetryConverter());
            }
        });
    }
}
