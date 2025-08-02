-- Scripts SQL para Tradução pt-BR do AAEmu
-- Estes scripts devem ser executados no arquivo compact.sqlite3

-- 1. Verificar se a coluna pt_br existe
PRAGMA table_info(localized_texts);

-- 2. Adicionar coluna pt_br se ela não existir
-- (Execute apenas se a coluna não existir)
ALTER TABLE localized_texts ADD COLUMN pt_br TEXT;

-- 3. Verificar quantos itens precisam de tradução
SELECT 
    tbl_name,
    COUNT(*) as total_items,
    COUNT(pt_br) as translated_items,
    (COUNT(*) - COUNT(pt_br)) as missing_translations
FROM localized_texts 
GROUP BY tbl_name
ORDER BY missing_translations DESC;

-- 4. Listar itens mais importantes sem tradução (primeiros 50)
SELECT 
    tbl_name,
    tbl_column_name,
    idx,
    en_us,
    pt_br
FROM localized_texts 
WHERE (pt_br IS NULL OR pt_br = '') 
    AND tbl_name IN ('items', 'npcs', 'buffs', 'quest_contexts')
    AND tbl_column_name = 'name'
ORDER BY tbl_name, idx
LIMIT 50;

-- 5. Template para tradução de itens básicos
-- (Substitua os valores pelos corretos)
INSERT OR REPLACE INTO localized_texts (tbl_name, tbl_column_name, idx, pt_br)
VALUES 
-- Exemplo de itens básicos (substitua pelos IDs reais)
('items', 'name', 1, 'Espada de Ferro'),
('items', 'name', 2, 'Escudo de Madeira'),
('items', 'name', 3, 'Poção de Vida'),
('items', 'name', 4, 'Arco Élfico'),
('items', 'name', 5, 'Armadura de Couro');

-- 6. Template para tradução de NPCs
INSERT OR REPLACE INTO localized_texts (tbl_name, tbl_column_name, idx, pt_br)
VALUES 
-- Exemplo de NPCs (substitua pelos IDs reais)
('npcs', 'name', 1, 'Mercador'),
('npcs', 'name', 2, 'Guarda'),
('npcs', 'name', 3, 'Ferreiro'),
('npcs', 'name', 4, 'Mago'),
('npcs', 'name', 5, 'Arqueiro');

-- 7. Verificar traduções específicas
SELECT 
    tbl_name,
    tbl_column_name,
    idx,
    en_us,
    pt_br
FROM localized_texts 
WHERE pt_br IS NOT NULL 
    AND pt_br != ''
ORDER BY tbl_name, idx;

-- 8. Procurar por texto específico (exemplo: "sword")
SELECT 
    tbl_name,
    tbl_column_name,
    idx,
    en_us,
    pt_br
FROM localized_texts 
WHERE LOWER(en_us) LIKE '%sword%'
ORDER BY tbl_name, idx;

-- 9. Copiar traduções do inglês temporariamente (USE COM CUIDADO!)
-- Este comando copia en_us para pt_br onde pt_br está vazio
-- Use apenas como base inicial, substitua por traduções reais depois
UPDATE localized_texts 
SET pt_br = en_us 
WHERE (pt_br IS NULL OR pt_br = '') 
    AND en_us IS NOT NULL 
    AND en_us != '';

-- 10. Estatísticas de tradução por categoria
SELECT 
    tbl_name,
    tbl_column_name,
    COUNT(*) as total,
    COUNT(CASE WHEN pt_br IS NOT NULL AND pt_br != '' THEN 1 END) as translated,
    ROUND(
        COUNT(CASE WHEN pt_br IS NOT NULL AND pt_br != '' THEN 1 END) * 100.0 / COUNT(*), 
        2
    ) as percentage_translated
FROM localized_texts 
GROUP BY tbl_name, tbl_column_name
ORDER BY tbl_name, tbl_column_name;

-- 11. Backup das traduções existentes
CREATE TABLE IF NOT EXISTS localized_texts_backup AS 
SELECT * FROM localized_texts WHERE pt_br IS NOT NULL AND pt_br != '';

-- 12. Restaurar backup (se necessário)
-- UPDATE localized_texts 
-- SET pt_br = (SELECT pt_br FROM localized_texts_backup WHERE 
--              localized_texts_backup.tbl_name = localized_texts.tbl_name AND
--              localized_texts_backup.tbl_column_name = localized_texts.tbl_column_name AND
--              localized_texts_backup.idx = localized_texts.idx)
-- WHERE EXISTS (SELECT 1 FROM localized_texts_backup WHERE 
--               localized_texts_backup.tbl_name = localized_texts.tbl_name AND
--               localized_texts_backup.tbl_column_name = localized_texts.tbl_column_name AND
--               localized_texts_backup.idx = localized_texts.idx);