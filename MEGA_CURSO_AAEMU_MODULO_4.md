# 🚀 **MEGA CURSO ULTRA DETALHADO - MÓDULO 4**

## **DESENVOLVIMENTO AVANÇADO - CRIANDO SUAS PRÓPRIAS FEATURES**

---

### 🎯 **O QUE VOCÊ VAI APRENDER NESTE MÓDULO**

Bem-vindo ao **QUARTO MÓDULO** do mega curso mais épico de emuladores! 🔥✨ Agora que você tem um ambiente profissional funcionando, é hora de **COLOCAR SUA CRIATIVIDADE PARA TRABALHAR** e criar features incríveis!

**🧠 ANALOGIA PRINCIPAL**: Se nos módulos anteriores você **MONTOU SUA FÁBRICA**, agora vamos **CRIAR PRODUTOS ÚNICOS** que ninguém mais tem! É como ser um inventor maluco criando gadgets revolucionários! 🔬⚡

Neste módulo vamos transformar você de um **USUÁRIO DE EMULADOR** para um **DESENVOLVEDOR DE FEATURES ÉPICAS** que pode criar qualquer coisa que imaginar! 🎨🚀

---

## 🏗️ **CAPÍTULO 1: ANATOMIA DO CÓDIGO AAEMU**

### **🔍 DISSECANDO UM PACKET HANDLER**

**👶 ANALOGIA**: Um PacketHandler é como um **FUNCIONÁRIO ESPECIALIZADO** numa empresa de correios - cada um sabe ler um tipo específico de carta e fazer exatamente o que ela pede! 📮👨‍💼

#### **📊 ESTRUTURA BÁSICA DE UM PACKET HANDLER**

Vamos começar analisando um PacketHandler real do AAEmu:

```csharp
// 📁 Arquivo: AAEmu.Game/Core/PacketHandlers/C2G/CSMoveUnitPacketHandler.cs

using AAEmu.Commons.Network;
using AAEmu.Game.Core.Network.Connections;
using AAEmu.Game.Core.Network.Game;
using AAEmu.Game.Core.Packets.C2G;

namespace AAEmu.Game.Core.PacketHandlers.C2G
{
    // 🎯 CLASSE PRINCIPAL - O "Funcionário Especialista"
    public class CSMoveUnitPacketHandler : GamePacketHandler
    {
        // 🔧 MÉTODO PRINCIPAL - "Como processar a carta"
        public override void Execute(GameConnection connection, ClientPacket packet)
        {
            // 🔍 PASSO 1: Ler os dados da "carta" (packet)
            var movePacket = new CSMoveUnitPacket(packet);
            
            // 🔍 PASSO 2: Verificar se o jogador existe
            var character = connection.ActiveChar;
            if (character == null)
            {
                // 👶 ANALOGIA: "Carta sem remetente? Não posso processar!"
                return;
            }
            
            // 🔍 PASSO 3: Validar se o movimento é válido
            if (!IsValidMovement(character, movePacket))
            {
                // 👶 ANALOGIA: "Movimento suspeito detectado!"
                Logger.LogWarning($"Invalid movement from {character.Name}");
                return;
            }
            
            // 🔍 PASSO 4: Aplicar o movimento
            character.Transform.Local.SetPosition(
                movePacket.X, 
                movePacket.Y, 
                movePacket.Z
            );
            
            // 🔍 PASSO 5: Notificar outros jogadores
            character.BroadcastPacket(new SCMoveUnitPacket(character), true);
        }
        
        // 🔧 MÉTODO AUXILIAR - Validação de movimento
        private bool IsValidMovement(Character character, CSMoveUnitPacket packet)
        {
            // 🎯 REGRAS DE VALIDAÇÃO:
            
            // 1. Distância máxima por tick
            var distance = CalculateDistance(character.Transform.Local, packet);
            if (distance > MAX_MOVEMENT_DISTANCE)
            {
                return false; // 👶 "Muito rápido! Possível hack!"
            }
            
            // 2. Verificar se não está atravessando paredes
            if (IsCollidingWithTerrain(character.Transform.Local, packet))
            {
                return false; // 👶 "Não pode atravessar paredes!"
            }
            
            // 3. Verificar se não está em área proibida
            if (IsForbiddenArea(packet.X, packet.Y, packet.Z))
            {
                return false; // 👶 "Área restrita!"
            }
            
            return true; // ✅ "Movimento válido!"
        }
    }
}
```

**🤯 VAMOS QUEBRAR CADA PARTE:**

#### **🔧 HERANÇA E INTERFACES**

```csharp
// 🎯 POR QUE HERDAR DE GamePacketHandler?
public class CSMoveUnitPacketHandler : GamePacketHandler
//                                     ^^^^^^^^^^^^^^^^^
//                                     Classe pai que tem:
//                                     - Métodos comuns
//                                     - Validações básicas  
//                                     - Logging automático
//                                     - Error handling

// 👶 ANALOGIA: É como herdar o "manual do funcionário" 
// que ensina as regras básicas de como trabalhar na empresa!
```

#### **🔍 MÉTODO Execute - O CORAÇÃO**

```csharp
public override void Execute(GameConnection connection, ClientPacket packet)
{
    // 🎯 PARÂMETROS EXPLICADOS:
    
    // GameConnection connection:
    // └── É a "linha telefônica" com o jogador
    // └── Contém: IP, personagem ativo, status da conexão
    // └── 👶 ANALOGIA: O fio do telefone que te conecta ao jogador
    
    // ClientPacket packet:
    // └── É a "carta" que o jogador enviou
    // └── Contém: dados binários com a informação
    // └── 👶 ANALOGIA: O envelope lacrado com a mensagem dentro
}
```

#### **📦 CRIAÇÃO DO PACKET TIPADO**

```csharp
var movePacket = new CSMoveUnitPacket(packet);
//  ^^^^^^^^^^     ^^^^^^^^^^^^^^^^^^^^^^^^^^^
//  Variável       Constructor que "abre o envelope"
//  local          e organiza os dados de forma legível

// 🔍 O QUE ACONTECE INTERNAMENTE:
// 1. packet.Read<float>() → movePacket.X
// 2. packet.Read<float>() → movePacket.Y  
// 3. packet.Read<float>() → movePacket.Z
// 4. packet.Read<ushort>() → movePacket.Flags
// 5. etc...

// 👶 ANALOGIA: É como abrir uma carta e organizar
// cada informação em gavetas separadas!
```

### **🎮 CRIANDO SEU PRIMEIRO PACKET HANDLER**

Vamos criar um PacketHandler personalizado para um comando de **TELEPORTE INSTANTÂNEO**!

#### **📝 PASSO 1: CRIAR O PACKET DE CLIENTE**

