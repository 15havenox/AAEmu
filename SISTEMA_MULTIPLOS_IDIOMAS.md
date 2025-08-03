# Sistema de Múltiplos Idiomas por Cliente - AAEmu

## Resumo da Implementação

Agora você tem um sistema completo que permite:
1. **Idioma padrão do servidor** (definido em `DefaultLanguage`)
2. **Idioma individual por personagem** (armazenado no banco de dados)
3. **Detecção automática de idioma** (por IP geográfico)
4. **Comando para mudança manual** (`/setlang`, `/language`, `/idioma`)
5. **Sistema de fallback inteligente** (idioma preferido → padrão → inglês)

## Como Funciona

### 1. Definição do Idioma

**Prioridade (ordem de preferência):**
1. Idioma preferido do personagem (`PreferredLanguage`)
2. Idioma padrão do servidor (`DefaultLanguage`)
3. Inglês (`en_us`) como último recurso

### 2. Configuração Inicial

**No Banco de Dados:**
```sql
-- Execute este script no seu banco MySQL:
ALTER TABLE `characters` ADD COLUMN `preferred_language` VARCHAR(10) DEFAULT '' COMMENT 'Player preferred language (pt_br, en_us, etc)';
```

**Na Configuração do Servidor:**
```json
{
  "DefaultLanguage": "pt_br"  // Idioma padrão para novos players
}
```

### 3. Como os Jogadores Definem o Idioma

**Automaticamente (no primeiro login):**
- Sistema detecta país por IP geográfico
- Define idioma automaticamente baseado no país
- Brasil = `pt_br`, EUA = `en_us`, etc.

**Manualmente (comando no jogo):**
```
/setlang pt_br     # Define português brasileiro
/language en_us    # Define inglês americano  
/idioma es_es      # Define espanhol
/setlang           # Mostra idioma atual e disponíveis
```

## Configuração Completa

### 1. Banco de Dados SQLite (compact.sqlite3)

Estrutura necessária na tabela `localized_texts`:
```sql
CREATE TABLE localized_texts (
    tbl_name TEXT,           -- Categoria (items, npcs, etc)
    tbl_column_name TEXT,    -- Campo (name, description)
    idx INTEGER,             -- ID do item
    en_us TEXT,              -- Inglês americano
    pt_br TEXT,              -- Português brasileiro
    es_es TEXT,              -- Espanhol (opcional)
    fr_fr TEXT,              -- Francês (opcional)
    de_de TEXT,              -- Alemão (opcional)
    ru_ru TEXT,              -- Russo (opcional)
    ko_kr TEXT,              -- Coreano (opcional)
    ja_jp TEXT,              -- Japonês (opcional)
    zh_cn TEXT,              -- Chinês simplificado (opcional)
    zh_tw TEXT               -- Chinês tradicional (opcional)
);
```

### 2. Configuração do Servidor

**Arquivo `Config.json`:**
```json
{
  "DefaultLanguage": "pt_br",
  "World": {
    "Name": "Servidor Brasil - AAEmu"
  }
  // ... outras configurações
}
```

### 3. Como o Sistema Escolhe o Idioma

**Para cada texto solicitado:**
1. **Verifica idioma preferido do personagem**
   - Se existe e está disponível → usa
2. **Fallback para idioma padrão do servidor**
   - Se diferente do preferido e disponível → usa  
3. **Fallback para inglês**
   - Se disponível → usa
4. **Valor fallback original**
   - Retorna o texto original como último recurso

## Exemplos de Uso

### No Código (para desenvolvedores)

```csharp
// Método antigo (usa idioma padrão do servidor)
var itemName = LocalizationManager.Instance.Get("items", "name", itemId, "Unknown Item");

// Método novo (usa idioma do personagem)
var itemName = LocalizationManager.Instance.Get(character, "items", "name", itemId, "Unknown Item");

// Método específico (força um idioma)
var itemName = LocalizationManager.Instance.Get("pt_br", "items", "name", itemId, "Unknown Item");
```

### Comandos no Jogo

```
# Ver idioma atual e idiomas disponíveis
/setlang

# Mudar para português brasileiro
/setlang pt_br

# Mudar para inglês
/language en_us

# Mudar para espanhol
/idioma es_es
```

## Cenários Práticos

