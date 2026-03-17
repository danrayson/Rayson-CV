import HttpClient from './httpClient';

export interface ContactRequest {
  name: string;
  email: string;
  subject: string;
  message: string;
}

class ContactService {
  private httpClient: HttpClient | null = null;

  private getHttpClient(): HttpClient {
    if (!this.httpClient) {
      this.httpClient = new HttpClient();
    }
    return this.httpClient;
  }

  async sendContactEmail(request: ContactRequest): Promise<void> {
    const client = this.getHttpClient();
    await client.post('contact', request);
  }
}

export const contactService = new ContactService();
