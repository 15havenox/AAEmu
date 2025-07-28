# Integração do Launcher com AAEmu

Este documento explica como integrar o launcher do ArcheAge BR com o servidor AAEmu.

## 📋 Visão Geral

O launcher precisa comunicar-se com o servidor AAEmu para:
1. Autenticar usuários
2. Verificar status dos servidores
3. Obter informações sobre atualizações
4. Lançar o cliente do jogo

## 🔧 Modificações Necessárias no AAEmu

### 1. Adicionar Controlador Web API

Crie um novo controlador no projeto `AAEmu.Login`:

```csharp
// AAEmu.Login/Controllers/LauncherController.cs
using Microsoft.AspNetCore.Mvc;
using AAEmu.Login.Core.Controllers;
using AAEmu.Login.Models;
using System.Text;

[ApiController]
[Route("api")]
public class LauncherController : ControllerBase
{
    private readonly ILoginController _loginController;
    private readonly ILogger<LauncherController> _logger;

    public LauncherController(ILoginController loginController, ILogger<LauncherController> logger)
    {
        _loginController = loginController;
        _logger = logger;
    }

    [HttpPost("auth")]
    public async Task<IActionResult> Authenticate([FromBody] AuthRequest request)
    {
        try
        {
            // Decodificar senha Base64
            var passwordBytes = Convert.FromBase64String(request.Password);
            
            // Simular uma conexão de login para autenticação
            // Você precisará adaptar isso à sua implementação específica
            var isValid = await ValidateCredentials(request.Username, passwordBytes);
            
            if (isValid)
            {
                var token = GenerateToken(request.Username);
                return Ok(new
                {
                    success = true,
                    token = token,
                    accountId = GetAccountId(request.Username),
                    message = "Autenticação realizada com sucesso"
                });
            }
            else
            {
                return Ok(new
                {
                    success = false,
                    message = "Email ou senha incorretos"
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro na autenticação do launcher");
            return StatusCode(500, new
            {
                success = false,
                message = "Erro interno do servidor"
            });
        }
    }

    [HttpGet("status")]
    public IActionResult GetServerStatus()
    {
        // Verificar status dos servidores
        var loginServerOnline = true; // Verificar se o servidor de login está rodando
        var gameServerOnline = CheckGameServerStatus();
        
        return Ok(new
        {
            status = loginServerOnline && gameServerOnline ? "online" : "offline",
            login = loginServerOnline,
            game = gameServerOnline,
            timestamp = DateTime.UtcNow,
            playersOnline = GetOnlinePlayersCount()
        });
    }

    [HttpGet("servers")]
    public IActionResult GetServerList()
    {
        // Retornar lista de servidores disponíveis
        var servers = new[]
        {
            new
            {
                id = 1,
                name = "ArcheAge BR: O Sonho!",
                status = "online",
                population = "Médio",
                ip = "servidor.archeagebr.com",
                port = 1234
            }
        };

        return Ok(new { servers });
    }

    [HttpGet("updates")]
    public IActionResult CheckUpdates()
    {
        // Verificar se há atualizações disponíveis
        var currentVersion = "1.0.0";
        var latestVersion = GetLatestGameVersion();
        
        return Ok(new
        {
            hasUpdate = currentVersion != latestVersion,
            version = latestVersion,
            downloadUrl = $"https://updates.archeagebr.com/game-{latestVersion}.zip",
            size = 15000000000, // 15GB em bytes
            changelog = new[]
            {
                "Correção de bugs no sistema de comércio",
                "Novos itens adicionados",
                "Melhorias de performance"
            }
        });
    }

    [HttpPost("verify")]
    public IActionResult VerifyToken([FromBody] VerifyRequest request)
    {
        var isValid = ValidateToken(request.Token);
        
        return Ok(new
        {
            valid = isValid,
            message = isValid ? "Token válido" : "Token inválido ou expirado"
        });
    }

    // Métodos auxiliares (implementar conforme sua lógica)
    private async Task<bool> ValidateCredentials(string username, byte[] password)
    {
        // Implementar validação usando o LoginController existente
        // Este é um exemplo simplificado
        return true; // Substituir pela lógica real
    }

    private string GenerateToken(string username)
    {
        // Gerar token JWT ou similar
        return Guid.NewGuid().ToString();
    }

    private int GetAccountId(string username)
    {
        // Obter ID da conta do banco de dados
        return 1; // Substituir pela lógica real
    }

    private bool CheckGameServerStatus()
    {
        // Verificar se o servidor de jogo está online
        return true; // Substituir pela lógica real
    }

    private int GetOnlinePlayersCount()
    {
        // Obter número de jogadores online
        return 50; // Substituir pela lógica real
    }

    private string GetLatestGameVersion()
    {
        // Obter versão mais recente do jogo
        return "1.0.0"; // Substituir pela lógica real
    }

    private bool ValidateToken(string token)
    {
        // Validar token
        return !string.IsNullOrEmpty(token); // Substituir pela lógica real
    }
}

// Modelos de request
public class AuthRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class VerifyRequest
{
    public string Token { get; set; }
}
```