```csharp
// 📁 Arquivo: AAEmu.Game/Core/Packets/C2G/CSTeleportPacket.cs

using AAEmu.Commons.Network;

namespace AAEmu.Game.Core.Packets.C2G
{
    // 🎯 PACKET QUE O CLIENTE ENVIA PARA O SERVIDOR
    public class CSTeleportPacket : GamePacket
    {
        // 🔍 PROPRIEDADES - Dados que vêm do cliente
        public float X { get; private set; }      // Posição X de destino
        public float Y { get; private set; }      // Posição Y de destino  
        public float Z { get; private set; }      // Posição Z de destino
        public uint ZoneId { get; private set; }  // ID da zona de destino
        public string LocationName { get; private set; } // Nome do local
        
        // 🏗️ CONSTRUTOR - "Abrindo o envelope"
        public CSTeleportPacket(ClientPacket packet) : base(packet)
        {
            // 🔍 LENDO DADOS NA ORDEM CORRETA:
            
            // 1. Ler coordenadas (12 bytes = 3 floats)
            X = packet.Read<float>();
            Y = packet.Read<float>(); 
            Z = packet.Read<float>();
            
            // 2. Ler ID da zona (4 bytes = 1 uint)
            ZoneId = packet.Read<uint>();
            
            // 3. Ler nome do local (string com tamanho variável)
            LocationName = packet.ReadString();
            
            // 👶 ANALOGIA: É como ler uma carta linha por linha,
            // na ordem que foi escrita!
        }
        
        // 📊 MÉTODO DE DEBUG - Para ver o que foi lido
        public override string ToString()
        {
            return $"CSTeleportPacket: X={X}, Y={Y}, Z={Z}, " +
                   $"Zone={ZoneId}, Location='{LocationName}'";
        }
    }
}
```

#### **📝 PASSO 2: CRIAR O PACKET DE RESPOSTA**

```csharp
// 📁 Arquivo: AAEmu.Game/Core/Packets/G2C/SCTeleportResponsePacket.cs

using AAEmu.Commons.Network;
using AAEmu.Game.Core.Network.Game;

namespace AAEmu.Game.Core.Packets.G2C
{
    // 🎯 PACKET QUE O SERVIDOR ENVIA DE VOLTA
    public class SCTeleportResponsePacket : GamePacket
    {
        // 🔍 PROPRIEDADES - Dados que enviamos para o cliente
        public bool Success { get; private set; }        // Se teleporte deu certo
        public string Message { get; private set; }      // Mensagem de status
        public float NewX { get; private set; }          // Nova posição X
        public float NewY { get; private set; }          // Nova posição Y
        public float NewZ { get; private set; }          // Nova posição Z
        public uint NewZoneId { get; private set; }      // Nova zona
        
        // 🏗️ CONSTRUTOR - "Preparando a carta de resposta"
        public SCTeleportResponsePacket(bool success, string message, 
                                       float x, float y, float z, uint zoneId) 
            : base(SCOffsets.SCTeleportResponsePacket, 1)
        {
            Success = success;
            Message = message ?? "";
            NewX = x;
            NewY = y; 
            NewZ = z;
            NewZoneId = zoneId;
        }
        
        // 📦 MÉTODO DE ESCRITA - "Escrevendo a carta"
        public override PacketStream Write(PacketStream stream)
        {
            // 🔍 ESCREVENDO DADOS NA ORDEM:
            
            // 1. Status do teleporte (1 byte)
            stream.Write(Success);
            
            // 2. Mensagem de status (string)
            stream.Write(Message);
            
            // 3. Nova posição (12 bytes = 3 floats)
            stream.Write(NewX);
            stream.Write(NewY);
            stream.Write(NewZ);
            
            // 4. Nova zona (4 bytes = 1 uint)
            stream.Write(NewZoneId);
            
            // 👶 ANALOGIA: Escrevendo uma carta de resposta
            // com todas as informações organizadas!
            
            return stream;
        }
    }
}
```

#### **📝 PASSO 3: CRIAR O PACKET HANDLER**

