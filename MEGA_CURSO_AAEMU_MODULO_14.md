# 🎓 MEGA CURSO ULTRA DETALHADO: MÓDULO 14
## 💰 SISTEMA DE ECONOMIA E TRADE - MERCADO VIRTUAL COMPLETO

---

## 🎯 **BEM-VINDO AO MÓDULO 14!**

Agora vamos mergulhar no coração econômico do AAEmu: **SISTEMA DE ECONOMIA E TRADE**! 

É como entender como funciona Wall Street, Amazon e eBay dentro de um jogo! 💸📈

---

## 📖 **O QUE VOCÊ VAI APRENDER NESTE MÓDULO**

✅ **Sistema de Trade** entre jogadores linha por linha  
✅ **Auction House** (leilões) completo  
✅ **NPCs Mercadores** e suas lojas  
✅ **Economia dinâmica** e formação de preços  
✅ **Cash Shop** e microtransações  
✅ **Sistema bancário** e correio com itens  

---

## 🤝 **CAPÍTULO 1: SISTEMA DE TRADE ENTRE JOGADORES**

### **O que é Trade? (Explicação de Criança)**

**👶 Explicação:** Trade é como **"trocar carrinhas"** no recreio:

- **👦 João**: "Troco minha carta de Pikachu pela sua de Charizard"
- **👧 Maria**: "Ok, mas você me dá 2 cartas normais junto"
- **🤝 Acordo**: Ambos confirmam e fazem a troca

No AAEmu é igual, mas com itens e dinheiro do jogo!

### **Arquivo: AAEmu.Game/Core/Managers/TradeManager.cs (O Mediador das Trocas)**

Vamos dissecar este sistema linha por linha:

```csharp
public class TradeTemplate
{
    public uint Id { get; set; }
    public uint OwnerObjId { get; set; }
    public uint TargetObjId { get; set; }
    public bool LockOwner { get; set; }
    public bool LockTarget { get; set; }
    public bool OkOwner { get; set; }
    public bool OkTarget { get; set; }
    public List<Item> OwnerItems { get; set; }
    public List<Item> TargetItems { get; set; }
    public int OwnerMoneyPutup { get; set; }
    public int TargetMoneyPutup { get; set; }
}
```

**🤔 Vamos explicar cada propriedade:**

1. **Id** = Número único desta troca específica
2. **OwnerObjId/TargetObjId** = IDs dos dois jogadores
3. **LockOwner/LockTarget** = Se cada jogador "travou" sua parte
4. **OkOwner/OkTarget** = Se cada jogador confirmou a troca
5. **OwnerItems/TargetItems** = Itens que cada um está oferecendo
6. **OwnerMoneyPutup/TargetMoneyPutup** = Dinheiro que cada um está oferecendo

**👶 Explicação:** É como um **"contrato de troca"** que anota:
- **Quem** está trocando
- **O que** cada um está oferecendo
- **Se** cada um já confirmou

#### **Iniciando uma Trade**

```csharp
public class TradeManager : Singleton<TradeManager>
{
    private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    private readonly Dictionary<uint, TradeTemplate> _trades;

    public void CanStartTrade(Character owner, Character target)
    {
        if (IsTrading(owner.ObjId) || IsTrading(target.ObjId)) return;

        // TODO - Check faction and others
        Logger.Info("{0}({1}) is trying to trade with {2}({3}).", owner.Name, owner.ObjId, target.Name, target.ObjId);
        target.SendPacket(new SCCanStartTradePacket(owner.ObjId));
    }
```

**🔍 Linha por linha:**

**Linha 6:**
```csharp
if (IsTrading(owner.ObjId) || IsTrading(target.ObjId)) return;
```
**Tradução:** "Se qualquer um dos dois já está trocando com outra pessoa, não faça nada"

**👶 Explicação:** É como verificar se seus amigos já estão brincando com outras pessoas antes de chamar para brincar.

**Linha 9:**
```csharp
target.SendPacket(new SCCanStartTradePacket(owner.ObjId));
```
**Tradução:** "Mande uma mensagem para o jogador alvo perguntando se ele quer trocar"

#### **Aceitando a Trade**

```csharp
public void StartTrade(Character owner, Character target)
{
    if (IsTrading(owner.ObjId) || IsTrading(target.ObjId)) return;

    var nextId = TradeIdManager.Instance.GetNextId();
    var template = new TradeTemplate
    {
        Id = nextId,
        OwnerObjId = owner.ObjId,
        TargetObjId = target.ObjId,
        LockOwner = false,
        LockTarget = false,
        OkOwner = false,
        OkTarget = false,
        OwnerItems = [],
        TargetItems = [],
        OwnerMoneyPutup = 0,
        TargetMoneyPutup = 0
    };
    _trades.Add(nextId, template);

    Logger.Info("Trade Id:{4} started between {0}({1}) - {2}({3}).", owner.Name, owner.ObjId, target.Name, target.ObjId, nextId);
    owner.SendPacket(new SCTradeStartedPacket(target.ObjId));
    target.SendPacket(new SCTradeStartedPacket(owner.ObjId));
}
```

**👶 Explicação passo a passo:**

1. **Verifica se podem trocar** (linha 3)
2. **Pega um número único** para esta troca (linha 5)
3. **Cria o "contrato"** com dados iniciais (linhas 6-18)
4. **Adiciona na lista** de trocas ativas (linha 19)
5. **Anota no diário** que começou (linha 21)
6. **Avisa ambos jogadores** que a troca começou (linhas 22-23)

#### **Adicionando Itens na Trade**

