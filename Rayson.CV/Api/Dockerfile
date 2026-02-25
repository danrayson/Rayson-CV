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

# Set environment variable for ASP.NET Core to listen on port 8080
# This overrides the default URL configuration
ENV ASPNETCORE_URLS=http://+:8080

ENV AuthOptions__Issuer=danrayson
ENV AuthOptions__Audience=RaysonCVUsers
ENV AuthOptions__IssuerSigningKey=development_signing_key_for_testing_only_not_for_production
ENV POSTGRES_HOST=host.docker.internal
ENV POSTGRES_PORT=5433
ENV POSTGRES_DATABASE=raysoncv
ENV POSTGRES_USERNAME=raysoncv_user
ENV POSTGRES_PASSWORD=development_password
ENV Cors__AllowedOrigins__0=http://localhost:3000

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
