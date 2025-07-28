const { contextBridge, ipcRenderer } = require('electron');

// Expor APIs seguras para o renderer
contextBridge.exposeInMainWorld('electronAPI', {
  // Aplicação
  getAppVersion: () => ipcRenderer.invoke('app-version'),
  minimizeWindow: () => ipcRenderer.invoke('minimize-window'),
  closeWindow: () => ipcRenderer.invoke('close-window'),
  
  // Instalação
  getInstallPath: () => ipcRenderer.invoke('get-install-path'),
  setInstallPath: () => ipcRenderer.invoke('set-install-path'),
  checkGameInstallation: () => ipcRenderer.invoke('check-game-installation'),
  
  // Credenciais do usuário
  getUserCredentials: () => ipcRenderer.invoke('get-user-credentials'),
  saveUserCredentials: (credentials) => ipcRenderer.invoke('save-user-credentials', credentials),
  
  // Links externos
  openExternal: (url) => ipcRenderer.invoke('open-external', url),
  
  // Auto-updater listeners
  onUpdaterChecking: (callback) => ipcRenderer.on('updater-checking', callback),
  onUpdaterUpdateAvailable: (callback) => ipcRenderer.on('updater-update-available', callback),
  onUpdaterUpdateNotAvailable: (callback) => ipcRenderer.on('updater-update-not-available', callback),
  onUpdaterError: (callback) => ipcRenderer.on('updater-error', callback),
  onUpdaterDownloadProgress: (callback) => ipcRenderer.on('updater-download-progress', callback),
  onUpdaterUpdateDownloaded: (callback) => ipcRenderer.on('updater-update-downloaded', callback),
  
  // Remover listeners
  removeAllListeners: (channel) => ipcRenderer.removeAllListeners(channel)
});

// API para comunicação com o servidor
contextBridge.exposeInMainWorld('serverAPI', {
  // Configurações do servidor (você deve ajustar esses valores)
  config: {
    loginServerUrl: 'http://localhost:1237', // URL do servidor de login AAEmu
    gameServerUrl: 'http://localhost:1234',  // URL do servidor de jogo AAEmu
    updateServerUrl: 'http://localhost:8080', // URL do servidor de atualizações
    websiteUrl: 'https://archeagebr.com',    // URL do site do servidor
    discordUrl: 'https://discord.gg/your-server' // URL do Discord
  }
});