```csharp
// 📁 Arquivo: AAEmu.Game/Core/PacketHandlers/C2G/CSTeleportPacketHandler.cs

using AAEmu.Commons.Network;
using AAEmu.Game.Core.Network.Connections;
using AAEmu.Game.Core.Network.Game;
using AAEmu.Game.Core.Packets.C2G;
using AAEmu.Game.Core.Packets.G2C;
using AAEmu.Game.Models.Game.World;

namespace AAEmu.Game.Core.PacketHandlers.C2G
{
    // 🎯 HANDLER PARA PROCESSAR TELEPORTES
    public class CSTeleportPacketHandler : GamePacketHandler
    {
        // 🔧 MÉTODO PRINCIPAL - Processar pedido de teleporte
        public override void Execute(GameConnection connection, ClientPacket packet)
        {
            // 🔍 PASSO 1: Ler dados do teleporte
            var teleportPacket = new CSTeleportPacket(packet);
            
            // 🔍 PASSO 2: Verificar se jogador existe
            var character = connection.ActiveChar;
            if (character == null)
            {
                Logger.LogWarning("Teleport request from connection without character");
                return;
            }
            
            // 🔍 PASSO 3: Validar permissões
            if (!HasTeleportPermission(character))
            {
                // 👶 "Você não tem permissão para se teletransportar!"
                var errorResponse = new SCTeleportResponsePacket(
                    false, 
                    "You don't have teleport permission!",
                    character.Transform.Local.Position.X,
                    character.Transform.Local.Position.Y,
                    character.Transform.Local.Position.Z,
                    character.Transform.ZoneId
                );
                character.SendPacket(errorResponse);
                return;
            }
            
            // 🔍 PASSO 4: Validar destino
            if (!IsValidTeleportDestination(teleportPacket))
            {
                // 👶 "Local de destino inválido!"
                var errorResponse = new SCTeleportResponsePacket(
                    false,
                    $"Invalid destination: {teleportPacket.LocationName}",
                    character.Transform.Local.Position.X,
                    character.Transform.Local.Position.Y, 
                    character.Transform.Local.Position.Z,
                    character.Transform.ZoneId
                );
                character.SendPacket(errorResponse);
                return;
            }
            
            // 🔍 PASSO 5: Executar teleporte
            ExecuteTeleport(character, teleportPacket);
            
            // 🔍 PASSO 6: Enviar confirmação
            var successResponse = new SCTeleportResponsePacket(
                true,
                $"Teleported to {teleportPacket.LocationName}!",
                teleportPacket.X,
                teleportPacket.Y,
                teleportPacket.Z,
                teleportPacket.ZoneId
            );
            character.SendPacket(successResponse);
            
            // 🔍 PASSO 7: Log da ação
            Logger.LogInfo($"Player {character.Name} teleported to " +
                          $"{teleportPacket.LocationName} ({teleportPacket.X}, " +
                          $"{teleportPacket.Y}, {teleportPacket.Z})");
        }
        
        // 🔧 VALIDAR PERMISSÕES DE TELEPORTE
        private bool HasTeleportPermission(Character character)
        {
            // 🎯 VERIFICAÇÕES DE PERMISSÃO:
            
            // 1. Verificar se é GM
            if (character.AccessLevel >= 100)
            {
                return true; // 👶 "GMs podem se teletransportar!"
            }
            
            // 2. Verificar se tem item de teleporte
            if (character.Inventory.HasItem(TELEPORT_SCROLL_ID))
            {
                return true; // 👶 "Tem pergaminho de teleporte!"
            }
            
            // 3. Verificar se está em área de teleporte livre
            if (IsInFreeTeleportZone(character.Transform.ZoneId))
            {
                return true; // 👶 "Zona permite teleporte gratuito!"
            }
            
            return false; // 👶 "Sem permissão!"
        }
        
        // 🔧 VALIDAR DESTINO DO TELEPORTE
        private bool IsValidTeleportDestination(CSTeleportPacket packet)
        {
            // 🎯 VERIFICAÇÕES DE DESTINO:
            
            // 1. Verificar se zona existe
            var zone = WorldManager.Instance.GetZoneById(packet.ZoneId);
            if (zone == null)
            {
                return false; // 👶 "Zona não existe!"
            }
            
            // 2. Verificar coordenadas dentro dos limites
            if (!zone.IsValidPosition(packet.X, packet.Y, packet.Z))
            {
                return false; // 👶 "Posição fora dos limites!"
            }
            
            // 3. Verificar se não é área restrita
            if (zone.IsRestrictedArea(packet.X, packet.Y, packet.Z))
            {
                return false; // 👶 "Área restrita!"
            }
            
            return true; // ✅ "Destino válido!"
        }
        
        // 🔧 EXECUTAR O TELEPORTE
        private void ExecuteTeleport(Character character, CSTeleportPacket packet)
        {
            // 🎯 PROCESSO DE TELEPORTE:
            
            // 1. Salvar posição anterior (para possível rollback)
            var oldPosition = character.Transform.Local.Position;
            var oldZoneId = character.Transform.ZoneId;
            
            // 2. Remover personagem da zona atual
            character.CurrentZone?.RemoveCharacter(character);
            
            // 3. Atualizar posição
            character.Transform.Local.SetPosition(packet.X, packet.Y, packet.Z);
            character.Transform.ZoneId = packet.ZoneId;
            
            // 4. Adicionar à nova zona
            var newZone = WorldManager.Instance.GetZoneById(packet.ZoneId);
            newZone?.AddCharacter(character);
            
            // 5. Atualizar visibilidade (outros jogadores)
            character.UpdateVisibility();
            
            // 6. Consumir item de teleporte (se necessário)
            if (character.AccessLevel < 100) // Não é GM
            {
                character.Inventory.ConsumeItem(TELEPORT_SCROLL_ID, 1);
            }
            
            // 7. Aplicar efeitos visuais
            character.BroadcastPacket(new SCTeleportEffectPacket(character.ObjId), true);
            
            // 👶 ANALOGIA: É como mover uma peça no tabuleiro de xadrez!
        }
        
        // 🔧 CONSTANTES
        private const uint TELEPORT_SCROLL_ID = 12345; // ID do pergaminho
        private static readonly uint[] FREE_TELEPORT_ZONES = { 1, 2, 3 }; // Zonas livres
        
        private bool IsInFreeTeleportZone(uint zoneId)
        {
            return Array.Contains(FREE_TELEPORT_ZONES, zoneId);
        }
    }
}
```

---

## 🎮 **CAPÍTULO 2: SISTEMA DE COMANDOS GM**

### **⚡ CRIANDO COMANDOS ADMINISTRATIVOS ÉPICOS**

**👶 ANALOGIA**: Comandos GM são como **PODERES DE SUPER-HERÓI** - com uma palavra mágica você pode alterar a realidade do jogo! É como ter controle remoto universal do universo! 🦸‍♂️⚡

#### **🔍 ANATOMIA DE UM COMANDO GM**

