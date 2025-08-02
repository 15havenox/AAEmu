using AAEmu.Game.Core.Managers;
using AAEmu.Game.Models.Game.Char;
using AAEmu.Game.Scripts.Commands;
using NLog;

namespace AAEmu.Game.Scripts.Commands;

public class AdminNames : ICommand
{
    protected static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    public string CommandName => "adminnames";
    public string Help => "Manage admin names with special characters";
    public string Description => "Create, list, or remove admin names with special characters";

    public void Execute(Character character, string[] args)
    {
        if (args.Length == 0)
        {
            character.SendMessage("Usage: .adminnames [create|list|remove|test] [name]");
            character.SendMessage("Examples:");
            character.SendMessage("  .adminnames create [ADM]Havenox");
            character.SendMessage("  .adminnames list");
            character.SendMessage("  .adminnames remove [ADM]Havenox");
            character.SendMessage("  .adminnames test [ADM]TestName");
            return;
        }

        var subCommand = args[0].ToLower();

        switch (subCommand)
        {
            case "create":
                if (args.Length < 2)
                {
                    character.SendMessage("Usage: .adminnames create [name]");
                    return;
                }
                CreateAdminName(character, args[1]);
                break;
            case "list":
                ListAdminNames(character);
                break;
            case "remove":
                if (args.Length < 2)
                {
                    character.SendMessage("Usage: .adminnames remove [name]");
                    return;
                }
                RemoveAdminName(character, args[1]);
                break;
            case "test":
                if (args.Length < 2)
                {
                    character.SendMessage("Usage: .adminnames test [name]");
                    return;
                }
                TestAdminName(character, args[1]);
                break;
            default:
                character.SendMessage("Unknown subcommand. Use: create, list, remove, test");
                break;
        }
    }

    private void CreateAdminName(Character character, string name)
    {
        // Check if name follows admin pattern
        if (!IsValidAdminName(name))
        {
            character.SendMessage($"Invalid admin name format: {name}");
            character.SendMessage("Valid formats: [ADM]Name, [GM]Name, [DEV]Name, etc.");
            return;
        }

        // Check if name already exists
        var existingId = NameManager.Instance.GetCharacterId(name);
        if (existingId != 0)
        {
            character.SendMessage($"Name '{name}' already exists (ID: {existingId})");
            return;
        }

        // Create the character in database
        try
        {
            // This would need to be implemented in CharacterManager
            // For now, just show the validation result
            var validationResult = NameManager.Instance.ValidateCharacterName(name);
            character.SendMessage($"Validation result for '{name}': {validationResult}");
            
            if (validationResult == CharacterCreateError.Ok)
            {
                character.SendMessage($"Admin name '{name}' is valid and can be created");
                character.SendMessage("Note: You'll need to create the character manually in the database");
            }
            else
            {
                character.SendMessage($"Admin name '{name}' is not valid: {validationResult}");
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Error creating admin name '{name}': {ex}");
            character.SendMessage($"Error creating admin name: {ex.Message}");
        }
    }

    private void ListAdminNames(Character character)
    {
        character.SendMessage("=== Admin Name Patterns ===");
        character.SendMessage("[ADM]Name - Administrator");
        character.SendMessage("[GM]Name - Game Master");
        character.SendMessage("[DEV]Name - Developer");
        character.SendMessage("[MOD]Name - Moderator");
        character.SendMessage("[ADMIN]Name - Admin");
        character.SendMessage("[STAFF]Name - Staff");
        character.SendMessage("[HELPER]Name - Helper");
        character.SendMessage("[SUPPORT]Name - Support");
        character.SendMessage("[OWNER]Name - Owner");
        character.SendMessage("[SERVER]Name - Server");
        character.SendMessage("[SYSTEM]Name - System");
        character.SendMessage("[BOT]Name - Bot");
        character.SendMessage("[NPC]Name - NPC");
        character.SendMessage("[TEST]Name - Test");
        character.SendMessage("[DEBUG]Name - Debug");
        character.SendMessage("[INFO]Name - Info");
        character.SendMessage("[WARN]Name - Warning");
        character.SendMessage("[ERROR]Name - Error");
        character.SendMessage("[CRITICAL]Name - Critical");
        character.SendMessage("[EMERGENCY]Name - Emergency");
    }

    private void RemoveAdminName(Character character, string name)
    {
        var characterId = NameManager.Instance.GetCharacterId(name);
        if (characterId == 0)
        {
            character.SendMessage($"Admin name '{name}' not found");
            return;
        }

        character.SendMessage($"Admin name '{name}' found (ID: {characterId})");
        character.SendMessage("Note: You'll need to remove the character manually from the database");
    }

    private void TestAdminName(Character character, string name)
    {
        character.SendMessage($"=== Testing Admin Name: {name} ===");
        
        var isValid = IsValidAdminName(name);
        character.SendMessage($"Valid admin format: {isValid}");
        
        var validationResult = NameManager.Instance.ValidateCharacterName(name);
        character.SendMessage($"Validation result: {validationResult}");
        
        var existingId = NameManager.Instance.GetCharacterId(name);
        character.SendMessage($"Existing character ID: {existingId}");
        
        if (existingId != 0)
        {
            var existingName = NameManager.Instance.GetCharacterName(existingId);
            character.SendMessage($"Existing character name: {existingName}");
        }
    }

    private bool IsValidAdminName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        // Admin name patterns
        var adminPatterns = new[]
        {
            @"^\[ADM\].*$",
            @"^\[GM\].*$",
            @"^\[DEV\].*$",
            @"^\[MOD\].*$",
            @"^\[ADMIN\].*$",
            @"^\[STAFF\].*$",
            @"^\[HELPER\].*$",
            @"^\[SUPPORT\].*$",
            @"^\[OWNER\].*$",
            @"^\[SERVER\].*$",
            @"^\[SYSTEM\].*$",
            @"^\[BOT\].*$",
            @"^\[NPC\].*$",
            @"^\[TEST\].*$",
            @"^\[DEBUG\].*$",
            @"^\[INFO\].*$",
            @"^\[WARN\].*$",
            @"^\[ERROR\].*$",
            @"^\[CRITICAL\].*$",
            @"^\[EMERGENCY\].*$"
        };

        foreach (var pattern in adminPatterns)
        {
            if (Regex.IsMatch(name, pattern, RegexOptions.IgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}