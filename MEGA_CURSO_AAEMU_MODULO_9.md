# 🗄️ **MEGA CURSO ULTRA DETALHADO - MÓDULO 9**

## **INTEGRAÇÃO COM DATABASE - A BIBLIOTECA UNIVERSAL DOS DADOS**

---

### 🎯 **O QUE VOCÊ VAI APRENDER NESTE MÓDULO**

Bem-vindo ao **MÓDULO 9: INTEGRAÇÃO COM DATABASE** do mega curso mais épico de emuladores! 🗄️✨ Agora que você é um engenheiro de comunicações espaciais, é hora de dominar o **COFRE SAGRADO DOS DADOS** - o sistema que guarda e protege cada informação do seu universo virtual!

**🧠 ANALOGIA PRINCIPAL**: Database Integration é como ter a **BIBLIOTECA DE ALEXANDRIA DIGITAL** combinada com um **COFRE SUÍÇO ULTRA SEGURO** - onde bilhões de informações são organizadas, protegidas, acessadas instantaneamente e nunca se perdem! 📚🔐

Neste módulo vamos transformar você de um **ENGENHEIRO DE COMUNICAÇÕES** para um **GUARDIÃO DOS DADOS UNIVERSAIS** que domina connection pooling, CRUD operations e error handling de nível enterprise! 🛡️⚡

---

## 🔗 **CAPÍTULO 1: CONNECTION POOLING - A FILA VIP DOS DADOS**

### **🏊 SISTEMA DE CONEXÕES COMO UMA PISCINA INTELIGENTE**

**👶 ANALOGIA**: Connection Pooling é como ter uma **PISCINA VIP INTELIGENTE** onde ao invés de cada pessoa construir sua própria piscina (cara e lenta), todos compartilham uma piscina gigante com múltiplas raias, sempre limpa e pronta para uso! 🏊‍♂️💎

#### **🔗 DATABASE CONNECTION POOL AVANÇADO**

