# Sistema de Trade Packs Completo - ArcheAge Emulator

## Por que Trade Packs têm Delay?

### Mecânicas do Jogo Original

1. **Balanceamento Econômico**: 
   - Trade packs são mais lucrativos que pesca
   - O delay evita inflação rápida
   - Adiciona risco ao investimento

2. **Realismo**:
   - Simula transporte e processamento
   - Diferencia de atividades instantâneas como pesca

3. **Estratégia**:
   - Jogadores devem planejar rotas
   - Adiciona tensão e risco
   - Cria oportunidades de PvP

### Comparação com Outras Atividades

| Atividade | Recompensa | Delay | Risco |
|-----------|------------|-------|-------|
| Pesca | Baixa | Instantâneo | Baixo |
| Trade Packs | Alta | 8-22 horas | Alto |
| Crafting | Média | Instantâneo | Médio |

## Configurações Disponíveis

### 1. Configuração de Arquivo JSON

**Arquivo**: `ExampleConfig.json`

```json
{
    "Specialty": {
        "TradePackMailDelayHours": 8.0,        // Delay em horas
        "InstantTradePackDelivery": false,      // Entrega instantânea
        "TradePackInterestRate": 5,            // Taxa de juros (%)
        "TradePackSellerShare": 0.80,          // Compartilhamento vendedor (80%)
        "TradePackLaborCost": 60,              // Custo de labor
        "TradePackMaxDistance": 2.5,           // Distância máxima do NPC
        "MaxSpecialtyRatio": 130,              // Ratio máximo (%)
        "MinSpecialtyRatio": 70,               // Ratio mínimo (%)
        "RatioDecreasePerPack": 0.5,           // Diminuição por pack (%)
        "RatioIncreasePerTick": 5.0,           // Aumento por tick (%)
        "RatioDecreaseTickMinutes": 1.0,       // Intervalo de diminuição (min)
        "RatioRegenTickMinutes": 60.0          // Intervalo de regeneração (min)
    }
}
```

### 2. Comandos In-Game

#### Ver Configuração Atual
```
.tradepackconfig show
```

#### Ativar Entrega Instantânea
```
.tradepackconfig instant true
```

#### Definir Delay Personalizado
```
.tradepackconfig delay 0.5    // 30 minutos
.tradepackconfig delay 2.0    // 2 horas
.tradepackconfig delay 24.0   // 24 horas
```

#### Ajustar Taxa de Juros
```
.tradepackconfig interest 10  // 10% de juros
.tradepackconfig interest 0   // Sem juros
```

#### Ajustar Custo de Labor
```
.tradepackconfig labor 30     // 30 labor
.tradepackconfig labor 0      // Sem custo
```

#### Ajustar Distância Máxima
```
.tradepackconfig distance 5.0 // 5 metros
.tradepackconfig distance 10.0 // 10 metros
```

#### Ajustar Compartilhamento
```
.tradepackconfig share 100    // 100% para vendedor
.tradepackconfig share 50     // 50% para vendedor
```

#### Resetar para Padrões
```
.tradepackconfig reset
```

## Exemplos de Configuração

### 1. Servidor Casual (Entrega Rápida)
```json
{
    "Specialty": {
        "TradePackMailDelayHours": 1.0,
        "InstantTradePackDelivery": false,
        "TradePackInterestRate": 3,
        "TradePackLaborCost": 30,
        "TradePackSellerShare": 0.90
    }
}
```

### 2. Servidor Hardcore (Delay Original)
```json
{
    "Specialty": {
        "TradePackMailDelayHours": 22.0,
        "InstantTradePackDelivery": false,
        "TradePackInterestRate": 8,
        "TradePackLaborCost": 80,
        "TradePackSellerShare": 0.70
    }
}
```

### 3. Servidor Teste (Instantâneo)
```json
{
    "Specialty": {
        "TradePackMailDelayHours": 0.0,
        "InstantTradePackDelivery": true,
        "TradePackInterestRate": 0,
        "TradePackLaborCost": 0,
        "TradePackSellerShare": 1.0
    }
}
```

## Como Funciona o Sistema

### 1. Fluxo de Venda
```
1. Jogador equipa trade pack
2. Interage com NPC de specialty
3. Sistema verifica:
   - Labor power suficiente
   - Distância do NPC
   - Trade pack válido
4. Calcula preço base + ratio + juros
5. Cria mail com delay configurado
6. Remove trade pack e consome labor
```

