# Guia dos Sistemas AAEmu - Produção

Este guia explica como usar os novos sistemas implementados para estabilização e expansão do servidor ArcheAge.

## 🔧 Sistema Modular (ModuleManager)

### Visão Geral
O ModuleManager permite carregar módulos/plugins dinamicamente, facilitando expansões sem modificar o código core.

### Como Criar um Módulo

```csharp
public class MeuModulo : GameModuleBase
{
    public override string Name => "MeuModulo";
    public override string Version => "1.0.0";

    public override void Initialize()
    {
        base.Initialize();
        // Lógica de inicialização
    }

    public override void OnPlayerJoin(Character player)
    {
        player.SendMessage("Bem-vindo! Módulo ativo.");
    }
}
```

### Comandos GM
```
/server modules list                    # Lista todos os módulos
/server modules enable MeuModulo        # Ativa um módulo
/server modules disable MeuModulo       # Desativa um módulo
/server modules reload                  # Recarrega módulos externos
```

### Pasta de Módulos
- Coloque DLLs de módulos externos em: `./Modules/`
- Módulos são carregados automaticamente na inicialização
- Use hot-reload para atualizar sem reiniciar o servidor

---

## 🎮 Sistema de Eventos (GameEventManager)

### Visão Geral
Sistema flexível para criar eventos customizados com duração, participação de jogadores e triggers automáticos.

### Criando um Evento Customizado

```csharp
public class EventoPersonalizado : CustomGameEvent
{
    public override string Name => "Meu Evento";
    public override string Description => "Descrição do evento";
    public override TimeSpan Duration => TimeSpan.FromHours(2);

    public override void Start()
    {
        base.Start();
        ChatManager.Instance.BroadcastNotice("Evento iniciado!");
    }

    public override void OnPlayerParticipate(Character player)
    {
        player.SendMessage("Você está participando do evento!");
        // Lógica de recompensas
    }
}
```

### Registrando Event Handlers

```csharp
// Em um módulo
public void RegisterEvents()
{
    GameEventManager.Instance.RegisterEventHandler<PlayerEventArgs>(
        "PlayerKill", OnPlayerKill);
}

private void OnPlayerKill(PlayerEventArgs args)
{
    // Lógica quando jogador mata outro
}
```

### Comandos GM
```
/server events list                     # Lista eventos ativos
/server events start doubleexp          # Inicia evento de XP duplo
/server events start worldboss          # Inicia evento de world boss
/server events stop 1                   # Para evento com ID 1
```

### Eventos Pré-definidos
1. **DoubleXpWeekendEvent** - XP duplo por 48h
2. **WorldBossEvent** - Boss mundial por 30min
3. **DailyBonusEvent** - Bônus diário por 24h

---

## 📊 Sistema de Monitoramento (ServerMetricsManager)

### Visão Geral
Monitora performance, saúde do servidor e estatísticas em tempo real.

### Métricas Disponíveis
- **Performance**: CPU, Memória, Disco
- **Jogadores**: Online, Total conectados
- **Rede**: Pacotes enviados/recebidos
- **Sistema**: Threads, Handles, GC
- **Jogo**: NPCs, Doodads, Containers ativo

### Comandos GM
```
/server metrics                         # Relatório completo
/server metrics json                    # Métricas em JSON
/server health                          # Status de saúde
```

### Usando em Código
```csharp
// Registrar métricas customizadas
ServerMetricsManager.Instance.SetMetric("custom_counter", 100);

// Registrar eventos de rede
ServerMetricsManager.Instance.RecordPacketReceived();
ServerMetricsManager.Instance.RecordPacketSent();

// Verificar saúde
var health = ServerMetricsManager.Instance.GetHealthStatus();
if (!health.IsHealthy)
{
    // Tomar ação
}
```

### Integração com APIs
```csharp
// Endpoint para métricas
[WebApiGet]
public object GetMetrics()
{
    return ServerMetricsManager.Instance.GetMetricsJson();
}
```

---

## 🚢 Melhorias de Física

### Sistema de Embarcações Aprimorado
- **Detecção de água melhorada**: Requer profundidade mínima
- **Gravidade**: Embarcações caem quando fora d'água
- **Controles otimizados**: Melhor responsividade

### HeightMap com Cache
- **Performance**: Cache automático de alturas
- **Menos overhead**: Reduz cálculos repetitivos
- **Configurável**: Tamanho de cache ajustável

---

## 🛠️ Comandos de Administração

### Sistema Principal
```bash
/server                                 # Mostra ajuda
/server test event                      # Testa sistema de eventos
/server test module                     # Testa sistema de módulos
/server test metrics                    # Testa sistema de métricas
```

### Exemplos de Uso em Produção

#### 1. Monitoramento Contínuo
```bash
# Verificar saúde do servidor regularmente
/server health

# Monitorar métricas durante picos de jogadores
/server metrics
```

#### 2. Gerenciamento de Eventos
```bash
# Fim de semana especial
/server events start doubleexp

# Evento semanal de boss
/server events start worldboss

# Parar todos os eventos
/server events list
/server events stop 1
/server events stop 2
```

#### 3. Manutenção de Módulos
```bash
# Verificar módulos carregados
/server modules list

# Desativar módulo problemático temporariamente
/server modules disable ProblematicModule

# Recarregar após atualização
/server modules reload
```

---

## 🔧 Desenvolvimento e Customização

### Estrutura de Arquivos
```
AAEmu.Game/
├── Core/Managers/
│   ├── ModuleManager.cs          # Sistema de módulos
│   ├── GameEventManager.cs       # Sistema de eventos
│   └── ServerMetricsManager.cs   # Sistema de métricas
├── Modules/                      # Módulos internos
│   └── ExampleEventModule.cs     # Exemplo de módulo
├── Physics/HeightMaps/
│   └── ImprovedHeightmapDetection.cs
└── Utils/Scripts/Commands/
    └── ServerManagementCommands.cs

./Modules/                        # Módulos externos (DLLs)
```

### Boas Práticas

#### Para Módulos
1. **Sempre herdar de `GameModuleBase`**
2. **Implementar `IEventModule` se usar eventos**
3. **Usar logging apropriado**
4. **Limpar recursos no `Shutdown()`**

#### Para Eventos
1. **Definir duração apropriada**
2. **Implementar lógica de participação**
3. **Usar broadcasts para notificar jogadores**
4. **Registrar métricas de participação**

#### Para Performance
1. **Monitorar métricas regularmente**
2. **Configurar alertas para CPU/Memória altos**
3. **Usar cache quando apropriado**
4. **Implementar timeouts para operações longas**

---

## 🚀 Próximos Passos

### Para Desenvolvedores
1. **Criar módulos específicos** para funcionalidades desejadas
2. **Implementar testes automatizados** para os sistemas
3. **Otimizar sistemas existentes** baseado nas métricas
4. **Adicionar mais eventos** conforme necessidade

### Para Administradores
1. **Configurar monitoramento automático**
2. **Criar cronograma de eventos regulares**
3. **Definir alertas de performance**
4. **Treinar equipe nos novos comandos**

### Exemplo de Cronograma Semanal
```
Segunda: /server events start daily (para todos)
Sexta: /server events start doubleexp
Sábado: /server events start worldboss
Domingo: /server health (verificação semanal)
```

---

*Este guia será atualizado conforme novos sistemas são implementados*