```csharp
// 📁 Arquivo: AAEmu.Game/Core/Database/DatabaseConnectionPool.cs

using System;
using System.Collections.Concurrent;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace AAEmu.Game.Core.Database
{
    // 🏊 POOL DE CONEXÕES - A "PISCINA VIP INTELIGENTE"
    public class DatabaseConnectionPool : IDisposable
    {
        private readonly ILogger<DatabaseConnectionPool> _logger;
        private readonly ConcurrentQueue<PooledConnection> _availableConnections;
        private readonly ConcurrentDictionary<string, PooledConnection> _activeConnections;
        private readonly SemaphoreSlim _connectionSemaphore;
        private readonly Timer _maintenanceTimer;
        private readonly ConnectionPoolStatistics _statistics;
        
        // ⚙️ CONFIGURAÇÕES DO POOL
        private readonly string _connectionString;
        private readonly int _minPoolSize = 5;
        private readonly int _maxPoolSize = 100;
        private readonly TimeSpan _connectionTimeout = TimeSpan.FromMinutes(30);
        private readonly TimeSpan _maintenanceInterval = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _connectionLifetime = TimeSpan.FromHours(1);

        public DatabaseConnectionPool(ILogger<DatabaseConnectionPool> logger, string connectionString)
        {
            _logger = logger;
            _connectionString = connectionString;
            _availableConnections = new ConcurrentQueue<PooledConnection>();
            _activeConnections = new ConcurrentDictionary<string, PooledConnection>();
            _connectionSemaphore = new SemaphoreSlim(_maxPoolSize, _maxPoolSize);
            _statistics = new ConnectionPoolStatistics();
            
            // ⏰ TIMER DE MANUTENÇÃO
            _maintenanceTimer = new Timer(PerformMaintenance, null, _maintenanceInterval, _maintenanceInterval);
            
            // 🚀 INICIALIZAR POOL
            _ = Task.Run(InitializePoolAsync);
            
            _logger.LogInformation("🏊 DatabaseConnectionPool inicializado - Piscina VIP pronta!");
        }

        // 🚀 INICIALIZAR POOL COM CONEXÕES MÍNIMAS
        private async Task InitializePoolAsync()
        {
            // 👶 ANALOGIA: É como encher a piscina com água limpa antes de abrir!
            
            try
            {
                for (int i = 0; i < _minPoolSize; i++)
                {
                    var connection = await CreateNewConnectionAsync();
                    if (connection != null)
                    {
                        _availableConnections.Enqueue(connection);
                        _statistics.TotalConnections++;
                    }
                }
                
                _logger.LogInformation("✅ Pool inicializado com {Count} conexões", _availableConnections.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao inicializar pool de conexões");
            }
        }

        // 🔗 OBTER CONEXÃO DO POOL
        public async Task<IPooledDatabaseConnection> GetConnectionAsync()
        {
            // 👶 ANALOGIA: É como pegar uma raia livre na piscina VIP!
            
            var startTime = DateTime.UtcNow;
            
            try
            {
                // 🚦 AGUARDAR SEMÁFORO (LIMITE DE CONEXÕES)
                await _connectionSemaphore.WaitAsync();
                
                PooledConnection pooledConnection = null;
                
                // 🔍 TENTAR OBTER CONEXÃO DISPONÍVEL
                while (_availableConnections.TryDequeue(out var availableConnection))
                {
                    // ✅ VERIFICAR SE CONEXÃO ESTÁ VÁLIDA
                    if (await IsConnectionValidAsync(availableConnection))
                    {
                        pooledConnection = availableConnection;
                        break;
                    }
                    else
                    {
                        // 🗑️ DESCARTAR CONEXÃO INVÁLIDA
                        await DisposeConnectionAsync(availableConnection);
                        _statistics.ConnectionsDiscarded++;
                    }
                }
                
                // 🆕 CRIAR NOVA CONEXÃO SE NECESSÁRIO
                if (pooledConnection == null)
                {
                    pooledConnection = await CreateNewConnectionAsync();
                    if (pooledConnection == null)
                    {
                        _connectionSemaphore.Release();
                        throw new InvalidOperationException("Não foi possível criar nova conexão");
                    }
                    _statistics.TotalConnections++;
                }
                
                // 📊 MARCAR COMO ATIVA
                pooledConnection.LastUsedAt = DateTime.UtcNow;
                pooledConnection.IsInUse = true;
                _activeConnections[pooledConnection.Id] = pooledConnection;
                
                // 📈 ATUALIZAR ESTATÍSTICAS
                var waitTime = DateTime.UtcNow - startTime;
                _statistics.ConnectionsAcquired++;
                _statistics.TotalWaitTime += waitTime;
                _statistics.AverageWaitTime = _statistics.TotalWaitTime.TotalMilliseconds / _statistics.ConnectionsAcquired;
                
                _logger.LogTrace("🔗 Conexão {Id} obtida em {Ms}ms", pooledConnection.Id, waitTime.TotalMilliseconds);
                
                return new PooledDatabaseConnection(pooledConnection, this);
            }
            catch (Exception ex)
            {
                _connectionSemaphore.Release();
                _logger.LogError(ex, "💥 Erro ao obter conexão do pool");
                throw;
            }
        }

        // 🔄 RETORNAR CONEXÃO PARA O POOL
        internal async Task ReturnConnectionAsync(PooledConnection connection)
        {
            // 👶 ANALOGIA: É como devolver a raia da piscina para outros usarem!
            
            try
            {
                if (connection == null) return;
                
                // 📊 REMOVER DAS CONEXÕES ATIVAS
                _activeConnections.TryRemove(connection.Id, out _);
                connection.IsInUse = false;
                
                // ✅ VERIFICAR SE CONEXÃO AINDA É VÁLIDA
                if (await IsConnectionValidAsync(connection) && 
                    !IsConnectionExpired(connection))
                {
                    // 🔄 RETORNAR PARA O POOL
                    _availableConnections.Enqueue(connection);
                    _statistics.ConnectionsReturned++;
                    
                    _logger.LogTrace("🔄 Conexão {Id} retornada ao pool", connection.Id);
                }
                else
                {
                    // 🗑️ DESCARTAR CONEXÃO EXPIRADA/INVÁLIDA
                    await DisposeConnectionAsync(connection);
                    _statistics.ConnectionsDiscarded++;
                    
                    _logger.LogTrace("🗑️ Conexão {Id} descartada (expirada/inválida)", connection.Id);
                }
                
                // 🚦 LIBERAR SEMÁFORO
                _connectionSemaphore.Release();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao retornar conexão para o pool");
                _connectionSemaphore.Release();
            }
        }

        // 🆕 CRIAR NOVA CONEXÃO
        private async Task<PooledConnection> CreateNewConnectionAsync()
        {
            // 👶 ANALOGIA: É como construir uma nova raia na piscina!
            
            try
            {
                var mysqlConnection = new MySqlConnection(_connectionString);
                await mysqlConnection.OpenAsync();
                
                // 🔧 CONFIGURAR CONEXÃO
                await ConfigureConnectionAsync(mysqlConnection);
                
                var pooledConnection = new PooledConnection
                {
                    Id = Guid.NewGuid().ToString(),
                    Connection = mysqlConnection,
                    CreatedAt = DateTime.UtcNow,
                    LastUsedAt = DateTime.UtcNow,
                    IsInUse = false
                };
                
                _logger.LogTrace("🆕 Nova conexão {Id} criada", pooledConnection.Id);
                
                return pooledConnection;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao criar nova conexão");
                return null;
            }
        }

        // 🔧 CONFIGURAR CONEXÃO
        private async Task ConfigureConnectionAsync(MySqlConnection connection)
        {
            // 👶 ANALOGIA: É como ajustar a temperatura e produtos químicos da piscina!
            
            try
            {
                // 🔧 CONFIGURAÇÕES DE PERFORMANCE
                using var command = connection.CreateCommand();
                
                // ⚡ OTIMIZAÇÕES DE PERFORMANCE
                command.CommandText = @"
                    SET SESSION sql_mode = 'TRADITIONAL';
                    SET SESSION autocommit = 1;
                    SET SESSION transaction_isolation = 'READ-COMMITTED';
                    SET SESSION innodb_lock_wait_timeout = 5;
                ";
                
                await command.ExecuteNonQueryAsync();
                
                _logger.LogTrace("🔧 Conexão configurada com otimizações de performance");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ Erro ao configurar conexão (continuando mesmo assim)");
            }
        }

        // ✅ VERIFICAR SE CONEXÃO É VÁLIDA
        private async Task<bool> IsConnectionValidAsync(PooledConnection pooledConnection)
        {
            // 👶 ANALOGIA: É como testar se a água da piscina está boa!
            
            try
            {
                if (pooledConnection?.Connection == null)
                    return false;
                
                if (pooledConnection.Connection.State != ConnectionState.Open)
                    return false;
                
                // 🏓 PING TEST
                using var command = pooledConnection.Connection.CreateCommand();
                command.CommandText = "SELECT 1";
                command.CommandTimeout = 5; // 5 segundos timeout
                
                var result = await command.ExecuteScalarAsync();
                return result != null && result.ToString() == "1";
            }
            catch (Exception ex)
            {
                _logger.LogTrace("❌ Conexão {Id} inválida: {Error}", 
                               pooledConnection?.Id, ex.Message);
                return false;
            }
        }

        // ⏰ VERIFICAR SE CONEXÃO EXPIROU
        private bool IsConnectionExpired(PooledConnection connection)
        {
            var age = DateTime.UtcNow - connection.CreatedAt;
            return age > _connectionLifetime;
        }

        // 🧹 MANUTENÇÃO DO POOL
        private void PerformMaintenance(object state)
        {
            // 👶 ANALOGIA: É como o limpador de piscina fazendo manutenção!
            
            try
            {
                var maintenanceStart = DateTime.UtcNow;
                
                // 🧹 LIMPAR CONEXÕES EXPIRADAS
                CleanupExpiredConnections();
                
                // 📊 AJUSTAR TAMANHO DO POOL
                AdjustPoolSize();
                
                // 📈 ATUALIZAR ESTATÍSTICAS
                UpdateStatistics();
                
                var maintenanceTime = DateTime.UtcNow - maintenanceStart;
                _logger.LogTrace("🧹 Manutenção do pool concluída em {Ms}ms", maintenanceTime.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro durante manutenção do pool");
            }
        }

        // 🧹 LIMPAR CONEXÕES EXPIRADAS
        private void CleanupExpiredConnections()
        {
            var connectionsToRemove = new List<PooledConnection>();
            var tempConnections = new List<PooledConnection>();
            
            // 🔍 VERIFICAR CONEXÕES DISPONÍVEIS
            while (_availableConnections.TryDequeue(out var connection))
            {
                if (IsConnectionExpired(connection))
                {
                    connectionsToRemove.Add(connection);
                }
                else
                {
                    tempConnections.Add(connection);
                }
            }
            
            // 🔄 RETORNAR CONEXÕES VÁLIDAS
            foreach (var connection in tempConnections)
            {
                _availableConnections.Enqueue(connection);
            }
            
            // 🗑️ DESCARTAR CONEXÕES EXPIRADAS
            foreach (var connection in connectionsToRemove)
            {
                _ = Task.Run(() => DisposeConnectionAsync(connection));
                _statistics.ConnectionsDiscarded++;
                _statistics.TotalConnections--;
            }
            
            if (connectionsToRemove.Count > 0)
            {
                _logger.LogDebug("🧹 {Count} conexões expiradas removidas", connectionsToRemove.Count);
            }
        }

        // 📊 AJUSTAR TAMANHO DO POOL
        private void AdjustPoolSize()
        {
            var currentSize = _availableConnections.Count;
            var activeSize = _activeConnections.Count;
            var totalSize = currentSize + activeSize;
            
            // 📈 ADICIONAR CONEXÕES SE NECESSÁRIO
            if (currentSize < _minPoolSize && totalSize < _maxPoolSize)
            {
                var connectionsToAdd = Math.Min(_minPoolSize - currentSize, _maxPoolSize - totalSize);
                
                _ = Task.Run(async () =>
                {
                    for (int i = 0; i < connectionsToAdd; i++)
                    {
                        var connection = await CreateNewConnectionAsync();
                        if (connection != null)
                        {
                            _availableConnections.Enqueue(connection);
                            _statistics.TotalConnections++;
                        }
                    }
                });
                
                _logger.LogDebug("📈 Adicionando {Count} conexões ao pool", connectionsToAdd);
            }
        }

        // 📊 ATUALIZAR ESTATÍSTICAS
        private void UpdateStatistics()
        {
            _statistics.AvailableConnections = _availableConnections.Count;
            _statistics.ActiveConnections = _activeConnections.Count;
            _statistics.PoolUtilization = _statistics.TotalConnections > 0 
                ? (double)_statistics.ActiveConnections / _statistics.TotalConnections * 100
                : 0;
        }

        // 🗑️ DESCARTAR CONEXÃO
        private async Task DisposeConnectionAsync(PooledConnection connection)
        {
            try
            {
                if (connection?.Connection != null)
                {
                    if (connection.Connection.State == ConnectionState.Open)
                    {
                        await connection.Connection.CloseAsync();
                    }
                    connection.Connection.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.LogTrace("⚠️ Erro ao descartar conexão: {Error}", ex.Message);
            }
        }

        // 📊 OBTER ESTATÍSTICAS
        public ConnectionPoolStatistics GetStatistics()
        {
            UpdateStatistics();
            return _statistics;
        }

        // 🔚 DISPOSE
        public void Dispose()
        {
            try
            {
                _maintenanceTimer?.Dispose();
                _connectionSemaphore?.Dispose();
                
                // 🗑️ DESCARTAR TODAS AS CONEXÕES
                while (_availableConnections.TryDequeue(out var connection))
                {
                    _ = DisposeConnectionAsync(connection);
                }
                
                foreach (var activeConnection in _activeConnections.Values)
                {
                    _ = DisposeConnectionAsync(activeConnection);
                }
                
                _logger.LogInformation("🔚 DatabaseConnectionPool finalizado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro durante dispose do pool");
            }
        }
    }

    // 🏊 CONEXÃO POOLED
    public class PooledConnection
    {
        public string Id { get; set; }
        public MySqlConnection Connection { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUsedAt { get; set; }
        public bool IsInUse { get; set; }
    }

    // 🔗 WRAPPER DE CONEXÃO POOLED
    public class PooledDatabaseConnection : IPooledDatabaseConnection
    {
        private readonly PooledConnection _pooledConnection;
        private readonly DatabaseConnectionPool _pool;
        private bool _disposed = false;

        public PooledDatabaseConnection(PooledConnection pooledConnection, DatabaseConnectionPool pool)
        {
            _pooledConnection = pooledConnection;
            _pool = pool;
        }

        public IDbConnection Connection => _pooledConnection.Connection;
        public string ConnectionId => _pooledConnection.Id;

        public IDbCommand CreateCommand()
        {
            ThrowIfDisposed();
            return _pooledConnection.Connection.CreateCommand();
        }

        public async Task<IDbTransaction> BeginTransactionAsync()
        {
            ThrowIfDisposed();
            return await _pooledConnection.Connection.BeginTransactionAsync();
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(PooledDatabaseConnection));
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                await _pool.ReturnConnectionAsync(_pooledConnection);
                _disposed = true;
            }
        }

        public void Dispose()
        {
            DisposeAsync().AsTask().Wait();
        }
    }

    // 📊 ESTATÍSTICAS DO POOL
    public class ConnectionPoolStatistics
    {
        public int TotalConnections { get; set; }
        public int AvailableConnections { get; set; }
        public int ActiveConnections { get; set; }
        public long ConnectionsAcquired { get; set; }
        public long ConnectionsReturned { get; set; }
        public long ConnectionsDiscarded { get; set; }
        public double PoolUtilization { get; set; }
        public TimeSpan TotalWaitTime { get; set; }
        public double AverageWaitTime { get; set; }
    }

    // 🔗 INTERFACE DE CONEXÃO POOLED
    public interface IPooledDatabaseConnection : IAsyncDisposable, IDisposable
    {
        IDbConnection Connection { get; }
        string ConnectionId { get; }
        IDbCommand CreateCommand();
        Task<IDbTransaction> BeginTransactionAsync();
    }
}
```

