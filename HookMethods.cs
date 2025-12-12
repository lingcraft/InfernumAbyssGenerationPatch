namespace InfernumAbyssGenerationPatch;

public class HookMethods : ICustomDetourProvider
{
    void ICustomDetourProvider.ModifyMethods()
    {
        // On 钩子
        HookHelper.ModifyMethodWithDetour(typeof(Terraria.WorldGen).GetMethod("oceanDepths"), OceanDepths);
        HookHelper.ModifyMethodWithDetour(GetMethod("InfernumMode.Content.WorldGeneration.CustomAbyss", "Generate"), Generate);
        HookHelper.ModifyMethodWithDetour(GetProperty("InfernumMode.Content.WorldGeneration.CustomAbyss", "AbyssTop")!.GetGetMethod(), AbyssTop);
        HookHelper.ModifyMethodWithDetour(GetProperty("InfernumMode.Content.WorldGeneration.CustomAbyss", "Layer2Top")!.GetGetMethod(), Layer2Top);
        HookHelper.ModifyMethodWithDetour(GetProperty("InfernumMode.Content.WorldGeneration.CustomAbyss", "Layer3Top")!.GetGetMethod(), Layer3Top);
        HookHelper.ModifyMethodWithDetour(GetProperty("InfernumMode.Content.WorldGeneration.CustomAbyss", "Layer4Top")!.GetGetMethod(), Layer4Top);

        // IL 钩子
        HookHelper.ModifyMethodWithIL(GetMethod("InfernumMode.Core.ILEditingStuff.AdjustAbyssDefinitionHook", "ChangeAbyssRequirement"), ChangeAbyssRequirement);
        HookHelper.ModifyMethodWithIL(GetMethod("InfernumMode.Core.ILEditingStuff.AdjustAbyssDefinitionHook", "ChangeLayer1Requirement"), ChangeLayer1Requirement);
        HookHelper.ModifyMethodWithIL(GetMethod("InfernumMode.Core.ILEditingStuff.AdjustAbyssDefinitionHook", "ChangeLayer2Requirement"), ChangeLayer2Requirement);
        HookHelper.ModifyMethodWithIL(GetMethod("InfernumMode.Core.ILEditingStuff.AdjustAbyssDefinitionHook", "ChangeLayer3Requirement"), ChangeLayer3Requirement);
        HookHelper.ModifyMethodWithIL(GetMethod("InfernumMode.Core.ILEditingStuff.AdjustAbyssDefinitionHook", "ChangeLayer4Requirement"), ChangeLayer4Requirement);
    }

    private bool OceanDepths(Func<int, int, bool> ori, int x, int y)
    {
        if (AbyssGen.IsInsideOfAbyss(new Point(x, y)))
        {
            return false;
        }

        return ori(x, y);
    }

    private void Generate(Action ori)
    {
        AbyssGen.Generate();
    }

    private int AbyssTop(Func<int> ori)
    {
        return AbyssGen.AbyssTop;
    }

    private int Layer2Top(Func<int> ori)
    {
        return AbyssGen.Layer2Top;
    }

    private int Layer3Top(Func<int> ori)
    {
        return AbyssGen.Layer3Top;
    }

    private int Layer4Top(Func<int> ori)
    {
        return AbyssGen.Layer4Top;
    }

    private void ChangeAbyssRequirement(ILContext il)
    {
        var cursor = new ILCursor(il);
        cursor.EmitLdarg1();
        cursor.EmitLdarg2();
        cursor.EmitLdarg3();
        cursor.EmitDelegate((Func<Player, int, bool> ori, Player player, out int playerYTileCoords) =>
        {
            Point point = player.Center.ToTileCoordinates();
            playerYTileCoords = point.Y;
            if (AbyssGen.InAnySubworld())
                return false;
            return !player.lavaWet && !player.honeyWet && AbyssGen.IsInsideOfAbyss(point);
        });
        cursor.EmitRet();
    }

