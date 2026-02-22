import { Component, ErrorInfo, ReactNode } from 'react';
import { loggingService } from '../services/loggingService';

interface ErrorBoundaryProps {
  children: ReactNode;
}

interface ErrorBoundaryState {
  hasError: boolean;
  error?: Error;
}

class ErrorBoundary extends Component<ErrorBoundaryProps, ErrorBoundaryState> {
  public state: ErrorBoundaryState = { hasError: false };

  public static getDerivedStateFromError(error: Error): ErrorBoundaryState {
    return { hasError: true, error };
  }

  public componentDidCatch(error: Error, errorInfo: ErrorInfo): void {
    loggingService.error(error.message, 'UI.ErrorBoundary', {
      stackTrace: error.stack,
      componentStack: errorInfo.componentStack,
    });
  }

  public render(): ReactNode {
    if (this.state.hasError) {
      return (
        <div className='grid h-screen place-items-center'>
          <div className='text-center'>
            <h1 className='text-2xl font-bold mb-4'>Something went wrong</h1>
            <p className='mb-4'>An unexpected error occurred. Please refresh the page.</p>
            <button
              className='btn btn-primary'
              onClick={() => window.location.reload()}
            >
              Refresh Page
            </button>
          </div>
        </div>
      );
    }
    return this.props.children;
  }
}

export default ErrorBoundary;
