import { getUserCorrelationId } from '../utils/correlation';
import { loggingService } from './loggingService';

class PageTrackingService {
  private currentPath = '';

  init() {
    this.trackPageView();

    const originalPushState = history.pushState;
    history.pushState = (...args) => {
      originalPushState.apply(history, args);
      this.trackPageView();
    };

    window.addEventListener('popstate', () => this.trackPageView());
  }

  private trackPageView() {
    const newPath = window.location.pathname + window.location.hash;
    if (newPath !== this.currentPath) {
      this.currentPath = newPath;
      loggingService.logPageView({
        path: newPath,
        referrer: document.referrer,
        correlationId: getUserCorrelationId(),
        userAgent: navigator.userAgent,
        language: navigator.language,
        screenWidth: window.innerWidth,
        screenHeight: window.innerHeight,
        timezone: Intl.DateTimeFormat().resolvedOptions().timeZone,
      });
    }
  }
}

export const pageTrackingService = new PageTrackingService();