---

## 📝 **CAPÍTULO 2: CRUD OPERATIONS - AS OPERAÇÕES SAGRADAS**

### **🔧 SISTEMA DE OPERAÇÕES COMO UM CIRURGIÃO DE DADOS**

**👶 ANALOGIA**: CRUD Operations são como ter um **CIRURGIÃO DE DADOS ULTRA PRECISO** que pode Create (criar), Read (ler), Update (atualizar) e Delete (deletar) informações com a precisão de um bisturi laser! 🏥⚡

#### **🎯 REPOSITORY PATTERN AVANÇADO**

```csharp
// 📁 Arquivo: AAEmu.Game/Core/Database/Repositories/BaseRepository.cs

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace AAEmu.Game.Core.Database.Repositories
{
    // 🎯 REPOSITÓRIO BASE - O "CIRURGIÃO CHEFE"
    public abstract class BaseRepository<TEntity, TKey> where TEntity : class
    {
        protected readonly ILogger _logger;
        protected readonly DatabaseConnectionPool _connectionPool;
        protected readonly string _tableName;
        
        // 📊 ESTATÍSTICAS DE OPERAÇÕES
        protected readonly RepositoryStatistics _statistics;

        protected BaseRepository(
            ILogger logger, 
            DatabaseConnectionPool connectionPool, 
            string tableName)
        {
            _logger = logger;
            _connectionPool = connectionPool;
            _tableName = tableName;
            _statistics = new RepositoryStatistics();
        }

        // 🆕 CREATE - CRIAR NOVA ENTIDADE
        public virtual async Task<TKey> CreateAsync(TEntity entity)
        {
            // 👶 ANALOGIA: É como fazer uma cirurgia para "nascer" um novo registro!
            
            var startTime = DateTime.UtcNow;
            
            try
            {
                await using var connection = await _connectionPool.GetConnectionAsync();
                await using var transaction = await connection.BeginTransactionAsync();
                
                try
                {
                    var insertSql = BuildInsertSql();
                    using var command = connection.CreateCommand();
                    command.CommandText = insertSql;
                    command.Transaction = transaction;
                    
                    // 📊 MAPEAR PARÂMETROS
                    MapEntityToParameters(command, entity);
                    
                    // 🚀 EXECUTAR INSERT
                    await command.ExecuteNonQueryAsync();
                    
                    // 🔑 OBTER ID GERADO
                    var newId = await GetLastInsertIdAsync(connection, transaction);
                    
                    // ✅ COMMIT TRANSACTION
                    await transaction.CommitAsync();
                    
                    // 📈 ATUALIZAR ESTATÍSTICAS
                    var duration = DateTime.UtcNow - startTime;
                    _statistics.CreateOperations++;
                    _statistics.TotalCreateTime += duration;
                    
                    _logger.LogTrace("🆕 Entidade criada na tabela {Table} com ID {Id} em {Ms}ms", 
                                   _tableName, newId, duration.TotalMilliseconds);
                    
                    return (TKey)Convert.ChangeType(newId, typeof(TKey));
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _statistics.CreateErrors++;
                _logger.LogError(ex, "💥 Erro ao criar entidade na tabela {Table}", _tableName);
                throw;
            }
        }

        // 📖 READ - LER ENTIDADE POR ID
        public virtual async Task<TEntity> GetByIdAsync(TKey id)
        {
            // 👶 ANALOGIA: É como fazer um raio-X para ver exatamente o que está lá!
            
            var startTime = DateTime.UtcNow;
            
            try
            {
                await using var connection = await _connectionPool.GetConnectionAsync();
                
                var selectSql = BuildSelectByIdSql();
                using var command = connection.CreateCommand();
                command.CommandText = selectSql;
                
                // 🔑 ADICIONAR PARÂMETRO ID
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@id";
                parameter.Value = id;
                command.Parameters.Add(parameter);
                
                // 📊 EXECUTAR QUERY
                using var reader = await command.ExecuteReaderAsync();
                
                TEntity entity = null;
                if (await reader.ReadAsync())
                {
                    entity = MapReaderToEntity(reader);
                }
                
                // 📈 ATUALIZAR ESTATÍSTICAS
                var duration = DateTime.UtcNow - startTime;
                _statistics.ReadOperations++;
                _statistics.TotalReadTime += duration;
                
                if (entity != null)
                {
                    _logger.LogTrace("📖 Entidade {Id} lida da tabela {Table} em {Ms}ms", 
                                   id, _tableName, duration.TotalMilliseconds);
                }
                
                return entity;
            }
            catch (Exception ex)
            {
                _statistics.ReadErrors++;
                _logger.LogError(ex, "💥 Erro ao ler entidade {Id} da tabela {Table}", id, _tableName);
                throw;
            }
        }

        // 📋 READ ALL - LER TODAS AS ENTIDADES
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(int limit = 1000, int offset = 0)
        {
            // 👶 ANALOGIA: É como fazer uma tomografia completa do banco!
            
            var startTime = DateTime.UtcNow;
            
            try
            {
                await using var connection = await _connectionPool.GetConnectionAsync();
                
                var selectAllSql = BuildSelectAllSql(limit, offset);
                using var command = connection.CreateCommand();
                command.CommandText = selectAllSql;
                
                // 📊 EXECUTAR QUERY
                using var reader = await command.ExecuteReaderAsync();
                
                var entities = new List<TEntity>();
                while (await reader.ReadAsync())
                {
                    var entity = MapReaderToEntity(reader);
                    entities.Add(entity);
                }
                
                // 📈 ATUALIZAR ESTATÍSTICAS
                var duration = DateTime.UtcNow - startTime;
                _statistics.ReadOperations++;
                _statistics.TotalReadTime += duration;
                
                _logger.LogTrace("📋 {Count} entidades lidas da tabela {Table} em {Ms}ms", 
                               entities.Count, _tableName, duration.TotalMilliseconds);
                
                return entities;
            }
            catch (Exception ex)
            {
                _statistics.ReadErrors++;
                _logger.LogError(ex, "💥 Erro ao ler entidades da tabela {Table}", _tableName);
                throw;
            }
        }

        // ✏️ UPDATE - ATUALIZAR ENTIDADE
        public virtual async Task<bool> UpdateAsync(TKey id, TEntity entity)
        {
            // 👶 ANALOGIA: É como fazer uma cirurgia de correção precisa!
            
            var startTime = DateTime.UtcNow;
            
            try
            {
                await using var connection = await _connectionPool.GetConnectionAsync();
                await using var transaction = await connection.BeginTransactionAsync();
                
                try
                {
                    var updateSql = BuildUpdateSql();
                    using var command = connection.CreateCommand();
                    command.CommandText = updateSql;
                    command.Transaction = transaction;
                    
                    // 📊 MAPEAR PARÂMETROS
                    MapEntityToParameters(command, entity);
                    
                    // 🔑 ADICIONAR PARÂMETRO ID
                    var idParameter = command.CreateParameter();
                    idParameter.ParameterName = "@id";
                    idParameter.Value = id;
                    command.Parameters.Add(idParameter);
                    
                    // 🚀 EXECUTAR UPDATE
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    
                    // ✅ COMMIT TRANSACTION
                    await transaction.CommitAsync();
                    
                    // 📈 ATUALIZAR ESTATÍSTICAS
                    var duration = DateTime.UtcNow - startTime;
                    _statistics.UpdateOperations++;
                    _statistics.TotalUpdateTime += duration;
                    
                    var success = rowsAffected > 0;
                    
                    _logger.LogTrace("✏️ Entidade {Id} {Status} na tabela {Table} em {Ms}ms", 
                                   id, success ? "atualizada" : "não encontrada", 
                                   _tableName, duration.TotalMilliseconds);
                    
                    return success;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _statistics.UpdateErrors++;
                _logger.LogError(ex, "💥 Erro ao atualizar entidade {Id} na tabela {Table}", id, _tableName);
                throw;
            }
        }

        // 🗑️ DELETE - DELETAR ENTIDADE
        public virtual async Task<bool> DeleteAsync(TKey id)
        {
            // 👶 ANALOGIA: É como fazer uma cirurgia de remoção precisa!
            
            var startTime = DateTime.UtcNow;
            
            try
            {
                await using var connection = await _connectionPool.GetConnectionAsync();
                await using var transaction = await connection.BeginTransactionAsync();
                
                try
                {
                    var deleteSql = BuildDeleteSql();
                    using var command = connection.CreateCommand();
                    command.CommandText = deleteSql;
                    command.Transaction = transaction;
                    
                    // 🔑 ADICIONAR PARÂMETRO ID
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@id";
                    parameter.Value = id;
                    command.Parameters.Add(parameter);
                    
                    // 🚀 EXECUTAR DELETE
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    
                    // ✅ COMMIT TRANSACTION
                    await transaction.CommitAsync();
                    
                    // 📈 ATUALIZAR ESTATÍSTICAS
                    var duration = DateTime.UtcNow - startTime;
                    _statistics.DeleteOperations++;
                    _statistics.TotalDeleteTime += duration;
                    
                    var success = rowsAffected > 0;
                    
                    _logger.LogTrace("🗑️ Entidade {Id} {Status} da tabela {Table} em {Ms}ms", 
                                   id, success ? "deletada" : "não encontrada", 
                                   _tableName, duration.TotalMilliseconds);
                    
                    return success;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _statistics.DeleteErrors++;
                _logger.LogError(ex, "💥 Erro ao deletar entidade {Id} da tabela {Table}", id, _tableName);
                throw;
            }
        }

        // 🔍 BUSCA AVANÇADA COM FILTROS
        public virtual async Task<IEnumerable<TEntity>> FindAsync(
            string whereClause, 
            object parameters = null, 
            string orderBy = null, 
            int limit = 1000, 
            int offset = 0)
        {
            // 👶 ANALOGIA: É como fazer uma busca específica com critérios precisos!
            
            var startTime = DateTime.UtcNow;
            
            try
            {
                await using var connection = await _connectionPool.GetConnectionAsync();
                
                var searchSql = BuildSearchSql(whereClause, orderBy, limit, offset);
                using var command = connection.CreateCommand();
                command.CommandText = searchSql;
                
                // 📊 ADICIONAR PARÂMETROS SE FORNECIDOS
                if (parameters != null)
                {
                    AddParametersToCommand(command, parameters);
                }
                
                // 📊 EXECUTAR QUERY
                using var reader = await command.ExecuteReaderAsync();
                
                var entities = new List<TEntity>();
                while (await reader.ReadAsync())
                {
                    var entity = MapReaderToEntity(reader);
                    entities.Add(entity);
                }
                
                // 📈 ATUALIZAR ESTATÍSTICAS
                var duration = DateTime.UtcNow - startTime;
                _statistics.SearchOperations++;
                _statistics.TotalSearchTime += duration;
                
                _logger.LogTrace("🔍 Busca retornou {Count} entidades da tabela {Table} em {Ms}ms", 
                               entities.Count, _tableName, duration.TotalMilliseconds);
                
                return entities;
            }
            catch (Exception ex)
            {
                _statistics.SearchErrors++;
                _logger.LogError(ex, "💥 Erro na busca da tabela {Table}", _tableName);
                throw;
            }
        }

        // 📊 CONTAR REGISTROS
        public virtual async Task<long> CountAsync(string whereClause = null, object parameters = null)
        {
            // 👶 ANALOGIA: É como contar quantos pacientes tem no hospital!
            
            try
            {
                await using var connection = await _connectionPool.GetConnectionAsync();
                
                var countSql = BuildCountSql(whereClause);
                using var command = connection.CreateCommand();
                command.CommandText = countSql;
                
                // 📊 ADICIONAR PARÂMETROS SE FORNECIDOS
                if (parameters != null)
                {
                    AddParametersToCommand(command, parameters);
                }
                
                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt64(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao contar registros da tabela {Table}", _tableName);
                throw;
            }
        }

        // 🏗️ MÉTODOS ABSTRATOS PARA IMPLEMENTAÇÃO
        protected abstract string BuildInsertSql();
        protected abstract string BuildSelectByIdSql();
        protected abstract string BuildSelectAllSql(int limit, int offset);
        protected abstract string BuildUpdateSql();
        protected abstract string BuildDeleteSql();
        protected abstract void MapEntityToParameters(IDbCommand command, TEntity entity);
        protected abstract TEntity MapReaderToEntity(IDataReader reader);

        // 🔧 MÉTODOS HELPER
        protected virtual string BuildSearchSql(string whereClause, string orderBy, int limit, int offset)
        {
            var sql = $"SELECT * FROM {_tableName}";
            
            if (!string.IsNullOrEmpty(whereClause))
                sql += $" WHERE {whereClause}";
            
            if (!string.IsNullOrEmpty(orderBy))
                sql += $" ORDER BY {orderBy}";
            
            sql += $" LIMIT {limit} OFFSET {offset}";
            
            return sql;
        }

        protected virtual string BuildCountSql(string whereClause)
        {
            var sql = $"SELECT COUNT(*) FROM {_tableName}";
            
            if (!string.IsNullOrEmpty(whereClause))
                sql += $" WHERE {whereClause}";
            
            return sql;
        }

        protected virtual async Task<long> GetLastInsertIdAsync(IPooledDatabaseConnection connection, IDbTransaction transaction)
        {
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT LAST_INSERT_ID()";
            command.Transaction = transaction;
            
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt64(result);
        }

        protected virtual void AddParametersToCommand(IDbCommand command, object parameters)
        {
            if (parameters == null) return;
            
            var properties = parameters.GetType().GetProperties();
            foreach (var property in properties)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = $"@{property.Name}";
                parameter.Value = property.GetValue(parameters) ?? DBNull.Value;
                command.Parameters.Add(parameter);
            }
        }

        // 📊 OBTER ESTATÍSTICAS
        public virtual RepositoryStatistics GetStatistics()
        {
            return _statistics;
        }
    }

    // 📊 ESTATÍSTICAS DO REPOSITÓRIO
    public class RepositoryStatistics
    {
        public long CreateOperations { get; set; }
        public long ReadOperations { get; set; }
        public long UpdateOperations { get; set; }
        public long DeleteOperations { get; set; }
        public long SearchOperations { get; set; }
        
        public long CreateErrors { get; set; }
        public long ReadErrors { get; set; }
        public long UpdateErrors { get; set; }
        public long DeleteErrors { get; set; }
        public long SearchErrors { get; set; }
        
        public TimeSpan TotalCreateTime { get; set; }
        public TimeSpan TotalReadTime { get; set; }
        public TimeSpan TotalUpdateTime { get; set; }
        public TimeSpan TotalDeleteTime { get; set; }
        public TimeSpan TotalSearchTime { get; set; }
        
        public double AverageCreateTime => CreateOperations > 0 ? TotalCreateTime.TotalMilliseconds / CreateOperations : 0;
        public double AverageReadTime => ReadOperations > 0 ? TotalReadTime.TotalMilliseconds / ReadOperations : 0;
        public double AverageUpdateTime => UpdateOperations > 0 ? TotalUpdateTime.TotalMilliseconds / UpdateOperations : 0;
        public double AverageDeleteTime => DeleteOperations > 0 ? TotalDeleteTime.TotalMilliseconds / DeleteOperations : 0;
        public double AverageSearchTime => SearchOperations > 0 ? TotalSearchTime.TotalMilliseconds / SearchOperations : 0;
        
        public long TotalOperations => CreateOperations + ReadOperations + UpdateOperations + DeleteOperations + SearchOperations;
        public long TotalErrors => CreateErrors + ReadErrors + UpdateErrors + DeleteErrors + SearchErrors;
        public double ErrorRate => TotalOperations > 0 ? (double)TotalErrors / TotalOperations * 100 : 0;
    }
}
```

