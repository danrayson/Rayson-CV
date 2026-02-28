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
    <div className="flex flex-col min-h-screen bg-base-200 p-4">
      <div className="flex-grow flex flex-col items-center justify-center">
        <h1 className="text-7xl font-bold mb-12 text-center">{"Design Develop Deploy"}</h1>
        
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 w-full max-w-6xl">
          <a
            href="/CV September 2024.pdf"
            download
            className="btn btn-primary text-2xl font-bold"
          >
            Download CV
          </a>

          {downloadUrl ? (
            <a
              href={downloadUrl}
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
    </div>
  );
};

export default LandingPage;