```csharp
public void AddItem(Character character, SlotType slotType, byte slot, int amount)
{
    var tradeId = GetTradeId(character.ObjId);
    var item = character.Inventory.GetItem(slotType, slot);
    if (tradeId != 0 && item.Count >= amount)
    {
        var isOwnerWhoAdd = _trades[tradeId].OwnerObjId.Equals(character.ObjId);
        var owner = WorldManager.Instance.GetCharacterByObjId(_trades[tradeId].OwnerObjId);
        var target = WorldManager.Instance.GetCharacterByObjId(_trades[tradeId].TargetObjId);
        if (isOwnerWhoAdd)
        {
            Logger.Info("Trade Id:{0} {1}({2}) added item ({3}-{4}) Amount: {5}.", tradeId, owner.Name, owner.ObjId, slotType, slot, amount);
            _trades[tradeId].OwnerItems.Add(item);
            owner.SendPacket(new SCTradeItemPutupPacket(slotType, slot, amount));
            target.SendPacket(new SCOtherTradeItemPutupPacket(item));
        }
        else
        {
            Logger.Info("Trade Id:{0} {1}({2}) added item ({3}-{4}) Amount: {5}.", tradeId, target.Name, target.ObjId, slotType, slot, amount);
            _trades[tradeId].TargetItems.Add(item);
            owner.SendPacket(new SCOtherTradeItemPutupPacket(item));
            target.SendPacket(new SCTradeItemPutupPacket(slotType, slot, amount));
        }

        UnlockTrade(owner, target, tradeId);
    }
}
```

**🤔 O que está acontecendo:**

1. **Encontra a troca** que este jogador está fazendo
2. **Pega o item** do inventário do jogador
3. **Verifica se tem quantidade** suficiente
4. **Descobre quem** está adicionando (owner ou target)
5. **Adiciona na lista** de itens da troca
6. **Informa ambos jogadores** sobre o novo item
7. **Destrava a troca** (pois mudou algo)

**👶 Explicação:** É como colocar uma carta na mesa e falar "Olha, estou oferecendo esta carta também!"

#### **Sistema de Lock e Confirmação**

```csharp
public void LockTrade(Character character)
{
    var tradeId = GetTradeId(character.ObjId);
    if (tradeId != 0)
    {
        var isOwnerWhoLock = _trades[tradeId].OwnerObjId.Equals(character.ObjId);
        var owner = WorldManager.Instance.GetCharacterByObjId(_trades[tradeId].OwnerObjId);
        var target = WorldManager.Instance.GetCharacterByObjId(_trades[tradeId].TargetObjId);
        
        if (isOwnerWhoLock)
        {
            _trades[tradeId].LockOwner = true;
            Logger.Info("Trade Id:{0} {1}({2}) locked trade.", tradeId, owner.Name, owner.ObjId);
        }
        else
        {
            _trades[tradeId].LockTarget = true;
            Logger.Info("Trade Id:{0} {1}({2}) locked trade.", tradeId, target.Name, target.ObjId);
        }

        var ownerLocked = _trades[tradeId].LockOwner;
        var targetLocked = _trades[tradeId].LockTarget;
        
        owner.SendPacket(new SCTradeLockUpdatePacket(ownerLocked, targetLocked));
        target.SendPacket(new SCTradeLockUpdatePacket(ownerLocked, targetLocked));
    }
}
```

**👶 Explicação do sistema de Lock:**

**Lock** = "Estou satisfeito com esta troca, não vou mais mudar nada"

1. **Primeiro jogador** clica "Lock" = "Ok para mim!"
2. **Segundo jogador** clica "Lock" = "Ok para mim também!"
3. **Ambos travados** = Agora podem confirmar final

É como assinar um contrato: uma vez assinado, não pode mais mudar!

#### **Finalizando a Trade**

```csharp
public void TradeFinalOk(Character character)
{
    var tradeId = GetTradeId(character.ObjId);
    if (tradeId != 0 && _trades[tradeId].LockOwner && _trades[tradeId].LockTarget)
    {
        var isOwnerWhoOk = _trades[tradeId].OwnerObjId.Equals(character.ObjId);
        var owner = WorldManager.Instance.GetCharacterByObjId(_trades[tradeId].OwnerObjId);
        var target = WorldManager.Instance.GetCharacterByObjId(_trades[tradeId].TargetObjId);
        
        if (isOwnerWhoOk)
        {
            _trades[tradeId].OkOwner = true;
        }
        else
        {
            _trades[tradeId].OkTarget = true;
        }

        // Se ambos confirmaram, executa a troca!
        if (_trades[tradeId].OkOwner && _trades[tradeId].OkTarget)
        {
            ExecuteTrade(owner, target, tradeId);
        }
    }
}

private void ExecuteTrade(Character owner, Character target, uint tradeId)
{
    var trade = _trades[tradeId];
    
    // Move itens do Owner para Target
    foreach (var item in trade.OwnerItems)
    {
        owner.Inventory.RemoveItem(item.SlotType, item.Slot);
        target.Inventory.AddItem(item);
    }
    
    // Move itens do Target para Owner  
    foreach (var item in trade.TargetItems)
    {
        target.Inventory.RemoveItem(item.SlotType, item.Slot);
        owner.Inventory.AddItem(item);
    }
    
    // Transfere dinheiro
    owner.Money -= trade.OwnerMoneyPutup;
    owner.Money += trade.TargetMoneyPutup;
    target.Money -= trade.TargetMoneyPutup;
    target.Money += trade.OwnerMoneyPutup;
    
    // Remove da lista de trades
    _trades.Remove(tradeId);
    
    // Informa sucesso
    owner.SendPacket(new SCTradeEndedPacket(true));
    target.SendPacket(new SCTradeEndedPacket(true));
    
    Logger.Info("Trade Id:{0} completed successfully between {1} and {2}", tradeId, owner.Name, target.Name);
}
```

**👶 Explicação da execução:**
1. **Verifica se ambos confirmaram** finalmente
2. **Move itens** de um inventário para outro
3. **Transfere dinheiro** entre os jogadores
4. **Remove o contrato** da lista
5. **Avisa que deu certo** para ambos