---

## 🚨 **CAPÍTULO 3: ERROR HANDLING - O SISTEMA IMUNOLÓGICO**

### **🛡️ TRATAMENTO DE ERROS COMO UM MÉDICO DE EMERGÊNCIA**

**👶 ANALOGIA**: Error Handling é como ter um **MÉDICO DE EMERGÊNCIA ULTRA ESPECIALIZADO** que diagnostica problemas instantaneamente, aplica tratamentos específicos, previne complicações e sempre tem um plano B, C e D! 🏥⚡

#### **🚨 DATABASE ERROR HANDLER AVANÇADO**

```csharp
// 📁 Arquivo: AAEmu.Game/Core/Database/ErrorHandling/DatabaseErrorHandler.cs

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace AAEmu.Game.Core.Database.ErrorHandling
{
    // 🚨 MANIPULADOR DE ERROS - O "MÉDICO DE EMERGÊNCIA"
    public class DatabaseErrorHandler
    {
        private readonly ILogger<DatabaseErrorHandler> _logger;
        private readonly ErrorHandlingStatistics _statistics;
        private readonly CircuitBreaker _circuitBreaker;
        private readonly RetryPolicy _retryPolicy;

        public DatabaseErrorHandler(ILogger<DatabaseErrorHandler> logger)
        {
            _logger = logger;
            _statistics = new ErrorHandlingStatistics();
            _circuitBreaker = new CircuitBreaker(logger);
            _retryPolicy = new RetryPolicy(logger);
        }

        // 🎯 EXECUTAR OPERAÇÃO COM TRATAMENTO DE ERRO
        public async Task<T> ExecuteWithErrorHandlingAsync<T>(
            string operationName,
            Func<Task<T>> operation,
            ErrorHandlingOptions options = null)
        {
            // 👶 ANALOGIA: É como fazer uma cirurgia com todos os equipamentos de emergência prontos!
            
            options ??= new ErrorHandlingOptions();
            var startTime = DateTime.UtcNow;
            Exception lastException = null;
            
            for (int attempt = 1; attempt <= options.MaxRetries; attempt++)
            {
                try
                {
                    // 🔍 VERIFICAR CIRCUIT BREAKER
                    if (_circuitBreaker.IsOpen)
                    {
                        throw new DatabaseCircuitBreakerException("Circuit breaker is open - database may be unavailable");
                    }
                    
                    // 🚀 EXECUTAR OPERAÇÃO
                    var result = await operation();
                    
                    // ✅ SUCESSO - RESETAR CIRCUIT BREAKER
                    _circuitBreaker.RecordSuccess();
                    
                    // 📊 ATUALIZAR ESTATÍSTICAS DE SUCESSO
                    var duration = DateTime.UtcNow - startTime;
                    _statistics.SuccessfulOperations++;
                    _statistics.TotalExecutionTime += duration;
                    
                    if (attempt > 1)
                    {
                        _logger.LogInformation("✅ Operação {Operation} bem-sucedida na tentativa {Attempt}", 
                                             operationName, attempt);
                    }
                    
                    return result;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    
                    // 📊 CLASSIFICAR ERRO
                    var errorInfo = ClassifyError(ex);
                    
                    // 📈 ATUALIZAR ESTATÍSTICAS
                    _statistics.TotalErrors++;
                    _statistics.ErrorsByType[errorInfo.Type] = _statistics.ErrorsByType.GetValueOrDefault(errorInfo.Type, 0) + 1;
                    
                    // 🚨 REGISTRAR FALHA NO CIRCUIT BREAKER
                    _circuitBreaker.RecordFailure();
                    
                    _logger.LogWarning("⚠️ Erro na operação {Operation} (tentativa {Attempt}/{Max}): {Error}", 
                                     operationName, attempt, options.MaxRetries, ex.Message);
                    
                    // 🔍 VERIFICAR SE DEVE TENTAR NOVAMENTE
                    if (attempt < options.MaxRetries && errorInfo.IsRetryable)
                    {
                        var delay = _retryPolicy.CalculateDelay(attempt, errorInfo);
                        
                        _logger.LogInformation("🔄 Tentando novamente em {Delay}ms...", delay.TotalMilliseconds);
                        
                        await Task.Delay(delay);
                        continue;
                    }
                    
                    // 💥 FALHA FINAL
                    break;
                }
            }
            
            // 🚨 TODAS AS TENTATIVAS FALHARAM
            var totalDuration = DateTime.UtcNow - startTime;
            _statistics.FailedOperations++;
            _statistics.TotalFailureTime += totalDuration;
            
            _logger.LogError(lastException, "💥 Operação {Operation} falhou após {Attempts} tentativas em {Duration}ms", 
                           operationName, options.MaxRetries, totalDuration.TotalMilliseconds);
            
            // 🎯 LANÇAR EXCEÇÃO ESPECIALIZADA
            throw CreateSpecializedException(operationName, lastException, options.MaxRetries);
        }

        // 🔍 CLASSIFICAR ERRO
        private DatabaseErrorInfo ClassifyError(Exception exception)
        {
            // 👶 ANALOGIA: É como um médico fazendo diagnóstico rápido!
            
            return exception switch
            {
                MySqlException mysqlEx => ClassifyMySqlError(mysqlEx),
                TimeoutException => new DatabaseErrorInfo
                {
                    Type = DatabaseErrorType.Timeout,
                    IsRetryable = true,
                    Severity = ErrorSeverity.Medium,
                    Description = "Database operation timed out"
                },
                InvalidOperationException => new DatabaseErrorInfo
                {
                    Type = DatabaseErrorType.InvalidOperation,
                    IsRetryable = false,
                    Severity = ErrorSeverity.High,
                    Description = "Invalid database operation"
                },
                _ => new DatabaseErrorInfo
                {
                    Type = DatabaseErrorType.Unknown,
                    IsRetryable = true,
                    Severity = ErrorSeverity.Medium,
                    Description = "Unknown database error"
                }
            };
        }

        // 🔍 CLASSIFICAR ERRO MYSQL ESPECÍFICO
        private DatabaseErrorInfo ClassifyMySqlError(MySqlException mysqlException)
        {
            // 👶 ANALOGIA: É como um especialista em MySQL fazendo diagnóstico preciso!
            
            return mysqlException.Number switch
            {
                // 🔗 ERROS DE CONEXÃO
                1040 => new DatabaseErrorInfo // Too many connections
                {
                    Type = DatabaseErrorType.ConnectionLimit,
                    IsRetryable = true,
                    Severity = ErrorSeverity.High,
                    Description = "Too many database connections"
                },
                2003 => new DatabaseErrorInfo // Can't connect to server
                {
                    Type = DatabaseErrorType.ConnectionFailed,
                    IsRetryable = true,
                    Severity = ErrorSeverity.Critical,
                    Description = "Cannot connect to database server"
                },
                2006 => new DatabaseErrorInfo // Server has gone away
                {
                    Type = DatabaseErrorType.ConnectionLost,
                    IsRetryable = true,
                    Severity = ErrorSeverity.High,
                    Description = "Database server connection lost"
                },
                
                // 🔒 ERROS DE LOCK/DEADLOCK
                1205 => new DatabaseErrorInfo // Lock wait timeout
                {
                    Type = DatabaseErrorType.LockTimeout,
                    IsRetryable = true,
                    Severity = ErrorSeverity.Medium,
                    Description = "Database lock timeout"
                },
                1213 => new DatabaseErrorInfo // Deadlock
                {
                    Type = DatabaseErrorType.Deadlock,
                    IsRetryable = true,
                    Severity = ErrorSeverity.Medium,
                    Description = "Database deadlock detected"
                },
                
                // 📊 ERROS DE DADOS
                1062 => new DatabaseErrorInfo // Duplicate entry
                {
                    Type = DatabaseErrorType.DuplicateKey,
                    IsRetryable = false,
                    Severity = ErrorSeverity.Low,
                    Description = "Duplicate key violation"
                },
                1452 => new DatabaseErrorInfo // Foreign key constraint
                {
                    Type = DatabaseErrorType.ForeignKeyViolation,
                    IsRetryable = false,
                    Severity = ErrorSeverity.Medium,
                    Description = "Foreign key constraint violation"
                },
                
                // 🗄️ ERROS DE STORAGE
                1114 => new DatabaseErrorInfo // Table is full
                {
                    Type = DatabaseErrorType.StorageFull,
                    IsRetryable = false,
                    Severity = ErrorSeverity.Critical,
                    Description = "Database table is full"
                },
                
                // 📦 DEFAULT
                _ => new DatabaseErrorInfo
                {
                    Type = DatabaseErrorType.MySqlError,
                    IsRetryable = IsGenerallyRetryable(mysqlException.Number),
                    Severity = ErrorSeverity.Medium,
                    Description = $"MySQL Error {mysqlException.Number}: {mysqlException.Message}"
                }
            };
        }

        // 🔍 VERIFICAR SE ERRO É GERALMENTE RETRYABLE
        private bool IsGenerallyRetryable(int errorNumber)
        {
            // 👶 ANALOGIA: É como saber quais doenças podem ser tratadas tentando novamente!
            
            var nonRetryableErrors = new[]
            {
                1062, // Duplicate entry
                1452, // Foreign key constraint
                1054, // Unknown column
                1146, // Table doesn't exist
                1064, // SQL syntax error
            };
            
            return !Array.Exists(nonRetryableErrors, x => x == errorNumber);
        }

        // 🎯 CRIAR EXCEÇÃO ESPECIALIZADA
        private Exception CreateSpecializedException(string operationName, Exception originalException, int maxRetries)
        {
            // 👶 ANALOGIA: É como dar um diagnóstico final preciso!
            
            var errorInfo = ClassifyError(originalException);
            
            return errorInfo.Type switch
            {
                DatabaseErrorType.ConnectionFailed => new DatabaseConnectionException(
                    $"Failed to connect to database during {operationName} after {maxRetries} attempts", 
                    originalException),
                
                DatabaseErrorType.Timeout => new DatabaseTimeoutException(
                    $"Database timeout during {operationName} after {maxRetries} attempts", 
                    originalException),
                
                DatabaseErrorType.Deadlock => new DatabaseDeadlockException(
                    $"Database deadlock during {operationName} after {maxRetries} attempts", 
                    originalException),
                
                DatabaseErrorType.DuplicateKey => new DatabaseDuplicateKeyException(
                    $"Duplicate key violation during {operationName}", 
                    originalException),
                
                DatabaseErrorType.ForeignKeyViolation => new DatabaseForeignKeyException(
                    $"Foreign key constraint violation during {operationName}", 
                    originalException),
                
                _ => new DatabaseOperationException(
                    $"Database operation {operationName} failed after {maxRetries} attempts: {originalException.Message}", 
                    originalException)
            };
        }

        // 📊 OBTER ESTATÍSTICAS
        public ErrorHandlingStatistics GetStatistics()
        {
            return _statistics;
        }

        // 🔄 RESETAR CIRCUIT BREAKER
        public void ResetCircuitBreaker()
        {
            _circuitBreaker.Reset();
            _logger.LogInformation("🔄 Circuit breaker resetado manualmente");
        }
    }

    // 🔍 INFORMAÇÕES DE ERRO
    public class DatabaseErrorInfo
    {
        public DatabaseErrorType Type { get; set; }
        public bool IsRetryable { get; set; }
        public ErrorSeverity Severity { get; set; }
        public string Description { get; set; }
    }

    // 🎯 TIPOS DE ERRO
    public enum DatabaseErrorType
    {
        Unknown,
        ConnectionFailed,
        ConnectionLost,
        ConnectionLimit,
        Timeout,
        Deadlock,
        LockTimeout,
        DuplicateKey,
        ForeignKeyViolation,
        StorageFull,
        InvalidOperation,
        MySqlError
    }

    // 📊 SEVERIDADE DO ERRO
    public enum ErrorSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }

    // ⚙️ OPÇÕES DE TRATAMENTO DE ERRO
    public class ErrorHandlingOptions
    {
        public int MaxRetries { get; set; } = 3;
        public TimeSpan BaseDelay { get; set; } = TimeSpan.FromMilliseconds(100);
        public TimeSpan MaxDelay { get; set; } = TimeSpan.FromSeconds(30);
        public bool UseExponentialBackoff { get; set; } = true;
        public bool UseJitter { get; set; } = true;
    }

    // 📊 ESTATÍSTICAS DE TRATAMENTO DE ERRO
    public class ErrorHandlingStatistics
    {
        public long SuccessfulOperations { get; set; }
        public long FailedOperations { get; set; }
        public long TotalErrors { get; set; }
        public Dictionary<DatabaseErrorType, long> ErrorsByType { get; set; } = new();
        public TimeSpan TotalExecutionTime { get; set; }
        public TimeSpan TotalFailureTime { get; set; }
        
        public double SuccessRate => (SuccessfulOperations + FailedOperations) > 0 
            ? (double)SuccessfulOperations / (SuccessfulOperations + FailedOperations) * 100 
            : 0;
        
        public double AverageExecutionTime => SuccessfulOperations > 0 
            ? TotalExecutionTime.TotalMilliseconds / SuccessfulOperations 
            : 0;
    }

    // 🚨 EXCEÇÕES ESPECIALIZADAS
    public class DatabaseConnectionException : Exception
    {
        public DatabaseConnectionException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class DatabaseTimeoutException : Exception
    {
        public DatabaseTimeoutException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class DatabaseDeadlockException : Exception
    {
        public DatabaseDeadlockException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class DatabaseDuplicateKeyException : Exception
    {
        public DatabaseDuplicateKeyException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class DatabaseForeignKeyException : Exception
    {
        public DatabaseForeignKeyException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class DatabaseOperationException : Exception
    {
        public DatabaseOperationException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class DatabaseCircuitBreakerException : Exception
    {
        public DatabaseCircuitBreakerException(string message) : base(message) { }
    }
}
```

