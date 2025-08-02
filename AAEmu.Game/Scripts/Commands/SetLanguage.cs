using AAEmu.Game.Core.Managers;
using AAEmu.Game.Core.Packets.G2C;
using AAEmu.Game.Models.Game;
using AAEmu.Game.Models.Game.Char;
using AAEmu.Game.Models.Game.Chat;

namespace AAEmu.Game.Scripts.Commands;

public class SetLanguage : ICommand
{
    public void OnLoad()
    {
        string[] name = { "setlang", "language", "idioma" };
        CommandManager.Instance.Register(name, this);
    }

    public string GetCommandLineHelp()
    {
        return "(target) <language>";
    }

    public string GetCommandHelpText()
    {
        return "Muda o idioma preferido do jogador. Idiomas disponíveis: " + 
               string.Join(", ", LocalizationManager.Instance.GetAvailableLanguages()) +
               "\nExemplos: /setlang pt_br, /language en_us";
    }

    public bool CanExecute(Character character, string[] args, int targetLevel)
    {
        return true; // Todos podem usar este comando
    }

    public bool Execute(Character character, string[] args, IMessageOutput messageOutput)
    {
        if (args.Length < 1)
        {
            var availableLanguages = LocalizationManager.Instance.GetAvailableLanguages();
            var currentLang = character.GetEffectiveLanguage();
            
            messageOutput.SendMessage(ChatType.System, 
                $"Idioma atual: {currentLang}\n" +
                $"Idiomas disponíveis: {string.Join(", ", availableLanguages)}\n" +
                $"Use: /setlang <idioma> (exemplo: /setlang pt_br)");
            return true;
        }

        var targetLanguage = args[0].ToLower();
        
        // Verificar se o idioma está disponível
        if (!LocalizationManager.Instance.IsLanguageAvailable(targetLanguage))
        {
            var availableLanguages = LocalizationManager.Instance.GetAvailableLanguages();
            messageOutput.SendMessage(ChatType.System, 
                $"Idioma '{targetLanguage}' não está disponível.\n" +
                $"Idiomas disponíveis: {string.Join(", ", availableLanguages)}");
            return false;
        }

        // Definir idioma preferido
        character.PreferredLanguage = targetLanguage;
        
        // Salvar no banco de dados
        character.Save();

        // Confirmar mudança
        messageOutput.SendMessage(ChatType.System, 
            $"Idioma alterado para: {targetLanguage}\n" +
            $"Reconecte-se para que algumas mudanças tenham efeito completo.");

        // Opcional: Atualizar interface imediatamente (se houver packets específicos)
        // character.SendPacket(new SCLanguageChangedPacket(targetLanguage));

        return true;
    }
}