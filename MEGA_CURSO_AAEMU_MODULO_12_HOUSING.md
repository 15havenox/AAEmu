# 🏠 **MÓDULO 12: HOUSING SYSTEM - CONSTRUÇÃO COMO JOGO DE LEGO**
## 🎓 O GUIA DEFINITIVO PARA CRIAR UM SISTEMA DE HABITAÇÃO COMPLETO

---

## 🎯 **INTRODUÇÃO COMPLETA AO HOUSING SYSTEM**

### **12.1 O que é o Housing System? - Explicação para Iniciantes**

Imagine que você está jogando com **LEGO digital**! O Housing System do ArcheAge é exatamente isso - um sistema onde os jogadores podem:

- **🏗️ Construir casas** do zero
- **🎨 Decorar** seus espaços
- **🌱 Plantar** jardins
- **🏪 Abrir lojas** para outros jogadores
- **💰 Pagar impostos** para manter tudo funcionando

É como se cada jogador pudesse ter seu próprio **pedacinho do mundo** para personalizar!

### **12.2 Por que Housing é Importante em MMORPGs?**

O sistema de habitação não é só "decoração bonita". Ele serve para:

#### **🏠 Senso de Propriedade**
- **"Este é MEU lugar!"** - Cria vínculo emocional com o jogo
- **Investimento de tempo** - Quanto mais você constrói, mais quer continuar
- **Status social** - Casa bonita = prestígio na comunidade

#### **💰 Driver Econômico**
- **Consumo de recursos** - Precisa de madeira, pedra, metal
- **Profissões integradas** - Carpinteiros, pedreiros, arquitetos
- **Mercado imobiliário** - Compra/venda de terrenos e casas

#### **🎮 Gameplay Loop**
- **Meta de longo prazo** - "Quero construir um castelo!"
- **Atividade social** - Visitar casas de amigos
- **Customização infinita** - Sempre tem algo para melhorar

### **12.3 Analogias para Entender o Sistema**

Para entender como funciona, vamos usar analogias:

#### **🏗️ Construção = LEGO**
- **Peças básicas**: Paredes, tetos, portas, janelas
- **Encaixe**: Cada peça tem pontos de conexão específicos
- **Regras**: Não pode flutuar no ar, precisa de base sólida
- **Criatividade**: Infinitas combinações possíveis

#### **📋 Validação = Inspetor de Obras**
- **Código de obras**: Regras que devem ser seguidas
- **Aprovação**: Sistema verifica se construção é válida
- **Correções**: Se algo está errado, aponta o problema
- **Licenças**: Permissões para construir em determinados locais

#### **💰 Impostos = Conta de Luz**
- **Pagamento regular**: Todo mês/semana tem que pagar
- **Consequências**: Se não pagar, casa pode ser demolida
- **Valor variável**: Casa maior = imposto maior
- **Benefícios**: Pagando, mantém todos os serviços

---

## 🏗️ **PARTE 1: ARQUITETURA DO SISTEMA DE HOUSING**

### **1.1 Visão Geral da Arquitetura**

O Housing System é como uma **fábrica de casas digitais** com várias estações de trabalho:

```
🏭 HOUSING FACTORY
├── 📍 Plot Manager (Gerencia terrenos)
├── 🏗️ Construction Manager (Gerencia construção)  
├── 📋 Validation Manager (Valida construções)
├── 💰 Tax Manager (Gerencia impostos)
├── 🎨 Decoration Manager (Gerencia decoração)
└── 🔐 Permission Manager (Gerencia permissões)
```

Cada "estação" tem sua responsabilidade específica, mas todas trabalham juntas.

### **1.2 Componentes Fundamentais**

#### **🏞️ Plot (Terreno)**
É como um **lote de terra** que você compra:
- **Coordenadas fixas** no mundo
- **Tamanho definido** (pequeno, médio, grande)
- **Tipo específico** (residencial, comercial, agrícola)
- **Proprietário único** (só uma pessoa pode ter)

#### **🏠 House (Casa)**
É a **construção principal** no terreno:
- **Fundação obrigatória** (como base de LEGO)
- **Paredes conectadas** (formam rooms)
- **Teto protetor** (completa a estrutura)
- **Portas e janelas** (acesso e ventilação)

#### **🎨 Decorations (Decorações)**
São os **objetos menores** dentro da casa:
- **Móveis** (camas, mesas, cadeiras)
- **Decorativos** (quadros, vasos, tapetes)
- **Funcionais** (baús, forjas, bancadas)
- **Iluminação** (tochas, lustres, velas)

#### **🌱 Farm (Fazenda)**
É a **área agrícola** do terreno:
- **Canteiros** para plantar
- **Árvores frutíferas**
- **Animais** (galinhas, porcos, vacas)
- **Equipamentos** (poços, moinhos)

### **1.3 Fluxo de Vida de uma Casa**

Uma casa passa pelas seguintes fases:

