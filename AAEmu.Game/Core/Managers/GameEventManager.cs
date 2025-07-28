using System.Collections.Concurrent;
using AAEmu.Commons.Utils;
using AAEmu.Game.Models.Game.Char;
using AAEmu.Game.Models.Game.Units;
using NLog;

namespace AAEmu.Game.Core.Managers;

/// <summary>
/// Gerenciador de eventos customizados do jogo
/// </summary>
public class GameEventManager : Singleton<GameEventManager>
{
    private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    
    private readonly ConcurrentDictionary<string, List<IGameEventHandler>> _eventHandlers = new();
    private readonly ConcurrentDictionary<uint, CustomGameEvent> _activeEvents = new();
    private readonly object _eventLock = new();

    /// <summary>
    /// Eventos ativos no servidor
    /// </summary>
    public IReadOnlyDictionary<uint, CustomGameEvent> ActiveEvents => _activeEvents;

    /// <summary>
    /// Inicializa o sistema de eventos
    /// </summary>
    public void Initialize()
    {
        Logger.Info("Initializing Game Event Manager...");
        // Initialization logic if needed
        Logger.Info("Game Event Manager initialized");
    }

    /// <summary>
    /// Registra um handler para um evento específico
    /// </summary>
    public void RegisterEventHandler<T>(string eventName, Action<T> handler) where T : GameEventArgs
    {
        var wrapper = new GameEventHandlerWrapper<T>(handler);
        RegisterEventHandler(eventName, wrapper);
    }

    /// <summary>
    /// Registra um handler de evento
    /// </summary>
    public void RegisterEventHandler(string eventName, IGameEventHandler handler)
    {
        _eventHandlers.AddOrUpdate(eventName,
            new List<IGameEventHandler> { handler },
            (key, list) =>
            {
                lock (list)
                {
                    list.Add(handler);
                }
                return list;
            });
    }

    /// <summary>
    /// Remove um handler de evento
    /// </summary>
    public void UnregisterEventHandler(string eventName, IGameEventHandler handler)
    {
        if (_eventHandlers.TryGetValue(eventName, out var handlers))
        {
            lock (handlers)
            {
                handlers.Remove(handler);
            }
        }
    }

    /// <summary>
    /// Dispara um evento customizado
    /// </summary>
    public void TriggerEvent<T>(string eventName, T eventArgs) where T : GameEventArgs
    {
        if (!_eventHandlers.TryGetValue(eventName, out var handlers))
            return;

        List<IGameEventHandler> handlersCopy;
        lock (handlers)
        {
            handlersCopy = new List<IGameEventHandler>(handlers);
        }

        foreach (var handler in handlersCopy)
        {
            try
            {
                handler.Handle(eventArgs);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error handling event {eventName}");
            }
        }
    }

    /// <summary>
    /// Inicia um evento customizado
    /// </summary>
    public bool StartEvent(CustomGameEvent gameEvent)
    {
        if (gameEvent == null)
            return false;

        lock (_eventLock)
        {
            if (_activeEvents.ContainsKey(gameEvent.Id))
            {
                Logger.Warn($"Event {gameEvent.Name} ({gameEvent.Id}) is already active");
                return false;
            }

            try
            {
                gameEvent.Start();
                _activeEvents[gameEvent.Id] = gameEvent;
                
                // Agendar fim do evento se tiver duração
                if (gameEvent.Duration > TimeSpan.Zero)
                {
                    ScheduleEventEnd(gameEvent);
                }

                TriggerEvent("EventStarted", new EventStartedArgs(gameEvent));
                Logger.Info($"Started event: {gameEvent.Name} ({gameEvent.Id})");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Failed to start event {gameEvent.Name}");
                return false;
            }
        }
    }

    /// <summary>
    /// Para um evento customizado
    /// </summary>
    public bool StopEvent(uint eventId)
    {
        lock (_eventLock)
        {
            if (!_activeEvents.TryRemove(eventId, out var gameEvent))
                return false;

            try
            {
                gameEvent.Stop();
                TriggerEvent("EventStopped", new EventStoppedArgs(gameEvent));
                Logger.Info($"Stopped event: {gameEvent.Name} ({eventId})");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error stopping event {eventId}");
                return false;
            }
        }
    }

    /// <summary>
    /// Para todos os eventos ativos
    /// </summary>
    public void StopAllEvents()
    {
        var eventIds = _activeEvents.Keys.ToList();
        foreach (var eventId in eventIds)
        {
            StopEvent(eventId);
        }
    }

    /// <summary>
    /// Agenda o fim automático de um evento
    /// </summary>
    private void ScheduleEventEnd(CustomGameEvent gameEvent)
    {
        TaskManager.Instance.Schedule(new EventEndTask(gameEvent.Id), null, gameEvent.Duration);
    }

