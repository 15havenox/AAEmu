#!/usr/bin/env node

const { execSync } = require('child_process');
const fs = require('fs-extra');
const path = require('path');

const args = process.argv.slice(2);
const platform = args.includes('--windows') ? 'win' : 
                args.includes('--mac') ? 'mac' : 
                args.includes('--linux') ? 'linux' : 'all';

console.log('🚀 Iniciando build do ArcheAge BR Launcher...');

// Verificar se todas as dependências estão instaladas
console.log('📦 Verificando dependências...');
try {
    execSync('npm list --depth=0', { stdio: 'ignore' });
} catch (error) {
    console.log('📦 Instalando dependências...');
    execSync('npm install', { stdio: 'inherit' });
}

// Criar pasta de assets se não existir
const assetsPath = path.join(__dirname, '..', 'assets');
if (!fs.existsSync(assetsPath)) {
    console.log('📁 Criando pasta de assets...');
    fs.ensureDirSync(assetsPath);
    
    // Criar arquivos placeholder se não existirem
    const placeholderFiles = [
        'icon.ico',
        'icon.icns', 
        'icon.png',
        'logo.png',
        'logo-small.png'
    ];
    
    placeholderFiles.forEach(file => {
        const filePath = path.join(assetsPath, file);
        if (!fs.existsSync(filePath)) {
            console.log(`📄 Criando placeholder: ${file}`);
            fs.writeFileSync(filePath, ''); // Arquivo vazio como placeholder
        }
    });
}

// Executar build
console.log(`🔨 Construindo para plataforma: ${platform}`);

try {
    let buildCommand;
    
    switch (platform) {
        case 'win':
            buildCommand = 'npm run build-win';
            break;
        case 'mac':
            buildCommand = 'electron-builder --mac';
            break;
        case 'linux':
            buildCommand = 'electron-builder --linux';
            break;
        default:
            buildCommand = 'npm run build';
    }
    
    execSync(buildCommand, { stdio: 'inherit' });
    
    console.log('✅ Build concluído com sucesso!');
    console.log('📁 Arquivos gerados na pasta dist/');
    
    // Listar arquivos gerados
    const distPath = path.join(__dirname, '..', 'dist');
    if (fs.existsSync(distPath)) {
        const files = fs.readdirSync(distPath);
        console.log('\n📄 Arquivos gerados:');
        files.forEach(file => {
            const filePath = path.join(distPath, file);
            const stats = fs.statSync(filePath);
            const size = (stats.size / 1024 / 1024).toFixed(2);
            console.log(`   • ${file} (${size} MB)`);
        });
    }
    
} catch (error) {
    console.error('❌ Erro durante o build:', error.message);
    process.exit(1);
}

// Verificações pós-build
console.log('\n🔍 Executando verificações...');

const distPath = path.join(__dirname, '..', 'dist');
if (!fs.existsSync(distPath)) {
    console.warn('⚠️  Pasta dist não foi criada');
} else {
    const files = fs.readdirSync(distPath);
    if (files.length === 0) {
        console.warn('⚠️  Nenhum arquivo foi gerado na pasta dist');
    } else {
        console.log('✅ Verificações concluídas');
    }
}

console.log('\n🎉 Build finalizado!');
console.log('📋 Próximos passos:');
console.log('   1. Teste o executável gerado');
console.log('   2. Configure seu servidor AAEmu com os endpoints necessários');
console.log('   3. Personalize os assets (logos, ícones)');
console.log('   4. Configure as URLs do servidor no preload.js');
console.log('\n💡 Para mais informações, consulte o README.md');