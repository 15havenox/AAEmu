# ArcheAge BR: O Sonho! - Launcher

Launcher oficial para o servidor privado **ArcheAge BR: O Sonho!** baseado no projeto AAEmu.

## 🎮 Características

- **Interface Moderna**: Design inspirado no ArcheAge com tema dark e elementos dourados
- **Login Seguro**: Autenticação por email compatível com o servidor AAEmu
- **Status do Servidor**: Verificação em tempo real do status dos servidores
- **Sistema de Atualizações**: Download automático de atualizações e primeira instalação
- **Indicador de Progresso**: Barra de progresso detalhada com velocidade de download
- **Gerenciamento de Instalação**: Escolha do diretório de instalação
- **Auto-Updater**: Atualizações automáticas do próprio launcher
- **Multiplataforma**: Compatível com Windows, macOS e Linux

## 🚀 Instalação para Desenvolvimento

### Pré-requisitos

- Node.js 18.x ou superior
- npm ou yarn
- Git

### Clonando e Configurando

```bash
# Clone o repositório
git clone https://github.com/seu-usuario/archeage-br-launcher.git
cd archeage-br-launcher/launcher

# Instale as dependências
npm install

# Execute em modo de desenvolvimento
npm run dev
```

### Scripts Disponíveis

```bash
# Executar em modo de desenvolvimento
npm run dev

# Executar em modo de produção
npm start

# Construir para distribuição
npm run build

# Construir apenas para Windows
npm run build-win

# Empacotar sem distribuir
npm run pack
```

## 🔧 Configuração

### Configuração do Servidor

Edite o arquivo `src/preload.js` para configurar as URLs do seu servidor:

```javascript
config: {
    loginServerUrl: 'http://seu-servidor.com:1237',  // Servidor de login AAEmu
    gameServerUrl: 'http://seu-servidor.com:1234',   // Servidor de jogo AAEmu
    updateServerUrl: 'http://seu-servidor.com:8080', // Servidor de atualizações
    websiteUrl: 'https://seu-site.com',              // Site oficial
    discordUrl: 'https://discord.gg/seu-discord'     // Discord do servidor
}
```

### Configuração de Recursos

Substitua os arquivos na pasta `assets/` pelos seus próprios:

- `logo.png` - Logo principal (120x120px)
- `logo-small.png` - Logo pequeno (16x16px)
- `icon.ico` - Ícone para Windows
- `icon.icns` - Ícone para macOS
- `icon.png` - Ícone para Linux

## 🏗️ Estrutura do Projeto

```
launcher/
├── src/
│   ├── main.js                 # Processo principal do Electron
│   ├── preload.js             # Script de ponte segura
│   ├── renderer/              # Interface do usuário
│   │   ├── index.html         # Estrutura HTML
│   │   ├── styles.css         # Estilos CSS
│   │   └── script.js          # Lógica do frontend
│   └── services/              # Serviços de backend
│       ├── aaemu-client.js    # Cliente para comunicação com AAEmu
│       └── game-updater.js    # Gerenciador de downloads/atualizações
├── assets/                    # Recursos visuais
├── package.json              # Configuração do projeto
└── README.md                 # Este arquivo
```

## 🔌 Integração com AAEmu

### API Endpoints Esperados

O launcher espera que seu servidor AAEmu exponha os seguintes endpoints:

#### Autenticação
```
POST /api/auth
Content-Type: application/json

{
    "username": "user@email.com",
    "password": "base64_encoded_password"
}
```

#### Status do Servidor
```
GET /api/status
```

#### Lista de Servidores
```
GET /api/servers
```

#### Verificação de Atualizações
```
GET /api/updates
```

### Configuração do Servidor AAEmu

Para integrar com o AAEmu, você precisará criar endpoints HTTP que exponham as funcionalidades necessárias. Exemplo básico em C#:

```csharp
// Adicione um controlador Web API ao seu projeto AAEmu.Login
[ApiController]
[Route("api")]
public class LauncherController : ControllerBase
{
    [HttpPost("auth")]
    public IActionResult Authenticate([FromBody] LoginRequest request)
    {
        // Implementar autenticação usando o LoginController existente
        // Retornar { success: true/false, token: "...", message: "..." }
    }

    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        // Verificar se os servidores estão online
        return Ok(new { status = "online", timestamp = DateTime.UtcNow });
    }
}
```

## 🎨 Personalização Visual

### Cores e Tema

As cores principais podem ser modificadas no arquivo `src/renderer/styles.css`:

```css
:root {
    --primary-color: #d4af37;      /* Dourado principal */
    --primary-dark: #b8941f;       /* Dourado escuro */
    --primary-light: #f4d03f;      /* Dourado claro */
    --background-dark: #1a1a1a;    /* Fundo escuro */
    --text-primary: #ffffff;       /* Texto principal */
}
```

### Fontes

O launcher usa as fontes:
- **Cinzel**: Para títulos e elementos elegantes
- **Open Sans**: Para texto geral

## 📦 Distribuição

### Construindo Executáveis

```bash
# Para Windows (gera instalador NSIS)
npm run build-win

# Para todas as plataformas
npm run build
```

Os arquivos de distribuição serão gerados na pasta `dist/`.

### Configuração de Auto-Update

Para habilitar atualizações automáticas, configure um servidor de releases e atualize a configuração no `package.json`:

```json
{
  "build": {
    "publish": {
      "provider": "github",
      "owner": "seu-usuario",
      "repo": "archeage-br-launcher"
    }
  }
}
```

## 🔧 Desenvolvimento

### Adicionando Novas Funcionalidades

1. **Frontend**: Modifique `src/renderer/script.js` e `src/renderer/index.html`
2. **Backend**: Adicione handlers IPC em `src/main.js`
3. **Serviços**: Crie novos serviços na pasta `src/services/`

### Debugger

Execute com `npm run dev` para ativar o DevTools automático.

## 🐛 Solução de Problemas

### Problemas Comuns

1. **Erro de conexão**: Verifique se as URLs do servidor estão corretas
2. **Download falha**: Verifique permissões da pasta de instalação
3. **Jogo não inicia**: Verifique se o executável existe no caminho correto

### Logs

Os logs são exibidos no console do DevTools (F12) em modo de desenvolvimento.

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.

## 🙏 Agradecimentos

- Equipe AAEmu pelo excelente trabalho no emulador
- XLGames pelo jogo original ArcheAge
- Comunidade Electron.js pelas ferramentas

---

**ArcheAge BR: O Sonho!** - Revivendo a magia de ArcheAge no Brasil 🇧🇷