---

## 🎯 **RESUMO DO MÓDULO 9 - VOCÊ AGORA É UM GUARDIÃO DOS DADOS UNIVERSAIS!**

### **🏆 HABILIDADES DE DATABASE INTEGRATION CONQUISTADAS:**

✅ **Connection Pooling**: Piscina VIP inteligente de conexões  
✅ **CRUD Operations**: Operações cirúrgicas precisas nos dados  
✅ **Error Handling**: Sistema imunológico contra falhas  
✅ **Transaction Management**: Controle transacional robusto  
✅ **Repository Pattern**: Padrão de acesso organizado aos dados  
✅ **Performance Monitoring**: Monitoramento de performance de queries  
✅ **Circuit Breaker**: Proteção contra cascata de falhas  
✅ **Retry Policies**: Políticas inteligentes de retry  
✅ **Exception Handling**: Tratamento especializado de exceções  
✅ **Statistics Tracking**: Rastreamento detalhado de estatísticas  

### **💎 SISTEMAS DE DATABASE CRIADOS:**

🏊 **Connection Pool**: Piscina VIP com gerenciamento inteligente  
🎯 **Base Repository**: Cirurgião de dados ultra preciso  
🚨 **Error Handler**: Médico de emergência para falhas  
🔄 **Circuit Breaker**: Sistema de proteção automática  
📊 **Statistics Monitor**: Monitor de performance em tempo real  
🔐 **Transaction Manager**: Gerenciador transacional seguro  
🛡️ **Exception Framework**: Framework de exceções especializadas  
📈 **Performance Tracker**: Rastreador de métricas avançado  

