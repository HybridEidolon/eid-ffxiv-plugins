using System;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;

namespace DynamicUIScaling;

public class ConfigWindow : Window, IDisposable
{
    private Configuration configuration;

    private Plugin Plugin { get; init; }

    // We give this window a constant ID using ###
    // This allows for labels being dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow(Plugin plugin) : base("DynamicUIScaling Config###DynamicUIScaling.Config")
    {
        Plugin = plugin;
        Flags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;

        var windowSizeConstraints = new WindowSizeConstraints();
        windowSizeConstraints.MinimumSize = new(400, 300);
        SizeConstraints = windowSizeConstraints;

        configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
    }

    public override void Draw()
    {
        ImGui.TextWrapped("DynamicUIScaling overrides the game's High Resolution Scaling behavior to make the UI scale consistently across resolutions.");

        // can't ref a property, so use a local copy
        var scaleFactorValue = configuration.ScaleFactor;
        if (ImGui.SliderFloat("Scale Factor", ref scaleFactorValue, 0.5f, 1.2f))
        {
            configuration.ScaleFactor = scaleFactorValue;
            // can save immediately on change, if you don't want to provide a "Save and Close" button
            configuration.Save();
            Plugin.Funcs.Rescale();
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.Text("Scale factor relative to the 1280x720 base resolution.");
            ImGui.EndTooltip();
        }
        var scaleMinitalkBubbles = configuration.ScaleMinitalkBubbles;
        if (ImGui.Checkbox("Scale Minitalk Bubbles", ref scaleMinitalkBubbles))
        {
            configuration.ScaleMinitalkBubbles = scaleMinitalkBubbles;
            configuration.Save();
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.Text("Minitalk bubbles will be scaled relative to the resolution, where they normally are not affected at all.");
            ImGui.EndTooltip();
        }
    }
}
