using AAEmu.Game.Core.Managers;
using AAEmu.Game.Core.Managers.World;
using AAEmu.Game.Models.Game.Char;
using AAEmu.Game.Scripts.Commands;
using NLog;

namespace AAEmu.Game.Scripts.Commands;

public class TestTradePacks : ICommand
{
    protected static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    public string CommandName => "testtradepacks";
    public string Help => "Test trade packs system";
    public string Description => "Test various aspects of the trade packs system";

    public void Execute(Character character, string[] args)
    {
        if (args.Length == 0)
        {
            character.SendMessage("Usage: .testtradepacks [info|mail|npc|ratio]");
            return;
        }

        var subCommand = args[0].ToLower();

        switch (subCommand)
        {
            case "info":
                TestTradePackInfo(character);
                break;
            case "mail":
                TestTradePackMail(character);
                break;
            case "npc":
                TestTradePackNpc(character);
                break;
            case "ratio":
                TestTradePackRatio(character);
                break;
            default:
                character.SendMessage("Unknown subcommand. Use: info, mail, npc, ratio");
                break;
        }
    }

    private void TestTradePackInfo(Character character)
    {
        var backpack = character.Inventory.Equipment.GetItemBySlot((int)EquipmentItemSlot.Backpack);
        
        character.SendMessage("=== Trade Pack Info ===");
        character.SendMessage($"Has backpack: {backpack != null}");
        
        if (backpack != null)
        {
            character.SendMessage($"Backpack TemplateId: {backpack.TemplateId}");
            character.SendMessage($"MadeUnitId: {backpack.MadeUnitId}");
            character.SendMessage($"CrafterId: {backpack.MadeUnitId != character.Id ? backpack.MadeUnitId : 0}");
            character.SendMessage($"Is Trade Pack: {backpack.Template is BackpackTemplate { BackpackType: BackpackType.TradePack } }");
        }

        var zoneGroupId = ZoneManager.Instance.GetZoneByKey(character.Transform.ZoneId)?.GroupId ?? 0;
        character.SendMessage($"Current Zone Group: {zoneGroupId}");
        character.SendMessage($"Labor Power: {character.LaborPower}");
    }

    private void TestTradePackMail(Character character)
    {
        character.SendMessage("=== Testing Trade Pack Mail ===");
        
        // Test mail creation
        var testMail = new MailForSpeciality(character, 0, 10001, 100, Item.Coins, 50000, 0, 50000, 0, 5);
        testMail.FinalizeForSeller();
        
        if (testMail.Send())
        {
            character.SendMessage("Test mail sent successfully!");
        }
        else
        {
            character.SendMessage("Failed to send test mail!");
        }
    }

    private void TestTradePackNpc(Character character)
    {
        character.SendMessage("=== Testing Trade Pack NPCs ===");
        
        var specialtyNpcs = SpecialtyManager.Instance._specialtyNpc;
        character.SendMessage($"Total specialty NPCs loaded: {specialtyNpcs.Count}");
        
        foreach (var npc in specialtyNpcs.Take(5)) // Show first 5
        {
            character.SendMessage($"NPC {npc.Key}: {npc.Value.Name} (Bundle: {npc.Value.SpecialtyBundleId})");
        }
        
        var bundleItems = SpecialtyManager.Instance._specialtyBundleItemsMapped;
        character.SendMessage($"Total bundle items loaded: {bundleItems.Count}");
    }

    private void TestTradePackRatio(Character character)
    {
        character.SendMessage("=== Testing Trade Pack Ratios ===");
        
        var ratio = SpecialtyManager.Instance.GetRatioForSpecialty(character);
        character.SendMessage($"Current ratio for player: {ratio}");
        
        var backpack = character.Inventory.Equipment.GetItemBySlot((int)EquipmentItemSlot.Backpack);
        if (backpack != null)
        {
            var zoneGroupId = ZoneManager.Instance.GetZoneByKey(character.Transform.ZoneId)?.GroupId ?? 0;
            character.SendMessage($"Zone Group: {zoneGroupId}, Backpack: {backpack.TemplateId}");
            
            // Test ratio for different zones
            for (uint zone = 1; zone <= 3; zone++)
            {
                var testRatio = SpecialtyManager.Instance.GetRatiosForTargetRoute(zoneGroupId, zone);
                character.SendMessage($"Route {zoneGroupId} -> {zone}: {testRatio.Count} items available");
            }
        }
    }
}