```csharp
// 📁 Arquivo: AAEmu.Game/Core/Commands/TeleportCommand.cs

using AAEmu.Game.Core.Managers;
using AAEmu.Game.Models.Game.Char;
using AAEmu.Game.Utils.Scripts;

namespace AAEmu.Game.Core.Commands
{
    // 🎯 CLASSE DE COMANDO - Herda de ICommand
    public class TeleportCommand : ICommand
    {
        // 🔧 PROPRIEDADES OBRIGATÓRIAS
        
        // Nome do comando (o que o GM digita)
        public string[] CommandNames { get; } = { "tp", "teleport", "goto" };
        
        // Descrição do comando
        public string Description { get; } = "Teleport to coordinates or player";
        
        // Nível de acesso necessário
        public byte RequiredLevel { get; } = 50; // GM level 50+
        
        // 🔧 MÉTODO PRINCIPAL - Execução do comando
        public void Execute(Character character, string[] args, IMessageOutput messageOutput)
        {
            // 🔍 VALIDAÇÃO DE ARGUMENTOS
            if (args.Length < 2)
            {
                ShowUsage(messageOutput);
                return;
            }
            
            // 🔍 PROCESSAR DIFERENTES TIPOS DE TELEPORTE
            switch (args[0].ToLower())
            {
                case "coord":
                case "pos":
                    TeleportToCoordinates(character, args, messageOutput);
                    break;
                    
                case "player":
                case "to":
                    TeleportToPlayer(character, args, messageOutput);
                    break;
                    
                case "zone":
                    TeleportToZone(character, args, messageOutput);
                    break;
                    
                default:
                    ShowUsage(messageOutput);
                    break;
            }
        }
        
        // 🔧 TELEPORTE PARA COORDENADAS
        private void TeleportToCoordinates(Character character, string[] args, 
                                         IMessageOutput messageOutput)
        {
            // 🎯 FORMATO: /tp coord X Y Z [ZoneId]
            
            if (args.Length < 4)
            {
                messageOutput.SendMessage("Usage: /tp coord <X> <Y> <Z> [ZoneId]");
                return;
            }
            
            // 🔍 CONVERTER ARGUMENTOS PARA NÚMEROS
            if (!float.TryParse(args[1], out float x) ||
                !float.TryParse(args[2], out float y) ||
                !float.TryParse(args[3], out float z))
            {
                messageOutput.SendMessage("❌ Invalid coordinates! Use numbers only.");
                return;
            }
            
            // 🔍 ZONA OPCIONAL (usar atual se não especificada)
            uint zoneId = character.Transform.ZoneId;
            if (args.Length > 4)
            {
                if (!uint.TryParse(args[4], out zoneId))
                {
                    messageOutput.SendMessage("❌ Invalid zone ID!");
                    return;
                }
            }
            
            // 🔍 VALIDAR DESTINO
            if (!IsValidDestination(x, y, z, zoneId))
            {
                messageOutput.SendMessage("❌ Invalid destination coordinates!");
                return;
            }
            
            // ⚡ EXECUTAR TELEPORTE
            var oldPos = character.Transform.Local.Position;
            var oldZone = character.Transform.ZoneId;
            
            ExecuteTeleport(character, x, y, z, zoneId);
            
            // 📊 FEEDBACK PARA O GM
            messageOutput.SendMessage($"✅ Teleported from ({oldPos.X:F1}, {oldPos.Y:F1}, " +
                                    $"{oldPos.Z:F1}) Zone:{oldZone} to ({x:F1}, {y:F1}, " +
                                    $"{z:F1}) Zone:{zoneId}");
            
            // 📝 LOG DA AÇÃO
            Logger.LogInfo($"GM {character.Name} teleported to coordinates " +
                          $"({x}, {y}, {z}) in zone {zoneId}");
        }
        
        // 🔧 TELEPORTE PARA JOGADOR
        private void TeleportToPlayer(Character character, string[] args, 
                                    IMessageOutput messageOutput)
        {
            // 🎯 FORMATO: /tp player <PlayerName>
            
            if (args.Length < 2)
            {
                messageOutput.SendMessage("Usage: /tp player <PlayerName>");
                return;
            }
            
            string targetName = args[1];
            
            // 🔍 BUSCAR JOGADOR ONLINE
            var targetPlayer = WorldManager.Instance.GetCharacterByName(targetName);
            if (targetPlayer == null)
            {
                messageOutput.SendMessage($"❌ Player '{targetName}' not found or offline!");
                return;
            }
            
            // 🔍 VERIFICAR SE NÃO É ELE MESMO
            if (targetPlayer.Id == character.Id)
            {
                messageOutput.SendMessage("❌ You cannot teleport to yourself!");
                return;
            }
            
            // ⚡ EXECUTAR TELEPORTE
            var targetPos = targetPlayer.Transform.Local.Position;
            var targetZone = targetPlayer.Transform.ZoneId;
            
            ExecuteTeleport(character, targetPos.X, targetPos.Y, targetPos.Z, targetZone);
            
            // 📊 FEEDBACK
            messageOutput.SendMessage($"✅ Teleported to player {targetPlayer.Name} " +
                                    $"at ({targetPos.X:F1}, {targetPos.Y:F1}, " +
                                    $"{targetPos.Z:F1}) Zone:{targetZone}");
            
            // 📢 NOTIFICAR O JOGADOR ALVO (OPCIONAL)
            targetPlayer.SendMessage($"🎯 GM {character.Name} teleported to your location");
            
            // 📝 LOG
            Logger.LogInfo($"GM {character.Name} teleported to player {targetPlayer.Name}");
        }
        
        // 🔧 TELEPORTE PARA ZONA
        private void TeleportToZone(Character character, string[] args, 
                                  IMessageOutput messageOutput)
        {
            // 🎯 FORMATO: /tp zone <ZoneId> [X] [Y] [Z]
            
            if (args.Length < 2)
            {
                messageOutput.SendMessage("Usage: /tp zone <ZoneId> [X] [Y] [Z]");
                return;
            }
            
            if (!uint.TryParse(args[1], out uint zoneId))
            {
                messageOutput.SendMessage("❌ Invalid zone ID!");
                return;
            }
            
            // 🔍 VERIFICAR SE ZONA EXISTE
            var zone = WorldManager.Instance.GetZoneById(zoneId);
            if (zone == null)
            {
                messageOutput.SendMessage($"❌ Zone {zoneId} does not exist!");
                return;
            }
            
            // 🔍 COORDENADAS OPCIONAIS (usar spawn da zona se não especificadas)
            float x = zone.SpawnPosition.X;
            float y = zone.SpawnPosition.Y;
            float z = zone.SpawnPosition.Z;
            
            if (args.Length >= 5)
            {
                if (!float.TryParse(args[2], out x) ||
                    !float.TryParse(args[3], out y) ||
                    !float.TryParse(args[4], out z))
                {
                    messageOutput.SendMessage("❌ Invalid coordinates!");
                    return;
                }
            }
            
            // ⚡ EXECUTAR TELEPORTE
            ExecuteTeleport(character, x, y, z, zoneId);
            
            // 📊 FEEDBACK
            messageOutput.SendMessage($"✅ Teleported to zone {zone.Name} ({zoneId}) " +
                                    $"at ({x:F1}, {y:F1}, {z:F1})");
        }
        
        // 🔧 MÉTODO AUXILIAR - Executar teleporte
        private void ExecuteTeleport(Character character, float x, float y, float z, uint zoneId)
        {
            // 👶 ANALOGIA: É como mover um boneco de um lugar para outro
            // no tabuleiro de um jogo de RPG!
            
            // 1. Remover da zona atual
            character.CurrentZone?.RemoveCharacter(character);
            
            // 2. Atualizar posição
            character.Transform.Local.SetPosition(x, y, z);
            character.Transform.ZoneId = zoneId;
            
            // 3. Adicionar à nova zona
            var newZone = WorldManager.Instance.GetZoneById(zoneId);
            newZone?.AddCharacter(character);
            
            // 4. Atualizar visibilidade
            character.UpdateVisibility();
            
            // 5. Efeito visual de teleporte
            character.BroadcastPacket(new SCTeleportEffectPacket(character.ObjId), true);
        }
        
        // 🔧 VALIDAÇÃO DE DESTINO
        private bool IsValidDestination(float x, float y, float z, uint zoneId)
        {
            var zone = WorldManager.Instance.GetZoneById(zoneId);
            return zone?.IsValidPosition(x, y, z) ?? false;
        }
        
        // 🔧 MOSTRAR COMO USAR
        private void ShowUsage(IMessageOutput messageOutput)
        {
            messageOutput.SendMessage("🎯 Teleport Command Usage:");
            messageOutput.SendMessage("  /tp coord <X> <Y> <Z> [ZoneId] - Teleport to coordinates");
            messageOutput.SendMessage("  /tp player <PlayerName> - Teleport to player");
            messageOutput.SendMessage("  /tp zone <ZoneId> [X] [Y] [Z] - Teleport to zone");
            messageOutput.SendMessage("Examples:");
            messageOutput.SendMessage("  /tp coord 100 200 50");
            messageOutput.SendMessage("  /tp player JohnDoe");
            messageOutput.SendMessage("  /tp zone 1 500 600 100");
        }
    }
}
```

#### **🎮 COMANDO SUPER AVANÇADO: SPAWN DE ITENS**

