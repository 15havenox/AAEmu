# 🎓 MEGA CURSO ULTRA DETALHADO: MÓDULO 13
## 📜 SISTEMA DE QUESTS E NPCS - DISSECAÇÃO COMPLETA

---

## 🎯 **BEM-VINDO AO MÓDULO 13!**

Agora vamos mergulhar no sistema mais complexo e interessante do AAEmu: **QUESTS E NPCs**! 

É como entender como funciona um diretor de cinema coordenando atores em um filme gigante! 🎬

---

## 📖 **O QUE VOCÊ VAI APRENDER NESTE MÓDULO**

✅ **Como funcionam as Quests** linha por linha  
✅ **Sistema de NPCs** e comportamento  
✅ **Interação Quest ↔ NPC** detalhada  
✅ **Sistema de AI** dos NPCs  
✅ **Quest Scripting** avançado  
✅ **Como criar Quests do zero**  

---

## 📜 **CAPÍTULO 1: ANATOMIA DE UMA QUEST**

### **O que é uma Quest? (Explicação de Criança)**

**👶 Explicação:** Uma quest é como uma **"missão especial"** que um NPC te dá:

- **📋 Lista de tarefas**: "Mate 5 lobos, colete 3 maçãs"
- **🎁 Recompensa**: "Te dou 100 de ouro e uma espada"
- **📚 História**: "Os lobos estão atacando nossa vila!"

É como quando sua mãe te dá uma lista de compras no supermercado e promete te dar dinheiro se você fizer tudo certinho!

### **Arquivo: AAEmu.Game/Models/Game/Quests/Quest.cs (DNA da Quest)**

Vamos dissecar esta classe linha por linha:

```csharp
public partial class Quest : PacketMarshaler
{
    private const int MaxObjectiveCount = 5;
    private readonly IQuestManager _questManager;
    private readonly ITaskManager _taskManager;
    private readonly ISkillManager _skillManager;
    private readonly IExpressTextManager _expressTextManager;
    private readonly IWorldManager _worldManager;
    private QuestComponentKind _step;
```

**🤔 Vamos explicar cada parte:**

**"partial class Quest"**
- **partial** = Esta classe está dividida em vários arquivos (como um livro em volumes)
- **PacketMarshaler** = Sabe como se transformar em dados para enviar pela rede

**👶 Explicação:** É como uma receita de bolo gigante que foi dividida em várias páginas para não ficar confuso.

**Constantes e Dependências:**

1. **MaxObjectiveCount = 5** = Uma quest pode ter no máximo 5 objetivos
2. **_questManager** = Gerente de todas as quests
3. **_taskManager** = Gerente de tarefas agendadas
4. **_skillManager** = Gerente de habilidades
5. **_expressTextManager** = Gerente de textos de diálogo
6. **_worldManager** = Gerente do mundo

**👶 Explicação:** É como ter vários ajudantes especializados para cada parte da missão!

#### **Propriedades Principais da Quest**

```csharp
/// <summary>
/// DB ID
/// </summary>
public long Id { get; set; }

/// <summary>
/// Quest Template Id
/// </summary>
public uint TemplateId { get; set; }

/// <summary>
/// Quest Template
/// </summary>
public IQuestTemplate Template { get; set; }
```

**🤔 Diferença entre Id, TemplateId e Template:**

- **Id** = Número único DESTA quest específica no banco de dados
- **TemplateId** = Tipo da quest (referência ao SQLite)
- **Template** = Informações completas da quest (nome, descrição, recompensas)

**👶 Explicação com analogia:**
- **Id** = Seu RG pessoal (único no mundo)
- **TemplateId** = Modelo do carro que você tem (ex: "Honda Civic 2020")
- **Template** = Manual completo do Honda Civic (especificações, instruções)

#### **Sistema de Objetivos**

```csharp
/// <summary>
/// Objective counters for the Progress step
/// </summary>
internal int[] Objectives { get; set; }

/// <summary>
/// Used to check Progress step
/// </summary>
public List<bool> ProgressStepResults { get; set; } = [];
```

**🤔 Como funcionam os objetivos:**

**Objectives** = Array de contadores
- Objectives[0] = Quantos lobos você matou (objetivo: 5)
- Objectives[1] = Quantas maçãs você coletou (objetivo: 3)
- Objectives[2] = Quantos NPCs você falou (objetivo: 1)

**ProgressStepResults** = Array de "completou ou não"
- ProgressStepResults[0] = true (matou 5 lobos ✅)
- ProgressStepResults[1] = false (só coletou 2 maçãs ❌)
- ProgressStepResults[2] = true (falou com NPC ✅)

**👶 Explicação:** É como uma lista de compras onde você vai riscando o que já pegou!

#### **Estados da Quest**

```csharp
/// <summary>
/// Current Quest Status
/// </summary>
public QuestStatus Status { get; set; }

/// <summary>
/// Current Quest Step
/// </summary>
public QuestComponentKind Step
{
    get => _step;
    set => SetStep(value);
}
```

**🔍 Estados possíveis de uma Quest:**

```csharp
public enum QuestStatus
{
    None = 0,           // Não existe
    Active = 1,         // Ativa (fazendo)
    Completed = 2,      // Completa (pronta para entregar)
    Dropped = 3,        // Abandonada
    Ready = 4           // Pronta para pegar
}

public enum QuestComponentKind
{
    None = 0,           // Nenhuma etapa
    Start = 1,          // Início (pegar a quest)
    Supply = 2,         // Suprimentos (dar itens)
    Progress = 3,       // Progresso (fazer tarefas)
    Ready = 4,          // Pronta (completou objetivos)
    Reward = 5,         // Recompensa (entregar)
    Drop = 6            // Abandonar
}
```

