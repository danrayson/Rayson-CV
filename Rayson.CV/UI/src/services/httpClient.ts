import axios, { AxiosInstance, AxiosResponse } from 'axios';
import { API_BASE_URL } from '../config';
import { loggingService } from './loggingService';

class HttpClient {
  private instance: AxiosInstance;

  constructor() {
    this.instance = axios.create({
      baseURL: API_BASE_URL
    });

    this.instance.interceptors.response.use(
      (response) => response,
      (error) => {
        loggingService.error(
          `API Error: ${error.config?.url} - ${error.message}`,
          'UI.HttpClient',
          {
            status: error.response?.status,
            data: error.response?.data,
            stack: error.stack
          }
        );
        return Promise.reject(error);
      }
    );
  }

  private async request<T>(method: 'get' | 'post' | 'put' | 'delete', url: string, bodyData?: any, queryStringData?: any): Promise<AxiosResponse<T>> {
    url = API_BASE_URL + url;
    if (queryStringData) {
      const queryString = Object.keys(queryStringData).map(key => `${encodeURIComponent(key)}=${encodeURIComponent(queryStringData[key])}`).join('&');
      url += `?${queryString}`;
    }
    try {
      const response = await this.instance({
        method,
        url,
        data: bodyData,
      });
      return response;
    } catch (error) {
      throw error;
    }
  }

  public async get<T>(url: string, queryStringData?: any): Promise<AxiosResponse<T>> {
    return this.request('get', url, undefined, queryStringData);
  }

  public async post<T>(url: string, data?: any, queryStringData?: any): Promise<AxiosResponse<T>> {
    return this.request('post', url, data, queryStringData);
  }

  public async put<T>(url: string, data?: any, queryStringData?: any): Promise<AxiosResponse<T>> {
    return this.request('put', url, data, queryStringData);
  }

  public async delete<T>(url: string, queryStringData?: any): Promise<AxiosResponse<T>> {
    return this.request('delete', url, null, queryStringData);
  }

  public async postStreaming<T>(url: string, data: T, onChunk: (chunk: string) => void): Promise<void> {
    const response = await fetch(`${API_BASE_URL}${url}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`API Error: ${response.status} - ${errorText}`);
    }

    if (!response.body) {
      throw new Error('No response body');
    }

    const reader = response.body.getReader();
    const decoder = new TextDecoder();
    let buffer = '';

    while (true) {
      const { done, value } = await reader.read();
      
      if (done) break;
      
      buffer += decoder.decode(value, { stream: true });
      
      const lines = buffer.split('\n');
      buffer = lines.pop() || '';
      
      for (const line of lines) {
        if (!line.trim()) continue;
        
        // Handle SSE format: "data: {...}" or "data: [DONE]"
        let jsonStr = line;
        if (line.startsWith('data: ')) {
          jsonStr = line.slice(6);
          if (jsonStr === '[DONE]' || jsonStr === '[DONE]\r') {
            return;
          }
        }
        
        try {
          const parsed = JSON.parse(jsonStr);
          
          if (parsed.message?.content) {
            onChunk(parsed.message.content);
          }
          
          if (parsed.error) {
            throw new Error(parsed.error);
          }
        } catch {
          // Skip invalid JSON
        }
      }
    }
  }
}

export default HttpClient;