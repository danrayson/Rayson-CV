import { useState, useEffect, useRef } from 'react';
import { loggingService } from '../services/loggingService';

interface PdfViewerModalProps {
  isOpen: boolean;
  onClose: () => void;
  pdfUrl: string;
}

type ToolbarSlot = {
  ShowSearchPopover?: any;
  GoToPreviousPage?: any;
  CurrentPageInput?: any;
  NumberOfPages?: any;
  GoToNextPage?: any;
  Zoom?: any;
  ZoomIn?: any;
  ZoomOut?: any;
  Download?: any;
  Print?: any;
  EnterFullScreen?: any;
  SwitchTheme?: any;
  [key: string]: any;
};

type TransformToolbarSlot = (slot: ToolbarSlot) => ToolbarSlot;

const transform: TransformToolbarSlot = (slot: ToolbarSlot) => {
  return {
    ...slot,
    Open: () => <></>,
    OpenMenuItem: () => <></>,
  };
};

interface PdfViewerInnerProps {
  pdfUrl: string;
  modules: {
    Viewer: any;
    Worker: any;
    defaultLayoutPlugin: any;
  };
}

const PdfViewerInner: React.FC<PdfViewerInnerProps> = ({ pdfUrl, modules }) => {
  const { Viewer, Worker, defaultLayoutPlugin } = modules;

  const defaultLayoutPluginInstance = defaultLayoutPlugin({
    renderToolbar: (Toolbar: any) => (
      <Toolbar>
        {(slots: ToolbarSlot) => {
          const transformedSlots = transform(slots);
          const {
            ShowSearchPopover,
            GoToPreviousPage,
            CurrentPageInput,
            NumberOfPages,
            GoToNextPage,
            Zoom,
            ZoomIn,
            ZoomOut,
            Download,
            Print,
            EnterFullScreen,
            SwitchTheme,
          } = transformedSlots;
          return (
            <div
              style={{
                display: 'flex',
                alignItems: 'center',
                width: '100%',
              }}
            >
              <div style={{ padding: '0 2px' }}>
                <ShowSearchPopover />
              </div>
              <div style={{ padding: '0 2px' }}>
                <GoToPreviousPage />
              </div>
              <div style={{ padding: '0 2px' }}>
                <CurrentPageInput />
              </div>
              <div style={{ padding: '0 2px' }}>
                <NumberOfPages />
              </div>
              <div style={{ padding: '0 2px' }}>
                <GoToNextPage />
              </div>
              <div style={{ padding: '0 2px', marginLeft: 'auto' }}>
                <ZoomOut />
              </div>
              <div style={{ padding: '0 2px' }}>
                <Zoom />
              </div>
              <div style={{ padding: '0 2px' }}>
                <ZoomIn />
              </div>
              <div style={{ padding: '0 2px' }}>
                <Download />
              </div>
              <div style={{ padding: '0 2px' }}>
                <Print />
              </div>
              <div style={{ padding: '0 2px' }}>
                <EnterFullScreen />
              </div>
              <div style={{ padding: '0 2px' }}>
                <SwitchTheme />
              </div>
            </div>
          );
        }}
      </Toolbar>
    ),
    sidebarTabs: () => [],
  });

  return (
    <Worker workerUrl="https://unpkg.com/pdfjs-dist@3.11.174/build/pdf.worker.min.js">
      <div style={{ width: '100%', height: '100%' }}>
        <Viewer
          fileUrl={pdfUrl}
          plugins={[defaultLayoutPluginInstance]}
        />
      </div>
    </Worker>
  );
};

const PdfViewerModal: React.FC<PdfViewerModalProps> = ({ isOpen, onClose, pdfUrl }) => {
  const dialogRef = useRef<HTMLDialogElement>(null);
  const [modules, setModules] = useState<{
    Viewer: any;
    Worker: any;
    defaultLayoutPlugin: any;
  } | null>(null);
  const isLoadingRef = useRef(false);

  useEffect(() => {
    if (!isOpen || isLoadingRef.current) return;

    isLoadingRef.current = true;

    const loadCss = () => {
      const cssUrls = [
        'https://unpkg.com/@react-pdf-viewer/core@3.12.0/lib/styles/index.css',
        'https://unpkg.com/@react-pdf-viewer/default-layout@3.12.0/lib/styles/index.css',
      ];

      cssUrls.forEach((href) => {
        if (!document.querySelector(`link[href="${href}"]`)) {
          const link = document.createElement('link');
          link.rel = 'stylesheet';
          link.href = href;
          document.head.appendChild(link);
        }
      });
    };

    const loadModules = async () => {
      try {
        const [core, layout] = await Promise.all([
          import('@react-pdf-viewer/core'),
          import('@react-pdf-viewer/default-layout'),
        ]);

        setModules({
          Viewer: core.Viewer,
          Worker: core.Worker,
          defaultLayoutPlugin: layout.defaultLayoutPlugin,
        });
      } catch (error) {
        console.error('Failed to load PDF viewer modules:', error);
      } finally {
        isLoadingRef.current = false;
      }
    };

    loadCss();
    loadModules();
  }, [isOpen]);

  useEffect(() => {
    const dialog = dialogRef.current;
    if (!dialog) return;

    if (isOpen) {
      dialog.showModal();
    } else {
      dialog.close();
    }
  }, [isOpen]);

  const handleBackdropClick = (e: React.MouseEvent) => {
    const dialog = dialogRef.current;
    if (!dialog) return;

    const rect = dialog.getBoundingClientRect();
    const isInDialog = (
      rect.top <= e.clientY &&
      e.clientY <= rect.top + rect.height &&
      rect.left <= e.clientX &&
      e.clientX <= rect.left + rect.width
    );

    if (!isInDialog) {
      dialog.close();
    }
  };

  return (
    <dialog
      ref={dialogRef}
      className="modal"
      onClick={handleBackdropClick}
      onClose={onClose}
    >
      <div className="modal-box w-full max-w-full h-[90vh] flex flex-col p-0 min-h-0">
        <div className="flex justify-between items-center p-4 border-b flex-shrink-0">
          <h3 className="font-bold text-lg">CV Preview</h3>
          <button
            data-track data-element-id="close-pdf"
            onClick={() => {
              loggingService.logClick('close-pdf', dialogRef.current?.textContent ?? '');
              dialogRef.current?.close();
            }}
            className="btn btn-sm btn-circle btn-ghost"
          >
            ✕
          </button>
        </div>
        <div className="flex-1 overflow-hidden min-h-0" style={{ width: '100%', height: '100%' }}>
          {!modules ? (
            <div className="flex justify-center items-center h-full">
              <span className="loading loading-spinner loading-lg"></span>
            </div>
          ) : (
            <PdfViewerInner modules={modules} pdfUrl={pdfUrl} />
          )}
        </div>
      </div>
    </dialog>
  );
};

export default PdfViewerModal;