    /// <summary>
    /// Registra participação de um jogador em um evento
    /// </summary>
    public void RegisterPlayerParticipation(uint eventId, Character player)
    {
        if (_activeEvents.TryGetValue(eventId, out var gameEvent))
        {
            try
            {
                gameEvent.OnPlayerParticipate(player);
                TriggerEvent("PlayerParticipated", new PlayerParticipatedArgs(gameEvent, player));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error registering player participation in event {eventId}");
            }
        }
    }

    /// <summary>
    /// Obtém um evento ativo por ID
    /// </summary>
    public CustomGameEvent? GetActiveEvent(uint eventId)
    {
        return _activeEvents.TryGetValue(eventId, out var gameEvent) ? gameEvent : null;
    }

    /// <summary>
    /// Obtém eventos ativos por tipo
    /// </summary>
    public IEnumerable<T> GetActiveEvents<T>() where T : CustomGameEvent
    {
        return _activeEvents.Values.OfType<T>();
    }
}

/// <summary>
/// Interface para handlers de eventos
/// </summary>
public interface IGameEventHandler
{
    void Handle(GameEventArgs eventArgs);
}

/// <summary>
/// Wrapper para handlers tipados
/// </summary>
public class GameEventHandlerWrapper<T> : IGameEventHandler where T : GameEventArgs
{
    private readonly Action<T> _handler;

    public GameEventHandlerWrapper(Action<T> handler)
    {
        _handler = handler;
    }

    public void Handle(GameEventArgs eventArgs)
    {
        if (eventArgs is T typedArgs)
        {
            _handler(typedArgs);
        }
    }
}

/// <summary>
/// Classe base para argumentos de eventos
/// </summary>
public abstract class GameEventArgs
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public bool IsCancelled { get; set; } = false;
}

/// <summary>
/// Argumentos para evento de jogador
/// </summary>
public class PlayerEventArgs : GameEventArgs
{
    public Character Player { get; }

    public PlayerEventArgs(Character player)
    {
        Player = player;
    }
}

/// <summary>
/// Argumentos para evento de combate
/// </summary>
public class CombatEventArgs : GameEventArgs
{
    public Unit Attacker { get; }
    public Unit Target { get; }
    public int Damage { get; }

    public CombatEventArgs(Unit attacker, Unit target, int damage)
    {
        Attacker = attacker;
        Target = target;
        Damage = damage;
    }
}

/// <summary>
/// Argumentos para início de evento
/// </summary>
public class EventStartedArgs : GameEventArgs
{
    public CustomGameEvent GameEvent { get; }

    public EventStartedArgs(CustomGameEvent gameEvent)
    {
        GameEvent = gameEvent;
    }
}

/// <summary>
/// Argumentos para fim de evento
/// </summary>
public class EventStoppedArgs : GameEventArgs
{
    public CustomGameEvent GameEvent { get; }

    public EventStoppedArgs(CustomGameEvent gameEvent)
    {
        GameEvent = gameEvent;
    }
}

/// <summary>
/// Argumentos para participação em evento
/// </summary>
public class PlayerParticipatedArgs : GameEventArgs
{
    public CustomGameEvent GameEvent { get; }
    public Character Player { get; }

    public PlayerParticipatedArgs(CustomGameEvent gameEvent, Character player)
    {
        GameEvent = gameEvent;
        Player = player;
    }
}

/// <summary>
/// Classe base para eventos customizados
/// </summary>
public abstract class CustomGameEvent
{
    private static uint _nextId = 1;
    
    public uint Id { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract TimeSpan Duration { get; }
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public bool IsActive { get; private set; }

    protected static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

    protected CustomGameEvent()
    {
        Id = _nextId++;
    }

    /// <summary>
    /// Inicia o evento
    /// </summary>
    public virtual void Start()
    {
        StartTime = DateTime.UtcNow;
        IsActive = true;
        Logger.Info($"Event {Name} started");
    }

    /// <summary>
    /// Para o evento
    /// </summary>
    public virtual void Stop()
    {
        EndTime = DateTime.UtcNow;
        IsActive = false;
        Logger.Info($"Event {Name} stopped");
    }

    /// <summary>
    /// Chamado quando um jogador participa do evento
    /// </summary>
    public virtual void OnPlayerParticipate(Character player)
    {
        // Override in derived classes
    }

    /// <summary>
    /// Obtém informações do evento
    /// </summary>
    public virtual string GetEventInfo()
    {
        var duration = Duration == TimeSpan.Zero ? "Permanent" : Duration.ToString();
        var status = IsActive ? "Active" : "Inactive";
        
        return $"Event: {Name}\nDescription: {Description}\nDuration: {duration}\nStatus: {status}";
    }
}

/// <summary>
/// Task para finalizar eventos automaticamente
/// </summary>
public class EventEndTask : AAEmu.Game.Models.Tasks.Task
{
    private readonly uint _eventId;

    public EventEndTask(uint eventId)
    {
        _eventId = eventId;
    }

    public override void Execute()
    {
        GameEventManager.Instance.StopEvent(_eventId);
    }
}