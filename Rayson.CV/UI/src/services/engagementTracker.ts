import { getUserCorrelationId } from '../utils/correlation';
import { loggingService } from './loggingService';

interface SectionEvent {
  type: 'visible' | 'hidden';
  sectionId: string;
  duration?: number;
}

class EngagementTracker {
  private observer: IntersectionObserver;
  private sectionTimers: Map<string, number> = new Map();

  constructor() {
    this.observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          const sectionId = entry.target.getAttribute('data-section-id');
          if (!sectionId) return;

          if (entry.isIntersecting) {
            this.sectionTimers.set(sectionId, Date.now());
            this.onEvent({ type: 'visible', sectionId });
          } else if (this.sectionTimers.has(sectionId)) {
            const duration = Date.now() - this.sectionTimers.get(sectionId)!;
            this.onEvent({ type: 'hidden', sectionId, duration });
            this.sectionTimers.delete(sectionId);
          }
        });
      },
      { threshold: 0.5 }
    );
  }

  private onEvent(event: SectionEvent) {
    const eventType = event.type === 'visible' ? 'SectionVisible' : 'SectionHidden';
    loggingService.logSectionEvent({
      sectionId: event.sectionId,
      duration: event.duration,
      correlationId: getUserCorrelationId(),
      eventType,
    });
  }

  init() {
    document.querySelectorAll('[data-section-id]').forEach((el) => this.observer.observe(el));
  }
}

export const engagementTracker = new EngagementTracker();
