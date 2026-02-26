import { useState, useRef, useEffect } from 'react';
import { ArrowLeftIcon, PaperAirplaneIcon } from '@heroicons/react/24/outline';
import { chatbotService, ChatMessage } from '../services/chatbotService';

const ChatbotPage: React.FC = () => {
  const [messages, setMessages] = useState<ChatMessage[]>([
    {
      role: 'assistant',
      content: 'Hi! I\'m here to answer questions about Rayson\'s CV. What would you like to know?'
    }
  ]);
  const [input, setInput] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

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

    try {
      const history = messages.map(m => ({ role: m.role, content: m.content }));
      const response = await chatbotService.sendMessage(userMessage.content, history);
      
      const assistantMessage: ChatMessage = {
        role: 'assistant',
        content: response.message
      };
      
      setMessages(prev => [...prev, assistantMessage]);
    } catch {
      const errorMessage: ChatMessage = {
        role: 'assistant',
        content: 'Sorry, I encountered an error. Please try again.'
      };
      setMessages(prev => [...prev, errorMessage]);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="relative min-h-screen bg-base-200 flex flex-col">
      <button
        onClick={() => window.history.back()}
        className="absolute top-4 left-4 btn btn-sm btn-ghost z-10"
      >
        <ArrowLeftIcon className="w-5 h-5" />
      </button>

      <div className="alert bg-warning/20 text-warning-content mt-16">
        <svg xmlns="http://www.w3.org/2000/svg" className="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
        </svg>
        <div>
          <p className="font-bold">AI Limitation Warning</p>
          <p className="text-sm">
            This chatbot uses TinyLlama and may occasionally provide inaccurate information. 
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
                    : 'chat-bubble-secondary rounded-2xl rounded-tl-sm'
                }`}
              >
                {message.content}
              </div>
            </div>
          ))}
          {isLoading && (
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
            placeholder="Ask me about Rayson's CV..."
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
