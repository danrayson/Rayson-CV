import { loggingService } from '../services/loggingService';

export function initializeErrorHandlers(): void {
  window.onerror = (message, source, lineno, colno, error) => {
    loggingService.error(
      `${message} at ${source}:${lineno}:${colno}`,
      'UI.Global',
      { stack: error?.stack }
    );
    return false;
  };

  window.onunhandledrejection = (event) => {
    const error = event.reason;
    loggingService.error(
      error?.message || 'Unhandled promise rejection',
      'UI.Promise',
      { stack: error?.stack }
    );
  };
}

export function withErrorHandling<T extends (...args: any[]) => Promise<any>>(
  fn: T,
  source: string
): T {
  return (async (...args: any[]) => {
    try {
      return await fn(...args);
    } catch (error: any) {
      loggingService.error(error.message, source, { stack: error.stack });
      throw error;
    }
  }) as T;
}

export function withSyncErrorHandling<T extends (...args: any[]) => any>(
  fn: T,
  source: string
): T {
  return ((...args: any[]) => {
    try {
      return fn(...args);
    } catch (error: any) {
      loggingService.error(error.message, source, { stack: error.stack });
      throw error;
    }
  }) as T;
}