**👶 Explicação do fluxo:**
1. **Start**: "Oi, você quer esta missão?"
2. **Supply**: "Aqui, leve estes itens para ajudar"
3. **Progress**: "Agora vá fazer as tarefas!"
4. **Ready**: "Você terminou tudo! Volte aqui!"
5. **Reward**: "Parabéns! Aqui está sua recompensa!"

#### **Sistema de Tempo**

```csharp
/// <summary>
/// End time for timed quests
/// </summary>
public DateTime Time { get; set; }

/// <summary>
/// Remaining time for this quest in milliseconds
/// </summary>
private int LeftTime => Time > DateTime.UtcNow ? (int)(Time - DateTime.UtcNow).TotalMilliseconds : -1;
```

**🤔 Por que algumas quests têm tempo limite?**

Algumas quests são **urgentes**:
- "Salve a vila em 30 minutos!"
- "Entregue esta carta antes do pôr do sol!"

**👶 Explicação:** É como quando o professor dá uma prova e diz "vocês têm 1 hora para terminar!"

---

## 🤖 **CAPÍTULO 2: SISTEMA DE NPCS - OS ATORES DO JOGO**

### **Arquivo: AAEmu.Game/Core/Managers/UnitManagers/NpcManager.cs (O Diretor de Cinema)**

Vamos dissecar como funciona o sistema de NPCs:

```csharp
public class NpcManager : Singleton<NpcManager>
{
    private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    private bool _loaded = false;

    private Dictionary<uint, NpcTemplate> _templates;
    private Dictionary<uint, MerchantGoods> _goods;
    private Dictionary<uint, TotalCharacterCustom> _totalCharacterCustoms;
    private Dictionary<uint, Dictionary<uint, List<BodyPartTemplate>>> _itemBodyParts;
    private Dictionary<uint, List<uint>> _tccLookup;
    private Random _loadCustomRandom = new(123456789);
    public Dictionary<uint, NpcSpawnerNpc> _npcSpawnerNpc;
    public Dictionary<uint, NpcSpawnerTemplate> _npcSpawners;
    public Dictionary<uint, List<uint>> _npcMemberAndSpawnerId;
    private static Dictionary<uint, Creature> _creatures = new();
```

**🤔 Vamos explicar cada "gaveta" do gerente:**

1. **_templates** = Moldes de todos os NPCs (como eles são)
2. **_goods** = O que cada mercador vende
3. **_totalCharacterCustoms** = Aparências customizadas
4. **_itemBodyParts** = Partes do corpo que podem usar itens
5. **_npcSpawners** = Onde e quando NPCs aparecem
6. **_creatures** = Informações de criatura

**👶 Explicação:** É como um diretor de cinema que tem:
- **Fichas dos atores** (templates)
- **Lista de produtos** que cada loja vende
- **Figurinos** disponíveis
- **Cronograma** de quando cada ator entra em cena

#### **Criando um NPC (Nascimento de um Ator)**

```csharp
public Npc Create(WorldInstance parentWorld, uint objectId, uint id)
{
    var template = GetTemplate(id);
    if (template == null)
    {
        return null;
    }

    var npc = new Npc();
    npc.ParentWorld = parentWorld;
    npc.ObjId = objectId > 0 ? objectId : ObjectIdManager.Instance.GetNextId();
    npc.TemplateId = id; // duplicate Id
    npc.Id = id;
    npc.Template = template;
    npc.ModelId = template.ModelId;
    npc.CanFly = ModelManager.Instance.IsFlyOrSwim(template.ModelId);
    npc.Faction = FactionManager.Instance.GetFaction(template.FactionId);
    npc.Level = template.Level;
    npc.Patrol = null;
```

**🔍 Vamos explicar cada linha:**

**Linha 3-7: Validação**
```csharp
var template = GetTemplate(id);
if (template == null)
{
    return null;
}
```
**Tradução:** "Busque o molde deste NPC. Se não existir, não crie nada."

**Linha 9-17: Criação Base**
```csharp
var npc = new Npc();
npc.ParentWorld = parentWorld;
npc.ObjId = objectId > 0 ? objectId : ObjectIdManager.Instance.GetNextId();
```

**👶 Explicação:**
1. **new Npc()** = "Crie um ator novo"
2. **ParentWorld** = "Em que mundo ele vai atuar"
3. **ObjId** = "Dê um número único para ele" (como RG)

**Linha 11-16: Propriedades Base**
```csharp
npc.TemplateId = id;
npc.Template = template;
npc.ModelId = template.ModelId;
npc.CanFly = ModelManager.Instance.IsFlyOrSwim(template.ModelId);
npc.Faction = FactionManager.Instance.GetFaction(template.FactionId);
npc.Level = template.Level;
```

**👶 Explicação:**
1. **TemplateId/Template** = "Que tipo de ator é"
2. **ModelId** = "Como ele parece visualmente"
3. **CanFly** = "Ele pode voar ou nadar?"
4. **Faction** = "De que time ele é"
5. **Level** = "Qual o nível dele"

#### **Sistema de Equipamentos do NPC**

```csharp
SetEquipItemTemplate(npc, template.Items.Headgear, EquipmentItemSlot.Head);
SetEquipItemTemplate(npc, template.Items.Necklace, EquipmentItemSlot.Neck);
SetEquipItemTemplate(npc, template.Items.Shirt, EquipmentItemSlot.Chest);
SetEquipItemTemplate(npc, template.Items.Belt, EquipmentItemSlot.Waist);
SetEquipItemTemplate(npc, template.Items.Pants, EquipmentItemSlot.Legs);
SetEquipItemTemplate(npc, template.Items.Gloves, EquipmentItemSlot.Hands);
SetEquipItemTemplate(npc, template.Items.Shoes, EquipmentItemSlot.Feet);
// ... e muitos outros
```