```csharp
// 📁 Arquivo: AAEmu.Game/Core/Commands/SpawnItemCommand.cs

public class SpawnItemCommand : ICommand
{
    public string[] CommandNames { get; } = { "spawn", "give", "item" };
    public string Description { get; } = "Spawn items for yourself or other players";
    public byte RequiredLevel { get; } = 30; // GM level 30+
    
    public void Execute(Character character, string[] args, IMessageOutput messageOutput)
    {
        // 🎯 FORMATOS SUPORTADOS:
        // /spawn <ItemId> [Quantity] [Grade]
        // /spawn <ItemId> [Quantity] [Grade] <PlayerName>
        // /spawn search <ItemName>
        // /spawn info <ItemId>
        
        if (args.Length == 0)
        {
            ShowUsage(messageOutput);
            return;
        }
        
        switch (args[0].ToLower())
        {
            case "search":
                SearchItems(args, messageOutput);
                break;
                
            case "info":
                ShowItemInfo(args, messageOutput);
                break;
                
            default:
                SpawnItem(character, args, messageOutput);
                break;
        }
    }
    
    // 🔍 BUSCAR ITENS POR NOME
    private void SearchItems(string[] args, IMessageOutput messageOutput)
    {
        if (args.Length < 2)
        {
            messageOutput.SendMessage("Usage: /spawn search <ItemName>");
            return;
        }
        
        string searchTerm = string.Join(" ", args.Skip(1));
        var items = ItemManager.Instance.SearchItemsByName(searchTerm);
        
        if (!items.Any())
        {
            messageOutput.SendMessage($"❌ No items found matching '{searchTerm}'");
            return;
        }
        
        messageOutput.SendMessage($"🔍 Found {items.Count} items matching '{searchTerm}':");
        
        foreach (var item in items.Take(10)) // Mostrar apenas 10 primeiros
        {
            messageOutput.SendMessage($"  📦 {item.Id}: {item.Name} (Grade: {item.Grade})");
        }
        
        if (items.Count > 10)
        {
            messageOutput.SendMessage($"... and {items.Count - 10} more items");
        }
    }
    
    // 📊 MOSTRAR INFORMAÇÕES DO ITEM
    private void ShowItemInfo(string[] args, IMessageOutput messageOutput)
    {
        if (args.Length < 2 || !uint.TryParse(args[1], out uint itemId))
        {
            messageOutput.SendMessage("Usage: /spawn info <ItemId>");
            return;
        }
        
        var itemTemplate = ItemManager.Instance.GetTemplate(itemId);
        if (itemTemplate == null)
        {
            messageOutput.SendMessage($"❌ Item {itemId} not found!");
            return;
        }
        
        // 📊 INFORMAÇÕES DETALHADAS
        messageOutput.SendMessage($"📦 Item Information:");
        messageOutput.SendMessage($"  ID: {itemTemplate.Id}");
        messageOutput.SendMessage($"  Name: {itemTemplate.Name}");
        messageOutput.SendMessage($"  Grade: {itemTemplate.Grade}");
        messageOutput.SendMessage($"  Type: {itemTemplate.ItemType}");
        messageOutput.SendMessage($"  Level: {itemTemplate.Level}");
        messageOutput.SendMessage($"  Max Stack: {itemTemplate.MaxCount}");
        messageOutput.SendMessage($"  Sellable: {(itemTemplate.Sellable ? "Yes" : "No")}");
        messageOutput.SendMessage($"  Description: {itemTemplate.Description}");
    }
    
    // ⚡ SPAWNAR ITEM
    private void SpawnItem(Character character, string[] args, IMessageOutput messageOutput)
    {
        // 🔍 VALIDAR ARGUMENTOS
        if (!uint.TryParse(args[0], out uint itemId))
        {
            messageOutput.SendMessage("❌ Invalid item ID!");
            return;
        }
        
        // 🔍 QUANTIDADE (padrão: 1)
        int quantity = 1;
        if (args.Length > 1 && !int.TryParse(args[1], out quantity))
        {
            messageOutput.SendMessage("❌ Invalid quantity!");
            return;
        }
        
        // 🔍 GRADE (padrão: 0)
        byte grade = 0;
        if (args.Length > 2 && !byte.TryParse(args[2], out grade))
        {
            messageOutput.SendMessage("❌ Invalid grade!");
            return;
        }
        
        // 🔍 JOGADOR ALVO (padrão: próprio GM)
        Character targetPlayer = character;
        if (args.Length > 3)
        {
            string targetName = args[3];
            targetPlayer = WorldManager.Instance.GetCharacterByName(targetName);
            if (targetPlayer == null)
            {
                messageOutput.SendMessage($"❌ Player '{targetName}' not found!");
                return;
            }
        }
        
        // 🔍 VERIFICAR SE ITEM EXISTE
        var itemTemplate = ItemManager.Instance.GetTemplate(itemId);
        if (itemTemplate == null)
        {
            messageOutput.SendMessage($"❌ Item {itemId} does not exist!");
            return;
        }
        
        // 🔍 VALIDAR QUANTIDADE
        if (quantity <= 0 || quantity > 1000)
        {
            messageOutput.SendMessage("❌ Quantity must be between 1 and 1000!");
            return;
        }
        
        // 🔍 VALIDAR GRADE
        if (grade > itemTemplate.MaxGrade)
        {
            messageOutput.SendMessage($"❌ Maximum grade for this item is {itemTemplate.MaxGrade}!");
            return;
        }
        
        // ⚡ CRIAR E DAR O ITEM
        try
        {
            var success = targetPlayer.Inventory.AddItem(itemId, quantity, grade);
            
            if (success)
            {
                // 📊 FEEDBACK DE SUCESSO
                string gradeText = grade > 0 ? $" (Grade {grade})" : "";
                
                if (targetPlayer.Id == character.Id)
                {
                    messageOutput.SendMessage($"✅ Spawned {quantity}x {itemTemplate.Name}{gradeText}");
                }
                else
                {
                    messageOutput.SendMessage($"✅ Gave {quantity}x {itemTemplate.Name}{gradeText} " +
                                            $"to {targetPlayer.Name}");
                    
                    // 📢 NOTIFICAR O JOGADOR
                    targetPlayer.SendMessage($"🎁 You received {quantity}x {itemTemplate.Name}{gradeText} " +
                                           $"from GM {character.Name}");
                }
                
                // 📝 LOG DA AÇÃO
                Logger.LogInfo($"GM {character.Name} spawned {quantity}x {itemTemplate.Name} " +
                              $"(ID:{itemId}, Grade:{grade}) for {targetPlayer.Name}");
            }
            else
            {
                messageOutput.SendMessage("❌ Failed to add item to inventory (full?)");
            }
        }
        catch (Exception ex)
        {
            messageOutput.SendMessage($"❌ Error spawning item: {ex.Message}");
            Logger.LogError($"Error in SpawnItemCommand: {ex}");
        }
    }
    
    // 🔧 MOSTRAR COMO USAR
    private void ShowUsage(IMessageOutput messageOutput)
    {
        messageOutput.SendMessage("🎁 Spawn Item Command Usage:");
        messageOutput.SendMessage("  /spawn <ItemId> [Quantity] [Grade] [PlayerName]");
        messageOutput.SendMessage("  /spawn search <ItemName> - Search for items");
        messageOutput.SendMessage("  /spawn info <ItemId> - Show item information");
        messageOutput.SendMessage("Examples:");
        messageOutput.SendMessage("  /spawn 1001 10 - Give yourself 10x item 1001");
        messageOutput.SendMessage("  /spawn 1001 5 3 JohnDoe - Give JohnDoe 5x item 1001 grade 3");
        messageOutput.SendMessage("  /spawn search sword - Search for items with 'sword' in name");
        messageOutput.SendMessage("  /spawn info 1001 - Show information about item 1001");
    }
}
```

