using System.Collections.Generic;
using Terraria.GameContent.Generation;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace InfernumAbyssGenPatch;

public class WorldGenSystem : ModSystem
{
    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        int finalCleanupIndex = tasks.FindIndex(g => g.Name == "Final Cleanup");
        if (finalCleanupIndex != -1)
        {
            tasks.Insert(++finalCleanupIndex, new PassLegacy("Adjust Abyss Liquid",
                (progress, config) =>
                {
                    progress.Message = Language.GetTextValue($"Mods.InfernumAbyssGenPatch.Message");
                    AbyssGen.ChangeLavaToWater();
                }));
        }
    }
}