# 📜 **MÓDULO 12: SISTEMA DE QUESTS (MISSÕES) - MEGA DETALHADO**
## 🎓 O GUIA DEFINITIVO PARA CRIAR UM SISTEMA DE MISSÕES COMPLETO

---

## 🎯 **PARTE 1: INTRODUÇÃO COMPLETA AO SISTEMA DE QUESTS**

### **1.1 O que são Quests? - Explicação para Iniciantes**

Imagine que você está jogando RPG e um NPC te dá uma missão: "Vá até a floresta, mate 10 lobos e traga suas peles". Isso é uma **Quest**! 

Mas vamos entender MUITO mais profundamente o que isso significa:

#### **Por que Quests Existem?**
1. **Narrativa**: Contam a história do mundo
2. **Progressão**: Fazem o jogador evoluir gradualmente
3. **Direcionamento**: Mostram onde ir e o que fazer
4. **Recompensa**: Dão motivos para continuar jogando
5. **Tutorial**: Ensinam mecânicas do jogo naturalmente

#### **Tipos de Quests no ArcheAge:**
- **Main Story**: História principal do jogo
- **Side Quests**: Histórias secundárias
- **Daily Quests**: Repetem todo dia
- **Weekly Quests**: Repetem toda semana
- **Event Quests**: Só durante eventos especiais
- **Chain Quests**: Uma leva à próxima
- **Raid Quests**: Para grupos grandes
- **PvP Quests**: Relacionadas a combate entre jogadores

### **1.2 A Psicologia por Trás das Quests**

**Por que os jogadores AMAM fazer quests?**

1. **Senso de Propósito**: "Eu tenho algo importante para fazer"
2. **Recompensa Garantida**: "Se eu fizer X, vou receber Y"
3. **Progresso Visível**: "Matei 7 de 10 lobos, quase lá!"
4. **Descoberta**: "O que vai acontecer depois?"
5. **Mastery**: "Estou ficando melhor no jogo"

**Como criar quests viciantes:**
- Objetivos claros e alcançáveis
- Recompensas proporcionais ao esforço
- Histórias interessantes
- Variedade de atividades
- Surpresas e reviravoltas

### **1.3 Arquitetura Mental do Sistema de Quests**

Pense no sistema de quests como uma **fábrica automática**:

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   QUEST GIVER   │────│   OBJECTIVES    │────│    REWARDS      │
│   (NPC/Item)    │    │  (Tarefas)      │    │ (Prêmios)       │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│    DATABASE     │    │   PROGRESS      │    │   COMPLETION    │
│  (Armazena)     │    │  (Progresso)    │    │  (Finalização)  │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

Cada peça dessa "fábrica" precisa funcionar perfeitamente para criar uma experiência fluida.

### **1.4 No ArcheAge, as Quests são Especiais**

No ArcheAge específicamente, as quests têm características únicas:

- **Escolhas Morais**: Algumas quests têm decisões que afetam a história
- **Craft Integration**: Muitas quests envolvem crafting de itens
- **Open World**: Quests acontecem em um mundo aberto persistente
- **Player Interaction**: Algumas quests requerem cooperação entre jogadores
- **Dynamic Events**: Quests podem ser afetadas por eventos do mundo
- **Trade System**: Algumas quests envolvem comércio entre jogadores

---

## 🔧 **PARTE 2: ANATOMIA COMPLETA DE UMA QUEST**

### **2.1 Pensando Como um Game Designer**

Antes de escrever código, vamos entender como criar uma quest **do conceito ao código**:

#### **Etapa 1: Conceito e História**
```
Pergunta: "Que história quero contar?"
Resposta: "Um jovem aventureiro precisa provar sua coragem"

Pergunta: "Como ele vai provar?"
Resposta: "Derrotando criaturas perigosas"

Pergunta: "Onde isso acontece?"
Resposta: "Na floresta próxima à cidade"

Pergunta: "Qual a recompensa adequada?"
Resposta: "Experiência, dinheiro e itens úteis"
```

#### **Etapa 2: Mecânicas de Jogo**
```
Objetivo: Matar 10 Lobos Selvagens
Localização: Floresta Sombria
Tempo estimado: 15-20 minutos
Dificuldade: Fácil (para iniciantes)
Pré-requisitos: Level 1, nenhuma quest anterior
```

#### **Etapa 3: Balanceamento**
```
EXP Reward: Suficiente para 1/4 do level atual
Gold Reward: Para comprar 1 poção + 1 equipamento básico
Item Reward: Algo útil para o próximo desafio
```

### **2.2 A Estrutura Técnica Completa de uma Quest**

Agora vamos traduzir isso para código. Uma Quest no AAEmu é muito mais complexa do que parece:

