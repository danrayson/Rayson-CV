using Presentation.Extensions;
using Database.SeedData;
using Database.Extensions;
using Presentation.Endpoints.Health;
using Presentation.Endpoints.Logging;
using Presentation.Endpoints.Chatbot;
using Infrastructure.Extensions;
using Infrastructure.Logging;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddLoggingConfiguration();

    builder.Services.AddControllers();
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("frontend", corsBuilder =>
        {
            var originsConfig = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
            if (originsConfig == null || originsConfig.Length == 0) throw new ArgumentException("Cors:AllowedOrigins not set.");
            corsBuilder.WithOrigins(originsConfig)
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials();
        });
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Rayson CV API", Version = "v1" });
    });

    builder.Services.AddPresentationServices();
    builder.Services.AddDatabaseServices(builder.Configuration);
    builder.Services.AddInfrastructureServices(builder.Configuration);

    var app = builder.Build();

    app.UseHttpsRedirection();
    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
    }
    app.UseCors("frontend");

    app.UseMiddleware<RequestLoggingMiddleware>();

    app.MapHealthEndpoints();
    app.MapLoggingEndpoints();
    app.MapChatbotEndpoints();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.EnableTryItOutByDefault();
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Rayson CV API V1");
        });
    }

    await app.RunMigrationsAsync();
    await app.InitializeRagAsync();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
