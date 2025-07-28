// Estado global da aplicação
const AppState = {
    isLoggedIn: false,
    serverOnline: false,
    gameInstalled: false,
    downloading: false,
    currentUser: null,
    installPath: null
};

// Utilitários
const Utils = {
    formatBytes(bytes) {
        if (bytes === 0) return '0 Bytes';
        const k = 1024;
        const sizes = ['Bytes', 'KB', 'MB', 'GB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
    },

    formatSpeed(bytesPerSecond) {
        return this.formatBytes(bytesPerSecond) + '/s';
    },

    showNotification(message, type = 'info', duration = 5000) {
        const container = document.getElementById('notifications');
        const notification = document.createElement('div');
        notification.className = `notification ${type}`;
        notification.textContent = message;
        
        container.appendChild(notification);
        
        setTimeout(() => {
            notification.style.animation = 'slideIn 0.3s ease reverse';
            setTimeout(() => {
                if (notification.parentNode) {
                    notification.parentNode.removeChild(notification);
                }
            }, 300);
        }, duration);
    },

    showModal(title, content, onConfirm = null, onCancel = null) {
        const overlay = document.getElementById('modalOverlay');
        const titleEl = document.getElementById('modalTitle');
        const bodyEl = document.getElementById('modalBody');
        const confirmBtn = document.getElementById('modalConfirm');
        const cancelBtn = document.getElementById('modalCancel');
        
        titleEl.textContent = title;
        bodyEl.innerHTML = content;
        overlay.style.display = 'flex';
        
        const hideModal = () => {
            overlay.style.display = 'none';
        };
        
        confirmBtn.onclick = () => {
            hideModal();
            if (onConfirm) onConfirm();
        };
        
        cancelBtn.onclick = () => {
            hideModal();
            if (onCancel) onCancel();
        };
        
        document.getElementById('modalClose').onclick = hideModal;
    }
};

// Gerenciador de autenticação
const AuthManager = {
    async login(email, password) {
        try {
            // Simular chamada para o servidor AAEmu
            // Em produção, você deve implementar a comunicação real com o servidor
            const response = await this.authenticateWithServer(email, password);
            
            if (response.success) {
                AppState.isLoggedIn = true;
                AppState.currentUser = { email };
                
                // Salvar credenciais se solicitado
                const rememberMe = document.getElementById('rememberMe').checked;
                await window.electronAPI.saveUserCredentials({ email, rememberMe });
                
                Utils.showNotification('Login realizado com sucesso!', 'success');
                this.updateUI();
                return true;
            } else {
                Utils.showNotification(response.message || 'Erro no login', 'error');
                return false;
            }
        } catch (error) {
            console.error('Erro no login:', error);
            Utils.showNotification('Erro de conexão com o servidor', 'error');
            return false;
        }
    },

    async authenticateWithServer(email, password) {
        // Implementar comunicação real com o servidor AAEmu
        // Por enquanto, vamos simular uma autenticação
        return new Promise((resolve) => {
            setTimeout(() => {
                // Simular validação - em produção, conectar ao servidor real
                if (email && password) {
                    resolve({ success: true, user: { email } });
                } else {
                    resolve({ success: false, message: 'Email ou senha inválidos' });
                }
            }, 1500);
        });
    },

    updateUI() {
        const loginBtn = document.getElementById('loginBtn');
        const playBtn = document.getElementById('playBtn');
        
        if (AppState.isLoggedIn) {
            loginBtn.innerHTML = '<span class="btn-text">Conectado</span>';
            loginBtn.disabled = true;
            
            if (AppState.serverOnline && AppState.gameInstalled) {
                playBtn.disabled = false;
            }
        }
    }
};

// Gerenciador de servidor
const ServerManager = {
    async checkServerStatus() {
        try {
            const statusDot = document.querySelector('.status-dot');
            const statusText = document.querySelector('.status-text');
            
            statusDot.className = 'status-dot checking';
            statusText.textContent = 'Verificando servidor...';
            
            // Implementar verificação real do servidor AAEmu
            const isOnline = await this.pingServer();
            
            if (isOnline) {
                statusDot.className = 'status-dot online';
                statusText.textContent = 'Servidor Online';
                AppState.serverOnline = true;
                
                if (AppState.isLoggedIn && AppState.gameInstalled) {
                    document.getElementById('playBtn').disabled = false;
                }
            } else {
                statusDot.className = 'status-dot offline';
                statusText.textContent = 'Servidor Offline';
                AppState.serverOnline = false;
                document.getElementById('playBtn').disabled = true;
            }
        } catch (error) {
            console.error('Erro ao verificar servidor:', error);
            const statusDot = document.querySelector('.status-dot');
            const statusText = document.querySelector('.status-text');
            statusDot.className = 'status-dot offline';
            statusText.textContent = 'Erro na verificação';
        }
    },

    async pingServer() {
        // Implementar ping real ao servidor AAEmu
        // Por enquanto, vamos simular
        return new Promise((resolve) => {
            setTimeout(() => {
                // Simular que o servidor está online 80% das vezes
                resolve(Math.random() > 0.2);
            }, 2000);
        });
    }
};

// Gerenciador de instalação e atualizações
const InstallManager = {
    async checkInstallation() {
        try {
            const isInstalled = await window.electronAPI.checkGameInstallation();
            AppState.gameInstalled = isInstalled;
            
            const clientVersionEl = document.getElementById('clientVersion');
            
            if (isInstalled) {
                clientVersionEl.textContent = 'v1.0.0 (Instalado)';
                
                if (AppState.isLoggedIn && AppState.serverOnline) {
                    document.getElementById('playBtn').disabled = false;
                }
            } else {
                clientVersionEl.textContent = 'Não instalado';
                this.showInstallOption();
            }
        } catch (error) {
            console.error('Erro ao verificar instalação:', error);
            document.getElementById('clientVersion').textContent = 'Erro na verificação';
        }
    },

    showInstallOption() {
        const playBtn = document.getElementById('playBtn');
        playBtn.innerHTML = '<span class="btn-text">Instalar Jogo</span>';
        playBtn.disabled = false;
        
        playBtn.onclick = () => {
            this.startInstallation();
        };
    },

    async startInstallation() {
        if (!AppState.isLoggedIn) {
            Utils.showNotification('Você precisa fazer login primeiro', 'warning');
            return;
        }

        Utils.showModal(
            'Confirmar Instalação',
            'Deseja iniciar o download e instalação do ArcheAge BR? Este processo pode demorar algumas horas dependendo da sua conexão.',
            () => {
                this.downloadGame();
            }
        );
    },

    async downloadGame() {
        AppState.downloading = true;
        const progressContainer = document.getElementById('downloadProgress');
        const progressFill = document.getElementById('progressFill');
        const progressPercentage = document.querySelector('.progress-percentage');
        const progressSpeed = document.querySelector('.progress-speed');
        const progressSize = document.querySelector('.progress-size');
        const playBtn = document.getElementById('playBtn');
        
        progressContainer.style.display = 'block';
        playBtn.disabled = true;
        playBtn.innerHTML = '<span class="btn-text">Baixando...</span>';
        
        // Simular download
        let progress = 0;
        const totalSize = 15 * 1024 * 1024 * 1024; // 15GB
        
        const updateProgress = () => {
            if (progress < 100 && AppState.downloading) {
                progress += Math.random() * 2;
                if (progress > 100) progress = 100;
                
                const currentSize = (totalSize * progress) / 100;
                const speed = Math.random() * 10 * 1024 * 1024; // Velocidade aleatória
                
                progressFill.style.width = `${progress}%`;
                progressPercentage.textContent = `${Math.round(progress)}%`;
                progressSpeed.textContent = Utils.formatSpeed(speed);
                progressSize.textContent = `${Utils.formatBytes(currentSize)} / ${Utils.formatBytes(totalSize)}`;
                
                if (progress >= 100) {
                    this.finishInstallation();
                } else {
                    setTimeout(updateProgress, 100 + Math.random() * 200);
                }
            }
        };
        
        updateProgress();
    },

    finishInstallation() {
        AppState.downloading = false;
        AppState.gameInstalled = true;
        
        const progressContainer = document.getElementById('downloadProgress');
        const playBtn = document.getElementById('playBtn');
        
        progressContainer.style.display = 'none';
        playBtn.innerHTML = '<span class="btn-text">Jogar</span><div class="btn-glow"></div>';
        playBtn.onclick = () => GameManager.launchGame();
        
        if (AppState.serverOnline) {
            playBtn.disabled = false;
        }
        
        Utils.showNotification('Jogo instalado com sucesso!', 'success');
        this.checkInstallation();
    }
};

// Gerenciador de execução do jogo
const GameManager = {
    async launchGame() {
        if (!AppState.isLoggedIn) {
            Utils.showNotification('Você precisa fazer login primeiro', 'warning');
            return;
        }
        
        if (!AppState.serverOnline) {
            Utils.showNotification('Servidor está offline', 'error');
            return;
        }
        
        if (!AppState.gameInstalled) {
            Utils.showNotification('Jogo não está instalado', 'error');
            return;
        }
        
        try {
            Utils.showNotification('Iniciando o jogo...', 'info');
            
            // Aqui você implementaria a execução real do cliente do jogo
            // Por exemplo, usando child_process no processo principal
            console.log('Lançando o jogo ArcheAge...');
            
            // Simular inicialização
            setTimeout(() => {
                Utils.showNotification('Jogo iniciado com sucesso!', 'success');
            }, 2000);
            
        } catch (error) {
            console.error('Erro ao iniciar o jogo:', error);
            Utils.showNotification('Erro ao iniciar o jogo', 'error');
        }
    }
};

// Inicialização da aplicação
document.addEventListener('DOMContentLoaded', async () => {
    // Configurar controles da janela
    document.getElementById('minimizeBtn').addEventListener('click', () => {
        window.electronAPI.minimizeWindow();
    });
    
    document.getElementById('closeBtn').addEventListener('click', () => {
        window.electronAPI.closeWindow();
    });
    
    // Configurar formulário de login
    document.getElementById('loginForm').addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const email = document.getElementById('email').value;
        const password = document.getElementById('password').value;
        const loginBtn = document.getElementById('loginBtn');
        
        if (!email || !password) {
            Utils.showNotification('Preencha todos os campos', 'warning');
            return;
        }
        
        // Mostrar loading
        loginBtn.querySelector('.btn-text').style.display = 'none';
        loginBtn.querySelector('.btn-loading').style.display = 'flex';
        loginBtn.disabled = true;
        
        const success = await AuthManager.login(email, password);
        
        // Esconder loading
        loginBtn.querySelector('.btn-text').style.display = 'block';
        loginBtn.querySelector('.btn-loading').style.display = 'none';
        
        if (!success) {
            loginBtn.disabled = false;
        }
    });
    
    // Configurar botão de alterar diretório de instalação
    document.getElementById('changeInstallPath').addEventListener('click', async () => {
        const newPath = await window.electronAPI.setInstallPath();
        if (newPath) {
            document.getElementById('installPath').textContent = newPath;
            AppState.installPath = newPath;
            Utils.showNotification('Diretório de instalação alterado', 'success');
        }
    });
    
    // Configurar links do rodapé
    document.getElementById('websiteLink').addEventListener('click', (e) => {
        e.preventDefault();
        window.electronAPI.openExternal(window.serverAPI.config.websiteUrl);
    });
    
    document.getElementById('discordLink').addEventListener('click', (e) => {
        e.preventDefault();
        window.electronAPI.openExternal(window.serverAPI.config.discordUrl);
    });
    
    document.getElementById('supportLink').addEventListener('click', (e) => {
        e.preventDefault();
        window.electronAPI.openExternal(window.serverAPI.config.websiteUrl + '/support');
    });
    
    document.getElementById('forgotPassword').addEventListener('click', (e) => {
        e.preventDefault();
        window.electronAPI.openExternal(window.serverAPI.config.websiteUrl + '/forgot-password');
    });
    
    document.getElementById('createAccount').addEventListener('click', (e) => {
        e.preventDefault();
        window.electronAPI.openExternal(window.serverAPI.config.websiteUrl + '/register');
    });
    
    // Configurar botão de jogar
    document.getElementById('playBtn').addEventListener('click', () => {
        GameManager.launchGame();
    });
    
    // Carregar informações iniciais
    try {
        // Carregar versão da aplicação
        const version = await window.electronAPI.getAppVersion();
        document.getElementById('appVersion').textContent = version;
        
        // Carregar credenciais salvas
        const credentials = await window.electronAPI.getUserCredentials();
        if (credentials.rememberMe && credentials.email) {
            document.getElementById('email').value = credentials.email;
            document.getElementById('rememberMe').checked = credentials.rememberMe;
        }
        
        // Carregar caminho de instalação
        const installPath = await window.electronAPI.getInstallPath();
        document.getElementById('installPath').textContent = installPath;
        AppState.installPath = installPath;
        
        // Verificar status inicial
        await Promise.all([
            ServerManager.checkServerStatus(),
            InstallManager.checkInstallation()
        ]);
        
        // Configurar verificação periódica do servidor
        setInterval(() => {
            ServerManager.checkServerStatus();
        }, 30000); // Verificar a cada 30 segundos
        
    } catch (error) {
        console.error('Erro na inicialização:', error);
        Utils.showNotification('Erro ao carregar configurações', 'error');
    }
    
    // Configurar listeners do auto-updater
    window.electronAPI.onUpdaterUpdateAvailable(() => {
        Utils.showNotification('Nova atualização disponível!', 'info');
    });
    
    window.electronAPI.onUpdaterDownloadProgress((event, progress) => {
        Utils.showNotification(`Baixando atualização: ${Math.round(progress.percent)}%`, 'info');
    });
    
    window.electronAPI.onUpdaterUpdateDownloaded(() => {
        Utils.showModal(
            'Atualização Pronta',
            'A atualização foi baixada. Deseja reiniciar o launcher para aplicá-la?',
            () => {
                // Aqui você implementaria o reinício da aplicação
                window.electronAPI.closeWindow();
            }
        );
    });
    
    // Mostrar mensagem de boas-vindas
    setTimeout(() => {
        Utils.showNotification('Bem-vindo ao ArcheAge BR: O Sonho!', 'success');
    }, 1000);
});

// Tratamento de erros globais
window.addEventListener('error', (event) => {
    console.error('Erro global:', event.error);
    Utils.showNotification('Ocorreu um erro inesperado', 'error');
});

window.addEventListener('unhandledrejection', (event) => {
    console.error('Promise rejeitada:', event.reason);
    Utils.showNotification('Erro de comunicação', 'error');
});