```csharp
/// <summary>
/// Representa uma Quest completa no sistema AAEmu
/// Esta é a classe MESTRE que define tudo sobre uma missão
/// </summary>
public class Quest
{
    #region Identificação Básica
    /// <summary>
    /// ID único da quest no sistema - NUNCA pode repetir
    /// Usado para referenciar esta quest em todo o código
    /// </summary>
    public uint Id { get; set; }
    
    /// <summary>
    /// Nome que aparece na interface do jogador
    /// Deve ser atrativo e indicar o objetivo
    /// Exemplo: "O Chamado da Floresta", "Primeira Caçada"
    /// </summary>
    public string Title { get; set; }
    
    /// <summary>
    /// História/contexto da missão - o "por que" fazer
    /// Deve imergir o jogador na narrativa
    /// Exemplo: "Os lobos estão atacando os viajantes..."
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// Texto quando o NPC oferece a quest
    /// Deve ser persuasivo e contextualizar a urgência
    /// </summary>
    public string AcceptText { get; set; }
    
    /// <summary>
    /// Texto quando o jogador completa a quest
    /// Deve dar sensação de conquista e fechamento
    /// </summary>
    public string CompleteText { get; set; }
    
    /// <summary>
    /// Texto se o jogador declinar a quest
    /// Deve manter a porta aberta para ele mudar de ideia
    /// </summary>
    public string DeclineText { get; set; }
    
    /// <summary>
    /// Categoria da quest (Main Story, Side, Daily, etc.)
    /// Usado para organização na interface
    /// </summary>
    public QuestCategory Category { get; set; }
    #endregion
    
    #region Objetivos e Recompensas
    /// <summary>
    /// Lista de tarefas que o jogador deve cumprir
    /// Cada objetivo é independente mas todos devem ser completados
    /// </summary>
    public List<QuestObjective> Objectives { get; set; } = new();
    
    /// <summary>
    /// Prêmios que o jogador recebe ao completar
    /// Devem ser balanceados com a dificuldade da quest
    /// </summary>
    public List<QuestReward> Rewards { get; set; } = new();
    
    /// <summary>
    /// Recompensas opcionais que o jogador pode escolher
    /// Útil quando queremos dar escolha ao jogador
    /// </summary>
    public List<QuestReward> ChoiceRewards { get; set; } = new();
    #endregion
    
    #region Pré-requisitos e Restrições
    /// <summary>
    /// Quests que devem ser completadas antes desta
    /// Usado para criar sequências narrativas
    /// </summary>
    public List<uint> PrerequisiteQuests { get; set; } = new();
    
    /// <summary>
    /// Itens que o jogador deve ter no inventário
    /// Para pegar a quest (verificado na aceitação)
    /// </summary>
    public List<uint> RequiredItems { get; set; } = new();
    
    /// <summary>
    /// Level mínimo para aceitar a quest
    /// Garante que o jogador tem habilidade suficiente
    /// </summary>
    public byte MinLevel { get; set; }
    
    /// <summary>
    /// Level máximo para aceitar a quest
    /// Evita que jogadores high-level peguem quests muito fáceis
    /// </summary>
    public byte MaxLevel { get; set; } = 255; // 255 = sem limite
    
    /// <summary>
    /// Quais classes podem fazer esta quest
    /// Algumas quests são específicas para certas classes
    /// </summary>
    public ClassType AllowedClass { get; set; } = ClassType.All;
    
    /// <summary>
    /// Qual facção pode fazer esta quest
    /// Importante para quests de PvP ou story específica
    /// </summary>
    public FactionType AllowedFaction { get; set; } = FactionType.All;
    #endregion
    
    #region Configurações de Repetição
    /// <summary>
    /// Se a quest pode ser repetida diariamente
    /// Reseta às 6h da manhã (hora do servidor)
    /// </summary>
    public bool IsDaily { get; set; }
    
    /// <summary>
    /// Se a quest pode ser repetida semanalmente
    /// Reseta toda segunda-feira às 6h
    /// </summary>
    public bool IsWeekly { get; set; }
    
    /// <summary>
    /// Se a quest só aparece durante eventos
    /// Controlado pelo sistema de eventos
    /// </summary>
    public bool IsEventQuest { get; set; }
    
    /// <summary>
    /// Data de início (para quests temporárias)
    /// null = sempre disponível
    /// </summary>
    public DateTime? StartDate { get; set; }
    
    /// <summary>
    /// Data de fim (para quests temporárias)
    /// null = nunca expira
    /// </summary>
    public DateTime? EndDate { get; set; }
    #endregion
    
    #region Configurações de Grupo
    /// <summary>
    /// Se a quest pode ser feita em grupo
    /// Todos do grupo próximos recebem progresso
    /// </summary>
    public bool AllowsGroup { get; set; } = true;
    
    /// <summary>
    /// Tamanho mínimo do grupo (0 = solo)
    /// Útil para forçar cooperação
    /// </summary>
    public byte MinGroupSize { get; set; } = 0;
    
    /// <summary>
    /// Tamanho máximo do grupo (0 = sem limite)
    /// Previne exploits com grupos grandes demais
    /// </summary>
    public byte MaxGroupSize { get; set; } = 0;
    
    /// <summary>
    /// Se todos do grupo devem estar na mesma zona
    /// Para quests que requerem presença física
    /// </summary>
    public bool RequireGroupProximity { get; set; } = true;
    
    /// <summary>
    /// Distância máxima entre membros do grupo (metros)
    /// Para considerar "próximos" para progresso compartilhado
    /// </summary>
    public float GroupProximityDistance { get; set; } = 50f;
    #endregion
    
    #region Configurações Avançadas
    /// <summary>
    /// NPC que dá esta quest (pode ser múltiplos)
    /// null = quest automática (triggered por evento)
    /// </summary>
    public uint? GiverNpcId { get; set; }
    
    /// <summary>
    /// NPC para quem entregar a quest (se diferente do giver)
    /// null = entrega para o mesmo que deu
    /// </summary>
    public uint? TurnInNpcId { get; set; }
    
    /// <summary>
    /// Zona onde a quest pode ser aceita
    /// null = qualquer lugar (se tiver o NPC)
    /// </summary>
    public uint? RequiredZoneId { get; set; }
    
    /// <summary>
    /// Se a quest auto-completa quando objetivos são atingidos
    /// ou se precisa voltar no NPC para entregar
    /// </summary>
    public bool AutoComplete { get; set; } = false;
    
    /// <summary>
    /// Se a quest é abandonável pelo jogador
    /// Algumas quests importantes não podem ser abandonadas
    /// </summary>
    public bool CanAbandon { get; set; } = true;
    
    /// <summary>
    /// Se a quest é compartilhável entre jogadores
    /// Permite que um jogador "passe" a quest para outro
    /// </summary>
    public bool CanShare { get; set; } = true;
    
    /// <summary>
    /// Prioridade de exibição na lista de quests
    /// Maior = mais importante, aparece no topo
    /// </summary>
    public int Priority { get; set; } = 0;
    
    /// <summary>
    /// Cor do título da quest na interface
    /// Indica dificuldade ou importância visualmente
    /// </summary>
    public QuestTitleColor TitleColor { get; set; } = QuestTitleColor.Normal;
    #endregion
    
    #region Métodos de Validação
    /// <summary>
    /// Verifica se todas as tarefas foram cumpridas
    /// Este é o método principal para saber se a quest pode ser entregue
    /// </summary>
    public bool IsCompleted()
    {
        // Se não tem objetivos, considera completa
        if (!Objectives.Any()) return true;
        
        // Todos os objetivos devem estar completos
        return Objectives.All(obj => obj.IsCompleted);
    }
    
    /// <summary>
    /// Verifica se jogador atende TODOS os requisitos para aceitar esta quest
    /// Este método é chamado antes de oferecer a quest ao jogador
    /// </summary>
    public bool CanAccept(Character character)
    {
        // Verifica level
        if (character.Level < MinLevel || character.Level > MaxLevel)
        {
            Logger.Debug($"Player {character.Name} level {character.Level} not in range {MinLevel}-{MaxLevel} for quest {Id}");
            return false;
        }
        
        // Verifica classe
        if (AllowedClass != ClassType.All && character.Class != AllowedClass)
        {
            Logger.Debug($"Player {character.Name} class {character.Class} not allowed for quest {Id} (requires {AllowedClass})");
            return false;
        }
        
        // Verifica facção
        if (AllowedFaction != FactionType.All && character.Faction != AllowedFaction)
        {
            Logger.Debug($"Player {character.Name} faction {character.Faction} not allowed for quest {Id}");
            return false;
        }
        
        // Verifica pré-requisitos de quests
        foreach (var prereq in PrerequisiteQuests)
        {
            if (!character.CompletedQuests.Contains(prereq))
            {
                Logger.Debug($"Player {character.Name} missing prerequisite quest {prereq} for quest {Id}");
                return false;
            }
        }
        
        // Verifica itens necessários
        foreach (var itemId in RequiredItems)
        {
            if (!character.Inventory.HasItem(itemId))
            {
                Logger.Debug($"Player {character.Name} missing required item {itemId} for quest {Id}");
                return false;
            }
        }
        
        // Verifica se já tem esta quest ativa
        if (character.ActiveQuests.Any(q => q.QuestId == Id))
        {
            Logger.Debug($"Player {character.Name} already has quest {Id} active");
            return false;
        }
        
        // Verifica se já completou (para quests não repetíveis)
        if (!IsDaily && !IsWeekly && character.CompletedQuests.Contains(Id))
        {
            Logger.Debug($"Player {character.Name} already completed non-repeatable quest {Id}");
            return false;
        }
        
        // Verifica datas de disponibilidade
        var now = DateTime.Now;
        if (StartDate.HasValue && now < StartDate.Value)
        {
            Logger.Debug($"Quest {Id} not yet available (starts {StartDate.Value})");
            return false;
        }
        
        if (EndDate.HasValue && now > EndDate.Value)
        {
            Logger.Debug($"Quest {Id} no longer available (ended {EndDate.Value})");
            return false;
        }
        
        // Verifica zona
        if (RequiredZoneId.HasValue && character.ZoneId != RequiredZoneId.Value)
        {
            Logger.Debug($"Player {character.Name} not in required zone {RequiredZoneId} for quest {Id}");
            return false;
        }
        
        // Se chegou até aqui, pode aceitar!
        return true;
    }
    
    /// <summary>
    /// Verifica se a quest pode ser entregue por este jogador
    /// Diferente de CanAccept - aqui verificamos se pode COMPLETAR
    /// </summary>
    public bool CanTurnIn(Character character)
    {
        // Deve estar na lista de quests ativas
        var activeQuest = character.ActiveQuests.FirstOrDefault(q => q.QuestId == Id);
        if (activeQuest == null) return false;
        
        // Todos os objetivos devem estar completos
        if (!IsCompleted()) return false;
        
        // Se tem NPC específico para entrega, deve estar próximo dele
        if (TurnInNpcId.HasValue)
        {
            var turnInNpc = character.CurrentZone.GetNpcById(TurnInNpcId.Value);
            if (turnInNpc == null) return false;
            
            var distance = Vector3.Distance(character.Position, turnInNpc.Position);
            if (distance > 10f) // 10 metros de distância máxima
            {
                Logger.Debug($"Player {character.Name} too far from turn-in NPC for quest {Id}");
                return false;
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// Calcula a experiência que esta quest deve dar baseada no level do jogador
    /// Permite balanceamento dinâmico de recompensas
    /// </summary>
    public uint CalculateExperienceReward(Character character)
    {
        // Quest de level mais alto que o jogador = mais EXP
        var levelDiff = MinLevel - character.Level;
        var baseExp = MinLevel * 100; // Base: 100 EXP por level da quest
        
        if (levelDiff > 5) return (uint)(baseExp * 1.5f); // 50% bonus para quests difíceis
        if (levelDiff > 0) return (uint)(baseExp * 1.2f); // 20% bonus para quests um pouco difíceis
        if (levelDiff > -5) return baseExp; // EXP normal
        
        // Quest muito fácil = EXP reduzida
        return (uint)(baseExp * 0.5f);
    }
    
    /// <summary>
    /// Calcula o ouro que esta quest deve dar
    /// Baseado na dificuldade e tempo estimado
    /// </summary>
    public uint CalculateGoldReward(Character character)
    {
        var baseGold = MinLevel * 10; // 10 ouro por level da quest
        
        // Quests em grupo dão mais ouro (assumindo mais dificuldade)
        if (MinGroupSize > 1) baseGold = (uint)(baseGold * 1.5f);
        
        // Quests diárias dão menos ouro (podem ser repetidas)
        if (IsDaily) baseGold = (uint)(baseGold * 0.7f);
        
        return (uint)baseGold;
    }
    #endregion
    
    #region Métodos de Utilidade
    /// <summary>
    /// Retorna uma descrição detalhada da quest para debug
    /// Útil para logs e ferramentas de admin
    /// </summary>
    public string GetDetailedInfo()
    {
        var info = new StringBuilder();
        info.AppendLine($"Quest {Id}: {Title}");
        info.AppendLine($"Category: {Category}");
        info.AppendLine($"Level Range: {MinLevel}-{MaxLevel}");
        info.AppendLine($"Class: {AllowedClass}");
        info.AppendLine($"Faction: {AllowedFaction}");
        info.AppendLine($"Objectives: {Objectives.Count}");
        info.AppendLine($"Rewards: {Rewards.Count}");
        info.AppendLine($"Prerequisites: {PrerequisiteQuests.Count}");
        info.AppendLine($"Repeatable: Daily={IsDaily}, Weekly={IsWeekly}");
        info.AppendLine($"Group: Min={MinGroupSize}, Max={MaxGroupSize}");
        
        return info.ToString();
    }
    
    /// <summary>
    /// Retorna uma versão "limpa" da quest para envio ao cliente
    /// Remove informações sensíveis ou desnecessárias
    /// </summary>
    public QuestClientData ToClientData()
    {
        return new QuestClientData
        {
            Id = Id,
            Title = Title,
            Description = Description,
            Category = Category,
            Objectives = Objectives.Select(obj => obj.ToClientData()).ToList(),
            Rewards = Rewards.Select(rew => rew.ToClientData()).ToList(),
            ChoiceRewards = ChoiceRewards.Select(rew => rew.ToClientData()).ToList(),
            MinLevel = MinLevel,
            MaxLevel = MaxLevel,
            IsDaily = IsDaily,
            IsWeekly = IsWeekly,
            AllowsGroup = AllowsGroup,
            CanAbandon = CanAbandon,
            Priority = Priority,
            TitleColor = TitleColor
        };
    }
    #endregion
}

/// <summary>
/// Categorias de quests para organização
/// </summary>
public enum QuestCategory
{
    MainStory,      // História principal
    SideQuest,      // Missões secundárias
    Daily,          // Diárias
    Weekly,         // Semanais
    Event,          // Eventos especiais
    Raid,           // Para grupos grandes
    PvP,            // Player vs Player
    Crafting,       // Relacionadas a profissões
    Exploration,    // Exploração do mundo
    Achievement,    // Conquistas/achievements
    Tutorial        // Ensinar mecânicas
}

/// <summary>
/// Cores do título da quest na interface
/// </summary>
public enum QuestTitleColor
{
    Normal,         // Branco/padrão
    Important,      // Amarelo
    Urgent,         // Laranja
    Critical,       // Vermelho
    Epic,           // Roxo
    Legendary       // Dourado
}

/// <summary>
/// Tipos de facção para restrições de quest
/// </summary>
public enum FactionType
{
    All,            // Qualquer facção
    Nuian,          // Facção Nuian
    Haranya,        // Facção Haranya
    Pirate,         // Piratas
    Neutral         // NPCs neutros
}

/// <summary>
/// Tipos de classe para restrições de quest
/// </summary>
public enum ClassType
{
    All,            // Qualquer classe
    Warrior,        // Guerreiro
    Mage,           // Mago
    Archer,         // Arqueiro
    // ... outras classes do ArcheAge
}
```

