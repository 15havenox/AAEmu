using AAEmu.Game.Core.Managers;
using AAEmu.Game.Models.Game.Char;

namespace AAEmu.Game.Modules;

/// <summary>
/// Módulo de exemplo demonstrando como criar eventos customizados
/// </summary>
public class ExampleEventModule : GameModuleBase, IEventModule
{
    public override string Name => "ExampleEventModule";
    public override string Version => "1.0.0";

    public override void Initialize()
    {
        base.Initialize();
        RegisterEvents();
    }

    public override void Shutdown()
    {
        UnregisterEvents();
        base.Shutdown();
    }

    public void RegisterEvents()
    {
        // Registrar handlers de eventos
        GameEventManager.Instance.RegisterEventHandler<PlayerEventArgs>("PlayerLogin", OnPlayerLogin);
        GameEventManager.Instance.RegisterEventHandler<PlayerEventArgs>("PlayerLogout", OnPlayerLogout);
        GameEventManager.Instance.RegisterEventHandler<CombatEventArgs>("PlayerKill", OnPlayerKill);
        
        Logger.Info("Example Event Module events registered");
    }

    public void UnregisterEvents()
    {
        // Em uma implementação real, você manteria referências aos handlers para poder removê-los
        Logger.Info("Example Event Module events unregistered");
    }

    public override void OnPlayerJoin(Character player)
    {
        // Trigger custom event when player joins
        GameEventManager.Instance.TriggerEvent("PlayerLogin", new PlayerEventArgs(player));
        
        // Example: Give welcome bonus
        player.SendMessage($"Welcome {player.Name}! You've received a login bonus!");
    }

    public override void OnPlayerLeave(Character player)
    {
        // Trigger custom event when player leaves
        GameEventManager.Instance.TriggerEvent("PlayerLogout", new PlayerEventArgs(player));
    }

    private void OnPlayerLogin(PlayerEventArgs args)
    {
        Logger.Info($"Player {args.Player.Name} logged in");
        
        // Example: Start a daily bonus event for the player
        var dailyBonusEvent = new DailyBonusEvent(args.Player);
        GameEventManager.Instance.StartEvent(dailyBonusEvent);
    }

    private void OnPlayerLogout(PlayerEventArgs args)
    {
        Logger.Info($"Player {args.Player.Name} logged out");
    }

    private void OnPlayerKill(CombatEventArgs args)
    {
        if (args.Target is Character targetPlayer && args.Attacker is Character attackerPlayer)
        {
            Logger.Info($"Player {attackerPlayer.Name} killed {targetPlayer.Name}");
            
            // Example: Award PvP points
            attackerPlayer.SendMessage("You gained PvP points for the kill!");
        }
    }
}

/// <summary>
/// Exemplo de evento customizado - Bônus diário
/// </summary>
public class DailyBonusEvent : CustomGameEvent
{
    private readonly Character _player;

    public override string Name => "Daily Bonus Event";
    public override string Description => "Daily login bonus for active players";
    public override TimeSpan Duration => TimeSpan.FromHours(24); // 24 horas

    public DailyBonusEvent(Character player)
    {
        _player = player;
    }

    public override void Start()
    {
        base.Start();
        
        // Give bonus items/experience/currency
        _player.SendMessage("Daily bonus activated! You'll receive bonuses for the next 24 hours.");
        
        // Example: Increase experience gain by 50%
        // _player.AddBuff(ExperienceBonusBuff);
    }

    public override void Stop()
    {
        base.Stop();
        
        if (_player.IsOnline)
        {
            _player.SendMessage("Daily bonus has expired. See you tomorrow!");
        }
    }

    public override void OnPlayerParticipate(Character player)
    {
        if (player.Id == _player.Id)
        {
            // Player is actively playing, could extend bonus or give additional rewards
            player.SendMessage("You're actively playing! Bonus time extended!");
        }
    }
}

/// <summary>
/// Exemplo de evento de servidor - Double XP Weekend
/// </summary>
public class DoubleXpWeekendEvent : CustomGameEvent
{
    public override string Name => "Double XP Weekend";
    public override string Description => "Double experience points for all players during weekend";
    public override TimeSpan Duration => TimeSpan.FromHours(48); // 48 horas

    private readonly HashSet<uint> _participatingPlayers = [];

    public override void Start()
    {
        base.Start();
        
        // Broadcast to all players
        ChatManager.Instance.BroadcastNotice("🎉 Double XP Weekend has started! Gain double experience for 48 hours!");
        
        Logger.Info("Double XP Weekend event started");
    }

    public override void Stop()
    {
        base.Stop();
        
        ChatManager.Instance.BroadcastNotice("Double XP Weekend has ended. Thanks for playing!");
        
        Logger.Info($"Double XP Weekend event ended. {_participatingPlayers.Count} players participated");
    }

    public override void OnPlayerParticipate(Character player)
    {
        if (_participatingPlayers.Add(player.Id))
        {
            player.SendMessage("You're now part of the Double XP Weekend! Enjoy the bonus experience!");
            
            // Apply double XP buff or modifier
            // ApplyDoubleXpBonus(player);
        }
    }
}

/// <summary>
/// Evento de raid/boss mundial
/// </summary>
public class WorldBossEvent : CustomGameEvent
{
    private readonly uint _bossNpcId;
    private readonly string _zoneName;

    public override string Name => "World Boss Event";
    public override string Description => $"A powerful boss has appeared in {_zoneName}!";
    public override TimeSpan Duration => TimeSpan.FromMinutes(30); // 30 minutos

    public WorldBossEvent(uint bossNpcId, string zoneName)
    {
        _bossNpcId = bossNpcId;
        _zoneName = zoneName;
    }

    public override void Start()
    {
        base.Start();
        
        // Spawn the world boss
        // var boss = NpcManager.Instance.SpawnWorldBoss(_bossNpcId, location);
        
        ChatManager.Instance.BroadcastNotice($"🔥 A powerful boss has appeared in {_zoneName}! Defeat it for rare rewards!");
        
        Logger.Info($"World Boss Event started - Boss ID: {_bossNpcId} in {_zoneName}");
    }

    public override void Stop()
    {
        base.Stop();
        
        // Remove boss if still alive
        // RemoveWorldBoss(_bossNpcId);
        
        ChatManager.Instance.BroadcastNotice("The World Boss event has ended.");
    }

    public override void OnPlayerParticipate(Character player)
    {
        // Track players who damage the boss
        player.SendMessage("You're now participating in the World Boss fight! Good luck!");
    }
}