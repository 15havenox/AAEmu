using System.Collections.Concurrent;
using System.Reflection;
using AAEmu.Commons.Utils;
using AAEmu.Game.Models.Game.Char;
using NLog;

namespace AAEmu.Game.Core.Managers;

/// <summary>
/// Sistema modular para carregar e gerenciar módulos/plugins do jogo
/// </summary>
public class ModuleManager : Singleton<ModuleManager>
{
    private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    
    private readonly ConcurrentDictionary<string, IGameModule> _modules = new();
    private readonly ConcurrentDictionary<string, Assembly> _loadedAssemblies = new();
    private bool _isInitialized = false;

    public IReadOnlyDictionary<string, IGameModule> LoadedModules => _modules;
    
    /// <summary>
    /// Inicializa o sistema de módulos
    /// </summary>
    public void Initialize()
    {
        if (_isInitialized)
            return;
            
        Logger.Info("Initializing Module Manager...");
        
        LoadCoreModules();
        LoadExternalModules();
        
        _isInitialized = true;
        Logger.Info($"Module Manager initialized with {_modules.Count} modules");
    }

    /// <summary>
    /// Carrega módulos internos do core
    /// </summary>
    private void LoadCoreModules()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            LoadModulesFromAssembly(assembly);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to load core modules");
        }
    }

    /// <summary>
    /// Carrega módulos externos de DLLs
    /// </summary>
    private void LoadExternalModules()
    {
        var modulesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules");
        
        if (!Directory.Exists(modulesPath))
        {
            Directory.CreateDirectory(modulesPath);
            Logger.Info($"Created modules directory: {modulesPath}");
            return;
        }

        var dllFiles = Directory.GetFiles(modulesPath, "*.dll", SearchOption.TopDirectoryOnly);
        
        foreach (var dllFile in dllFiles)
        {
            try
            {
                LoadModuleFromFile(dllFile);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Failed to load module from {dllFile}");
            }
        }
    }

    /// <summary>
    /// Carrega um módulo de um arquivo DLL específico
    /// </summary>
    private void LoadModuleFromFile(string filePath)
    {
        var assembly = Assembly.LoadFrom(filePath);
        _loadedAssemblies[filePath] = assembly;
        LoadModulesFromAssembly(assembly);
    }

    /// <summary>
    /// Carrega todos os módulos de um assembly
    /// </summary>
    private void LoadModulesFromAssembly(Assembly assembly)
    {
        var moduleTypes = assembly.GetTypes()
            .Where(t => !t.IsInterface && !t.IsAbstract && typeof(IGameModule).IsAssignableFrom(t));

        foreach (var moduleType in moduleTypes)
        {
            try
            {
                var module = (IGameModule)Activator.CreateInstance(moduleType)!;
                RegisterModule(module);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Failed to instantiate module {moduleType.Name}");
            }
        }
    }

    /// <summary>
    /// Registra um módulo no sistema
    /// </summary>
    public bool RegisterModule(IGameModule module)
    {
        if (module == null)
            return false;
            
        if (_modules.ContainsKey(module.Name))
        {
            Logger.Warn($"Module {module.Name} is already registered");
            return false;
        }

        try
        {
            if (module.IsEnabled)
            {
                module.Initialize();
            }
            
            _modules[module.Name] = module;
            Logger.Info($"Registered module: {module.Name} v{module.Version} (Enabled: {module.IsEnabled})");
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Failed to register module {module.Name}");
            return false;
        }
    }

    /// <summary>
    /// Remove um módulo do sistema
    /// </summary>
    public bool UnregisterModule(string moduleName)
    {
        if (!_modules.TryRemove(moduleName, out var module))
            return false;

        try
        {
            if (module.IsEnabled)
            {
                module.Shutdown();
            }
            
            Logger.Info($"Unregistered module: {moduleName}");
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Error while unregistering module {moduleName}");
            return false;
        }
    }

    /// <summary>
    /// Ativa ou desativa um módulo
    /// </summary>
    public bool SetModuleEnabled(string moduleName, bool enabled)
    {
        if (!_modules.TryGetValue(moduleName, out var module))
            return false;

        try
        {
            if (enabled && !module.IsEnabled)
            {
                module.Initialize();
                module.IsEnabled = true;
                Logger.Info($"Enabled module: {moduleName}");
            }
            else if (!enabled && module.IsEnabled)
            {
                module.Shutdown();
                module.IsEnabled = false;
                Logger.Info($"Disabled module: {moduleName}");
            }
            
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Failed to set module {moduleName} enabled state to {enabled}");
            return false;
        }
    }

    /// <summary>
    /// Obtém um módulo específico por nome
    /// </summary>
    public T? GetModule<T>(string moduleName) where T : class, IGameModule
    {
        return _modules.TryGetValue(moduleName, out var module) ? module as T : null;
    }

    /// <summary>
    /// Obtém todos os módulos de um tipo específico
    /// </summary>
    public IEnumerable<T> GetModules<T>() where T : class, IGameModule
    {
        return _modules.Values.OfType<T>();
    }

    /// <summary>
    /// Notifica todos os módulos quando um jogador entra
    /// </summary>
    public void OnPlayerJoin(Character player)
    {
        foreach (var module in _modules.Values.Where(m => m.IsEnabled))
        {
            try
            {
                module.OnPlayerJoin(player);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Module {module.Name} failed to handle player join event");
            }
        }
    }

    /// <summary>
    /// Notifica todos os módulos quando um jogador sai
    /// </summary>
    public void OnPlayerLeave(Character player)
    {
        foreach (var module in _modules.Values.Where(m => m.IsEnabled))
        {
            try
            {
                module.OnPlayerLeave(player);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Module {module.Name} failed to handle player leave event");
            }
        }
    }

    /// <summary>
    /// Recarrega todos os módulos externos
    /// </summary>
    public void ReloadExternalModules()
    {
        Logger.Info("Reloading external modules...");
        
        // Descarregar módulos externos existentes
        var externalModules = _modules.Values
            .Where(m => !IsFromCoreAssembly(m))
            .ToList();

        foreach (var module in externalModules)
        {
            UnregisterModule(module.Name);
        }

        // Recarregar módulos externos
        LoadExternalModules();
        
        Logger.Info("External modules reloaded");
    }

    /// <summary>
    /// Verifica se um módulo é do assembly principal
    /// </summary>
    private bool IsFromCoreAssembly(IGameModule module)
    {
        return module.GetType().Assembly == Assembly.GetExecutingAssembly();
    }

    /// <summary>
    /// Desliga todos os módulos
    /// </summary>
    public void Shutdown()
    {
        Logger.Info("Shutting down Module Manager...");
        
        foreach (var module in _modules.Values.Where(m => m.IsEnabled))
        {
            try
            {
                module.Shutdown();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error shutting down module {module.Name}");
            }
        }
        
        _modules.Clear();
        _loadedAssemblies.Clear();
        _isInitialized = false;
        
        Logger.Info("Module Manager shut down");
    }
}

