# ===========================================
# Stage 1: Build
# Uses the full .NET SDK to compile the application
# ===========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

# Set working directory to /src for build operations
WORKDIR /src

# Copy project files first to enable layer caching for NuGet restore
# If csproj files don't change, Docker caches the restore step
COPY ["Presentation/Presentation.csproj", "Presentation/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Database/Database.csproj", "Database/"]

# Restore NuGet packages based on project references
RUN dotnet restore "Presentation/Presentation.csproj"

# Copy all source code from the build context
# This includes everything in the Api directory not in .dockerignore
COPY . .

# Change to the Presentation project directory
WORKDIR "/src/Presentation"

# Build and publish the application
# -c Release: Use Release configuration for optimized output
# -o /app/publish: Output compiled files to /app/publish
# /p:UseAppHost=false: Don't create a native executable (smaller output)
RUN dotnet publish "Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ===========================================
# Stage 2: Runtime
# Uses the smaller ASP.NET runtime image for the final container
# ===========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime

# Install curl for health checks and create a non-root user for security
# --no-cache: Don't cache package index (smaller image)
# adduser: Create 'appuser' with UID 1000, no password (-S), in root group
RUN apk add --no-cache curl && \
    adduser -u 1000 -S appuser -G root

# Set working directory to /app for the runtime
WORKDIR /app

# Copy published artifacts from the build stage
# --from=build: Source from the build stage defined above
COPY --from=build /app/publish .

# Copy wwwroot for static files (PDF CV, desktop apps)
COPY --from=build /src/wwwroot ./wwwroot

# Set environment variable for ASP.NET Core to listen on port 8080
# This overrides the default URL configuration
ENV ASPNETCORE_URLS=http://+:8080

# ===========================================
# Runtime Environment Variables
# ===========================================
# The following environment variables MUST be passed at runtime
# via docker-compose or environment file:
#
# Required:
#   - POSTGRES_HOST       (e.g., postgres)
#   - POSTGRES_PORT       (e.g., 5432)
#   - POSTGRES_DATABASE   (e.g., raysoncv)
#   - POSTGRES_USERNAME   (e.g., postgres)
#   - POSTGRES_PASSWORD   (secure password)
#   - OLLAMA__URL        (e.g., http://ollama:11434)
#
# Optional:
#   - ASPNETCORE_ENVIRONMENT  (Development/Production)
#   - Cors__AllowedOrigins    (comma-separated list of allowed origins)
#   - JWT_SIGNING_KEY         (your secure signing key)
#   - AuthOptions__Issuer     (JWT issuer)
#   - AuthOptions__Audience   (JWT audience)
# ===========================================

EXPOSE 8080

# Define health check to verify the application is running
# --interval=30s: Check every 30 seconds
# --timeout=3s: Wait 3 seconds for response before considering unhealthy
# --start-period=10s: Grace period on container start before checking
# --retries=3: Mark unhealthy after 3 consecutive failures
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health/live || exit 1

# Switch to the non-root user for security
USER appuser

# Set the entry point for the container
# Executes: dotnet Presentation.dll
ENTRYPOINT ["dotnet", "Presentation.dll"]
