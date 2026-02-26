# ===========================================
# Stage 1: Build
# Uses Node.js to compile the React/Vite application
# ===========================================
FROM node:20-alpine AS build

# Declare build argument for API URL (passed from docker-compose)
# This is required for Docker to accept the --build-arg
ARG VITE_API_BASE_URL
ARG VITE_APP_DOWNLOAD_URL

# Set working directory for build operations
WORKDIR /app

# Copy package files first to enable Docker layer caching
# If package.json hasn't changed, npm install will be cached
COPY package*.json ./

# Install ALL dependencies (including devDependencies like vite, typescript)
# --production flag would skip devDependencies and break the build
RUN npm install

# Copy the rest of the application source code
COPY . .

# Generate Tailwind CSS
RUN npx tailwindcss -i ./src/index.css -o ./src/styles/tailwind.css

# Build the application with Vite
# VITE_API_BASE_URL is injected at build time and embedded into the JS bundle
RUN npm run build

# ===========================================
# Stage 2: Runtime
# Uses Node.js with health server
# ===========================================
FROM node:20-alpine AS runtime

# Install curl for health checks
RUN apk add --no-cache curl

# Set working directory for the runtime
WORKDIR /app

# Ensure /app directory is owned by node user
RUN chown node:node /app

# Copy only the built static files from the build stage
# This excludes node_modules and source code, keeping image small
COPY --from=build --chown=node:node /app/dist ./dist

# Copy health server
COPY --chown=node:node health-server.mjs ./

# Copy package.json for runtime dependencies
COPY --chown=node:node package*.json ./

# Switch to the built-in node user before npm install
# Files created by npm will then be owned by node user
USER node

# Install only production dependencies (pino for logging)
RUN npm install --omit=dev

# Document that the container listens on port 3000
EXPOSE 3000

# Define health check to verify the server is running
# Uses /health/live endpoint (no API dependency check)
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:3000/health/live || exit 1

# Start health server (serves static files + health endpoints)
CMD ["node", "health-server.mjs"]
