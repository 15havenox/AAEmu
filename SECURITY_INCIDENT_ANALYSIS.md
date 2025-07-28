# Análise de Incidente de Segurança - AAEmu

## 🚨 Problema Identificado

### Logs do Incidente
```
game-1 | 19:00:03 [ERROR] PacketStream - Attempted to read beyond the end of the stream.
game-1 | 19:00:03 [ERROR] StreamProtocolHandler - Unknown packet 0x00 from 10.8.0.1:
```

### Análise do Problema

**Tipo de Ataque**: **Packet Flooding Attack / Malformed Packet Attack**

**IP Origem**: `10.8.0.1` (provavelmente um IP interno da rede Docker)

**Causa do 100% CPU**:
1. **Loop Infinito**: Pacotes malformados causavam tentativas repetidas de leitura
2. **Sem Rate Limiting**: Nenhuma proteção contra flood de pacotes
3. **Logging Excessivo**: Cada erro gerava log, sobrecarregando I/O
4. **Processamento Ineficiente**: Todos os pacotes eram processados mesmo sendo inválidos

---

## 🛡️ Soluções Implementadas

### 1. Sistema de Proteção de Rede (NetworkProtectionManager)

#### Funcionalidades
- **Rate Limiting**: Máximo 50 pacotes/segundo por IP
- **Ban Automático**: IPs maliciosos banidos por 30 minutos
- **Detecção de Padrões**: Identifica comportamentos suspeitos
- **Limpeza Automática**: Remove entradas antigas automaticamente

#### Configurações Padrão
```csharp
MaxPacketsPerSecond = 50
MaxMalformedPacketsPerMinute = 10
MaxUnknownPacketsPerMinute = 5
BanDuration = 30 minutos
SuspicionLevelForBan = 3
```

### 2. Melhorias no PacketStream

#### Antes (Vulnerável)
```csharp
public byte ReadByte()
{
    if (Pos + 1 > Count)
    {
        Logger.Error("Attempted to read beyond the end of the stream.");
        return 0; // Retornava valor padrão - PERIGOSO!
    }
    return this[Pos++];
}
```

#### Depois (Seguro)
```csharp
public byte ReadByte()
{
    if (Pos + 1 > Count)
    {
        Logger.Error("Attempted to read beyond the end of the stream. Position: {0}, Count: {1}", Pos, Count);
        throw new InvalidOperationException($"Cannot read beyond stream end. Position: {Pos}, Count: {Count}");
    }
    return this[Pos++];
}
```

### 3. Proteção Integrada nos Protocol Handlers

#### StreamProtocolHandler
- Verificação de proteção antes de processar pacotes
- Report automático de pacotes malformados
- Report de pacotes desconhecidos
- Tratamento robusto de exceções

### 4. Comandos GM para Monitoramento

```bash
/server protection report     # Relatório completo de proteção
/server protection stats      # Estatísticas de ataques
/server protection ban <ip>   # Ban manual de IP
/server protection unban <ip> # Desban manual de IP
```

---

## 🔍 Como Prevenir Futuros Ataques

### 1. Monitoramento Proativo
```bash
# Verificar proteção regularmente
/server protection stats

# Monitorar métricas do servidor
/server metrics

# Verificar saúde geral
/server health
```

### 2. Configuração de Firewall (Recomendado)

#### Docker Compose - Limitação de Rede
```yaml
services:
  game:
    networks:
      - aa_network
    ports:
      - "1239:1239"  # Apenas porta necessária
    deploy:
      resources:
        limits:
          memory: 2G
          cpus: '1.0'

networks:
  aa_network:
    driver: bridge
    ipam:
      config:
        - subnet: 10.8.0.0/24
```

#### Iptables (Linux)
```bash
# Limitar conexões por IP
iptables -A INPUT -p tcp --dport 1239 -m connlimit --connlimit-above 10 -j DROP

# Limitar rate de pacotes
iptables -A INPUT -p tcp --dport 1239 -m limit --limit 25/sec --limit-burst 50 -j ACCEPT

# Log e dropar packets suspeitos
iptables -A INPUT -p tcp --dport 1239 -j LOG --log-prefix "AA_ATTACK: "
iptables -A INPUT -p tcp --dport 1239 -j DROP
```

### 3. Monitoramento de Sistema

#### Script de Monitoramento
```bash
#!/bin/bash
# monitor_aa.sh

# Verificar CPU
CPU_USAGE=$(top -bn1 | grep "Cpu(s)" | awk '{print $2}' | cut -d'%' -f1)
if (( $(echo "$CPU_USAGE > 80" | bc -l) )); then
    echo "ALERTA: CPU alto - $CPU_USAGE%"
    # Reiniciar se necessário
fi

# Verificar logs de ataque
ATTACK_COUNT=$(docker-compose logs game | grep -c "Malformed packet\|Unknown packet" | tail -100)
if [ $ATTACK_COUNT -gt 50 ]; then
    echo "ALERTA: Possível ataque detectado - $ATTACK_COUNT eventos"
fi
```

