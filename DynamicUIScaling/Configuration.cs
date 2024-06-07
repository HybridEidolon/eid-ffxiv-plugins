using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace DynamicUIScaling;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public float ScaleFactor { get; set; } = 0.75f;

    public bool ScaleMinitalkBubbles { get; set; } = true;

    [NonSerialized]
    private DalamudPluginInterface? pluginInterface;

    public void Initialize(DalamudPluginInterface pluginInterface)
    {
        this.pluginInterface = pluginInterface;
    }

    public void Save()
    {
        pluginInterface!.SavePluginConfig(this);
    }
}