### **2.3 Entendendo Cada Campo da Quest**

Vamos analisar CADA campo da nossa classe Quest e entender o **porquê** existe:

#### **Identificação Básica - Por que cada campo é importante:**

1. **Id**: Como um RG, identifica unicamente a quest
2. **Title**: O "nome artístico" que atrai o jogador
3. **Description**: A história que motiva a ação
4. **AcceptText**: O "convencimento" do NPC
5. **CompleteText**: A "celebração" da conquista
6. **DeclineText**: A "segunda chance" de aceitar
7. **Category**: Organização visual na interface

#### **Pré-requisitos - O Sistema de Portas:**

Imagine os pré-requisitos como **portas trancadas**:
- **MinLevel**: Porta que só abre com "força" suficiente
- **PrerequisiteQuests**: Portas que precisam de "chaves" (outras quests)
- **RequiredItems**: Portas que precisam de "ferramentas" (itens)
- **AllowedClass**: Portas que só reconhecem certas "identidades"
- **AllowedFaction**: Portas com "controle de acesso" político

#### **Configurações Temporais - O Elemento Tempo:**

- **IsDaily/IsWeekly**: Como um "trabalho" que você pode fazer periodicamente
- **StartDate/EndDate**: Como um "evento" com data marcada
- **IsEventQuest**: Como uma "festa" especial

