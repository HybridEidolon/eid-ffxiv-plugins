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
    private IDalamudPluginInterface? pluginInterface;

    public void Initialize(IDalamudPluginInterface pluginInterface)
    {
        this.pluginInterface = pluginInterface;
    }

    public void Save()
    {
        pluginInterface!.SavePluginConfig(this);
    }
}
