namespace InfernumAbyssGenerationPatch;

public class WorldGen : ModSystem
{
    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        int brimstoneCragIndex = tasks.FindIndex(g => g.Name.Equals("Brimstone Crag"));
        if (brimstoneCragIndex != -1)
        {
            tasks.Insert(++brimstoneCragIndex, new PassLegacy("Supply Brimstone Slag", (progress, config) =>
            {
                progress.Message = GetText("WorldGen.SupplyBrimstoneSlag");
                AbyssGen.SupplyBrimstoneSlag();
            }));
        }
        int finalCleanupIndex = tasks.FindIndex(g => g.Name.Equals("Final Cleanup"));
        if (finalCleanupIndex != -1)
        {
            tasks.Insert(++finalCleanupIndex, new PassLegacy("Correct Abyss Liquid", (progress, config) =>
            {
                progress.Message = GetText("WorldGen.CorrectAbyssLiquid");
                AbyssGen.ChangeLavaToWater();
            }));
        }
    }
}