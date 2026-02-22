import HttpClient from './httpClient';

interface ClientLogEvent {
  level: 'Error' | 'Warning' | 'Information';
  message: string;
  source?: string;
  browserInfo?: string;
  stackTrace?: string;
  additionalData?: Record<string, unknown>;
}

class LoggingService {
  private httpClient: HttpClient | null = null;

  private getHttpClient(): HttpClient {
    if (!this.httpClient) {
      this.httpClient = new HttpClient();
    }
    return this.httpClient;
  }

  private getBrowserInfo(): string {
    return `${navigator.userAgent} | ${window.location.href}`;
  }

  private async log(level: ClientLogEvent['level'], message: string, source?: string, additionalData?: Record<string, unknown>): Promise<void> {
    try {
      const event: ClientLogEvent = {
        level,
        message,
        source: source ?? 'UI',
        browserInfo: this.getBrowserInfo(),
        additionalData,
      };

      await this.getHttpClient().post('logs', event);
    } catch {
      console.error('Failed to send log to API');
    }
  }

  public error(message: string, source?: string, additionalData?: Record<string, unknown>): void {
    this.log('Error', message, source, additionalData);
    console.error(`[${source ?? 'UI'}] ${message}`, additionalData ?? '');
  }

  public warn(message: string, source?: string, additionalData?: Record<string, unknown>): void {
    this.log('Warning', message, source, additionalData);
    console.warn(`[${source ?? 'UI'}] ${message}`, additionalData ?? '');
  }

  public info(message: string, source?: string, additionalData?: Record<string, unknown>): void {
    this.log('Information', message, source, additionalData);
    console.info(`[${source ?? 'UI'}] ${message}`, additionalData ?? '');
  }
}

export const loggingService = new LoggingService();
