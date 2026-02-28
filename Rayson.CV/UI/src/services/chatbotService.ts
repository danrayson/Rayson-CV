import HttpClient from './httpClient';

export interface ChatMessage {
  role: 'user' | 'assistant';
  content: string;
}

interface ChatbotRequest {
  message: string;
}

interface ChatbotResponse {
  message: string;
}

class ChatbotService {
  private httpClient: HttpClient | null = null;

  private getHttpClient(): HttpClient {
    if (!this.httpClient) {
      this.httpClient = new HttpClient();
    }
    return this.httpClient;
  }

  async sendMessage(message: string): Promise<ChatbotResponse> {
    const client = this.getHttpClient();
    
    const request: ChatbotRequest = {
      message
    };

    const response = await client.post<ChatbotResponse>('chatbot', request);
    return response.data;
  }

  async sendMessageStreaming(message: string, onChunk: (chunk: string) => void): Promise<void> {
    const client = this.getHttpClient();
    
    const request: ChatbotRequest = {
      message
    };

    await client.postStreaming('chatbot/stream', request, onChunk);
  }
}

export const chatbotService = new ChatbotService();
