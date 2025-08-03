# 🎮 **Comandos de GM - AAEmu Server**

## 🔧 **O que é o comando `/feature`?**

O comando `/feature` controla **recursos/funcionalidades** do servidor que podem ser ligados ou desligados. É como um "painel de controle" para ativar/desativar sistemas específicos do jogo.

### **Uso do /feature:**
```bash
/feature check                    # Ver status de todas as features
/feature set <id> <true/false>    # Ativar/desativar feature específica
```

### **Features Principais (IDs importantes):**
- **0 - siege**: Sistema de cerco
- **1 - allowFamilyChanges**: Permitir mudanças de família
- **3 - houseSale**: Venda de casas
- **4 - premium**: Sistema premium
- **6 - nations**: Sistema de nações
- **36 - ranking**: Sistema de ranking
- **38 - ingamecashshop**: Loja de cash no jogo
- **44 - aaPoint**: Sistema de AA Points
- **45 - itemSecure**: Segurança de itens
- **52 - beautyshopBypass**: Bypass na loja de beleza
- **74 - dwarfWarborn**: Raças Anão e Warborn
- **81 - hudAuctionButton**: Botão de leilão na HUD

---

## 📋 **Lista Completa de Comandos GM**

### **🎮 COMANDOS BÁSICOS**

#### **`/help`**
- **Uso**: `/help [comando]`
- **Função**: Mostra ajuda geral ou específica de um comando
- **Exemplo**: `/help teleport`

#### **`/echo <texto>`**
- **Uso**: `/echo <mensagem>`
- **Função**: Repete a mensagem (teste de comandos)
- **Exemplo**: `/echo Servidor funcionando!`

---

### **👤 GERENCIAMENTO DE PERSONAGEM**

#### **`/heal [target]`**
- **Uso**: `/heal` ou `/heal <player>`
- **Função**: Cura HP/MP completo
- **Exemplo**: `/heal João`

#### **`/revive [target]`**
- **Uso**: `/revive` ou `/revive <player>`
- **Função**: Revive personagem morto
- **Exemplo**: `/revive Maria`

#### **`/invisible`**
- **Uso**: `/invisible`
- **Função**: Torna invisível/visível (toggle)

#### **`/godmode`**
- **Uso**: `/godmode`
- **Função**: Ativa/desativa modo deus (invencível)

#### **`/fly`**
- **Uso**: `/fly`
- **Função**: Ativa/desativa modo voo

---

### **💰 ECONOMIA E ITENS**

#### **`/gold <amount> [target]`**
- **Uso**: `/gold 1000` ou `/gold 1000 João`
- **Função**: Adiciona ouro ao jogador
- **Exemplo**: `/gold 50000 Maria`

#### **`/item add <id> [quantidade] [grade]`**
- **Uso**: `/item add 1234 5 3`
- **Função**: Adiciona item ao inventário
- **Exemplo**: `/item add 1001 1 5` (Espada +5)

#### **`/item search <nome>`**
- **Uso**: `/item search sword`
- **Função**: Busca itens por nome

#### **`/item grade <slot> <grade>`**
- **Uso**: `/item grade 1 7`
- **Função**: Muda grade do item equipado

#### **`/kit <nome>`**
- **Uso**: `/kit starter`
- **Função**: Dá kit pré-definido de itens
- **Kits**: `starter`, `warrior`, `mage`, `archer`, `test`

---

### **🌍 MOVIMENTO E TELEPORTE**

#### **`/teleport <x> <y> <z> [world]`**
- **Uso**: `/teleport 1000 1000 100`
- **Função**: Teleporta para coordenadas específicas
- **Exemplo**: `/teleport 1000 1000 100 1`

#### **`/teleport <player>`**
- **Uso**: `/teleport João`
- **Função**: Teleporta para outro jogador

#### **`/move <distance>`**
- **Uso**: `/move 10`
- **Função**: Move para frente X metros

#### **`/moveto <player>`**
- **Uso**: `/moveto Maria`
- **Função**: Teleporta jogador para você

#### **`/moveall`**
- **Uso**: `/moveall`
- **Função**: Teleporta todos os jogadores para você

