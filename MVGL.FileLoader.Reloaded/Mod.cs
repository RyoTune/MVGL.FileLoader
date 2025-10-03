#if DEBUG
using System.Diagnostics;
#endif
using MVGL.FileLoader.Interfaces;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using MVGL.FileLoader.Reloaded.Template;
using MVGL.FileLoader.Reloaded.Configuration;
using Reloaded.Mod.Interfaces.Internal;

namespace MVGL.FileLoader.Reloaded;

public class Mod : ModBase, IExports
{
    private readonly IModLoader _modLoader;
    private readonly IReloadedHooks? _hooks;
    private readonly ILogger _log;
    private readonly IMod _owner;

    public static Config Config = null!;
    private readonly IModConfig _modConfig;
    private readonly MvglModRegistry _registry;
    private readonly MvglModLoader _loader;
    private readonly IMvglApi _api;

    public Mod(ModContext context)
    {
        _modLoader = context.ModLoader;
        _hooks = context.Hooks;
        _log = context.Logger;
        _owner = context.Owner;
        Config = context.Configuration;
        _modConfig = context.ModConfig;
#if DEBUG
        Debugger.Launch();
#endif
        Project.Initialize(_modConfig, _modLoader, _log, true);
        Log.LogLevel = Config.LogLevel;

        _registry = new();
        _loader = new(_registry);
        
        _api = new MvglApi(_modLoader, _registry);
        _modLoader.AddOrReplaceController(_owner, _api);
        
        _api.AddProbingPath("mvgl-loader");
    }

    public Type[] GetTypes() => [typeof(IMvglApi)];

    #region Standard Overrides

    public override void ConfigurationUpdated(Config configuration)
    {
        // Apply settings from configuration.
        // ... your code here.
        Config = configuration;
        _log.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
        Log.LogLevel = Config.LogLevel;
    }

    #endregion

    #region For Exports, Serialization etc.

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Mod()
    {
    }
#pragma warning restore CS8618

    #endregion
}