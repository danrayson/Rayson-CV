# RaysonCV

A bootstrapping project to create new applications with .NET API backend and React frontend.

## Architecture

| Component | Technology | Port |
|-----------|------------|------|
| API | .NET 8.0 | 13245 (host), 8080 (container) |
| UI | React + Vite | 3000 |
| Ollama | AI Chatbot | 11435 (host), 11434 (container) |

## Prerequisites

- Docker & Docker Compose
- .NET 8.0 SDK (for local API debugging)
- Node.js 20+ (for local UI debugging)
- VSCode with Docker extension (for container debugging)

## Quick Start

### 1. Environment Setup

Configure `.env` with your values:

```bash
# Edit .env with your values
```

### 2. Start Services

Choose your development scenario:

#### UI Only
Run UI + Ollama in Docker, debug API locally in VSCode:
```bash
# Start
docker compose -f docker-compose.dev.ui.yml up -d

# Stop
docker compose -f docker-compose.dev.ui.yml down
```

#### API Only
Run API + Ollama in Docker, debug UI locally:
```bash
# Start
docker compose -f docker-compose.dev.api.yml up -d

# Stop
docker compose -f docker-compose.dev.api.yml down
```

#### Full Stack
Run all services in Docker:
```bash
# Start
docker compose -f docker-compose.dev.full.yml up -d

# Stop
docker compose -f docker-compose.dev.full.yml down
```

## Chatbot

The chatbot feature uses Ollama with the TinyLlama model to answer questions about Daniel Rayson's CV. It provides a conversational interface for visitors to learn about professional background, skills, and experience.

### Architecture

```
User -> UI (ChatbotPage) -> API (/chatbot endpoint) -> Ollama (TinyLlama)
```

### Components

| Layer | Component | Description |
|-------|-----------|-------------|
| UI | `UI/src/pages/ChatbotPage.tsx` | WhatsApp-style chat interface |
| UI | `UI/src/services/chatbotService.ts` | Calls API `/chatbot` endpoint |
| API | `Api/Presentation/Endpoints/Chatbot/ChatbotEndpoints.cs` | Minimal API endpoint |
| API | `Api/Application/Chatbot/` | Request/Response DTOs, service interface |
| API | `Api/Infrastructure/Chatbot/OllamaChatbotService.cs` | Ollama API integration |
| Domain | `Api/Domain/Resources/cv.md` | Embedded CV content |

### Usage

1. Start the API (with Ollama):
   ```bash
   docker compose -f docker-compose.dev.api.yml up -d
   ```

2. Run the UI locally:
   ```bash
   cd Rayson.CV/UI
   npm run dev
   ```

3. Navigate to the chatbot page and ask questions about the CV.

### API Endpoint

- **URL**: `POST /chatbot`
- **Auth**: Anonymous (`.AllowAnonymous()`)
- **Request**:
  ```json
  {
    "message": "What is Rayson's primary skill?",
    "history": [
      { "role": "user", "content": "Hi" },
      { "role": "assistant", "content": "Hello! How can I help?" }
    ]
  }
  ```
- **Response**:
  ```json
  {
    "message": "Daniel's primary skill is..."
  }
  ```

### Configuration

The chatbot service is configured via environment variables:

| Variable | Description | Default |
|----------|-------------|---------|
| `OLLAMA__BASEURL` | Ollama server URL | `http://ollama:11434` |

The following are hardcoded in the service:
- **Model**: `tinyllama` (not configurable)
- **Max tokens**: 128 (limits response length)

### System Prompt

The chatbot uses a system prompt that includes:
- Instructions to answer only CV-related questions
- Instructions to keep answers brief (1-2 sentences)
- Instructions to politely decline off-topic questions
- The CV content from `Api/Domain/Resources/cv.md`

### Docker Setup

Ollama runs in its own container with:
- Custom Dockerfile (`ollama.Dockerfile`) based on `ollama/ollama:latest`
- Startup script (`ollama-startup.sh`) that pulls TinyLlama model on first run
- Health check confirms TinyLlama model is available

**Ports**: 11434 (container), 11435 (host)

## VSCode Debugging

### Debug API Locally

1. Start Ollama:
   ```bash
   docker compose -f docker-compose.dev.api.yml up -d
   ```

2. In VSCode, select ".NET Core Launch (web)" from the debug dropdown

3. Press F5 to start debugging

The API will be available at `http://localhost:5000` (configured in `launch.json`)

### Debug API in Docker Container

1. Start Ollama:
   ```bash
   docker compose -f docker-compose.dev.api.yml up -d
   ```

2. In VSCode, select "Docker: .NET Core Debug" from the debug dropdown

3. Press F5 to start debugging

The API will be available at `http://localhost:13245`

### Debug UI Locally

1. Start API and Ollama:
   ```bash
   docker compose -f docker-compose.dev.api.yml up -d
   ```

2. Navigate to UI folder and run:
   ```bash
   cd Rayson.CV/UI
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
docker logs raysoncv-api
docker logs raysoncv-ui
docker logs raysoncv-ollama
```

### Stop All Containers
```bash
docker compose -f docker-compose.dev.full.yml down
```

### Remove All Data (Including Ollama Volume)
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

The following environment variables are required:

