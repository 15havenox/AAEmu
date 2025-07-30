# 📜 **MÓDULO 12: SISTEMA DE QUESTS (MISSÕES) - MEGA DETALHADO**

## 🎯 **Introdução Completa ao Sistema de Quests**

Imagine que você está jogando RPG e um NPC te dá uma missão: "Vá até a floresta, mate 10 lobos e traga suas peles". Isso é uma **Quest**! 

No ArcheAge, as quests são o coração da experiência do jogador. Elas:
- **Guiam o jogador** através do mundo
- **Contam a história** do jogo
- **Recompensam** com experiência, itens e dinheiro
- **Ensinam** mecânicas do jogo

---

## 🔧 **Anatomia de uma Quest - Explicação Detalhada**

Vamos destrinchar uma quest como se fosse um quebra-cabeça:

```csharp
public class Quest
{
    // Identificação única da quest
    public uint Id { get; set; }
    
    // Nome que aparece na interface
    public string Title { get; set; }
    
    // História/contexto da missão
    public string Description { get; set; }
    
    // Texto quando aceita a quest
    public string AcceptText { get; set; }
    
    // Texto quando completa a quest
    public string CompleteText { get; set; }
    
    // Lista de tarefas a serem cumpridas
    public List<QuestObjective> Objectives { get; set; }
    
    // Prêmios por completar
    public List<QuestReward> Rewards { get; set; }
    
    // Quests que devem ser feitas antes desta
    public List<uint> PrerequisiteQuests { get; set; }
    
    // Level mínimo para pegar a quest
    public byte MinLevel { get; set; }
    
    // Level máximo (para não pegar quest muito fácil)
    public byte MaxLevel { get; set; }
    
    // Qual classe pode fazer esta quest
    public ClassType AllowedClass { get; set; }
    
    // Se a quest é repetível diariamente
    public bool IsDaily { get; set; }
    
    // Se pode ser feita em grupo
    public bool AllowsGroup { get; set; }
    
    // Verifica se todas as tarefas foram cumpridas
    public bool IsCompleted()
    {
        return Objectives.All(obj => obj.IsCompleted);
    }
    
    // Verifica se jogador pode pegar esta quest
    public bool CanAccept(Character character)
    {
        // Checa level
        if (character.Level < MinLevel || character.Level > MaxLevel)
            return false;
            
        // Checa se já fez quests necessárias
        foreach (var prereq in PrerequisiteQuests)
        {
            if (!character.CompletedQuests.Contains(prereq))
                return false;
        }
        
        // Checa se já tem esta quest
        if (character.ActiveQuests.Any(q => q.Id == Id))
            return false;
            
        // Checa se já completou (não repetível)
        if (!IsDaily && character.CompletedQuests.Contains(Id))
            return false;
            
        return true;
    }
}
```

---

## 🎯 **Sistema de Objetivos - Como Funcionam as Tarefas**

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