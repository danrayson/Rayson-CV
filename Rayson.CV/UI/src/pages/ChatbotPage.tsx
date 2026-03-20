import { useState, useRef, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { ArrowLeftIcon, PaperAirplaneIcon } from '@heroicons/react/24/outline';
import ReactMarkdown from 'react-markdown';
import remarkGfm from 'remark-gfm';
import { chatbotService, ChatMessage } from '../services/chatbotService';

const ChatbotPage: React.FC = () => {
  const navigate = useNavigate();
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
  const inputRef = useRef<HTMLInputElement>(null);

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
      await chatbotService.sendMessageStreaming(
        userMessage.content,
        messages,
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
      // Use setTimeout to defer focus until after React re-renders and removes the disabled attribute
      setTimeout(() => inputRef.current?.focus(), 0);
    }
  };

  return (
    <div className="relative h-screen bg-base-200">
      <button
        onClick={() => navigate('/')}
        className="fixed top-4 left-4 btn btn-sm btn-ghost z-10"
      >
        <ArrowLeftIcon className="w-5 h-5" />
        Home
      </button>

      <div className="absolute top-20 bottom-20 left-4 right-4">
        <div className="max-w-3xl mx-auto h-full border border-base-300 rounded-2xl bg-base-100 overflow-y-auto">
          <div className="p-4 space-y-2 text-base-content">
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
                  {message.role === 'user' ? (
                    message.content
                  ) : (
                    <div className="prose prose-sm max-w-none prose-headings:mt-0 prose-p:my-1 prose-ul:my-1 prose-ol:my-1 prose-li:my-0">
                      <ReactMarkdown remarkPlugins={[remarkGfm]}>
                        {message.content}
                      </ReactMarkdown>
                    </div>
                  )}
                </div>
              </div>
            ))}
            {streamingContent && (
              <div className="chat chat-start">
                <div className="chat-bubble chat-bubble-secondary rounded-2xl rounded-tl-sm text-left">
                  <div className="prose prose-sm max-w-none prose-headings:mt-0 prose-p:my-1 prose-ul:my-1 prose-ol:my-1 prose-li:my-0">
                    <ReactMarkdown remarkPlugins={[remarkGfm]}>
                      {streamingContent}
                    </ReactMarkdown>
                  </div>
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
      </div>

      <div className="fixed bottom-0 left-0 right-0 bg-base-100 px-4 py-3">
        <form onSubmit={handleSubmit} className="max-w-3xl mx-auto flex items-center gap-2">
          <input
            ref={inputRef}
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
