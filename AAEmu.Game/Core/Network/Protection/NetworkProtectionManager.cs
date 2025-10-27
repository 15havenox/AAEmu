using System.Collections.Concurrent;
using System.Net;
using AAEmu.Commons.Utils;
using NLog;

namespace AAEmu.Game.Core.Network.Protection;

/// <summary>
/// Sistema de proteção contra ataques de rede
/// </summary>
public class NetworkProtectionManager : Singleton<NetworkProtectionManager>
{
    private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

    // Configurações de proteção
    private readonly NetworkProtectionConfig _config = new();
    
    // Tracking de IPs
    private readonly ConcurrentDictionary<string, IPTracker> _ipTrackers = new();
    private readonly ConcurrentDictionary<string, DateTime> _bannedIPs = new();
    private readonly ConcurrentDictionary<string, int> _suspiciousIPs = new();
    
    // Limpeza automática
    private readonly Timer _cleanupTimer;
    
    // Estatísticas
    public NetworkProtectionStats Stats { get; } = new();

    public NetworkProtectionManager()
    {
        _cleanupTimer = new Timer(CleanupExpiredEntries, null, 
            TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }

    /// <summary>
    /// Inicializa o sistema de proteção
    /// </summary>
    public void Initialize()
    {
        Logger.Info("Network Protection Manager initialized");
        Logger.Info($"Config: MaxPacketsPerSecond={_config.MaxPacketsPerSecond}, " +
                   $"BanDuration={_config.BanDuration.TotalMinutes}min, " +
                   $"MaxMalformedPackets={_config.MaxMalformedPacketsPerMinute}");
    }

    /// <summary>
    /// Verifica se um IP está banido
    /// </summary>
    public bool IsIPBanned(string ip)
    {
        if (_bannedIPs.TryGetValue(ip, out var banTime))
        {
            if (DateTime.UtcNow - banTime < _config.BanDuration)
            {
                return true;
            }
            else
            {
                // Ban expirado, remover
                _bannedIPs.TryRemove(ip, out _);
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// Verifica se um pacote deve ser processado
    /// </summary>
    public bool ShouldProcessPacket(string ip, uint packetType)
    {
        // Verificar se IP está banido
        if (IsIPBanned(ip))
        {
            Stats.PacketsBlocked++;
            return false;
        }

        // Obter ou criar tracker para o IP
        var tracker = _ipTrackers.GetOrAdd(ip, _ => new IPTracker());
        
        // Verificar rate limiting
        if (tracker.ShouldRateLimit(_config))
        {
            HandleRateLimitViolation(ip, tracker);
            Stats.PacketsBlocked++;
            return false;
        }

        // Registrar pacote
        tracker.RecordPacket(packetType);
        Stats.PacketsProcessed++;
        
        return true;
    }

    /// <summary>
    /// Reporta um pacote malformado
    /// </summary>
    public void ReportMalformedPacket(string ip, uint packetType = 0)
    {
        Logger.Warn($"Malformed packet from {ip}, type: 0x{packetType:x2}");
        
        var tracker = _ipTrackers.GetOrAdd(ip, _ => new IPTracker());
        tracker.RecordMalformedPacket();
        
        Stats.MalformedPackets++;

        // Verificar se excedeu limite de pacotes malformados
        if (tracker.MalformedPacketsLastMinute >= _config.MaxMalformedPacketsPerMinute)
        {
            Logger.Warn($"IP {ip} exceeded malformed packet limit, applying temporary ban");
            BanIP(ip, "Excessive malformed packets");
        }
    }

    /// <summary>
    /// Reporta um pacote desconhecido
    /// </summary>
    public void ReportUnknownPacket(string ip, uint packetType)
    {
        Logger.Warn($"Unknown packet 0x{packetType:x2} from {ip}");
        
        var tracker = _ipTrackers.GetOrAdd(ip, _ => new IPTracker());
        tracker.RecordUnknownPacket(packetType);
        
        Stats.UnknownPackets++;

        // Se muitos pacotes desconhecidos, pode ser ataque
        if (tracker.UnknownPacketsLastMinute >= _config.MaxUnknownPacketsPerMinute)
        {
            Logger.Warn($"IP {ip} sending too many unknown packets, marking as suspicious");
            MarkIPAsSuspicious(ip);
        }
    }

    /// <summary>
    /// Bane um IP temporariamente
    /// </summary>
    public void BanIP(string ip, string reason)
    {
        _bannedIPs[ip] = DateTime.UtcNow;
        Stats.IPsBanned++;
        
        Logger.Warn($"IP {ip} banned for {_config.BanDuration.TotalMinutes} minutes. Reason: {reason}");
        
        // Notificar outros sistemas se necessário
        OnIPBanned?.Invoke(ip, reason);
    }

    /// <summary>
    /// Marca um IP como suspeito
    /// </summary>
    private void MarkIPAsSuspicious(string ip)
    {
        var suspicionLevel = _suspiciousIPs.AddOrUpdate(ip, 1, (key, value) => value + 1);
        
        if (suspicionLevel >= _config.SuspicionLevelForBan)
        {
            BanIP(ip, $"Suspicious activity (level {suspicionLevel})");
        }
    }

    /// <summary>
    /// Lida com violação de rate limit
    /// </summary>
    private void HandleRateLimitViolation(string ip, IPTracker tracker)
    {
        Logger.Warn($"Rate limit exceeded for IP {ip}: {tracker.PacketsLastSecond} packets/sec");
        
        // Aumentar suspicion level
        MarkIPAsSuspicious(ip);
        
        // Se muito severo, ban imediato
        if (tracker.PacketsLastSecond > _config.MaxPacketsPerSecond * 2)
        {
            BanIP(ip, $"Severe rate limit violation: {tracker.PacketsLastSecond} packets/sec");
        }
    }

    /// <summary>
    /// Remove entradas expiradas do cache
    /// </summary>
    private void CleanupExpiredEntries(object? state)
    {
        try
        {
            var now = DateTime.UtcNow;
            var expiredIPs = new List<string>();

            // Limpar trackers antigos
            foreach (var kvp in _ipTrackers)
            {
                if (now - kvp.Value.LastActivity > TimeSpan.FromMinutes(10))
                {
                    expiredIPs.Add(kvp.Key);
                }
                else
                {
                    kvp.Value.Cleanup(now);
                }
            }

            foreach (var ip in expiredIPs)
            {
                _ipTrackers.TryRemove(ip, out _);
            }

            // Limpar bans expirados
            var expiredBans = _bannedIPs
                .Where(kvp => now - kvp.Value > _config.BanDuration)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var ip in expiredBans)
            {
                _bannedIPs.TryRemove(ip, out _);
                Logger.Info($"Ban lifted for IP {ip}");
            }

            // Limpar suspeitos antigos
            var expiredSuspicious = _suspiciousIPs
                .Where(kvp => now - GetIPTracker(kvp.Key)?.LastActivity > TimeSpan.FromHours(1))
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var ip in expiredSuspicious)
            {
                _suspiciousIPs.TryRemove(ip, out _);
            }

        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error during network protection cleanup");
        }
    }

    /// <summary>
    /// Obtém tracker de um IP
    /// </summary>
    public IPTracker? GetIPTracker(string ip)
    {
        return _ipTrackers.TryGetValue(ip, out var tracker) ? tracker : null;
    }

    /// <summary>
    /// Obtém relatório de proteção
    /// </summary>
    public string GetProtectionReport()
    {
        return $@"
=== NETWORK PROTECTION REPORT ===
Active IP Trackers: {_ipTrackers.Count}
Banned IPs: {_bannedIPs.Count}
Suspicious IPs: {_suspiciousIPs.Count}

=== STATISTICS ===
Packets Processed: {Stats.PacketsProcessed:N0}
Packets Blocked: {Stats.PacketsBlocked:N0}
Malformed Packets: {Stats.MalformedPackets:N0}
Unknown Packets: {Stats.UnknownPackets:N0}
IPs Banned: {Stats.IPsBanned:N0}

=== RECENT BANS ===";
    }

    /// <summary>
    /// Evento disparado quando um IP é banido
    /// </summary>
    public event Action<string, string>? OnIPBanned;

    /// <summary>
    /// Limpa recursos
    /// </summary>
    public void Shutdown()
    {
        _cleanupTimer?.Dispose();
        Logger.Info("Network Protection Manager shut down");
    }
}

/// <summary>
/// Configurações de proteção de rede
/// </summary>
public class NetworkProtectionConfig
{
    public int MaxPacketsPerSecond { get; set; } = 50;
    public int MaxMalformedPacketsPerMinute { get; set; } = 10;
    public int MaxUnknownPacketsPerMinute { get; set; } = 5;
    public TimeSpan BanDuration { get; set; } = TimeSpan.FromMinutes(30);
    public int SuspicionLevelForBan { get; set; } = 3;
    public int MaxPacketSize { get; set; } = 8192; // 8KB
}

/// <summary>
/// Tracker para um IP específico
/// </summary>
public class IPTracker
{
    private readonly Queue<DateTime> _packetTimes = new();
    private readonly Queue<DateTime> _malformedTimes = new();
    private readonly Queue<DateTime> _unknownPacketTimes = new();
    private readonly ConcurrentDictionary<uint, int> _packetTypeCounts = new();

    public DateTime LastActivity { get; private set; } = DateTime.UtcNow;
    public int PacketsLastSecond => CountPacketsInWindow(TimeSpan.FromSeconds(1));
    public int MalformedPacketsLastMinute => CountMalformedInWindow(TimeSpan.FromMinutes(1));
    public int UnknownPacketsLastMinute => CountUnknownInWindow(TimeSpan.FromMinutes(1));

    public void RecordPacket(uint packetType)
    {
        LastActivity = DateTime.UtcNow;
        
        lock (_packetTimes)
        {
            _packetTimes.Enqueue(DateTime.UtcNow);
            _packetTypeCounts.AddOrUpdate(packetType, 1, (key, value) => value + 1);
        }
    }

    public void RecordMalformedPacket()
    {
        LastActivity = DateTime.UtcNow;
        
        lock (_malformedTimes)
        {
            _malformedTimes.Enqueue(DateTime.UtcNow);
        }
    }

    public void RecordUnknownPacket(uint packetType)
    {
        LastActivity = DateTime.UtcNow;
        
        lock (_unknownPacketTimes)
        {
            _unknownPacketTimes.Enqueue(DateTime.UtcNow);
        }
    }

    public bool ShouldRateLimit(NetworkProtectionConfig config)
    {
        return PacketsLastSecond > config.MaxPacketsPerSecond;
    }

    private int CountPacketsInWindow(TimeSpan window)
    {
        var cutoff = DateTime.UtcNow - window;
        lock (_packetTimes)
        {
            while (_packetTimes.Count > 0 && _packetTimes.Peek() < cutoff)
            {
                _packetTimes.Dequeue();
            }
            return _packetTimes.Count;
        }
    }

    private int CountMalformedInWindow(TimeSpan window)
    {
        var cutoff = DateTime.UtcNow - window;
        lock (_malformedTimes)
        {
            while (_malformedTimes.Count > 0 && _malformedTimes.Peek() < cutoff)
            {
                _malformedTimes.Dequeue();
            }
            return _malformedTimes.Count;
        }
    }

    private int CountUnknownInWindow(TimeSpan window)
    {
        var cutoff = DateTime.UtcNow - window;
        lock (_unknownPacketTimes)
        {
            while (_unknownPacketTimes.Count > 0 && _unknownPacketTimes.Peek() < cutoff)
            {
                _unknownPacketTimes.Dequeue();
            }
            return _unknownPacketTimes.Count;
        }
    }

    public void Cleanup(DateTime now)
    {
        var oldCutoff = now - TimeSpan.FromMinutes(5);
        
        lock (_packetTimes)
        {
            while (_packetTimes.Count > 0 && _packetTimes.Peek() < oldCutoff)
            {
                _packetTimes.Dequeue();
            }
        }
        
        lock (_malformedTimes)
        {
            while (_malformedTimes.Count > 0 && _malformedTimes.Peek() < oldCutoff)
            {
                _malformedTimes.Dequeue();
            }
        }
        
        lock (_unknownPacketTimes)
        {
            while (_unknownPacketTimes.Count > 0 && _unknownPacketTimes.Peek() < oldCutoff)
            {
                _unknownPacketTimes.Dequeue();
            }
        }
    }
}

/// <summary>
/// Estatísticas de proteção de rede
/// </summary>
public class NetworkProtectionStats
{
    public long PacketsProcessed { get; set; }
    public long PacketsBlocked { get; set; }
    public long MalformedPackets { get; set; }
    public long UnknownPackets { get; set; }
    public long IPsBanned { get; set; }
}