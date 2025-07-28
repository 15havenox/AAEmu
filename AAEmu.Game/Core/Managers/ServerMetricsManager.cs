using System.Collections.Concurrent;
using System.Diagnostics;
using AAEmu.Commons.Utils;
using AAEmu.Game.Core.Managers.World;
using NLog;

namespace AAEmu.Game.Core.Managers;

/// <summary>
/// Gerenciador de métricas e monitoramento do servidor
/// </summary>
public class ServerMetricsManager : Singleton<ServerMetricsManager>
{
    private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

    private readonly Timer _metricsTimer;
    private readonly ConcurrentDictionary<string, object> _metrics = new();
    private readonly ConcurrentDictionary<string, PerformanceCounter> _performanceCounters = new();
    private readonly object _lockObject = new();

    // Performance tracking
    private readonly Stopwatch _serverUptime = new();
    private long _totalPacketsReceived = 0;
    private long _totalPacketsSent = 0;
    private long _totalPlayersConnected = 0;
    private long _totalDatabaseQueries = 0;

    // Real-time metrics
    public ServerMetrics CurrentMetrics { get; private set; } = new();

    public ServerMetricsManager()
    {
        _metricsTimer = new Timer(UpdateMetrics, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
    }

    /// <summary>
    /// Inicializa o sistema de métricas
    /// </summary>
    public void Initialize()
    {
        Logger.Info("Initializing Server Metrics Manager...");
        
        _serverUptime.Start();
        InitializePerformanceCounters();
        
        // Métricas iniciais
        _metrics["server_start_time"] = DateTime.UtcNow;
        _metrics["version"] = GetType().Assembly.GetName().Version?.ToString() ?? "Unknown";
        
        Logger.Info("Server Metrics Manager initialized");
    }

    /// <summary>
    /// Inicializa contadores de performance do sistema
    /// </summary>
    private void InitializePerformanceCounters()
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                _performanceCounters["cpu_usage"] = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _performanceCounters["memory_available"] = new PerformanceCounter("Memory", "Available MBytes");
                _performanceCounters["disk_time"] = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");
            }
        }
        catch (Exception ex)
        {
            Logger.Warn(ex, "Failed to initialize performance counters (may require admin rights)");
        }
    }

    /// <summary>
    /// Atualiza as métricas do servidor
    /// </summary>
    private void UpdateMetrics(object? state)
    {
        try
        {
            lock (_lockObject)
            {
                var newMetrics = new ServerMetrics
                {
                    Timestamp = DateTime.UtcNow,
                    OnlinePlayers = GetOnlinePlayerCount(),
                    ServerUptime = _serverUptime.Elapsed,
                    TotalMemoryUsage = GC.GetTotalMemory(false),
                    Gen0Collections = GC.CollectionCount(0),
                    Gen1Collections = GC.CollectionCount(1),
                    Gen2Collections = GC.CollectionCount(2),
                    ThreadCount = Process.GetCurrentProcess().Threads.Count,
                    HandleCount = Process.GetCurrentProcess().HandleCount,
                    TotalPacketsReceived = _totalPacketsReceived,
                    TotalPacketsSent = _totalPacketsSent,
                    TotalPlayersConnected = _totalPlayersConnected,
                    TotalDatabaseQueries = _totalDatabaseQueries,
                    ActiveSystems = GetActiveSystemsCount()
                };

                // Métricas de sistema (Windows apenas)
                if (OperatingSystem.IsWindows())
                {
                    newMetrics.CpuUsage = GetPerformanceCounterValue("cpu_usage");
                    newMetrics.AvailableMemoryMB = GetPerformanceCounterValue("memory_available");
                    newMetrics.DiskUsage = GetPerformanceCounterValue("disk_time");
                }

                CurrentMetrics = newMetrics;
                
                // Log métricas críticas
                if (newMetrics.OnlinePlayers > 0)
                {
                    Logger.Debug($"Metrics Update - Players: {newMetrics.OnlinePlayers}, Memory: {newMetrics.TotalMemoryUsage / 1024 / 1024}MB, CPU: {newMetrics.CpuUsage:F1}%");
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error updating server metrics");
        }
    }

    /// <summary>
    /// Obtém valor de um contador de performance
    /// </summary>
    private float GetPerformanceCounterValue(string counterName)
    {
        try
        {
            if (_performanceCounters.TryGetValue(counterName, out var counter))
            {
                return counter.NextValue();
            }
        }
        catch (Exception ex)
        {
            Logger.Warn(ex, $"Failed to read performance counter: {counterName}");
        }
        return 0f;
    }

    /// <summary>
    /// Obtém número de jogadores online
    /// </summary>
    private int GetOnlinePlayerCount()
    {
        try
        {
            return WorldManager.Instance.GetAllCharacters().Count;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Obtém contagem de sistemas ativos
    /// </summary>
    private Dictionary<string, int> GetActiveSystemsCount()
    {
        var systems = new Dictionary<string, int>();
        
        try
        {
            systems["active_modules"] = ModuleManager.Instance.LoadedModules.Count(m => m.Value.IsEnabled);
            systems["active_events"] = GameEventManager.Instance.ActiveEvents.Count;
            systems["active_containers"] = ItemManager.Instance._allPersistentContainers?.Count ?? 0;
            systems["active_npcs"] = WorldManager.Instance.GetAllNpcs().Count;
            systems["active_doodads"] = WorldManager.Instance.GetAllDoodads().Count;
        }
        catch (Exception ex)
        {
            Logger.Warn(ex, "Error collecting system counts");
        }

        return systems;
    }

    /// <summary>
    /// Registra pacote recebido
    /// </summary>
    public void RecordPacketReceived()
    {
        Interlocked.Increment(ref _totalPacketsReceived);
    }

    /// <summary>
    /// Registra pacote enviado
    /// </summary>
    public void RecordPacketSent()
    {
        Interlocked.Increment(ref _totalPacketsSent);
    }

    /// <summary>
    /// Registra jogador conectado
    /// </summary>
    public void RecordPlayerConnected()
    {
        Interlocked.Increment(ref _totalPlayersConnected);
    }

    /// <summary>
    /// Registra query de banco de dados
    /// </summary>
    public void RecordDatabaseQuery()
    {
        Interlocked.Increment(ref _totalDatabaseQueries);
    }

    /// <summary>
    /// Define uma métrica customizada
    /// </summary>
    public void SetMetric(string key, object value)
    {
        _metrics[key] = value;
    }

    /// <summary>
    /// Obtém uma métrica customizada
    /// </summary>
    public T? GetMetric<T>(string key)
    {
        if (_metrics.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return default;
    }

    /// <summary>
    /// Obtém relatório completo de métricas
    /// </summary>
    public string GetMetricsReport()
    {
        var metrics = CurrentMetrics;
        var report = $@"
=== SERVER METRICS REPORT ===
Timestamp: {metrics.Timestamp:yyyy-MM-dd HH:mm:ss} UTC
Uptime: {metrics.ServerUptime}
Online Players: {metrics.OnlinePlayers}
Total Players Connected: {metrics.TotalPlayersConnected}

=== PERFORMANCE ===
CPU Usage: {metrics.CpuUsage:F1}%
Memory Usage: {metrics.TotalMemoryUsage / 1024 / 1024:F0} MB
Available Memory: {metrics.AvailableMemoryMB:F0} MB
Disk Usage: {metrics.DiskUsage:F1}%
Thread Count: {metrics.ThreadCount}
Handle Count: {metrics.HandleCount}

=== NETWORK ===
Packets Received: {metrics.TotalPacketsReceived:N0}
Packets Sent: {metrics.TotalPacketsSent:N0}
Database Queries: {metrics.TotalDatabaseQueries:N0}

=== GARBAGE COLLECTION ===
Gen 0 Collections: {metrics.Gen0Collections}
Gen 1 Collections: {metrics.Gen1Collections}
Gen 2 Collections: {metrics.Gen2Collections}

=== ACTIVE SYSTEMS ===";

        foreach (var system in metrics.ActiveSystems)
        {
            report += $"\n{system.Key}: {system.Value}";
        }

        return report;
    }

    /// <summary>
    /// Obtém métricas em formato JSON para APIs
    /// </summary>
    public object GetMetricsJson()
    {
        var metrics = CurrentMetrics;
        return new
        {
            timestamp = metrics.Timestamp,
            uptime_seconds = metrics.ServerUptime.TotalSeconds,
            online_players = metrics.OnlinePlayers,
            performance = new
            {
                cpu_usage_percent = metrics.CpuUsage,
                memory_usage_bytes = metrics.TotalMemoryUsage,
                available_memory_mb = metrics.AvailableMemoryMB,
                disk_usage_percent = metrics.DiskUsage,
                thread_count = metrics.ThreadCount,
                handle_count = metrics.HandleCount
            },
            network = new
            {
                total_packets_received = metrics.TotalPacketsReceived,
                total_packets_sent = metrics.TotalPacketsSent,
                total_database_queries = metrics.TotalDatabaseQueries
            },
            garbage_collection = new
            {
                gen0_collections = metrics.Gen0Collections,
                gen1_collections = metrics.Gen1Collections,
                gen2_collections = metrics.Gen2Collections
            },
            systems = metrics.ActiveSystems,
            custom_metrics = _metrics.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
        };
    }

    /// <summary>
    /// Verifica se o servidor está saudável
    /// </summary>
    public HealthStatus GetHealthStatus()
    {
        var metrics = CurrentMetrics;
        var status = new HealthStatus { IsHealthy = true };

        // Check CPU usage
        if (metrics.CpuUsage > 90)
        {
            status.IsHealthy = false;
            status.Issues.Add("High CPU usage");
        }

        // Check memory usage
        if (metrics.AvailableMemoryMB < 512) // Less than 512MB available
        {
            status.IsHealthy = false;
            status.Issues.Add("Low available memory");
        }

        // Check if too many collections are happening
        var totalCollections = metrics.Gen0Collections + metrics.Gen1Collections + metrics.Gen2Collections;
        if (totalCollections > 10000) // Arbitrary threshold
        {
            status.Warnings.Add("High garbage collection activity");
        }

        return status;
    }

    /// <summary>
    /// Limpa recursos
    /// </summary>
    public void Shutdown()
    {
        Logger.Info("Shutting down Server Metrics Manager...");
        
        _metricsTimer?.Dispose();
        
        foreach (var counter in _performanceCounters.Values)
        {
            counter?.Dispose();
        }
        
        _performanceCounters.Clear();
        _serverUptime.Stop();
        
        Logger.Info("Server Metrics Manager shut down");
    }
}

/// <summary>
/// Estrutura de métricas do servidor
/// </summary>
public class ServerMetrics
{
    public DateTime Timestamp { get; set; }
    public int OnlinePlayers { get; set; }
    public TimeSpan ServerUptime { get; set; }
    public long TotalMemoryUsage { get; set; }
    public float CpuUsage { get; set; }
    public float AvailableMemoryMB { get; set; }
    public float DiskUsage { get; set; }
    public int ThreadCount { get; set; }
    public int HandleCount { get; set; }
    public long TotalPacketsReceived { get; set; }
    public long TotalPacketsSent { get; set; }
    public long TotalPlayersConnected { get; set; }
    public long TotalDatabaseQueries { get; set; }
    public int Gen0Collections { get; set; }
    public int Gen1Collections { get; set; }
    public int Gen2Collections { get; set; }
    public Dictionary<string, int> ActiveSystems { get; set; } = new();
}

/// <summary>
/// Status de saúde do servidor
/// </summary>
public class HealthStatus
{
    public bool IsHealthy { get; set; }
    public List<string> Issues { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}