```
1. 📍 SELEÇÃO DE TERRENO
   ↓ Jogador escolhe um plot vazio
   
2. 💰 COMPRA DO TERRENO  
   ↓ Paga o valor e se torna proprietário
   
3. 📐 PLANEJAMENTO
   ↓ Decide o que vai construir
   
4. 🏗️ CONSTRUÇÃO
   ↓ Coloca peças uma por uma
   
5. 📋 VALIDAÇÃO
   ↓ Sistema verifica se está correto
   
6. 🎨 DECORAÇÃO
   ↓ Adiciona móveis e decorações
   
7. 🔓 HABITAÇÃO
   ↓ Casa fica funcional para uso
   
8. 💰 MANUTENÇÃO
   ↓ Paga impostos regulares
   
9. 🔄 EVOLUÇÃO
   ↓ Expande, reforma, melhora
   
10. 💀 DEMOLIÇÃO (opcional)
    ↓ Remove tudo e recupera materiais
```

---

## 🏗️ **PARTE 2: IMPLEMENTAÇÃO TÉCNICA COMPLETA**

### **2.1 Classe Plot - O Terreno Base**

```csharp
/// <summary>
/// Representa um terreno onde o jogador pode construir
/// É como um "lote de terra" com regras específicas
/// </summary>
public class Plot
{
    #region Identificação Básica
    /// <summary>
    /// ID único do plot no mundo
    /// Como o "número do lote" na prefeitura
    /// </summary>
    public uint Id { get; set; }
    
    /// <summary>
    /// Nome amigável do plot
    /// Ex: "Terreno da Colina Verde"
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Posição central do terreno no mundo
    /// Como o "endereço" no mapa
    /// </summary>
    public Vector3 CenterPosition { get; set; }
    
    /// <summary>
    /// Zona onde o plot está localizado
    /// Como o "bairro" da cidade
    /// </summary>
    public uint ZoneId { get; set; }
    #endregion
    
    #region Propriedades Físicas
    /// <summary>
    /// Tipo do terreno (residencial, comercial, agrícola)
    /// Define o que pode ser construído
    /// </summary>
    public PlotType Type { get; set; }
    
    /// <summary>
    /// Tamanho do terreno
    /// Como "pequeno, médio, grande"
    /// </summary>
    public PlotSize Size { get; set; }
    
    /// <summary>
    /// Forma geométrica do terreno
    /// Retângulo com largura e altura
    /// </summary>
    public PlotShape Shape { get; set; }
    
    /// <summary>
    /// Pontos que definem os limites do terreno
    /// Como "cerca virtual" que marca o limite
    /// </summary>
    public List<Vector3> BoundaryPoints { get; set; } = new();
    
    /// <summary>
    /// Área total em metros quadrados
    /// Calculada automaticamente baseada nos pontos
    /// </summary>
    public float TotalArea => CalculateArea();
    
    /// <summary>
    /// Elevação do terreno (altura mínima e máxima)
    /// Para validar se construção não flutua
    /// </summary>
    public float MinElevation { get; set; }
    public float MaxElevation { get; set; }
    #endregion
    
    #region Propriedade e Ownership
    /// <summary>
    /// ID do jogador que possui este terreno
    /// null = terreno livre para compra
    /// </summary>
    public uint? OwnerId { get; set; }
    
    /// <summary>
    /// Nome do proprietário atual
    /// Para exibição na interface
    /// </summary>
    public string OwnerName { get; set; }
    
    /// <summary>
    /// Quando o terreno foi comprado
    /// Para calcular tempo de posse
    /// </summary>
    public DateTime? PurchasedAt { get; set; }
    
    /// <summary>
    /// Valor pago pelo terreno
    /// Para histórico e cálculo de impostos
    /// </summary>
    public uint PurchasePrice { get; set; }
    
    /// <summary>
    /// Se o terreno está disponível para venda
    /// Proprietário pode colocar à venda
    /// </summary>
    public bool IsForSale { get; set; }
    
    /// <summary>
    /// Preço de venda definido pelo proprietário
    /// Se IsForSale = true
    /// </summary>
    public uint SalePrice { get; set; }
    #endregion
    
    #region Sistema de Impostos
    /// <summary>
    /// Valor do imposto por período
    /// Calculado baseado no tamanho e construções
    /// </summary>
    public uint TaxAmount { get; set; }
    
    /// <summary>
    /// Frequência de cobrança do imposto
    /// Ex: Weekly, Monthly
    /// </summary>
    public TaxPeriod TaxPeriod { get; set; }
    
    /// <summary>
    /// Próxima data de vencimento do imposto
    /// Se não pagar, terreno pode ser perdido
    /// </summary>
    public DateTime NextTaxDate { get; set; }
    
    /// <summary>
    /// Quantos períodos em atraso
    /// Após X períodos, terreno é confiscado
    /// </summary>
    public int TaxPeriodsOverdue { get; set; }
    
    /// <summary>
    /// Se o terreno está com impostos em dia
    /// </summary>
    public bool IsTaxCurrent => TaxPeriodsOverdue == 0;
    #endregion
    
    #region Restrições e Regras
    /// <summary>
    /// Número máximo de casas permitidas
    /// Pequeno=1, Médio=2, Grande=5, etc.
    /// </summary>
    public byte MaxHouses { get; set; }
    
    /// <summary>
    /// Área máxima que pode ser construída
    /// Em porcentagem da área total (ex: 60%)
    /// </summary>
    public float MaxBuildableAreaPercent { get; set; } = 0.6f;
    
    /// <summary>
    /// Altura máxima das construções
    /// Para não fazer arranha-céus em área residencial
    /// </summary>
    public float MaxBuildHeight { get; set; }
    
    /// <summary>
    /// Distância mínima da borda do terreno
    /// Setback obrigatório para construções
    /// </summary>
    public float MinBorderDistance { get; set; }
    
    /// <summary>
    /// Lista de tipos de construção permitidos
    /// Ex: House, Shop, Farm, Warehouse
    /// </summary>
    public List<BuildingType> AllowedBuildingTypes { get; set; } = new();
    
    /// <summary>
    /// Regras especiais específicas deste plot
    /// Ex: "Apenas construções de madeira"
    /// </summary>
    public List<string> SpecialRules { get; set; } = new();
    #endregion
    
    #region Estado Atual
    /// <summary>
    /// Lista de todas as construções no terreno
    /// Houses, decorations, farms, etc.
    /// </summary>
    public List<Construction> Constructions { get; set; } = new();
    
    /// <summary>
    /// Área total já construída
    /// Soma de todas as construções
    /// </summary>
    public float BuiltArea => Constructions.Sum(c => c.FootprintArea);
    
    /// <summary>
    /// Área ainda disponível para construir
    /// </summary>
    public float AvailableArea => (TotalArea * MaxBuildableAreaPercent) - BuiltArea;
    
    /// <summary>
    /// Se há espaço para mais construções
    /// </summary>
    public bool HasSpaceForBuilding => AvailableArea > 0;
    
    /// <summary>
    /// Número de casas atualmente construídas
    /// </summary>
    public int CurrentHouseCount => Constructions.Count(c => c.Type == ConstructionType.House);
    
    /// <summary>
    /// Se pode construir mais casas
    /// </summary>
    public bool CanBuildMoreHouses => CurrentHouseCount < MaxHouses;
    #endregion
    
    #region Permissões e Acesso
    /// <summary>
    /// Lista de jogadores com permissões especiais
    /// Ex: amigos que podem entrar, decorar, etc.
    /// </summary>
    public List<PlotPermission> Permissions { get; set; } = new();
    
    /// <summary>
    /// Se outros jogadores podem visitar
    /// Public, Private, Friends Only
    /// </summary>
    public PlotAccessLevel AccessLevel { get; set; } = PlotAccessLevel.Public;
    
    /// <summary>
    /// Senha para acesso (se AccessLevel = Password)
    /// </summary>
    public string AccessPassword { get; set; }
    
    /// <summary>
    /// Lista de jogadores banidos do terreno
    /// </summary>
    public List<uint> BannedPlayerIds { get; set; } = new();
    #endregion
    
    #region Métodos de Validação
    /// <summary>
    /// Verifica se jogador pode construir algo neste plot
    /// </summary>
    public bool CanPlayerBuild(Character player, ConstructionType constructionType, Vector3 position, Vector3 size)
    {
        // Verifica ownership
        if (OwnerId != player.Id)
        {
            // Verifica permissões
            var permission = Permissions.FirstOrDefault(p => p.PlayerId == player.Id);
            if (permission == null || !permission.CanBuild)
            {
                Logger.Debug($"Player {player.Name} has no build permission on plot {Id}");
                return false;
            }
        }
        
        // Verifica impostos em dia
        if (!IsTaxCurrent)
        {
            Logger.Debug($"Plot {Id} has overdue taxes, cannot build");
            return false;
        }
        
        // Verifica tipo de construção permitido
        if (!AllowedBuildingTypes.Contains(GetBuildingType(constructionType)))
        {
            Logger.Debug($"Construction type {constructionType} not allowed on plot {Id}");
            return false;
        }
        
        // Verifica se está dentro dos limites
        if (!IsWithinBoundaries(position, size))
        {
            Logger.Debug($"Construction position outside plot boundaries");
            return false;
        }
        
        // Verifica setback das bordas
        if (!RespectsBorderDistance(position, size))
        {
            Logger.Debug($"Construction too close to plot border");
            return false;
        }
        
        // Verifica área disponível
        var constructionArea = size.X * size.Y;
        if (constructionArea > AvailableArea)
        {
            Logger.Debug($"Not enough area available: needs {constructionArea}, has {AvailableArea}");
            return false;
        }
        
        // Verifica altura máxima
        if (size.Z > MaxBuildHeight)
        {
            Logger.Debug($"Construction too tall: {size.Z} > {MaxBuildHeight}");
            return false;
        }
        
        // Verifica número máximo de casas
        if (constructionType == ConstructionType.House && !CanBuildMoreHouses)
        {
            Logger.Debug($"Maximum number of houses reached: {CurrentHouseCount}/{MaxHouses}");
            return false;
        }
        
        // Verifica sobreposição com construções existentes
        if (OverlapsWithExistingConstruction(position, size))
        {
            Logger.Debug($"Construction overlaps with existing building");
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Verifica se posição está dentro dos limites do terreno
    /// </summary>
    private bool IsWithinBoundaries(Vector3 position, Vector3 size)
    {
        // Cria retângulo da construção
        var constructionBounds = new RectangleF(
            position.X - size.X / 2,
            position.Y - size.Y / 2,
            size.X,
            size.Y
        );
        
        // Verifica se está completamente dentro do plot
        return IsPointInside(constructionBounds.Left, constructionBounds.Top) &&
               IsPointInside(constructionBounds.Right, constructionBounds.Top) &&
               IsPointInside(constructionBounds.Left, constructionBounds.Bottom) &&
               IsPointInside(constructionBounds.Right, constructionBounds.Bottom);
    }
    
    /// <summary>
    /// Verifica se ponto está dentro do polígono do plot
    /// Usa algoritmo ray casting
    /// </summary>
    private bool IsPointInside(float x, float y)
    {
        int intersections = 0;
        
        for (int i = 0; i < BoundaryPoints.Count; i++)
        {
            var p1 = BoundaryPoints[i];
            var p2 = BoundaryPoints[(i + 1) % BoundaryPoints.Count];
            
            if (((p1.Y > y) != (p2.Y > y)) &&
                (x < (p2.X - p1.X) * (y - p1.Y) / (p2.Y - p1.Y) + p1.X))
            {
                intersections++;
            }
        }
        
        return (intersections % 2) == 1;
    }
    
    /// <summary>
    /// Verifica distância mínima das bordas
    /// </summary>
    private bool RespectsBorderDistance(Vector3 position, Vector3 size)
    {
        // Para simplificar, assume plot retangular
        // Em implementação real, calcularia distância até polígono
        
        var plotBounds = GetBoundingRectangle();
        var constructionBounds = new RectangleF(
            position.X - size.X / 2,
            position.Y - size.Y / 2,
            size.X,
            size.Y
        );
        
        // Verifica distância de cada borda
        var leftDistance = constructionBounds.Left - plotBounds.Left;
        var rightDistance = plotBounds.Right - constructionBounds.Right;
        var topDistance = constructionBounds.Top - plotBounds.Top;
        var bottomDistance = plotBounds.Bottom - constructionBounds.Bottom;
        
        return leftDistance >= MinBorderDistance &&
               rightDistance >= MinBorderDistance &&
               topDistance >= MinBorderDistance &&
               bottomDistance >= MinBorderDistance;
    }
    
    /// <summary>
    /// Verifica sobreposição com construções existentes
    /// </summary>
    private bool OverlapsWithExistingConstruction(Vector3 position, Vector3 size)
    {
        var newBounds = new RectangleF(
            position.X - size.X / 2,
            position.Y - size.Y / 2,
            size.X,
            size.Y
        );
        
        foreach (var construction in Constructions)
        {
            var existingBounds = new RectangleF(
                construction.Position.X - construction.Size.X / 2,
                construction.Position.Y - construction.Size.Y / 2,
                construction.Size.X,
                construction.Size.Y
            );
            
            if (newBounds.IntersectsWith(existingBounds))
                return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Calcula área total do plot baseada nos pontos de contorno
    /// Usa fórmula Shoelace
    /// </summary>
    private float CalculateArea()
    {
        if (BoundaryPoints.Count < 3) return 0;
        
        float area = 0;
        int n = BoundaryPoints.Count;
        
        for (int i = 0; i < n; i++)
        {
            int j = (i + 1) % n;
            area += BoundaryPoints[i].X * BoundaryPoints[j].Y;
            area -= BoundaryPoints[j].X * BoundaryPoints[i].Y;
        }
        
        return Math.Abs(area) / 2.0f;
    }
    
    /// <summary>
    /// Retorna retângulo que engloba todo o plot
    /// </summary>
    private RectangleF GetBoundingRectangle()
    {
        if (!BoundaryPoints.Any()) return RectangleF.Empty;
        
        var minX = BoundaryPoints.Min(p => p.X);
        var maxX = BoundaryPoints.Max(p => p.X);
        var minY = BoundaryPoints.Min(p => p.Y);
        var maxY = BoundaryPoints.Max(p => p.Y);
        
        return new RectangleF(minX, minY, maxX - minX, maxY - minY);
    }
    #endregion
    
    #region Métodos de Impostos
    /// <summary>
    /// Calcula valor do imposto baseado no terreno e construções
    /// </summary>
    public uint CalculateTaxAmount()
    {
        // Taxa base por área
        var baseTax = TotalArea * GetBaseTaxRatePerSquareMeter();
        
        // Taxa adicional por construções
        var constructionTax = 0u;
        foreach (var construction in Constructions)
        {
            constructionTax += construction.GetTaxValue();
        }
        
        // Multiplicador por tipo de zona
        var zoneMultiplier = GetZoneTaxMultiplier();
        
        return (uint)((baseTax + constructionTax) * zoneMultiplier);
    }
    
    /// <summary>
    /// Processa pagamento de imposto
    /// </summary>
    public bool PayTax(Character player, uint amount)
    {
        if (player.Id != OwnerId) return false;
        if (amount < TaxAmount) return false;
        if (!player.Currency.CanAfford(amount)) return false;
        
        // Deduz dinheiro
        player.Currency.Spend(amount);
        
        // Atualiza próxima data
        NextTaxDate = CalculateNextTaxDate();
        
        // Zera atraso
        TaxPeriodsOverdue = 0;
        
        Logger.Info($"Player {player.Name} paid tax {amount} for plot {Id}");
        
        return true;
    }
    
    /// <summary>
    /// Processa multa por atraso no imposto
    /// </summary>
    public void ProcessTaxOverdue()
    {
        if (DateTime.Now >= NextTaxDate)
        {
            TaxPeriodsOverdue++;
            NextTaxDate = CalculateNextTaxDate();
            
            Logger.Warning($"Plot {Id} tax overdue, periods: {TaxPeriodsOverdue}");
            
            // Se muito atrasado, confisca o terreno
            if (TaxPeriodsOverdue >= 3) // 3 períodos = confisco
            {
                ConfiscatePlot();
            }
        }
    }
    
    /// <summary>
    /// Confisca o terreno por falta de pagamento
    /// </summary>
    private void ConfiscatePlot()
    {
        Logger.Warning($"Confiscating plot {Id} due to unpaid taxes");
        
        // Remove todas as construções
        Constructions.Clear();
        
        // Remove ownership
        OwnerId = null;
        OwnerName = null;
        PurchasedAt = null;
        PurchasePrice = 0;
        
        // Reset flags
        IsForSale = false;
        SalePrice = 0;
        TaxPeriodsOverdue = 0;
        
        // Limpa permissões
        Permissions.Clear();
        BannedPlayerIds.Clear();
        
        // Volta para público
        AccessLevel = PlotAccessLevel.Public;
        AccessPassword = null;
    }
    #endregion
    
    #region Métodos de Utilidade
    /// <summary>
    /// Retorna informações resumidas do plot
    /// </summary>
    public PlotInfo GetInfo()
    {
        return new PlotInfo
        {
            Id = Id,
            Name = Name,
            Type = Type,
            Size = Size,
            Area = TotalArea,
            OwnerName = OwnerName,
            IsOwned = OwnerId.HasValue,
            IsForSale = IsForSale,
            SalePrice = SalePrice,
            TaxAmount = TaxAmount,
            NextTaxDate = NextTaxDate,
            IsTaxCurrent = IsTaxCurrent,
            ConstructionCount = Constructions.Count,
            AvailableArea = AvailableArea
        };
    }
    
    /// <summary>
    /// Exporta dados para cliente
    /// </summary>
    public PlotClientData ToClientData()
    {
        return new PlotClientData
        {
            Id = Id,
            Name = Name,
            CenterPosition = CenterPosition,
            BoundaryPoints = BoundaryPoints,
            Type = Type,
            Size = Size,
            OwnerName = OwnerName,
            IsForSale = IsForSale,
            SalePrice = SalePrice,
            AccessLevel = AccessLevel,
            Constructions = Constructions.Select(c => c.ToClientData()).ToList()
        };
    }
    #endregion
}

/// <summary>
/// Tipos de terreno disponíveis
/// </summary>
public enum PlotType
{
    Residential,    // Residencial - casas familiares
    Commercial,     // Comercial - lojas e negócios
    Agricultural,   // Agrícola - fazendas e plantações
    Industrial,     // Industrial - workshops e manufatura
    Mixed,          // Misto - combinação de usos
    Special         // Especial - eventos, guildas, etc.
}

/// <summary>
/// Tamanhos de terreno
/// </summary>
public enum PlotSize
{
    Tiny,       // 8x8 metros
    Small,      // 16x16 metros  
    Medium,     // 24x24 metros
    Large,      // 32x32 metros
    Huge,       // 44x44 metros
    Mansion     // 64x64 metros
}

/// <summary>
/// Formas geométricas do terreno
/// </summary>
public enum PlotShape
{
    Square,         // Quadrado
    Rectangle,      // Retângulo
    Circle,         // Círculo
    Triangle,       // Triângulo
    Hexagon,        // Hexágono
    Irregular       // Forma irregular
}

/// <summary>
/// Períodos de cobrança de imposto
/// </summary>
public enum TaxPeriod
{
    Daily,      // Diário (para testes)
    Weekly,     // Semanal
    Monthly,    // Mensal
    Quarterly   // Trimestral
}

/// <summary>
/// Níveis de acesso ao terreno
/// </summary>
public enum PlotAccessLevel
{
    Public,         // Público - qualquer um pode entrar
    FriendsOnly,    // Só amigos podem entrar
    GuildOnly,      // Só membros da guilda
    Private,        // Só o dono pode entrar
    Password        // Requer senha
}

/// <summary>
/// Permissão específica de um jogador no terreno
/// </summary>
public class PlotPermission
{
    public uint PlayerId { get; set; }
    public string PlayerName { get; set; }
    public bool CanEnter { get; set; } = true;
    public bool CanBuild { get; set; } = false;
    public bool CanDecorate { get; set; } = false;
    public bool CanHarvest { get; set; } = false;
    public bool CanManagePermissions { get; set; } = false;
    public DateTime GrantedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    
    public bool IsValid => !ExpiresAt.HasValue || DateTime.Now < ExpiresAt.Value;
}

/// <summary>
/// Informações resumidas do plot
/// </summary>
public class PlotInfo
{
    public uint Id { get; set; }
    public string Name { get; set; }
    public PlotType Type { get; set; }
    public PlotSize Size { get; set; }
    public float Area { get; set; }
    public string OwnerName { get; set; }
    public bool IsOwned { get; set; }
    public bool IsForSale { get; set; }
    public uint SalePrice { get; set; }
    public uint TaxAmount { get; set; }
    public DateTime NextTaxDate { get; set; }
    public bool IsTaxCurrent { get; set; }
    public int ConstructionCount { get; set; }
    public float AvailableArea { get; set; }
}

/// <summary>
/// Dados do plot para envio ao cliente
/// </summary>
public class PlotClientData
{
    public uint Id { get; set; }
    public string Name { get; set; }
    public Vector3 CenterPosition { get; set; }
    public List<Vector3> BoundaryPoints { get; set; }
    public PlotType Type { get; set; }
    public PlotSize Size { get; set; }
    public string OwnerName { get; set; }
    public bool IsForSale { get; set; }
    public uint SalePrice { get; set; }
    public PlotAccessLevel AccessLevel { get; set; }
    public List<ConstructionClientData> Constructions { get; set; }
}
```

