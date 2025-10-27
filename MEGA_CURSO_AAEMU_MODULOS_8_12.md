# 🎓 MEGA CURSO ULTRA DETALHADO: MÓDULOS 8-12
## 🌐 SISTEMAS AVANÇADOS DO AAEMU EXPLICADOS LINHA POR LINHA

---

## 🌐 **MÓDULO 8: SISTEMA DE NETWORKING ULTRA DETALHADO**

Agora vamos DISSECAR o sistema de rede mais complexo do AAEmu! É como entender como funciona o sistema nervoso de um corpo humano! 🧠

### **Arquivo: AAEmu.Game/Core/Network/Game/GameNetwork.cs (O Sistema Nervoso Central)**

Vamos analisar CADA linha deste arquivo gigante:

```csharp
using System.Net;

using AAEmu.Commons.Network.Core;
using AAEmu.Commons.Utils;
using AAEmu.Game.Core.Packets.C2G;
using AAEmu.Game.Core.Packets.Proxy;
using AAEmu.Game.Models;

using NLog;
```

**👶 Explicação dos "using":**
- **System.Net** = Ferramentas de rede do sistema
- **AAEmu.Commons.Network.Core** = Nossa base de rede personalizada
- **AAEmu.Game.Core.Packets.C2G** = Pacotes que vêm do Cliente para o Game
- **AAEmu.Game.Core.Packets.Proxy** = Pacotes especiais de proxy
- **NLog** = Nosso sistema de diário

#### **Declaração da Classe (O Cérebro da Rede)**

```csharp
public class GameNetwork : Singleton<GameNetwork>
{
    private Server _server;
    private GameProtocolHandler _handler;
    private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
```

**🤔 Vamos explicar cada componente:**

**"Singleton<GameNetwork>"**
- **Singleton** = Padrão que garante só existir UMA instância desta classe
- É como ter um único "central telefônica" para todo o jogo

**👶 Explicação:** Imagine uma cidade com várias escolas. Cada escola tem um telefone, mas existe apenas UMA central telefônica que conecta todas. O GameNetwork é essa central única!

**Variáveis privadas:**

1. **Server _server** = O servidor TCP que aceita conexões
2. **GameProtocolHandler _handler** = O "tradutor" que entende pacotes do jogo
3. **Logger** = O diário que anota tudo

#### **Construtor (Nascimento do Sistema de Rede)**

```csharp
private GameNetwork()
{
    _handler = new GameProtocolHandler();

    // World
    RegisterPacket(CSOffsets.X2EnterWorldPacket, 1, typeof(X2EnterWorldPacket));
    RegisterPacket(CSOffsets.CSLeaveWorldPacket, 1, typeof(CSLeaveWorldPacket));
    RegisterPacket(CSOffsets.CSCancelLeaveWorldPacket, 1, typeof(CSCancelLeaveWorldPacket));
    // ... centenas de outros pacotes
}
```

**🤔 O que está acontecendo aqui?**

O construtor está "ensinando" o sistema de rede a reconhecer TODOS os tipos de mensagem que podem chegar do cliente!

**👶 Explicação com analogia:**
Imagine que você trabalha numa empresa que recebe cartas de todo o mundo. Antes de abrir a empresa, você precisa ensinar seus funcionários:

- "Se chegar uma carta com selo azul, é pedido de produto"
- "Se chegar uma carta com selo vermelho, é reclamação"  
- "Se chegar uma carta com selo verde, é elogio"

O `RegisterPacket` está fazendo exatamente isso - ensinando o servidor a reconhecer cada tipo de "carta" (pacote) que pode chegar!

#### **Vamos Analisar Alguns Pacotes Específicos:**

```csharp
RegisterPacket(CSOffsets.X2EnterWorldPacket, 1, typeof(X2EnterWorldPacket));
```

**🔍 Dissecando cada parte:**
- **CSOffsets.X2EnterWorldPacket** = Número identificador deste tipo de pacote
- **1** = Nível do protocolo (versão)
- **typeof(X2EnterWorldPacket)** = Classe que vai processar este pacote

**👶 Explicação:** É como dizer "Se chegar carta tipo 0x001, nível 1, chame o funcionário X2EnterWorldPacket para processar"

#### **Exemplo de Pacotes do Mundo Virtual:**

```csharp
// Pacotes de Expedição (Guilds)
RegisterPacket(CSOffsets.CSCreateExpeditionPacket, 1, typeof(CSCreateExpeditionPacket));
RegisterPacket(CSOffsets.CSInviteToExpeditionPacket, 1, typeof(CSInviteToExpeditionPacket));
RegisterPacket(CSOffsets.CSLeaveExpeditionPacket, 1, typeof(CSLeaveExpeditionPacket));
```

**👶 Explicação:** Estes são pacotes para:
1. **Criar uma guild** ("Quero criar um clã chamado 'Os Corajosos'")
2. **Convidar para guild** ("Quero convidar João para meu clã")
3. **Sair da guild** ("Quero sair do clã")

#### **Pacotes de Personagem:**

```csharp
RegisterPacket(CSOffsets.CSListCharacterPacket, 1, typeof(CSListCharacterPacket));
RegisterPacket(CSOffsets.CSCreateCharacterPacket, 1, typeof(CSCreateCharacterPacket));
RegisterPacket(CSOffsets.CSDeleteCharacterPacket, 1, typeof(CSDeleteCharacterPacket));
RegisterPacket(CSOffsets.CSSelectCharacterPacket, 1, typeof(CSSelectCharacterPacket));
```

**👶 Explicação:** Estes pacotes controlam:
1. **Mostrar lista de personagens** ("Quais personagens eu tenho?")
2. **Criar novo personagem** ("Quero criar um Elfo Arqueiro")
3. **Deletar personagem** ("Quero apagar meu personagem")
4. **Escolher personagem** ("Quero jogar com meu Elfo")

#### **Pacotes de Movimento:**

```csharp
RegisterPacket(CSOffsets.CSMoveUnitPacket, 1, typeof(CSMoveUnitPacket));
```

**👶 Explicação:** Este é um dos pacotes mais importantes! Toda vez que você anda no jogo, este pacote é enviado com sua nova posição.