#### **Configurações de Grupo - A Dinâmica Social:**

- **AllowsGroup**: Se permite "trabalho em equipe"
- **MinGroupSize**: Quantas pessoas **obrigatoriamente** devem participar
- **MaxGroupSize**: O limite de "multidão"
- **RequireGroupProximity**: Se todos devem estar "juntos fisicamente"

### **2.4 Fluxo de Vida de uma Quest**

Uma quest passa por várias **fases** na vida de um jogador:

```
1. CRIAÇÃO     → Quest é definida pelo designer
2. CARREGAMENTO → Quest é carregada do banco para memória
3. DESCOBERTA  → Jogador encontra NPC que oferece
4. AVALIAÇÃO   → Sistema verifica se jogador pode aceitar
5. OFERTA      → Quest é mostrada ao jogador
6. ACEITAÇÃO   → Jogador decide aceitar
7. ATIVAÇÃO    → Quest entra na lista ativa do jogador
8. EXECUÇÃO    → Jogador trabalha nos objetivos
9. PROGRESSO   → Sistema atualiza progresso automaticamente
10. COMPLETUDE → Todos objetivos são atingidos
11. ENTREGA    → Jogador retorna ao NPC (se necessário)
12. RECOMPENSA → Sistema dá os prêmios
13. FINALIZAÇÃO→ Quest sai da lista ativa
14. ARQUIVO    → Quest vai para lista de completadas
```

