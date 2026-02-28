const { app, BrowserWindow } = require('electron');
const path = require('path');

console.log('starting main');

function createWindow() {
  console.log('Creating window');
  const win = new BrowserWindow({
    width: 800,
    height: 600,
    webPreferences: {
      preload: path.join(__dirname, 'preload.cjs'),
      nodeIntegration: true,
      contextIsolation: true,
    },
  });
  console.log(path.join(__dirname, '..', 'dist', 'index.html'));
  win.loadFile(path.join(__dirname, '..', 'dist', 'index.html'));
}

while(!app.isReady){
  console.log('waiting for app to be ready');
}
app.whenReady().then(createWindow);

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    console.log('quit')
    app.quit();
  }
});

app.on('activate', () => {
  console.log('activating...')
  if (BrowserWindow.getAllWindows().length === 0) {
    createWindow();
    console.log('activated')
  }
});