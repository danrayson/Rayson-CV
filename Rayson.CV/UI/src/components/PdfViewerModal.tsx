import { useEffect, useRef } from 'react';
import { Viewer, Worker } from '@react-pdf-viewer/core';
import { defaultLayoutPlugin } from '@react-pdf-viewer/default-layout';
import type { ToolbarSlot, TransformToolbarSlot } from '@react-pdf-viewer/toolbar';
import '@react-pdf-viewer/core/lib/styles/index.css';
import '@react-pdf-viewer/default-layout/lib/styles/index.css';

interface PdfViewerModalProps {
  isOpen: boolean;
  onClose: () => void;
  pdfUrl: string;
}

const transform: TransformToolbarSlot = (slot: ToolbarSlot) => {
  return {
    ...slot,
    Open: () => <></>,
    OpenMenuItem: () => <></>,
  };
};

const PdfViewerModal: React.FC<PdfViewerModalProps> = ({ isOpen, onClose, pdfUrl }) => {
  const dialogRef = useRef<HTMLDialogElement>(null);
  const defaultLayoutPluginInstance = defaultLayoutPlugin({
    renderToolbar: (Toolbar) => (
      <Toolbar>
        {(slots) => {
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
            onClick={() => dialogRef.current?.close()}
            className="btn btn-sm btn-circle btn-ghost"
          >
            ✕
          </button>
        </div>
        <div className="flex-1 overflow-hidden min-h-0" style={{ width: '100%', height: '100%' }}>
          <Worker workerUrl="https://unpkg.com/pdfjs-dist@3.11.174/build/pdf.worker.min.js">
            <div style={{ width: '100%', height: '100%' }}>
              <Viewer
                fileUrl={pdfUrl}
                plugins={[defaultLayoutPluginInstance]}
              />
            </div>
          </Worker>
        </div>
      </div>
    </dialog>
  );
};

export default PdfViewerModal;