Cada fase tem suas **validações** e **side effects**.

---

## 🎯 **PARTE 3: SISTEMA DE OBJETIVOS - O CORAÇÃO DAS QUESTS**

### **3.1 A Filosofia dos Objetivos**

Um objetivo é como uma **tarefa bem definida** na vida real:
- **Específico**: "Mate 10 lobos" (não "mate alguns lobos")
- **Mensurável**: Progresso visível (7/10 lobos mortos)
- **Alcançável**: Não é impossível para o level do jogador
- **Relevante**: Faz sentido no contexto da história
- **Temporal**: Pode ser completado em tempo razoável

### **3.2 Arquitetura Técnica dos Objetivos**

Cada quest tem **objetivos** - tarefas específicas que o jogador deve cumprir:

```csharp
public class QuestObjective
{
    public uint Id { get; set; }
    
    // Texto que aparece no log de quest
    public string Description { get; set; }
    
    // Tipo de objetivo (matar, coletar, etc.)
    public QuestObjectiveType Type { get; set; }
    
    // ID do que deve ser feito (monstro, item, npc)
    public uint TargetId { get; set; }
    
    // Quantos precisa (ex: 10 lobos)
    public int RequiredAmount { get; set; }
    
    // Quantos já foram feitos
    public int CurrentAmount { get; set; }
    
    // Posição específica (para objectives de localização)
    public Vector3 TargetPosition { get; set; }
    
    // Raio de alcance da posição
    public float TargetRadius { get; set; }
    
    // Se está completo
    public bool IsCompleted => CurrentAmount >= RequiredAmount;
    
    // Atualiza progresso do objetivo
    public void UpdateProgress(uint targetId, int amount = 1)
    {
        // Só atualiza se for o alvo correto
        if (targetId == TargetId && !IsCompleted)
        {
            CurrentAmount = Math.Min(CurrentAmount + amount, RequiredAmount);
        }
    }
    
    // Verifica se jogador está na posição correta
    public bool IsAtCorrectLocation(Vector3 playerPosition)
    {
        if (Type != QuestObjectiveType.ReachLocation)
            return true;
            
        float distance = Vector3.Distance(playerPosition, TargetPosition);
        return distance <= TargetRadius;
    }
}

// Todos os tipos possíveis de objetivos
public enum QuestObjectiveType
{
    KillMonster,     // Matar X monstros específicos
    CollectItem,     // Coletar X itens específicos
    TalkToNpc,       // Falar com NPC específico
    ReachLocation,   // Chegar em local específico
    UseItem,         // Usar item específico
    CastSkill,       // Usar habilidade específica
    EquipItem,       // Equipar item específico
    CraftItem,       // Craftar item específico
    GainExperience,  // Ganhar X de experiência
    GainLevel,       // Subir de level
    JoinGuild,       // Entrar em guilda
    PvpKill,         // Matar jogador em PvP
    CompleteQuest,   // Completar outra quest
    OpenContainer,   // Abrir baú/container
    HarvestResource, // Coletar recurso (minerar, etc.)
    DeliverItem,     // Entregar item para NPC
    EscortNpc,       // Escoltar NPC até local
    DefendArea,      // Defender área por tempo
    Survive          // Sobreviver por tempo
}
```

