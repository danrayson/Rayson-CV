const getDownloadUrl = () => {
  const baseUrl = import.meta.env.VITE_APP_DOWNLOAD_URL || '';
  const blobBaseUrl = baseUrl ? `${baseUrl}/$web` : '';
  const platform = navigator.platform.toLowerCase();
  const userAgent = navigator.userAgent.toLowerCase();
  
  if (platform.includes('mac') || userAgent.includes('mac')) {
    return `${blobBaseUrl}/RaysonCV.dmg`;
  }
  if (platform.includes('win') || userAgent.includes('win')) {
    return `${blobBaseUrl}/RaysonCV-Setup.exe`;
  }
  if (platform.includes('linux')) {
    return `${blobBaseUrl}/RaysonCV.AppImage`;
  }
  return null;
};

const LandingPage: React.FC = () => {
  const downloadUrl = getDownloadUrl();

  const handleAppClick = () => {
    if (!downloadUrl) {
      alert('Desktop app is not available for your operating system. Supported platforms: Windows, macOS, and Linux.');
    }
  };

  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-base-200 p-4">
      <h1 className="text-7xl font-bold mb-12 text-center">Design, Develop, Deploy</h1>
      
      <div className="flex flex-row gap-4 w-full max-w-6xl">
        <a
          href="/CV September 2024.pdf"
          download
          className="btn btn-primary aspect-square flex-1 text-2xl font-bold"
        >
          Download CV
        </a>

        {downloadUrl ? (
          <a
            href={downloadUrl}
            download
            className="btn btn-info aspect-square flex-1 text-2xl font-bold"
          >
            Download App
          </a>
        ) : (
          <button
            onClick={handleAppClick}
            className="btn btn-info aspect-square flex-1 text-2xl font-bold"
          >
            Download App
          </button>
        )}
        
        <a
          href="#/chatbot"
          className="btn btn-secondary aspect-square flex-1 text-2xl font-bold"
        >
          Chat with AI
        </a>
        
        <a
          href="https://github.com"
          target="_blank"
          rel="noopener noreferrer"
          className="btn btn-accent aspect-square flex-1 text-2xl font-bold"
        >
          View GitHub
        </a>
      </div>
    </div>
  );
};

export default LandingPage;