---

## 🗄️ **CAPÍTULO 3: DATABASE DESIGN AVANÇADO**

### **📊 CRIANDO TABELAS PERSONALIZADAS**

**👶 ANALOGIA**: O banco de dados é como uma **BIBLIOTECA GIGANTE** com milhões de gavetas organizadas. Cada tabela é uma seção (livros, revistas, DVDs) e cada registro é um item específico naquela gaveta! 📚🗃️

#### **🔧 CRIANDO SISTEMA DE GUILDS PERSONALIZADO**

```sql
-- 📁 Arquivo: SQL/CustomFeatures/guild_system.sql

-- 🏰 TABELA PRINCIPAL DE GUILDS
CREATE TABLE IF NOT EXISTS custom_guilds (
    -- 🔑 CHAVE PRIMÁRIA
    guild_id INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    
    -- 📝 INFORMAÇÕES BÁSICAS
    guild_name VARCHAR(50) NOT NULL UNIQUE,
    guild_tag VARCHAR(6) NOT NULL UNIQUE,
    guild_description TEXT,
    
    -- 👑 LIDERANÇA
    leader_id INT UNSIGNED NOT NULL,
    
    -- 📊 ESTATÍSTICAS
    member_count INT UNSIGNED DEFAULT 0,
    max_members INT UNSIGNED DEFAULT 50,
    guild_level TINYINT UNSIGNED DEFAULT 1,
    guild_exp INT UNSIGNED DEFAULT 0,
    
    -- 💰 RECURSOS
    guild_gold BIGINT UNSIGNED DEFAULT 0,
    guild_points INT UNSIGNED DEFAULT 0,
    
    -- 🎨 CUSTOMIZAÇÃO
    guild_emblem_id INT UNSIGNED DEFAULT 0,
    guild_color_primary INT UNSIGNED DEFAULT 0xFFFFFF,
    guild_color_secondary INT UNSIGNED DEFAULT 0x000000,
    
    -- ⚙️ CONFIGURAÇÕES
    guild_type ENUM('PvE', 'PvP', 'Mixed', 'Social') DEFAULT 'Mixed',
    recruitment_open BOOLEAN DEFAULT TRUE,
    guild_message VARCHAR(200) DEFAULT '',
    
    -- 📅 TIMESTAMPS
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    -- 🔍 ÍNDICES PARA PERFORMANCE
    INDEX idx_guild_name (guild_name),
    INDEX idx_guild_tag (guild_tag),
    INDEX idx_leader_id (leader_id),
    INDEX idx_guild_level (guild_level),
    INDEX idx_created_at (created_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 👥 TABELA DE MEMBROS DA GUILD
CREATE TABLE IF NOT EXISTS custom_guild_members (
    -- 🔑 CHAVE PRIMÁRIA COMPOSTA
    guild_id INT UNSIGNED NOT NULL,
    character_id INT UNSIGNED NOT NULL,
    
    -- 🎖️ RANK E PERMISSÕES
    guild_rank ENUM('Leader', 'Officer', 'Veteran', 'Member', 'Recruit') DEFAULT 'Member',
    rank_custom_name VARCHAR(30) DEFAULT '',
    
    -- 📊 CONTRIBUIÇÕES
    contribution_points INT UNSIGNED DEFAULT 0,
    total_donated_gold BIGINT UNSIGNED DEFAULT 0,
    
    -- 📅 ATIVIDADE
    joined_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    last_online TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    last_contribution TIMESTAMP NULL,
    
    -- 📝 NOTAS
    member_note VARCHAR(100) DEFAULT '',
    officer_note VARCHAR(100) DEFAULT '',
    
    -- 🔑 CHAVES
    PRIMARY KEY (guild_id, character_id),
    
    -- 🔗 FOREIGN KEYS
    FOREIGN KEY (guild_id) REFERENCES custom_guilds(guild_id) 
        ON DELETE CASCADE ON UPDATE CASCADE,
    
    -- 🔍 ÍNDICES
    INDEX idx_character_id (character_id),
    INDEX idx_guild_rank (guild_rank),
    INDEX idx_contribution (contribution_points DESC),
    INDEX idx_joined_at (joined_at),
    INDEX idx_last_online (last_online)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 🏛️ TABELA DE BUILDINGS DA GUILD
CREATE TABLE IF NOT EXISTS custom_guild_buildings (
    -- 🔑 CHAVE PRIMÁRIA
    building_id INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    
    -- 🔗 RELAÇÃO COM GUILD
    guild_id INT UNSIGNED NOT NULL,
    
    -- 🏗️ TIPO E INFORMAÇÕES
    building_type ENUM('Hall', 'Warehouse', 'Workshop', 'Barracks', 'Tower', 'Farm') NOT NULL,
    building_name VARCHAR(50) NOT NULL,
    building_level TINYINT UNSIGNED DEFAULT 1,
    
    -- 📍 LOCALIZAÇÃO
    zone_id INT UNSIGNED NOT NULL,
    position_x FLOAT NOT NULL,
    position_y FLOAT NOT NULL,
    position_z FLOAT NOT NULL,
    rotation FLOAT DEFAULT 0,
    
    -- 📊 STATUS
    building_status ENUM('Planning', 'Construction', 'Active', 'Upgrading', 'Damaged') DEFAULT 'Planning',
    construction_progress FLOAT DEFAULT 0.0, -- 0.0 a 100.0
    
    -- 💰 CUSTOS E RECURSOS
    construction_cost_gold BIGINT UNSIGNED DEFAULT 0,
    construction_cost_materials JSON, -- {"item_id": quantity, ...}
    maintenance_cost_daily INT UNSIGNED DEFAULT 0,
    
    -- 📅 TIMESTAMPS
    construction_started TIMESTAMP NULL,
    construction_completed TIMESTAMP NULL,
    last_maintenance TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    -- 🔗 FOREIGN KEY
    FOREIGN KEY (guild_id) REFERENCES custom_guilds(guild_id) 
        ON DELETE CASCADE ON UPDATE CASCADE,
    
    -- 🔍 ÍNDICES
    INDEX idx_guild_id (guild_id),
    INDEX idx_building_type (building_type),
    INDEX idx_zone_position (zone_id, position_x, position_y),
    INDEX idx_status (building_status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ⚔️ TABELA DE GUERRAS ENTRE GUILDS
CREATE TABLE IF NOT EXISTS custom_guild_wars (
    -- 🔑 CHAVE PRIMÁRIA
    war_id INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    
    -- 🔗 GUILDS PARTICIPANTES
    attacker_guild_id INT UNSIGNED NOT NULL,
    defender_guild_id INT UNSIGNED NOT NULL,
    
    -- 📊 STATUS DA GUERRA
    war_status ENUM('Declared', 'Active', 'Ended', 'Cancelled') DEFAULT 'Declared',
    war_type ENUM('Conquest', 'Siege', 'Skirmish') DEFAULT 'Skirmish',
    
    -- ⏰ TEMPO
    declared_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    starts_at TIMESTAMP NOT NULL,
    ends_at TIMESTAMP NULL,
    duration_hours INT UNSIGNED DEFAULT 24,
    
    -- 🎯 OBJETIVOS
    war_objective VARCHAR(200),
    victory_condition ENUM('Kills', 'Territory', 'Resources', 'Time') DEFAULT 'Kills',
    target_value INT UNSIGNED DEFAULT 100,
    
    -- 📊 PONTUAÇÃO
    attacker_score INT UNSIGNED DEFAULT 0,
    defender_score INT UNSIGNED DEFAULT 0,
    
    -- 🏆 RESULTADO
    winner_guild_id INT UNSIGNED NULL,
    war_result ENUM('Attacker_Victory', 'Defender_Victory', 'Draw', 'Cancelled') NULL,
    
    -- 💰 APOSTAS E RECOMPENSAS
    stake_gold BIGINT UNSIGNED DEFAULT 0,
    stake_items JSON, -- {"item_id": quantity, ...}
    
    -- 📝 NOTAS
    war_declaration_msg VARCHAR(300),
    war_end_reason VARCHAR(200),
    
    -- 🔗 FOREIGN KEYS
    FOREIGN KEY (attacker_guild_id) REFERENCES custom_guilds(guild_id) 
        ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (defender_guild_id) REFERENCES custom_guilds(guild_id) 
        ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (winner_guild_id) REFERENCES custom_guilds(guild_id) 
        ON DELETE SET NULL ON UPDATE CASCADE,
    
    -- 🔍 ÍNDICES
    INDEX idx_attacker (attacker_guild_id),
    INDEX idx_defender (defender_guild_id),
    INDEX idx_war_status (war_status),
    INDEX idx_declared_at (declared_at),
    INDEX idx_starts_at (starts_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 📈 TABELA DE EVENTOS DA GUILD (LOG)
CREATE TABLE IF NOT EXISTS custom_guild_events (
    -- 🔑 CHAVE PRIMÁRIA
    event_id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    
    -- 🔗 RELAÇÃO
    guild_id INT UNSIGNED NOT NULL,
    character_id INT UNSIGNED NULL, -- NULL para eventos automáticos
    
    -- 📝 INFORMAÇÕES DO EVENTO
    event_type ENUM('Join', 'Leave', 'Kick', 'Promote', 'Demote', 'Donate', 
                   'Build', 'War_Declare', 'War_End', 'Level_Up', 'Message') NOT NULL,
    event_description VARCHAR(300) NOT NULL,
    
    -- 📊 DADOS EXTRAS (JSON para flexibilidade)
    event_data JSON, -- Dados específicos do evento
    
    -- 📅 TIMESTAMP
    occurred_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    -- 🔗 FOREIGN KEY
    FOREIGN KEY (guild_id) REFERENCES custom_guilds(guild_id) 
        ON DELETE CASCADE ON UPDATE CASCADE,
    
    -- 🔍 ÍNDICES
    INDEX idx_guild_id (guild_id),
    INDEX idx_event_type (event_type),
    INDEX idx_occurred_at (occurred_at),
    INDEX idx_character_id (character_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 🎖️ TABELA DE RANKS CUSTOMIZÁVEIS
CREATE TABLE IF NOT EXISTS custom_guild_ranks (
    -- 🔑 CHAVE PRIMÁRIA
    rank_id INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    
    -- 🔗 GUILD
    guild_id INT UNSIGNED NOT NULL,
    
    -- 📝 INFORMAÇÕES DO RANK
    rank_name VARCHAR(30) NOT NULL,
    rank_level TINYINT UNSIGNED NOT NULL, -- 1 = mais alto, 10 = mais baixo
    rank_color INT UNSIGNED DEFAULT 0xFFFFFF,
    
    -- 🔐 PERMISSÕES (bitwise flags)
    permissions INT UNSIGNED DEFAULT 0,
    -- Bit 0: Invite members
    -- Bit 1: Kick members  
    -- Bit 2: Promote/demote
    -- Bit 3: Access warehouse
    -- Bit 4: Withdraw gold
    -- Bit 5: Declare war
    -- Bit 6: Build structures
    -- Bit 7: Edit guild info
    -- etc...
    
    -- 📅 TIMESTAMPS
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    -- 🔗 FOREIGN KEY
    FOREIGN KEY (guild_id) REFERENCES custom_guilds(guild_id) 
        ON DELETE CASCADE ON UPDATE CASCADE,
    
    -- 🔍 ÍNDICES E CONSTRAINTS
    UNIQUE KEY unique_guild_rank_level (guild_id, rank_level),
    INDEX idx_guild_id (guild_id),
    INDEX idx_rank_level (rank_level)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

#### **🔧 STORED PROCEDURES PARA GUILDS**

```sql
-- 📁 Arquivo: SQL/CustomFeatures/guild_procedures.sql

