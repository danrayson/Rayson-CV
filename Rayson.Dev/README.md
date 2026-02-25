# RaysonDev

A bootstrapping project to create new applications with .NET API backend and React frontend.

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
| `VITE_API_BASE_URL` | API URL for UI (build time) |
| `API_HEALTH_URL` | API health URL for UI health checks |
| `LOCAL_CONNECTION_STRING` | Connection string for local debugging |
| `LOG_LEVEL` | Log level for UI health server |

## Health Check Endpoints

Both API and UI expose health check endpoints for Azure Container Apps probes and monitoring.

### API Endpoints

| Endpoint | Purpose | Checks |
|----------|---------|--------|
| `GET /health` | Full health status | Self + PostgreSQL connectivity |
| `GET /health/live` | Liveness probe | Self only (no dependency checks) |
| `GET /health/ready` | Readiness probe | PostgreSQL connectivity |

**Example Response (GET /health):**
```json
{
  "status": "Healthy",
  "checks": {
    "postgresql": {
      "status": "Healthy",
      "description": null,
      "duration": 42.5
    }
  },
  "totalDuration": 45.2
}
```

### UI Endpoints

| Endpoint | Purpose | Checks |
|----------|---------|--------|
| `GET /health` | Full health status | Self + API connectivity |
| `GET /health/live` | Liveness probe | Self only |
| `GET /health/ready` | Readiness probe | API connectivity |

**Example Response (GET /health):**
```json
{
  "status": "Healthy",
  "checks": {
    "ui": {
      "status": "Healthy",
      "description": "UI server is running"
    },
    "api": {
      "status": "Healthy",
      "statusCode": 200
    }
  },
  "totalDuration": 0
}
```

### Azure Container Apps Configuration

When deploying to Azure Container Apps, configure probes as follows:

```yaml
probes:
  - type: Startup
    httpGet:
      path: /health/live
      port: 8080
    initialDelaySeconds: 10
    periodSeconds: 10
    failureThreshold: 30
  - type: Liveness
    httpGet:
      path: /health/live
      port: 8080
    periodSeconds: 30
  - type: Readiness
    httpGet:
      path: /health/ready
      port: 8080
    periodSeconds: 10
```

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

## Logging

The application uses Serilog for structured logging, writing to the container console.

**Log Enrichment:**
- Environment name
- Machine name
- Thread ID
- Correlation ID (for request tracing)

### Log Sources

| Source | Transport | Notes |
|--------|-----------|-------|
| API | Serilog → Console | Full structured logging |
| UI Health Server | Pino → Console | Captured by Docker logs |
| UI Client | HTTP POST → API `/logs` | Error boundary + manual logging |

### API Log Endpoint

The API exposes an endpoint for client-side logging:

```
POST /logs
Content-Type: application/json

{
  "level": "Error",
  "message": "Something went wrong",
  "source": "UI.ErrorBoundary",
  "browserInfo": "Mozilla/5.0...",
  "stackTrace": "Error: ...",
  "additionalData": { "key": "value" }
}
```

### Viewing Logs

**Docker Logs (UI health server):**
```bash
docker logs raysondev-ui
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
├── Test/
│   └── e2e/               # End-to-end BDD tests
├── .env                    # Local environment variables (gitignored)
├── .env.example            # Environment template
└── docker-compose.*.yml    # Docker Compose configurations
```

## Testing

### E2E Tests (Playwright + Cucumber)

The project includes an end-to-end testing framework using Playwright for browser automation and Cucumber for BDD-style scenarios written in Gherkin syntax.

#### Test Structure

```
Test/e2e/
├── features/              # Gherkin scenario files (.feature)
├── support/               # API client and utilities
│   ├── api-client.ts      # Direct API calls for test data
│   └── browser-utils.ts   # Shared browser helpers
├── steps.js               # Cucumber step definitions
├── cucumber.js            # Cucumber configuration
└── package.json
```

#### Running Tests Locally

1. Install dependencies:
   ```bash
   cd Test/e2e
   npm install
   npx playwright install chromium
   ```

2. Start the full stack:
   ```bash
   docker compose -f docker-compose.dev.full.yml up -d
   ```

3. Run tests:
   ```bash
   npm run e2e:local
   ```

Tests connect to the API at `http://localhost:13245` and UI at `http://localhost:3000` by default. The API health is checked before tests run with automatic retries.

#### Running Tests Against Staging

Tests run automatically in CI after deployment to staging. To run manually:

```bash
cd Test/e2e
npm install
npx playwright install chromium
E2E_API_URL=https://your-staging-api.example.com \
E2E_UI_URL=https://your-staging-ui.example.com \
npm run e2e:staging
```

#### Test User

E2E tests use a static test user that must be seeded in the database:

- **Email**: `testuser@test.com`
- **Password**: `TestPassword123!`

To seed this user, add it to the database seed data or manually create it.

#### Writing New Tests

1. Create a `.feature` file in `Test/e2e/features/`:
   ```gherkin
   Feature: Feature Name
     Scenario: Description
       Given I am on the login page
       When I enter valid credentials
       Then I should see the dashboard
   ```

2. Add step definitions in `Test/e2e/steps.js` (JavaScript, not TypeScript).

3. Run tests to verify:
   ```bash
   npm run e2e:local
   ```

#### Reports

HTML reports are generated in `Test/e2e/reports/cucumber.html` after each test run. Screenshots are captured on failure and saved to `Test/e2e/reports/screenshots/`.

## Deployment

Staging and production deployments use GitHub Actions to deploy containers to Azure Container Apps. Secrets are managed via Azure Key Vault.
