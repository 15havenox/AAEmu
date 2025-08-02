# Guia para Implementar Tradução Português Brasileiro (pt-BR) no AAEmu

## Visão Geral
O AAEmu utiliza um sistema de localização baseado em SQLite que carrega traduções de uma tabela `localized_texts` no arquivo `compact.sqlite3`. Este guia explica como adicionar suporte completo para português brasileiro.

## Estrutura do Sistema de Localização

### Como Funciona
1. **LocalizationManager**: Gerencia todas as traduções carregadas do banco SQLite
2. **Banco de Dados**: `Data/compact.sqlite3` contém a tabela `localized_texts`
3. **Configuração**: `DefaultLanguage` em `AppConfiguration.cs` define o idioma padrão
4. **Uso**: `LocalizationManager.Instance.Get("tabela", "coluna", id, "fallback")`

### Arquivos Principais
- `AAEmu.Game/Core/Managers/LocalizationManager.cs` - Gerenciador de traduções
- `AAEmu.Game/Models/AppConfiguration.cs` - Configuração do idioma padrão
- `Data/compact.sqlite3` - Banco de dados com traduções (do cliente ArcheAge)

## Passo a Passo para Implementar pt-BR

### 1. Obter o Arquivo de Dados do Cliente
```bash
# O arquivo compact.sqlite3 vem do cliente ArcheAge
# Coloque-o em: AAEmu.Game/Data/compact.sqlite3
# Ou configure em: AAEmu.Game/Configurations/ClientData.json
```

### 2. Verificar Estrutura da Tabela localized_texts
A tabela deve ter a seguinte estrutura:
```sql
CREATE TABLE localized_texts (
    tbl_name TEXT,           -- Nome da tabela (ex: "items", "npcs", "buffs")
    tbl_column_name TEXT,    -- Nome da coluna (ex: "name", "description")
    idx INTEGER,             -- ID do item
    en_us TEXT,              -- Texto em inglês
    pt_br TEXT,              -- Texto em português brasileiro (ADICIONAR SE NÃO EXISTIR)
    -- outras colunas de idiomas...
);
```

### 3. Adicionar Coluna pt_br (se não existir)
```sql
-- Se a coluna pt_br não existir, adicione:
ALTER TABLE localized_texts ADD COLUMN pt_br TEXT;
```

### 4. Configurar Idioma Padrão
Edite `AAEmu.Game/Models/AppConfiguration.cs`:
```csharp
public string DefaultLanguage { get; set; } = "pt_br"; // Mudou de "en_us" para "pt_br"
```

### 5. Traduzir Conteúdo
Popule a coluna `pt_br` com traduções. Exemplo:
```sql
-- Traduzir itens
UPDATE localized_texts 
SET pt_br = 'Espada de Ferro' 
WHERE tbl_name = 'items' AND tbl_column_name = 'name' AND idx = 1001;

-- Traduzir NPCs
UPDATE localized_texts 
SET pt_br = 'Mercador João' 
WHERE tbl_name = 'npcs' AND tbl_column_name = 'name' AND idx = 2001;
```

## Categorias de Tradução Principais

### Itens (`items`)
- `name` - Nome do item
- `description` - Descrição do item

### NPCs (`npcs`)
- `name` - Nome do NPC

### Buffs (`buffs`)
- `name` - Nome do buff/debuff

### Quests (`quest_contexts`)
- `name` - Nome da quest

### Outros
- `doodad_almighties` - Objetos interativos
- `bubble_effects` - Efeitos de balão/fala
- `fish_details` - Detalhes de peixes
- `slaves` - Escravos/servos
- `transfers` - Transferências
- `housings` - Habitações
- `system_factions` - Facções do sistema
- `return_points` - Pontos de retorno

## Scripts Úteis

### Script para Encontrar Itens Sem Tradução
```sql
SELECT tbl_name, tbl_column_name, idx, en_us
FROM localized_texts 
WHERE pt_br IS NULL OR pt_br = ''
ORDER BY tbl_name, idx;
```

### Script para Copiar Traduções de Inglês (Temporário)
```sql
-- Use apenas como base, substitua por traduções reais
UPDATE localized_texts 
SET pt_br = en_us 
WHERE pt_br IS NULL OR pt_br = '';
```

## Exemplo de Configuração Completa

### 1. Estrutura de Diretórios
```
AAEmu.Game/
├── Data/
│   └── compact.sqlite3        # Banco com traduções
├── Configurations/
│   └── ClientData.json        # Configuração de fonte de dados
└── Core/Managers/
    └── LocalizationManager.cs # Gerenciador de localização
```

### 2. Configuração do ClientData.json
```json
{
    "ClientData": {
        "Sources": [
            "ClientData",
            "ClientData/game_pak",
            "Data"
        ]
    }
}
```

### 3. Configuração no Config.json do Servidor
```json
{
    "DefaultLanguage": "pt_br"
}
```

## Testando a Tradução

1. **Verificar Carregamento**:
   - No log do servidor, procure: "Loaded X translations in pt_br"

2. **Testar no Jogo**:
   - Nomes de itens devem aparecer em português
   - Nomes de NPCs devem estar traduzidos
   - Mensagens do sistema em português

3. **Debug**:
   - Use `LocalizationManager.Instance.Get("items", "name", itemId)` para testar específicos

## Dicas Importantes

1. **Fallback**: Se não houver tradução pt_br, o sistema usa o valor fallback
2. **Performance**: Traduções são carregadas na memória na inicialização
3. **Encoding**: Use UTF-8 para caracteres especiais (ã, ç, etc.)
4. **Backup**: Sempre faça backup do compact.sqlite3 antes de modificar

## Estrutura de Comando de Tradução

Para automatizar traduções, você pode criar scripts SQL ou usar ferramentas como:

```sql
-- Template para tradução em lote
INSERT OR REPLACE INTO localized_texts (tbl_name, tbl_column_name, idx, pt_br)
VALUES 
('items', 'name', 1001, 'Espada de Ferro'),
('items', 'name', 1002, 'Escudo de Madeira'),
('npcs', 'name', 2001, 'Mercador João');
```

## Próximos Passos

1. Obtenha o arquivo `compact.sqlite3` do cliente ArcheAge
2. Configure o idioma padrão para `pt_br`
3. Adicione/verifique a coluna `pt_br` na tabela `localized_texts`
4. Comece traduzindo os itens mais importantes (armas, armaduras, NPCs principais)
5. Teste e refine as traduções

---

**Nota**: Este sistema mantém compatibilidade com outros idiomas e permite fallback automático quando traduções não estão disponíveis.