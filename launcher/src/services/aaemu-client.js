const axios = require('axios');
const crypto = require('crypto-js');

class AAEmuClient {
    constructor() {
        this.loginServerUrl = 'http://localhost:1237'; // URL do servidor de login AAEmu
        this.gameServerUrl = 'http://localhost:1234';  // URL do servidor de jogo AAEmu
        this.timeout = 10000; // 10 segundos de timeout
    }

    /**
     * Autentica o usuário no servidor AAEmu
     * @param {string} email - Email do usuário
     * @param {string} password - Senha do usuário
     * @returns {Promise<{success: boolean, message?: string, token?: string}>}
     */
    async authenticate(email, password) {
        try {
            // Criptografar a senha usando Base64 (como esperado pelo AAEmu)
            const hashedPassword = crypto.enc.Base64.stringify(crypto.enc.Utf8.parse(password));
            
            const response = await axios.post(`${this.loginServerUrl}/api/auth`, {
                username: email, // AAEmu usa username mas podemos mapear do email
                password: hashedPassword
            }, {
                timeout: this.timeout,
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (response.status === 200 && response.data.success) {
                return {
                    success: true,
                    token: response.data.token,
                    accountId: response.data.accountId
                };
            } else {
                return {
                    success: false,
                    message: response.data.message || 'Falha na autenticação'
                };
            }
        } catch (error) {
            console.error('Erro na autenticação:', error);
            
            if (error.code === 'ECONNREFUSED') {
                return {
                    success: false,
                    message: 'Não foi possível conectar ao servidor de login'
                };
            }
            
            if (error.response) {
                const status = error.response.status;
                switch (status) {
                    case 401:
                        return {
                            success: false,
                            message: 'Email ou senha incorretos'
                        };
                    case 403:
                        return {
                            success: false,
                            message: 'Conta banida ou suspensa'
                        };
                    case 500:
                        return {
                            success: false,
                            message: 'Erro interno do servidor'
                        };
                    default:
                        return {
                            success: false,
                            message: 'Erro desconhecido no servidor'
                        };
                }
            }
            
            return {
                success: false,
                message: 'Erro de conexão com o servidor'
            };
        }
    }

    /**
     * Verifica se o servidor de login está online
     * @returns {Promise<boolean>}
     */
    async checkLoginServerStatus() {
        try {
            const response = await axios.get(`${this.loginServerUrl}/api/status`, {
                timeout: 5000
            });
            return response.status === 200;
        } catch (error) {
            console.error('Erro ao verificar servidor de login:', error);
            return false;
        }
    }

    /**
     * Verifica se o servidor de jogo está online
     * @returns {Promise<boolean>}
     */
    async checkGameServerStatus() {
        try {
            const response = await axios.get(`${this.gameServerUrl}/api/status`, {
                timeout: 5000
            });
            return response.status === 200;
        } catch (error) {
            console.error('Erro ao verificar servidor de jogo:', error);
            return false;
        }
    }

    /**
     * Verifica se ambos os servidores estão online
     * @returns {Promise<{login: boolean, game: boolean, overall: boolean}>}
     */
    async checkServerStatus() {
        const [loginStatus, gameStatus] = await Promise.all([
            this.checkLoginServerStatus(),
            this.checkGameServerStatus()
        ]);

        return {
            login: loginStatus,
            game: gameStatus,
            overall: loginStatus && gameStatus
        };
    }

    /**
     * Obtém informações sobre atualizações disponíveis
     * @returns {Promise<{hasUpdate: boolean, version?: string, downloadUrl?: string, size?: number}>}
     */
    async checkForUpdates() {
        try {
            const response = await axios.get(`${this.loginServerUrl}/api/updates`, {
                timeout: this.timeout
            });

            if (response.status === 200) {
                return response.data;
            }
            
            return { hasUpdate: false };
        } catch (error) {
            console.error('Erro ao verificar atualizações:', error);
            return { hasUpdate: false };
        }
    }

    /**
     * Obtém a lista de servidores de jogo disponíveis
     * @returns {Promise<Array<{id: number, name: string, status: string, population: string}>>}
     */
    async getServerList() {
        try {
            const response = await axios.get(`${this.loginServerUrl}/api/servers`, {
                timeout: this.timeout
            });

            if (response.status === 200) {
                return response.data.servers || [];
            }
            
            return [];
        } catch (error) {
            console.error('Erro ao obter lista de servidores:', error);
            return [];
        }
    }

    /**
     * Verifica se a conta do usuário tem acesso
     * @param {string} token - Token de autenticação
     * @returns {Promise<{hasAccess: boolean, message?: string}>}
     */
    async verifyAccess(token) {
        try {
            const response = await axios.post(`${this.loginServerUrl}/api/verify`, {
                token
            }, {
                timeout: this.timeout,
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                }
            });

            return {
                hasAccess: response.status === 200 && response.data.valid,
                message: response.data.message
            };
        } catch (error) {
            console.error('Erro ao verificar acesso:', error);
            return {
                hasAccess: false,
                message: 'Erro na verificação de acesso'
            };
        }
    }

    /**
     * Configura as URLs dos servidores
     * @param {string} loginUrl - URL do servidor de login
     * @param {string} gameUrl - URL do servidor de jogo
     */
    setServerUrls(loginUrl, gameUrl) {
        this.loginServerUrl = loginUrl;
        this.gameServerUrl = gameUrl;
    }
}

module.exports = AAEmuClient;