### 2. Sistema de Mails
```
- Mail para vendedor: Recompensa principal
- Mail para crafter: Parte do lucro (se diferente)
- Delay configurável: 0-24+ horas
- Entrega instantânea: Opcional
```

### 3. Sistema de Ratios
```
- Ratio dinâmico baseado em vendas
- Diminui com mais vendas
- Regenera com o tempo
- Configurável por zona
```

## Código Fonte Relevante

### 1. Configuração de Delay
**Arquivo**: `AAEmu.Game/Models/Game/Mails/MailForSpeciality.cs`
```csharp
private static TimeSpan TradePackMailDelay => AppConfiguration.Instance.Specialty.InstantTradePackDelivery 
    ? TimeSpan.Zero 
    : TimeSpan.FromHours(AppConfiguration.Instance.Specialty.TradePackMailDelayHours);
```

### 2. Configuração de Labor
**Arquivo**: `AAEmu.Game/Core/Managers/World/SpecialtyManager.cs`
```csharp
var laborCost = AppConfiguration.Instance.Specialty.TradePackLaborCost;
if (player.LaborPower < laborCost)
{
    player.SendErrorMessage(ErrorMessageType.NotEnoughLaborPower);
    return 0;
}
```

### 3. Configuração de Distância
```csharp
if (MathUtil.CalculateDistance(player.Transform.World.Position, npc.Transform.World.Position) > AppConfiguration.Instance.Specialty.TradePackMaxDistance)
{
    player.SendErrorMessage(ErrorMessageType.TooFarAway);
    return 0;
}
```

## Troubleshooting

### Problema: "NotEnoughLaborPower"
**Causa**: Labor power insuficiente
**Solução**: Ajustar `TradePackLaborCost` ou aguardar regeneração

### Problema: "TooFarAway"
**Causa**: Muito longe do NPC
**Solução**: Ajustar `TradePackMaxDistance` ou aproximar-se

### Problema: Mail não chega
**Causa**: Delay muito alto ou sistema de mail com problema
**Solução**: Verificar `TradePackMailDelayHours` ou `InstantTradePackDelivery`

### Problema: Preço muito baixo
**Causa**: Ratio baixo na zona
**Solução**: Ajustar `MaxSpecialtyRatio` e `MinSpecialtyRatio`

## Logs de Debug

O sistema inclui logs detalhados:
```
[DEBUG] SellSpecialty - Player: PlayerName, NPC ObjId: 12345
[DEBUG] SellSpecialty - Base price: 50000
[DEBUG] SellSpecialty - Price ratio: 100
[DEBUG] SellSpecialty - CrafterId: 0, MadeUnitId: 0, PlayerId: 123
[DEBUG] SellSpecialty - Final calculations - Seller: 50000, Crafter: 0, ItemType: 0
[DEBUG] SellSpecialty - Seller mail sent successfully
[DEBUG] SellSpecialty - Trade pack sold successfully for 50000 base price
```

## Comandos de Teste

### Testar Sistema de Trade Packs
```
.testtradepacks info    - Informações do trade pack atual
.testtradepacks mail    - Testar criação de mail
.testtradepacks npc     - NPCs de specialty carregados
.testtradepacks ratio   - Sistema de ratios
```

### Configurar Trade Packs
```
.tradepackconfig show           - Ver configuração atual
.tradepackconfig instant true   - Ativar entrega instantânea
.tradepackconfig delay 0.5      - Delay de 30 minutos
.tradepackconfig interest 10    - 10% de juros
.tradepackconfig labor 30       - 30 labor
.tradepackconfig distance 5.0   - 5 metros de distância
.tradepackconfig share 100      - 100% para vendedor
.tradepackconfig reset          - Resetar para padrões
```

## Conclusão

O sistema de trade packs agora é **100% configurável** e permite:

- ✅ **Delay personalizável**: 0-24+ horas
- ✅ **Entrega instantânea**: Opcional
- ✅ **Taxa de juros configurável**: 0-100%
- ✅ **Custo de labor ajustável**: 0-100+
- ✅ **Distância máxima configurável**: 0-100+ metros
- ✅ **Compartilhamento de lucro**: 0-100%
- ✅ **Comandos in-game**: Para configuração dinâmica
- ✅ **Logs detalhados**: Para debug
- ✅ **Compatibilidade total**: Com sistemas existentes

Agora você pode adaptar o sistema de trade packs para qualquer tipo de servidor, desde casual até hardcore!