-- 🏰 PROCEDURE: CRIAR NOVA GUILD
DELIMITER $$

CREATE PROCEDURE CreateGuild(
    IN p_guild_name VARCHAR(50),
    IN p_guild_tag VARCHAR(6),
    IN p_leader_id INT UNSIGNED,
    IN p_guild_description TEXT,
    OUT p_guild_id INT UNSIGNED,
    OUT p_result_code INT,
    OUT p_result_message VARCHAR(200)
)
BEGIN
    DECLARE v_existing_guild INT DEFAULT 0;
    DECLARE v_existing_tag INT DEFAULT 0;
    DECLARE v_leader_has_guild INT DEFAULT 0;
    
    -- 🔍 VALIDAÇÕES
    
    -- Verificar se nome já existe
    SELECT COUNT(*) INTO v_existing_guild 
    FROM custom_guilds 
    WHERE guild_name = p_guild_name;
    
    IF v_existing_guild > 0 THEN
        SET p_result_code = 1;
        SET p_result_message = 'Guild name already exists';
        SET p_guild_id = 0;
        LEAVE;
    END IF;
    
    -- Verificar se tag já existe
    SELECT COUNT(*) INTO v_existing_tag 
    FROM custom_guilds 
    WHERE guild_tag = p_guild_tag;
    
    IF v_existing_tag > 0 THEN
        SET p_result_code = 2;
        SET p_result_message = 'Guild tag already exists';
        SET p_guild_id = 0;
        LEAVE;
    END IF;
    
    -- Verificar se líder já tem guild
    SELECT COUNT(*) INTO v_leader_has_guild 
    FROM custom_guild_members 
    WHERE character_id = p_leader_id;
    
    IF v_leader_has_guild > 0 THEN
        SET p_result_code = 3;
        SET p_result_message = 'Leader already belongs to a guild';
        SET p_guild_id = 0;
        LEAVE;
    END IF;
    
    -- ⚡ CRIAR GUILD
    START TRANSACTION;
    
    INSERT INTO custom_guilds (
        guild_name, guild_tag, leader_id, guild_description, member_count
    ) VALUES (
        p_guild_name, p_guild_tag, p_leader_id, p_guild_description, 1
    );
    
    SET p_guild_id = LAST_INSERT_ID();
    
    -- Adicionar líder como membro
    INSERT INTO custom_guild_members (
        guild_id, character_id, guild_rank, joined_at
    ) VALUES (
        p_guild_id, p_leader_id, 'Leader', NOW()
    );
    
    -- Criar ranks padrão
    CALL CreateDefaultGuildRanks(p_guild_id);
    
    -- Log do evento
    INSERT INTO custom_guild_events (
        guild_id, character_id, event_type, event_description
    ) VALUES (
        p_guild_id, p_leader_id, 'Join', 
        CONCAT('Guild created by ', p_leader_id)
    );
    
    COMMIT;
    
    SET p_result_code = 0;
    SET p_result_message = 'Guild created successfully';
    
