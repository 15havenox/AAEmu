using AAEmu.Game.Core.Managers;
using AAEmu.Game.Models.Game.Char;
using AAEmu.Game.Modules;
using AAEmu.Game.Utils.Scripts.SubCommands;

namespace AAEmu.Game.Utils.Scripts.Commands;

public class ServerManagementCommands : ICommand
{
    public void OnLoad()
    {
        CommandManager.Instance.Register("server", this);
    }

    public string[] CommandNames { get; } = { "server", "srv" };

    public void Execute(Character character, string[] args, IMessageOutput messageOutput)
    {
        if (args.Length == 0)
        {
            ShowHelp(messageOutput);
            return;
        }

        var subCommand = args[0].ToLower();

        switch (subCommand)
        {
            case "metrics":
                HandleMetricsCommand(character, args, messageOutput);
                break;
            case "modules":
                HandleModulesCommand(character, args, messageOutput);
                break;
            case "events":
                HandleEventsCommand(character, args, messageOutput);
                break;
            case "health":
                HandleHealthCommand(character, messageOutput);
                break;
            case "test":
                HandleTestCommand(character, args, messageOutput);
                break;
            case "protection":
                HandleProtectionCommand(character, args, messageOutput);
                break;
            default:
                ShowHelp(messageOutput);
                break;
        }
    }

    private void ShowHelp(IMessageOutput messageOutput)
    {
        messageOutput.SendMessage("=== Server Management Commands ===");
        messageOutput.SendMessage("/server metrics - Show server metrics");
        messageOutput.SendMessage("/server modules [list|enable|disable] [name] - Manage modules");
        messageOutput.SendMessage("/server events [list|start|stop] [type] - Manage events");
        messageOutput.SendMessage("/server health - Show server health status");
        messageOutput.SendMessage("/server protection [report|unban] [ip] - Network protection");
        messageOutput.SendMessage("/server test [event] - Test various systems");
    }

    private void HandleMetricsCommand(Character character, string[] args, IMessageOutput messageOutput)
    {
        if (args.Length > 1 && args[1].ToLower() == "json")
        {
            var jsonMetrics = ServerMetricsManager.Instance.GetMetricsJson();
            messageOutput.SendMessage($"Metrics JSON: {System.Text.Json.JsonSerializer.Serialize(jsonMetrics, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })}");
        }
        else
        {
            var report = ServerMetricsManager.Instance.GetMetricsReport();
            var lines = report.Split('\n');
            foreach (var line in lines)
            {
                messageOutput.SendMessage(line);
            }
        }
    }

    private void HandleModulesCommand(Character character, string[] args, IMessageOutput messageOutput)
    {
        if (args.Length < 2)
        {
            messageOutput.SendMessage("Usage: /server modules [list|enable|disable|reload] [name]");
            return;
        }

        var action = args[1].ToLower();

        switch (action)
        {
            case "list":
                var modules = ModuleManager.Instance.LoadedModules;
                messageOutput.SendMessage($"=== Loaded Modules ({modules.Count}) ===");
                foreach (var module in modules)
                {
                    var status = module.Value.IsEnabled ? "ENABLED" : "DISABLED";
                    messageOutput.SendMessage($"{module.Key} v{module.Value.Version} - {status}");
                }
                break;

            case "enable":
                if (args.Length < 3)
                {
                    messageOutput.SendMessage("Usage: /server modules enable <module_name>");
                    return;
                }
                var enableResult = ModuleManager.Instance.SetModuleEnabled(args[2], true);
                messageOutput.SendMessage(enableResult ? $"Module {args[2]} enabled" : $"Failed to enable module {args[2]}");
                break;

            case "disable":
                if (args.Length < 3)
                {
                    messageOutput.SendMessage("Usage: /server modules disable <module_name>");
                    return;
                }
                var disableResult = ModuleManager.Instance.SetModuleEnabled(args[2], false);
                messageOutput.SendMessage(disableResult ? $"Module {args[2]} disabled" : $"Failed to disable module {args[2]}");
                break;

            case "reload":
                ModuleManager.Instance.ReloadExternalModules();
                messageOutput.SendMessage("External modules reloaded");
                break;

            default:
                messageOutput.SendMessage("Invalid action. Use: list, enable, disable, or reload");
                break;
        }
    }

    private void HandleEventsCommand(Character character, string[] args, IMessageOutput messageOutput)
    {
        if (args.Length < 2)
        {
            messageOutput.SendMessage("Usage: /server events [list|start|stop] [id]");
            return;
        }

        var action = args[1].ToLower();

        switch (action)
        {
            case "list":
                var events = GameEventManager.Instance.ActiveEvents;
                messageOutput.SendMessage($"=== Active Events ({events.Count}) ===");
                foreach (var evt in events)
                {
                    messageOutput.SendMessage($"ID: {evt.Key}, Name: {evt.Value.Name}, Active: {evt.Value.IsActive}");
                    messageOutput.SendMessage($"  Description: {evt.Value.Description}");
                    messageOutput.SendMessage($"  Duration: {evt.Value.Duration}");
                }
                break;

            case "start":
                if (args.Length < 3)
                {
                    messageOutput.SendMessage("Available events: doubleexp, worldboss, daily");
                    return;
                }
                StartTestEvent(args[2], character, messageOutput);
                break;

            case "stop":
                if (args.Length < 3 || !uint.TryParse(args[2], out var eventId))
                {
                    messageOutput.SendMessage("Usage: /server events stop <event_id>");
                    return;
                }
                var stopResult = GameEventManager.Instance.StopEvent(eventId);
                messageOutput.SendMessage(stopResult ? $"Event {eventId} stopped" : $"Failed to stop event {eventId}");
                break;

            default:
                messageOutput.SendMessage("Invalid action. Use: list, start, or stop");
                break;
        }
    }

