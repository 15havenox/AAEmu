#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script auxiliar para tradução pt-BR do AAEmu
Este script ajuda a gerenciar traduções no banco SQLite
"""

import sqlite3
import json
import csv
import argparse
import os

class AAEmuTranslationHelper:
    def __init__(self, db_path="Data/compact.sqlite3"):
        self.db_path = db_path
        self.conn = None
        
    def connect(self):
        """Conecta ao banco de dados"""
        if not os.path.exists(self.db_path):
            print(f"Erro: Arquivo {self.db_path} não encontrado!")
            print("Certifique-se de que o arquivo compact.sqlite3 do cliente ArcheAge está na pasta Data/")
            return False
            
        self.conn = sqlite3.connect(self.db_path)
        self.conn.row_factory = sqlite3.Row
        return True
        
    def close(self):
        """Fecha a conexão com o banco"""
        if self.conn:
            self.conn.close()
            
    def check_pt_br_column(self):
        """Verifica se a coluna pt_br existe"""
        cursor = self.conn.cursor()
        cursor.execute("PRAGMA table_info(localized_texts)")
        columns = [row[1] for row in cursor.fetchall()]
        return 'pt_br' in columns
        
    def add_pt_br_column(self):
        """Adiciona a coluna pt_br se ela não existir"""
        if not self.check_pt_br_column():
            print("Adicionando coluna pt_br...")
            cursor = self.conn.cursor()
            cursor.execute("ALTER TABLE localized_texts ADD COLUMN pt_br TEXT")
            self.conn.commit()
            print("Coluna pt_br adicionada com sucesso!")
        else:
            print("Coluna pt_br já existe.")
            
    def export_untranslated(self, output_file="untranslated.csv", categories=None):
        """Exporta itens sem tradução para CSV"""
        if categories is None:
            categories = ['items', 'npcs', 'buffs', 'quest_contexts']
            
        cursor = self.conn.cursor()
        
        query = """
        SELECT tbl_name, tbl_column_name, idx, en_us, pt_br
        FROM localized_texts 
        WHERE (pt_br IS NULL OR pt_br = '') 
            AND tbl_name IN ({})
            AND tbl_column_name = 'name'
            AND en_us IS NOT NULL
        ORDER BY tbl_name, idx
        """.format(','.join(['?' for _ in categories]))
        
        cursor.execute(query, categories)
        rows = cursor.fetchall()
        
        with open(output_file, 'w', newline='', encoding='utf-8') as csvfile:
            writer = csv.writer(csvfile)
            writer.writerow(['Categoria', 'Coluna', 'ID', 'Texto_Inglês', 'Tradução_PT-BR'])
            
            for row in rows:
                writer.writerow([row['tbl_name'], row['tbl_column_name'], 
                               row['idx'], row['en_us'], ''])
                               
        print(f"Exportados {len(rows)} itens sem tradução para {output_file}")
        
    def import_translations(self, csv_file):
        """Importa traduções de um arquivo CSV"""
        if not os.path.exists(csv_file):
            print(f"Erro: Arquivo {csv_file} não encontrado!")
            return
            
        cursor = self.conn.cursor()
        imported = 0
        
        with open(csv_file, 'r', encoding='utf-8') as csvfile:
            reader = csv.DictReader(csvfile)
            
            for row in reader:
                if row['Tradução_PT-BR'].strip():  # Se há tradução
                    cursor.execute("""
                        UPDATE localized_texts 
                        SET pt_br = ? 
                        WHERE tbl_name = ? AND tbl_column_name = ? AND idx = ?
                    """, (row['Tradução_PT-BR'], row['Categoria'], 
                          row['Coluna'], int(row['ID'])))
                    imported += 1
                    
        self.conn.commit()
        print(f"Importadas {imported} traduções do arquivo {csv_file}")
        
    def get_translation_stats(self):
        """Mostra estatísticas de tradução"""
        cursor = self.conn.cursor()
        
        cursor.execute("""
            SELECT 
                tbl_name,
                tbl_column_name,
                COUNT(*) as total,
                COUNT(CASE WHEN pt_br IS NOT NULL AND pt_br != '' THEN 1 END) as translated,
                ROUND(
                    COUNT(CASE WHEN pt_br IS NOT NULL AND pt_br != '' THEN 1 END) * 100.0 / COUNT(*), 
                    2
                ) as percentage
            FROM localized_texts 
            GROUP BY tbl_name, tbl_column_name
            ORDER BY tbl_name, tbl_column_name
        """)
        
        print("\n=== Estatísticas de Tradução ===")
        print(f"{'Categoria':<20} {'Coluna':<15} {'Total':<8} {'Traduzido':<10} {'%':<8}")
        print("-" * 70)
        
        for row in cursor.fetchall():
            print(f"{row['tbl_name']:<20} {row['tbl_column_name']:<15} "
                  f"{row['total']:<8} {row['translated']:<10} {row['percentage']:<8}%")
                  
    def search_text(self, search_term, language='en_us'):
        """Procura por texto específico"""
        cursor = self.conn.cursor()
        
        cursor.execute(f"""
            SELECT tbl_name, tbl_column_name, idx, en_us, pt_br
            FROM localized_texts 
            WHERE LOWER({language}) LIKE ?
            ORDER BY tbl_name, idx
            LIMIT 20
        """, (f'%{search_term.lower()}%',))
        
        rows = cursor.fetchall()
        
        print(f"\n=== Resultados para '{search_term}' ===")
        for row in rows:
            print(f"{row['tbl_name']}/{row['tbl_column_name']}[{row['idx']}]: "
                  f"{row['en_us']} -> {row['pt_br'] or '(sem tradução)'}")

def main():
    parser = argparse.ArgumentParser(description='Auxiliar de tradução pt-BR para AAEmu')
    parser.add_argument('--db', default='Data/compact.sqlite3', 
                       help='Caminho para o arquivo compact.sqlite3')
    parser.add_argument('--action', required=True, 
                       choices=['setup', 'export', 'import', 'stats', 'search'],
                       help='Ação a executar')
    parser.add_argument('--file', help='Arquivo CSV para exportar/importar')
    parser.add_argument('--search', help='Termo para buscar')
    parser.add_argument('--categories', nargs='+', 
                       default=['items', 'npcs', 'buffs', 'quest_contexts'],
                       help='Categorias para exportar')
    
    args = parser.parse_args()
    
    helper = AAEmuTranslationHelper(args.db)
    
    if not helper.connect():
        return 1
        
    try:
        if args.action == 'setup':
            helper.add_pt_br_column()
            
        elif args.action == 'export':
            output_file = args.file or 'untranslated.csv'
            helper.export_untranslated(output_file, args.categories)
            
        elif args.action == 'import':
            if not args.file:
                print("Erro: --file é obrigatório para importar")
                return 1
            helper.import_translations(args.file)
            
        elif args.action == 'stats':
            helper.get_translation_stats()
            
        elif args.action == 'search':
            if not args.search:
                print("Erro: --search é obrigatório para buscar")
                return 1
            helper.search_text(args.search)
            
    finally:
        helper.close()
        
    return 0

if __name__ == '__main__':
    exit(main())