/// <summary>
/// Interface base para todos os módulos do jogo
/// </summary>
public interface IGameModule
{
    /// <summary>
    /// Nome único do módulo
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Versão do módulo
    /// </summary>
    string Version { get; }
    
    /// <summary>
    /// Se o módulo está ativo
    /// </summary>
    bool IsEnabled { get; set; }
    
    /// <summary>
    /// Inicializa o módulo
    /// </summary>
    void Initialize();
    
    /// <summary>
    /// Finaliza o módulo
    /// </summary>
    void Shutdown();
    
    /// <summary>
    /// Chamado quando um jogador entra no servidor
    /// </summary>
    void OnPlayerJoin(Character player);
    
    /// <summary>
    /// Chamado quando um jogador sai do servidor
    /// </summary>
    void OnPlayerLeave(Character player);
}

/// <summary>
/// Interface para módulos que gerenciam eventos
/// </summary>
public interface IEventModule : IGameModule
{
    /// <summary>
    /// Registra eventos do módulo
    /// </summary>
    void RegisterEvents();
    
    /// <summary>
    /// Remove registros de eventos
    /// </summary>
    void UnregisterEvents();
}

/// <summary>
/// Classe base para facilitar implementação de módulos
/// </summary>
public abstract class GameModuleBase : IGameModule
{
    protected static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    
    public abstract string Name { get; }
    public abstract string Version { get; }
    public bool IsEnabled { get; set; } = true;

    public virtual void Initialize()
    {
        Logger.Info($"Initializing module {Name} v{Version}");
    }

    public virtual void Shutdown()
    {
        Logger.Info($"Shutting down module {Name}");
    }

    public virtual void OnPlayerJoin(Character player)
    {
        // Override in derived classes if needed
    }

    public virtual void OnPlayerLeave(Character player)
    {
        // Override in derived classes if needed
    }
}