---

## 🏆 **Sistema de Recompensas - Como Premiar o Jogador**

Quando completamos uma quest, ganhamos prêmios! Vamos criar um sistema justo:

```csharp
public class QuestReward
{
    public QuestRewardType Type { get; set; }
    public uint ItemId { get; set; }       // Para recompensas de item
    public int Amount { get; set; }        // Quantidade
    public uint Experience { get; set; }   // EXP ganha
    public uint Gold { get; set; }         // Dinheiro ganho
    public uint SkillPoints { get; set; }  // Pontos de habilidade
    
    // Aplica a recompensa ao jogador
    public void GiveToPlayer(Character player)
    {
        switch (Type)
        {
            case QuestRewardType.Experience:
                player.GainExperience(Experience);
                break;
                
            case QuestRewardType.Gold:
                player.Currency.AddGold(Gold);
                break;
                
            case QuestRewardType.Item:
                var item = ItemManager.CreateItem(ItemId, Amount);
                if (!player.Inventory.AddItem(item))
                {
                    // Se inventário cheio, envia por correio
                    MailManager.SendItemByMail(player, item, "Quest Reward");
                }
                break;
                
            case QuestRewardType.SkillPoints:
                player.SkillPoints += SkillPoints;
                break;
        }
        
        // Log da recompensa
        Logger.Info($"Player {player.Name} received quest reward: {Type} - {Amount}");
    }
}

public enum QuestRewardType
{
    Experience,    // Experiência
    Gold,         // Dinheiro
    Item,         // Item específico
    SkillPoints,  // Pontos de habilidade
    Reputation,   // Reputação com facção
    Title,        // Título para o jogador
    Achievement   // Conquista/achievement
}
```

---

## 🧠 **Quest Manager - O Cérebro do Sistema**

Agora vamos criar o gerenciador que coordena tudo:

