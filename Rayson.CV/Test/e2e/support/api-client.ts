import dotenv from 'dotenv';

dotenv.config({ path: '.env' });

const API_BASE_URL = process.env.E2E_API_URL || 'http://localhost:13245';

export interface SignInResponse {
  status: number;
  token?: string;
  headers: Headers;
}

export interface ApiResponse {
  ok: boolean;
  status: number;
  body?: unknown;
}

export async function signIn(email: string, password: string): Promise<SignInResponse> {
  const response = await fetch(`${API_BASE_URL}/auth/signin`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ email, password }),
  });

  const token = response.headers.get('X-Auth-Token') || undefined;

  return {
    status: response.status,
    token,
    headers: response.headers,
  };
}

export async function signUp(email: string, password: string): Promise<ApiResponse> {
  const response = await fetch(`${API_BASE_URL}/auth/signup`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ email, password }),
  });

  let body: unknown;
  try {
    body = await response.json();
  } catch {
    body = undefined;
  }

  return {
    ok: response.ok,
    status: response.status,
    body,
  };
}

export async function requestPasswordReset(email: string): Promise<ApiResponse> {
  const response = await fetch(`${API_BASE_URL}/auth/request-password-reset`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ email }),
  });

  let body: unknown;
  try {
    body = await response.json();
  } catch {
    body = undefined;
  }

  return {
    ok: response.ok,
    status: response.status,
    body,
  };
}

export async function resetPassword(token: string, newPassword: string): Promise<ApiResponse> {
  const response = await fetch(`${API_BASE_URL}/auth/reset-password`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ token, newPassword }),
  });

  let body: unknown;
  try {
    body = await response.json();
  } catch {
    body = undefined;
  }

  return {
    ok: response.ok,
    status: response.status,
    body,
  };
}

export async function checkHealth(): Promise<boolean> {
  try {
    const response = await fetch(`${API_BASE_URL}/health/live`);
    return response.ok;
  } catch {
    return false;
  }
}

export async function getAuthToken(email: string, password: string): Promise<string | null> {
  const response = await signIn(email, password);
  return response.token || null;
}

export function setAuthToken(token: string): void {
  globalThis.sessionStorage.setItem('x-auth-token', token);
}

export function clearAuthToken(): void {
  globalThis.sessionStorage.removeItem('x-auth-token');
}