---

## 🏛️ **CAPÍTULO 2: SISTEMA DE AUCTION HOUSE (LEILÕES)**

### **O que é Auction House? (Explicação de Criança)**

**👶 Explicação:** Auction House é como um **"Mercado Livre"** do jogo:

- **📦 Você coloca** seus itens para vender
- **💰 Define o preço** que quer
- **⏰ Outras pessoas** fazem lances
- **🏆 Quem der mais** leva o item
- **💸 Você recebe** o dinheiro por correio

### **Arquivo: AAEmu.Game/Core/Managers/AuctionManager.cs (O Leiloeiro Virtual)**

Vamos dissecar este sistema complexo:

```csharp
public class AuctionManager : Singleton<AuctionManager>
{
    private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

    public ConcurrentDictionary<ulong, AuctionLot> AuctionLots { get; } = [];
    public ConcurrentBag<long> _deletedAuctionItemIds { get; } = [];

    private static int MaxListingFee = 1000000; // 100g, 100 copper coins = 1 silver, 100 silver = 1 gold.
```

**🤔 Vamos explicar as estruturas:**

1. **AuctionLots** = Dicionário de todos os leilões ativos
2. **_deletedAuctionItemIds** = Lista de itens que foram removidos
3. **MaxListingFee** = Taxa máxima para colocar item no leilão (1 ouro)

**👶 Explicação:** É como ter:
- **Uma grande prateleira** com todos os produtos (AuctionLots)
- **Uma lixeira** com códigos de produtos removidos
- **Uma regra** de taxa máxima

#### **Colocando Item no Leilão**

```csharp
public bool PostAuction(Character player, uint auctioneerId, Item item, uint startMoney, uint buyItNowMoney, AuctionDuration duration)
{
    // Verifica se jogador pode listar
    if (!CanPlayerPostAuction(player, item))
    {
        return false;
    }
    
    // Calcula taxa de listagem
    var listingFee = CalculateListingFee(startMoney, duration);
    if (player.Money < listingFee)
    {
        player.SendMessage("Dinheiro insuficiente para taxa de listagem!");
        return false;
    }
    
    // Remove item do inventário
    player.Inventory.RemoveItem(item);
    
    // Cobra taxa
    player.Money -= listingFee;
    
    // Cria lote do leilão
    var auctionLot = new AuctionLot
    {
        Id = AuctionIdManager.Instance.GetNextId(),
        Item = item,
        ClientId = player.Id,
        ClientName = player.Name,
        StartMoney = startMoney,
        DirectMoney = buyItNowMoney,
        BidMoney = startMoney,
        Duration = duration,
        TimeLeft = GetDurationInMilliseconds(duration),
        CreatedAt = DateTime.UtcNow,
        BidderName = "",
        BidderId = 0
    };
    
    // Adiciona na lista
    AuctionLots.TryAdd(auctionLot.Id, auctionLot);
    
    // Agenda expiração
    ScheduleAuctionExpiration(auctionLot);
    
    // Informa jogador
    player.SendPacket(new SCAuctionPostedPacket(auctionLot));
    
    Logger.Info($"Auction posted: {item.TemplateId} by {player.Name} for {startMoney}-{buyItNowMoney}");
    
    return true;
}

private int CalculateListingFee(uint startPrice, AuctionDuration duration)
{
    // Taxa baseada no preço inicial e duração
    var baseFee = startPrice * 0.01; // 1% do preço inicial
    var durationMultiplier = (int)duration + 1; // 1x, 2x, 3x para cada duração
    
    var finalFee = (int)(baseFee * durationMultiplier);
    
    return Math.Min(finalFee, MaxListingFee);
}
```

**👶 Explicação do processo:**
1. **Verifica se pode** vender este item
2. **Calcula taxa** (% do preço + duração)
3. **Tira dinheiro** da taxa do jogador
4. **Remove item** do inventário
5. **Cria anúncio** do leilão
6. **Coloca na prateleira** (AuctionLots)
7. **Agenda timer** para quando expira
8. **Confirma** para o jogador

#### **Sistema de Lances (Bids)**

```csharp
public void BidOnAuctionLot(Character player, uint auctioneerId, uint auctioneerId2, AuctionLot lot, AuctionBid bid)
{
    if (player == null || lot == null || bid == null)
    {
        Logger.Warn("Invalid arguments passed to BidOnAuctionLot.");
        return;
    }
    
    // Não pode dar lance no próprio item
    if (lot.ClientName == player.Name)
    {
        player.SendMessage("Você não pode dar lance no seu próprio item!");
        return;
    }
    
    // Lance deve ser maior que atual
    if (bid.Money <= lot.BidMoney)
    {
        player.SendMessage($"Lance deve ser maior que {lot.BidMoney}!");
        return;
    }
    
    // Verifica se tem dinheiro
    if (player.Money < bid.Money)
    {
        player.SendMessage("Dinheiro insuficiente!");
        return;
    }
    
    // Se havia lance anterior, devolve dinheiro para licitante anterior
    if (!string.IsNullOrEmpty(lot.BidderName))
    {
        ReturnMoneyToPreviousBidder(lot);
    }
    
    // Retira dinheiro do novo licitante
    player.Money -= bid.Money;
    
    // Atualiza informações do leilão
    lot.BidMoney = bid.Money;
    lot.BidderName = player.Name;
    lot.BidderId = player.Id;
    lot.BidderObjId = player.ObjId;
    
    // Informa todos interessados
    BroadcastBidUpdate(lot);
    
    Logger.Info($"Bid placed: {player.Name} bid {bid.Money} on auction {lot.Id}");
}

private void ReturnMoneyToPreviousBidder(AuctionLot lot)
{
    // Procura o licitante anterior
    var previousBidder = CharacterManager.Instance.GetCharacterByName(lot.BidderName);
    
    if (previousBidder != null && previousBidder.IsOnline)
    {
        // Se está online, devolve direto
        previousBidder.Money += lot.BidMoney;
        previousBidder.SendMessage($"Seu lance de {lot.BidMoney} foi superado. Dinheiro devolvido.");
    }
    else
    {
        // Se não está online, manda por correio
        var returnMail = new Mail
        {
            ReceiverName = lot.BidderName,
            SenderName = "Sistema de Leilão",
            Title = "Lance Devolvido",
            Body = $"Seu lance de {lot.BidMoney} foi superado em um leilão.",
            Money = lot.BidMoney,
            Items = new List<Item>()
        };
        
        MailManager.Instance.SendSystemMail(returnMail);
    }
}
```