#### **Pacotes de Combate:**

```csharp
RegisterPacket(CSOffsets.CSStartSkillPacket, 1, typeof(CSStartSkillPacket));
RegisterPacket(CSOffsets.CSStopCastingPacket, 1, typeof(CSStopCastingPacket));
RegisterPacket(CSOffsets.CSRemoveBuffPacket, 1, typeof(CSRemoveBuffPacket));
```

**👶 Explicação:**
1. **Usar skill** ("Quero lançar Bola de Fogo no monstro")
2. **Cancelar skill** ("Mudei de ideia, para de lançar")
3. **Remover buff** ("Quero cancelar este efeito em mim")

#### **Função Start (Ligando o Sistema)**

```csharp
public void Start()
{
    var config = AppConfiguration.Instance.Network;
    _server = new Server(config.Host.Equals("*") ? IPAddress.Any : IPAddress.Parse(config.Host), config.Port, _handler);
    _server.Start();

    Logger.Info("Network started");
}
```

**🔍 Linha por linha:**

**Linha 3:**
```csharp
var config = AppConfiguration.Instance.Network;
```
**Tradução:** "Pega as configurações de rede do arquivo de configuração"

**Linha 4:**
```csharp
_server = new Server(config.Host.Equals("*") ? IPAddress.Any : IPAddress.Parse(config.Host), config.Port, _handler);
```

**🤔 Vamos quebrar esta linha complexa:**

1. **config.Host.Equals("*")** = Se o host configurado for "*" (qualquer IP)
2. **IPAddress.Any** = Aceita conexões de qualquer IP
3. **IPAddress.Parse(config.Host)** = Senão, usa o IP específico configurado
4. **config.Port** = Porta configurada (normalmente 1239)
5. **_handler** = O tradutor de pacotes que criamos

**👶 Explicação:** É como abrir uma loja e decidir:
- Se config = "*": "Aceito clientes de qualquer lugar"
- Se config = "127.0.0.1": "Só aceito clientes locais"

**Linha 5:**
```csharp
_server.Start();
```
**Tradução:** "Abra as portas da loja!"

**Linha 7:**
```csharp
Logger.Info("Network started");
```
**Tradução:** "Anote no diário: 'Rede iniciada com sucesso'"

---

### **Arquivo: AAEmu.Game/Core/Network/Game/GamePacket.cs (O DNA dos Pacotes)**

Agora vamos entender como funciona um pacote individual:

```csharp
public abstract class GamePacket : PacketBase<GameConnection>
{
    public byte Level { get; set; }

    protected GamePacket(ushort typeId, byte level) : base(typeId)
    {
        Level = level;
    }
```

**🤔 Explicando a herança:**
- **GamePacket** herda de **PacketBase<GameConnection>**
- É como dizer "Todo GamePacket é um tipo especial de PacketBase"

**👶 Explicação:** É como dizer "Todo carro é um tipo especial de veículo". GamePacket é o "carro", PacketBase é o "veículo".

#### **Função Execute (Executando a Ação)**

```csharp
/// <summary>
/// This is called in Encode after Read() in the case of GamePackets
/// The purpose is to separate packet data from packet behavior
/// </summary>
public virtual void Execute() { }
```

**🤔 Por que função vazia?**
Esta é uma função "virtual" - cada tipo específico de pacote vai implementar sua própria versão.

**👶 Explicação:** É como ter uma regra "todo funcionário deve trabalhar", mas cada funcionário trabalha de forma diferente:
- Secretária: digita documentos
- Limpeza: limpa salas  
- Segurança: vigia a entrada

#### **Função Encode (Preparando para Envio)**

```csharp
public override PacketStream Encode()
{
    var ps = new PacketStream();
    try
    {
        var packet = new PacketStream()
            .Write((byte)0xdd)
            .Write(Level);

        var body = new PacketStream()
            .Write(TypeId)
            .Write(this);

        if (Level == 1)
        {
            packet
                .Write((byte)0) // hash
                .Write((byte)0); // count
        }

        packet.Write(body, false);
        ps.Write(packet);
    }
    catch (Exception ex)
    {
        Logger.Fatal(ex);
        throw;
    }
    // ... código de log
    return ps;
}
```

**🤔 O que está acontecendo aqui?**

Esta função está "empacotando" os dados para envio, como embalar um presente:

**👶 Explicação passo a passo:**

1. **Cria a caixa principal** (`var ps = new PacketStream()`)
2. **Coloca etiqueta especial** (`Write((byte)0xdd)` - identifica como pacote do jogo)
3. **Anota o nível** (`Write(Level)`)
4. **Empacota o conteúdo** (TypeId + dados do pacote)
5. **Se for nível 1, adiciona selo especial** (hash e count)
6. **Fecha a caixa** (`ps.Write(packet)`)

#### **Função Decode (Abrindo Pacote Recebido)**

```csharp
public override PacketBase<GameConnection> Decode(PacketStream ps)
{
    try
    {
        Read(ps);
        
        var logString = $"GamePacket: C->S type {TypeId:X3} {ToString()?.Substring(23)}{Verbose()}";
        // ... código de log

        Execute();
    }
    catch (Exception ex)
    {
        Logger.Error("GamePacket: C->S type {0:X3} {1}", TypeId, ToString()?.Substring(23));
        Logger.Fatal(ex);
        throw;
    }

    return this;
}
```

**👶 Explicação:** É como receber uma carta:
1. **Abre o envelope** (`Read(ps)`)
2. **Anota no diário que chegou uma carta** (log)
3. **Executa o que a carta pede** (`Execute()`)
4. **Se der erro, anota no diário** (catch)

---

## 🗃️ **MÓDULO 9: SISTEMA DE BANCO DE DADOS ULTRA DETALHADO**

Agora vamos entender como o AAEmu gerencia dados! É como estudar como funciona a memória de um cérebro gigante! 🧠💾

### **Por que Dois Bancos de Dados? (A Estratégia Genial)**

O AAEmu usa uma estratégia muito inteligente que vou explicar com uma analogia:

#### **🏛️ A Analogia da Biblioteca + Arquivo Pessoal**

