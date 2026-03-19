import { loggingService } from './loggingService';

class EngagementTracker {
  public setupEventListeners(): void {
    window.addEventListener('section-visible', ((e: Event) => {
      const customEvent = e as CustomEvent<{ sectionId: string }>;
      loggingService.logSectionEvent({ sectionId: customEvent.detail.sectionId });
    }) as EventListener);
  }

  public init(): void {
    this.setupEventListeners();
  }
}

export const engagementTracker = new EngagementTracker();