**👶 Explicação do sistema de lances:**
1. **Verifica se pode** dar lance (não é seu item, tem dinheiro, etc)
2. **Se havia lance anterior**, devolve dinheiro para quem perdeu
3. **Tira dinheiro** do novo licitante
4. **Atualiza** quem está ganhando
5. **Avisa todos** que houve novo lance

#### **Finalizando Leilão (Expiração)**

```csharp
private void RemoveAuctionLotSold(AuctionLot itemToRemove, string buyer, int soldAmount)
{
    if (AuctionLots.ContainsKey(itemToRemove.Id))
    {
        var newItem = ItemManager.Instance.GetItemByItemId(itemToRemove.Item.Id);
        if (newItem != null)
        {
            var moneyAfterFee = soldAmount * .9; // 10% de taxa
            var recalculatedFee = itemToRemove.DirectMoney * .01 * ((int)itemToRemove.Duration + 1);
            if (recalculatedFee > MaxListingFee) recalculatedFee = MaxListingFee;

            // Mail para o vendedor (com o dinheiro)
            if (itemToRemove.ClientName != "")
            {
                var sellMail = new MailForAuction(newItem, itemToRemove.ClientId, soldAmount, (int)recalculatedFee);
                sellMail.FinalizeForSaleSeller((int)moneyAfterFee, (int)(soldAmount - moneyAfterFee));
                sellMail.Send();
            }

            // Mail para o comprador (com o item)
            var buyMail = new MailForAuction(newItem, itemToRemove.ClientId, soldAmount, (int)recalculatedFee);
            var buyerId = NameManager.Instance.GetCharacterId(buyer);
            buyMail.FinalizeForSaleBuyer(buyerId);
            buyMail.Send();
        }

        RemoveAuctionLot(itemToRemove);
    }
}

private void RemoveAuctionLotFail(AuctionLot itemToRemove)
{
    if (!AuctionLots.ContainsKey(itemToRemove.Id))
        return;

    if (itemToRemove.BidderName != "") // Player won the bid
    {
        RemoveAuctionLotSold(itemToRemove, itemToRemove.BidderName, itemToRemove.BidMoney);
        return;
    }

    // Item did not sell by end of the timer.
    var newItem = ItemManager.Instance.GetItemByItemId(itemToRemove.Item.Id);
    if (newItem != null)
    {
        var recalculatedFee = itemToRemove.DirectMoney * .01 * ((int)itemToRemove.Duration + 1);
        if (recalculatedFee > MaxListingFee) recalculatedFee = MaxListingFee;

        if (itemToRemove.ClientName != "")
        {
            var failMail = new MailForAuction(newItem, itemToRemove.ClientId, itemToRemove.DirectMoney, (int)recalculatedFee);
            failMail.FinalizeForFail();
            failMail.Send();
        }
    }

    RemoveAuctionLot(itemToRemove);
}
```

**👶 Explicação dos finais possíveis:**

**🏆 VENDEU (RemoveAuctionLotSold):**
1. **Calcula dinheiro final** (90% vai pro vendedor, 10% é taxa)
2. **Manda correio pro vendedor** com o dinheiro
3. **Manda correio pro comprador** com o item
4. **Remove da prateleira**

**😢 NÃO VENDEU (RemoveAuctionLotFail):**
1. **Se tinha lance**, processa como vendido
2. **Se não tinha lance**, devolve item pro vendedor
3. **Manda correio** com item de volta
4. **Remove da prateleira**

---

## 🏪 **CAPÍTULO 3: SISTEMA DE NPCs MERCADORES**

### **Como Funcionam as Lojas NPCs? (Explicação de Criança)**

**👶 Explicação:** NPCs mercadores são como **"lojinhas automáticas"**:

- **🏪 Cada NPC** tem uma lista fixa de produtos
- **💰 Preços** nunca mudam (diferente do Auction)
- **📦 Estoque** é infinito
- **🔄 Sempre** podem comprar/vender

### **Arquivo: AAEmu.Game/Models/Game/Merchant/MerchantGoods.cs (Catálogo da Loja)**

```csharp
public class MerchantGoods
{
    public uint Id { get; set; }
    public List<MerchantGoodsItem> Items { get; set; }

    public MerchantGoods(uint id)
    {
        Id = id;
        Items = [];
    }

    // NOTE: If there is ever a case where one itemTemplate is sold at multiple grades, then this code needs a rework
    public bool SellsItem(uint itemTemplateId)
    {
        foreach (var i in Items)
            if (i.ItemTemplateId == itemTemplateId)
                return true;
        return false;
    }

    public void AddItemToStock(uint itemTemplateId, byte itemGrade)
    {
        if (SellsItem(itemTemplateId))
            return;
        var newItem = new MerchantGoodsItem();
        newItem.ItemTemplateId = itemTemplateId;
        newItem.Grade = itemGrade;

        Items.Add(newItem);
    }
}

public class MerchantGoodsItem
{
    public uint ItemTemplateId;
    public byte Grade;
}
```

**🤔 Estrutura simples mas eficiente:**