Imagine que você está estudando para ser médico:

**📚 BIBLIOTECA PÚBLICA (SQLite - compact.sqlite3)**
- Tem TODOS os livros médicos do mundo
- Você pode LER, mas não pode ESCREVER
- Se você fizer anotações, usa post-its temporários
- Livros: anatomia, doenças, medicamentos, procedimentos

**📋 SEU ARQUIVO PESSOAL (MySQL - aaemu_game/aaemu_login)**  
- Suas anotações pessoais de estudo
- Seus casos clínicos tratados
- Seu progresso nos estudos
- Você pode LER e ESCREVER à vontade

**🎯 Por que essa estratégia é genial:**
1. **Separação clara**: Dados oficiais vs. dados dos jogadores
2. **Eficiência**: SQLite é super rápido para leitura
3. **Flexibilidade**: MySQL é perfeito para dados dinâmicos
4. **Manutenção**: Atualizar dados oficiais não afeta jogadores

### **Estrutura Detalhada dos Bancos**

#### **📀 SQLite Database (compact.sqlite3) - O Manual Oficial**

```sql
-- Estrutura simplificada do que TEM no SQLite:

TABLE items (
    id INTEGER PRIMARY KEY,
    name TEXT NOT NULL,
    description TEXT,
    item_type INTEGER,
    max_stack_size INTEGER,
    required_level INTEGER,
    base_price INTEGER,
    icon_id INTEGER
    -- + dezenas de outras colunas
);

TABLE npcs (
    id INTEGER PRIMARY KEY,
    name TEXT NOT NULL,
    model_id INTEGER,
    faction_id INTEGER,
    ai_file TEXT,
    level INTEGER,
    hp INTEGER,
    mp INTEGER
    -- + muito mais dados
);

TABLE zones (
    id INTEGER PRIMARY KEY,
    name TEXT NOT NULL,
    zone_key TEXT,
    world_id INTEGER,
    climate_id INTEGER,
    map_file TEXT
    -- + dados do mundo
);

TABLE quests (
    id INTEGER PRIMARY KEY,
    name TEXT NOT NULL,
    description TEXT,
    level_min INTEGER,
    level_max INTEGER,
    race_required INTEGER,
    faction_required INTEGER
    -- + dados das missões
);
```

**👶 Explicação:** É como ter enciclopédias médicas com:
- **Livro de Medicamentos**: todos os remédios existentes
- **Livro de Doenças**: todas as doenças conhecidas  
- **Livro de Anatomia**: como funciona o corpo humano
- **Livro de Procedimentos**: como fazer cirurgias

#### **🗄️ MySQL Database (aaemu_game) - Seus Registros Pessoais**

```sql
-- Base de dados dos JOGADORES (simplificada):

TABLE characters (
    id INTEGER PRIMARY KEY AUTO_INCREMENT,
    account_id INTEGER NOT NULL,
    name VARCHAR(18) NOT NULL,
    race TINYINT NOT NULL,
    gender TINYINT NOT NULL,
    level INTEGER DEFAULT 1,
    experience INTEGER DEFAULT 0,
    hp INTEGER DEFAULT 100,
    mp INTEGER DEFAULT 100,
    x FLOAT DEFAULT 0,
    y FLOAT DEFAULT 0, 
    z FLOAT DEFAULT 0,
    money BIGINT DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    -- + muito mais dados pessoais
);

TABLE items (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    character_id INTEGER NOT NULL,
    template_id INTEGER NOT NULL,  -- Referência ao SQLite
    count INTEGER DEFAULT 1,
    slot_type TINYINT NOT NULL,
    slot INTEGER NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (character_id) REFERENCES characters(id),
    INDEX (template_id)  -- Para buscar rápido no SQLite
);

TABLE character_quests (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    character_id INTEGER NOT NULL,
    quest_id INTEGER NOT NULL,  -- Referência ao SQLite
    status TINYINT DEFAULT 0,   -- 0=ativo, 1=completo, 2=abandonado
    progress INTEGER DEFAULT 0,
    started_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    completed_at TIMESTAMP NULL,
    
    FOREIGN KEY (character_id) REFERENCES characters(id)
);
```

**👶 Explicação:** É como seus cadernos de estudo:
- **Caderno de Pacientes**: registros dos pacientes que você atendeu
- **Caderno de Progresso**: em que matéria você está, que prova fez
- **Caderno de Casos**: casos interessantes que você tratou

### **Como os Bancos Trabalham Juntos (A Mágica)**

Vamos ver um exemplo prático de como isso funciona:

#### **Exemplo 1: Dando um Item para o Jogador**

