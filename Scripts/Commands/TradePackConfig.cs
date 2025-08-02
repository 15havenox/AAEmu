using AAEmu.Game.Core.Managers;
using AAEmu.Game.Models.Game.Char;
using AAEmu.Game.Scripts.Commands;
using NLog;

namespace AAEmu.Game.Scripts.Commands;

public class TradePackConfig : ICommand
{
    protected static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    public string CommandName => "tradepackconfig";
    public string Help => "Configure trade pack settings";
    public string Description => "View and modify trade pack configuration settings";

    public void Execute(Character character, string[] args)
    {
        if (args.Length == 0)
        {
            ShowCurrentConfig(character);
            return;
        }

        var subCommand = args[0].ToLower();

        switch (subCommand)
        {
            case "show":
                ShowCurrentConfig(character);
                break;
            case "instant":
                if (args.Length < 2)
                {
                    character.SendMessage("Usage: .tradepackconfig instant [true|false]");
                    return;
                }
                SetInstantDelivery(character, args[1]);
                break;
            case "delay":
                if (args.Length < 2)
                {
                    character.SendMessage("Usage: .tradepackconfig delay [hours]");
                    return;
                }
                SetMailDelay(character, args[1]);
                break;
            case "interest":
                if (args.Length < 2)
                {
                    character.SendMessage("Usage: .tradepackconfig interest [percentage]");
                    return;
                }
                SetInterestRate(character, args[1]);
                break;
            case "labor":
                if (args.Length < 2)
                {
                    character.SendMessage("Usage: .tradepackconfig labor [cost]");
                    return;
                }
                SetLaborCost(character, args[1]);
                break;
            case "distance":
                if (args.Length < 2)
                {
                    character.SendMessage("Usage: .tradepackconfig distance [meters]");
                    return;
                }
                SetMaxDistance(character, args[1]);
                break;
            case "share":
                if (args.Length < 2)
                {
                    character.SendMessage("Usage: .tradepackconfig share [percentage]");
                    return;
                }
                SetSellerShare(character, args[1]);
                break;
            case "reset":
                ResetToDefaults(character);
                break;
            default:
                character.SendMessage("Unknown subcommand. Use: show, instant, delay, interest, labor, distance, share, reset");
                break;
        }
    }

    private void ShowCurrentConfig(Character character)
    {
        var config = AppConfiguration.Instance.Specialty;
        
        character.SendMessage("=== Trade Pack Configuration ===");
        character.SendMessage($"Instant Delivery: {config.InstantTradePackDelivery}");
        character.SendMessage($"Mail Delay: {config.TradePackMailDelayHours} hours");
        character.SendMessage($"Interest Rate: {config.TradePackInterestRate}%");
        character.SendMessage($"Labor Cost: {config.TradePackLaborCost}");
        character.SendMessage($"Max Distance: {config.TradePackMaxDistance} meters");
        character.SendMessage($"Seller Share: {config.TradePackSellerShare * 100}%");
        character.SendMessage($"Max Ratio: {config.MaxSpecialtyRatio}%");
        character.SendMessage($"Min Ratio: {config.MinSpecialtyRatio}%");
        character.SendMessage($"Ratio Decrease Per Pack: {config.RatioDecreasePerPack}%");
        character.SendMessage($"Ratio Increase Per Tick: {config.RatioIncreasePerTick}%");
        character.SendMessage($"Ratio Decrease Tick: {config.RatioDecreaseTickMinutes} minutes");
        character.SendMessage($"Ratio Regen Tick: {config.RatioRegenTickMinutes} minutes");
    }

    private void SetInstantDelivery(Character character, string value)
    {
        if (!bool.TryParse(value, out var instant))
        {
            character.SendMessage("Invalid value. Use 'true' or 'false'");
            return;
        }

        AppConfiguration.Instance.Specialty.InstantTradePackDelivery = instant;
        character.SendMessage($"Instant trade pack delivery set to: {instant}");
        
        if (instant)
        {
            character.SendMessage("Trade packs will now be delivered instantly!");
        }
        else
        {
            character.SendMessage($"Trade packs will be delivered after {AppConfiguration.Instance.Specialty.TradePackMailDelayHours} hours");
        }
    }

    private void SetMailDelay(Character character, string value)
    {
        if (!double.TryParse(value, out var delay) || delay < 0)
        {
            character.SendMessage("Invalid value. Use a positive number of hours");
            return;
        }

        AppConfiguration.Instance.Specialty.TradePackMailDelayHours = delay;
        character.SendMessage($"Trade pack mail delay set to: {delay} hours");
    }

    private void SetInterestRate(Character character, string value)
    {
        if (!int.TryParse(value, out var rate) || rate < 0)
        {
            character.SendMessage("Invalid value. Use a positive percentage");
            return;
        }

        AppConfiguration.Instance.Specialty.TradePackInterestRate = rate;
        character.SendMessage($"Trade pack interest rate set to: {rate}%");
    }

    private void SetLaborCost(Character character, string value)
    {
        if (!int.TryParse(value, out var cost) || cost < 0)
        {
            character.SendMessage("Invalid value. Use a positive number");
            return;
        }

        AppConfiguration.Instance.Specialty.TradePackLaborCost = cost;
        character.SendMessage($"Trade pack labor cost set to: {cost}");
    }

    private void SetMaxDistance(Character character, string value)
    {
        if (!double.TryParse(value, out var distance) || distance < 0)
        {
            character.SendMessage("Invalid value. Use a positive number of meters");
            return;
        }

        AppConfiguration.Instance.Specialty.TradePackMaxDistance = distance;
        character.SendMessage($"Trade pack max distance set to: {distance} meters");
    }

    private void SetSellerShare(Character character, string value)
    {
        if (!double.TryParse(value, out var share) || share < 0 || share > 100)
        {
            character.SendMessage("Invalid value. Use a percentage between 0 and 100");
            return;
        }

        AppConfiguration.Instance.Specialty.TradePackSellerShare = share / 100.0;
        character.SendMessage($"Trade pack seller share set to: {share}%");
    }

    private void ResetToDefaults(Character character)
    {
        var config = AppConfiguration.Instance.Specialty;
        
        config.InstantTradePackDelivery = false;
        config.TradePackMailDelayHours = 8.0;
        config.TradePackInterestRate = 5;
        config.TradePackLaborCost = 60;
        config.TradePackMaxDistance = 2.5;
        config.TradePackSellerShare = 0.80;
        config.MaxSpecialtyRatio = 130;
        config.MinSpecialtyRatio = 70;
        config.RatioDecreasePerPack = 0.5;
        config.RatioIncreasePerTick = 5.0;
        config.RatioDecreaseTickMinutes = 1.0;
        config.RatioRegenTickMinutes = 60.0;
        
        character.SendMessage("Trade pack configuration reset to defaults");
    }
}