```csharp
public class QuestManager
{
    private static readonly Dictionary<uint, Quest> _questTemplates = new();
    private static readonly Dictionary<uint, List<uint>> _questsByLevel = new();
    private static readonly Dictionary<uint, List<uint>> _questsByNpc = new();
    
    // Carrega todas as quests do banco de dados
    public static async Task LoadQuests()
    {
        Logger.Info("Carregando quests do banco de dados...");
        
        var quests = await DatabaseManager.GetAllQuests();
        
        foreach (var quest in quests)
        {
            _questTemplates[quest.Id] = quest;
            
            // Indexa por level para facilitar busca
            if (!_questsByLevel.ContainsKey(quest.MinLevel))
                _questsByLevel[quest.MinLevel] = new List<uint>();
            _questsByLevel[quest.MinLevel].Add(quest.Id);
            
            // Indexa por NPC que dá a quest
            if (!_questsByNpc.ContainsKey(quest.GiverNpcId))
                _questsByNpc[quest.GiverNpcId] = new List<uint>();
            _questsByNpc[quest.GiverNpcId].Add(quest.Id);
        }
        
        Logger.Info($"Carregadas {quests.Count} quests com sucesso!");
    }
    
    // Pega quest pelo ID
    public static Quest GetQuest(uint questId)
    {
        _questTemplates.TryGetValue(questId, out Quest quest);
        return quest;
    }
    
    // Pega quests disponíveis para um jogador
    public static List<Quest> GetAvailableQuests(Character player)
    {
        var availableQuests = new List<Quest>();
        
        foreach (var quest in _questTemplates.Values)
        {
            if (quest.CanAccept(player))
            {
                availableQuests.Add(quest);
            }
        }
        
        return availableQuests;
    }
    
    // Jogador aceita uma quest
    public static bool AcceptQuest(Character player, uint questId)
    {
        var quest = GetQuest(questId);
        if (quest == null || !quest.CanAccept(player))
            return false;
        
        // Cria instância ativa da quest para o jogador
        var activeQuest = new ActiveQuest
        {
            QuestId = questId,
            PlayerId = player.Id,
            AcceptedAt = DateTime.Now,
            Objectives = quest.Objectives.Select(obj => new ActiveQuestObjective
            {
                ObjectiveId = obj.Id,
                CurrentAmount = 0,
                IsCompleted = false
            }).ToList()
        };
        
        player.ActiveQuests.Add(activeQuest);
        
        // Salva no banco de dados
        DatabaseManager.SaveActiveQuest(activeQuest);
        
        // Notifica o cliente
        player.SendPacket(new QuestAcceptedPacket(questId));
        
        Logger.Info($"Player {player.Name} accepted quest: {quest.Title}");
        return true;
    }
    
    // Atualiza progresso de quest
    public static void UpdateQuestProgress(Character player, QuestObjectiveType type, uint targetId, int amount = 1)
    {
        foreach (var activeQuest in player.ActiveQuests)
        {
            var quest = GetQuest(activeQuest.QuestId);
            if (quest == null) continue;
            
            for (int i = 0; i < quest.Objectives.Count; i++)
            {
                var objective = quest.Objectives[i];
                var activeObjective = activeQuest.Objectives[i];
                
                // Verifica se este objetivo corresponde ao evento
                if (objective.Type == type && objective.TargetId == targetId && !activeObjective.IsCompleted)
                {
                    activeObjective.CurrentAmount = Math.Min(
                        activeObjective.CurrentAmount + amount, 
                        objective.RequiredAmount
                    );
                    
                    // Marca como completo se necessário
                    if (activeObjective.CurrentAmount >= objective.RequiredAmount)
                    {
                        activeObjective.IsCompleted = true;
                        
                        // Notifica jogador
                        player.SendPacket(new QuestObjectiveCompletedPacket(activeQuest.QuestId, objective.Id));
                        
                        Logger.Info($"Player {player.Name} completed objective: {objective.Description}");
                    }
                    
                    // Atualiza progresso no cliente
                    player.SendPacket(new QuestProgressPacket(activeQuest.QuestId, objective.Id, activeObjective.CurrentAmount));
                    
                    // Salva progresso
                    DatabaseManager.UpdateQuestProgress(activeQuest);
                }
            }
            
            // Verifica se a quest toda foi completada
            if (quest.IsCompleted() && activeQuest.Objectives.All(obj => obj.IsCompleted))
            {
                CompleteQuest(player, activeQuest.QuestId);
            }
        }
    }
    
    // Completa uma quest
    public static void CompleteQuest(Character player, uint questId)
    {
        var quest = GetQuest(questId);
        var activeQuest = player.ActiveQuests.FirstOrDefault(q => q.QuestId == questId);
        
        if (quest == null || activeQuest == null)
            return;
        
        // Remove da lista de quests ativas
        player.ActiveQuests.Remove(activeQuest);
        
        // Adiciona à lista de quests completadas
        player.CompletedQuests.Add(questId);
        
        // Dá as recompensas
        foreach (var reward in quest.Rewards)
        {
            reward.GiveToPlayer(player);
        }
        
        // Salva no banco
        DatabaseManager.CompleteQuest(player.Id, questId);
        
        // Notifica o cliente
        player.SendPacket(new QuestCompletedPacket(questId, quest.Rewards));
        
        Logger.Info($"Player {player.Name} completed quest: {quest.Title}");
        
        // Verifica se desbloqueou novas quests
        CheckUnlockedQuests(player);
    }
    
    // Verifica se completar uma quest desbloqueou outras
    private static void CheckUnlockedQuests(Character player)
    {
        var newQuests = GetAvailableQuests(player);
        if (newQuests.Any())
        {
            player.SendPacket(new NewQuestsAvailablePacket(newQuests.Select(q => q.Id).ToList()));
        }
    }
}
```

---

## 🔗 **Integração com Outros Sistemas**

As quests não existem isoladamente. Elas se conectam com TUDO no jogo:

