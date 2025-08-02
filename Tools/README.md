# Ferramentas de Tradução pt-BR para AAEmu

Este diretório contém ferramentas para facilitar a implementação da tradução em português brasileiro no AAEmu.

## Arquivos Incluídos

### `translation_scripts.sql`
Scripts SQL para executar diretamente no banco de dados `compact.sqlite3`:
- Verificar estrutura da tabela
- Adicionar coluna pt_br
- Exportar itens sem tradução  
- Importar traduções
- Estatísticas de progresso

### `translation_helper.py`
Script Python para automação de tarefas de tradução:
- Configuração automática do banco
- Exportação para CSV
- Importação de traduções
- Estatísticas detalhadas
- Busca de textos

## Como Usar

### 1. Preparação
Primeiro, certifique-se de ter o arquivo `compact.sqlite3` do cliente ArcheAge na pasta `AAEmu.Game/Data/`.

### 2. Usando o Script Python

#### Configuração inicial:
```bash
cd Tools
python translation_helper.py --action setup
```

#### Exportar itens sem tradução:
```bash
python translation_helper.py --action export --file itens_para_traduzir.csv
```

#### Ver estatísticas:
```bash
python translation_helper.py --action stats
```

#### Buscar texto específico:
```bash
python translation_helper.py --action search --search "sword"
```

#### Importar traduções de CSV:
```bash
python translation_helper.py --action import --file traducoes_concluidas.csv
```

### 3. Usando Scripts SQL Diretamente

Com qualquer cliente SQLite (como DB Browser for SQLite):

1. Abra `compact.sqlite3`
2. Execute os scripts do arquivo `translation_scripts.sql`
3. Use os templates para adicionar traduções

### 4. Workflow Recomendado

1. **Setup inicial**:
   ```bash
   python translation_helper.py --action setup
   ```

2. **Exportar para tradução**:
   ```bash
   python translation_helper.py --action export --categories items npcs --file prioridade.csv
   ```

3. **Traduzir no Excel/LibreOffice**:
   - Abra o arquivo CSV
   - Preencha a coluna "Tradução_PT-BR"
   - Salve o arquivo

4. **Importar traduções**:
   ```bash
   python translation_helper.py --action import --file prioridade.csv
   ```

5. **Verificar progresso**:
   ```bash
   python translation_helper.py --action stats
   ```

6. **Configurar servidor**:
   - Edite `AAEmu.Game/Models/AppConfiguration.cs`
   - Mude `DefaultLanguage` para `"pt_br"`
   - Ou use `ExampleConfig-ptBR.json`

## Formato do CSV

O arquivo CSV exportado/importado tem o seguinte formato:

| Categoria | Coluna | ID | Texto_Inglês | Tradução_PT-BR |
|-----------|--------|----|--------------|----------------|
| items     | name   | 1  | Iron Sword   | Espada de Ferro |
| npcs      | name   | 10 | Merchant     | Mercador       |

## Categorias Principais

- **items**: Itens do jogo (armas, armaduras, consumíveis)
- **npcs**: NPCs e criaturas
- **buffs**: Buffs e debuffs
- **quest_contexts**: Nomes de quests
- **doodad_almighties**: Objetos interativos
- **bubble_effects**: Textos de balão
- **system_factions**: Facções

## Dicas

1. **Priorize**: Comece com itens e NPCs mais importantes
2. **Consistência**: Mantenha terminologia consistente
3. **Backup**: Sempre faça backup antes de modificar
4. **Teste**: Teste no servidor após cada lote de traduções
5. **Encoding**: Use UTF-8 para caracteres especiais (ã, ç, etc.)

## Requisitos

- Python 3.6+
- Cliente SQLite (para scripts manuais)
- Arquivo `compact.sqlite3` do cliente ArcheAge

## Troubleshooting

### "Arquivo compact.sqlite3 não encontrado"
- Certifique-se de que o arquivo está em `AAEmu.Game/Data/compact.sqlite3`
- Ou use `--db caminho/para/arquivo.sqlite3`

### "Coluna pt_br não existe"
- Execute: `python translation_helper.py --action setup`

### Traduções não aparecem no jogo
- Verifique se `DefaultLanguage` está configurado como `"pt_br"`
- Reinicie o servidor após mudanças de configuração
- Verifique os logs do servidor para erros

## Exemplo Completo

```bash
# 1. Setup inicial
python translation_helper.py --action setup

# 2. Exportar itens prioritários
python translation_helper.py --action export --categories items --file items.csv

# 3. (Traduzir manualmente no Excel/LibreOffice)

# 4. Importar traduções
python translation_helper.py --action import --file items.csv

# 5. Verificar progresso
python translation_helper.py --action stats

# 6. Configurar servidor para pt-BR e reiniciar
```