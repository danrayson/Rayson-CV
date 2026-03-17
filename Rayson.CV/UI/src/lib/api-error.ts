import { AxiosError } from 'axios';

export interface ApiErrorResponse {
  errors: Record<string, string[]>;
}

export interface ApiError {
  response?: {
    status: number;
    data: ApiErrorResponse;
  };
}

export const isAxiosError = (error: unknown): error is AxiosError<ApiErrorResponse> => {
  return error instanceof AxiosError;
};

export const isApiErrorResponse = (data: unknown): data is ApiErrorResponse => {
  return typeof data === 'object' && data !== null && 'errors' in data;
};

export const getApiErrors = (error: unknown): Record<string, string[]> | null => {
  if (isAxiosError(error) && error.response?.data) {
    const data = error.response.data;
    if (isApiErrorResponse(data)) {
      return data.errors;
    }
  }
  return null;
};

export const getApiErrorStatus = (error: unknown): number | null => {
  if (isAxiosError(error) && error.response) {
    return error.response.status;
  }
  return null;
};

export const isApiError = (error: unknown): error is ApiError => {
  if (isAxiosError(error)) {
    return true;
  }
  return typeof error === 'object' && error !== null && 'response' in error;
};