```csharp
// Integração com sistema de combate
public class CombatSystem
{
    public void OnMonsterKilled(Character killer, Npc monster)
    {
        // Atualiza quests de "matar monstros"
        QuestManager.UpdateQuestProgress(killer, QuestObjectiveType.KillMonster, monster.TemplateId);
        
        // Se estava em grupo, atualiza para todos do grupo
        if (killer.Group != null)
        {
            foreach (var member in killer.Group.Members)
            {
                if (Vector3.Distance(member.Position, killer.Position) <= 50f) // 50 metros
                {
                    QuestManager.UpdateQuestProgress(member, QuestObjectiveType.KillMonster, monster.TemplateId);
                }
            }
        }
    }
}

// Integração com sistema de itens
public class ItemSystem
{
    public void OnItemObtained(Character player, uint itemId, int amount)
    {
        // Atualiza quests de "coletar itens"
        QuestManager.UpdateQuestProgress(player, QuestObjectiveType.CollectItem, itemId, amount);
    }
    
    public void OnItemCrafted(Character player, uint itemId, int amount)
    {
        // Atualiza quests de "craftar itens"
        QuestManager.UpdateQuestProgress(player, QuestObjectiveType.CraftItem, itemId, amount);
    }
}

// Integração com sistema de NPCs
public class NpcSystem
{
    public void OnNpcTalk(Character player, uint npcId)
    {
        // Atualiza quests de "falar com NPC"
        QuestManager.UpdateQuestProgress(player, QuestObjectiveType.TalkToNpc, npcId);
        
        // Mostra quests disponíveis deste NPC
        var availableQuests = QuestManager.GetQuestsFromNpc(npcId, player);
        if (availableQuests.Any())
        {
            player.SendPacket(new NpcQuestListPacket(npcId, availableQuests));
        }
    }
}
```

---

## 💻 **Exemplo Prático - Criando sua Primeira Quest**

Vamos criar uma quest simples do zero para você entender na prática:

```csharp
public static Quest CreateFirstQuest()
{
    return new Quest
    {
        Id = 1001,
        Title = "Bem-vindo ao ArcheAge!",
        Description = "Olá, jovem aventureiro! Para começar sua jornada, preciso que você prove sua coragem. Vá até a floresta próxima e derrote 5 Lobos Selvagens. Cuidado, eles são mais perigosos do que parecem!",
        AcceptText = "Aceito o desafio! Vou derrotar esses lobos!",
        CompleteText = "Excelente trabalho, aventureiro! Você provou que tem potencial. Aqui está sua recompensa.",
        
        MinLevel = 1,
        MaxLevel = 5,
        AllowedClass = ClassType.All,
        IsDaily = false,
        AllowsGroup = true,
        
        PrerequisiteQuests = new List<uint>(), // Nenhuma quest necessária
        
        Objectives = new List<QuestObjective>
        {
            new QuestObjective
            {
                Id = 1,
                Description = "Derrote Lobos Selvagens: {current}/{required}",
                Type = QuestObjectiveType.KillMonster,
                TargetId = 101, // ID do Lobo Selvagem
                RequiredAmount = 5,
                CurrentAmount = 0
            }
        },
        
        Rewards = new List<QuestReward>
        {
            new QuestReward
            {
                Type = QuestRewardType.Experience,
                Experience = 150
            },
            new QuestReward
            {
                Type = QuestRewardType.Gold,
                Gold = 25
            },
            new QuestReward
            {
                Type = QuestRewardType.Item,
                ItemId = 501, // Poção de Cura Menor
                Amount = 3
            }
        }
    };
}
```

---

## 🧪 **Testes e Debugging do Sistema de Quests**

Como testar se suas quests funcionam corretamente:

```csharp
[Test]
public void Quest_ShouldComplete_WhenAllObjectivesCompleted()
{
    // Arrange
    var player = TestHelper.CreateTestPlayer();
    var quest = CreateFirstQuest();
    
    // Act - Jogador aceita a quest
    QuestManager.AcceptQuest(player, quest.Id);
    
    // Simula matar 5 lobos
    for (int i = 0; i < 5; i++)
    {
        QuestManager.UpdateQuestProgress(player, QuestObjectiveType.KillMonster, 101);
    }
    
    // Assert
    Assert.IsTrue(player.CompletedQuests.Contains(quest.Id));
    Assert.AreEqual(150, player.Experience); // Ganhou EXP
    Assert.AreEqual(25, player.Currency.Gold); // Ganhou ouro
}

[Test]
public void Quest_ShouldNotAccept_WhenPlayerLevelTooLow()
{
    // Arrange
    var player = TestHelper.CreateTestPlayer(level: 0); // Level muito baixo
    var quest = CreateFirstQuest(); // Precisa level 1
    
    // Act
    bool accepted = QuestManager.AcceptQuest(player, quest.Id);
    
    // Assert
    Assert.IsFalse(accepted);
    Assert.IsFalse(player.ActiveQuests.Any());
}
```

---

## 🎓 **RESUMO DO MÓDULO 12**

Agora você sabe TUDO sobre o sistema de quests:
- ✅ Como criar quests complexas
- ✅ Sistema de objetivos flexível  
- ✅ Recompensas balanceadas
- ✅ Integração com todos os sistemas do jogo
- ✅ Testes e debugging

O sistema de quests é o coração da experiência do jogador. Com este conhecimento, você pode criar aventuras épicas que vão manter os jogadores engajados por horas! 🎮

---

**🎯 PRÓXIMO PASSO:** Módulo 13 - Sistema Econômico