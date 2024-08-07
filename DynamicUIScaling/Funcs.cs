
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;

namespace DynamicUIScaling;

internal sealed unsafe class Funcs : IDisposable {
    private Plugin Plugin { get; }

    private delegate sbyte CheckHiResScaleDelegate(RaptureAtkUnitManager* unk, sbyte changingResolution);
    private Hook<CheckHiResScaleDelegate> checkHiResScaleHook;

    [Signature("48 8B C4 53 55 41 54 41 57 48 83 EC 68 48 89 70 08 45 33 FF 4C 89 70 D8 48")]
    private readonly delegate* unmanaged<RaptureAtkUnitManager*, float, sbyte, sbyte> setGlobalUiScaleFactorNative = null!;

    internal Funcs(Plugin plugin)
    {
        Plugin = plugin;

        Plugin.GameInteropProvider.InitializeFromAttributes(this);
        checkHiResScaleHook = Plugin.GameInteropProvider.HookFromSignature<CheckHiResScaleDelegate>("48 89 5C 24 08 48 89 6C 24 10 48 89 74 24 18 57 48 83 EC 20 0F B6 EA 48 8B F1 ?? ?? ?? ?? ?? 32", OnCheckHiResScale);
        checkHiResScaleHook.Enable();

        OnCheckHiResScale(RaptureAtkUnitManager.Instance(), 0);

        // Plugin.AddonLifecycle.RegisterListener(AddonEvent.PreDraw, "ChatLog", ChatLogPanelPreDraw);
        // Plugin.AddonLifecycle.RegisterListener(AddonEvent.PostDraw, "ChatLog", ChatLogPanelPostDraw);

        Plugin.Framework.Update += MiniTalkRescale;
    }

    private float CalculateScale()
    {
        Device* device = Device.Instance();
        float width = device->Width;
        float height = device->Height;

        return height / (720.0f * (16.0f/9.0f / (width/height))) * Math.Max(Plugin.Configuration.ScaleFactor, 0.1f);
    }

    public void Rescale()
    {
        OnCheckHiResScale(RaptureAtkUnitManager.Instance(), 0);
    }

    private sbyte OnCheckHiResScale(RaptureAtkUnitManager* self, sbyte resolutionChanged)
    {
        Device* device = Device.Instance();
        if (device == null)
        {
            return 0;
        }

        float scale = CalculateScale();

        setGlobalUiScaleFactorNative(self, scale, resolutionChanged == 0 ? (sbyte)1 : (sbyte)0);

        return 1;
    }

    private void MiniTalkRescale(object _)
    {
        if (!Plugin.Configuration.ScaleMinitalkBubbles)
        {
            return;
        }

        float scale = CalculateScale();
        var miniTalk = RaptureAtkUnitManager.Instance()->GetAddonByName("_MiniTalk");

        if (miniTalk == null)
        {
            return;
        }

        List<uint> nodes = [2, 20001, 20002, 20003, 20004, 20005, 20006, 20007, 20008, 20009];
        foreach (var nodeId in nodes)
        {
            var node = miniTalk->GetNodeById(nodeId);
            if (node != null)
            {
                node->ScaleX = scale;
                node->ScaleY = scale;
            }
        }
    }

    private void ChatLogPanelPreDraw(AddonEvent type, AddonArgs args)
    {
        float scale = CalculateScale();

        var unitManager = RaptureAtkUnitManager.Instance();
        AtkUnitBase* log = unitManager->GetAddonByName("ChatLog");
        var logResizeAnchor = log->GetNodeById(2);
        if (logResizeAnchor != null)
        {
            logResizeAnchor->SetScale(1.0f, 1.0f);
            logResizeAnchor->SetWidth(28);
            logResizeAnchor->SetHeight(28);
        }
        var textInput = (AtkComponentTextInput*)log->GetComponentNodeById(5);
    }

    private void ChatLogPanelPostDraw(AddonEvent type, AddonArgs args)
    {
        float scale = CalculateScale();

        var unitManager = RaptureAtkUnitManager.Instance();
        AtkUnitBase* log = unitManager->GetAddonByName("ChatLog");
        var logResizeAnchor = log->GetNodeById(2);
        if (logResizeAnchor != null)
        {
            // logResizeAnchor->SetScale(1.0f, 1.0f);
        }
    }

    public void Dispose()
    {
    }
}