    private void StartTestEvent(string eventType, Character character, IMessageOutput messageOutput)
    {
        CustomGameEvent? gameEvent = eventType.ToLower() switch
        {
            "doubleexp" => new DoubleXpWeekendEvent(),
            "worldboss" => new WorldBossEvent(1001, "Test Zone"),
            "daily" => new DailyBonusEvent(character),
            _ => null
        };

        if (gameEvent == null)
        {
            messageOutput.SendMessage($"Unknown event type: {eventType}");
            return;
        }

        var result = GameEventManager.Instance.StartEvent(gameEvent);
        messageOutput.SendMessage(result ? $"Started {gameEvent.Name} (ID: {gameEvent.Id})" : $"Failed to start {gameEvent.Name}");
    }

    private void HandleHealthCommand(Character character, IMessageOutput messageOutput)
    {
        var health = ServerMetricsManager.Instance.GetHealthStatus();
        
        messageOutput.SendMessage($"=== Server Health Status ===");
        messageOutput.SendMessage($"Overall Status: {(health.IsHealthy ? "HEALTHY" : "UNHEALTHY")}");
        
        if (health.Issues.Any())
        {
            messageOutput.SendMessage("Issues:");
            foreach (var issue in health.Issues)
            {
                messageOutput.SendMessage($"  - {issue}");
            }
        }
        
        if (health.Warnings.Any())
        {
            messageOutput.SendMessage("Warnings:");
            foreach (var warning in health.Warnings)
            {
                messageOutput.SendMessage($"  - {warning}");
            }
        }

        if (health.IsHealthy && !health.Warnings.Any())
        {
            messageOutput.SendMessage("All systems operational");
        }
    }

    private void HandleTestCommand(Character character, string[] args, IMessageOutput messageOutput)
    {
        if (args.Length < 2)
        {
            messageOutput.SendMessage("Usage: /server test [event|module|metrics]");
            return;
        }

        var testType = args[1].ToLower();

        switch (testType)
        {
            case "event":
                // Test custom event system
                GameEventManager.Instance.TriggerEvent("TestEvent", new PlayerEventArgs(character));
                messageOutput.SendMessage("Test event triggered");
                break;

            case "module":
                // Test module system
                var testModule = ModuleManager.Instance.GetModule<ExampleEventModule>("ExampleEventModule");
                if (testModule != null)
                {
                    messageOutput.SendMessage($"Example module found: {testModule.Name} v{testModule.Version}");
                }
                else
                {
                    messageOutput.SendMessage("Example module not found");
                }
                break;

            case "metrics":
                // Test metrics recording
                ServerMetricsManager.Instance.RecordPacketReceived();
                ServerMetricsManager.Instance.RecordPacketSent();
                ServerMetricsManager.Instance.SetMetric("test_metric", DateTime.UtcNow.ToString());
                messageOutput.SendMessage("Test metrics recorded");
                break;

            default:
                messageOutput.SendMessage("Invalid test type. Use: event, module, or metrics");
                break;
        }
    }

    private void HandleProtectionCommand(Character character, string[] args, IMessageOutput messageOutput)
    {
        if (args.Length < 2)
        {
            messageOutput.SendMessage("Usage: /server protection [report|unban|ban] [ip]");
            return;
        }

        var action = args[1].ToLower();
        var protectionManager = AAEmu.Game.Core.Network.Protection.NetworkProtectionManager.Instance;

        switch (action)
        {
            case "report":
                var report = protectionManager.GetProtectionReport();
                var lines = report.Split('\n');
                foreach (var line in lines)
                {
                    messageOutput.SendMessage(line);
                }
                break;

            case "unban":
                if (args.Length < 3)
                {
                    messageOutput.SendMessage("Usage: /server protection unban <ip>");
                    return;
                }
                var ipToUnban = args[2];
                // Manual unban by removing from banned list (would need to add this method)
                messageOutput.SendMessage($"Manual unban feature would be implemented here for IP: {ipToUnban}");
                break;

            case "ban":
                if (args.Length < 3)
                {
                    messageOutput.SendMessage("Usage: /server protection ban <ip>");
                    return;
                }
                var ipToBan = args[2];
                protectionManager.BanIP(ipToBan, "Manual ban by GM");
                messageOutput.SendMessage($"IP {ipToBan} has been banned manually");
                break;

            case "stats":
                var stats = protectionManager.Stats;
                messageOutput.SendMessage("=== Protection Statistics ===");
                messageOutput.SendMessage($"Packets Processed: {stats.PacketsProcessed:N0}");
                messageOutput.SendMessage($"Packets Blocked: {stats.PacketsBlocked:N0}");
                messageOutput.SendMessage($"Malformed Packets: {stats.MalformedPackets:N0}");
                messageOutput.SendMessage($"Unknown Packets: {stats.UnknownPackets:N0}");
                messageOutput.SendMessage($"IPs Banned: {stats.IPsBanned:N0}");
                break;

            default:
                messageOutput.SendMessage("Invalid action. Use: report, unban, ban, or stats");
                break;
        }
    }
}