**👶 Explicação:** É como um figurinista vestindo o ator:
- "Coloque este chapéu na cabeça"
- "Coloque este colar no pescoço"
- "Coloque esta camisa no peito"

Cada NPC usa os equipamentos definidos no seu template!

#### **Sistema de Vida e Status**

```csharp
npc.InitializeSpawnBuffs();
npc.UpdateGearBonuses(null, null);

npc.Hp = npc.MaxHp;
npc.Mp = npc.MaxMp;
```

**👶 Explicação:**
1. **InitializeSpawnBuffs()** = "Ative efeitos especiais iniciais"
2. **UpdateGearBonuses()** = "Calcule bônus dos equipamentos"
3. **Hp = MaxHp** = "Inicie com vida cheia"
4. **Mp = MaxMp** = "Inicie com mana cheia"

#### **Sistema de Inteligência Artificial**

```csharp
if (npc.Template.AiFileId > 0)
{
    var ai = AIUtils.GetAiByType((AiParamType)npc.Template.AiFileId, npc);
    if (ai == null)
        return npc;

    npc.Ai = ai;
    AIManager.Instance.AddAi(ai);
    npc.Ai.Start();
}
```

**🤔 O que é AI (Inteligência Artificial)?**

É o "cérebro" do NPC que decide o que ele faz:
- **Merchant AI** = Fica parado vendendo itens
- **Guard AI** = Patrulha uma área, ataca inimigos
- **Aggressive AI** = Ataca jogadores que se aproximam
- **Friendly AI** = Apenas conversa com jogadores

**👶 Explicação:** É como dar um "roteiro" para cada ator:
- **Vendedor**: "Fique parado e venda produtos"
- **Guarda**: "Ande pela cidade e proteja"
- **Monstro**: "Ataque quem se aproximar"

---

## 🎭 **CAPÍTULO 3: INTERAÇÃO QUEST ↔ NPC**

Agora vamos ver como Quests e NPCs trabalham juntos! É a parte mais mágica! ✨

### **Fluxo Completo: Da Criação à Conclusão**

#### **1. Jogador Fala com NPC Quest Giver**

```csharp
// Pacote enviado quando jogador clica no NPC
public class CSInteractNPCPacket : GamePacket
{
    public uint NpcObjId { get; set; }
    public uint Type { get; set; }  // Tipo de interação
    
    public override void Read(PacketStream stream)
    {
        NpcObjId = stream.ReadUInt32();
        Type = stream.ReadUInt32();
    }
    
    public override void Execute()
    {
        var character = Connection.ActiveCharacter;
        if (character == null) return;
        
        var npc = WorldManager.Instance.GetNpc(NpcObjId);
        if (npc == null) return;
        
        // Processa interação
        NpcManager.Instance.HandleNpcInteraction(character, npc, Type);
    }
}
```

**👶 Explicação:** É como ir numa loja e falar "Oi, posso ver seus produtos?"

#### **2. Sistema Verifica Quests Disponíveis**

```csharp
public void HandleNpcInteraction(Character character, Npc npc, uint interactionType)
{
    // Verifica se NPC tem quests para oferecer
    var availableQuests = GetAvailableQuestsForNpc(npc.TemplateId, character);
    
    // Verifica se jogador tem quests para entregar neste NPC
    var completableQuests = GetCompletableQuestsForNpc(npc.TemplateId, character);
    
    // Monta menu de interação
    var interactionMenu = new NpcInteractionMenu();
    
    // Adiciona opções de novas quests
    foreach (var quest in availableQuests)
    {
        interactionMenu.AddOption(new QuestStartOption(quest));
    }
    
    // Adiciona opções de quests para entregar
    foreach (var quest in completableQuests)
    {
        interactionMenu.AddOption(new QuestCompleteOption(quest));
    }
    
    // Verifica se é vendedor
    if (npc.Template.MerchantGoods != null)
    {
        interactionMenu.AddOption(new MerchantOption());
    }
    
    // Envia menu para jogador
    SendInteractionMenu(character, interactionMenu);
}

private List<QuestTemplate> GetAvailableQuestsForNpc(uint npcId, Character character)
{
    var availableQuests = new List<QuestTemplate>();
    
    // Busca todas as quests que este NPC pode dar
    var npcQuests = QuestManager.Instance.GetQuestsByStartNpc(npcId);
    
    foreach (var questTemplate in npcQuests)
    {
        // Verifica se jogador pode pegar esta quest
        if (CanCharacterStartQuest(character, questTemplate))
        {
            availableQuests.Add(questTemplate);
        }
    }
    
    return availableQuests;
}

private bool CanCharacterStartQuest(Character character, QuestTemplate quest)
{
    // Verifica nível mínimo
    if (character.Level < quest.MinLevel)
        return false;
        
    // Verifica nível máximo
    if (quest.MaxLevel > 0 && character.Level > quest.MaxLevel)
        return false;
        
    // Verifica raça
    if (quest.RaceRequired != Race.All && character.Race != quest.RaceRequired)
        return false;
        
    // Verifica se já tem esta quest
    if (character.Quests.HasQuest(quest.Id))
        return false;
        
    // Verifica se já completou esta quest (e não é repetível)
    if (!quest.Repeatable && character.Quests.HasCompletedQuest(quest.Id))
        return false;
        
    // Verifica quests pré-requisitos
    foreach (var prerequisiteId in quest.Prerequisites)
    {
        if (!character.Quests.HasCompletedQuest(prerequisiteId))
            return false;
    }
    
    return true;
}
```

