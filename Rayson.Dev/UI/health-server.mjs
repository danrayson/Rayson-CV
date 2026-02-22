import http from 'http';
import { createReadStream } from 'fs';
import { stat } from 'fs/promises';
import { extname, join } from 'path';

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
    return { status: 'Unhealthy', statusCode: response.status };
  } catch (error) {
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
      } catch {}
    }
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
  console.log(`Health server running on port ${PORT}`);
  console.log(`API health URL: ${API_URL}`);
});
