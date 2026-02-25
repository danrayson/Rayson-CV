import axios, { AxiosInstance, AxiosRequestConfig, AxiosResponse, InternalAxiosRequestConfig } from 'axios';
import { API_BASE_URL } from '../config';
import { loggingService } from './loggingService';

class HttpClient {
  private instance: AxiosInstance;

  constructor() {
    this.instance = axios.create({
      baseURL: API_BASE_URL // Set the base URL for all requests made by this instance
    });

    // Add interceptor to handle adding authorization tokens to requests
    this.instance.interceptors.request.use(
      (config: AxiosRequestConfig) => {
        const token = localStorage.getItem('x-auth-token'); // Retrieve the token from local storage
        if (token && config.headers) { // Check if the token exists and headers are defined
          config.headers['Authorization'] = `Bearer ${token}`; // Add the token to the Authorization header
        }
        return Promise.resolve(config as InternalAxiosRequestConfig); // Return the modified request configuration
      },
      (error) => Promise.reject(error) // Reject any errors that occur during the request
    );

    // Add response interceptor to log API errors
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

  public setAuthToken(token: string): void {
    localStorage.setItem('x-auth-token', token); // Store the token in local storage for use in requests
  }

  private async request<T>(method: 'get' | 'post' | 'put' | 'delete', url: string, bodyData?: any, queryStringData?: any): Promise<AxiosResponse<T>> {
    url = API_BASE_URL + url;// This seems superfluous considering we've set the base url during construction.
    if (queryStringData) {
      const queryString = Object.keys(queryStringData).map(key => `${encodeURIComponent(key)}=${encodeURIComponent(queryStringData[key])}`).join('&');
      url += `?${queryString}`;
    }
    try {
      const response = await this.instance({
        method,
        url,
        data: bodyData,
      }); // Make a request using the Axios instance
      return response; // Return the response
    } catch (error) {
      throw error; // Rethrow any errors that occur during the request
    }
  }

  public async get<T>(url: string, queryStringData?: any): Promise<AxiosResponse<T>> {
    return this.request('get', url, undefined, queryStringData); // Make a GET request to the specified URL with optional query string data
  }

  public async post<T>(url: string, data?: any, queryStringData?: any): Promise<AxiosResponse<T>> {
    return this.request('post', url, data, queryStringData); // Make a POST request to the specified URL with optional data
  }

  public async put<T>(url: string, data?: any, queryStringData?: any): Promise<AxiosResponse<T>> {
    return this.request('put', url, data, queryStringData); // Make a PUT request to the specified URL with optional data
  }

  public async delete<T>(url: string, queryStringData?: any): Promise<AxiosResponse<T>> {
    return this.request('delete', url, null, queryStringData); // Make a DELETE request to the specified URL
  }
}

export default HttpClient;