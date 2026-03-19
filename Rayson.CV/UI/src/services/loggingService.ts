import HttpClient from './httpClient';
import { getUserCorrelationId } from '../utils/correlation';

export interface ApiCallEvent {
  eventType: 'ApiCall';
  method: string;
  path: string;
  status: number;
  duration: number;
  correlationId: string;
}

export interface PageViewEvent {
  eventType: 'PageView';
  path: string;
  referrer: string;
  correlationId: string;
  userAgent: string;
  language: string;
  screenWidth: number;
  screenHeight: number;
  timezone: string;
}

export interface SectionEvent {
  eventType: 'SectionVisible' | 'SectionHidden';
  sectionId: string;
  duration?: number;
  correlationId: string;
}

export interface ClickEvent {
  eventType: 'Click';
  elementId: string;
  elementText: string;
  correlationId: string;
}

type ClientLogEvent = ApiCallEvent | PageViewEvent | SectionEvent | ClickEvent;

class LoggingService {
  private httpClient: HttpClient | null = null;

  private getHttpClient(): HttpClient {
    if (!this.httpClient) {
      this.httpClient = new HttpClient();
    }
    return this.httpClient;
  }

  private async log(event: ClientLogEvent): Promise<void> {
    try {
      await this.getHttpClient().post('logs', event);
    } catch {
      // Silently fail - do not log or console as that causes infinite loops
    }
  }

  public logApiCall(event: Omit<ApiCallEvent, 'eventType'>): void {
    this.log({ ...event, eventType: 'ApiCall' });
    console.info(`[ApiCall] ${event.method} ${event.path} - ${event.status} (${event.duration.toFixed(0)}ms)`);
  }

  public logPageView(event: Omit<PageViewEvent, 'eventType'>): void {
    this.log({ ...event, eventType: 'PageView' });
    console.info(`[PageView] ${event.path}`);
  }

  public logSectionEvent(event: Omit<SectionEvent, 'eventType'> & { eventType: 'SectionVisible' | 'SectionHidden' }): void {
    this.log({ ...event, eventType: event.eventType });
    console.info(`[${event.eventType}] ${event.sectionId}${event.duration ? ` (${event.duration}ms)` : ''}`);
  }

  public logClick(elementId: string, elementText: string): void {
    this.log({
      eventType: 'Click',
      elementId,
      elementText,
      correlationId: getUserCorrelationId(),
    });
  }

  public error(message: string, source?: string, additionalData?: Record<string, unknown>): void {
    console.error(`[${source ?? 'UI'}] ${message}`, additionalData ?? '');
    this.log({
      eventType: 'ApiCall',
      method: 'ERROR',
      path: source ?? 'Unknown',
      status: 0,
      duration: 0,
      correlationId: getUserCorrelationId(),
    });
  }
}

export const loggingService = new LoggingService();