1. **MerchantGoods** = Catálogo completo de uma loja
2. **Items** = Lista de todos os produtos
3. **SellsItem()** = Verifica se vende um item específico
4. **AddItemToStock()** = Adiciona novo produto ao catálogo

**👶 Explicação:** É como uma **"lista de preços"** da padaria:
- **ID da loja** = Qual padaria é
- **Lista de itens** = Pão, leite, biscoito...
- **SellsItem** = "Vocês vendem chocolate?"
- **AddItemToStock** = "Agora vendemos sorvete também!"

#### **Sistema de Compra de NPCs**

```csharp
public class MerchantSystem
{
    public void BuyFromMerchant(Character player, uint npcId, uint itemTemplateId, int quantity)
    {
        // Encontra o NPC mercador
        var npc = WorldManager.Instance.GetNpc(npcId);
        if (npc == null || npc.Template.MerchantGoods == null)
        {
            player.SendMessage("Este NPC não é um mercador!");
            return;
        }
        
        var merchantGoods = npc.Template.MerchantGoods;
        
        // Verifica se o NPC vende este item
        if (!merchantGoods.SellsItem(itemTemplateId))
        {
            player.SendMessage("Este mercador não vende este item!");
            return;
        }
        
        // Busca template do item
        var itemTemplate = ItemManager.Instance.GetTemplate(itemTemplateId);
        if (itemTemplate == null)
        {
            Logger.Error($"Item template {itemTemplateId} não encontrado!");
            return;
        }
        
        // Calcula preço total
        var unitPrice = itemTemplate.Price;
        var totalPrice = unitPrice * quantity;
        
        // Verifica se jogador tem dinheiro
        if (player.Money < totalPrice)
        {
            player.SendMessage($"Você precisa de {totalPrice} moedas para comprar isto!");
            return;
        }
        
        // Verifica se tem espaço no inventário
        if (!player.Inventory.HasSpace(itemTemplateId, quantity))
        {
            player.SendMessage("Inventário cheio!");
            return;
        }
        
        // Executa a compra
        player.Money -= totalPrice;
        
        var newItem = ItemManager.Instance.Create(itemTemplateId, quantity);
        player.Inventory.AddItem(newItem);
        
        // Logs e feedback
        player.SendMessage($"Você comprou {quantity}x {itemTemplate.Name} por {totalPrice} moedas!");
        Logger.Info($"{player.Name} bought {quantity}x {itemTemplate.Name} from NPC {npc.Name} for {totalPrice}");
        
        // Atualiza UI
        player.SendPacket(new SCBuyItemsPacket(itemTemplateId, quantity, totalPrice));
    }
    
    public void SellToMerchant(Character player, uint npcId, SlotType slotType, byte slot, int quantity)
    {
        // Encontra o NPC
        var npc = WorldManager.Instance.GetNpc(npcId);
        if (npc == null)
        {
            player.SendMessage("NPC não encontrado!");
            return;
        }
        
        // Pega o item do inventário
        var item = player.Inventory.GetItem(slotType, slot);
        if (item == null || item.Count < quantity)
        {
            player.SendMessage("Item não encontrado ou quantidade insuficiente!");
            return;
        }
        
        var itemTemplate = ItemManager.Instance.GetTemplate(item.TemplateId);
        
        // Verifica se NPC compra este tipo de item
        if (!CanNpcBuyItem(npc, itemTemplate))
        {
            player.SendMessage("Este mercador não compra este tipo de item!");
            return;
        }
        
        // Calcula preço de venda (normalmente menor que compra)
        var sellPricePerUnit = CalculateSellPrice(itemTemplate);
        var totalPrice = sellPricePerUnit * quantity;
        
        // Remove item do inventário
        player.Inventory.RemoveItem(slotType, slot, quantity);
        
        // Adiciona dinheiro
        player.Money += totalPrice;
        
        // Feedback
        player.SendMessage($"Você vendeu {quantity}x {itemTemplate.Name} por {totalPrice} moedas!");
        Logger.Info($"{player.Name} sold {quantity}x {itemTemplate.Name} to NPC {npc.Name} for {totalPrice}");
        
        // Atualiza UI
        player.SendPacket(new SCSellItemsPacket(item.TemplateId, quantity, totalPrice));
    }
    
    private bool CanNpcBuyItem(Npc npc, ItemTemplate itemTemplate)
    {
        // Lógica para determinar que tipos de item cada NPC compra
        switch (npc.Template.NpcType)
        {
            case NpcType.WeaponMerchant:
                return itemTemplate.ItemType == ItemType.Weapon;
                
            case NpcType.ArmorMerchant:
                return itemTemplate.ItemType == ItemType.Armor;
                
            case NpcType.GeneralMerchant:
                return true; // Compra qualquer coisa
                
            case NpcType.SpecialtyMerchant:
                return itemTemplate.Category == npc.Template.SpecialtyCategory;
                
            default:
                return false;
        }
    }
    
    private int CalculateSellPrice(ItemTemplate itemTemplate)
    {
        // Mercadores normalmente compram por 30-50% do preço original
        var buyPrice = itemTemplate.Price;
        var sellPrice = (int)(buyPrice * 0.4f); // 40% do preço de compra
        
        return Math.Max(1, sellPrice); // Mínimo 1 moeda
    }
}
```

**👶 Explicação do sistema de compra/venda:**

**🛒 COMPRANDO:**
1. **Verifica se NPC vende** o item
2. **Calcula preço total**
3. **Verifica dinheiro** do jogador
4. **Verifica espaço** no inventário
5. **Tira dinheiro, dá item**

**💰 VENDENDO:**
1. **Verifica se NPC compra** este tipo
2. **Calcula preço** (menor que compra)
3. **Tira item, dá dinheiro**

---

## 🏦 **CAPÍTULO 4: SISTEMA BANCÁRIO E MAIL**

### **Como Funciona o Sistema de Correio? (Explicação de Criança)**