    private void ChangeLayer1Requirement(ILContext il)
    {
        var cursor = new ILCursor(il);
        cursor.EmitLdarg1();
        cursor.EmitLdarg2();
        cursor.EmitLdarg3();
        cursor.EmitDelegate((Func<object, Player, bool> ori, object self, Player player) =>
        {
            if (AbyssGen.InPostAEWUpdateWorld)
            {
                if (Main.remixWorld)
                {
                    return AbyssGen.MeetsBaseAbyssRequirement(player, out int playerYTileCoords) &&
                           playerYTileCoords >= AbyssGen.Layer2Top;
                }
                else
                {
                    return AbyssGen.MeetsBaseAbyssRequirement(player, out int playerYTileCoords) &&
                           playerYTileCoords <= AbyssGen.Layer2Top;
                }
            }

            return ori(self, player);
        });
        cursor.EmitRet();
    }

    private void ChangeLayer2Requirement(ILContext il)
    {
        var cursor = new ILCursor(il);
        cursor.EmitLdarg1();
        cursor.EmitLdarg2();
        cursor.EmitLdarg3();
        cursor.EmitDelegate((Func<object, Player, bool> ori, object self, Player player) =>
        {
            if (AbyssGen.InPostAEWUpdateWorld)
            {
                if (Main.remixWorld)
                {
                    return AbyssGen.MeetsBaseAbyssRequirement(player, out int playerYTileCoords) &&
                           AbyssGen.Layer3Top <= playerYTileCoords && playerYTileCoords < AbyssGen.Layer2Top;
                }
                else
                {
                    return AbyssGen.MeetsBaseAbyssRequirement(player, out int playerYTileCoords) &&
                           AbyssGen.Layer2Top < playerYTileCoords && playerYTileCoords <= AbyssGen.Layer3Top;
                }
            }

            return ori(self, player);
        });
        cursor.EmitRet();
    }

    private void ChangeLayer3Requirement(ILContext il)
    {
        var cursor = new ILCursor(il);
        cursor.EmitLdarg1();
        cursor.EmitLdarg2();
        cursor.EmitLdarg3();
        cursor.EmitDelegate((Func<object, Player, bool> ori, object self, Player player) =>
        {
            if (AbyssGen.InPostAEWUpdateWorld)
            {
                if (Main.remixWorld)
                {
                    return AbyssGen.MeetsBaseAbyssRequirement(player, out int playerYTileCoords) &&
                           AbyssGen.Layer4Top <= playerYTileCoords && playerYTileCoords < AbyssGen.Layer3Top;
                }
                else
                {
                    return AbyssGen.MeetsBaseAbyssRequirement(player, out int playerYTileCoords) &&
                           AbyssGen.Layer3Top < playerYTileCoords && playerYTileCoords <= AbyssGen.Layer4Top;
                }
            }

            return ori(self, player);
        });
        cursor.EmitRet();
    }

    private void ChangeLayer4Requirement(ILContext il)
    {
        var cursor = new ILCursor(il);
        cursor.EmitLdarg1();
        cursor.EmitLdarg2();
        cursor.EmitLdarg3();
        cursor.EmitDelegate((Func<object, Player, bool> ori, object self, Player player) =>
        {
            if (AbyssGen.InPostAEWUpdateWorld)
            {
                if (Main.remixWorld)
                {
                    return AbyssGen.MeetsBaseAbyssRequirement(player, out int playerYTileCoords) &&
                           AbyssGen.AbyssBottom <= playerYTileCoords && playerYTileCoords < AbyssGen.Layer4Top;
                }
                else
                {
                    return AbyssGen.MeetsBaseAbyssRequirement(player, out int playerYTileCoords) &&
                           AbyssGen.Layer4Top < playerYTileCoords && playerYTileCoords <= AbyssGen.AbyssBottom;
                }
            }

            return ori(self, player);
        });
        cursor.EmitRet();
    }
}