**👶 Explicação:** É como quando você vai numa escola se inscrever:
1. **Verifica idade** (nível mínimo/máximo)
2. **Verifica documentos** (raça permitida)
3. **Verifica se já é aluno** (não tem quest repetida)
4. **Verifica se formou** (não completou quest não-repetível)
5. **Verifica pré-requisitos** (terminou matérias anteriores)

#### **3. Jogador Escolhe Iniciar uma Quest**

```csharp
public class CSStartQuestContextPacket : GamePacket
{
    public uint QuestContextId { get; set; }  // ID da quest
    public uint NpcObjId { get; set; }        // NPC que está dando
    public uint Selected { get; set; }        // Opção escolhida
    
    public override void Execute()
    {
        var character = Connection.ActiveCharacter;
        var npc = WorldManager.Instance.GetNpc(NpcObjId);
        
        if (character == null || npc == null) return;
        
        // Inicia a quest
        QuestManager.Instance.StartQuest(character, QuestContextId, npc);
    }
}
```

#### **4. Sistema Inicia a Quest**

```csharp
public void StartQuest(Character character, uint questId, Npc questGiver)
{
    var template = GetTemplate(questId);
    if (template == null)
    {
        Logger.Error($"Quest template {questId} não encontrada!");
        return;
    }
    
    // Verifica se pode iniciar (dupla verificação)
    if (!CanCharacterStartQuest(character, template))
    {
        SendQuestError(character, QuestError.CannotStart);
        return;
    }
    
    // Cria instância da quest
    var quest = new Quest
    {
        Id = QuestIdManager.Instance.GetNextId(),
        TemplateId = questId,
        Template = template,
        Owner = character,
        Status = QuestStatus.Active,
        Step = QuestComponentKind.Start,
        Objectives = new int[MaxObjectiveCount],
        ProgressStepResults = new List<bool>(),
        Time = template.TimeLimit > 0 ? DateTime.UtcNow.AddMinutes(template.TimeLimit) : DateTime.MaxValue,
        DoodadId = questGiver?.ObjId ?? 0
    };
    
    // Adiciona quest ao jogador
    character.Quests.AddQuest(quest);
    
    // Salva no banco de dados
    SaveQuest(quest);
    
    // Processa componente inicial (Supply)
    ProcessQuestComponent(quest, QuestComponentKind.Supply);
    
    // Informa o jogador
    SendQuestStarted(character, quest);
    
    // Log
    Logger.Info($"Quest '{template.Name}' iniciada por {character.Name}");
}
```

**👶 Explicação:** É como se inscrever numa aula:
1. **Verifica se a matéria existe**
2. **Verifica se você pode se inscrever**
3. **Cria sua "matrícula"** (instância da quest)
4. **Te adiciona na lista de alunos**
5. **Salva no sistema da escola**
6. **Te dá material inicial** (Supply)
7. **Confirma sua inscrição**

#### **5. Sistema de Supply (Dando Itens Iniciais)**

```csharp
private void ProcessQuestComponent(Quest quest, QuestComponentKind component)
{
    switch (component)
    {
        case QuestComponentKind.Supply:
            ProcessSupplyComponent(quest);
            break;
        case QuestComponentKind.Progress:
            ProcessProgressComponent(quest);
            break;
        case QuestComponentKind.Reward:
            ProcessRewardComponent(quest);
            break;
    }
}

private void ProcessSupplyComponent(Quest quest)
{
    var template = quest.Template;
    var character = quest.Owner as Character;
    
    // Dá itens iniciais da quest
    foreach (var supplyItem in template.SupplyItems)
    {
        var item = ItemManager.Instance.Create(supplyItem.ItemId, supplyItem.Count);
        if (item != null)
        {
            character.Inventory.AddItem(item);
            Logger.Info($"Quest {quest.TemplateId}: Item {supplyItem.ItemId} x{supplyItem.Count} dado para {character.Name}");
        }
    }
    
    // Avança para próxima etapa
    quest.Step = QuestComponentKind.Progress;
    
    // Agenda verificação de progresso
    EnqueueEvaluation(quest);
}
```

**👶 Explicação:** É como quando você se inscreve numa aula de culinária e recebem:
- **Aventais** (quest items)
- **Receitas** (instruções)
- **Ingredientes básicos** (supply items)

#### **6. Sistema de Progress (Fazendo as Tarefas)**

```csharp
private void ProcessProgressComponent(Quest quest)
{
    var template = quest.Template;
    var objectives = template.ProgressObjectives;
    
    // Reseta resultados
    quest.ProgressStepResults.Clear();
    
    for (int i = 0; i < objectives.Count; i++)
    {
        var objective = objectives[i];
        bool completed = false;
        
        switch (objective.Type)
        {
            case QuestObjectiveType.KillMobs:
                completed = quest.Objectives[i] >= objective.Count;
                break;
                
            case QuestObjectiveType.CollectItems:
                completed = CheckItemsInInventory(quest.Owner, objective.ItemId, objective.Count);
                break;
                
            case QuestObjectiveType.TalkToNpc:
                completed = quest.Objectives[i] >= 1;
                break;
                
            case QuestObjectiveType.ReachLocation:
                completed = CheckPlayerInArea(quest.Owner, objective.AreaId);
                break;
        }
        
        quest.ProgressStepResults.Add(completed);
    }
    
    // Verifica se TODOS os objetivos foram completados
    bool allCompleted = quest.ProgressStepResults.All(x => x);
    
    if (allCompleted)
    {
        quest.Step = QuestComponentKind.Ready;
        quest.ReadyToReportNpc = true;
        
        // Informa jogador que pode entregar
        SendQuestReadyToComplete(quest.Owner as Character, quest);
        
        Logger.Info($"Quest {quest.TemplateId} pronta para entrega por {quest.Owner.Name}");
    }
}
```

