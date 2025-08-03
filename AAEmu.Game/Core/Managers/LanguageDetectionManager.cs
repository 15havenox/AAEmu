using AAEmu.Commons.Utils;
using AAEmu.Game.Models.Game.Char;
using NLog;
using System.Globalization;
using System.Net;

namespace AAEmu.Game.Core.Managers;

public class LanguageDetectionManager : Singleton<LanguageDetectionManager>
{
    private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

    private readonly Dictionary<string, string> _countryToLanguage = new()
    {
        // Português
        ["BR"] = "pt_br",
        ["PT"] = "pt_br",
        ["AO"] = "pt_br", // Angola
        ["MZ"] = "pt_br", // Moçambique
        
        // Inglês
        ["US"] = "en_us",
        ["GB"] = "en_us",
        ["CA"] = "en_us",
        ["AU"] = "en_us",
        ["NZ"] = "en_us",
        
        // Espanhol
        ["ES"] = "es_es",
        ["MX"] = "es_es",
        ["AR"] = "es_es",
        ["CO"] = "es_es",
        ["CL"] = "es_es",
        ["PE"] = "es_es",
        
        // Francês
        ["FR"] = "fr_fr",
        ["BE"] = "fr_fr",
        ["CH"] = "fr_fr",
        
        // Alemão
        ["DE"] = "de_de",
        ["AT"] = "de_de",
        
        // Russo
        ["RU"] = "ru_ru",
        ["BY"] = "ru_ru",
        ["KZ"] = "ru_ru",
        
        // Coreano
        ["KR"] = "ko_kr",
        
        // Japonês
        ["JP"] = "ja_jp",
        
        // Chinês
        ["CN"] = "zh_cn",
        ["TW"] = "zh_tw",
        ["HK"] = "zh_tw",
    };

    public string DetectLanguageFromIP(IPAddress ipAddress)
    {
        try
        {
            // Para IPs locais, usar idioma padrão
            if (IsLocalIP(ipAddress))
            {
                Logger.Debug($"Local IP detected: {ipAddress}, using default language");
                return "";
            }

            // Aqui você pode integrar com um serviço de geolocalização
            // Por exemplo: MaxMind GeoIP2, ip-api.com, etc.
            var countryCode = GetCountryCodeFromIP(ipAddress);
            
            if (!string.IsNullOrEmpty(countryCode) && _countryToLanguage.TryGetValue(countryCode, out var language))
            {
                // Verificar se o idioma está disponível no servidor
                if (LocalizationManager.Instance.IsLanguageAvailable(language))
                {
                    Logger.Info($"Auto-detected language {language} for IP {ipAddress} (Country: {countryCode})");
                    return language;
                }
            }

            Logger.Debug($"Could not auto-detect language for IP {ipAddress} (Country: {countryCode})");
        }
        catch (Exception ex)
        {
            Logger.Warn(ex, $"Error detecting language from IP {ipAddress}");
        }

        return "";
    }

    public string DetectLanguageFromText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "";

        text = text.ToLower();

        // Detectar caracteres específicos de idiomas
        if (ContainsPortugueseCharacters(text))
            return "pt_br";
        
        if (ContainsRussianCharacters(text))
            return "ru_ru";
            
        if (ContainsKoreanCharacters(text))
            return "ko_kr";
            
        if (ContainsJapaneseCharacters(text))
            return "ja_jp";
            
        if (ContainsChineseCharacters(text))
            return "zh_cn";

        // Detectar palavras comuns
        if (ContainsPortugueseWords(text))
            return "pt_br";
            
        if (ContainsSpanishWords(text))
            return "es_es";
            
        if (ContainsFrenchWords(text))
            return "fr_fr";
            
        if (ContainsGermanWords(text))
            return "de_de";

