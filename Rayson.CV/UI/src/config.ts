const getApiBaseUrl = () => {
  console.log("import.meta.env.VITE_API_BASE_URL is \"", import.meta.env.VITE_API_BASE_URL, "\"");
  if (import.meta.env.VITE_API_BASE_URL) {
    console.log("returning import.meta.env.VITE_API_BASE_URL");
    return import.meta.env.VITE_API_BASE_URL;
  }
  console.log("window.location.hostname is \"", window.location.hostname, "\"");
  if (window.location.hostname.trim() === 'localhost' || window.location.hostname.trim() === '127.0.0.1' || window.location.hostname.trim() === '') {
    console.log("returning 'http://localhost:5001'");
    return 'http://localhost:5001';
  }

  console.log("returning 'http://simple-api:5000'.  This is probably wrong.");
  return 'http://simple-api:5000';
};

export const API_BASE_URL = getApiBaseUrl();