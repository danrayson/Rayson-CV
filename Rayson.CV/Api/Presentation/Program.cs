using Microsoft.EntityFrameworkCore;
using Presentation.Extensions;
using Database.SeedData;
using Database.Extensions;
using Presentation.Endpoints.Auth;
using Presentation.Endpoints.Health;
using Presentation.Endpoints.Logging;
using Infrastructure.Auth;
using Infrastructure.Extensions;
using Infrastructure.Logging;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddLoggingConfiguration();

    var authOptionsSectionName = "AuthOptions";
    builder.Services.AddOptions<AuthOptions>().Bind(builder.Configuration.GetSection(authOptionsSectionName));

    builder.Services.AddControllers();
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(corsBuilder =>
        {
            var originsConfig = builder.Configuration["Cors:AllowedOrigins"];
            var allowedOrigins = string.IsNullOrEmpty(originsConfig)
                ? builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>()
                : originsConfig.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            corsBuilder.WithOrigins(allowedOrigins)
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .WithExposedHeaders("X-Auth-Token");
        });
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Rayson CV API", Version = "v1" });
    });

    builder.Services.AddPresentationServices();
    builder.Services.AddDatabaseServices(builder.Configuration);
    builder.Services.AddInfrastructureServices();

    var app = builder.Build();

    app.UseMiddleware<RequestLoggingMiddleware>();

    app.MapAuthEndpoints();
    app.MapHealthEndpoints();
    app.MapLoggingEndpoints();

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

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.UseAuthentication();
    await app.RunMigrations();
    if (!app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
        app.UseHsts();
    }
    app.UseCors();
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