```csharp
public class ItemManager
{
    public void GiveItemToPlayer(uint playerId, uint itemTemplateId, int quantity)
    {
        // PASSO 1: Busca informações do item no SQLite (dados originais)
        var itemTemplate = GetItemTemplateFromSQLite(itemTemplateId);
        if (itemTemplate == null)
        {
            Logger.Error($"Item template {itemTemplateId} não encontrado no SQLite!");
            return;
        }
        
        // PASSO 2: Verifica se jogador pode carregar este item
        if (itemTemplate.RequiredLevel > player.Level)
        {
            Logger.Info($"Jogador nível {player.Level} não pode usar item nível {itemTemplate.RequiredLevel}");
            return;
        }
        
        // PASSO 3: Cria instância PERSONALIZADA no MySQL
        var newItem = new Item()
        {
            CharacterId = playerId,
            TemplateId = itemTemplateId,  // Liga com SQLite
            Count = quantity,
            SlotType = ItemSlotType.Inventory,
            Slot = FindEmptySlot(playerId),
            CreatedAt = DateTime.Now,
            
            // Propriedades que podem mudar (não estão no template):
            Durability = itemTemplate.MaxDurability,  // Começa 100%
            EnchantLevel = 0,  // Não está encantado
            SocketedGems = null,  // Sem gemas
            CustomName = null  // Sem nome personalizado
        };
        
        // PASSO 4: Salva no MySQL (dados do jogador)
        SaveItemToMySQL(newItem);
        
        // PASSO 5: Informa o jogador
        SendItemToClient(player, newItem, itemTemplate);
        
        Logger.Info($"Item {itemTemplate.Name} x{quantity} dado para jogador {player.Name}");
    }
    
    private ItemTemplate GetItemTemplateFromSQLite(uint templateId)
    {
        using var connection = SQLite.CreateConnection();
        var query = "SELECT * FROM items WHERE id = @id";
        var cmd = new SQLiteCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", templateId);
        
        var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new ItemTemplate
            {
                Id = Convert.ToUInt32(reader["id"]),
                Name = reader["name"].ToString(),
                Description = reader["description"].ToString(),
                ItemType = (ItemType)Convert.ToInt32(reader["item_type"]),
                MaxStackSize = Convert.ToInt32(reader["max_stack_size"]),
                RequiredLevel = Convert.ToInt32(reader["required_level"]),
                MaxDurability = Convert.ToInt32(reader["max_durability"]),
                BasePrice = Convert.ToInt32(reader["base_price"])
                // ... outros campos
            };
        }
        
        return null;
    }
    
    private void SaveItemToMySQL(Item item)
    {
        using var connection = MySQL.CreateConnection();
        var query = @"
            INSERT INTO items (character_id, template_id, count, slot_type, slot, durability, enchant_level, created_at)
            VALUES (@charId, @templateId, @count, @slotType, @slot, @durability, @enchant, @created)";
        
        var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@charId", item.CharacterId);
        cmd.Parameters.AddWithValue("@templateId", item.TemplateId);
        cmd.Parameters.AddWithValue("@count", item.Count);
        cmd.Parameters.AddWithValue("@slotType", (int)item.SlotType);
        cmd.Parameters.AddWithValue("@slot", item.Slot);
        cmd.Parameters.AddWithValue("@durability", item.Durability);
        cmd.Parameters.AddWithValue("@enchant", item.EnchantLevel);
        cmd.Parameters.AddWithValue("@created", item.CreatedAt);
        
        cmd.ExecuteNonQuery();
    }
}
```

**👶 Explicação do fluxo:**
1. **Consulta a enciclopédia** (SQLite): "Como é uma Espada de Ferro?"
2. **Verifica regras** (SQLite): "Jogador pode usar?"
3. **Cria registro pessoal** (MySQL): "João ganhou Espada de Ferro única dele"
4. **Salva no arquivo pessoal** (MySQL): "Espada está no slot 5 do inventário"
5. **Informa João**: "Você ganhou uma Espada de Ferro!"

---

## 👤 **MÓDULO 10: SISTEMA DE PERSONAGENS ULTRA DETALHADO**

Agora vamos DISSECAR como funciona um personagem no AAEmu! É como estudar anatomia humana! 🧬

### **Arquivo: AAEmu.Game/Models/Game/Char/Character.cs (O DNA do Personagem)**

Vamos analisar esta classe GIGANTE (2801 linhas!) linha por linha:

```csharp
public partial class Character : Unit, ICharacter
{
    public override UnitTypeFlag TypeFlag { get; } = UnitTypeFlag.Character;
    public override BaseUnitType BaseUnitType => BaseUnitType.Character;
```

**🤔 Por que "partial class"?**
A classe Character é TÃO grande que foi dividida em vários arquivos! É como um livro de medicina dividido em volumes.

**👶 Explicação da herança:**
- **Character** herda de **Unit** (unidade básica do jogo)
- **Character** implementa **ICharacter** (contrato de personagem)

É como dizer: "Todo personagem É uma unidade do jogo E deve seguir as regras de personagem"

#### **Propriedades Básicas (Identidade)**

```csharp
public uint AccountId { get; set; }
public Race Race { get; set; }
public Gender Gender { get; set; }
public uint ServerId { get; set; }
```

**👶 Explicação:** São como o "RG" do personagem:
- **AccountId**: A que conta pertence
- **Race**: Elfo, Humano, Anão, etc.
- **Gender**: Masculino ou Feminino  
- **ServerId**: Em que servidor existe

#### **Sistema de Labor (Energia de Trabalho)**

```csharp
/// <summary>
/// Cached representation of Account Labor
/// </summary>
public short LaborPower
{
    get => _laborPower;
    set
    {
        if (_laborPower == value)
            return;
        _laborPower = value;
        AccountManager.Instance.UpdateLabor(AccountId, value);
    }
}
```

**🤔 O que é Labor Power?**
É como "energia de trabalho" do ArcheAge. Você gasta labor para:
- Cortar árvores
- Minerar pedras
- Criar itens
- Plantar crops

**👶 Explicação do código:**
1. **get => _laborPower** = "Quando alguém perguntar meu labor, responda o valor guardado"
2. **if (_laborPower == value) return** = "Se o novo valor é igual ao atual, não faça nada"
3. **_laborPower = value** = "Guarda o novo valor"
4. **AccountManager.Instance.UpdateLabor(...)** = "Avisa o gerente de contas que mudou"

**🎯 Por que essa complexidade?**
Labor é compartilhado entre TODOS os personagens da conta! Se você gastar labor num personagem, todos os outros da mesma conta são afetados.

#### **Sistema de Habilidades (Suas Especialidades)**

```csharp
public AbilityType Ability1 { get; set; }
public AbilityType Ability2 { get; set; }
public AbilityType Ability3 { get; set; }
```

**👶 Explicação:** Em ArcheAge, você pode escolher 3 "especializações":
- **Ability1**: Magia (ex: Feitiçaria)
- **Ability2**: Combate (ex: Combate)  
- **Ability3**: Sobrevivência (ex: Defesa)

A combinação define sua "classe":
- Feitiçaria + Combate + Defesa = "Spellsword"
- Tiro + Sobrevivência + Furtividade = "Primeval"

#### **Sistema de Combate**

```csharp
public DateTime LastCast { get; set; }
public bool IsInPostCast { get; set; }
public bool IgnoreSkillCooldowns { get; set; }
```

**👶 Explicação:**
- **LastCast**: Quando foi a última vez que usou uma skill
- **IsInPostCast**: Está no "tempo de recuperação" após usar skill
- **IgnoreSkillCooldowns**: Admin/GM pode ignorar cooldowns

#### **Sistema Social (Relacionamentos)**