### **🧠 ANALOGIAS ÉPICAS APRENDIDAS:**

🏊 **Connection Pool** = Piscina VIP inteligente compartilhada  
🎯 **CRUD Operations** = Cirurgião de dados ultra preciso  
🚨 **Error Handling** = Médico de emergência especializado  
📚 **Database** = Biblioteca de Alexandria digital + cofre suíço  
🔄 **Retry Policy** = Médico que não desiste do paciente  

### **🎓 CONQUISTAS DESBLOQUEADAS:**

🏆 **Database Master** - Domina todos os aspectos de banco de dados  
🏊 **Pool Specialist** - Gerencia conexões como um mestre  
🎯 **CRUD Expert** - Executa operações com precisão cirúrgica  
🚨 **Error Guardian** - Protege contra todas as falhas possíveis  
📊 **Performance Analyst** - Monitora e otimiza queries  
🔄 **Resilience Engineer** - Cria sistemas à prova de falhas  

### **🌟 SEU NÍVEL ATUAL:**

**🛡️ GUARDIÃO DOS DADOS UNIVERSAIS**  
- ✅ Gerencia bilhões de registros com eficiência  
- ✅ Protege dados com segurança de cofre suíço  
- ✅ Executa operações com precisão cirúrgica  
- ✅ Trata erros como médico de emergência  
- ✅ Monitora performance em tempo real  
- ✅ Cria sistemas resilientes à prova de falhas  

---

## 🚀 **PRÓXIMO MÓDULO: SISTEMA DE CHARACTERS**

No próximo módulo vamos mergulhar no **MÓDULO 10: SISTEMA DE CHARACTERS** - Character.cs, stats system e inventory management! 👤⚡

**Continue sua jornada épica para se tornar um MESTRE ABSOLUTO em emuladores AAEmu!** 🏆⚡