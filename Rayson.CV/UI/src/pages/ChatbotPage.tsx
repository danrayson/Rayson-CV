import { useState, useRef, useEffect } from 'react';
import { ArrowLeftIcon, PaperAirplaneIcon, ExclamationTriangleIcon } from '@heroicons/react/24/outline';
import { chatbotService, ChatMessage } from '../services/chatbotService';

const ChatbotPage: React.FC = () => {
  const [messages, setMessages] = useState<ChatMessage[]>([
    {
      role: 'assistant',
      content: 'Hi! I\'m here to answer questions about Dan Rayson\'s CV. What would you like to know?'
    }
  ]);
  const [input, setInput] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [streamingContent, setStreamingContent] = useState('');
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages, streamingContent]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!input.trim() || isLoading) return;

    const userMessage: ChatMessage = {
      role: 'user',
      content: input.trim()
    };

    setMessages(prev => [...prev, userMessage]);
    setInput('');
    setIsLoading(true);
    setStreamingContent('');

    let finalContent = '';

    try {
      const history = messages.map(m => ({ role: m.role, content: m.content }));
      
      await chatbotService.sendMessageStreaming(
        userMessage.content,
        history,
        (chunk) => {
          setStreamingContent(prev => prev + chunk);
          finalContent += chunk;
        }
      );
      
      if (finalContent) {
        const assistantMessage: ChatMessage = {
          role: 'assistant',
          content: finalContent
        };
        setMessages(prev => [...prev, assistantMessage]);
      }
    } catch {
      const errorMessage: ChatMessage = {
        role: 'assistant',
        content: 'Sorry, I encountered an error. Please try again.'
      };
      setMessages(prev => [...prev, errorMessage]);
    } finally {
      setIsLoading(false);
      setStreamingContent('');
    }
  };

  return (
    <div className="relative min-h-screen bg-base-200 flex flex-col">
      <button
        onClick={() => window.history.back()}
        className="absolute top-4 left-4 btn btn-sm btn-ghost z-10"
      >
        <ArrowLeftIcon className="w-5 h-5" />
        Home
      </button>

      <div className="alert alert-warning mt-16">
        <ExclamationTriangleIcon className="h-6 w-6" />
        <div>
          <p className="font-bold">AI Limitation Warning</p>
          <p className="text-sm">
            This chatbot uses TinyLlama and provides inaccurate information on a regular basis. 
            For accurate details, please{' '}
            <a href="/CV September 2024.pdf" download className="underline font-bold">
              download the real CV
            </a>.
          </p>
        </div>
      </div>

      <div className="flex-1 overflow-y-auto p-4 pb-24">
        <div className="max-w-3xl mx-auto space-y-2">
          {messages.map((message, index) => (
            <div
              key={index}
              className={`chat ${message.role === 'user' ? 'chat-end' : 'chat-start'}`}
            >
              <div 
                className={`chat-bubble ${
                  message.role === 'user' 
                    ? 'chat-bubble-primary rounded-2xl rounded-tr-sm' 
                    : 'chat-bubble-secondary rounded-2xl rounded-tl-sm text-left'
                }`}
              >
                {message.content}
              </div>
            </div>
          ))}
          {streamingContent && (
            <div className="chat chat-start">
              <div className="chat-bubble chat-bubble-secondary rounded-2xl rounded-tl-sm text-left">
                {streamingContent}
              </div>
            </div>
          )}
          {isLoading && !streamingContent && (
            <div className="chat chat-start">
              <div className="chat-bubble bg-base-300 rounded-2xl rounded-tl-sm">
                <div className="flex gap-1">
                  <span className="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style={{ animationDelay: '0ms' }}></span>
                  <span className="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style={{ animationDelay: '150ms' }}></span>
                  <span className="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style={{ animationDelay: '300ms' }}></span>
                </div>
              </div>
            </div>
          )}
          <div ref={messagesEndRef} />
        </div>
      </div>

      <div className="fixed bottom-0 left-0 right-0 bg-base-100 px-4 py-3">
        <form onSubmit={handleSubmit} className="max-w-3xl mx-auto flex items-center gap-2">
          <input
            type="text"
            value={input}
            onChange={(e) => setInput(e.target.value)}
            placeholder="Ask me about Dan Rayson's CV..."
            className="flex-1 bg-base-300 rounded-full px-4 py-2.5 outline-none placeholder-base-content/50"
            disabled={isLoading}
          />
          <button
            type="submit"
            className="btn btn-circle btn-primary"
            disabled={isLoading || !input.trim()}
          >
            {isLoading ? (
              <span className="loading loading-spinner loading-sm"></span>
            ) : (
              <PaperAirplaneIcon className="w-5 h-5" />
            )}
          </button>
        </form>
      </div>
    </div>
  );
};

export default ChatbotPage;