```csharp
public string FactionName { get; set; }
public string OriginFactionName { get; set; }
public uint Family { get; set; }
```

**👶 Explicação:**
- **FactionName**: Que nação atual (pode mudar através de imigração)
- **OriginFactionName**: Nação original (nunca muda)
- **Family**: ID da família adotiva

#### **Sistema de Morte**

```csharp
public short DeadCount { get; set; }
public DateTime DeadTime { get; set; }
public int RezWaitDuration { get; set; }
public DateTime RezTime { get; set; }
public int RezPenaltyDuration { get; set; }
```

**👶 Explicação:** Quando você morre no ArcheAge:
1. **DeadCount**: Quantas vezes morreu
2. **DeadTime**: Quando morreu  
3. **RezWaitDuration**: Quanto tempo deve esperar para reviver
4. **RezTime**: Quando pode reviver
5. **RezPenaltyDuration**: Quanto tempo fica com penalidade

#### **Sistema Econômico**

```csharp
public long Money { get; set; }
public long Money2 { get; set; }
public int HonorPoint { get; set; }
public int VocationPoint { get; set; }
public short CrimePoint { get; set; }
public int CrimeRecord { get; set; }
public int JuryPoint { get; set; }
```

**👶 Explicação das "moedas":
- **Money**: Ouro normal (comprar/vender)
- **Money2**: Gilda Stars (moeda especial)
- **HonorPoint**: Pontos de honra (PvP)
- **VocationPoint**: Pontos de vocação (trabalho)
- **CrimePoint**: Pontos de crime (ser bandido)
- **CrimeRecord**: Histórico criminal
- **JuryPoint**: Pontos de júri (julgar criminosos)

#### **Sistema de Experiência**

```csharp
public int Experience { get; private set; }
public int RecoverableExp { get; set; }
```

**🤔 Por que "private set" no Experience?**
Experiência só pode ser alterada através de funções específicas que validam se é legal. Evita hacks!

**👶 Explicação:**
- **Experience**: XP atual do personagem
- **RecoverableExp**: XP que pode ser "recuperado" se morrer

#### **Inventário e Itens**

```csharp
public const int MaxActionSlots = 85;
public ActionSlot[] Slots { get; set; }
public Inventory Inventory { get; set; }
public byte NumInventorySlots { get; set; }
public short NumBankSlots { get; set; }
public ItemContainer BuyBackItems { get; set; }
```

**👶 Explicação:**
- **MaxActionSlots = 85**: Quantos atalhos você pode ter na tela
- **Slots**: Seus atalhos (skills, itens, emotes)
- **Inventory**: Seu inventário principal
- **NumInventorySlots**: Quantos slots de inventário você tem
- **NumBankSlots**: Quantos slots de banco você tem  
- **BuyBackItems**: Itens que você vendeu e pode recomprar

#### **Sistemas Especializados**

```csharp
public BondDoodad Bonding { get; set; }
public CharacterQuests Quests { get; set; }
public CharacterMails Mails { get; set; }
public CharacterAppellations Appellations { get; set; }
public CharacterAbilities Abilities { get; set; }
public CharacterPortals Portals { get; set; }
```

**👶 Explicação:**
- **Bonding**: Com que objeto você está interagindo (crafting, coleta)
- **Quests**: Suas missões ativas/completas
- **Mails**: Seus e-mails no jogo
- **Appellations**: Seus títulos ("Destruidor de Dragões")
- **Abilities**: Suas habilidades e níveis
- **Portals**: Portais que você pode usar

---

## ⚔️ **MÓDULO 11: SISTEMA DE COMBATE E SKILLS**

Agora vamos entender como funciona o combate no AAEmu! É como entender como funciona uma luta de espadas! ⚔️

### **Como Funciona uma Skill (Habilidade)**

Vamos acompanhar o que acontece quando um jogador usa uma skill:

#### **1. Jogador Clica na Skill**

```csharp
// Pacote enviado pelo cliente
public class CSStartSkillPacket : GamePacket
{
    public uint SkillId { get; set; }
    public uint TargetId { get; set; }
    public SkillCasterType CasterType { get; set; }
    public BaseUnitType TargetType { get; set; }
    public float PosX { get; set; }
    public float PosY { get; set; }
    public float PosZ { get; set; }
    
    public override void Read(PacketStream stream)
    {
        SkillId = stream.ReadUInt32();
        TargetId = stream.ReadUInt32(); 
        CasterType = (SkillCasterType)stream.ReadByte();
        TargetType = (BaseUnitType)stream.ReadByte();
        PosX = stream.ReadSingle();
        PosY = stream.ReadSingle();
        PosZ = stream.ReadSingle();
    }
    
    public override void Execute()
    {
        // Aqui é onde a mágica acontece!
        var character = Connection.ActiveCharacter;
        if (character == null) return;
        
        // Chama o gerente de skills
        SkillManager.Instance.UseSkill(character, SkillId, TargetId, TargetType, PosX, PosY, PosZ);
    }
}
```

**👶 Explicação:** É como um bilhete que diz:
- "Eu quero usar a skill #1234"
- "No alvo #5678"  
- "Na posição X=100, Y=200, Z=50"

#### **2. Servidor Processa a Skill**

