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

#### Full Stack
Run all services in Docker:
```bash
# Start
docker compose -f docker-compose.dev.full.yml up -d

# Stop
docker compose -f docker-compose.dev.full.yml down
```

## Chatbot

The chatbot feature uses Ollama with the llama3.2:latest model to answer questions about Daniel Rayson's CV. It provides a conversational interface for visitors to learn about professional background, skills, and experience.

### Architecture

```
User -> UI (ChatbotPage) -> API (/chatbot endpoint) -> Ollama (llama3.2:latest)
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
   docker compose -f docker-compose.dev.full.yml up -d
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
- **Model**: `llama3.2:latest` (not configurable)
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
- Startup script (`ollama-startup.sh`) that pulls llama3.2:latest model on first run
- Health check confirms llama3.2:latest model is available

**Ports**: 11434 (container), 11435 (host)

## VSCode Debugging

### Debug API Locally

1. Start the services:
   ```bash
   docker compose -f docker-compose.dev.full.yml up -d
   ```

2. In VSCode, select ".NET Core Launch (web)" from the debug dropdown

3. Press F5 to start debugging

The API will be available at `http://localhost:5000` (configured in `launch.json`)

### Debug API in Docker Container

1. Start the services:
   ```bash
   docker compose -f docker-compose.dev.full.yml up -d
   ```

2. In VSCode, select "Docker: .NET Core Debug" from the debug dropdown

3. Press F5 to start debugging

The API will be available at `http://localhost:13245`

### Debug UI Locally

1. Start API:
   ```bash
   docker compose -f docker-compose.dev.full.yml up -d
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

### Remove All Data
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
| `OLLAMA__BASEURL` | Ollama server URL (e.g., `http://ollama:11434` for Docker) |

## Health Check Endpoints

Both API and UI expose health check endpoints for container orchestrator probes and monitoring.

### API Endpoints

| Endpoint | Purpose | Checks |
|----------|---------|--------|
| `GET /health` | Full health status | Self |
| `GET /health/live` | Liveness probe | Self only (no dependency checks) |
| `GET /health/ready` | Readiness probe | Self |

**Example Response (GET /health):**
```json
{
  "status": "Healthy"
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

## CORS Configuration

CORS origins are configuration-driven and whitelist only specified domains.

Set the environment variable:
```
Cors:AllowedOrigins=https://yourdomain.com,https://www.yourdomain.com
```

For multiple origins use comma-separated values.

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
│   │   ├── Infrastructure/     # External services (Auth, Chatbot, Logging)
│   ├── UI/                     # React + Vite frontend
│   ├── Test/
│   │   └── e2e/                # End-to-end BDD tests (Playwright + Cucumber)
│   ├── .env                    # Local environment variables (gitignored)
│   └── docker-compose.*.yml   # Docker Compose configurations
├── .github/workflows/          # CI/CD workflows
└── .vscode/                    # VSCode launch configs and tasks
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

## CI/CD

GitHub Actions workflow at `.github/workflows/deploy-production.yml`:

### Trigger
- Manual workflow dispatch (with optional api_base_url input)

### Jobs
1. **build-electron-linux** - Builds Electron app for Linux (.AppImage)
2. **build-electron-mac** - Builds Electron app for macOS (.dmg)
3. **build-electron-windows** - Builds Electron app for Windows (.exe)

### Usage
1. Go to GitHub Actions → Deploy Production workflow
2. Click "Run workflow" with your desired `api_base_url`
3. Wait for builds to complete
4. Download the artifacts (electron-linux, electron-mac, electron-windows)

## Deployment

### Production Setup
- Docker containers for API and UI

### Environment Configuration
- `ASPNETCORE_ENVIRONMENT=Production` in production
- CORS origins must be explicitly configured
- JWT signing key must be secure (minimum 16 characters)
- Use HTTPS only in production (HSTS enabled)

### Docker Production

To deploy using Docker Compose:

1. **Backup** - Backup environment and docker-compose files:
   ```bash
   cp Api/.env.production ~/.raysoncv-backup/.env.production
   cp docker-compose.prod.full.yml ~/.raysoncv-backup/docker-compose.prod.full.yml
   ```

2. **Pull code** - Pull latest code from main:
   ```bash
   git checkout main && git pull origin main
   ```

3. **Restore** - Restore environment file:
   ```bash
   cp ~/.raysoncv-backup/.env.production Api/.env.production
   ```

4. **Electron** - Build Electron desktop apps and download artifacts:
   - Go to GitHub Actions → Deploy Production workflow
   - Click "Run workflow" with your desired `api_base_url`
   - Wait for build to complete
   - Download the artifacts (electron-linux, electron-mac, electron-windows)
   - Extract the downloaded files into `Api/wwwroot/` in the project

5. **Build images** - Build Docker images:
   ```bash
   docker compose -f docker-compose.prod.full.yml --env-file .env.production build
   ```

6. **Deploy** - Stop old containers, then start new containers (logs will be shown; press `d` to detach):
   ```bash
   docker compose -f ~/.raysoncv-backup/docker-compose.prod.full.yml --env-file ~/.raysoncv-backup/.env.production down && \
   docker compose -f docker-compose.prod.full.yml --env-file .env.production up
   ```

7. **Cleanup** - Clean up unused Docker resources:
   ```bash
   docker system prune
   ```

**Note:** Once `.env.production` contains real sensitive values it should not be committed to version control.  The file in version control contains placeholder values only for guidance.

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