**👶 Explicação:** É como o professor verificando sua lição de casa:
- ✅ **Matou 5 lobos** = Tarefa 1 completa
- ❌ **Coletou 2/3 maçãs** = Tarefa 2 incompleta
- ✅ **Falou com NPC** = Tarefa 3 completa

Só quando TODAS estão ✅ você pode entregar!

#### **7. Entregando a Quest**

```csharp
public class CSCompleteQuestContextPacket : GamePacket
{
    public uint QuestContextId { get; set; }
    public uint NpcObjId { get; set; }
    public uint Selected { get; set; }  // Recompensa escolhida (se tem opções)
    
    public override void Execute()
    {
        var character = Connection.ActiveCharacter;
        var npc = WorldManager.Instance.GetNpc(NpcObjId);
        
        QuestManager.Instance.CompleteQuest(character, QuestContextId, npc, Selected);
    }
}

public void CompleteQuest(Character character, uint questId, Npc questReceiver, uint selectedReward)
{
    var quest = character.Quests.GetQuest(questId);
    if (quest == null || quest.Step != QuestComponentKind.Ready)
    {
        SendQuestError(character, QuestError.NotReady);
        return;
    }
    
    var template = quest.Template;
    
    // Consome itens necessários
    foreach (var requiredItem in template.CompletionItems)
    {
        if (!character.Inventory.RemoveItem(requiredItem.ItemId, requiredItem.Count))
        {
            SendQuestError(character, QuestError.MissingItems);
            return;
        }
    }
    
    // Dá recompensas
    GiveQuestRewards(character, template, selectedReward);
    
    // Marca como completada
    quest.Status = QuestStatus.Completed;
    quest.Step = QuestComponentKind.Reward;
    
    // Remove da lista ativa e adiciona ao histórico
    character.Quests.CompleteQuest(quest);
    
    // Salva no banco
    SaveQuest(quest);
    
    // Informa jogador
    SendQuestCompleted(character, quest);
    
    // Verifica se desbloqueia novas quests
    CheckUnlockedQuests(character);
    
    Logger.Info($"Quest '{template.Name}' completada por {character.Name}");
}

private void GiveQuestRewards(Character character, QuestTemplate template, uint selectedReward)
{
    // Experiência
    if (template.ExpReward > 0)
    {
        character.AddExperience(template.ExpReward);
    }
    
    // Dinheiro
    if (template.MoneyReward > 0)
    {
        character.Money += template.MoneyReward;
    }
    
    // Itens fixos
    foreach (var rewardItem in template.RewardItems)
    {
        var item = ItemManager.Instance.Create(rewardItem.ItemId, rewardItem.Count);
        character.Inventory.AddItem(item);
    }
    
    // Item opcional (se jogador escolheu)
    if (selectedReward > 0 && template.OptionalRewardItems.Count > selectedReward)
    {
        var chosenReward = template.OptionalRewardItems[(int)selectedReward];
        var item = ItemManager.Instance.Create(chosenReward.ItemId, chosenReward.Count);
        character.Inventory.AddItem(item);
    }
}
```

**👶 Explicação:** É como entregar um trabalho escolar:
1. **Professor verifica** se você fez tudo
2. **Você entrega materiais** que usou no trabalho
3. **Professor te dá nota** (experiência)
4. **Você ganha estrelinhas** (recompensas)
5. **Seu nome vai no quadro de honra** (histórico)
6. **Libera próximas matérias** (novas quests)

---

## 🧠 **CAPÍTULO 4: SISTEMA DE AI DOS NPCS**

Agora vamos entender como os NPCs "pensam"! É a parte mais fascinante! 🤖

### **Tipos de AI Implementados**

```csharp
public enum AiParamType
{
    None = 0,
    Dummy = 1,              // NPC que não faz nada
    Combatant = 2,          // NPC que luta
    Peaceful = 3,           // NPC pacífico
    Merchant = 4,           // Vendedor
    Guard = 5,              // Guarda
    WildAnimal = 6,         // Animal selvagem
    Pet = 7,                // Pet domesticado
    Mount = 8,              // Montaria
    Aggressive = 9,         // Sempre ataca
    Defensive = 10,         // Só ataca se atacado
    Patrol = 11,            // Patrulha uma rota
    Scripted = 12           // Comportamento scriptado
}
```

#### **Exemplo: AI Combatant (NPC que Luta)**

