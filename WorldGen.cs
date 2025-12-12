namespace InfernumAbyssGenerationPatch;

public class WorldGen : ModSystem
{
    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        int finalCleanupIndex = tasks.FindIndex(g => g.Name == "Final Cleanup");
        if (finalCleanupIndex != -1)
        {
            tasks.Insert(++finalCleanupIndex, new PassLegacy("Adjust Abyss Liquid", (progress, config) =>
            {
                progress.Message = GetText("Message");
                AbyssGen.ChangeLavaToWater();
            }));
        }
    }
}