### Cenário 1: Servidor Brasileiro
- **DefaultLanguage**: `"pt_br"`
- **Cliente do Brasil**: Auto-detecta `pt_br`
- **Cliente dos EUA**: Auto-detecta `en_us`, mas pode usar `/setlang pt_br`
- **Resultado**: Brasileiros veem em português, estrangeiros em inglês (mas podem mudar)

### Cenário 2: Servidor Internacional  
- **DefaultLanguage**: `"en_us"`
- **Clientes diversos**: Auto-detecta baseado no país
- **Resultado**: Cada jogador vê no seu idioma preferido automaticamente

### Cenário 3: Servidor Multilíngue
- **DefaultLanguage**: `"en_us"`
- **Suporte**: `pt_br`, `es_es`, `fr_fr`, `de_de`
- **Resultado**: Jogadores podem escolher entre 5 idiomas

## Detecção Automática de Idioma

### Por Localização Geográfica (IP)
O sistema mapeia países para idiomas:
```
Brasil (BR) → pt_br
Portugal (PT) → pt_br  
EUA (US) → en_us
Reino Unido (GB) → en_us
Espanha (ES) → es_es
México (MX) → es_es
França (FR) → fr_fr
Alemanha (DE) → de_de
Rússia (RU) → ru_ru
Coreia (KR) → ko_kr
Japão (JP) → ja_jp
China (CN) → zh_cn
Taiwan (TW) → zh_tw
```

### Por Análise de Texto
Detecta idioma baseado em:
- **Caracteres especiais**: ã, ç, õ (português), Cyrillic (russo), etc.
- **Palavras comuns**: "não", "você" (português), "que", "con" (espanhol), etc.

## Vantagens do Sistema

### Para Administradores
- ✅ **Flexibilidade**: Servidor pode ter idioma padrão diferente do inglês
- ✅ **Automático**: Jogadores são configurados automaticamente no primeiro login
- ✅ **Backwards Compatible**: Funciona com código existente
- ✅ **Performance**: Traduções carregadas uma vez na memória

### Para Jogadores
- ✅ **Personalização**: Cada jogador pode ter seu idioma preferido
- ✅ **Facilidade**: Comando simples para mudança
- ✅ **Automático**: Detecta idioma baseado na localização
- ✅ **Fallback Inteligente**: Sempre mostra algo, mesmo sem tradução

## Monitoramento e Logs

O sistema gera logs informativos:
```
[INFO] Loaded 15847 translations across 5 languages: en_us, pt_br, es_es, fr_fr, de_de
[INFO] Auto-detected language pt_br for IP 200.123.45.67 (Country: BR)
[INFO] Auto-set language to pt_br for character João based on IP
[INFO] Character Maria changed language from en_us to pt_br
```

## Troubleshooting

### "Traduções não aparecem"
1. Verifique se a coluna `pt_br` existe na tabela `localized_texts`
2. Confirme que há dados na coluna (não NULL/vazio)
3. Verifique se `DefaultLanguage` está configurado corretamente

### "Comando /setlang não funciona"
1. Verifique se o script `SetLanguage.cs` foi carregado
2. Confirme que a coluna `preferred_language` foi adicionada à tabela `characters`

### "Detecção automática não funciona"
1. IPs locais (127.0.0.1, 192.168.x.x) usam configuração padrão
2. Para produção, considere integrar com serviço real de geolocalização

## Expandindo o Sistema

### Adicionando Novos Idiomas
1. **No SQLite**: `ALTER TABLE localized_texts ADD COLUMN it_it TEXT;`
2. **No código**: Adicione mapeamento no `LanguageDetectionManager`
3. **Populate**: Adicione traduções na nova coluna

### Integrando Geolocalização Real
```csharp
// Substitua GetCountryCodeFromIP() por:
private async Task<string> GetCountryCodeFromIP(IPAddress ipAddress)
{
    var client = new HttpClient();
    var response = await client.GetAsync($"http://ip-api.com/json/{ipAddress}?fields=countryCode");
    var json = await response.Content.ReadAsStringAsync();
    var data = JsonSerializer.Deserialize<dynamic>(json);
    return data.countryCode;
}
```

---

## Conclusão

Agora você tem um sistema completo de múltiplos idiomas que:
- **Detecta automaticamente** o idioma do jogador
- **Permite mudança manual** via comandos
- **Mantém compatibilidade** com código existente  
- **Oferece fallbacks inteligentes** para traduções ausentes
- **É facilmente expansível** para novos idiomas

Cada jogador pode ter sua experiência personalizada no idioma de sua preferência, enquanto o servidor mantém um idioma padrão configurável!