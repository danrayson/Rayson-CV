using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Events;

namespace Infrastructure.Extensions;

public static class LoggingExtensions
{
    public static void AddLoggingConfiguration(this WebApplicationBuilder builder)
    {
        var logLevelStr = builder.Configuration["LOG_LEVEL"] ?? "Information";
        var logLevel = Enum.Parse<LogEventLevel>(logLevelStr, ignoreCase: true);
        var seqUrl = builder.Configuration["Serilog:SeqUrl"] ?? "http://localhost:5341";
        var seqApiKey = builder.Configuration["Serilog:ApiKey"];

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(logLevel)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Extensions.Diagnostics.HealthChecks", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.HttpMessageHandler", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithProperty("Application", "RaysonCV")
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Seq(seqUrl, apiKey: seqApiKey)
            .CreateBootstrapLogger();

        builder.Host.UseSerilog((context, _, loggerConfiguration) =>
        {
            loggerConfiguration
                .MinimumLevel.Is(logLevel)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Extensions.Diagnostics.HealthChecks", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.HttpMessageHandler", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithProperty("Application", "RaysonCV")
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.Seq(seqUrl, apiKey: seqApiKey);
        });
    }
}
