import { useState } from 'react';
import PdfViewerModal from '../components/PdfViewerModal';

const getAppDownloadUrl = () => {
  const baseUrl = import.meta.env.VITE_API_BASE_URL || '';
  const platform = navigator.platform.toLowerCase();
  const userAgent = navigator.userAgent.toLowerCase();
  
  if (platform.includes('mac') || userAgent.includes('mac')) {
    return `${baseUrl}files/app/mac`;
  }
  if (platform.includes('win') || userAgent.includes('win')) {
    return `${baseUrl}files/app/win`;
  }
  if (platform.includes('linux')) {
    return `${baseUrl}files/app/linux`;
  }
  return null;
};

const getCvDownloadUrl = () => {
  const baseUrl = import.meta.env.VITE_API_BASE_URL || '';
  return `${baseUrl}files/cv`;
};

const LandingPage: React.FC = () => {
  const appDownloadUrl = getAppDownloadUrl();
  const cvDownloadUrl = getCvDownloadUrl();
  const [isPdfModalOpen, setIsPdfModalOpen] = useState(false);

  const handleAppClick = () => {
    if (!appDownloadUrl) {
      alert('Desktop app is not available for your operating system. Supported platforms: Windows, macOS, and Linux.');
    }
  };

  return (
    <div className="flex flex-col min-h-screen bg-base-200 p-4">
      <div className="flex-grow flex flex-col items-center justify-center">
        <h1 className="text-7xl font-bold mb-12 text-center">{"Design Develop Deploy"}</h1>
        
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 w-full max-w-6xl">
          <button
            onClick={() => setIsPdfModalOpen(true)}
            className="btn btn-primary text-2xl font-bold"
          >
            View CV
          </button>

          {appDownloadUrl ? (
            <a
              href={appDownloadUrl}
              download
              className="btn btn-primary text-2xl font-bold"
            >
              Download App
            </a>
          ) : (
            <button
              onClick={handleAppClick}
              className="btn btn-disabled text-2xl font-bold"
            >
              Download Unavailable
            </button>
          )}
          
          <a
            href="#/chatbot"
            className="btn btn-primary text-2xl font-bold"
          >
            Chat with AI
          </a>
          
          <a
            href="https://github.com/danrayson/Rayson-CV"
            target="_blank"
            rel="noopener noreferrer"
            className="btn btn-primary text-2xl font-bold"
          >
            View GitHub
          </a>
        </div>
      </div>
      <p className="text-sm opacity-30">Dan Rayson's CV Website</p>

      <PdfViewerModal
        isOpen={isPdfModalOpen}
        onClose={() => setIsPdfModalOpen(false)}
        pdfUrl={cvDownloadUrl}
      />
    </div>
  );
};

export default LandingPage;