```csharp
public class SkillManager
{
    public void UseSkill(Character caster, uint skillId, uint targetId, BaseUnitType targetType, float posX, float posY, float posZ)
    {
        // PASSO 1: Busca informações da skill no banco SQLite
        var skillTemplate = GetSkillTemplate(skillId);
        if (skillTemplate == null)
        {
            Logger.Error($"Skill {skillId} não encontrada!");
            return;
        }
        
        // PASSO 2: Verifica se jogador PODE usar esta skill
        if (!CanUseSkill(caster, skillTemplate))
        {
            SendSkillError(caster, SkillError.NotEnoughMP);
            return;
        }
        
        // PASSO 3: Encontra o alvo
        BaseUnit target = null;
        if (targetId > 0)
        {
            target = WorldManager.Instance.GetUnit(targetId);
            if (target == null)
            {
                SendSkillError(caster, SkillError.InvalidTarget);
                return;
            }
        }
        
        // PASSO 4: Verifica distância e linha de visão
        if (target != null && !IsInRange(caster, target, skillTemplate.Range))
        {
            SendSkillError(caster, SkillError.TooFarAway);
            return;
        }
        
        // PASSO 5: Verifica cooldown
        if (IsInCooldown(caster, skillId))
        {
            SendSkillError(caster, SkillError.InCooldown);
            return;
        }
        
        // PASSO 6: Consome recursos (MP, HP, Labor, etc)
        ConsumeResources(caster, skillTemplate);
        
        // PASSO 7: Inicia o cast (tempo de preparação)
        StartCasting(caster, skillTemplate, target, posX, posY, posZ);
    }
    
    private bool CanUseSkill(Character caster, SkillTemplate skill)
    {
        // Verifica MP
        if (caster.Mp < skill.MpCost)
            return false;
            
        // Verifica nível da skill
        var playerSkill = caster.Skills.GetSkill(skill.Id);
        if (playerSkill == null || playerSkill.Level < skill.RequiredLevel)
            return false;
            
        // Verifica se está vivo
        if (caster.Hp <= 0)
            return false;
            
        // Verifica se não está em outro cast
        if (caster.IsInPostCast)
            return false;
            
        return true;
    }
    
    private void StartCasting(Character caster, SkillTemplate skill, BaseUnit target, float posX, float posY, float posZ)
    {
        // Cria objeto que representa o cast em andamento
        var casting = new ActiveSkillCast
        {
            Caster = caster,
            Skill = skill,
            Target = target,
            TargetPosition = new Vector3(posX, posY, posZ),
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddMilliseconds(skill.CastTime),
            State = SkillCastState.Casting
        };
        
        // Adiciona na lista de casts ativos
        _activeCasts.Add(casting);
        
        // Marca personagem como "castando"
        caster.IsInPostCast = true;
        caster.LastCast = DateTime.Now;
        
        // Informa todos jogadores próximos que este jogador começou a castar
        var packet = new SCSkillStartedPacket
        {
            CasterId = caster.ObjId,
            SkillId = skill.Id,
            TargetId = target?.ObjId ?? 0,
            CastTime = skill.CastTime,
            PosX = posX,
            PosY = posY,
            PosZ = posZ
        };
        
        caster.BroadcastPacket(packet, true); // true = incluir próprio jogador
        
        // Agenda execução da skill para quando o cast terminar
        TaskManager.Instance.Schedule(() => ExecuteSkill(casting), skill.CastTime);
    }
}
```

#### **3. Executando a Skill (Quando Cast Termina)**

```csharp
private void ExecuteSkill(ActiveSkillCast casting)
{
    // Verifica se cast não foi cancelado
    if (casting.State != SkillCastState.Casting)
        return;
        
    var caster = casting.Caster;
    var skill = casting.Skill;
    var target = casting.Target;
    
    // Marca cast como completo
    casting.State = SkillCastState.Completed;
    caster.IsInPostCast = false;
    
    // Aplica cooldown
    ApplyCooldown(caster, skill.Id, skill.Cooldown);
    
    // Executa efeitos da skill baseado no tipo
    switch (skill.SkillType)
    {
        case SkillType.Damage:
            ExecuteDamageSkill(caster, target, skill);
            break;
            
        case SkillType.Heal:
            ExecuteHealSkill(caster, target, skill);
            break;
            
        case SkillType.Buff:
            ExecuteBuffSkill(caster, target, skill);
            break;
            
        case SkillType.Teleport:
            ExecuteTeleportSkill(caster, casting.TargetPosition, skill);
            break;
            
        default:
            Logger.Error($"Tipo de skill não implementado: {skill.SkillType}");
            break;
    }
    
    // Remove da lista de casts ativos
    _activeCasts.Remove(casting);
    
    // Informa que skill foi executada
    var packet = new SCSkillExecutedPacket
    {
        CasterId = caster.ObjId,
        SkillId = skill.Id,
        TargetId = target?.ObjId ?? 0
    };
    
    caster.BroadcastPacket(packet, true);
}
```

#### **4. Exemplo: Skill de Dano**

```csharp
private void ExecuteDamageSkill(Character caster, BaseUnit target, SkillTemplate skill)
{
    if (target == null || target.Hp <= 0)
        return;
        
    // CALCULA O DANO
    var damage = CalculateDamage(caster, target, skill);
    
    // APLICA O DANO
    target.ReduceHp(caster, damage);
    
    // VERIFICA SE MATOU
    if (target.Hp <= 0)
    {
        target.Die(caster);
        
        // Se matou jogador, ganha experiência
        if (target is Character targetChar && caster != targetChar)
        {
            var expGain = CalculateExperienceGain(caster, targetChar);
            caster.AddExperience(expGain);
        }
    }
    
    // INFORMA O DANO
    var damagePacket = new SCDamagePacket
    {
        AttackerId = caster.ObjId,
        TargetId = target.ObjId,
        Damage = damage,
        DamageType = skill.DamageType,
        IsCritical = damage > skill.BaseDamage * 1.5f // Critial se > 150% do dano base
    };
    
    // Envia para todos jogadores próximos
    WorldManager.Instance.BroadcastPacketToRange(caster, damagePacket, 100f);
}

private int CalculateDamage(Character caster, BaseUnit target, SkillTemplate skill)
{
    // Fórmula básica de dano
    var baseDamage = skill.BaseDamage;
    var attackPower = caster.TotalAp; // Attack Power total do jogador
    var defense = target.Defense;
    
    // Aplica modificadores de atributos
    var strBonus = caster.Str * skill.StrModifier;
    var intBonus = caster.Int * skill.IntModifier;
    var agiBonus = caster.Agi * skill.AgiModifier;
    
    // Calcula dano bruto
    var rawDamage = baseDamage + attackPower + strBonus + intBonus + agiBonus;
    
    // Aplica defesa (defesa reduz % do dano)
    var damageReduction = defense / (defense + 100f); // Fórmula típica de MMO
    var finalDamage = (int)(rawDamage * (1f - damageReduction));
    
    // Chance de crítico
    var critChance = caster.CritRate / 100f;
    if (Random.Shared.NextSingle() <= critChance)
    {
        finalDamage = (int)(finalDamage * caster.CritDamage);
    }
    
    // Variação aleatória (±10%)
    var variation = Random.Shared.Next(-10, 11) / 100f;
    finalDamage = (int)(finalDamage * (1f + variation));
    
    // Dano mínimo de 1
    return Math.Max(1, finalDamage);
}
```

