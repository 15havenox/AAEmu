using AAEmu.Commons.Utils;
using AAEmu.Game.Models;
using AAEmu.Game.Models.Game.Char;
using AAEmu.Game.Utils.DB;
using NLog;

namespace AAEmu.Game.Core.Managers;

public class LocalizationManager : Singleton<LocalizationManager>
{
    private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

    private readonly Dictionary<string, Dictionary<string, string>> _translations = [];
    private readonly HashSet<string> _availableLanguages = [];

    private static string GetLookupKey(string tblName, string tblColumn, long index)
    {
        return $"{tblName}:{tblColumn}:{index}";
    }

    public void Load()
    {
        Logger.Info("Loading translations ...");

        using (var connection = SQLite.CreateConnection())
        {
            // First, discover available language columns
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "PRAGMA table_info(localized_texts)";
                command.Prepare();
                using (var reader = new SQLiteWrapperReader(command.ExecuteReader()))
                {
                    while (reader.Read())
                    {
                        var columnName = reader.GetString("name");
                        // Check if it's a language column (skip system columns)
                        if (columnName != "tbl_name" && columnName != "tbl_column_name" && columnName != "idx")
                        {
                            _availableLanguages.Add(columnName);
                        }
                    }
                }
            }

            // Load translations for all available languages
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM localized_texts";
                command.Prepare();
                using (var reader = new SQLiteWrapperReader(command.ExecuteReader()))
                {
                    while (reader.Read())
                    {
                        var tblName = reader.GetString("tbl_name");
                        var tblColumn = reader.GetString("tbl_column_name");
                        var idx = reader.GetInt64("idx");

                        foreach (var language in _availableLanguages)
                        {
                            try
                            {
                                var translationValue = reader.GetString(language);
                                if (!string.IsNullOrEmpty(translationValue))
                                {
                                    AddTranslation(language, tblName, tblColumn, idx, translationValue);
                                }
                            }
                            catch
                            {
                                // Column might not exist, skip silently
                            }
                        }
                    }
                }
            }
        }

        var totalTranslations = _translations.Values.Sum(lang => lang.Count);
        Logger.Info($"Loaded {totalTranslations} translations across {_availableLanguages.Count} languages: {string.Join(", ", _availableLanguages)}");
    }

    public void AddTranslation(string language, string tblName, string tblColumn, long index, string translationValue)
    {
        if (!_translations.ContainsKey(language))
            _translations[language] = [];

        var key = GetLookupKey(tblName, tblColumn, index);
        if (!_translations[language].TryAdd(key, translationValue))
            Logger.Error($"Failed to add translation: {language}:{tblName}:{tblColumn}:{index}");
    }

    // Legacy method for compatibility
    public void AddTranslation(string tblName, string tblColumn, long index, string translationValue)
    {
        AddTranslation(AppConfiguration.Instance.DefaultLanguage, tblName, tblColumn, index, translationValue);
    }

    public string Get(string tblName, string tblColumn, long index, string fallbackValue = "")
    {
        return Get(AppConfiguration.Instance.DefaultLanguage, tblName, tblColumn, index, fallbackValue);
    }

    public string Get(string language, string tblName, string tblColumn, long index, string fallbackValue = "")
    {
        var key = GetLookupKey(tblName, tblColumn, index);
        
        // Try requested language first
        if (_translations.TryGetValue(language, out var langTranslations) && 
            langTranslations.TryGetValue(key, out var translatedText) && 
            !string.IsNullOrEmpty(translatedText))
        {
            return translatedText;
        }

        // Fall back to default language if different from requested
        if (language != AppConfiguration.Instance.DefaultLanguage && 
            _translations.TryGetValue(AppConfiguration.Instance.DefaultLanguage, out var defaultTranslations) &&
            defaultTranslations.TryGetValue(key, out var defaultText) && 
            !string.IsNullOrEmpty(defaultText))
        {
            return defaultText;
        }

        // Fall back to English if available and different from default
        if (language != "en_us" && AppConfiguration.Instance.DefaultLanguage != "en_us" &&
            _translations.TryGetValue("en_us", out var enTranslations) &&
            enTranslations.TryGetValue(key, out var enText) && 
            !string.IsNullOrEmpty(enText))
        {
            return enText;
        }

        return fallbackValue;
    }

    public HashSet<string> GetAvailableLanguages()
    {
        return new HashSet<string>(_availableLanguages);
    }

    public bool IsLanguageAvailable(string language)
    {
        return _availableLanguages.Contains(language);
    }

    // Character-specific methods
    public string Get(Character character, string tblName, string tblColumn, long index, string fallbackValue = "")
    {
        return Get(character?.GetEffectiveLanguage() ?? AppConfiguration.Instance.DefaultLanguage, 
                  tblName, tblColumn, index, fallbackValue);
    }
}
