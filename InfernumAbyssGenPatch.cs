using FullSerializer.Internal;
using System;
using System.Reflection;
using Terraria.ModLoader;

namespace InfernumAbyssGenPatch;

public class InfernumAbyssGenPatch : Mod
{
    public static InfernumAbyssGenPatch Instance => ModContent.GetInstance<InfernumAbyssGenPatch>();

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

        var generate = GetMethod("InfernumMode.Content.WorldGeneration.CustomAbyss", "Generate");
        if (generate is null)
        {
            return;
        }

        MonoModHooks.Add(generate, (Action orig) => AbyssGen.Generate());
    }

    public override void Unload()
    {
        InfernumMod = null;
        CalamityMod = null;
        LuminanceMod = null;
        ClearCache();
    }
}