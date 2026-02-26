# AI-Generated Documentation

This file contains AI-generated documentation for the RaysonCV project. Content is organized by component. This documentation is designed to guide AI assistants in understanding the codebase, its patterns, and conventions.

---

## Project Overview

RaysonCV is a bootstrapping project to create new applications with:
- **Frontend**: React + Vite SPA with Tailwind CSS and DaisyUI
- **Backend**: .NET 8.0 Web API with clean architecture
- **Database**: PostgreSQL 16 with Entity Framework Core
- **Authentication**: JWT-based token authentication with email/password
- **Logging**: Serilog writing to container console
- **Containerization**: Docker with multi-stage builds

### Architecture Pattern
The API follows **Clean Architecture** with these layers:
- **Presentation**: Minimal API endpoints, request/response models
- **Application**: Business logic, service interfaces
- **Domain**: Entities, domain interfaces
- **Infrastructure**: External services (Auth, Email, Logging, Database)

---

## UI

### Technology Stack
- React 18.3 + Vite 5.4
- TypeScript 5.5
- Tailwind CSS 3.4 + DaisyUI 4.12
- React Router DOM 6.27
- Axios for HTTP requests
- Highcharts for data visualization
- ESLint with TypeScript support

### Key Components
- **Pages**: `LandingPage.tsx`, `Basic.tsx` (in `/src/pages/`)
- **Components**: `Login.tsx`, `SignUp.tsx`, `ForgottenPassword.tsx`, `ResetPassword.tsx`, `ErrorBoundary.tsx` (in `/src/components/`)
- **Elements**: `FormRow.tsx`, `ValidationMessages.tsx` (in `/src/elements/`)
- **Services**: `httpClient.ts`, `loggingService.ts` (in `/src/services/`)
- **Routing**: Uses `HashRouter` (not BrowserRouter) - important for Electron compatibility

### State Management
- Currently uses **local component state** with `useState` and `useEffect`
- No external state management library (Redux, Zustand, etc.) currently in use
- Token stored in `localStorage` under key `x-auth-token`

### API Integration
All API calls go through `httpClient.ts` which:
- Uses Axios instance with base URL from `config.ts`
- Automatically adds `Authorization: Bearer <token>` header from localStorage
- Has response interceptor that logs API errors via `loggingService`
- Provides typed methods: `get()`, `post()`, `put()`, `delete()`

### Conventions

**To Follow:**
- Use functional components with TypeScript
- Use `.tsx` extension for components render JSX
- that Use `.ts` extension for utility/types only
- Use `React.FC` type for component typing
- Use `import.meta.env.VITE_*` prefix for environment variables
- Use DaisyUI component classes for styling
- Use Tailwind utility classes for custom styling
- Define API response types in `/src/types/` or inline
- Use `HashRouter` for routing (not BrowserRouter)

**To Avoid:**
- Do NOT use class components
- Do NOT use JavaScript (`.js`/`.jsx`) - use TypeScript only
- Do NOT hardcode API URLs - use `config.ts` and environment variables
- Do NOT put secrets in UI code - API handles authentication
- Do NOT use CSS modules or styled-components - use Tailwind
- Do NOT use fetch - use the `httpClient` service

---

## API

### Technology Stack
- .NET 8.0 ASP.NET Core
- Entity Framework Core 8.0
- Minimal API endpoints (not MVC controllers)
- FluentValidation for request validation
- Swashbuckle/Swagger for API documentation
- JWT Bearer authentication
- Serilog for structured logging

### Project Structure
```
Api/
├── Application/       # Business logic, interfaces, DTOs
│   ├── Auth/
│   ├── Core/
│   ├── Health/
│   └── Logging/
├── Database/          # EF Core context, migrations, repositories
│   ├── Auth/          # Identity entities
│   ├── Migrations/
│   └── SeedData/
├── Domain/            # Entities, domain interfaces
├── Infrastructure/    # External services implementation
│   ├── Auth/         # TokenService, AuthService
│   ├── Logging/
│   └── Health/
└── Presentation/      # Minimal API endpoints
    ├── Endpoints/
    └── Extensions/
```

### Endpoints
- **Auth** (`/auth`):
  - `POST /auth/signup` - Register with email/password
  - `POST /auth/signin` - Login, returns token in `X-Auth-Token` header
  - `POST /auth/request-password-reset` - Send password reset email
  - `PUT /auth/reset-password` - Reset password with token
- **Health** (`/health`):
  - `GET /health/live` - Liveness probe
  - `GET /health/ready` - Readiness probe
