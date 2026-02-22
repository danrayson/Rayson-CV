using Microsoft.EntityFrameworkCore;
using Presentation.Endpoints.DailyMarketData;
using Presentation.Extensions;
using Database.SeedData;
using Presentation.Exceptions;
using Database.Extensions;
using Presentation.Endpoints.Auth;
using Infrastructure.Auth;
using Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

var csvSettingsSectionName = "DataSeedLocation";
var csvSettings = builder.Configuration.GetSection(csvSettingsSectionName).Get<DataSeedLocationSettings>()
    ?? throw new SettingsMissingException(csvSettingsSectionName);
builder.Services.AddSingleton(csvSettings);

var authOptionsSectionName = "AuthOptions";
builder.Services.AddOptions<AuthOptions>().Bind(builder.Configuration.GetSection(authOptionsSectionName));

// Add services to the container.
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
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "RaysonDev Dashboard API", Version = "v1" });
});

builder.Services.AddPresentationServices();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new SettingsMissingException("DefaultConnection");
builder.Services.AddDatabaseServices(connectionString);
builder.Services.AddInfrastructureServices();

var app = builder.Build();

//Map endpoints
app.MapDailyMarketDataEndpoints();
app.MapAuthEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.EnableTryItOutByDefault();
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "RaysonDev Dashboard API V1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseAuthentication();
app.RunMigrations();
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseHsts();
}
app.UseCors();
app.Run();