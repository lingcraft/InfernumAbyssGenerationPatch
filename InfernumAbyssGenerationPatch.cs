using Luminance.Core.Hooking;
using System;
using Terraria;
using Terraria.ModLoader;

namespace InfernumAbyssGenerationPatch;

public class InfernumAbyssGenerationPatch : Mod
{
    private class DisplayNameUpdater : ModSystem
    {
        public override void OnLocalizationsLoaded()
        {
            Instance.DisplayName = GetText("ModName");
        }
    }

    public static InfernumAbyssGenerationPatch Instance => ModContent.GetInstance<InfernumAbyssGenerationPatch>();

    public override void Load()
    {
        if (!ModLoader.TryGetMod("InfernumMode", out InfernumMod))
        {
            return;
        }

        if (!ModLoader.TryGetMod("CalamityMod", out CalamityMod))
        {
            return;
        }

        if (!ModLoader.TryGetMod("Luminance", out LuminanceMod))
        {
            return;
        }
    }

    public override void Unload()
    {
        InfernumMod = null;
        CalamityMod = null;
        LuminanceMod = null;
        ClearCache();
    }
}