| Variable | Description |
|----------|-------------|
| `JWT_ISSUER` | JWT token issuer |
| `JWT_AUDIENCE` | JWT token audience |
| `JWT_SIGNING_KEY` | JWT signing key (min 16 chars) |
| `VITE_API_BASE_URL` | API URL for UI (build time) |
| `API_HEALTH_URL` | API health URL for UI health checks |
| `LOG_LEVEL` | Log level for UI health server |
| `OLLAMA__BASEURL` | Ollama server URL (e.g., `http://ollama:11434` for Docker, `http://ca-ollama-staging.internal.<domain>:11434` for Azure) |

## Health Check Endpoints

Both API and UI expose health check endpoints for Azure Container Apps probes and monitoring.

### API Endpoints

| Endpoint | Purpose | Checks |
|----------|---------|--------|
| `GET /health` | Full health status | Self only |
| `GET /health/live` | Liveness probe | Self only (no dependency checks) |
| `GET /health/ready` | Readiness probe | Self only |

**Example Response (GET /health):**
```json
{
  "status": "Healthy",
  "checks": {},
  "totalDuration": 0.5
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
docker logs raysoncv-ui
```

## Project Structure

```
Rayson.CV/                      # Root solution folder
├── Rayson.CV/                  # Main project folder
│   ├── Api/                    # .NET API backend
│   │   ├── Presentation/       # Minimal API endpoints, Program.cs
│   │   ├── Application/        # Business logic, interfaces, DTOs
│   │   ├── Domain/             # Entities, interfaces
│   │   └── Infrastructure/     # External services (Chatbot, Logging)
│   ├── UI/                     # React + Vite frontend
│   ├── Test/
│   │   └── e2e/               # End-to-end BDD tests (Playwright + Cucumber)
│   ├── .env                    # Local environment variables (gitignored)
│   └── docker-compose.*.yml   # Docker Compose configurations
├── infra/                      # Azure Bicep infrastructure templates
│   ├── main-core.bicep         # Core resources (RG, ACR, Storage, Container Apps Env)
│   ├── main-apps.bicep        # Workload resources (API, UI)
│   └── modules/                # Reusable Bicep modules
├── .github/workflows/          # CI/CD workflows
└── .vscode/                   # VSCode launch configs and tasks
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

## Azure Infrastructure

Infrastructure is defined in Bicep and located at project root `/infra/`:

### Main Templates
- `infra/main-core.bicep` - Creates subscription-level resources: Resource Group, Container Registry, Storage Account, Container Apps Environment
- `infra/main-apps.bicep` - Creates workload resources: API Container App, UI Container App

### Modules
- `infra/modules/api-container.bicep` - Azure Container App for .NET API (port 8080, 0.5 CPU, 1Gi memory, 1-3 replicas)
- `infra/modules/ui-container.bicep` - Azure Container App for React UI (port 3000)
- `infra/modules/container-apps-environment.bicep` - Container Apps Environment
- `infra/modules/storage.bicep` - Azure Storage Account
- `infra/modules/container-registry.bicep` - Azure Container Registry

### Azure Services
- **Azure Container Apps** - Hosts API and UI containers
- **Azure Container Registry** - Stores Docker images
- **Azure Storage Account** - General purpose storage + Ollama model persistence

## CI/CD

GitHub Actions workflow at `.github/workflows/deploy-staging.yml`:

### Trigger
- Push to `develop` branch
- Manual workflow dispatch (with optional imageTag input)

### Jobs
1. **build** - Compiles .NET API and builds React UI
2. **deploy-core** - Deploys core Azure infrastructure (Resource Group, Container Registry, Storage, Container Apps Environment)
3. **push** - Builds and pushes Docker images to Azure Container Registry
4. **deploy-apps** - Deploys container apps (API, UI) via Bicep
5. **build-electron-*** - Builds Electron apps for Linux, macOS, Windows
6. **upload-electron** - Uploads Electron apps to Azure Blob Storage
7. **e2e-tests** - Runs E2E tests against staging

### Environment
- Staging (Azure `uksouth` region, resource group `rg-raysoncv-staging`)

### Secrets Required
- `AZURE_CREDENTIALS` - Azure service principal credentials
- `JWT_SIGNING_KEY` - JWT token signing key

### Deploying

Deployments are triggered via:
- **Pull Request**: Merge to `develop` branch via GitHub PR
- **Manual Trigger**: Run workflow from GitHub Actions UI with optional imageTag

## Deployment

### Production Setup
- Docker containers for API and UI
- Azure Container Apps for orchestration

### Environment Configuration
- `ASPNETCORE_ENVIRONMENT=Production` in production
- CORS origins must be explicitly configured
- JWT signing key must be secure (minimum 16 characters)
- Use HTTPS only in production (HSTS enabled)

### Monitoring
- **Development**: Container console logs
- Health endpoints (`/health/live`, `/health/ready`) for container orchestrator
- Serilog with enrichment: correlation ID, environment, thread info

### Conventions
- Use **HTTPS only** in production
- Use **environment-specific** configuration
- Enable **HSTS** in production
- Use **secure JWT keys** (minimum 256-bit)
- Configure **CORS** explicitly (not allow all)
- Use **health checks** for container orchestration
- Set **LOG_LEVEL=Information** or lower in production
