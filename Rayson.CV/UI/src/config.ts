const getApiBaseUrl = () => {
  if (import.meta.env.VITE_API_BASE_URL) {
    return import.meta.env.VITE_API_BASE_URL;
  }
  throw new Error('VITE_API_BASE_URL not set');
};

export const API_BASE_URL = getApiBaseUrl();