END$$

-- 👥 PROCEDURE: ADICIONAR MEMBRO
CREATE PROCEDURE AddGuildMember(
    IN p_guild_id INT UNSIGNED,
    IN p_character_id INT UNSIGNED,
    IN p_inviter_id INT UNSIGNED,
    OUT p_result_code INT,
    OUT p_result_message VARCHAR(200)
)
BEGIN
    DECLARE v_guild_exists INT DEFAULT 0;
    DECLARE v_member_count INT DEFAULT 0;
    DECLARE v_max_members INT DEFAULT 0;
    DECLARE v_already_member INT DEFAULT 0;
    DECLARE v_inviter_can_invite INT DEFAULT 0;
    
    -- 🔍 VALIDAÇÕES
    
    -- Verificar se guild existe
    SELECT COUNT(*), max_members INTO v_guild_exists, v_max_members
    FROM custom_guilds 
    WHERE guild_id = p_guild_id;
    
    IF v_guild_exists = 0 THEN
        SET p_result_code = 1;
        SET p_result_message = 'Guild does not exist';
        LEAVE;
    END IF;
    
    -- Verificar se já é membro de alguma guild
    SELECT COUNT(*) INTO v_already_member
    FROM custom_guild_members 
    WHERE character_id = p_character_id;
    
    IF v_already_member > 0 THEN
        SET p_result_code = 2;
        SET p_result_message = 'Character already belongs to a guild';
        LEAVE;
    END IF;
    
    -- Verificar limite de membros
    SELECT member_count INTO v_member_count
    FROM custom_guilds 
    WHERE guild_id = p_guild_id;
    
    IF v_member_count >= v_max_members THEN
        SET p_result_code = 3;
        SET p_result_message = 'Guild is full';
        LEAVE;
    END IF;
    
    -- Verificar se quem convida tem permissão
    SELECT COUNT(*) INTO v_inviter_can_invite
    FROM custom_guild_members gm
    JOIN custom_guild_ranks gr ON (gm.guild_id = gr.guild_id)
    WHERE gm.guild_id = p_guild_id 
      AND gm.character_id = p_inviter_id
      AND (gr.permissions & 1) > 0; -- Bit 0 = invite permission
    
    IF v_inviter_can_invite = 0 THEN
        SET p_result_code = 4;
        SET p_result_message = 'Inviter does not have permission';
        LEAVE;
    END IF;
    
    -- ⚡ ADICIONAR MEMBRO
    START TRANSACTION;
    
    INSERT INTO custom_guild_members (
        guild_id, character_id, guild_rank, joined_at
    ) VALUES (
        p_guild_id, p_character_id, 'Member', NOW()
    );
    
    -- Atualizar contador
    UPDATE custom_guilds 
    SET member_count = member_count + 1 
    WHERE guild_id = p_guild_id;
    
    -- Log do evento
    INSERT INTO custom_guild_events (
        guild_id, character_id, event_type, event_description
    ) VALUES (
        p_guild_id, p_character_id, 'Join', 
        CONCAT('Invited by character ID ', p_inviter_id)
    );
    
    COMMIT;
    
    SET p_result_code = 0;
    SET p_result_message = 'Member added successfully';
    
END$$

DELIMITER ;
```

---

## 🎯 **RESUMO DO MÓDULO 4 - VOCÊ AGORA É UM DESENVOLVEDOR DE ELITE!**

### **🏆 HABILIDADES DE DESENVOLVIMENTO CONQUISTADAS:**

✅ **Packet Handlers**: Criação de handlers personalizados para comunicação cliente-servidor  
✅ **Comandos GM**: Sistema completo de comandos administrativos avançados  
✅ **Database Design**: Criação de tabelas complexas com relacionamentos  
✅ **Stored Procedures**: Lógica de negócio avançada no banco de dados  
✅ **Validação de Dados**: Sistemas robustos de validação e segurança  
✅ **Logging Avançado**: Sistema de auditoria e rastreamento de ações  
✅ **Error Handling**: Tratamento profissional de erros e exceções  
✅ **Performance**: Otimizações de consultas e índices de banco  

### **💎 FEATURES ÉPICAS CRIADAS:**

🎮 **Sistema de Teleporte**: Teleporte instantâneo com validações avançadas  
⚡ **Comandos GM Avançados**: Teleporte, spawn de itens, busca inteligente  
🏰 **Sistema de Guilds**: Guild completa com ranks, permissões, guerras  
🗄️ **Database Profissional**: Estrutura normalizada com procedures  
🔐 **Sistema de Permissões**: Controle granular de acesso por bits  
📊 **Sistema de Logs**: Auditoria completa de todas as ações  

### **🚀 PRÓXIMO MÓDULO 5: NETWORK PROGRAMMING E PERFORMANCE**

No **Módulo 5** vamos dominar:

🌐 **Criação de Packets Customizados**  
⚡ **Otimização de Performance**  
🔒 **Segurança e Anti-Cheat**  
🧪 **Testing e Quality Assurance**  
📊 **Monitoring e Profiling**  
🐳 **Deployment e Docker**  

**Continue para se tornar um MESTRE ABSOLUTO em emuladores!** 🎓⚡