---

## 🏠 **PARTE 3: SISTEMA DE CONSTRUÇÃO**

### **3.1 A Classe Construction - Construções do Terreno**

```csharp
/// <summary>
/// Representa qualquer construção dentro de um plot
/// Como uma "peça de LEGO" no terreno
/// </summary>
public class Construction
{
    #region Identificação
    /// <summary>
    /// ID único da construção
    /// </summary>
    public uint Id { get; set; }
    
    /// <summary>
    /// ID do template desta construção
    /// Define aparência, tamanho, propriedades
    /// </summary>
    public uint TemplateId { get; set; }
    
    /// <summary>
    /// Tipo específico de construção
    /// </summary>
    public ConstructionType Type { get; set; }
    
    /// <summary>
    /// Nome personalizado (opcional)
    /// Ex: "Casa da Família Silva"
    /// </summary>
    public string CustomName { get; set; }
    #endregion
    
    #region Posicionamento
    /// <summary>
    /// Posição no mundo (centro da construção)
    /// </summary>
    public Vector3 Position { get; set; }
    
    /// <summary>
    /// Rotação em graus (0-360)
    /// </summary>
    public float Rotation { get; set; }
    
    /// <summary>
    /// Tamanho da construção (largura, profundidade, altura)
    /// </summary>
    public Vector3 Size { get; set; }
    
    /// <summary>
    /// Área da "pegada" no terreno
    /// </summary>
    public float FootprintArea => Size.X * Size.Y;
    #endregion
    
    #region Propriedade
    /// <summary>
    /// Plot onde esta construção está localizada
    /// </summary>
    public uint PlotId { get; set; }
    
    /// <summary>
    /// Jogador que construiu
    /// </summary>
    public uint BuilderId { get; set; }
    
    /// <summary>
    /// Quando foi construída
    /// </summary>
    public DateTime BuiltAt { get; set; }
    #endregion
    
    #region Estado
    /// <summary>
    /// Fase atual da construção
    /// </summary>
    public ConstructionPhase Phase { get; set; }
    
    /// <summary>
    /// Progresso da construção (0.0 a 1.0)
    /// </summary>
    public float Progress { get; set; }
    
    /// <summary>
    /// HP atual da construção
    /// Pode ser danificada e reparada
    /// </summary>
    public uint CurrentHitPoints { get; set; }
    
    /// <summary>
    /// HP máximo baseado no template
    /// </summary>
    public uint MaxHitPoints { get; set; }
    
    /// <summary>
    /// Se a construção está funcional
    /// </summary>
    public bool IsFunctional => Phase == ConstructionPhase.Complete && CurrentHitPoints > 0;
    #endregion
    
    #region Recursos e Custos
    /// <summary>
    /// Materiais usados na construção
    /// Para cálculo de impostos e valor
    /// </summary>
    public List<ConstructionMaterial> MaterialsUsed { get; set; } = new();
    
    /// <summary>
    /// Valor total investido
    /// Soma dos materiais + mão de obra
    /// </summary>
    public uint TotalValue => CalculateTotalValue();
    
    /// <summary>
    /// Custo de manutenção diário
    /// </summary>
    public uint MaintenanceCost { get; set; }
    #endregion
    
    #region Funcionalidades
    /// <summary>
    /// Componentes funcionais desta construção
    /// Ex: cama (pode dormir), forja (pode craftar)
    /// </summary>
    public List<ConstructionComponent> Components { get; set; } = new();
    
    /// <summary>
    /// Inventário interno (se aplicável)
    /// Para armazéns, baús, etc.
    /// </summary>
    public Container InternalStorage { get; set; }
    
    /// <summary>
    /// Permissões específicas desta construção
    /// </summary>
    public List<ConstructionPermission> Permissions { get; set; } = new();
    #endregion
    
    #region Métodos Principais
    /// <summary>
    /// Avança o progresso da construção
    /// </summary>
    public bool AddProgress(Character builder, float progressAmount)
    {
        // Verifica se pode construir
        if (Phase == ConstructionPhase.Complete) return false;
        if (!CanPlayerBuild(builder)) return false;
        
        // Adiciona progresso
        Progress = Math.Min(Progress + progressAmount, 1.0f);
        
        // Verifica se completou
        if (Progress >= 1.0f)
        {
            CompleteConstruction();
        }
        
        return true;
    }
    
    /// <summary>
    /// Finaliza a construção
    /// </summary>
    private void CompleteConstruction()
    {
        Phase = ConstructionPhase.Complete;
        Progress = 1.0f;
        CurrentHitPoints = MaxHitPoints;
        
        // Ativa componentes funcionais
        foreach (var component in Components)
        {
            component.Activate();
        }
        
        Logger.Info($"Construction {Id} completed on plot {PlotId}");
    }
    
    /// <summary>
    /// Verifica se jogador pode construir/modificar
    /// </summary>
    private bool CanPlayerBuild(Character player)
    {
        // Se é o construtor original
        if (BuilderId == player.Id) return true;
        
        // Verifica permissões no plot
        var plot = PlotManager.GetPlot(PlotId);
        if (plot?.OwnerId == player.Id) return true;
        
        // Verifica permissões específicas
        var permission = Permissions.FirstOrDefault(p => p.PlayerId == player.Id);
        return permission?.CanModify == true;
    }
    
    /// <summary>
    /// Calcula valor total da construção
    /// </summary>
    private uint CalculateTotalValue()
    {
        var materialValue = MaterialsUsed.Sum(m => m.Quantity * m.UnitValue);
        var laborValue = (uint)(materialValue * 0.3f); // 30% de mão de obra
        return materialValue + laborValue;
    }
    
    /// <summary>
    /// Calcula valor do imposto para esta construção
    /// </summary>
    public uint GetTaxValue()
    {
        var baseValue = TotalValue * 0.01f; // 1% do valor como base
        var sizeMultiplier = FootprintArea / 100f; // Por 100m²
        return (uint)(baseValue * sizeMultiplier);
    }
    
    /// <summary>
    /// Demole a construção
    /// </summary>
    public List<ConstructionMaterial> Demolish(Character player)
    {
        if (!CanPlayerBuild(player)) return new List<ConstructionMaterial>();
        
        // Calcula materiais recuperados (70% do original)
        var recoveredMaterials = new List<ConstructionMaterial>();
        foreach (var material in MaterialsUsed)
        {
            var recovered = new ConstructionMaterial
            {
                ItemId = material.ItemId,
                Quantity = (uint)(material.Quantity * 0.7f),
                UnitValue = material.UnitValue
            };
            recoveredMaterials.Add(recovered);
        }
        
        // Remove do plot
        var plot = PlotManager.GetPlot(PlotId);
        plot?.Constructions.Remove(this);
        
        Logger.Info($"Construction {Id} demolished by {player.Name}");
        
        return recoveredMaterials;
    }
    
    /// <summary>
    /// Repara danos na construção
    /// </summary>
    public bool Repair(Character player, List<ConstructionMaterial> materials)
    {
        if (!CanPlayerBuild(player)) return false;
        if (CurrentHitPoints >= MaxHitPoints) return false;
        
        // Calcula quanto pode reparar com os materiais
        var repairAmount = CalculateRepairAmount(materials);
        
        // Aplica reparo
        CurrentHitPoints = Math.Min(CurrentHitPoints + repairAmount, MaxHitPoints);
        
        // Consome materiais
        ConsumeMaterials(player, materials);
        
        Logger.Info($"Construction {Id} repaired by {player.Name}");
        
        return true;
    }
    #endregion
    
    #region Dados para Cliente
    /// <summary>
    /// Converte para dados que podem ser enviados ao cliente
    /// </summary>
    public ConstructionClientData ToClientData()
    {
        return new ConstructionClientData
        {
            Id = Id,
            TemplateId = TemplateId,
            Type = Type,
            CustomName = CustomName,
            Position = Position,
            Rotation = Rotation,
            Size = Size,
            Phase = Phase,
            Progress = Progress,
            CurrentHitPoints = CurrentHitPoints,
            MaxHitPoints = MaxHitPoints,
            Components = Components.Select(c => c.ToClientData()).ToList()
        };
    }
    #endregion
}

/// <summary>
/// Tipos de construção
/// </summary>
public enum ConstructionType
{
    // Habitação
    House,          // Casa residencial
    Mansion,        // Mansão grande
    Cottage,        // Chalé pequeno
    Apartment,      // Apartamento
    
    // Comercial
    Shop,           // Loja
    Warehouse,      // Armazém
    Workshop,       // Oficina
    Tavern,         // Taverna
    
    // Agrícola
    Farm,           // Fazenda
    Barn,           // Celeiro
    Greenhouse,     // Estufa
    Mill,           // Moinho
    
    // Especial
    Guild,          // Casa de guilda
    Temple,         // Templo
    Observatory,    // Observatório
    Library,        // Biblioteca
    
    // Decorativo
    Garden,         // Jardim
    Fountain,       // Fonte
    Statue,         // Estátua
    Bridge          // Ponte
}

/// <summary>
/// Fases da construção
/// </summary>
public enum ConstructionPhase
{
    Planning,       // Planejamento (ghost mode)
    Foundation,     // Fundação
    Framing,        // Estrutura
    Walls,          // Paredes
    Roofing,        // Telhado
    Finishing,      // Acabamentos
    Complete        // Completa
}

/// <summary>
/// Material usado na construção
/// </summary>
public class ConstructionMaterial
{
    public uint ItemId { get; set; }
    public uint Quantity { get; set; }
    public uint UnitValue { get; set; }
    public string ItemName { get; set; }
}

/// <summary>
/// Componente funcional de uma construção
/// </summary>
public class ConstructionComponent
{
    public uint Id { get; set; }
    public ComponentType Type { get; set; }
    public bool IsActive { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
    
    public void Activate()
    {
        IsActive = true;
        Logger.Debug($"Component {Type} activated");
    }
    
    public ComponentClientData ToClientData()
    {
        return new ComponentClientData
        {
            Id = Id,
            Type = Type,
            IsActive = IsActive,
            Properties = Properties
        };
    }
}

/// <summary>
/// Tipos de componente funcional
/// </summary>
public enum ComponentType
{
    // Funcionalidade básica
    Door,           // Porta (acesso)
    Window,         // Janela (luz)
    Stairs,         // Escada (subir/descer)
    
    // Armazenamento
    Chest,          // Baú
    Wardrobe,       // Guarda-roupa
    BookShelf,      // Estante
    
    // Produção
    Workbench,      // Bancada de trabalho
    Forge,          // Forja
    Alchemy,        // Mesa de alquimia
    Loom,           // Tear
    
    // Descanso
    Bed,            // Cama
    Chair,          // Cadeira
    Sofa,           // Sofá
    
    // Utilidade
    Fireplace,      // Lareira
    Well,           // Poço
    Mailbox,        // Caixa de correio
    Teleporter      // Teletransporte
}

/// <summary>
/// Permissão específica de construção
/// </summary>
public class ConstructionPermission
{
    public uint PlayerId { get; set; }
    public bool CanEnter { get; set; }
    public bool CanUse { get; set; }
    public bool CanModify { get; set; }
    public DateTime GrantedAt { get; set; }
}

/// <summary>
/// Dados de construção para cliente
/// </summary>
public class ConstructionClientData
{
    public uint Id { get; set; }
    public uint TemplateId { get; set; }
    public ConstructionType Type { get; set; }
    public string CustomName { get; set; }
    public Vector3 Position { get; set; }
    public float Rotation { get; set; }
    public Vector3 Size { get; set; }
    public ConstructionPhase Phase { get; set; }
    public float Progress { get; set; }
    public uint CurrentHitPoints { get; set; }
    public uint MaxHitPoints { get; set; }
    public List<ComponentClientData> Components { get; set; }
}

/// <summary>
/// Dados de componente para cliente
/// </summary>
public class ComponentClientData
{
    public uint Id { get; set; }
    public ComponentType Type { get; set; }
    public bool IsActive { get; set; }
    public Dictionary<string, object> Properties { get; set; }
}
```

---

## 🎓 **RESUMO DO MÓDULO 12**

Agora você viu o **Housing System** completo:
- ✅ Sistema de terrenos (plots) ultra-detalhado
- ✅ Validações complexas de construção
- ✅ Sistema de impostos automático
- ✅ Permissões granulares
- ✅ Construções modulares como LEGO
- ✅ Componentes funcionais
- ✅ Sistema de materiais e custos

O Housing System é como criar um **SimCity dentro do MMORPG** - os jogadores podem construir, personalizar e gerenciar suas propriedades virtuais!

**🎯 PRÓXIMO:** Módulo 13 - Sistema de Quests e NPCs!