```csharp
public class CombatantAi : AiAvatarBase
{
    private BaseUnit _currentTarget;
    private DateTime _lastAttackTime;
    private DateTime _lastMoveTime;
    private bool _isRetreating;
    private Vector3 _spawnPosition;
    
    public override void Initialize()
    {
        base.Initialize();
        _spawnPosition = Owner.Position;
        _currentTarget = null;
        _isRetreating = false;
        
        // Configura comportamento
        AggroRange = Owner.Template.AggroRange;
        LeashRange = Owner.Template.LeashRange;
        AttackSpeed = Owner.Template.AttackSpeed;
        
        Logger.Debug($"CombatantAi inicializada para {Owner.Name}");
    }
    
    public override void Update(TimeSpan delta)
    {
        if (Owner.Hp <= 0)
        {
            HandleDeath();
            return;
        }
        
        // Máquina de estados
        switch (CurrentState)
        {
            case AiState.Idle:
                HandleIdleState();
                break;
                
            case AiState.Searching:
                HandleSearchingState();
                break;
                
            case AiState.Combat:
                HandleCombatState();
                break;
                
            case AiState.Retreating:
                HandleRetreatingState();
                break;
                
            case AiState.Returning:
                HandleReturningState();
                break;
        }
        
        base.Update(delta);
    }
    
    private void HandleIdleState()
    {
        // Procura inimigos próximos
        var nearbyEnemies = GetNearbyEnemies(AggroRange);
        
        if (nearbyEnemies.Count > 0)
        {
            // Escolhe alvo mais próximo
            _currentTarget = nearbyEnemies
                .OrderBy(e => GetDistance(Owner.Position, e.Position))
                .First();
                
            Logger.Debug($"{Owner.Name} detectou inimigo: {_currentTarget.Name}");
            ChangeState(AiState.Combat);
            return;
        }
        
        // Se não há inimigos, pode patrulhar ou ficar parado
        if (Owner.Template.HasPatrolRoute)
        {
            ContinuePatrol();
        }
    }
    
    private void HandleCombatState()
    {
        if (_currentTarget == null || _currentTarget.Hp <= 0)
        {
            _currentTarget = null;
            ChangeState(AiState.Idle);
            return;
        }
        
        var distanceToTarget = GetDistance(Owner.Position, _currentTarget.Position);
        var distanceToSpawn = GetDistance(Owner.Position, _spawnPosition);
        
        // Verifica se target saiu do leash range
        if (distanceToSpawn > LeashRange)
        {
            Logger.Debug($"{Owner.Name} perdeu target (leash range)");
            _currentTarget = null;
            ChangeState(AiState.Returning);
            return;
        }
        
        // Se está muito longe do target, move-se para perto
        if (distanceToTarget > Owner.Template.AttackRange)
        {
            MoveTowards(_currentTarget.Position);
        }
        else
        {
            // Está perto o suficiente, ataca!
            if (CanAttack())
            {
                PerformAttack(_currentTarget);
            }
        }
    }
    
    private void HandleRetreatingState()
    {
        // Move em direção ao spawn point
        if (GetDistance(Owner.Position, _spawnPosition) > 2.0f)
        {
            MoveTowards(_spawnPosition);
        }
        else
        {
            // Chegou no spawn, volta ao estado idle
            ChangeState(AiState.Idle);
            
            // Regenera vida se necessário
            if (Owner.Hp < Owner.MaxHp)
            {
                Owner.Hp = Owner.MaxHp;
                Logger.Debug($"{Owner.Name} regenerou vida completa");
            }
        }
    }
    
    private bool CanAttack()
    {
        var timeSinceLastAttack = DateTime.UtcNow - _lastAttackTime;
        return timeSinceLastAttack.TotalMilliseconds >= (1000.0 / AttackSpeed);
    }
    
    private void PerformAttack(BaseUnit target)
    {
        // Usa skill de ataque padrão ou ataque básico
        var attackSkill = Owner.Template.DefaultAttackSkill;
        
        if (attackSkill != null)
        {
            Owner.UseSkill(attackSkill.Id, target);
        }
        else
        {
            // Ataque básico
            var damage = CalculateBasicAttackDamage(target);
            target.ReduceHp(Owner, damage);
            
            // Broadcast do ataque
            var attackPacket = new SCCombatEngagedPacket
            {
                AttackerId = Owner.ObjId,
                TargetId = target.ObjId,
                Damage = damage
            };
            Owner.BroadcastPacket(attackPacket, true);
        }
        
        _lastAttackTime = DateTime.UtcNow;
        Logger.Debug($"{Owner.Name} atacou {target.Name}");
    }
    
    private List<BaseUnit> GetNearbyEnemies(float range)
    {
        var enemies = new List<BaseUnit>();
        var nearbyUnits = WorldManager.Instance.GetUnitsInRange(Owner.Position, range);
        
        foreach (var unit in nearbyUnits)
        {
            if (IsEnemy(unit))
            {
                enemies.Add(unit);
            }
        }
        
        return enemies;
    }
    
    private bool IsEnemy(BaseUnit unit)
    {
        // Verifica se é inimigo baseado na facção
        if (unit is Character character)
        {
            return FactionManager.Instance.IsHostile(Owner.Faction, character.Faction);
        }
        
        return false;
    }
}
```

**👶 Explicação da AI:**

**É como um guarda robô que segue estas regras:**

1. **Estado Idle (Parado)**: "Olhe ao redor procurando problemas"
2. **Estado Combat (Lutando)**: "Se viu inimigo, vá bater nele!"
3. **Estado Retreating (Fugindo)**: "Se ficou muito ferido, volte para casa"
4. **Estado Returning (Voltando)**: "Se inimigo fugiu muito longe, volte ao posto"

**Regras importantes:**
- **AggroRange**: "Até que distância posso ver inimigos"
- **LeashRange**: "Até que distância posso perseguir" 
- **AttackSpeed**: "Quantas vezes por segundo posso atacar"

---

## 📜 **CAPÍTULO 5: CRIANDO UMA QUEST DO ZERO**

Agora vou te ensinar a criar uma quest completa! Vamos fazer uma quest chamada **"A Invasão dos Goblins"**! 

### **Passo 1: Planejando a Quest**

**📋 Documento de Design:**

```
QUEST: A Invasão dos Goblins
LEVEL: 5-10
NPC INÍCIO: Capitão Marcus (Guarda da Vila)
NPC FINAL: Capitão Marcus

HISTÓRIA:
"Goblins estão atacando nossa vila! Precisamos que você nos ajude a defendê-la!"

OBJETIVOS:
1. Mate 8 Goblins Verdes
2. Colete 5 Orelhas de Goblin
3. Volte para Capitão Marcus

RECOMPENSAS:
- 250 XP
- 50 Moedas de Ouro
- Espada de Bronze (opcional)
- Escudo de Madeira (opcional)
```

### **Passo 2: Criando no Banco SQLite**

