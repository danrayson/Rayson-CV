# RaysonCV UI

React + TypeScript + Vite frontend for the RaysonCV application.

## Tech Stack

- React 18.3 + Vite 5.4
- TypeScript 5.5
- Tailwind CSS 3.4 + DaisyUI 4.12
- React Router DOM 6.27 (HashRouter for Electron compatibility)
- Axios for HTTP requests
- Highcharts for data visualization

## Styles

Tailwind CSS is used with DaisyUI. The compiled CSS is generated automatically by Vite during development and build.

To manually rebuild styles (if needed):
```bash
npx tailwindcss -i ./src/index.css -o ./src/styles/tailwind.css --watch
```

## Scripts

| Command | Description |
|---------|-------------|
| `npm run dev` | Start development server |
| `npm run build` | Build for production |
| `npm run lint` | Run ESLint |
| `npm run preview` | Preview production build |
| `npm run electron:dev` | Run as Electron desktop app (dev) |
| `npm run electron:build` | Build Electron desktop app |

## Running

### Development

```bash
npm install
npm run dev
```

The app will be available at `http://localhost:5173` (Vite default)

### Docker

```bash
cd ../../
docker compose -f docker-compose.dev.full.yml up -d --build
```

Or build and run standalone:

```bash
cd Rayson.CV/UI
docker build -t raysoncv-web-gui .
docker run -p 3000:3000 raysoncv-web-gui
```

### Electron Desktop App

```bash
npm run electron:dev    # Development mode
npm run electron:build  # Build production executable
```

## API Integration

All API calls go through `src/services/httpClient.ts` which:
- Uses Axios instance with base URL from `config.ts`
- Automatically adds `Authorization: Bearer <token>` header from localStorage
- Has response interceptor that logs API errors
- Provides typed methods: `get()`, `post()`, `put()`, `delete()`

Token is stored in `localStorage` under key `x-auth-token`.

## Routing

Uses `HashRouter` (not BrowserRouter) for Electron compatibility. Routes are defined in `App.tsx`.

## Key Components

- **Pages**: `LandingPage.tsx`, `Basic.tsx` (in `src/pages/`)
- **Components**: `Login.tsx`, `SignUp.tsx`, `ForgottenPassword.tsx`, `ResetPassword.tsx`, `ErrorBoundary.tsx` (in `src/components/`)
- **Elements**: `FormRow.tsx`, `ValidationMessages.tsx` (in `src/elements/`)
- **Services**: `httpClient.ts`, `loggingService.ts` (in `src/services/`)
