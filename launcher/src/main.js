const { app, BrowserWindow, ipcMain, dialog, shell } = require('electron');
const path = require('path');
const fs = require('fs-extra');
const Store = require('electron-store');
const { autoUpdater } = require('electron-updater');

const store = new Store();

let mainWindow;
let isDev = process.argv.includes('--dev');

function createWindow() {
  mainWindow = new BrowserWindow({
    width: 1200,
    height: 800,
    minWidth: 1000,
    minHeight: 700,
    frame: false,
    icon: path.join(__dirname, '../assets/icon.png'),
    webPreferences: {
      nodeIntegration: false,
      contextIsolation: true,
      enableRemoteModule: false,
      preload: path.join(__dirname, 'preload.js')
    }
  });

  mainWindow.loadFile(path.join(__dirname, 'renderer/index.html'));

  if (isDev) {
    mainWindow.webContents.openDevTools();
  }

  // Evitar que o usuário navegue para URLs externas
  mainWindow.webContents.on('will-navigate', (event, navigationUrl) => {
    const parsedUrl = new URL(navigationUrl);
    if (parsedUrl.origin !== 'file://') {
      event.preventDefault();
    }
  });

  mainWindow.webContents.setWindowOpenHandler(({ url }) => {
    shell.openExternal(url);
    return { action: 'deny' };
  });
}

app.whenReady().then(() => {
  createWindow();

  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) {
      createWindow();
    }
  });

  // Configurar auto-updater
  if (!isDev) {
    autoUpdater.checkForUpdatesAndNotify();
  }
});

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit();
  }
});

// IPC Handlers
ipcMain.handle('app-version', () => {
  return app.getVersion();
});

ipcMain.handle('minimize-window', () => {
  mainWindow.minimize();
});

ipcMain.handle('close-window', () => {
  app.quit();
});

ipcMain.handle('get-install-path', () => {
  return store.get('installPath', path.join(require('os').homedir(), 'ArcheageBR'));
});

ipcMain.handle('set-install-path', async () => {
  const result = await dialog.showOpenDialog(mainWindow, {
    properties: ['openDirectory'],
    title: 'Selecione o diretório de instalação'
  });

  if (!result.canceled && result.filePaths.length > 0) {
    const selectedPath = path.join(result.filePaths[0], 'ArcheageBR');
    store.set('installPath', selectedPath);
    return selectedPath;
  }
  
  return null;
});

ipcMain.handle('get-user-credentials', () => {
  return {
    email: store.get('email', ''),
    rememberMe: store.get('rememberMe', false)
  };
});

ipcMain.handle('save-user-credentials', (event, { email, rememberMe }) => {
  store.set('email', email);
  store.set('rememberMe', rememberMe);
});

ipcMain.handle('check-game-installation', async () => {
  const installPath = store.get('installPath', path.join(require('os').homedir(), 'ArcheageBR'));
  const gameExePath = path.join(installPath, 'bin', 'archeage.exe');
  
  try {
    await fs.access(gameExePath);
    return true;
  } catch {
    return false;
  }
});

ipcMain.handle('open-external', (event, url) => {
  shell.openExternal(url);
});

// Auto-updater events
autoUpdater.on('checking-for-update', () => {
  mainWindow.webContents.send('updater-checking');
});

autoUpdater.on('update-available', (info) => {
  mainWindow.webContents.send('updater-update-available', info);
});

autoUpdater.on('update-not-available', (info) => {
  mainWindow.webContents.send('updater-update-not-available', info);
});

autoUpdater.on('error', (err) => {
  mainWindow.webContents.send('updater-error', err);
});

autoUpdater.on('download-progress', (progressObj) => {
  mainWindow.webContents.send('updater-download-progress', progressObj);
});

autoUpdater.on('update-downloaded', (info) => {
  mainWindow.webContents.send('updater-update-downloaded', info);
});

process.on('uncaughtException', (error) => {
  console.error('Uncaught Exception:', error);
});

process.on('unhandledRejection', (reason, promise) => {
  console.error('Unhandled Rejection at:', promise, 'reason:', reason);
});