```sql
-- Primeiro, inserimos a quest template
INSERT INTO quest_contexts (
    id,
    name,
    description,
    level_min,
    level_max,
    race_required,
    repeatable,
    start_npc_id,
    complete_npc_id,
    exp_reward,
    money_reward,
    time_limit
) VALUES (
    1001,  -- ID único da quest
    'A Invasão dos Goblins',
    'Goblins estão atacando a vila. Mate 8 goblins e colete suas orelhas como prova.',
    5,     -- Nível mínimo
    10,    -- Nível máximo  
    0,     -- 0 = todas as raças
    1,     -- 1 = repetível
    2001,  -- ID do Capitão Marcus
    2001,  -- Mesmo NPC para entregar
    250,   -- 250 XP
    50,    -- 50 moedas
    0      -- 0 = sem limite de tempo
);

-- Criamos os componentes da quest
INSERT INTO quest_components (
    id,
    quest_context_id,
    component_kind,
    next_component,
    npc_ai_id,
    skill_id,
    skill_self
) VALUES 
-- Componente START (início)
(10001, 1001, 1, 10002, 2001, 0, 0),
-- Componente SUPPLY (dar itens iniciais) 
(10002, 1001, 2, 10003, 0, 0, 0),
-- Componente PROGRESS (fazer tarefas)
(10003, 1001, 3, 10004, 0, 0, 0),
-- Componente REWARD (dar recompensas)
(10004, 1001, 5, 0, 2001, 0, 0);

-- Criamos os atos (ações) de cada componente
INSERT INTO quest_acts (
    id,
    quest_component_id,
    act_detail_type,
    act_detail_id
) VALUES
-- Supply: dar uma poção de vida
(100001, 10002, 'QuestActSupplyItem', 100001),
-- Progress: matar goblins
(100002, 10003, 'QuestActObjKillNpc', 100002),
-- Progress: coletar orelhas
(100003, 10003, 'QuestActObjCollectItem', 100003),
-- Reward: dar experiência
(100004, 10004, 'QuestActRewardExp', 100004),
-- Reward: dar dinheiro
(100005, 10004, 'QuestActRewardMoney', 100005),
-- Reward: escolher arma
(100006, 10004, 'QuestActRewardSelectiveItem', 100006);

-- Detalhes dos atos
-- Supply: Poção de Vida x3
INSERT INTO quest_act_supply_items (id, item_id, count) 
VALUES (100001, 500, 3);  -- 500 = ID da Poção de Vida

-- Progress: Matar 8 Goblins Verdes
INSERT INTO quest_act_obj_kill_npcs (id, npc_id, count, use_alias)
VALUES (100002, 1500, 8, 0);  -- 1500 = ID do Goblin Verde

-- Progress: Coletar 5 Orelhas de Goblin
INSERT INTO quest_act_obj_collect_items (id, item_id, count, cleanup, drop_when_destroy)
VALUES (100003, 501, 5, 1, 1);  -- 501 = ID da Orelha de Goblin

-- Reward: 250 XP
INSERT INTO quest_act_reward_exps (id, exp)
VALUES (100004, 250);

-- Reward: 50 Moedas
INSERT INTO quest_act_reward_moneys (id, amount)
VALUES (100005, 50);

-- Reward: Escolha entre Espada ou Escudo
INSERT INTO quest_act_reward_selective_items (id, item_id, count)
VALUES 
(100006, 1001, 1),  -- Espada de Bronze
(100006, 1002, 1);  -- Escudo de Madeira
```

### **Passo 3: Criando a Lógica no Código**