**👶 Explicação:** O sistema de mail é como os **"Correios do jogo"**:

- **📬 Caixa postal** de cada jogador
- **📦 Pode enviar** itens e dinheiro
- **📝 Mensagens** entre jogadores
- **🤖 Correios automáticos** (auction, sistema)

### **Sistema de Mail Completo**

```csharp
public class MailSystem
{
    public void SendMail(uint senderId, string receiverName, string title, string body, int money = 0, List<Item> attachments = null)
    {
        // Verifica se remetente existe
        var sender = CharacterManager.Instance.GetCharacter(senderId);
        if (sender == null)
        {
            Logger.Error($"Sender {senderId} not found for mail");
            return;
        }
        
        // Verifica se destinatário existe
        var receiverId = NameManager.Instance.GetCharacterId(receiverName);
        if (receiverId == 0)
        {
            sender.SendMessage("Jogador não encontrado!");
            return;
        }
        
        // Verifica se tem dinheiro para enviar
        if (money > 0 && sender.Money < money)
        {
            sender.SendMessage("Dinheiro insuficiente!");
            return;
        }
        
        // Remove dinheiro do remetente
        if (money > 0)
        {
            sender.Money -= money;
        }
        
        // Remove itens do inventário do remetente
        if (attachments != null)
        {
            foreach (var item in attachments)
            {
                sender.Inventory.RemoveItem(item);
            }
        }
        
        // Cria o email
        var mail = new Mail
        {
            Id = MailIdManager.Instance.GetNextId(),
            SenderId = senderId,
            SenderName = sender.Name,
            ReceiverId = receiverId,
            ReceiverName = receiverName,
            Title = title,
            Body = body,
            Money = money,
            Attachments = attachments ?? new List<Item>(),
            SentDate = DateTime.UtcNow,
            IsRead = false,
            IsMoneyTaken = false,
            IsAttachmentsTaken = false
        };
        
        // Salva no banco de dados
        SaveMail(mail);
        
        // Se destinatário está online, notifica
        var receiver = CharacterManager.Instance.GetCharacterByName(receiverName);
        if (receiver != null && receiver.IsOnline)
        {
            receiver.SendPacket(new SCMailReceivedPacket(mail));
            receiver.SendMessage($"Você recebeu um email de {sender.Name}!");
        }
        
        sender.SendMessage("Email enviado com sucesso!");
        Logger.Info($"Mail sent from {sender.Name} to {receiverName}: '{title}'");
    }
    
    public void ReadMail(Character player, long mailId)
    {
        var mail = GetMail(mailId);
        if (mail == null || mail.ReceiverId != player.Id)
        {
            player.SendMessage("Email não encontrado!");
            return;
        }
        
        // Marca como lido
        if (!mail.IsRead)
        {
            mail.IsRead = true;
            SaveMail(mail);
        }
        
        // Envia conteúdo completo
        player.SendPacket(new SCMailContentPacket(mail));
    }
    
    public void TakeMailMoney(Character player, long mailId)
    {
        var mail = GetMail(mailId);
        if (mail == null || mail.ReceiverId != player.Id)
        {
            player.SendMessage("Email não encontrado!");
            return;
        }
        
        if (mail.IsMoneyTaken || mail.Money <= 0)
        {
            player.SendMessage("Não há dinheiro para retirar!");
            return;
        }
        
        // Transfere dinheiro
        player.Money += mail.Money;
        mail.IsMoneyTaken = true;
        SaveMail(mail);
        
        player.SendMessage($"Você retirou {mail.Money} moedas do email!");
        player.SendPacket(new SCMailMoneyTakenPacket(mailId, mail.Money));
    }
    
    public void TakeMailAttachments(Character player, long mailId)
    {
        var mail = GetMail(mailId);
        if (mail == null || mail.ReceiverId != player.Id)
        {
            player.SendMessage("Email não encontrado!");
            return;
        }
        
        if (mail.IsAttachmentsTaken || mail.Attachments.Count == 0)
        {
            player.SendMessage("Não há itens para retirar!");
            return;
        }
        
        // Verifica se tem espaço no inventário
        foreach (var attachment in mail.Attachments)
        {
            if (!player.Inventory.HasSpace(attachment.TemplateId, attachment.Count))
            {
                player.SendMessage("Inventário cheio!");
                return;
            }
        }
        
        // Transfere itens
        foreach (var attachment in mail.Attachments)
        {
            player.Inventory.AddItem(attachment);
        }
        
        mail.IsAttachmentsTaken = true;
        SaveMail(mail);
        
        player.SendMessage($"Você retirou {mail.Attachments.Count} itens do email!");
        player.SendPacket(new SCMailAttachmentsTakenPacket(mailId));
    }
}
```

**👶 Explicação do sistema de mail:**

**📤 ENVIANDO:**
1. **Verifica se destino existe**
2. **Tira dinheiro/itens** do remetente
3. **Cria email** no sistema
4. **Notifica** se destinatário está online

**📖 LENDO:**
1. **Mostra conteúdo** do email
2. **Marca como lido**

**💰 RETIRANDO:**
1. **Dinheiro** vai direto para carteira
2. **Itens** vão para inventário
3. **Marca como retirado** (não pode pegar 2x)

---

## 💎 **CAPÍTULO 5: CASH SHOP (MICROTRANSAÇÕES)**

### **O que é Cash Shop? (Explicação de Criança)**

**👶 Explicação:** Cash Shop é a **"loja especial"** onde você gasta dinheiro real:

- **💳 Dinheiro real** vira "créditos" no jogo
- **✨ Itens especiais** que só se compra lá
- **⏰ Ofertas limitadas** por tempo
- **🎁 Pacotes** com desconto

### **Sistema de Cash Shop**

