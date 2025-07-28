const fs = require('fs-extra');
const path = require('path');
const axios = require('axios');
const StreamZip = require('node-stream-zip');
const { EventEmitter } = require('events');

class GameUpdater extends EventEmitter {
    constructor(installPath) {
        super();
        this.installPath = installPath;
        this.downloadActive = false;
        this.downloadSpeed = 0;
        this.currentProgress = 0;
    }

    /**
     * Verifica se há atualizações disponíveis
     * @returns {Promise<{hasUpdate: boolean, version?: string, downloadUrl?: string, size?: number}>}
     */
    async checkForUpdates() {
        try {
            // URL do servidor de atualizações (você deve configurar isso)
            const updateServerUrl = 'http://localhost:8080/api/updates';
            
            const response = await axios.get(updateServerUrl, {
                timeout: 10000
            });

            if (response.status === 200) {
                const updateInfo = response.data;
                const currentVersion = await this.getCurrentVersion();
                
                return {
                    hasUpdate: updateInfo.version !== currentVersion,
                    version: updateInfo.version,
                    downloadUrl: updateInfo.downloadUrl,
                    size: updateInfo.size,
                    changelog: updateInfo.changelog
                };
            }
            
            return { hasUpdate: false };
        } catch (error) {
            console.error('Erro ao verificar atualizações:', error);
            return { hasUpdate: false };
        }
    }

    /**
     * Obtém a versão atual do jogo instalado
     * @returns {Promise<string>}
     */
    async getCurrentVersion() {
        try {
            const versionFile = path.join(this.installPath, 'version.txt');
            
            if (await fs.pathExists(versionFile)) {
                const version = await fs.readFile(versionFile, 'utf8');
                return version.trim();
            }
            
            return '0.0.0';
        } catch (error) {
            console.error('Erro ao obter versão atual:', error);
            return '0.0.0';
        }
    }

    /**
     * Baixa e instala uma atualização do jogo
     * @param {string} downloadUrl - URL do arquivo de atualização
     * @param {number} totalSize - Tamanho total do download
     * @returns {Promise<boolean>}
     */
    async downloadUpdate(downloadUrl, totalSize) {
        if (this.downloadActive) {
            throw new Error('Download já está em progresso');
        }

        this.downloadActive = true;
        this.currentProgress = 0;
        
        try {
            await fs.ensureDir(this.installPath);
            
            const fileName = path.basename(downloadUrl);
            const downloadPath = path.join(this.installPath, 'temp', fileName);
            
            await fs.ensureDir(path.dirname(downloadPath));
            
            const success = await this.downloadFile(downloadUrl, downloadPath, totalSize);
            
            if (success) {
                this.emit('extracting');
                await this.extractUpdate(downloadPath);
                await fs.remove(path.dirname(downloadPath)); // Remover pasta temp
                this.emit('completed');
                return true;
            }
            
            return false;
        } catch (error) {
            console.error('Erro no download/instalação:', error);
            this.emit('error', error);
            return false;
        } finally {
            this.downloadActive = false;
        }
    }

    /**
     * Baixa um arquivo com progresso
     * @param {string} url - URL do arquivo
     * @param {string} filePath - Caminho onde salvar
     * @param {number} totalSize - Tamanho total esperado
     * @returns {Promise<boolean>}
     */
    async downloadFile(url, filePath, totalSize) {
        return new Promise((resolve, reject) => {
            const writer = fs.createWriteStream(filePath);
            let downloadedBytes = 0;
            let lastTime = Date.now();
            let lastBytes = 0;

            axios({
                method: 'GET',
                url: url,
                responseType: 'stream',
                timeout: 30000
            }).then(response => {
                const stream = response.data;
                
                stream.on('data', (chunk) => {
                    downloadedBytes += chunk.length;
                    const currentTime = Date.now();
                    const timeDiff = currentTime - lastTime;
                    
                    // Calcular velocidade a cada segundo
                    if (timeDiff >= 1000) {
                        const bytesDiff = downloadedBytes - lastBytes;
                        this.downloadSpeed = bytesDiff / (timeDiff / 1000);
                        lastTime = currentTime;
                        lastBytes = downloadedBytes;
                    }
                    
                    this.currentProgress = (downloadedBytes / totalSize) * 100;
                    
                    this.emit('progress', {
                        percentage: this.currentProgress,
                        downloadedBytes,
                        totalBytes: totalSize,
                        speed: this.downloadSpeed
                    });
                });
                
                stream.pipe(writer);
                
                writer.on('finish', () => {
                    resolve(true);
                });
                
                writer.on('error', (error) => {
                    reject(error);
                });
                
            }).catch(error => {
                reject(error);
            });
        });
    }

    /**
     * Extrai um arquivo de atualização
     * @param {string} archivePath - Caminho do arquivo compactado
     */
    async extractUpdate(archivePath) {
        return new Promise((resolve, reject) => {
            const zip = new StreamZip.async({ file: archivePath });
            
            zip.extract(null, this.installPath)
                .then(() => {
                    resolve();
                })
                .catch(error => {
                    reject(error);
                })
                .finally(() => {
                    zip.close();
                });
        });
    }

    /**
     * Verifica se o jogo está instalado
     * @returns {Promise<boolean>}
     */
    async isGameInstalled() {
        try {
            const gameExePath = path.join(this.installPath, 'bin', 'archeage.exe');
            return await fs.pathExists(gameExePath);
        } catch (error) {
            return false;
        }
    }

    /**
     * Instala o jogo completo (primeira instalação)
     * @param {string} downloadUrl - URL do instalador completo
     * @param {number} totalSize - Tamanho total do download
     * @returns {Promise<boolean>}
     */
    async installGame(downloadUrl, totalSize) {
        this.emit('installing');
        return await this.downloadUpdate(downloadUrl, totalSize);
    }

    /**
     * Cancela o download/instalação atual
     */
    cancelDownload() {
        if (this.downloadActive) {
            this.downloadActive = false;
            this.emit('cancelled');
        }
    }

    /**
     * Obtém informações sobre o espaço em disco
     * @returns {Promise<{free: number, total: number}>}
     */
    async getDiskSpace() {
        try {
            const stats = await fs.stat(this.installPath);
            // Nota: fs.stat não fornece informações de espaço livre
            // Para isso, seria necessária uma biblioteca adicional como 'node-disk-info'
            return {
                free: 0, // Placeholder
                total: 0 // Placeholder
            };
        } catch (error) {
            return { free: 0, total: 0 };
        }
    }

    /**
     * Atualiza a versão instalada
     * @param {string} version - Nova versão
     */
    async updateVersion(version) {
        try {
            const versionFile = path.join(this.installPath, 'version.txt');
            await fs.writeFile(versionFile, version, 'utf8');
        } catch (error) {
            console.error('Erro ao atualizar versão:', error);
        }
    }
}

module.exports = GameUpdater;