#### **`/position` ou `/pos`**
- **Uso**: `/position`
- **Função**: Mostra sua posição atual (X, Y, Z)

---

### **👥 GERENCIAMENTO DE JOGADORES**

#### **`/kick <player> [motivo]`**
- **Uso**: `/kick João Violação de regras`
- **Função**: Expulsa jogador do servidor

#### **`/online`**
- **Uso**: `/online`
- **Função**: Lista jogadores online

#### **`/setfaction <player> <factionId>`**
- **Uso**: `/setfaction João 1`
- **Função**: Muda facção do jogador

#### **`/title <id>` ou `/appellation <id>`**
- **Uso**: `/title 123`
- **Função**: Adiciona título ao personagem

#### **`/changelevel <level> [target]`**
- **Uso**: `/changelevel 55 João`
- **Função**: Muda level do jogador

#### **`/addxp <amount> [target]`**
- **Uso**: `/addxp 100000`
- **Função**: Adiciona experiência

#### **`/addlabor <amount> [target]`**
- **Uso**: `/addlabor 5000`
- **Função**: Adiciona pontos de labor

---

### **🏠 HOUSING E CONSTRUÇÃO**

#### **`/house build <templateId> [x] [y] [z]`**
- **Uso**: `/house build 1234`
- **Função**: Constrói casa no local atual

#### **`/house demo <houseId>`**
- **Uso**: `/house demo 5678`
- **Função**: Demolish casa específica

#### **`/house info`**
- **Uso**: `/house info`
- **Função**: Informações da casa mais próxima

---

### **👹 NPCs E SPAWNS**

#### **`/spawn npc <id> [quantidade]`**
- **Uso**: `/spawn npc 1234`
- **Função**: Spawna NPC no local atual

#### **`/spawn doodad <id>`**
- **Uso**: `/spawn doodad 5678`
- **Função**: Spawna objeto/doodad

#### **`/despawn <objId>`**
- **Uso**: `/despawn 12345`
- **Função**: Remove objeto específico

#### **`/despawnall [tipo]`**
- **Uso**: `/despawnall npc`
- **Função**: Remove todos NPCs da área

#### **`/npc info <objId>`**
- **Uso**: `/npc info 12345`
- **Função**: Informações detalhadas do NPC

---

### **⚔️ COMBATE E HABILIDADES**

#### **`/addbuff <id> [target]`**
- **Uso**: `/addbuff 123 João`
- **Função**: Adiciona buff ao jogador

#### **`/useskill <id> [target]`**
- **Uso**: `/useskill 456`
- **Função**: Usa habilidade específica

#### **`/damage <amount> [target]`**
- **Uso**: `/damage 500 João`
- **Função**: Causa dano específico

#### **`/kill [target]`**
- **Uso**: `/kill João`
- **Função**: Mata jogador instantaneamente

#### **`/clearcombat`**
- **Uso**: `/clearcombat`
- **Função**: Remove estado de combate

#### **`/resetcooldowns`**
- **Uso**: `/resetcooldowns`
- **Função**: Reseta cooldowns de todas habilidades

#### **`/ignorecooldowns`**
- **Uso**: `/ignorecooldowns`
- **Função**: Ignora cooldowns (toggle)

---

### **🌊 WORLD EDITING**

#### **`/wateredit add <x> <y>`**
- **Uso**: `/wateredit add 1000 1000`
- **Função**: Adiciona ponto de água

#### **`/wateredit remove <x> <y>`**
- **Uso**: `/wateredit remove 1000 1000`
- **Função**: Remove ponto de água

#### **`/height [value]`**
- **Uso**: `/height 100`
- **Função**: Mostra/define altura do terreno

#### **`/snow <intensity>`**
- **Uso**: `/snow 5`
- **Função**: Ativa efeito de neve

---

### **🔧 SISTEMA E DEBUG**

#### **`/reloadconfig`**
- **Uso**: `/reloadconfig`
- **Função**: Recarrega configurações do servidor

#### **`/scripts reload`**
- **Uso**: `/scripts reload`
- **Função**: Recarrega scripts lua/C#

