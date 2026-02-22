# RaysonDev

A financial market data dashboard application with .NET API backend and React frontend.

## Architecture

| Component | Technology | Port |
|-----------|------------|------|
| Database | PostgreSQL 16 | 5433 (host) |
| API | .NET 8.0 | 13245 (host), 8080 (container) |
| UI | React + Vite | 3000 |

## Prerequisites

- Docker & Docker Compose
- .NET 8.0 SDK (for local API debugging)
- Node.js 20+ (for local UI debugging)
- VSCode with Docker extension (for container debugging)

## Quick Start

### 1. Environment Setup

Copy the example environment file and configure:

```bash
cp .env.example .env
# Edit .env with your values
```

### 2. Start Services

Choose your development scenario:

#### Database Only
Run PostgreSQL for local debugging of both API and UI:
```bash
# Start
docker compose -f docker-compose.dev.db.yml up -d

# Stop
docker compose -f docker-compose.dev.db.yml down
```

#### Database + UI
Run PostgreSQL and UI in Docker, debug API locally in VSCode:
```bash
# Start
docker compose -f docker-compose.dev.db-ui.yml up -d

# Stop
docker compose -f docker-compose.dev.db-ui.yml down
```

#### Database + API
Run PostgreSQL and API in Docker, debug UI locally:
```bash
# Start
docker compose -f docker-compose.dev.db-api.yml up -d

# Stop
docker compose -f docker-compose.dev.db-api.yml down
```

#### Full Stack
Run all services in Docker:
```bash
# Start
docker compose -f docker-compose.dev.full.yml up -d

# Stop
docker compose -f docker-compose.dev.full.yml down
```

## VSCode Debugging

### Debug API in Docker Container

1. Start the database:
   ```bash
   docker compose -f docker-compose.dev.db.yml up -d
   # OR
   docker compose -f docker-compose.dev.db-ui.yml up -d
   ```

2. In VSCode, select "Docker: .NET Core Debug" from the debug dropdown

3. Press F5 to start debugging

The API will be available at `http://localhost:13245`

### Debug UI Locally

1. Start database and API:
   ```bash
   docker compose -f docker-compose.dev.db-api.yml up -d
   ```

2. Navigate to UI folder and run:
   ```bash
   cd UI
   npm install
   npm run dev
   ```

## Useful Commands

### View Running Containers
```bash
docker ps
```

### View Container Logs
```bash
docker logs raysondev-api
docker logs raysondev-ui
docker logs raysondev-postgres
```

### Stop All Containers
```bash
docker compose -f docker-compose.dev.full.yml down
```

### Remove All Data (Including Database Volume)
```bash
docker compose -f docker-compose.dev.full.yml down -v
```

### Rebuild Containers
```bash
docker compose -f docker-compose.dev.full.yml up -d --build
```

### Rebuild Without Cache
```bash
docker compose -f docker-compose.dev.full.yml build --no-cache
docker compose -f docker-compose.dev.full.yml up -d
```

## Environment Variables

See `.env.example` for all required variables:

| Variable | Description |
|----------|-------------|
| `POSTGRES_DB` | Database name |
| `POSTGRES_USER` | Database username |
| `POSTGRES_PASSWORD` | Database password |
| `JWT_ISSUER` | JWT token issuer |
| `JWT_AUDIENCE` | JWT token audience |
| `JWT_SIGNING_KEY` | JWT signing key (min 16 chars) |
| `DATA_SEED_PATH` | Path to seed data files |
| `VITE_API_BASE_URL` | API URL for UI (build time) |
| `LOCAL_CONNECTION_STRING` | Connection string for local debugging |

## CORS Configuration

CORS origins are configuration-driven and whitelist only specified domains.

### Local Development

CORS is pre-configured in Docker Compose files and the Dockerfile:
- `http://localhost:3000` (UI running locally or in Docker)

### Azure Deployment

Set the environment variable in Azure Container Apps:

```
Cors:AllowedOrigins=https://your-ui.azurecontainerapps.io,https://yourcustomdomain.com
```

For multiple origins, use comma-separated values or array syntax:
```
Cors:AllowedOrigins__0=https://your-ui.azurecontainerapps.io
Cors:AllowedOrigins__1=https://yourcustomdomain.com
```

## Database Migrations

Migrations are applied automatically on API startup. To create a new migration:

```bash
cd Api
dotnet ef migrations add MigrationName -p Database -s Presentation
```

## Project Structure

```
Rayson.Dev/
├── Api/                    # .NET API backend
│   ├── Presentation/       # API endpoints, Program.cs
│   ├── Application/        # Business logic
│   ├── Domain/             # Entities, interfaces
│   ├── Infrastructure/     # External services
│   └── Database/           # EF Core, migrations, seed data
├── UI/                     # React + Vite frontend
├── .env                    # Local environment variables (gitignored)
├── .env.example            # Environment template
└── docker-compose.*.yml    # Docker Compose configurations
```

## Deployment

Staging and production deployments use GitHub Actions to deploy containers to Azure Container Apps. Secrets are managed via Azure Key Vault.