- **Logging** (`/logging`):
  - `POST /logging/client` - Receive client-side logs

### Authentication
- JWT tokens with 12-hour expiration
- Token passed via `X-Auth-Token` header (also exposed in response header)
- Claims include: `sub` (user ID), `name`, `email`, `role`
- Uses ASP.NET Core Identity with PostgreSQL store

### Database Integration
- Entity Framework Core with Npgsql (PostgreSQL provider)
- Repository pattern with generic `Repository<T>` class
- Soft delete via `DeletedAt` field on `Entity` base class
- Identity tables for user/role management

### Conventions

**To Follow:**
- Use **Minimal API** pattern (`.MapPost()`, `.MapGet()`) - NOT MVC controllers
- Use **FluentValidation** with `[FluentValidation.Attributes]` and `AddFluentValidationAutoValidation()`
- Use **ServiceResponse<T>** wrapper for all responses (never return raw objects)
- Use **ServiceResponseCodes** enum for status codes
- Return `IResult` from endpoint methods
- Follow the clean architecture folder structure
- Use **primary constructors** for classes (C# 12 feature)
- Register services in appropriate extension methods in `ServiceCollectionExtensions.cs`
- Use **primary constructor injection** for DI
- Use async/await for all I/O operations
- **Interfaces** in Application/Domain layers, **implementations** in Infrastructure layer
- **Request models** in Presentation layer (e.g., `SignUpEmailPasswordRequest`)
- **Validators** co-located with endpoints (e.g., `SignUpEmailPasswordRequestValidator`)
- Use `required` keyword for required request properties
- Use `IOptions<T>` pattern for configuration classes
- Use `Domain.Entity` base class with `Id` and `DeletedAt` for soft deletes
- Use `IRepository<T>` interface in Domain, implementation in Database layer

**To Avoid:**
- Do NOT use MVC controllers (`[ApiController]` + `ControllerBase`)
- Do NOT return `void` or raw types from endpoints - always use `IResult`
- Do NOT use `IActionResult` - use `IResult` with `Results.Ok()`, `Results.Created()`, etc.
- Do NOT put business logic in endpoints - delegate to Application layer services
- Do NOT use synchronous database calls
- Do NOT return exceptions directly - convert to `ServiceResponse` with error codes
- Do NOT add external dependencies (NuGet packages) in Application or Domain layers - only in Infrastructure

---

## Chatbot

### Overview
The chatbot feature uses Ollama with the TinyLlama model to answer questions about Daniel Rayson's CV. It provides a conversational interface for visitors to learn about professional background, skills, and experience.

### Architecture

```
User -> UI (ChatbotPage) -> API (/chatbot endpoint) -> Ollama (TinyLlama)
```

### Components

1. **UI**: `UI/src/pages/ChatbotPage.tsx`
   - WhatsApp-style chat interface
   - Message history maintained in component state
   - DaisyUI dark theme styling

2. **UI Service**: `UI/src/services/chatbotService.ts`
   - Calls API `/chatbot` endpoint
   - Sends message + conversation history

3. **API Endpoint**: `Api/Presentation/Endpoints/Chatbot/ChatbotEndpoints.cs`
   - `POST /chatbot` - anonymous endpoint
   - Accepts: `{ message: string, history?: { role: string, content: string }[] }`
   - Returns: `{ message: string }`

4. **Application Layer**: `Api/Application/Chatbot/`
   - `IChatbotService` interface
   - `ChatbotRequest`, `ChatbotResponse` DTOs
   - `ICvProvider` interface (contract for CV content)

5. **Infrastructure Layer**: `Api/Infrastructure/Chatbot/`
   - `OllamaChatbotService` - calls Ollama API
   - `CvProvider` - reads CV content from embedded resource
   - `OllamaSettings`, `OllamaChatRequest`, `OllamaMessage`, `OllamaChatResponse`

6. **Domain Layer**: `Api/Domain/Resources/cv.md`
   - Embedded resource containing CV content
   - Copied to output as embedded resource

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
- The CV content

This is constructed in `OllamaChatbotService.GetChatResponseAsync()`.

### Docker Setup

**Custom Dockerfile**: `ollama.Dockerfile`
- Based on `ollama/ollama:latest`
- Installs curl for health checks
- Uses startup script to pull TinyLlama model on first run

**Startup Script**: `ollama-startup.sh`
- Starts Ollama server
- Waits for server to be ready
- Pulls TinyLlama model (if not already present)
- Keeps container running

**Docker Compose Files**:
- `docker-compose.dev.db-api.yml` - Includes Ollama
- `docker-compose.dev.full.yml` - Includes Ollama
- Healthcheck confirms TinyLlama model is available before marking container healthy

### Azure Bicep

**Modules**:
- `infra/modules/ollama-container.bicep` - Deploys Ollama container app
- `infra/modules/api-container.bicep` - Updated with `OLLAMA__BASEURL` env var

**Internal Communication**:
- API connects to Ollama via internal FQDN: `http://ca-ollama-{environment}.internal.{domain}:11434`
- No service binding used (Azure Container Apps service bindings don't work for container-to-container)

### Conventions

**To Follow:**
- Use `ICvProvider` in Application layer for CV content contract
- Use `CvProvider` in Infrastructure layer for implementation
- Embed CV data in Domain layer as resource
- Use `ServiceResponse<T>` wrapper for API responses
- Use Minimal API pattern for endpoints
- Keep environment variables for configuration
- Hardcode model name (only one model is used)

**To Avoid:**
- Do NOT make model configurable - always use TinyLlama
- Do NOT store CV in database - use embedded resource
- Do NOT use service bindings for container apps - use internal DNS

---

## Database

### Technology Stack
- PostgreSQL 16 (Alpine image for smaller footprint)
- Entity Framework Core 8.0
- Npgsql as database provider

### Schema
- Uses ASP.NET Core Identity schema (Users, Roles, RoleClaims, UserRoles, UserClaims, UserLogins, UserTokens)
- Custom `ApplicationUser` and `ApplicationRole` extending Identity base classes
- All entities inherit from `Domain.Entity` base class with `Id` and `DeletedAt` fields

### Migrations
- Located in `Api/Database/Migrations/`
- Initial migration: `20260222125915_Init.cs`
- Run automatically on startup via `app.RunMigrations()` extension

### Conventions

**To Follow:**
- Use **soft deletes** (set `DeletedAt` field) instead of hard deletes
- Use **EF Core migrations** for schema changes
- Use **repository pattern** for data access (via `Repository<T>`)
- Follow naming: `PascalCase` for tables and columns
- Use **async** methods for all database operations

**To Avoid:**
- Do NOT use hard deletes - always use soft delete
- Do NOT write raw SQL queries unless absolutely necessary
- Do NOT modify migrations manually after creation
- Do NOT create stored procedures for simple CRUD - use EF Core

---

## Docker Setup

### Docker Compose Files
- `docker-compose.dev.db.yml` - PostgreSQL only
- `docker-compose.dev.db-ui.yml` - PostgreSQL + UI
- `docker-compose.dev.db-api.yml` - PostgreSQL + API + Ollama
- `docker-compose.dev.full.yml` - Full stack (PostgreSQL, API, UI, Ollama)

### Services
1. **postgres**: PostgreSQL 16 Alpine
   - Port: 5432 (internal), 5433 (host)
   - Health check: `pg_isready`
   - Volume: `postgres-data`

2. **api**: .NET 8.0 ASP.NET Core
   - Port: 8080 (container), 13245 (host)
   - Health check: `http://localhost:8080/health/live`
   - Multi-stage build (SDK + Runtime)

3. **ollama**: Ollama AI server
   - Port: 11434 (container), 11435 (host)
   - Uses custom Dockerfile (`ollama.Dockerfile`) with curl + startup script
   - Health check: `curl -s http://localhost:11434/api/tags | grep -q tinyllama`
   - Volume: `ollama-data` (persists downloaded models)
   - Custom entrypoint: `ollama-startup.sh` (pulls TinyLlama model on first run)

4. **ui**: Node.js with health server
   - Port: 3000
   - Health check: `http://localhost:3000/health/live`
   - Multi-stage build (node:20-alpine)
   - Uses custom `health-server.mjs` for serving static files

### Environment Variables
The following variables are required:
- `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD`
- `JWT_ISSUER`, `JWT_AUDIENCE`, `JWT_SIGNING_KEY`
- `VITE_API_BASE_URL` - UI build argument
- `LOG_LEVEL` - Debug/Information/Warning/Error
- `ASPNETCORE_ENVIRONMENT` - Development/Production
- `OLLAMA__BASEURL` - Ollama server URL (e.g., `http://ollama:11434` for Docker, `http://ca-ollama-staging.internal.<domain>:11434` for Azure)

### Conventions

**To Follow:**
- Use **multi-stage builds** to minimize image size
- Use **Alpine** base images for smaller footprint
- Copy `package*.json` and run `npm install` BEFORE copying source code (layer caching)
- Copy csproj files and run `dotnet restore` BEFORE copying source code (layer caching)
- Use **non-root users** in containers (`appuser` for API, `node` for UI)
- Include **health checks** for all services
- Use **internal network** between services (don't expose unnecessary ports)
- Use **environment variables** for all configuration
- Document all build arguments in Dockerfile

**To Avoid:**
- Do NOT hardcode credentials in Dockerfiles
- Do NOT use `latest` tag for base images - use specific versions
- Do NOT copy source code before package restore (breaks layer caching)
- Do NOT run containers as root
- Do NOT expose internal service ports to host unnecessarily

---

## Infrastructure

### Bicep Templates
Infrastructure is defined in Bicep and located at project root `/infra/`:

**Main Templates:**
- `main-core.bicep` - Creates subscription-level resources: Resource Group, Container Registry, Storage Account, Container Apps Environment
- `main-apps.bicep` - Creates workload resources: PostgreSQL, API Container App, UI Container App

**Modules:**
- `modules/api-container.bicep` - Azure Container App for .NET API (port 8080, 0.5 CPU, 1Gi memory, 1-3 replicas)
- `modules/ui-container.bicep` - Azure Container App for React UI (port 3000)
- `modules/postgres-service.bicep` - Azure Database for PostgreSQL Flexible Server
- `modules/container-apps-environment.bicep` - Container Apps Environment
- `modules/storage.bicep` - Azure Storage Account
- `modules/container-registry.bicep` - Azure Container Registry

### Azure Services
- **Azure Container Apps** - Hosts API and UI containers
- **Azure Container Registry** - Stores Docker images
- **Azure Database for PostgreSQL Flexible Server** - PostgreSQL database
- **Azure Storage Account** - General purpose storage

### CI/CD
GitHub Actions workflow at `.github/workflows/deploy-staging.yml`:

**Trigger:**
- Push to `develop` branch
- Manual workflow dispatch (with optional imageTag input)

**Jobs:**
1. **build** - Compiles .NET API and builds React UI
2. **deploy-core** - Deploys core Azure infrastructure (Resource Group, Container Registry, Storage, Container Apps Environment)
3. **push** - Builds and pushes Docker images to Azure Container Registry
4. **deploy-apps** - Deploys container apps (PostgreSQL, API, UI) via Bicep

**Environment:** Staging (Azure `uksouth` region, resource group `rg-raysoncv-staging`)

**Secrets Required:**
- `AZURE_CREDENTIALS` - Azure service principal credentials
- `JWT_SIGNING_KEY` - JWT token signing key

### Conventions

**To Follow:**
- Use **Bicep** for infrastructure as code (not Terraform)
- Use **modules** to organize reusable infrastructure components
- Use **environment variables** for all configuration (12-factor app)
- Use **Azure Key Vault** for secrets in production
- Store secrets as **secure parameters** in Bicep (`@secure()`)
- Use **dependsOn** explicitly for resource dependencies

**To Avoid:**
- Do NOT hardcode Azure credentials in code
- Do NOT commit secrets to version control
- Do NOT use ARM JSON templates - use Bicep instead
- Do NOT hardcode resource names - use variables/parameters

---

## Development Workflow

### Local Development
1. Configure `.env` with your values
2. Run `docker compose -f docker-compose.dev.db.yml up -d` for database
3. For API: Open `Api/Presentation` in VSCode, run/debug with .NET debugger
4. For UI: Run `npm install && npm run dev` in `UI/` directory
5. Access UI at `http://localhost:3000`, API at `http://localhost:13245`

### Debugging
- **API**: Use VSCode .NET debugger with `.vscode/launch.json` (if configured)
- **UI**: Use browser DevTools, React Developer Tools extension
- **Database**: Connect via DBeaver or similar on port 5433

### Code Style
- **C#**: Use implicit usings, nullable reference types enabled, primary constructors
- **TypeScript**: Strict mode enabled, use `React.FC`, avoid `any`
- **Formatting**: Prettier/ESLint for UI, dotnet format for API (if configured)

### Conventions

**To Follow:**
- Use **Git** for version control
- Use **feature branches** for development
- Run **linting** before committing (`npm run lint` for UI)
- Use **environment files** (`.env`) for local development only
- Follow existing code style in each codebase section

**To Avoid:**
- Do NOT commit secrets or credentials
- Do NOT push directly to main/master
- Do NOT skip linting/formatting checks
- Do NOT leave debugging code (console.log, print statements) in production

---

## Testing

### Technology Stack
- **Playwright** - Browser automation for React frontend + .NET backend
- **Cucumber** - Parses Gherkin scenarios, executes step definitions
- **TypeScript** - Type-safe step definitions

### Project Structure
```
Test/e2e/
├── features/                    # Gherkin scenario files (.feature)
│   └── auth.feature             # Authentication test scenarios
├── support/                     # Helper utilities
│   ├── api-client.ts            # Direct API calls for test data
│   └── browser-utils.ts         # Shared browser helpers
├── steps.js                     # Cucumber step definitions (JavaScript)
├── cucumber.js                   # Cucumber configuration
└── package.json
```

### Configuration
- **Local**: Runs against `http://localhost:3000` (UI) and `http://localhost:13245` (API)
- **Staging**: Runs against Azure staging URLs via GitHub Actions
- **Environment variables**:
  - `E2E_API_URL` - API base URL (defaults to `http://localhost:13245`)
  - `E2E_UI_URL` - UI base URL (defaults to `http://localhost:3000`)
- **Cucumber config**: `cucumber.js` at project root (not Playwright config)

### Test Data Strategy
- **Static test user**: Seeded in database for login tests
  - Email: `testuser@test.com`
  - Password: `TestPassword123!`
- **Dynamic users**: Created via API for registration tests (unique per test run)

### Running Tests

**Local development:**
```bash
cd Test/e2e
npm install
npx playwright install chromium
npm run e2e:local
```

**Staging:**
```bash
E2E_API_URL=https://your-staging-api.example.com \
E2E_UI_URL=https://your-staging-ui.example.com \
npm run e2e:staging
```

**Staging (via CI):**
- Runs automatically in GitHub Actions after deploy to staging
- Triggered on push to `develop` branch

### Conventions

**To Follow:**
- Use **Gherkin syntax** (.feature files) for scenarios - readable by non-technical stakeholders
- Use **JavaScript** for step definitions (in `steps.js` at root)
- Follow the **Given-When-Then** pattern
- Use the **API client** in `support/api-client.ts` for test data setup
- Take **screenshots on failure** for debugging (saved to `reports/screenshots/`)
- Use environment variables for URLs (`E2E_API_URL`, `E2E_UI_URL`)

**To Avoid:**
- Do NOT write tests that depend on implementation details
- Do NOT hardcode test data that changes per environment (use API client)
- Do NOT create tests that are tightly coupled (prefer independent scenarios)
- Do NOT use TypeScript for step definitions (use plain JavaScript)

---

## Deployment

### Production Setup
- Docker containers for API and UI
- Azure Container Apps for orchestration
- Azure PostgreSQL Flexible Server for database

### Environment Configuration
- `ASPNETCORE_ENVIRONMENT=Production` in production
- CORS origins must be explicitly configured
- JWT signing key must be secure (minimum 16 characters)
- Use HTTPS only in production (HSTS enabled)
- Database connection should use SSL

### Monitoring
- **Development**: Container console logs
- Health endpoints (`/health/live`, `/health/ready`) for container orchestrator
- Serilog with enrichment: correlation ID, environment, thread info

### Conventions

**To Follow:**
- Use **HTTPS only** in production
- Use **environment-specific** configuration
- Enable **HSTS** in production
- Use **secure JWT keys** (minimum 256-bit)
- Configure **CORS** explicitly (not allow all)
- Use **health checks** for container orchestration
- Set **LOG_LEVEL=Information** or lower in production

**To Avoid:**
- Do NOT use development credentials in production
- Do NOT expose detailed error messages to clients
- Do NOT disable HTTPS
- Do NOT use HTTP for internal communication between services

---

## General AI Guidelines

### What This Documentation Is For
This file exists to help AI assistants understand:
1. The project's technology stack and architecture
2. Code organization and folder structure
3. Coding patterns and conventions to follow
4. Anti-patterns and conventions to avoid
5. Configuration and environment setup

### How to Use This Documentation
When making changes to this codebase:
1. Read this file first to understand context
2. Follow the conventions listed for each section
3. Avoid the anti-patterns listed
4. Match existing code style in the area you're modifying

### When to Ask for Clarification
If the user asks you to:
- Add a feature that contradicts this documentation - ask first
- Refactor significantly - explain the proposed changes first
- Add new dependencies - verify they don't conflict with existing stack
- Make security-related changes - always confirm with user first
