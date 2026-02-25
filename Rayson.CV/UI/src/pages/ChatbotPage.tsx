import { ArrowLeftIcon } from '@heroicons/react/24/outline';

const ChatbotPage: React.FC = () => {
  return (
    <div className="relative min-h-screen bg-base-200">
      <button
        onClick={() => window.history.back()}
        className="absolute top-8 left-8 btn btn-lg"
      >
        <ArrowLeftIcon className="w-6 h-6 mr-2" />
        Home
      </button>

      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <h1 className="text-4xl font-bold mb-4">Chat with AI</h1>
          <p className="text-lg">Coming soon! This feature will allow you to ask questions about my CV.</p>
        </div>
      </div>
    </div>
  );
};

export default ChatbotPage;