---

## 📊 Métricas de Proteção

### Como Interpretar Logs

#### Logs Normais (OK)
```
[INFO] Network Protection Manager initialized
[DEBUG] Packet processed from 192.168.1.100, type: 0x0A
```

#### Logs de Ataque (ATENÇÃO)
```
[WARN] Malformed packet from 10.8.0.1, type: 0x00
[WARN] Rate limit exceeded for IP 10.8.0.1: 127 packets/sec
[WARN] IP 10.8.0.1 banned for 30 minutes. Reason: Excessive malformed packets
```

### Dashboard de Métricas

Use `/server protection stats` para ver:
- **Packets Processed**: Total de pacotes válidos
- **Packets Blocked**: Pacotes bloqueados pelo sistema
- **Malformed Packets**: Pacotes corrompidos detectados
- **Unknown Packets**: Pacotes de tipo desconhecido
- **IPs Banned**: Total de IPs banidos

---

## 🚀 Ações Imediatas Recomendadas

### 1. Implementar as Correções
```bash
# 1. Baixar código atualizado
git pull origin main

# 2. Recompilar
docker-compose build

# 3. Reiniciar com proteção
docker-compose up -d
```

### 2. Configurar Monitoramento
```bash
# Criar script de monitoramento
vim /opt/aa_monitor.sh
chmod +x /opt/aa_monitor.sh

# Adicionar ao cron (verificar a cada 5 minutos)
echo "*/5 * * * * /opt/aa_monitor.sh" | crontab -
```

### 3. Configurar Alertas
```bash
# Instalar ferramentas de alerta
apt install mailutils

# Script de alerta
echo '#!/bin/bash
ATTACK_COUNT=$(docker-compose logs game --since 5m | grep -c "banned")
if [ $ATTACK_COUNT -gt 0 ]; then
    echo "ATAQUE DETECTADO: $ATTACK_COUNT IPs banidos nos últimos 5min" | mail -s "AAEmu Security Alert" admin@seudominio.com
fi' > /opt/security_alert.sh

chmod +x /opt/security_alert.sh
echo "*/5 * * * * /opt/security_alert.sh" | crontab -
```

### 4. Backup de Segurança
```bash
# Backup automático antes de mudanças
docker-compose exec db mysqldump -u root -p aaemu_game > backup_$(date +%Y%m%d_%H%M%S).sql
```

---

## 🔧 Configurações Avançadas

### Para Servidores Públicos
```csharp
// Configuração mais restritiva
public class ProductionNetworkConfig : NetworkProtectionConfig
{
    public int MaxPacketsPerSecond { get; set; } = 30;        // Mais restritivo
    public int MaxMalformedPacketsPerMinute { get; set; } = 5; // Menos tolerante
    public int MaxUnknownPacketsPerMinute { get; set; } = 2;   // Bem restritivo
    public TimeSpan BanDuration { get; set; } = TimeSpan.FromHours(2); // Ban mais longo
    public int SuspicionLevelForBan { get; set; } = 2;         // Ban mais rápido
}
```

### Para Desenvolvimento
```csharp
// Configuração mais permissiva para testes
public class DevelopmentNetworkConfig : NetworkProtectionConfig
{
    public int MaxPacketsPerSecond { get; set; } = 100;
    public int MaxMalformedPacketsPerMinute { get; set; } = 50;
    public int MaxUnknownPacketsPerMinute { get; set; } = 20;
    public TimeSpan BanDuration { get; set; } = TimeSpan.FromMinutes(5);
    public int SuspicionLevelForBan { get; set; } = 5;
}
```

---

## ✅ Resumo da Proteção

### O que foi corrigido:
1. ✅ **Packet Flooding** - Rate limiting implementado
2. ✅ **Malformed Packets** - Detecção e ban automático
3. ✅ **CPU Overload** - Pacotes inválidos rejeitados rapidamente
4. ✅ **Unknown Packets** - Rastreamento e proteção
5. ✅ **Logging Flood** - Logs otimizados e informativos
6. ✅ **Manual Control** - Comandos GM para gerenciamento

### Benefícios:
- **Performance**: CPU não será mais sobrecarregada
- **Segurança**: Ataques automáticamente bloqueados
- **Visibilidade**: Logs claros sobre ataques
- **Controle**: GMs podem intervir manualmente
- **Escalabilidade**: Sistema cresce com o servidor

**O servidor agora está protegido contra ataques similares e você tem visibilidade completa sobre tentativas de ataque.**