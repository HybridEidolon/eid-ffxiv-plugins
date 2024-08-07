using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;

namespace DynamicUIScaling;

public sealed class Plugin : IDalamudPlugin
{
    private IDalamudPluginInterface PluginInterface { get; init; }
    public IGameInteropProvider GameInteropProvider { get; init; }
    public IPluginLog PluginLog { get; init; }
    public IAddonLifecycle AddonLifecycle { get; init; }
    public IFramework Framework { get; init; }
    public IGameConfig GameConfig { get; init; }

    public Configuration Configuration { get; init; }
    public ConfigWindow ConfigWindow { get; init; }

    public readonly WindowSystem WindowSystem = new("DynamicUIScaling");

    internal Funcs Funcs { get; init; }

    public Plugin(
        IDalamudPluginInterface pluginInterface,
        IGameInteropProvider gameInteropProvider,
        IPluginLog pluginLog,
        IAddonLifecycle addonLifecycle,
        IFramework framework,
        IGameConfig gameConfig)
    {
        PluginInterface = pluginInterface;
        GameInteropProvider = gameInteropProvider;
        PluginLog = pluginLog;
        AddonLifecycle = addonLifecycle;
        Framework = framework;
        GameConfig = gameConfig;

        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(PluginInterface);

        ConfigWindow = new(this);

        WindowSystem.AddWindow(ConfigWindow);

        PluginInterface.UiBuilder.Draw += DrawUI;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        Funcs = new Funcs(this);
    }

    public void Dispose()
    {
        WindowSystem?.RemoveAllWindows();
        Funcs?.Dispose();
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
}
