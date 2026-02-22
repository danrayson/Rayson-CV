import http from 'http';
import { createReadStream } from 'fs';
import { stat } from 'fs/promises';
import { extname, join } from 'path';
import pino from 'pino';

const logger = pino({
  level: process.env.LOG_LEVEL || 'info',
});

const PORT = 3000;
const API_URL = process.env.API_HEALTH_URL || 'http://api:8080/health';
const STATIC_DIR = './dist';

const MIME_TYPES = {
  '.html': 'text/html',
  '.js': 'application/javascript',
  '.css': 'text/css',
  '.json': 'application/json',
  '.svg': 'image/svg+xml',
  '.png': 'image/png',
  '.ico': 'image/x-icon',
};

async function checkApiHealth() {
  try {
    const response = await fetch(API_URL, {
      method: 'GET',
      signal: AbortSignal.timeout(5000)
    });

    if (response.ok) {
      return { status: 'Healthy', statusCode: response.status };
    }
    logger.warn({ statusCode: response.status }, 'API health check returned non-OK status');
    return { status: 'Unhealthy', statusCode: response.status };
  } catch (error) {
    logger.error({ error: error.message }, 'API health check failed');
    return { status: 'Unhealthy', error: error.message };
  }
}

function sendJson(res, statusCode, data) {
  res.writeHead(statusCode, { 'Content-Type': 'application/json; charset=utf-8' });
  res.end(JSON.stringify(data, null, 2));
}

async function handleHealth(res, includeApi = true) {
  const checks = {
    ui: { status: 'Healthy', description: 'UI server is running' }
  };

  if (includeApi) {
    checks.api = await checkApiHealth();
  }

  const allHealthy = Object.values(checks).every(c => c.status === 'Healthy');
  const statusCode = allHealthy ? 200 : 503;

  if (!allHealthy) {
    logger.warn({ checks }, 'Health check failed');
  }

  sendJson(res, statusCode, {
    status: allHealthy ? 'Healthy' : 'Unhealthy',
    checks,
    totalDuration: 0
  });
}

async function serveStatic(res, url) {
  let filePath = join(STATIC_DIR, url === '/' ? 'index.html' : url);
  const ext = extname(filePath);
  const mimeType = MIME_TYPES[ext] || 'application/octet-stream';

  try {
    const stats = await stat(filePath);
    if (stats.isDirectory()) {
      filePath = join(filePath, 'index.html');
    }
    res.writeHead(200, { 'Content-Type': mimeType });
    createReadStream(filePath).pipe(res);
  } catch (error) {
    if (error.code === 'ENOENT' && !ext) {
      try {
        res.writeHead(200, { 'Content-Type': 'text/html' });
        createReadStream(join(STATIC_DIR, 'index.html')).pipe(res);
        return;
      } catch (fallbackError) {
        logger.error({ error: fallbackError.message, url }, 'Failed to serve fallback index.html');
      }
    }
    logger.warn({ error: error.message, url, filePath }, 'Static file not found');
    res.writeHead(404);
    res.end(JSON.stringify({ error: 'Not found' }));
  }
}

const server = http.createServer(async (req, res) => {
  const url = req.url.split('?')[0];

  if (url === '/health' || url === '/health/ready') {
    await handleHealth(res, true);
  } else if (url === '/health/live') {
    await handleHealth(res, false);
  } else {
    await serveStatic(res, url);
  }
});

server.listen(PORT, () => {
  logger.info({ port: PORT, apiUrl: API_URL }, 'Health server started');
});

process.on('SIGTERM', () => {
  logger.info('Received SIGTERM, shutting down gracefully');
  server.close(() => {
    logger.info('Server closed');
    process.exit(0);
  });
});

process.on('SIGINT', () => {
  logger.info('Received SIGINT, shutting down gracefully');
  server.close(() => {
    logger.info('Server closed');
    process.exit(0);
  });
});

process.on('uncaughtException', (error) => {
  logger.fatal({ error: error.message, stack: error.stack }, 'Uncaught exception');
  process.exit(1);
});

process.on('unhandledRejection', (reason, promise) => {
  logger.error({ reason }, 'Unhandled promise rejection');
});