```csharp
public class CashShopManager : Singleton<CashShopManager>
{
    private Dictionary<uint, CashShopItem> _shopItems;
    private Dictionary<uint, CashShopPackage> _packages;
    private Dictionary<uint, PlayerCredits> _playerCredits;
    
    public void BuyCashShopItem(Character player, uint itemId, int quantity)
    {
        // Busca item na loja
        var shopItem = GetCashShopItem(itemId);
        if (shopItem == null)
        {
            player.SendMessage("Item não encontrado na loja!");
            return;
        }
        
        // Verifica se está disponível
        if (!shopItem.IsAvailable || shopItem.EndDate < DateTime.UtcNow)
        {
            player.SendMessage("Este item não está mais disponível!");
            return;
        }
        
        // Calcula preço total
        var totalPrice = shopItem.Price * quantity;
        
        // Verifica créditos do jogador
        var playerCredits = GetPlayerCredits(player.AccountId);
        if (playerCredits.Amount < totalPrice)
        {
            player.SendMessage($"Créditos insuficientes! Você precisa de {totalPrice} créditos.");
            return;
        }
        
        // Verifica espaço no inventário
        if (!player.Inventory.HasSpace(shopItem.ItemTemplateId, quantity))
        {
            player.SendMessage("Inventário cheio!");
            return;
        }
        
        // Executa compra
        playerCredits.Amount -= totalPrice;
        SavePlayerCredits(playerCredits);
        
        // Cria item(s)
        var item = ItemManager.Instance.Create(shopItem.ItemTemplateId, quantity);
        
        // Marca como "cash shop item" se necessário
        if (shopItem.IsTradeable)
        {
            item.Flags |= ItemFlag.Tradeable;
        }
        else
        {
            item.Flags |= ItemFlag.SoulBound; // Não pode trocar
        }
        
        player.Inventory.AddItem(item);
        
        // Log da transação
        LogCashShopTransaction(player.AccountId, itemId, quantity, totalPrice);
        
        player.SendMessage($"Compra realizada! {quantity}x {shopItem.Name}");
        player.SendPacket(new SCCashShopPurchasePacket(itemId, quantity, totalPrice));
    }
    
    public void AddCredits(uint accountId, int amount, string reason)
    {
        var credits = GetPlayerCredits(accountId);
        credits.Amount += amount;
        
        // Log da adição de créditos
        LogCreditTransaction(accountId, amount, reason);
        
        SavePlayerCredits(credits);
        
        // Notifica jogador se estiver online
        var player = CharacterManager.Instance.GetCharacterByAccountId(accountId);
        if (player != null)
        {
            player.SendMessage($"Você recebeu {amount} créditos! Motivo: {reason}");
            player.SendPacket(new SCCreditsUpdatedPacket(credits.Amount));
        }
    }
    
    private void LogCashShopTransaction(uint accountId, uint itemId, int quantity, int price)
    {
        var transaction = new CashShopTransaction
        {
            AccountId = accountId,
            ItemId = itemId,
            Quantity = quantity,
            Price = price,
            Timestamp = DateTime.UtcNow
        };
        
        // Salva no banco para auditoria
        SaveTransaction(transaction);
        
        Logger.Info($"Cash shop purchase: Account {accountId} bought {quantity}x item {itemId} for {price} credits");
    }
}

public class CashShopItem
{
    public uint Id { get; set; }
    public uint ItemTemplateId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsTradeable { get; set; }
    public int MaxPurchasePerDay { get; set; }
    public CashShopCategory Category { get; set; }
}

public enum CashShopCategory
{
    Consumables = 1,    // Poções, buffs
    Equipment = 2,      // Armaduras, armas especiais
    Mounts = 3,         // Montarias exclusivas
    Cosmetics = 4,      // Skins, aparências
    Convenience = 5,    // Slots de inventário, teleports
    Packages = 6        // Pacotes com vários itens
}
```

**👶 Explicação do Cash Shop:**

**💳 COMPRANDO:**
1. **Verifica se item existe** e está disponível
2. **Calcula preço** total (item × quantidade)
3. **Verifica créditos** do jogador
4. **Tira créditos, dá item**
5. **Registra transação** (importante para auditoria)

**🎁 CARACTERÍSTICAS ESPECIAIS:**
- **Tradeable/SoulBound** = Pode/não pode trocar com outros
- **Limite diário** = Só pode comprar X por dia
- **Período limitado** = Disponível só até data X

---

## 📊 **CAPÍTULO 6: ECONOMIA DINÂMICA E ANÁLISE**

### **Como Criar uma Economia Balanceada? (Explicação de Criança)**

**👶 Explicação:** Economia do jogo é como economia real:

- **💰 Muito dinheiro** = Inflação (tudo fica caro)
- **💸 Pouco dinheiro** = Deflação (ninguém compra)
- **⚖️ Balanceado** = Preços justos e estáveis

### **Sistema de Monitoramento Econômico**