```csharp
// Classe específica para nossa quest customizada
public class GoblinInvasionQuest : QuestActTemplate
{
    public override bool Use(BaseUnit caster, BaseUnit target)
    {
        var character = caster as Character;
        if (character == null) return false;
        
        var quest = character.Quests.GetQuest(1001); // ID da nossa quest
        if (quest == null) return false;
        
        // Lógica especial se necessário
        Logger.Info($"Processando quest especial: Invasão dos Goblins para {character.Name}");
        
        return true;
    }
}

// Handler para quando goblin morre
public class GoblinKillHandler
{
    public static void OnGoblinKilled(Character killer, Npc goblin)
    {
        // Verifica se jogador tem quest ativa
        var quest = killer.Quests.GetQuest(1001);
        if (quest == null || quest.Status != QuestStatus.Active) 
            return;
            
        // Verifica se é o tipo certo de goblin
        if (goblin.TemplateId != 1500) // Goblin Verde
            return;
            
        // Incrementa contador
        quest.Objectives[0]++; // Primeiro objetivo = matar goblins
        
        // Chance de dropar orelha (75%)
        if (Random.Shared.Next(100) < 75)
        {
            var ear = ItemManager.Instance.Create(501, 1); // Orelha de Goblin
            killer.Inventory.AddItem(ear);
            
            killer.SendMessage("Você obteve uma Orelha de Goblin!");
        }
        
        // Verifica progresso
        var killProgress = quest.Objectives[0];
        var earCount = killer.Inventory.GetItemCount(501);
        
        // Informa progresso
        killer.SendMessage($"Goblins mortos: {killProgress}/8");
        killer.SendMessage($"Orelhas coletadas: {earCount}/5");
        
        // Se completou tudo, marca como pronta
        if (killProgress >= 8 && earCount >= 5)
        {
            quest.Step = QuestComponentKind.Ready;
            quest.ReadyToReportNpc = true;
            
            killer.SendMessage("Quest completada! Volte para o Capitão Marcus!");
            
            // Efeito visual de quest completa
            var questCompletePacket = new SCQuestCompletePacket
            {
                QuestId = quest.TemplateId
            };
            killer.SendPacket(questCompletePacket);
        }
        
        // Agenda reavaliação da quest
        QuestManager.Instance.EnqueueEvaluation(quest);
    }
}

// Sistema de diálogo do NPC
public class CapitaoMarcusDialogue
{
    public static void HandleDialogue(Character player, Npc npc)
    {
        var quest = player.Quests.GetQuest(1001);
        
        if (quest == null)
        {
            // Jogador não tem a quest - oferece para começar
            ShowQuestOfferDialogue(player, npc);
        }
        else if (quest.Status == QuestStatus.Active)
        {
            if (quest.ReadyToReportNpc)
            {
                // Quest pronta para entregar
                ShowQuestCompleteDialogue(player, npc);
            }
            else
            {
                // Quest em progresso
                ShowQuestProgressDialogue(player, npc, quest);
            }
        }
        else
        {
            // Quest já completada
            ShowNormalDialogue(player, npc);
        }
    }
    
    private static void ShowQuestOfferDialogue(Character player, Npc npc)
    {
        var dialogue = new NpcDialogue();
        dialogue.AddMessage("Olá, guerreiro! Nossa vila está sendo atacada por goblins!");
        dialogue.AddMessage("Você poderia nos ajudar a defendê-la?");
        dialogue.AddOption("Aceitar Quest", () => StartGoblinQuest(player, npc));
        dialogue.AddOption("Agora não", () => { /* fecha diálogo */ });
        
        player.SendDialogue(dialogue);
    }
    
    private static void ShowQuestCompleteDialogue(Character player, Npc npc)
    {
        var quest = player.Quests.GetQuest(1001);
        var killCount = quest.Objectives[0];
        var earCount = player.Inventory.GetItemCount(501);
        
        var dialogue = new NpcDialogue();
        dialogue.AddMessage($"Excelente trabalho! Você matou {killCount} goblins!");
        dialogue.AddMessage($"E trouxe {earCount} orelhas como prova!");
        dialogue.AddMessage("Escolha sua recompensa:");
        dialogue.AddOption("Espada de Bronze", () => CompleteQuestWithReward(player, 0));
        dialogue.AddOption("Escudo de Madeira", () => CompleteQuestWithReward(player, 1));
        
        player.SendDialogue(dialogue);
    }
    
    private static void CompleteQuestWithReward(Character player, int rewardChoice)
    {
        QuestManager.Instance.CompleteQuest(player, 1001, null, (uint)rewardChoice);
        
        player.SendMessage("Obrigado por salvar nossa vila!");
        player.SendMessage("Os goblins foram derrotados graças a você!");
    }
}
```

### **Passo 4: Testando a Quest**

```csharp
// Comando GM para testar a quest
[Command("testquest")]
public void TestGoblinQuest(Character character)
{
    Logger.Info($"Testando quest Invasão dos Goblins para {character.Name}");
    
    // Força início da quest
    QuestManager.Instance.StartQuest(character, 1001, null);
    
    // Spawna alguns goblins para testar
    for (int i = 0; i < 5; i++)
    {
        var goblin = NpcManager.Instance.Create(1500); // Goblin Verde
        goblin.Position = character.Position.AddDistanceToFront(5 + i * 2);
        
        character.CurrentInstance.AddObject(goblin);
    }
    
    character.SendMessage("Quest iniciada! Goblins spawnados à sua frente!");
}

// Comando para simular kill
[Command("killgoblin")]
public void SimulateGoblinKill(Character character)
{
    GoblinKillHandler.OnGoblinKilled(character, null);
    character.SendMessage("Simulou morte de 1 goblin!");
}

// Comando para dar orelhas
[Command("giveears")]
public void GiveGoblinEars(Character character, int count = 1)
{
    var ear = ItemManager.Instance.Create(501, count);
    character.Inventory.AddItem(ear);
    character.SendMessage($"Recebeu {count} Orelhas de Goblin!");
}
```

**👶 Explicação do teste:**
1. **testquest** = Inicia a quest automaticamente
2. **killgoblin** = Simula que você matou um goblin  
3. **giveears** = Te dá orelhas para testar

---

## 🎉 **PARABÉNS! VOCÊ CONCLUIU O MÓDULO 13!**

Você agora domina completamente:

✅ **Anatomia de uma Quest** - Cada propriedade e estado  
✅ **Sistema de NPCs** - Como são criados e gerenciados  
✅ **Interação Quest ↔ NPC** - Fluxo completo  
✅ **Sistema de AI** - Como NPCs "pensam" e agem  
✅ **Criação de Quest** - Do planejamento ao código  

### 📚 **RESUMO DOS CONCEITOS PRINCIPAIS**

1. **Quest States**: None → Active → Ready → Completed
2. **Quest Components**: Start → Supply → Progress → Reward
3. **NPC AI Types**: Combatant, Merchant, Guard, Peaceful, etc.
4. **Database Structure**: SQLite (templates) + MySQL (instances)
5. **Event System**: Quest evaluation queue e handlers

### 🔥 **SISTEMAS COMPLEXOS DOMINADOS**

- **Quest Template System** com componentes modulares
- **NPC AI State Machine** com comportamentos inteligentes  
- **Objective Tracking** em tempo real
- **Reward Distribution** com opções múltiplas
- **Prerequisites** e unlock chains

### 🚀 **PRÓXIMO MÓDULO: ECONOMIA E TRADE**

No MÓDULO 14, vamos mergulhar em:
- Sistema de mercadores e lojas
- Auction House (leilões)
- Sistema de trade entre jogadores
- Economia dinâmica e preços
- Sistema de crafting avançado

**Continue comigo nesta jornada épica!** 🎓✨

Quer continuar imediatamente com o MÓDULO 14? 🤔