### 2. Configurar Startup/Program.cs

Adicione suporte a Web API no `Program.cs` do AAEmu.Login:

```csharp
// No método ConfigureServices
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("LauncherPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// No método Configure/Run
app.UseRouting();
app.UseCors("LauncherPolicy");
app.MapControllers();
```

### 3. Configurar Roteamento

Certifique-se de que o AAEmu.Login está configurado para aceitar requisições HTTP na porta correta.

## 🌐 Servidor de Atualizações

### Estrutura do Servidor de Updates

Crie um servidor simples para hospedar atualizações:

```javascript
// update-server.js
const express = require('express');
const fs = require('fs');
const path = require('path');

const app = express();
const PORT = 8080;

app.use(express.static('public'));

app.get('/api/updates', (req, res) => {
    const updateInfo = {
        version: '1.0.1',
        downloadUrl: 'http://localhost:8080/downloads/game-1.0.1.zip',
        size: 15000000000,
        changelog: [
            'Correção de bugs críticos',
            'Novos conteúdos adicionados',
            'Melhorias de performance'
        ]
    };
    
    res.json(updateInfo);
});

app.listen(PORT, () => {
    console.log(`Servidor de atualizações rodando na porta ${PORT}`);
});
```

### Estrutura de Diretórios

```
update-server/
├── package.json
├── server.js
└── public/
    └── downloads/
        ├── game-1.0.0.zip
        ├── game-1.0.1.zip
        └── patches/
```

## 🔐 Segurança

### Autenticação Segura

1. **Hashing de Senhas**: Use bcrypt ou similar para hash das senhas
2. **Tokens JWT**: Implemente tokens JWT com expiração
3. **HTTPS**: Use sempre HTTPS em produção
4. **Rate Limiting**: Implemente rate limiting nas APIs

### Exemplo de Implementação Segura

```csharp
// Usando bcrypt para validação de senhas
private async Task<bool> ValidateCredentials(string username, string password)
{
    using var connection = MySQL.CreateConnection();
    using var command = connection.CreateCommand();
    
    command.CommandText = "SELECT password FROM users WHERE username = @username";
    command.Parameters.AddWithValue("@username", username);
    
    var hashedPassword = await command.ExecuteScalarAsync() as string;
    
    if (hashedPassword == null) return false;
    
    return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
}
```

## 🚀 Execução do Cliente

### Handler para Lançar o Jogo

Adicione ao `main.js` do launcher:

```javascript
ipcMain.handle('launch-game', async (event, { token, serverId }) => {
    const { spawn } = require('child_process');
    const path = require('path');
    
    const installPath = store.get('installPath');
    const gameExePath = path.join(installPath, 'bin', 'archeage.exe');
    
    // Parâmetros para o cliente do jogo
    const args = [
        '--server', serverAPI.config.gameServerUrl,
        '--token', token,
        '--launcher-mode'
    ];
    
    try {
        const gameProcess = spawn(gameExePath, args, {
            detached: true,
            stdio: 'ignore'
        });
        
        gameProcess.unref();
        return { success: true };
    } catch (error) {
        return { success: false, error: error.message };
    }
});
```

## 📝 Configuração Final

### 1. URLs do Servidor

Edite `src/preload.js` no launcher:

```javascript
config: {
    loginServerUrl: 'http://seu-servidor.com:1237',
    gameServerUrl: 'http://seu-servidor.com:1234',
    updateServerUrl: 'http://seu-servidor.com:8080',
    websiteUrl: 'https://archeagebr.com',
    discordUrl: 'https://discord.gg/archeagebr'
}
```

### 2. Configuração do AAEmu

Ajuste as configurações no `Config.json` do AAEmu.Login:

```json
{
    "Network": {
        "Host": "*",
        "Port": 1237
    },
    "WebApi": {
        "Enabled": true,
        "Port": 1237,
        "AllowedOrigins": ["http://localhost:*", "https://archeagebr.com"]
    }
}
```

## 🧪 Testes

### Testando a Integração

1. **Iniciar AAEmu.Login**:
   ```bash
   cd AAEmu.Login
   dotnet run
   ```

2. **Iniciar servidor de updates**:
   ```bash
   cd update-server
   node server.js
   ```

3. **Testar endpoints**:
   ```bash
   # Testar status
   curl http://localhost:1237/api/status
   
   # Testar autenticação
   curl -X POST http://localhost:1237/api/auth \
        -H "Content-Type: application/json" \
        -d '{"username":"test@example.com","password":"dGVzdA=="}'
   ```

## 🔄 Fluxo Completo

1. **Usuário abre o launcher**
2. **Launcher verifica status do servidor** → `GET /api/status`
3. **Usuário faz login** → `POST /api/auth`
4. **Launcher verifica atualizações** → `GET /api/updates`
5. **Se necessário, baixa atualizações**
6. **Usuário clica em "Jogar"**
7. **Launcher executa o cliente do jogo** com token de autenticação

Este fluxo garante que apenas usuários autenticados possam acessar o jogo e que sempre tenham a versão mais atualizada.