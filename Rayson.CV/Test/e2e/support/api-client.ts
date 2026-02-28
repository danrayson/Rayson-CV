import dotenv from 'dotenv';

dotenv.config({ path: '.env' });

const API_BASE_URL = process.env.E2E_API_URL || 'http://localhost:13245';

export async function checkHealth(): Promise<boolean> {
  try {
    const response = await fetch(`${API_BASE_URL}/health/live`);
    return response.ok;
  } catch {
    return false;
  }
}
