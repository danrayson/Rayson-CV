# ===========================================
# Stage 1: Build
# Uses Node.js to compile the React/Vite application
# ===========================================
FROM node:20-alpine AS build

# Declare build argument for API URL (passed from docker-compose)
# This is required for Docker to accept the --build-arg
ARG VITE_API_BASE_URL

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

# Build the application with Vite
# VITE_API_BASE_URL is injected at build time and embedded into the JS bundle
RUN npm run build

# ===========================================
# Stage 2: Runtime
# Minimal image with only the built static files and a server
# ===========================================
FROM node:20-alpine AS runtime

# Install serve globally to serve static files
# curl is needed for health checks
RUN apk add --no-cache curl && \
    npm install -g serve

# Set working directory for the runtime
WORKDIR /app

# Copy only the built static files from the build stage
# This excludes node_modules and source code, keeping image small
COPY --from=build /app/dist ./dist

# Change ownership to the built-in node user
# The node:20-alpine image already has a 'node' user with UID 1000
RUN chown -R node:node /app

# Document that the container listens on port 3000
EXPOSE 3000

# Define health check to verify the server is running
# --interval=30s: Check every 30 seconds
# --timeout=3s: Wait 3 seconds for response
# --start-period=5s: Grace period on container start
# --retries=3: Mark unhealthy after 3 consecutive failures
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:3000/ || exit 1

# Switch to the built-in node user for security
USER node

# Start the static file server
# -s: Rewrite all requests to index.html (SPA routing)
# -l 3000: Listen on port 3000
CMD ["serve", "-s", "dist", "-l", "3000"]