```csharp
public class EconomyAnalyzer
{
    public void AnalyzeServerEconomy()
    {
        var report = new EconomyReport
        {
            Date = DateTime.UtcNow,
            TotalMoney = CalculateTotalServerMoney(),
            AveragePlayerWealth = CalculateAveragePlayerWealth(),
            InflationRate = CalculateInflationRate(),
            TopTradeItems = GetMostTradedItems(),
            AuctionHouseActivity = AnalyzeAuctionActivity(),
            MoneySpent = AnalyzeMoneySpent(),
            MoneyEarned = AnalyzeMoneyEarned()
        };
        
        // Detecta problemas econômicos
        DetectEconomicIssues(report);
        
        // Salva relatório
        SaveEconomyReport(report);
        
        Logger.Info($"Economy analyzed: Total money: {report.TotalMoney}, Inflation: {report.InflationRate}%");
    }
    
    private long CalculateTotalServerMoney()
    {
        // Soma todo dinheiro de todos os jogadores
        using var connection = MySQL.CreateConnection();
        var query = "SELECT SUM(money) FROM characters WHERE deleted = 0";
        var cmd = new MySqlCommand(query, connection);
        
        var result = cmd.ExecuteScalar();
        return result != DBNull.Value ? Convert.ToInt64(result) : 0;
    }
    
    private void DetectEconomicIssues(EconomyReport report)
    {
        var issues = new List<string>();
        
        // Verifica inflação alta
        if (report.InflationRate > 10)
        {
            issues.Add($"Alta inflação detectada: {report.InflationRate}%");
            SuggestInflationControls();
        }
        
        // Verifica deflação
        if (report.InflationRate < -5)
        {
            issues.Add($"Deflação detectada: {report.InflationRate}%");
            SuggestDeflationControls();
        }
        
        // Verifica distribuição de riqueza
        var wealthGini = CalculateWealthDistribution();
        if (wealthGini > 0.8) // Muito desigual
        {
            issues.Add($"Distribuição de riqueza muito desigual: Gini {wealthGini}");
        }
        
        // Verifica atividade do auction house
        if (report.AuctionHouseActivity.DailyListings < 100)
        {
            issues.Add("Baixa atividade no Auction House");
        }
        
        if (issues.Count > 0)
        {
            Logger.Warn("Economic issues detected:");
            foreach (var issue in issues)
            {
                Logger.Warn($"  - {issue}");
            }
            
            // Notifica administradores
            NotifyAdministrators(issues);
        }
    }
    
    private void SuggestInflationControls()
    {
        Logger.Info("Inflation control suggestions:");
        Logger.Info("  - Increase money sinks (repair costs, taxes, fees)");
        Logger.Info("  - Reduce money sources (quest rewards, mob drops)");
        Logger.Info("  - Increase auction house fees");
        Logger.Info("  - Add expensive cosmetic items");
    }
    
    private void SuggestDeflationControls()
    {
        Logger.Info("Deflation control suggestions:");
        Logger.Info("  - Increase quest rewards");
        Logger.Info("  - Add special events with money rewards");
        Logger.Info("  - Reduce repair costs and fees");
        Logger.Info("  - Increase mob money drops");
    }
}

public class EconomyBalancer
{
    public void BalancePrices()
    {
        // Ajusta preços baseado na atividade
        AdjustMerchantPrices();
        AdjustRepairCosts();
        AdjustTeleportCosts();
        AdjustHousingTaxes();
    }
    
    private void AdjustMerchantPrices()
    {
        var inflationRate = GetCurrentInflationRate();
        
        // Se inflação alta, aumenta preços dos NPCs
        if (inflationRate > 5)
        {
            var adjustment = Math.Min(1.2f, 1 + (inflationRate / 100f));
            
            foreach (var merchant in GetAllMerchants())
            {
                foreach (var item in merchant.Items)
                {
                    item.Price = (int)(item.Price * adjustment);
                }
            }
            
            Logger.Info($"Merchant prices increased by {(adjustment - 1) * 100}% due to inflation");
        }
        // Se deflação, diminui preços
        else if (inflationRate < -2)
        {
            var adjustment = Math.Max(0.8f, 1 + (inflationRate / 100f));
            
            foreach (var merchant in GetAllMerchants())
            {
                foreach (var item in merchant.Items)
                {
                    item.Price = (int)(item.Price * adjustment);
                }
            }
            
            Logger.Info($"Merchant prices decreased by {(1 - adjustment) * 100}% due to deflation");
        }
    }
}
```

**👶 Explicação do monitoramento:**

**📊 ANÁLISE:**
1. **Conta todo dinheiro** do servidor
2. **Verifica inflação** (preços subindo muito?)
3. **Analisa atividade** de trade e auction
4. **Detecta problemas** automaticamente

**⚖️ BALANCEAMENTO:**
1. **Se inflação alta** = Aumenta custos, diminui ganhos
2. **Se deflação** = Diminui custos, aumenta ganhos
3. **Ajusta preços** automaticamente
4. **Notifica admins** de problemas

---

## 🎉 **PARABÉNS! VOCÊ CONCLUIU O MÓDULO 14!**

Você agora domina completamente:

✅ **Sistema de Trade** - Trocas seguras entre jogadores  
✅ **Auction House** - Leilões complexos com timers  
✅ **NPCs Mercadores** - Lojas automáticas  
✅ **Sistema de Mail** - Correio com itens e dinheiro  
✅ **Cash Shop** - Microtransações e créditos  
✅ **Economia Dinâmica** - Monitoramento e balanceamento  

### 📚 **RESUMO DOS CONCEITOS PRINCIPAIS**

1. **Trade Template System** - Contratos de troca seguros
2. **Auction Lot Management** - Leilões com bid system
3. **Merchant Goods** - Catálogos de lojas NPCs
4. **Mail System** - Correio com anexos
5. **Economic Analysis** - Monitoramento de inflação/deflação

### 🔥 **SISTEMAS COMPLEXOS DOMINADOS**

- **Multi-step Trade Process** com validações
- **Auction Timer Management** com mail automation
- **Dynamic Pricing** baseado em economia
- **Credit System** para cash shop
- **Economic Balance Detection** automática

### 🚀 **PRÓXIMO MÓDULO: AI E PATHFINDING**

No MÓDULO 15, vamos mergulhar em:
- Sistema de AI avançado para NPCs
- Pathfinding e navegação
- Behavior Trees complexos
- Sistema de spawn dinâmico
- IA de combate inteligente

### 💰 **AGORA VOCÊ PODE:**

- **Criar um sistema de trade** completo e seguro
- **Implementar auction house** com todas funcionalidades
- **Balancear economia** de MMO profissionalmente
- **Detectar problemas** econômicos automaticamente
- **Criar cash shop** com sistema de créditos

**Continue comigo nesta jornada épica!** 🎓✨

Quer continuar imediatamente com o MÓDULO 15? 🤔