#### **`/around <distance>`**
- **Uso**: `/around 50`
- **Função**: Lista objetos em volta (raio em metros)

#### **`/findobject <termo>`**
- **Uso**: `/findobject sword`
- **Função**: Procura objetos por nome/ID

#### **`/getattribute <nome>`**
- **Uso**: `/getattribute hp`
- **Função**: Mostra valor de atributo específico

#### **`/showinventory [target]`**
- **Uso**: `/showinventory João`
- **Função**: Mostra inventário do jogador

---

### **📢 COMUNICAÇÃO**

#### **`/announce <tipo> <mensagem>`**
- **Uso**: `/announce 1 Manutenção em 10 minutos!`
- **Função**: Anuncia mensagem para todos
- **Tipos**: 1=Amarelo, 2=Vermelho, 3=Verde, 4=Azul

#### **`/testchat <canal> <mensagem>`**
- **Uso**: `/testchat 1 Teste`
- **Função**: Testa canais de chat

---

### **🎯 COMANDOS ESPECIAIS**

#### **`/setlang <idioma>` ou `/language <idioma>`**
- **Uso**: `/setlang pt_br`
- **Função**: Muda idioma do personagem
- **Idiomas**: `pt_br`, `en_us`, `es_es`, `fr_fr`, etc.

#### **`/soloparty`**
- **Uso**: `/soloparty`
- **Função**: Cria party solo (para testar raid features)

#### **`/time [hora]`**
- **Uso**: `/time 12`
- **Função**: Muda/mostra hora do servidor

#### **`/pingposition`**
- **Uso**: `/pingposition`
- **Função**: Marca posição no mapa para o grupo

---

## 🔐 **Níveis de Acesso**

### **Níveis de GM (Access Level):**
- **0**: Jogador normal
- **10**: Moderador básico
- **50**: GM Júnior
- **100**: GM Sênior
- **255**: Administrador

### **Comandos por Nível:**
- **Level 0**: `/position`, `/echo`
- **Level 10**: `/heal`, `/teleport` (próprio)
- **Level 50**: `/spawn`, `/item`, `/gold`
- **Level 100**: `/kick`, `/setfaction`, `/feature`
- **Level 255**: Todos os comandos

---

## 💡 **Dicas de Uso**

### **Comandos Mais Úteis para GMs:**
1. **`/around 100`** - Ver o que há por perto
2. **`/teleport player`** - Ir até jogador
3. **`/heal`** - Curar-se rapidamente
4. **`/item add 1001 1 5`** - Item básico para teste
5. **`/feature check`** - Ver status do servidor

### **Debugging Common Issues:**
```bash
/around 50           # Ver objetos próximos
/findobject npc      # Encontrar NPCs com problemas
/showinventory       # Verificar inventário de player
/getattribute hp     # Verificar HP atual
/position            # Saber localização exata
```

### **Setup Rápido para Teste:**
```bash
/godmode             # Ficar invencível
/fly                 # Voar livremente
/kit test            # Kit básico de itens
/changelevel 55      # Level máximo
/ignorecooldowns     # Sem cooldowns
```

---

## ⚠️ **Cuidados Importantes**

1. **Backup**: Sempre faça backup antes de usar comandos de world editing
2. **Players Online**: Evite comandos pesados com muitos players online
3. **Features**: Mudanças no `/feature` podem exigir reconexão
4. **Coordenadas**: Verifique coordenadas antes de usar `/teleport`
5. **IDs**: Confirme IDs de itens/NPCs antes de spawnar

---

## 📝 **Logs e Monitoramento**

Todos os comandos GM são logados automaticamente. Monitore:
- **Logs do servidor**: Para erros de comandos
- **Chat logs**: Para comunicação GM
- **Database logs**: Para mudanças permanentes

Use `/echo` para testar se comandos estão funcionando antes de executar comandos importantes!

---

**📋 Resumo**: O AAEmu possui mais de 80 comandos GM diferentes, desde básicos como `/heal` até avançados como `/wateredit`. O comando `/feature` é especial pois controla funcionalidades globais do servidor que afetam todos os jogadores!