**👶 Explicação do combate:**
1. **Jogador clica**: "Quero usar Bola de Fogo"
2. **Servidor verifica**: "Você pode? Tem MP? Não está em cooldown?"
3. **Inicia cast**: "Ok, você vai castar por 3 segundos"
4. **Termina cast**: "Pronto! Bola de Fogo executada!"
5. **Calcula dano**: "Seu INT + poder da skill - defesa do inimigo = 150 de dano"
6. **Aplica efeitos**: "Inimigo perdeu 150 HP e pegou fogo por 10 segundos"
7. **Informa everyone**: "João usou Bola de Fogo em Goblin por 150 de dano!"

---

## 🏠 **MÓDULO 12: SISTEMA DE HOUSING (CASAS)**

O sistema de housing do ArcheAge é um dos mais complexos! Vamos entender como funciona:

### **Conceitos Básicos do Housing**

#### **🏗️ O que é Housing no ArcheAge?**

**👶 Explicação:** Housing é como um jogo de "SimCity" dentro do ArcheAge:
- Você pode **comprar terrenos**
- **Construir casas** personalizadas
- **Decorar** como quiser
- **Plantar** crops ao redor
- **Pagar impostos** para manter

#### **Tipos de Construções:**

```csharp
public enum HouseType
{
    Farmhouse = 1,      // Casa de fazenda (pequena)
    Cottage = 2,        // Chalé (média)
    Mansion = 3,        // Mansão (grande)
    Castle = 4,         // Castelo (gigante)
    TownHouse = 5,      // Casa da cidade
    Warehouse = 6,      // Armazém
    Windmill = 7,       // Moinho de vento
    Sawmill = 8,        // Serraria
    Mill = 9            // Moinho
}
```

#### **Estados de uma Casa:**

```csharp
public enum HouseState
{
    UnBuilt = 0,        // Não construída (só o terreno)
    Building = 1,       // Em construção
    Built = 2,          // Construída e funcionando
    Sold = 3,           // Vendida (esperando comprador)
    Demolishing = 4,    // Sendo demolida
    Destroyed = 5       // Destruída (por falta de imposto)
}
```

### **Processo de Construção de Casa**

#### **1. Jogador Escolhe Local e Tipo**

```csharp
public class CSCreateHousePacket : GamePacket
{
    public uint HouseTemplateId { get; set; }  // Tipo de casa
    public float PosX { get; set; }            // Posição X
    public float PosY { get; set; }            // Posição Y  
    public float PosZ { get; set; }            // Posição Z
    public float RotZ { get; set; }            // Rotação
    
    public override void Execute()
    {
        var character = Connection.ActiveCharacter;
        if (character == null) return;
        
        HousingManager.Instance.CreateHouse(character, HouseTemplateId, PosX, PosY, PosZ, RotZ);
    }
}
```

#### **2. Servidor Valida Local**

```csharp
public class HousingManager
{
    public void CreateHouse(Character character, uint templateId, float x, float y, float z, float rotZ)
    {
        // PASSO 1: Busca template da casa
        var houseTemplate = GetHouseTemplate(templateId);
        if (houseTemplate == null)
        {
            SendHousingError(character, HousingError.InvalidTemplate);
            return;
        }
        
        // PASSO 2: Verifica se jogador tem dinheiro
        if (character.Money < houseTemplate.BuildCost)
        {
            SendHousingError(character, HousingError.NotEnoughMoney);
            return;
        }
        
        // PASSO 3: Verifica se local é válido
        if (!IsValidHouseLocation(x, y, z, houseTemplate))
        {
            SendHousingError(character, HousingError.InvalidLocation);
            return;
        }
        
        // PASSO 4: Verifica conflitos com outras casas
        if (HasHouseConflict(x, y, z, houseTemplate))
        {
            SendHousingError(character, HousingError.LocationOccupied);
            return;
        }
        
        // PASSO 5: Verifica limite de casas por jogador
        var playerHouseCount = GetPlayerHouseCount(character.Id);
        if (playerHouseCount >= GetMaxHousesPerPlayer(character.AccountId))
        {
            SendHousingError(character, HousingError.TooManyHouses);
            return;
        }
        
        // PASSO 6: Cobra o dinheiro
        character.Money -= houseTemplate.BuildCost;
        
        // PASSO 7: Cria a casa no banco de dados
        var house = new House
        {
            Id = HousingIdManager.Instance.GetNextId(),
            OwnerId = character.Id,
            OwnerName = character.Name,
            TemplateId = templateId,
            Name = $"{character.Name}'s {houseTemplate.Name}",
            X = x,
            Y = y,
            Z = z,
            RotZ = rotZ,
            State = HouseState.UnBuilt,
            BuildStep = 0,
            TaxDueDate = DateTime.Now.AddDays(7), // 7 dias para pagar primeira taxa
            CreatedAt = DateTime.Now
        };
        
        SaveHouse(house);
        
        // PASSO 8: Spawna a casa no mundo (estado "foundation")
        SpawnHouseInWorld(house);
        
        // PASSO 9: Informa jogador
        SendHouseCreated(character, house);
        
        Logger.Info($"Casa {houseTemplate.Name} criada por {character.Name} em ({x}, {y}, {z})");
    }
    
    private bool IsValidHouseLocation(float x, float y, float z, HouseTemplate template)
    {
        // Verifica se está numa zona permitida para housing
        var zone = ZoneManager.Instance.GetZoneByPosition(x, y, z);
        if (zone == null || !zone.AllowHousing)
            return false;
            
        // Verifica se não está muito perto da água
        if (IsNearWater(x, y, z, 10f))
            return false;
            
        // Verifica se terreno não é muito inclinado
        var heightDifference = GetTerrainHeightDifference(x, y, template.Width, template.Height);
        if (heightDifference > template.MaxHeightDifference)
            return false;
            
        // Verifica se não está em área protegida (cidades, dungeons)
        if (IsProtectedArea(x, y, z))
            return false;
            
        return true;
    }
    
    private bool HasHouseConflict(float x, float y, float z, HouseTemplate template)
    {
        // Define área da nova casa
        var newHouseBounds = new Rectangle(
            x - template.Width / 2f,
            y - template.Height / 2f,
            template.Width,
            template.Height
        );
        
        // Verifica todas as casas próximas
        var nearbyHouses = GetHousesInRange(x, y, template.Width + template.Height);
        
        foreach (var house in nearbyHouses)
        {
            var houseTemplate = GetHouseTemplate(house.TemplateId);
            var existingBounds = new Rectangle(
                house.X - houseTemplate.Width / 2f,
                house.Y - houseTemplate.Height / 2f,
                houseTemplate.Width,
                houseTemplate.Height
            );
            
            // Se as áreas se sobrepõem, há conflito
            if (newHouseBounds.IntersectsWith(existingBounds))
                return true;
        }
        
        return false;
    }
}
```

