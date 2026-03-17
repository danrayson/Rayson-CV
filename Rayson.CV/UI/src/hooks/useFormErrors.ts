import { useState } from 'react';
import { getApiErrors, getApiErrorStatus, isAxiosError } from '@/lib/api-error';

interface UseFormErrorsReturn {
  errors: Record<string, string[]>;
  showErrors: boolean;
  setErrors: (errors: Record<string, string[]>) => void;
  setShowErrors: (show: boolean) => void;
  handleApiError: (error: unknown, customMessages?: Record<number, string>) => void;
  clearErrors: () => void;
}

export const useFormErrors = (): UseFormErrorsReturn => {
  const [errors, setErrors] = useState<Record<string, string[]>>({});
  const [showErrors, setShowErrors] = useState(false);

  const handleApiError = (error: unknown, customMessages?: Record<number, string>): void => {
    const defaultMessages: Record<number, string> = {
      400: 'Please check your input and try again.',
      401: 'You are not authorized to perform this action.',
      403: 'You do not have permission to perform this action.',
      404: 'The requested resource was not found.',
      500: 'An internal server error occurred.',
      503: 'The service is temporarily unavailable.',
    };

    const messages = { ...defaultMessages, ...customMessages };

    const apiErrors = getApiErrors(error);
    const status = getApiErrorStatus(error);

    if (apiErrors) {
      setErrors(apiErrors);
    } else if (status !== null) {
      setErrors({ 'Error': [messages[status] || 'An unexpected error occurred.'] });
    } else if (!isAxiosError(error)) {
      setErrors({ 'Error': ['An unexpected error occurred.'] });
    } else {
      setErrors({ 'Error': ['An unexpected error occurred.'] });
    }
    setShowErrors(true);
  };

  const clearErrors = (): void => {
    setErrors({});
    setShowErrors(false);
  };

  return {
    errors,
    showErrors,
    setErrors,
    setShowErrors,
    handleApiError,
    clearErrors,
  };
};
