import { loggingService } from '@/services/loggingService';

export function useTrackedClick<T extends React.MouseEvent | MouseEvent>(
  elementId: string,
  originalHandler?: () => void
) {
  return (e: T) => {
    const text = (e.currentTarget as HTMLElement).textContent?.trim() ?? '';
    loggingService.logClick(elementId, text);
    originalHandler?.();
  };
}