#### **3. Processo de Construção**

```csharp
public void BuildHouse(Character character, uint houseId, uint materialTemplateId, int quantity)
{
    var house = GetHouse(houseId);
    if (house == null || house.OwnerId != character.Id)
    {
        SendHousingError(character, HousingError.NotYourHouse);
        return;
    }
    
    if (house.State != HouseState.UnBuilt)
    {
        SendHousingError(character, HousingError.AlreadyBuilt);
        return;
    }
    
    var houseTemplate = GetHouseTemplate(house.TemplateId);
    var currentStep = house.BuildStep;
    
    // Verifica se material é correto para esta etapa
    var requiredMaterial = houseTemplate.BuildSteps[currentStep];
    if (requiredMaterial.MaterialId != materialTemplateId)
    {
        SendHousingError(character, HousingError.WrongMaterial);
        return;
    }
    
    // Verifica se jogador tem material suficiente
    var playerMaterial = character.Inventory.GetItemByTemplateId(materialTemplateId);
    if (playerMaterial == null || playerMaterial.Count < requiredMaterial.Quantity)
    {
        SendHousingError(character, HousingError.NotEnoughMaterials);
        return;
    }
    
    // Consome os materiais
    character.Inventory.ConsumeItem(materialTemplateId, requiredMaterial.Quantity);
    
    // Avança etapa de construção
    house.BuildStep++;
    
    // Verifica se terminou de construir
    if (house.BuildStep >= houseTemplate.BuildSteps.Count)
    {
        house.State = HouseState.Built;
        house.CompletedAt = DateTime.Now;
        
        // Spawna versão completa da casa
        DespawnHouseFromWorld(house.Id);
        SpawnCompleteHouse(house);
        
        SendHouseCompleted(character, house);
        Logger.Info($"Casa {house.Name} concluída por {character.Name}");
    }
    else
    {
        // Atualiza visual da construção
        UpdateHouseConstructionVisual(house);
        SendConstructionProgress(character, house);
    }
    
    // Salva progresso
    SaveHouse(house);
}
```

#### **4. Sistema de Impostos**

```csharp
public void ProcessHouseTaxes()
{
    var housesWithTaxDue = GetHousesWithTaxDue();
    
    foreach (var house in housesWithTaxDue)
    {
        var owner = CharacterManager.Instance.GetCharacter(house.OwnerId);
        var houseTemplate = GetHouseTemplate(house.TemplateId);
        var weeklyTax = houseTemplate.WeeklyTax;
        
        if (owner != null && owner.Money >= weeklyTax)
        {
            // Jogador pode pagar
            owner.Money -= weeklyTax;
            house.TaxDueDate = house.TaxDueDate.AddDays(7);
            house.TaxPaidUntil = house.TaxDueDate;
            
            SendTaxPaid(owner, house, weeklyTax);
            SaveHouse(house);
            
            Logger.Info($"Taxa de {weeklyTax} paga para casa {house.Name} por {owner.Name}");
        }
        else
        {
            // Jogador não pode pagar - casa vira "heavy tax"
            house.State = HouseState.Destroyed;
            house.DestroyedAt = DateTime.Now;
            house.DestroyReason = "Tax not paid";
            
            // Remove casa do mundo
            DespawnHouseFromWorld(house.Id);
            
            // Cria pacote de demolição com 50% dos materiais
            CreateDemolitionPackage(house);
            
            // Notifica owner se estiver online
            if (owner != null)
            {
                SendHouseDestroyed(owner, house, "Taxes not paid");
            }
            
            SaveHouse(house);
            Logger.Info($"Casa {house.Name} de {house.OwnerName} destruída por falta de pagamento");
        }
    }
}
```

**👶 Explicação do sistema de impostos:**
1. **Toda semana** você deve pagar taxa da casa
2. **Se não pagar** = casa vira "heavy tax" (vermelho no mapa)  
3. **Depois de tempo** = casa é demolida automaticamente
4. **50% dos materiais** voltam como "pacote de demolição"
5. **Terreno fica livre** para outro jogador construir

---

## 🎉 **PARABÉNS! VOCÊ CONCLUIU OS MÓDULOS 8-12!**

Você agora entende:

✅ **Sistema de Networking** - Como pacotes viajam e são processados  
✅ **Sistema de Banco de Dados** - Como SQLite + MySQL trabalham juntos  
✅ **Sistema de Personagens** - Anatomia completa de um Character  
✅ **Sistema de Combate** - Como skills e dano funcionam  
✅ **Sistema de Housing** - Como casas são construídas e mantidas  

### 🚀 **PRÓXIMOS MÓDULOS DISPONÍVEIS:**

- **MÓDULO 13**: Sistema de Quests e NPCs
- **MÓDULO 14**: Sistema de Economia e Trade
- **MÓDULO 15**: Sistema de AI e Pathfinding  
- **MÓDULO 16**: Performance e Otimização
- **MÓDULO 17**: Testing e Debugging
- **MÓDULO 18**: Deployment e Manutenção

**Continue me perguntando! Este mega curso não tem fim!** 🎓✨