        // Default para inglês se não detectar nada específico
        return "";
    }

    public void AutoSetLanguageOnFirstLogin(Character character)
    {
        // Se o personagem já tem idioma definido, não mudar
        if (!string.IsNullOrEmpty(character.PreferredLanguage))
            return;

        try
        {
            // Tentar detectar por IP (se disponível)
            var connection = character.Connection;
            if (connection?.Ip != null)
            {
                var detectedLanguage = DetectLanguageFromIP(connection.Ip);
                if (!string.IsNullOrEmpty(detectedLanguage))
                {
                    character.PreferredLanguage = detectedLanguage;
                    Logger.Info($"Auto-set language to {detectedLanguage} for character {character.Name} based on IP");
                    return;
                }
            }

            // Fallback: usar configuração regional do sistema se disponível
            var systemLanguage = DetectSystemLanguage();
            if (!string.IsNullOrEmpty(systemLanguage))
            {
                character.PreferredLanguage = systemLanguage;
                Logger.Info($"Auto-set language to {systemLanguage} for character {character.Name} based on system locale");
            }
        }
        catch (Exception ex)
        {
            Logger.Warn(ex, $"Error auto-setting language for character {character.Name}");
        }
    }

    private bool IsLocalIP(IPAddress ipAddress)
    {
        if (IPAddress.IsLoopback(ipAddress))
            return true;

        var bytes = ipAddress.GetAddressBytes();
        
        // 192.168.x.x
        if (bytes[0] == 192 && bytes[1] == 168)
            return true;
            
        // 10.x.x.x
        if (bytes[0] == 10)
            return true;
            
        // 172.16.x.x - 172.31.x.x
        if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31)
            return true;

        return false;
    }

    private string GetCountryCodeFromIP(IPAddress ipAddress)
    {
        // Implementação simplificada - em produção, use um serviço real
        // Exemplo com MaxMind GeoIP2 ou API gratuita como ip-api.com
        
        // Para teste, simular alguns países baseado nos últimos octetos
        var bytes = ipAddress.GetAddressBytes();
        var lastOctet = bytes[bytes.Length - 1];
        
        return lastOctet switch
        {
            < 50 => "BR",   // Simular Brasil
            < 100 => "US",  // Simular EUA
            < 150 => "ES",  // Simular Espanha
            < 200 => "FR",  // Simular França
            _ => "US"       // Default EUA
        };
    }

    private string DetectSystemLanguage()
    {
        try
        {
            var culture = CultureInfo.CurrentCulture;
            var languageCode = culture.TwoLetterISOLanguageName.ToLower();
            var countryCode = culture.Name.Split('-').LastOrDefault()?.ToUpper();

            // Mapear para nossos códigos de idioma
            return languageCode switch
            {
                "pt" => "pt_br",
                "en" => "en_us", 
                "es" => "es_es",
                "fr" => "fr_fr",
                "de" => "de_de",
                "ru" => "ru_ru",
                "ko" => "ko_kr",
                "ja" => "ja_jp",
                "zh" => countryCode == "CN" ? "zh_cn" : "zh_tw",
                _ => ""
            };
        }
        catch
        {
            return "";
        }
    }

    private bool ContainsPortugueseCharacters(string text)
    {
        return text.Any(c => "ãçõáéíóúâêîôûàèìòù".Contains(c));
    }

    private bool ContainsRussianCharacters(string text)
    {
        return text.Any(c => c >= 'а' && c <= 'я');
    }

    private bool ContainsKoreanCharacters(string text)
    {
        return text.Any(c => (c >= '가' && c <= '힣') || (c >= 'ㄱ' && c <= 'ㅣ'));
    }

    private bool ContainsJapaneseCharacters(string text)
    {
        return text.Any(c => (c >= 'ひ' && c <= 'ゖ') || (c >= 'カ' && c <= 'ヿ'));
    }

    private bool ContainsChineseCharacters(string text)
    {
        return text.Any(c => c >= 0x4E00 && c <= 0x9FFF);
    }

    private bool ContainsPortugueseWords(string text)
    {
        string[] words = { "sim", "não", "você", "que", "para", "com", "uma", "ser", "ter", "ele", "por", "bem", "muito", "como", "quando", "onde" };
        return words.Any(word => text.Contains(word));
    }

    private bool ContainsSpanishWords(string text)
    {
        string[] words = { "que", "con", "una", "ser", "tener", "por", "como", "cuando", "donde", "qué", "cómo", "cuándo", "dónde" };
        return words.Any(word => text.Contains(word));
    }

    private bool ContainsFrenchWords(string text)
    {
        string[] words = { "que", "avec", "une", "être", "avoir", "par", "comme", "quand", "où", "très", "bien" };
        return words.Any(word => text.Contains(word));
    }

    private bool ContainsGermanWords(string text)
    {
        string[] words = { "mit", "eine", "sein", "haben", "wie", "wenn", "wo", "sehr", "gut", "der", "die", "das" };
        return words.Any(word => text.Contains(word));
    }
}