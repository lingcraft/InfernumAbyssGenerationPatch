using Luminance.Common.Utilities;

namespace InfernumAbyssGenerationPatch;

public static class AbyssGen
{
    #region Fields and Properties

    // Loop variables that are accessed via getter methods should be stored externally in local variables for performance reasons.
    public static int MinAbyssWidth => MaxAbyssWidth / 5;

    public static int MaxAbyssWidth => BiomeWidth + 130;

    // 0-1 value that determines the threshold for layer 1 spaghetti caves being carved out. At 0, no tiles are carved out, at 1, all tiles are carved out.
    // This is used in the formula 'abs(noise(x, y)) < r' to determine whether the cave should remove tiles.
    public static readonly float[] Layer1SpaghettiCaveCarveOutThresholds =
    [
        0.0396f,
        0.0506f,
        0.0521f
    ];

    public const int Layer1SmallPlantCreationChance = 6;

    public const float Layer1ForestNoiseMagnificationFactor = 0.00181f;

    public const float Layer1ForestMinNoiseValue = 0.235f;

    public const int Layer1KelpCreationChance = 18;

    public static int Layer2TrenchCount => (int)Math.Sqrt(Main.maxTilesX / 176f);

    public const int MinStartingTrenchWidth = 5;

    public const int MaxStartingTrenchWidth = 8;

    public const int MinEndingTrenchWidth = 20;

    public const int MaxEndingTrenchWidth = 27;

    public const float TrenchTightnessFactor = 1.72f;

    public const float TrenchWidthNoiseMagnificationFactor = 0.00292f;

    public const float TrenchOffsetNoiseMagnificationFactor = 0.00261f;

    public const int MaxTrenchOffset = 28;

    public const int Layer2WildlifeSpawnAttempts = 35200;

    public const int Layer3CaveCarveoutSteps = 124;

    public const int MinLayer3CaveSize = 9;

    public const int MaxLayer3CaveSize = 16;

    public static readonly float[] Layer3SpaghettiCaveCarveOutThresholds =
    [
        0.1389f
    ];

    public const float Layer3CrystalCaveMagnificationFactor = 0.00109f;

    public const float CrystalCaveNoiseThreshold = 0.58f;

    public const int Layer3VentCount = 10;

    public const int Layer3SquidDenOuterRadius = 60;

    public const int Layer3SquidDenInnerRadius = Layer3SquidDenOuterRadius - 16;

    public const int EidolistPedestalRadius = 20;

    // How thick walls should be between the insides of the abyss and the outside. This should be relatively high, since you don't want the boring
    // vanilla caverns to be visible from within the abyss, for immersion reasons.
    public const int WallThickness = 70;

    #endregion Fields and Properties

    #region 修改字段

    public static int AbyssTop => Main.remixWorld ? YStart - 10 : YStart + BlockDepth - 44;

    public static int Layer2Top => (int)(Main.remixWorld ? YStart * 0.878f : Main.rockLayer + Main.maxTilesY * 0.084f);

    public static int Layer3Top => (int)(Main.remixWorld ? YStart * 0.676f : Main.rockLayer + Main.maxTilesY * 0.184f);

    public static int Layer4Top => (int)(Main.remixWorld ? YStart * 0.419f : Main.rockLayer + Main.maxTilesY * 0.33f);

    #endregion 修改字段

    #region 反射字段

    public static int BiomeWidth => GetValue<PropertyInfo, int>("CalamityMod.World.SulphurousSea", "BiomeWidth");

    public static int YStart => GetValue<PropertyInfo, int>("CalamityMod.World.SulphurousSea", "YStart");

    public static int BlockDepth => GetValue<PropertyInfo, int>("CalamityMod.World.SulphurousSea", "BlockDepth");

    public static float SandstoneEdgeNoiseMagnification =>
        GetValue<float>("CalamityMod.World.SulphurousSea", "SandstoneEdgeNoiseMagnification");

    public static int AbyssBottom
    {
        get => GetValue<int>("CalamityMod.World.Abyss", "AbyssChasmBottom");
        set => SetValue("CalamityMod.World.Abyss", "AbyssChasmBottom", value);
    }

    public static float SpaghettiCaveMagnification =>
        GetValue<float>("CalamityMod.World.SulphurousSea", "SpaghettiCaveMagnification");

    public static bool InPostAEWUpdateWorld
    {
        get => GetValue<PropertyInfo, bool>("InfernumMode.Core.GlobalInstances.Systems.WorldSaveSystem",
            "InPostAEWUpdateWorld");
        set => SetValue<PropertyInfo>("InfernumMode.Core.GlobalInstances.Systems.WorldSaveSystem",
            "InPostAEWUpdateWorld", value);
    }

    public static int AbyssLayer1ForestSeed
    {
        get => GetValue<PropertyInfo, int>("InfernumMode.Core.GlobalInstances.Systems.WorldSaveSystem",
            "AbyssLayer1ForestSeed");
        set => SetValue<PropertyInfo>("InfernumMode.Core.GlobalInstances.Systems.WorldSaveSystem",
            "AbyssLayer1ForestSeed", value);
    }

    public static int AbyssLayer3CavernSeed
    {
        get => GetValue<PropertyInfo, int>("InfernumMode.Core.GlobalInstances.Systems.WorldSaveSystem",
            "AbyssLayer3CavernSeed");
        set => SetValue<PropertyInfo>("InfernumMode.Core.GlobalInstances.Systems.WorldSaveSystem",
            "AbyssLayer3CavernSeed", value);
    }

    public static Point SquidDenCenter
    {
        get => GetValue<PropertyInfo, Point>("InfernumMode.Core.GlobalInstances.Systems.WorldSaveSystem",
            "SquidDenCenter");
        set => SetValue<PropertyInfo>("InfernumMode.Core.GlobalInstances.Systems.WorldSaveSystem", "SquidDenCenter",
            value);
    }

    public static bool AtLeftSideOfWorld
    {
        get => GetValue<bool>("CalamityMod.World.Abyss", "AtLeftSideOfWorld");
        set => SetValue("CalamityMod.World.Abyss", "AtLeftSideOfWorld", value);
    }

    #endregion 反射字段

    #region 反射方法

    public static int GetActualX(int x) => InvokeMethod<int>("CalamityMod.World.SulphurousSea", "GetActualX", x);

    public static float FractalBrownianMotion(float x, float y, int seed, int octaves, float gain = 0.5f,
        float lacunarity = 2f) =>
        InvokeMethod<float>("CalamityMod.World.SulphurousSea", "FractalBrownianMotion",
            x, y, seed, octaves, gain, lacunarity);

    public static bool AttemptToGrowVine(Point p) =>
        InvokeMethod<bool>("InfernumMode.Content.Tiles.Abyss.SulphurousGroundVines", "AttemptToGrowVine", p);

    public static Point GetGroundPositionFrom(Point p, GenSearch search = null) =>
        InvokeMethod<Point>("InfernumMode.Utilities", "GetGroundPositionFrom",
            p, search);

    public static bool InAnySubworld() => InvokeMethod<bool>("CalamityMod.WeakReferenceSupport", "InAnySubworld");

    public static bool MeetsBaseAbyssRequirement(Player player, out int playerYTileCoords)
    {
        var method = GetMethod("CalamityMod.BiomeManagers.AbyssLayer1Biome", "MeetsBaseAbyssRequirement");
        var args = new object[] { player, null };
        bool result = (bool)method?.Invoke(null, args)!;
        playerYTileCoords = (int)args[1];
        return result;
    }

    #endregion 反射方法

    #region Placement Methods

    public static void Generate()
    {
        // Define the bottom of the abyss first and foremost.
        AbyssBottom = Main.remixWorld ? 0 : Main.UnderworldLayer - 42;

        // Mark this world as being made in the AEW update.
        InPostAEWUpdateWorld = true;

        GenerateGravelBlock(); // 砾石
        GenerateSulphurousSeaCut(); // 硫磺海通道
        GenerateLayer1(); // 深渊第1层
        GenerateLayer2(out List<Point> trenchBottoms); // 深渊第2层
        GenerateLayer3(trenchBottoms, out Point layer4ConvergencePoint); // 深渊第3层
        GenerateLayer4(layer4ConvergencePoint); // 深渊第4层
        GenerateVoidstone(); // 虚空石
    }

    #endregion Placement Methods

    #region Generation Functions

    public static void GenerateGravelBlock()
    {
        int minWidth = MinAbyssWidth;
        int maxWidth = MaxAbyssWidth;
        int top = AbyssTop;
        int bottom = AbyssBottom;
        ushort gravelID = GetTileId("CalamityMod.Tiles.Abyss.AbyssGravel");
        ushort gravelWallID = GetWallId("CalamityMod.Walls.CalamityMod.Walls.AbyssGravelWall");

        for (int i = 1; i < maxWidth; i++)
        {
            int x = GetActualX(i);
            if (Main.remixWorld)
            {
                for (int y = top; y > bottom; y--)
                {
                    // Decide whether to cut off due to a Y point being far enough.
                    float yCompletion = Utils.GetLerpValue(top, bottom - 1f, y, true);
                    if (i >= GetWidth(yCompletion, minWidth, maxWidth))
                        continue;

                    // Otherwise, lay down gravel.
                    Main.tile[x, y].TileType = gravelID;
                    Main.tile[x, y].WallType = gravelWallID;
                    Main.tile[x, y].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                    Main.tile[x, y].Get<TileWallWireStateData>().IsHalfBlock = false;
                    Main.tile[x, y].Get<TileWallWireStateData>().HasTile = true;
                    Main.tile[x, y].Get<LiquidData>().LiquidType = LiquidID.Water;
                    Main.tile[x, y].LiquidAmount = 255;
                }
            }
            else
            {
                for (int y = top; y < bottom; y++)
                {
                    // Decide whether to cut off due to a Y point being far enough.
                    float yCompletion = Utils.GetLerpValue(top, bottom - 1f, y, true);
                    if (i >= GetWidth(yCompletion, minWidth, maxWidth))
                        continue;

                    // Otherwise, lay down gravel.
                    Main.tile[x, y].TileType = gravelID;
                    Main.tile[x, y].WallType = gravelWallID;
                    Main.tile[x, y].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                    Main.tile[x, y].Get<TileWallWireStateData>().IsHalfBlock = false;
                    Main.tile[x, y].Get<TileWallWireStateData>().HasTile = true;
                    Main.tile[x, y].Get<LiquidData>().LiquidType = LiquidID.Water;
                    Main.tile[x, y].LiquidAmount = 255;
                }
            }
        }
    }

    public static void GenerateSulphurousSeaCut()
    {
        int topOfSulphSea = Main.remixWorld ? YStart - 40 : YStart - 10;
        int bottomOfSulphSea = Main.remixWorld ? YStart + BlockDepth - 60 : AbyssTop + 25;
        int centerX = GetActualX(BiomeWidth / 2 - 32);
        if (Main.remixWorld)
        {
            for (int y = bottomOfSulphSea; y > topOfSulphSea; y--)
            {
                float yCompletion = Utils.GetLerpValue(bottomOfSulphSea, topOfSulphSea - 1f, y, true);
                int width = (int)Utils.Remap(yCompletion, 0f, 0.33f, 1f, 36f) + Terraria.WorldGen.genRand.Next(-1, 2);

                // Carve out water through the sulph sea.
                for (int dx = -width; dx < width; dx++)
                {
                    int x = centerX + dx;
                    Main.tile[x, y].Get<TileWallWireStateData>().HasTile = false;
                    Main.tile[x, y].Get<LiquidData>().LiquidType = LiquidID.Water;
                    Main.tile[x, y].LiquidAmount = 255;
                    Tile.SmoothSlope(x, y);
                }
            }
        }
        else
        {
            for (int y = topOfSulphSea; y < bottomOfSulphSea; y++)
            {
                float yCompletion = Utils.GetLerpValue(topOfSulphSea, bottomOfSulphSea - 1f, y, true);
                int width = (int)Utils.Remap(yCompletion, 0f, 0.33f, 1f, 36f) + Terraria.WorldGen.genRand.Next(-1, 2);

                // Carve out water through the sulph sea.
                for (int dx = -width; dx < width; dx++)
                {
                    int x = centerX + dx;
                    Main.tile[x, y].Get<TileWallWireStateData>().HasTile = false;
                    Main.tile[x, y].Get<LiquidData>().LiquidType = LiquidID.Water;
                    Main.tile[x, y].LiquidAmount = 255;
                    Tile.SmoothSlope(x, y);
                }
            }
        }
    }

    public static void GenerateLayer1()
    {
        int topOfLayer1 = AbyssTop;
        int bottomOfLayer1 = Layer2Top;
        int entireAbyssTop = AbyssTop;
        int entireAbyssBottom = AbyssBottom;
        int minWidth = MinAbyssWidth;
        int maxWidth = MaxAbyssWidth;

        for (int c = 0; c < Layer1SpaghettiCaveCarveOutThresholds.Length; c++)
        {
            int caveSeed = Terraria.WorldGen.genRand.Next();
            if (Main.remixWorld)
            {
                for (int y = topOfLayer1; y > bottomOfLayer1; y--)
                {
                    float yCompletion = Utils.GetLerpValue(entireAbyssTop, entireAbyssBottom, y, true);
                    int width = GetWidth(yCompletion, minWidth, maxWidth) - WallThickness;
                    for (int i = 2; i < width; i++)
                    {
                        // Initialize variables for the cave.
                        int x = GetActualX(i);
                        float noise = FractalBrownianMotion(i * SpaghettiCaveMagnification,
                            y * SpaghettiCaveMagnification, caveSeed, 5);

                        // Bias noise away from 0, effectively making caves less likely to appear, based on how close it is to the edges and bottom.
                        float biasAwayFrom0Interpolant =
                            Utils.GetLerpValue(topOfLayer1 - 12f, topOfLayer1, y, true) * 0.2f;
                        biasAwayFrom0Interpolant += Utils.GetLerpValue(width - 24f, width - 9f, i, true) * 0.4f;
                        biasAwayFrom0Interpolant +=
                            Utils.GetLerpValue(bottomOfLayer1 + 16f, bottomOfLayer1 + 3f, y, true) * 0.4f;

                        // If the noise is less than 0, bias to -1, if it's greater than 0, bias away to 1.
                        // This is done instead of biasing to -1 or 1 without exception to ensure that in doing so the noise does not cross into the
                        // cutout threshold near 0 as it interpolates.
                        noise = Lerp(noise, Math.Sign(noise), biasAwayFrom0Interpolant);

                        if (Math.Abs(noise) < Layer1SpaghettiCaveCarveOutThresholds[c])
                        {
                            Main.tile[x, y].Get<TileWallWireStateData>().HasTile = false;
                            Main.tile[x, y].Get<LiquidData>().LiquidType = LiquidID.Water;
                            Main.tile[x, y].LiquidAmount = 255;
                            Tile.SmoothSlope(x, y);
                        }
                    }
                }
            }
            else
            {
                for (int y = topOfLayer1; y < bottomOfLayer1; y++)
                {
                    float yCompletion = Utils.GetLerpValue(entireAbyssTop, entireAbyssBottom, y, true);
                    int width = GetWidth(yCompletion, minWidth, maxWidth) - WallThickness;
                    for (int i = 2; i < width; i++)
                    {
                        // Initialize variables for the cave.
                        int x = GetActualX(i);
                        float noise = FractalBrownianMotion(i * SpaghettiCaveMagnification,
                            y * SpaghettiCaveMagnification, caveSeed, 5);

                        // Bias noise away from 0, effectively making caves less likely to appear, based on how close it is to the edges and bottom.
                        float biasAwayFrom0Interpolant =
                            Utils.GetLerpValue(topOfLayer1 + 12f, topOfLayer1, y, true) * 0.2f;
                        biasAwayFrom0Interpolant += Utils.GetLerpValue(width - 24f, width - 9f, i, true) * 0.4f;
                        biasAwayFrom0Interpolant +=
                            Utils.GetLerpValue(bottomOfLayer1 - 16f, bottomOfLayer1 - 3f, y, true) * 0.4f;

                        // If the noise is less than 0, bias to -1, if it's greater than 0, bias away to 1.
                        // This is done instead of biasing to -1 or 1 without exception to ensure that in doing so the noise does not cross into the
                        // cutout threshold near 0 as it interpolates.
                        noise = Lerp(noise, Math.Sign(noise), biasAwayFrom0Interpolant);

                        if (Math.Abs(noise) < Layer1SpaghettiCaveCarveOutThresholds[c])
                        {
                            Main.tile[x, y].Get<TileWallWireStateData>().HasTile = false;
                            Main.tile[x, y].Get<LiquidData>().LiquidType = LiquidID.Water;
                            Main.tile[x, y].LiquidAmount = 255;
                            Tile.SmoothSlope(x, y);
                        }
                    }
                }
            }
        }

        // Clear out any stray tiles created by the cave generation.
        Rectangle layer1Area = new(1, topOfLayer1, maxWidth - WallThickness, bottomOfLayer1 - topOfLayer1);
        ClearOutStrayTiles(layer1Area);

        // Generate sulphurous gravel on the cave walls.
        GenerateLayer1SulphurousShale(layer1Area);

        // Generate scenic tiles.
        GeneratePostSkeletronTiles(layer1Area);
        GenerateAbyssalKelp(layer1Area);

        // Create random blobs of planty mush.
        GeneratePlantyMushClumps(layer1Area);

        GenerateScenicTilesInArea(layer1Area, 10, 1, [GetTileId("CalamityMod.Tiles.Abyss.AbyssGravel")],
        [
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.AbyssGiantKelp1"),
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.AbyssGiantKelp2"),
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.AbyssGiantKelp3"),
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.AbyssGiantKelp4")
        ]);
        GenerateScenicTilesInArea(layer1Area, 9, 1, [GetTileId("CalamityMod.Tiles.Abyss.AbyssGravel")],
        [
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.AbyssVent1"),
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.AbyssVent2"),
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.AbyssVent3")
        ]);
        GenerateScenicTilesInArea(layer1Area, 7, 1, [GetTileId("CalamityMod.Tiles.Abyss.AbyssGravel")],
        [
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.GravelPile1"),
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.GravelPile2"),
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.GravelPile3")
        ]);
        GenerateScenicTilesInArea(layer1Area, 20, 1, [GetTileId("CalamityMod.Tiles.Abyss.AbyssGravel")],
        [
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.PirateCrate1"),
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.PirateCrate2"),
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.PirateCrate3"),
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.PirateCrate4"),
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.PirateCrate5"),
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.PirateCrate6")
        ]);
        GenerateScenicTilesInArea(layer1Area, 8, 1, [GetTileId("CalamityMod.Tiles.Abyss.SulphurousShale")],
        [
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.ShalePile1"),
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.ShalePile2"),
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.ShalePile3")
        ]);
    }

    public static void GenerateLayer1SulphurousShale(Rectangle layer1Area)
    {
        int sandstoneSeed = Terraria.WorldGen.genRand.Next();
        AbyssLayer1ForestSeed = Terraria.WorldGen.genRand.Next();

        ushort abyssGravelID = GetTileId("CalamityMod.Tiles.Abyss.AbyssGravel");
        ushort sulphuricShaleID = GetTileId("CalamityMod.Tiles.Abyss.SulphurousShale");

        // Edge score evaluation function that determines the propensity a tile has to become sandstone.
        // This is based on how much nearby empty areas there are, allowing for "edges" to appear.
        static int getEdgeScore(int x, int y)
        {
            int edgeScore = 0;
            for (int dx = x - 6; dx <= x + 6; dx++)
            {
                if (dx == x)
                    continue;

                if (!Framing.GetTileSafely(dx, y).HasTile)
                    edgeScore++;
            }

            for (int dy = y - 6; dy <= y + 6; dy++)
            {
                if (dy == y)
                    continue;

                if (!Framing.GetTileSafely(x, dy).HasTile)
                    edgeScore++;
            }

            return edgeScore;
        }

        for (int i = layer1Area.Left; i < layer1Area.Right; i++)
        {
            if (Main.remixWorld)
            {
                for (int y = layer1Area.Top; y >= layer1Area.Bottom; y--)
                {
                    int x = GetActualX(i);
                    float sulphurousConvertChance = FractalBrownianMotion(i * SandstoneEdgeNoiseMagnification,
                        y * SandstoneEdgeNoiseMagnification, sandstoneSeed, 7) * 0.5f + 0.5f;

                    // Make the sandstone appearance chance dependant on the edge score.
                    sulphurousConvertChance *= Utils.GetLerpValue(4f, 11f, getEdgeScore(x, y), true);

                    if (Terraria.WorldGen.genRand.NextFloat() > sulphurousConvertChance || sulphurousConvertChance < 0.57f)
                        continue;

                    // Convert to sulphuric shale as necessary.
                    for (int dx = -2; dx <= 2; dx++)
                    {
                        for (int dy = -2; dy <= 2; dy++)
                        {
                            if (Terraria.WorldGen.InWorld(x + dx, y + dy))
                            {
                                if (Framing.GetTileSafely(x + dx, y + dy).TileType == abyssGravelID)
                                {
                                    Main.tile[x + dx, y + dy].TileType = sulphuricShaleID;

                                    // Encourage the growth of ground vines.
                                    if (InsideOfLayer1Forest(new(x + dx, y + dy)))
                                    {
                                        int vineHeight = Terraria.WorldGen.genRand.Next(9, 12);
                                        Point vinePosition = new(x + dx, y + dy);

                                        for (int ddy = 0; ddy < vineHeight; ddy++)
                                        {
                                            if (ddy <= 0)
                                                TileLoader.RandomUpdate(vinePosition.X, vinePosition.Y - ddy,
                                                    Framing.GetTileSafely(vinePosition.X, vinePosition.Y - ddy)
                                                        .TileType);
                                            else
                                                AttemptToGrowVine(new(vinePosition.X,
                                                    vinePosition.Y - ddy));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int y = layer1Area.Top; y <= layer1Area.Bottom; y++)
                {
                    int x = GetActualX(i);
                    float sulphurousConvertChance = FractalBrownianMotion(i * SandstoneEdgeNoiseMagnification,
                        y * SandstoneEdgeNoiseMagnification, sandstoneSeed, 7) * 0.5f + 0.5f;

                    // Make the sandstone appearance chance dependant on the edge score.
                    sulphurousConvertChance *= Utils.GetLerpValue(4f, 11f, getEdgeScore(x, y), true);

                    if (Terraria.WorldGen.genRand.NextFloat() > sulphurousConvertChance || sulphurousConvertChance < 0.57f)
                        continue;

                    // Convert to sulphuric shale as necessary.
                    for (int dx = -2; dx <= 2; dx++)
                    {
                        for (int dy = -2; dy <= 2; dy++)
                        {
                            if (Terraria.WorldGen.InWorld(x + dx, y + dy))
                            {
                                if (Framing.GetTileSafely(x + dx, y + dy).TileType == abyssGravelID)
                                {
                                    Main.tile[x + dx, y + dy].TileType = sulphuricShaleID;

                                    // Encourage the growth of ground vines.
                                    if (InsideOfLayer1Forest(new(x + dx, y + dy)))
                                    {
                                        int vineHeight = Terraria.WorldGen.genRand.Next(9, 12);
                                        Point vinePosition = new(x + dx, y + dy);

                                        for (int ddy = 0; ddy < vineHeight; ddy++)
                                        {
                                            if (ddy <= 0)
                                                TileLoader.RandomUpdate(vinePosition.X, vinePosition.Y - ddy,
                                                    Framing.GetTileSafely(vinePosition.X, vinePosition.Y - ddy)
                                                        .TileType);
                                            else
                                                AttemptToGrowVine(new(vinePosition.X,
                                                    vinePosition.Y - ddy));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public static void GenerateAbyssalKelp(Rectangle layer1Area)
    {
        ushort kelpID = GetTileId("InfernumMode.Content.Tiles.Abyss.AbyssalKelp");
        for (int i = layer1Area.Left; i < layer1Area.Right; i++)
        {
            if (Main.remixWorld)
            {
                for (int y = layer1Area.Top; y >= layer1Area.Bottom; y--)
                {
                    int x = GetActualX(i);
                    Tile t = Framing.GetTileSafely(x, y);
                    Tile above = Framing.GetTileSafely(x, y - 1);

                    // Randomly create kelp upward.
                    if (Terraria.WorldGen.SolidTile(t) && !above.HasTile &&
                        Terraria.WorldGen.genRand.NextBool(Layer1KelpCreationChance /
                                                           (InsideOfLayer1Forest(new(x, y)) ? 4 : 1)))
                    {
                        int kelpHeight = Terraria.WorldGen.genRand.Next(6, 12);
                        bool areaIsOccupied = false;

                        // Check if the area where the kelp would be created is occupied.
                        for (int dy = 0; dy < kelpHeight + 4; dy++)
                        {
                            if (Framing.GetTileSafely(x, y - dy - 1).HasTile)
                            {
                                areaIsOccupied = true;
                                break;
                            }
                        }

                        if (areaIsOccupied)
                            continue;

                        for (int dy = 0; dy < kelpHeight; dy++)
                        {
                            Main.tile[x, y - dy - 1].TileType = kelpID;
                            Main.tile[x, y - dy - 1].Get<TileWallWireStateData>().TileFrameX = 0;

                            if (dy == 0)
                                Main.tile[x, y - dy - 1].Get<TileWallWireStateData>().TileFrameY = 72;
                            else if (dy == kelpHeight - 1)
                                Main.tile[x, y - dy - 1].Get<TileWallWireStateData>().TileFrameY = 0;
                            else
                                Main.tile[x, y - dy - 1].Get<TileWallWireStateData>().TileFrameY =
                                    (short)(Terraria.WorldGen.genRand.Next(1, 4) * 18);

                            Main.tile[x, y - dy - 1].Get<TileWallWireStateData>().IsHalfBlock = false;
                            Main.tile[x, y - dy - 1].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                            Main.tile[x, y - dy - 1].Get<TileWallWireStateData>().HasTile = true;
                        }
                    }
                }
            }
            else
            {
                for (int y = layer1Area.Top; y <= layer1Area.Bottom; y++)
                {
                    int x = GetActualX(i);
                    Tile t = Framing.GetTileSafely(x, y);
                    Tile above = Framing.GetTileSafely(x, y - 1);

                    // Randomly create kelp upward.
                    if (Terraria.WorldGen.SolidTile(t) && !above.HasTile &&
                        Terraria.WorldGen.genRand.NextBool(Layer1KelpCreationChance /
                                                           (InsideOfLayer1Forest(new(x, y)) ? 4 : 1)))
                    {
                        int kelpHeight = Terraria.WorldGen.genRand.Next(6, 12);
                        bool areaIsOccupied = false;

                        // Check if the area where the kelp would be created is occupied.
                        for (int dy = 0; dy < kelpHeight + 4; dy++)
                        {
                            if (Framing.GetTileSafely(x, y - dy - 1).HasTile)
                            {
                                areaIsOccupied = true;
                                break;
                            }
                        }

                        if (areaIsOccupied)
                            continue;

                        for (int dy = 0; dy < kelpHeight; dy++)
                        {
                            Main.tile[x, y - dy - 1].TileType = kelpID;
                            Main.tile[x, y - dy - 1].Get<TileWallWireStateData>().TileFrameX = 0;

                            if (dy == 0)
                                Main.tile[x, y - dy - 1].Get<TileWallWireStateData>().TileFrameY = 72;
                            else if (dy == kelpHeight - 1)
                                Main.tile[x, y - dy - 1].Get<TileWallWireStateData>().TileFrameY = 0;
                            else
                                Main.tile[x, y - dy - 1].Get<TileWallWireStateData>().TileFrameY =
                                    (short)(Terraria.WorldGen.genRand.Next(1, 4) * 18);

                            Main.tile[x, y - dy - 1].Get<TileWallWireStateData>().IsHalfBlock = false;
                            Main.tile[x, y - dy - 1].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                            Main.tile[x, y - dy - 1].Get<TileWallWireStateData>().HasTile = true;
                        }
                    }
                }
            }
        }
    }

    public static void GeneratePlantyMushClumps(Rectangle layer1Area)
    {
        for (int i = 0; i < 18; i++)
        {
            int x = GetActualX(Terraria.WorldGen.genRand.Next(layer1Area.Left + 90, layer1Area.Right - 70));
            int y = Main.remixWorld
                ? Terraria.WorldGen.genRand.Next(layer1Area.Bottom + 15, layer1Area.Top - 40)
                : Terraria.WorldGen.genRand.Next(layer1Area.Top + 40, layer1Area.Bottom - 15);
            Point p = new(x, y);

            // Make the surrounding pyre molten.
            WorldUtils.Gen(p, new Shapes.Circle(Terraria.WorldGen.genRand.Next(14, 18)), Actions.Chain(
            [
                new Modifiers.RadialDither(12f, 18f),
                new Modifiers.Conditions(new Conditions.IsTile(GetTileId("CalamityMod.Tiles.Abyss.AbyssGravel"),
                    GetTileId("CalamityMod.Tiles.Abyss.SulphurousShale"))),
                new Actions.SetTile(GetTileId("CalamityMod.Tiles.Abyss.PlantyMush")),
                new Actions.PlaceWall(GetWallId("CalamityMod.Walls.AbyssGravelWall")),
            ]));
        }
    }

    public static void GeneratePostSkeletronTiles(Rectangle layer1Area)
    {
        Dictionary<int, Point> tilesLeft = new()
        {
            [GetTileId("InfernumMode.Content.Tiles.Abyss.IronBootsSkeleton")] = new(3, 2),
            [GetTileId("InfernumMode.Content.Tiles.Abyss.StrangeOrbTile")] = new(2, 2)
        };

        for (int i = 0; i < 72000; i++)
        {
            if (tilesLeft.Count <= 0)
                break;

            int x = GetActualX(Terraria.WorldGen.genRand.Next(layer1Area.Left + 72, layer1Area.Right - 72));
            int y = Main.remixWorld
                ? Terraria.WorldGen.genRand.Next(layer1Area.Bottom + 40, layer1Area.Top - 48)
                : Terraria.WorldGen.genRand.Next(layer1Area.Top + 48, layer1Area.Bottom - 40);

            bool areaIsOccupied = false;
            bool solidGround = true;

            // Check if the area where the kelp would be created is occupied.
            for (int dx = 0; dx < 5; dx++)
            {
                for (int dy = 0; dy < 3; dy++)
                {
                    if (Framing.GetTileSafely(x + dx, y - dy).HasTile)
                    {
                        areaIsOccupied = true;
                        break;
                    }
                }

                if (!Terraria.WorldGen.SolidTile(Framing.GetTileSafely(x + dx, y + 1)) ||
                    !Framing.GetTileSafely(x + dx, y + 1).HasTile)
                    solidGround = false;
            }

            if (areaIsOccupied || !solidGround)
                continue;

            int tileID = tilesLeft.Last().Key;
            for (int dx = 0; dx < tilesLeft[tileID].X; dx++)
            {
                for (int dy = 0; dy < tilesLeft[tileID].Y; dy++)
                {
                    Main.tile[x + dx, y - dy].TileType = (ushort)tileID;
                    Main.tile[x + dx, y - dy].Get<TileWallWireStateData>().TileFrameX = (short)(dx * 18);
                    Main.tile[x + dx, y - dy].Get<TileWallWireStateData>().TileFrameY =
                        (short)((tilesLeft[tileID].Y - dy) * 18 - 18);
                    Main.tile[x + dx, y - dy].Get<TileWallWireStateData>().IsHalfBlock = false;
                    Main.tile[x + dx, y - dy].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                    Main.tile[x + dx, y - dy].Get<TileWallWireStateData>().HasTile = true;
                }
            }

            tilesLeft.Remove(tileID);
        }
    }

    public static void GenerateLayer2(out List<Point> trenchBottoms)
    {
        int trenchCount = Layer2TrenchCount;
        int topOfLayer2 = Layer2Top + Main.remixWorld.ToDirectionInt() * 30;
        int bottomOfLayer2 = Layer3Top;
        int maxWidth = MaxAbyssWidth - WallThickness;

        // Initialize the trench list.
        List<Point> trenchTops = [];
        trenchBottoms = [];

        // Generate a bunch of preset trenches that reach down to the bottom of the layer. They are mostly vertical, but can wind a bit, and are filled with bioluminescent plants.
        for (int i = 0; i < trenchCount; i++)
        {
            int trenchX = (int)Lerp(104f, maxWidth * 0.7f, i / (float)(trenchCount - 1f)) +
                          Terraria.WorldGen.genRand.Next(-15, 15);
            int trenchY = topOfLayer2 + Main.remixWorld.ToDirectionInt() * Terraria.WorldGen.genRand.Next(25, 35);
            trenchTops.Add(new(GetActualX(trenchX), trenchY));
            trenchBottoms.Add(GenerateLayer2Trench(new(GetActualX(trenchX), trenchY),
                bottomOfLayer2 - Main.remixWorld.ToDirectionInt() * 4));
        }

        // Create a connecting cavern between the left and rightmost trenches.
        GenerateLayer2ConnectingCavern(trenchTops.First(), trenchBottoms.Last());

        Rectangle layer2Area = new(1, topOfLayer2, maxWidth, bottomOfLayer2 - topOfLayer2);
        GenerateLayer2Wildlife(layer2Area);
    }

    public static Point GenerateLayer2Trench(Point start, int cutOffPoint)
    {
        Point currentPoint = start;
        ushort voidstoneID = GetTileId("CalamityMod.Tiles.Abyss.Voidstone");
        ushort voidstoneWallID = GetWallId("CalamityMod.Walls.VoidstoneWallUnsafe");

        // Descend downward, carving out gravel.
        int startingWidth = Terraria.WorldGen.genRand.Next(MinStartingTrenchWidth, MaxStartingTrenchWidth);
        int endingWidth = Terraria.WorldGen.genRand.Next(MinEndingTrenchWidth, MaxEndingTrenchWidth);
        int offsetSeed = Terraria.WorldGen.genRand.Next();
        int widthSeed = Terraria.WorldGen.genRand.Next();

        if (Main.remixWorld)
        {
            while (currentPoint.Y > cutOffPoint)
            {
                float yCompletion = Utils.GetLerpValue(start.Y, cutOffPoint, currentPoint.Y, true);
                float noiseWidthOffset = FractalBrownianMotion(currentPoint.X * TrenchWidthNoiseMagnificationFactor,
                    currentPoint.Y * TrenchWidthNoiseMagnificationFactor, widthSeed, 5) * endingWidth * 0.2f;
                int width = (int)(Lerp(startingWidth, endingWidth, Pow(yCompletion, TrenchTightnessFactor)) +
                                  noiseWidthOffset);
                width = Utils.Clamp(width, startingWidth, endingWidth);

                // Calculate the horizontal offset of the current trench.
                int currentOffset =
                    (int)(FractalBrownianMotion(currentPoint.X * TrenchOffsetNoiseMagnificationFactor,
                        currentPoint.Y * TrenchOffsetNoiseMagnificationFactor, offsetSeed, 5) * MaxTrenchOffset);

                // Occasionally carve out lumenyl and voidstone shells at the edges of the current point.
                if (Terraria.WorldGen.genRand.NextBool(50) && currentPoint.Y > cutOffPoint + width + 8 &&
                    currentPoint.Y <= start.Y - 30)
                {
                    int shellRadius = width + Terraria.WorldGen.genRand.Next(4);
                    Point voidstoneShellCenter =
                        new(
                            currentPoint.X + currentOffset +
                            Terraria.WorldGen.genRand.NextBool().ToDirectionInt() * width / 2 + Terraria.WorldGen.genRand.Next(-4, 4),
                            currentPoint.Y - shellRadius - 1);
                    voidstoneShellCenter.X = Utils.Clamp(voidstoneShellCenter.X, 35, Main.maxTilesX - 35);

                    WorldUtils.Gen(voidstoneShellCenter, new Shapes.Circle(shellRadius), Actions.Chain(
                    [
                        new Modifiers.Blotches(),
                        new Actions.SetTile(voidstoneID),
                        new Actions.PlaceWall(voidstoneWallID),
                    ]));

                    // Carve out the inside of the shell.
                    WorldUtils.Gen(voidstoneShellCenter, new Shapes.Circle(shellRadius - 2), Actions.Chain(
                    [
                        new Actions.ClearTile(true),
                        new Actions.PlaceWall(voidstoneWallID),
                        new Actions.SetLiquid()
                    ]));
                }

                for (int dx = -width / 2; dx < width / 2; dx++)
                    ResetToWater(new(currentPoint.X + dx + currentOffset, currentPoint.Y));

                currentPoint.Y--;
            }
        }
        else
        {
            while (currentPoint.Y < cutOffPoint)
            {
                float yCompletion = Utils.GetLerpValue(start.Y, cutOffPoint, currentPoint.Y, true);
                float noiseWidthOffset = FractalBrownianMotion(currentPoint.X * TrenchWidthNoiseMagnificationFactor,
                    currentPoint.Y * TrenchWidthNoiseMagnificationFactor, widthSeed, 5) * endingWidth * 0.2f;
                int width = (int)(Lerp(startingWidth, endingWidth, Pow(yCompletion, TrenchTightnessFactor)) +
                                  noiseWidthOffset);
                width = Utils.Clamp(width, startingWidth, endingWidth);

                // Calculate the horizontal offset of the current trench.
                int currentOffset =
                    (int)(FractalBrownianMotion(currentPoint.X * TrenchOffsetNoiseMagnificationFactor,
                        currentPoint.Y * TrenchOffsetNoiseMagnificationFactor, offsetSeed, 5) * MaxTrenchOffset);

                // Occasionally carve out lumenyl and voidstone shells at the edges of the current point.
                if (Terraria.WorldGen.genRand.NextBool(50) && currentPoint.Y < cutOffPoint - width - 8 &&
                    currentPoint.Y >= start.Y + 30)
                {
                    int shellRadius = width + Terraria.WorldGen.genRand.Next(4);
                    Point voidstoneShellCenter =
                        new(
                            currentPoint.X + currentOffset +
                            Terraria.WorldGen.genRand.NextBool().ToDirectionInt() * width / 2 + Terraria.WorldGen.genRand.Next(-4, 4),
                            currentPoint.Y + shellRadius + 1);
                    voidstoneShellCenter.X = Utils.Clamp(voidstoneShellCenter.X, 35, Main.maxTilesX - 35);

                    WorldUtils.Gen(voidstoneShellCenter, new Shapes.Circle(shellRadius), Actions.Chain(
                    [
                        new Modifiers.Blotches(),
                        new Actions.SetTile(voidstoneID),
                        new Actions.PlaceWall(voidstoneWallID),
                    ]));

                    // Carve out the inside of the shell.
                    WorldUtils.Gen(voidstoneShellCenter, new Shapes.Circle(shellRadius - 2), Actions.Chain(
                    [
                        new Actions.ClearTile(true),
                        new Actions.PlaceWall(voidstoneWallID),
                        new Actions.SetLiquid()
                    ]));
                }

                for (int dx = -width / 2; dx < width / 2; dx++)
                    ResetToWater(new(currentPoint.X + dx + currentOffset, currentPoint.Y));

                currentPoint.Y++;
            }
        }

        // Return the end position of the trench. This is used by the layer 3 gen to determine where caves should begin being carved out.
        return currentPoint;
    }

    public static void GenerateLayer2ConnectingCavern(Point topLeft, Point bottomRight)
    {
        topLeft.Y -= Main.remixWorld.ToDirectionInt() * 24;
        bottomRight.Y += Main.remixWorld.ToDirectionInt() * 24;

        // Maintain coordinate consistency such that the top left point is actually to the left.
        if (topLeft.X > bottomRight.X)
            Utils.Swap(ref topLeft, ref bottomRight);

        // Generate a ragged cavern between the trenches.
        int width = Terraria.WorldGen.genRand.Next(MinEndingTrenchWidth, MaxEndingTrenchWidth) / 3;
        int offsetSeed = Terraria.WorldGen.genRand.Next();
        Point currentPoint = topLeft;

        while (!currentPoint.ToVector2().WithinRange(bottomRight.ToVector2(), 18f))
        {
            // Calculate the horizontal offset of the current trench.
            int currentOffset =
                (int)(FractalBrownianMotion(currentPoint.X * TrenchOffsetNoiseMagnificationFactor * 3f,
                          currentPoint.Y * TrenchOffsetNoiseMagnificationFactor * 3f, offsetSeed, 5) *
                      MaxTrenchOffset *
                      0.67f);

            for (int dx = -width / 2; dx < width / 2; dx++)
            {
                for (int dy = 0; dy < 3; dy++)
                    ResetToWater(new(currentPoint.X + dx + currentOffset, currentPoint.Y + dy));
            }

            currentPoint = (currentPoint.ToVector2() +
                            (bottomRight.ToVector2() - currentPoint.ToVector2()).SafeNormalize(Vector2.UnitY) * 2f)
                .ToPoint();
        }
    }

    public static void GenerateLayer2Wildlife(Rectangle area)
    {
        List<int> wildlifeVariants =
        [
            GetTileId("InfernumMode.Content.Tiles.Abyss.AbyssalCoral"),
            GetTileId("InfernumMode.Content.Tiles.Abyss.FluorescentPolyp"),
            GetTileId("InfernumMode.Content.Tiles.Abyss.HadalSeagrass"),
            GetTileId("InfernumMode.Content.Tiles.Abyss.LumenylPolyp"),
        ];

        for (int i = 0; i < Layer2WildlifeSpawnAttempts; i++)
        {
            Point potentialPosition = new(GetActualX(Terraria.WorldGen.genRand.Next(area.Left + 30, area.Right - 30)),
                Terraria.WorldGen.genRand.Next(Math.Min(area.Top, area.Bottom), Math.Max(area.Top, area.Bottom)));
            Terraria.WorldGen.PlaceTile(potentialPosition.X, potentialPosition.Y, Terraria.WorldGen.genRand.Next(wildlifeVariants));
        }

        GenerateScenicTilesInArea(area, 10, 1, [GetTileId("CalamityMod.Tiles.Abyss.AbyssGravel")],
        [
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.AbyssGiantKelp1"),
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.AbyssGiantKelp2"),
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.AbyssGiantKelp3"),
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.AbyssGiantKelp4")
        ]);
        GenerateScenicTilesInArea(area, 9, 1, [GetTileId("CalamityMod.Tiles.Abyss.AbyssGravel")],
        [
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.AbyssVent1"),
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.AbyssVent2"),
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.AbyssVent3")
        ]);
        GenerateScenicTilesInArea(area, 7, 1, [GetTileId("CalamityMod.Tiles.Abyss.AbyssGravel")],
        [
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.BulbTree1"),
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.BulbTree2"),
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.BulbTree3")
        ]);
        GenerateScenicTilesInArea(area, 7, 1, [GetTileId("CalamityMod.Tiles.Abyss.AbyssGravel")],
        [
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.GravelPile1"),
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.GravelPile2"),
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.GravelPile3")
        ]);
        GenerateScenicTilesInArea(area, 10, 5, [GetTileId("CalamityMod.Tiles.Abyss.AbyssGravel")],
        [
            GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.TenebrisRemnant")
        ]);
        GenerateScenicTilesInArea(area, 5, 16, [GetTileId("CalamityMod.Tiles.Abyss.AbyssGravel")],
        [
            GetTileId("InfernumMode.Content.Tiles.Abyss.AbyssalKelp")
        ]);
    }

    public static void GenerateLayer3(List<Point> trenchBottoms, out Point layer4ConvergencePoint)
    {
        int topOfLayer3 = Layer3Top;
        int bottomOfLayer3 = Layer4Top;
        int entireAbyssTop = AbyssTop;
        int entireAbyssBottom = AbyssBottom;
        ushort voidstoneWallID = GetWallId("CalamityMod.Walls.VoidstoneWallUnsafe");
        layer4ConvergencePoint = new(GetActualX((MaxAbyssWidth - WallThickness) / 2),
            Layer4Top - Main.remixWorld.ToDirectionInt() * 5);
        List<int> caveSeeds = [];
        List<Vector2> caveNoisePositions = [];
        List<Point> caveEndPoints = [];
        int minWidth = MinAbyssWidth;
        int maxWidth = MaxAbyssWidth;

        // Initialize cave positions.
        for (int i = 0; i < trenchBottoms.Count; i++)
        {
            caveSeeds.Add(Terraria.WorldGen.genRand.Next());
            caveNoisePositions.Add(Terraria.WorldGen.genRand.NextVector2Unit());
            caveEndPoints.Add(trenchBottoms[i]);
        }

        // Carve out the base caves.
        for (int i = 0; i < trenchBottoms.Count; i++)
        {
            for (int j = 0; j < 196; j++)
            {
                int carveOutArea = (int)Utils.Remap(caveNoisePositions[i].Y, Math.Min(Layer3Top, Layer4Top),
                    Math.Max(Layer3Top, Layer4Top), MinLayer3CaveSize, MaxLayer3CaveSize);

                // Slightly update the coordinates of the input value, in "noise space".
                // This causes the worm's shape to be slightly different in the next frame.
                // The x coordinate of the input value is shifted in a negative direction,
                // which propagates the previous Perlin-noise values over to subsequent
                // segments.  This produces a "slithering" effect.
                caveNoisePositions[i] +=
                    new Vector2(-carveOutArea * 0.0002f, -Main.remixWorld.ToDirectionInt() * 0.0033f);

                // Make caves converge towards a central pit the closer they are to reaching the 4th layer.
                float convergenceInterpolant =
                    Utils.GetLerpValue(Layer4Top + Main.remixWorld.ToDirectionInt() * 75f,
                        Layer4Top + Main.remixWorld.ToDirectionInt() * 8f, caveEndPoints[i].Y, true);

                // Make caves stay within the abyss.
                float moveToEdgeInterpolant;
                Vector2 edgeDirection;
                if (AtLeftSideOfWorld)
                {
                    edgeDirection = -Vector2.UnitX;
                    moveToEdgeInterpolant = Utils.GetLerpValue(MaxAbyssWidth - WallThickness - 32f,
                        MaxAbyssWidth - WallThickness, caveEndPoints[i].X, true);
                }
                else
                {
                    edgeDirection = Vector2.UnitX;
                    moveToEdgeInterpolant = Utils.GetLerpValue(Main.maxTilesX - (MaxAbyssWidth - WallThickness),
                        Main.maxTilesX - (MaxAbyssWidth - WallThickness - 32f), caveEndPoints[i].X, true);
                }

                // Make caves stay within layer 3.
                float moveDownwardInterpolant =
                    Utils.GetLerpValue(Layer3Top - Main.remixWorld.ToDirectionInt() * 50f,
                        Layer3Top - Main.remixWorld.ToDirectionInt() * 20f, caveEndPoints[i].Y, true);

                Vector2 directionToConvergencePoint =
                    (layer4ConvergencePoint.ToVector2() - caveEndPoints[i].ToVector2()).SafeNormalize(
                        -Main.remixWorld.ToDirectionInt() * Vector2.UnitY);
                Vector2 caveMoveDirection =
                    (TwoPi * FractalBrownianMotion(caveNoisePositions[i].X, caveNoisePositions[i].Y, caveSeeds[i],
                        5)).ToRotationVector2();
                caveMoveDirection = Vector2.Lerp(caveMoveDirection, edgeDirection, moveToEdgeInterpolant);
                caveMoveDirection = Vector2.Lerp(caveMoveDirection,
                    -Main.remixWorld.ToDirectionInt() * Vector2.UnitY, moveDownwardInterpolant);
                caveMoveDirection = Vector2
                    .Lerp(caveMoveDirection, directionToConvergencePoint, convergenceInterpolant)
                    .SafeNormalize(Vector2.Zero);

                Vector2 caveMoveOffset = caveMoveDirection * carveOutArea * 0.333f;

                // 防止x过大
                float yCompletion = Utils.GetLerpValue(entireAbyssTop, entireAbyssBottom, caveEndPoints[i].Y, true);
                if (caveEndPoints[i].X >= GetWidth(yCompletion, minWidth, maxWidth) - WallThickness)
                    caveMoveOffset.X = 0;

                caveEndPoints[i] += caveMoveOffset.ToPoint();
                caveEndPoints[i] = new(Utils.Clamp(caveEndPoints[i].X, 45, Main.maxTilesX - 45),
                    caveEndPoints[i].Y);

                WorldUtils.Gen(caveEndPoints[i], new Shapes.Circle(carveOutArea), Actions.Chain(
                [
                    new Actions.ClearTile(),
                    new Actions.PlaceWall(voidstoneWallID),
                    new Actions.SetLiquid(),
                    new Actions.Smooth()
                ]));
            }
        }

        // Carve out finer, spaghetti caves.
        for (int c = 0; c < Layer3SpaghettiCaveCarveOutThresholds.Length; c++)
        {
            int caveSeed = Terraria.WorldGen.genRand.Next();
            if (Main.remixWorld)
            {
                for (int y = topOfLayer3; y > bottomOfLayer3 + 14; y--)
                {
                    float yCompletion = Utils.GetLerpValue(entireAbyssTop, entireAbyssBottom, y, true);
                    int width = GetWidth(yCompletion, minWidth, maxWidth) - WallThickness;
                    for (int i = 2; i < width; i++)
                    {
                        // Initialize variables for the cave.
                        int x = GetActualX(i);
                        float noise = FractalBrownianMotion(i * SpaghettiCaveMagnification,
                            y * SpaghettiCaveMagnification, caveSeed, 3);

                        // Bias noise away from 0, effectively making caves less likely to appear, based on how close it is to the edges and bottom.
                        float biasAwayFrom0Interpolant =
                            Utils.GetLerpValue(width - 24f, width - 9f, i, true) * 0.4f;
                        biasAwayFrom0Interpolant +=
                            Utils.GetLerpValue(Layer4Top + 24f, Layer4Top + 10f, y, true) * 0.4f;

                        // If the noise is less than 0, bias to -1, if it's greater than 0, bias away to 1.
                        // This is done instead of biasing to -1 or 1 without exception to ensure that in doing so the noise does not cross into the
                        // cutout threshold near 0 as it interpolates.
                        noise = Lerp(noise, Math.Sign(noise), biasAwayFrom0Interpolant);

                        if (Math.Abs(noise) < Layer3SpaghettiCaveCarveOutThresholds[c])
                        {
                            Main.tile[x, y].Get<TileWallWireStateData>().HasTile = false;
                            Main.tile[x, y].Get<LiquidData>().LiquidType = LiquidID.Water;
                            Main.tile[x, y].WallType = voidstoneWallID;
                            Main.tile[x, y].LiquidAmount = 255;
                            Tile.SmoothSlope(x, y);
                        }
                    }
                }
            }
            else
            {
                for (int y = topOfLayer3; y < bottomOfLayer3 - 14; y++)
                {
                    float yCompletion = Utils.GetLerpValue(entireAbyssTop, entireAbyssBottom, y, true);
                    int width = GetWidth(yCompletion, minWidth, maxWidth) - WallThickness;
                    for (int i = 2; i < width; i++)
                    {
                        // Initialize variables for the cave.
                        int x = GetActualX(i);
                        float noise = FractalBrownianMotion(i * SpaghettiCaveMagnification,
                            y * SpaghettiCaveMagnification, caveSeed, 3);

                        // Bias noise away from 0, effectively making caves less likely to appear, based on how close it is to the edges and bottom.
                        float biasAwayFrom0Interpolant =
                            Utils.GetLerpValue(width - 24f, width - 9f, i, true) * 0.4f;
                        biasAwayFrom0Interpolant +=
                            Utils.GetLerpValue(Layer4Top - 24f, Layer4Top - 10f, y, true) * 0.4f;

                        // If the noise is less than 0, bias to -1, if it's greater than 0, bias away to 1.
                        // This is done instead of biasing to -1 or 1 without exception to ensure that in doing so the noise does not cross into the
                        // cutout threshold near 0 as it interpolates.
                        noise = Lerp(noise, Math.Sign(noise), biasAwayFrom0Interpolant);

                        if (Math.Abs(noise) < Layer3SpaghettiCaveCarveOutThresholds[c])
                        {
                            Main.tile[x, y].Get<TileWallWireStateData>().HasTile = false;
                            Main.tile[x, y].Get<LiquidData>().LiquidType = LiquidID.Water;
                            Main.tile[x, y].WallType = voidstoneWallID;
                            Main.tile[x, y].LiquidAmount = 255;
                            Tile.SmoothSlope(x, y);
                        }
                    }
                }
            }
        }

        // Carve out a large area at the layer 4 entrance.
        WorldUtils.Gen(layer4ConvergencePoint, new Shapes.Circle(72), Actions.Chain(
        [
            new Actions.ClearTile(),
            new Actions.PlaceWall(voidstoneWallID),
            new Actions.SetLiquid(),
            new Actions.Smooth()
        ]));

        // Clear out any stray tiles created by the cave generation.
        Rectangle layer3Area = new(1, topOfLayer3, maxWidth - WallThickness, bottomOfLayer3 - topOfLayer3);
        ClearOutStrayTiles(layer3Area);

        AbyssLayer3CavernSeed = Terraria.WorldGen.genRand.Next();

        // Generate pyre mantle in the hydrothermic zone.
        GenerateLayer3PyreMantle(layer3Area);

        // Generate scenic hydrothermal vents.
        GenerateLayer3Vents(layer3Area);

        // Scatter crystals. This encompasses the creation of the lumenyl zone.
        GenerateLayer3LumenylCrystals(layer3Area);

        // Generate the squid den.
        GenerateLayer3SquidDen(layer3Area);

        // Create scenic tiles.
        GenerateScenicTilesInArea(layer3Area, 12, 1,
            [GetTileId("CalamityMod.Tiles.Abyss.PyreMantle"), GetTileId("CalamityMod.Tiles.Abyss.PyreMantleMolten")],
            [
                GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.SpiderCoral1"),
                GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.SpiderCoral2"),
                GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.SpiderCoral3"),
                GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.SpiderCoral4"),
                GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.SpiderCoral5")
            ]);
        GenerateScenicTilesInArea(layer3Area, 14, 1,
            [GetTileId("CalamityMod.Tiles.Abyss.PyreMantle"), GetTileId("CalamityMod.Tiles.Abyss.PyreMantleMolten")],
            [
                GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.ThermalVent1"),
                GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.ThermalVent2"),
                GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.ThermalVent3")
            ]);
    }

    public static void GenerateLayer3Vents(Rectangle area)
    {
        ushort gravelID = GetTileId("CalamityMod.Tiles.Abyss.AbyssGravel");
        ushort scoriaOre = GetTileId("CalamityMod.Tiles.Ores.ScoriaOre");
        List<Point> ventPositions = [];

        int tries = 0;
        for (int i = 0; i < Layer3VentCount; i++)
        {
            tries++;
            if (tries >= 20000)
                break;

            Point potentialVentPosition = new(GetActualX(Terraria.WorldGen.genRand.Next(area.Left + 30, area.Right - 30)),
                Terraria.WorldGen.genRand.Next(Math.Min(area.Top, area.Bottom), Math.Max(area.Top, area.Bottom)));
            Tile t = Framing.GetTileSafely(potentialVentPosition.X, potentialVentPosition.Y);

            // Ignore placement positions that are already occupied.
            if (t.HasTile)
            {
                i--;
                continue;
            }

            // Ignore positions that are close to an existing vent.
            Point floor = GetGroundPositionFrom(potentialVentPosition,
                Main.remixWorld ? new Searches.Up(9001) : new Searches.Down(9001));
            if (ventPositions.Any(p => p.ToVector2().Distance(floor.ToVector2()) < 24f))
            {
                i--;
                continue;
            }

            Point floorLeft = GetGroundPositionFrom(
                new Point(potentialVentPosition.X - 3, potentialVentPosition.Y),
                Main.remixWorld ? new Searches.Up(9001) : new Searches.Down(9001));
            Point floorRight = GetGroundPositionFrom(
                new Point(potentialVentPosition.X + 3, potentialVentPosition.Y),
                Main.remixWorld ? new Searches.Up(9001) : new Searches.Down(9001));
            Point ceiling = GetGroundPositionFrom(potentialVentPosition,
                Main.remixWorld ? new Searches.Down(9001) : new Searches.Up(9001));

            // Ignore cramped spaces.
            if (Distance(floor.Y, ceiling.Y) < 10)
            {
                i--;
                continue;
            }

            // Ignore steep spaces.
            float averageY = Math.Abs(floorLeft.Y + floor.Y + floorRight.Y) / 3f;
            if (Distance(averageY, floor.Y) >= 4f)
            {
                i--;
                continue;
            }

            // Ignore points outside of the hydrothermal zone.
            int zeroBiasedX = floor.X;
            if (zeroBiasedX >= Main.maxTilesX / 2)
                zeroBiasedX = Main.maxTilesX - zeroBiasedX;

            if (!InsideOfLayer3HydrothermalZone(new(zeroBiasedX, floor.Y)))
            {
                i--;
                continue;
            }

            // Make the surrounding pyre molten.
            WorldUtils.Gen(floor, new Shapes.Circle(Terraria.WorldGen.genRand.Next(15, 22)), Actions.Chain(
            [
                new Modifiers.Conditions(new Conditions.IsTile(GetTileId("CalamityMod.Tiles.Abyss.PyreMantle"))),
                new Actions.SetTile(GetTileId("CalamityMod.Tiles.Abyss.PyreMantleMolten")),

                new Actions.PlaceWall(GetWallId("CalamityMod.Walls.PyreMantleWall"))
            ]));

            // Generate a stand of scoria.
            int moundHeight = Terraria.WorldGen.genRand.Next(4, 9);
            int scoriaGroundSize = Terraria.WorldGen.genRand.Next(5, 7);
            WorldUtils.Gen(new(floor.X, floor.Y - Main.remixWorld.ToDirectionInt() * scoriaGroundSize / 2),
                new Shapes.Slime(scoriaGroundSize), Actions.Chain(
                [
                    new Actions.SetTile(scoriaOre, true),
                ]));
            WorldUtils.Gen(floor, new Shapes.Mound(5, moundHeight), Actions.Chain(
            [
                new Actions.SetTile(gravelID, true),
            ]));

            ushort ventID = (ushort)Terraria.WorldGen.genRand.Next(new int[]
            {
                GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.ThermalVent1"),
                GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.ThermalVent2"),
                GetTileId("CalamityMod.Tiles.Abyss.AbyssAmbient.ThermalVent3")
            });
            if (Distance(
                    GetGroundPositionFrom(
                        new Point(floor.X, floor.Y + Main.remixWorld.ToDirectionInt() * moundHeight),
                        Main.remixWorld ? new Searches.Down(50) : new Searches.Up(50)).Y,
                    floor.Y + Main.remixWorld.ToDirectionInt() * moundHeight) >= 7f)
                PlaceObjectWithGoddamnForce(floor.X, floor.Y + Main.remixWorld.ToDirectionInt() * moundHeight,
                    ventID, 0);

            ventPositions.Add(floor);
        }
    }

    public static void GenerateLayer3LumenylCrystals(Rectangle area)
    {
        TryToGenerateLumenylCrystals(area, 80, false);
        TryToGenerateLumenylCrystals(area, 700, true, (x, y) => InsideOfLayer3LumenylZone(new(x, y)));
        TryToGenerateLumenylCrystals(area, 3000, false, (x, y) => InsideOfLayer3LumenylZone(new(x, y)));
    }

    public static void TryToGenerateLumenylCrystals(Rectangle area, int placementCount, bool largeCrystals,
        Func<int, int, bool> extraCondition = null)
    {
        ushort lumenylID = GetTileId("CalamityMod.Tiles.Abyss.LumenylCrystals");
        if (largeCrystals)
            lumenylID = GetTileId("InfernumMode.Content.Tiles.Abyss.LargeLumenylCrystal");

        var fakeItem = new Item();
        fakeItem.SetDefaults(ItemID.StoneBlock);

        int tries = 0;

        for (int i = 0; i < placementCount; i++)
        {
            // Give up once enough tries have been attempted.
            tries++;
            if (tries >= 32000)
                break;

            Point potentialCrystalPosition = new(GetActualX(Terraria.WorldGen.genRand.Next(area.Left + 30, area.Right - 30)),
                Terraria.WorldGen.genRand.Next(Math.Min(area.Top, area.Bottom), Math.Max(area.Top, area.Bottom)));
            Tile t = Framing.GetTileSafely(potentialCrystalPosition.X, potentialCrystalPosition.Y);

            // Ignore placement positions that are already occupied.
            if (t.HasTile)
            {
                i--;
                continue;
            }

            // Ignore placement positions with nothing to attach to.
            Tile left = Framing.GetTileSafely(potentialCrystalPosition.X - 1, potentialCrystalPosition.Y);
            Tile right = Framing.GetTileSafely(potentialCrystalPosition.X + 1, potentialCrystalPosition.Y);
            Tile top = Framing.GetTileSafely(potentialCrystalPosition.X, potentialCrystalPosition.Y - 1);
            Tile bottom = Framing.GetTileSafely(potentialCrystalPosition.X, potentialCrystalPosition.Y + 1);
            if (!left.HasTile && !right.HasTile && !top.HasTile && !bottom.HasTile)
            {
                i--;
                continue;
            }

            if (!Terraria.WorldGen.SolidTile(left) && !Terraria.WorldGen.SolidTile(right) && !Terraria.WorldGen.SolidTile(top) &&
                !Terraria.WorldGen.SolidTile(bottom))
            {
                i--;
                continue;
            }

            // Ignore placement positions that violate the extra condition, if it exists.
            if (!extraCondition?.Invoke(potentialCrystalPosition.X, potentialCrystalPosition.Y) ?? false)
            {
                i--;
                continue;
            }

            t.TileType = lumenylID;
            t.HasTile = true;
            t.IsHalfBlock = false;
            t.Get<TileWallWireStateData>().Slope = SlopeType.Solid;

            t.TileFrameX = (short)(Terraria.WorldGen.genRand.Next(18) * 18);

            bool invalidPlacement = false;

            if (bottom.HasTile && Main.tileSolid[bottom.TileType] && bottom.Slope == 0 && !bottom.IsHalfBlock)
            {
                t.TileFrameY = 0;
                if (largeCrystals &&
                    (Utilities.DistanceToTileCollisionHit(potentialCrystalPosition.ToWorldCoordinates(),
                        -Vector2.UnitY, 25) ?? 500f) < 64f)
                    invalidPlacement = true;
            }
            else if (top.HasTile && Main.tileSolid[top.TileType] && top.Slope == 0 && !top.IsHalfBlock)
            {
                t.TileFrameY = 18;
                if (largeCrystals &&
                    (Utilities.DistanceToTileCollisionHit(potentialCrystalPosition.ToWorldCoordinates(),
                        Vector2.UnitY, 25) ?? 500f) < 64f)
                    invalidPlacement = true;
            }
            else if (right.HasTile && Main.tileSolid[right.TileType] && right.Slope == 0 && !right.IsHalfBlock)
            {
                t.TileFrameY = 36;
                if (largeCrystals &&
                    (Utilities.DistanceToTileCollisionHit(potentialCrystalPosition.ToWorldCoordinates(),
                        -Vector2.UnitX, 25) ?? 500f) < 64f)
                    invalidPlacement = true;
            }
            else if (left.HasTile && Main.tileSolid[left.TileType] && left.Slope == 0 && !left.IsHalfBlock)
            {
                t.TileFrameY = 54;
                if (largeCrystals &&
                    (Utilities.DistanceToTileCollisionHit(potentialCrystalPosition.ToWorldCoordinates(),
                        Vector2.UnitX, 25) ?? 500f) < 64f)
                    invalidPlacement = true;
            }

            if (invalidPlacement)
            {
                i--;
                t.HasTile = false;
            }
        }
    }

    public static void GenerateLayer3PyreMantle(Rectangle area)
    {
        int top = Layer3Top + Main.remixWorld.ToDirectionInt() * 10;
        int wallSeed = Terraria.WorldGen.genRand.Next();
        ushort gravelID = GetTileId("CalamityMod.Tiles.Abyss.AbyssGravel");
        ushort pyreID = GetTileId("CalamityMod.Tiles.Abyss.PyreMantle");
        ushort pyreWallID = GetWallId("CalamityMod.Walls.PyreMantleWall");
        FastRandom rng = new(Terraria.WorldGen.genRand.Next());

        for (int i = area.Left; i < area.Right + 36; i++)
        {
            int x = GetActualX(i);
            if (Main.remixWorld)
            {
                for (int y = area.Top; y > area.Bottom; y--)
                {
                    if (Main.tile[x, y].TileType != gravelID || !Main.tile[x, y].HasTile)
                        continue;

                    if (!InsideOfLayer3HydrothermalZone(new(i, y)))
                        continue;

                    float ditherChance = Utils.GetLerpValue(top, top - 50f, y, true) *
                                         Utils.GetLerpValue(area.Right - 25f, area.Right - 60f, i, true);

                    // Perform dithering.
                    if (rng.NextFloat() > ditherChance)
                        continue;

                    Main.tile[x, y].TileType = pyreID;

                    if (FractalBrownianMotion(x * Layer3CrystalCaveMagnificationFactor * 4f,
                            y * Layer3CrystalCaveMagnificationFactor * 4f, wallSeed, 5) >= 0f)
                        Main.tile[x, y].WallType = pyreWallID;
                }
            }
            else
            {
                for (int y = area.Top; y < area.Bottom; y++)
                {
                    if (Main.tile[x, y].TileType != gravelID || !Main.tile[x, y].HasTile)
                        continue;

                    if (!InsideOfLayer3HydrothermalZone(new(i, y)))
                        continue;

                    float ditherChance = Utils.GetLerpValue(top, top + 50f, y, true) *
                                         Utils.GetLerpValue(area.Right - 25f, area.Right - 60f, i, true);

                    // Perform dithering.
                    if (rng.NextFloat() > ditherChance)
                        continue;

                    Main.tile[x, y].TileType = pyreID;

                    if (FractalBrownianMotion(x * Layer3CrystalCaveMagnificationFactor * 4f,
                            y * Layer3CrystalCaveMagnificationFactor * 4f, wallSeed, 5) >= 0f)
                        Main.tile[x, y].WallType = pyreWallID;
                }
            }
        }
    }

    public static void GenerateLayer3SquidDen(Rectangle area)
    {
        ushort voidstoneID = GetTileId("CalamityMod.Tiles.Abyss.Voidstone");
        ushort voidstoneWallID = GetWallId("CalamityMod.Walls.VoidstoneWallUnsafe");

        // Generate a voidstone circle for the den and then cut out most of its insides to create a shell.
        SquidDenCenter = new(GetActualX(area.Left + Layer3SquidDenOuterRadius + 15),
            area.Top - Main.remixWorld.ToDirectionInt() * Layer3SquidDenOuterRadius);
        WorldUtils.Gen(SquidDenCenter, new Shapes.Circle(Layer3SquidDenOuterRadius), Actions.Chain(
        [
            new Modifiers.Blotches(3),
            new Actions.SetTile(voidstoneID),
            new Actions.PlaceWall(voidstoneWallID)
        ]));
        WorldUtils.Gen(SquidDenCenter, new Shapes.Circle(Layer3SquidDenInnerRadius), Actions.Chain(
        [
            new Modifiers.Blotches(4),
            new Actions.ClearTile(),
            new Modifiers.Blotches(4),
            new Actions.SetLiquid()
        ]));

        // Carve out caves that cut through the shell.
        Point caveStart = new(SquidDenCenter.X + AtLeftSideOfWorld.ToDirectionInt() * Layer3SquidDenOuterRadius,
            SquidDenCenter.Y - Main.remixWorld.ToDirectionInt() * Layer3SquidDenOuterRadius);
        for (int i = 0; i < 50; i++)
        {
            float cavePerpendicularAngle = Sin(i * 0.39f);
            Point currentCavePosition = Vector2
                .Lerp(caveStart.ToVector2(), SquidDenCenter.ToVector2(), i / 49f).ToPoint();
            Vector2 directionToCenter =
                (SquidDenCenter.ToVector2() - caveStart.ToVector2()).SafeNormalize(Vector2.Zero);
            currentCavePosition += (directionToCenter.RotatedBy(cavePerpendicularAngle) * 8f).ToPoint() *
                                   new Point(1, -1);

            WorldUtils.Gen(currentCavePosition, new Shapes.Circle(3), Actions.Chain(
            [
                new Actions.ClearTile(),
                new Actions.SetLiquid()
            ]));
        }

        // Create a lot of crystals inside of the squid den.
        Point zeroBiasedDenCenter =
            new(area.Left + Layer3SquidDenOuterRadius + 15, SquidDenCenter.Y);
        Rectangle denArea = Utils.CenteredRectangle(zeroBiasedDenCenter.ToVector2(),
            Vector2.One * Layer3SquidDenOuterRadius * 2.4f);
        TryToGenerateLumenylCrystals(denArea, 200, false);
        TryToGenerateLumenylCrystals(denArea, 300, true);
    }

    public static void GenerateLayer4(Point pedestalPosition)
    {
        int minWidth = MinAbyssWidth;
        int maxWidth = MaxAbyssWidth;
        int entireAbyssTop = AbyssTop;
        int top = Layer4Top;
        int bottom = AbyssBottom;
        int offsetSeed = Terraria.WorldGen.genRand.Next();
        ushort voidstoneWallID = GetWallId("CalamityMod.Walls.VoidstoneWallUnsafe");
        var terminusPedestalPosition = new Point((MaxAbyssWidth - WallThickness - 40) / 2, 80);

        for (int i = 1; i < maxWidth; i++)
        {
            int x = GetActualX(i);
            int yOffset = (int)Math.Abs(FractalBrownianMotion(x / 276f, 0.4f, offsetSeed, 5) * 40f);
            if (Main.remixWorld)
            {
                for (int y = top + yOffset; y > bottom + WallThickness - yOffset / 3; y--)
                {
                    // Decide whether to cut off due to a Y point being far enough.
                    int xOffset = (int)Math.Abs(FractalBrownianMotion(1.34f, y / 209f, offsetSeed, 5) * 20f);
                    float yCompletion = Utils.GetLerpValue(entireAbyssTop, bottom - 1f, y, true);
                    if (i >= GetWidth(yCompletion, minWidth, maxWidth) - WallThickness + xOffset)
                        continue;

                    // 终末石基座
                    if (x >= terminusPedestalPosition.X - 1 && x <= terminusPedestalPosition.X + 1 &&
                        y == terminusPedestalPosition.Y + 1)
                        continue;

                    // Otherwise, clear out water.
                    Main.tile[x, y].WallType = voidstoneWallID;
                    ResetToWater(new(x, y));
                }
            }
            else
            {
                for (int y = top - yOffset; y < bottom - WallThickness + yOffset / 3; y++)
                {
                    // Decide whether to cut off due to a Y point being far enough.
                    int xOffset = (int)Math.Abs(FractalBrownianMotion(1.34f, y / 209f, offsetSeed, 5) * 20f);
                    float yCompletion = Utils.GetLerpValue(entireAbyssTop, bottom - 1f, y, true);
                    if (i >= GetWidth(yCompletion, minWidth, maxWidth) - WallThickness + xOffset)
                        continue;

                    // Otherwise, clear out water.
                    Main.tile[x, y].WallType = voidstoneWallID;
                    ResetToWater(new(x, y));
                }
            }
        }

        // Create the eidolist pedestal.
        GenerateEidolistPedestal(new(pedestalPosition.X,
            pedestalPosition.Y + Main.remixWorld.ToDirectionInt() * 8));

        // Place the Terminus tile.
        GenerateTerminusTile(Main.remixWorld
            ? terminusPedestalPosition
            : new(pedestalPosition.X, pedestalPosition.Y - Main.remixWorld.ToDirectionInt() * 50));
    }

    public static void GenerateEidolistPedestal(Point pedestalCenter)
    {
        SetValue<PropertyInfo>("InfernumMode.Core.GlobalInstances.Systems.WorldSaveSyste",
            "EidolistWorshipPedestalCenter",
            new Point(pedestalCenter.X, pedestalCenter.Y + Main.remixWorld.ToDirectionInt() * 2));

        ushort voidstoneID = GetTileId("CalamityMod.Tiles.Abyss.Voidstone");
        WorldUtils.Gen(pedestalCenter,
            InvokeConstructor<GenShape>("InfernumMode.Common.Worldgen.CustomInfernumShapes", "HalfCircle",
                EidolistPedestalRadius),
            Actions.Chain(
            [
                new Modifiers.Blotches(2, 0.36),
                new Actions.SetTile(voidstoneID)
            ]));
    }

    public static void GenerateTerminusTile(Point searchPosition)
    {
        int terminusTileID = GetTileId("InfernumMode.Content.Tiles.Abyss.TerminusTile");
        //int dylanShellID = GetTileId("CalamityMod.Tiles.Abyss.AbyssFossilTile");

        if (Main.remixWorld)
        {
            Terraria.WorldGen.PlaceTile(searchPosition.X, searchPosition.Y, terminusTileID);
        }
        else
        {
            for (int tries = 0; tries < 180; tries++)
            {
                Point groundPosition = GetGroundPositionFrom(searchPosition,
                    Main.remixWorld ? new Searches.Up(9001) : new Searches.Down(9001));
                groundPosition.Y += Main.remixWorld.ToDirectionInt();

                if (Terraria.WorldGen.PlaceTile(groundPosition.X, groundPosition.Y, terminusTileID))
                    break;

                // Slide to the right for a different search position if the current one didn't find a valid spot.
                searchPosition.X++;
            }
        }
    }

    public static void GenerateVoidstone()
    {
        int top = Layer3Top + Main.remixWorld.ToDirectionInt() * 10;
        int bottom = AbyssBottom - Main.remixWorld.ToDirectionInt() * 10;
        ushort gravelID = GetTileId("CalamityMod.Tiles.Abyss.AbyssGravel");
        ushort gravelWallID = GetWallId("CalamityMod.Walls.AbyssGravelWall");
        ushort voidstoneID = GetTileId("CalamityMod.Tiles.Abyss.Voidstone");
        ushort voidstoneWallID = GetWallId("CalamityMod.Walls.VoidstoneWall");
        ushort pyreID = GetTileId("CalamityMod.Tiles.Abyss.PyreMantle");
        FastRandom rng = new(Terraria.WorldGen.genRand.Next());

        if (Main.remixWorld)
        {
            for (int y = top; y > bottom; y--)
            {
                float ditherChance = Utils.GetLerpValue(top, top - 16f, y, true);
                for (int i = 0; i < MaxAbyssWidth; i++)
                {
                    Tile t = Framing.GetTileSafely(GetActualX(i), y);

                    // Don't convert tiles that aren't abyss gravel in some way.
                    if ((t.TileType != gravelID || !t.HasTile) && t.WallType != gravelWallID)
                        continue;

                    // Don't convert pyre mantle.
                    if (t.TileType == pyreID)
                        continue;

                    // Perform dithering.
                    if (rng.NextFloat() > ditherChance)
                        continue;

                    t.TileType = voidstoneID;
                    t.WallType = voidstoneWallID;
                }
            }
        }
        else
        {
            for (int y = top; y < bottom; y++)
            {
                float ditherChance = Utils.GetLerpValue(top, top + 16f, y, true);
                for (int i = 0; i < MaxAbyssWidth; i++)
                {
                    Tile t = Framing.GetTileSafely(GetActualX(i), y);

                    // Don't convert tiles that aren't abyss gravel in some way.
                    if ((t.TileType != gravelID || !t.HasTile) && t.WallType != gravelWallID)
                        continue;

                    // Don't convert pyre mantle.
                    if (t.TileType == pyreID)
                        continue;

                    // Perform dithering.
                    if (rng.NextFloat() > ditherChance)
                        continue;

                    t.TileType = voidstoneID;
                    t.WallType = voidstoneWallID;
                }
            }
        }
    }

    #endregion Generation Functions

    #region Utilities

    public static int GetWidth(float yCompletion, int minWidth, int maxWidth)
    {
        return (int)Lerp(minWidth, maxWidth,
            Pow(yCompletion * Utils.GetLerpValue(1f, 0.81f, yCompletion, true), 0.13f));
    }

    public static void ClearOutStrayTiles(Rectangle area)
    {
        int width = BiomeWidth;
        int depth = BlockDepth;
        List<ushort> blockTileTypes =
        [
            GetTileId("CalamityMod.Tiles.Abyss.AbyssGravel"),
            GetTileId("CalamityMod.Tiles.Abyss.Voidstone")
        ];

        void getAttachedPoints(int x, int y, List<Point> points)
        {
            Tile t = Framing.GetTileSafely(x, y);
            Point p = new(x, y);
            if (!blockTileTypes.Contains(t.TileType) || !t.HasTile || points.Count > 672 || points.Contains(p))
                return;

            points.Add(p);

            getAttachedPoints(x + 1, y, points);
            getAttachedPoints(x - 1, y, points);
            getAttachedPoints(x, y + 1, points);
            getAttachedPoints(x, y - 1, points);
        }

        // Clear out stray chunks created by caverns.
        for (int i = area.Left; i < area.Right; i++)
        {
            int x = GetActualX(i);
            if (Main.remixWorld)
            {
                for (int y = area.Top; y > area.Bottom; y--)
                {
                    List<Point> chunkPoints = [];
                    getAttachedPoints(x, y, chunkPoints);

                    if (chunkPoints.Count is >= 2 and < 672)
                    {
                        foreach (Point p in chunkPoints)
                            ResetToWater(p);
                    }

                    // Clear any tiles that have no nearby tiles.
                    if (!Framing.GetTileSafely(x - 1, y).HasTile &&
                        !Framing.GetTileSafely(x + 1, y).HasTile &&
                        !Framing.GetTileSafely(x, y - 1).HasTile &&
                        !Framing.GetTileSafely(x, y + 1).HasTile)
                    {
                        ResetToWater(new(x, y));
                    }
                }
            }
            else
            {
                for (int y = area.Top; y < area.Bottom; y++)
                {
                    List<Point> chunkPoints = [];
                    getAttachedPoints(x, y, chunkPoints);

                    if (chunkPoints.Count is >= 2 and < 672)
                    {
                        foreach (Point p in chunkPoints)
                            ResetToWater(p);
                    }

                    // Clear any tiles that have no nearby tiles.
                    if (!Framing.GetTileSafely(x - 1, y).HasTile &&
                        !Framing.GetTileSafely(x + 1, y).HasTile &&
                        !Framing.GetTileSafely(x, y - 1).HasTile &&
                        !Framing.GetTileSafely(x, y + 1).HasTile)
                    {
                        ResetToWater(new(x, y));
                    }
                }
            }
        }
    }

    public static void ResetToWater(Point p)
    {
        Terraria.WorldGen.KillTile(p.X, p.Y);

        Main.tile[p].Get<TileWallWireStateData>().HasTile = false;
        Main.tile[p].Get<LiquidData>().LiquidType = LiquidID.Water;
        Main.tile[p].LiquidAmount = 255;

        if (p.X >= 5 && p.Y < Main.maxTilesX - 5)
            Tile.SmoothSlope(p.X, p.Y);
    }


    public static void GenerateScenicTilesInArea(Rectangle area, int chance, int variants, ushort[] groundTiles,
        ushort[] tileVariants)
    {
        for (int i = area.Left; i < area.Right; i++)
        {
            int x = GetActualX(i);
            if (Main.remixWorld)
            {
                for (int y = area.Top; y > area.Bottom; y--)
                {
                    Tile t = Framing.GetTileSafely(x, y);
                    Tile below = Framing.GetTileSafely(x, y + 1);
                    if (!Terraria.WorldGen.SolidTile(below) || t.HasTile)
                        continue;

                    if (!Terraria.WorldGen.genRand.NextBool(chance))
                        continue;

                    if (!groundTiles.Contains(below.TileType))
                        continue;

                    ushort tileID = Terraria.WorldGen.genRand.Next(tileVariants);
                    PlaceObjectWithGoddamnForce(x, y, tileID, Terraria.WorldGen.genRand.Next(variants));
                }
            }
            else
            {
                for (int y = area.Top; y < area.Bottom; y++)
                {
                    Tile t = Framing.GetTileSafely(x, y);
                    Tile below = Framing.GetTileSafely(x, y + 1);
                    if (!Terraria.WorldGen.SolidTile(below) || t.HasTile)
                        continue;

                    if (!Terraria.WorldGen.genRand.NextBool(chance))
                        continue;

                    if (!groundTiles.Contains(below.TileType))
                        continue;

                    ushort tileID = Terraria.WorldGen.genRand.Next(tileVariants);
                    PlaceObjectWithGoddamnForce(x, y, tileID, Terraria.WorldGen.genRand.Next(variants));
                }
            }
        }
    }

    public static void PlaceObjectWithGoddamnForce(int x, int y, ushort tileID, int variant)
    {
        if (!TileObject.CanPlace(x, y, tileID, variant, 0, out _, true))
            return;

        int width = 1;
        int height = 1;
        var tileData = TileObjectData.GetTileData(tileID, variant);
        if (tileData is not null)
        {
            width = tileData.CoordinateFullWidth / 18;
            height = tileData.CoordinateFullHeight / 18;
        }

        for (int i = 0; i < width; i++)
        {
            if (!Terraria.WorldGen.SolidTile(x + i, y + 1))
                return;
        }

        // Clear out tiles that were in the area previously, to prevent overlap.
        for (int i = x; i < x + width; i++)
        {
            for (int j = 0; j < height; j++)
                Terraria.WorldGen.KillTile(i, y - j);
        }

        for (int i = x; i < x + width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Main.tile[i, y - j].TileType = tileID;
                Main.tile[i, y - j].Get<TileWallWireStateData>().HasTile = true;
                Main.tile[i, y - j].Get<TileWallWireStateData>().TileFrameX =
                    (short)((i - x + variant * width) * 18);
                Main.tile[i, y - j].Get<TileWallWireStateData>().TileFrameY = (short)((height - j - 1) * 18);
                Main.tile[i, y - j].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                Main.tile[i, y - j].Get<TileWallWireStateData>().IsHalfBlock = false;
            }
        }
    }

    public static bool InsideOfLayer1Forest(Point p)
    {
        int x = p.X;
        if (x >= Main.maxTilesX / 2)
            x = Main.maxTilesX - x;

        if (x >= BiomeWidth - WallThickness + 20)
            return false;

        if ((!Main.remixWorld && (p.Y < AbyssTop + 25 || p.Y >= Layer2Top - 5)) ||
            (Main.remixWorld && (p.Y > AbyssTop - 25 || p.Y <= Layer2Top + 5)))
            return false;

        float forestNoise = FractalBrownianMotion(p.X * Layer1ForestNoiseMagnificationFactor,
            p.Y * Layer1ForestNoiseMagnificationFactor, AbyssLayer1ForestSeed, 5) * 0.5f + 0.5f;
        return forestNoise > Layer1ForestMinNoiseValue;
    }

    public static bool InsideOfLayer3LumenylZone(Point p)
    {
        int x = p.X;
        if (x >= Main.maxTilesX / 2)
            x = Main.maxTilesX - x;

        if (x >= MaxAbyssWidth - WallThickness + 20)
            return false;

        if ((!Main.remixWorld && (p.Y < Layer3Top + 1 || p.Y >= Layer4Top - 5)) ||
            (Main.remixWorld && (p.Y > Layer3Top - 1 || p.Y <= Layer4Top + 5)))
            return false;

        // The squid den is always considered a part of the lumenyl zone.
        if (InsideOfLayer3SquidDen(p))
            return false;

        float verticalOffset = FractalBrownianMotion(p.X * Layer3CrystalCaveMagnificationFactor,
            p.Y * Layer3CrystalCaveMagnificationFactor, AbyssLayer3CavernSeed, 4) * 45f;

        // Bias towards crystal caves as they reach the fourth layer.
        return Utils.Remap(p.Y + verticalOffset, Layer4Top + Main.remixWorld.ToDirectionInt() * 126f,
            Layer4Top + Main.remixWorld.ToDirectionInt() * 104f, 1f, 0f) < CrystalCaveNoiseThreshold;
    }

    public static bool InsideOfLayer3HydrothermalZone(Point p)
    {
        int x = p.X;
        if (Terraria.WorldGen.generatingWorld && x >= Main.maxTilesX / 2)
            x = Main.maxTilesX - x;

        if (x >= MaxAbyssWidth - WallThickness + 1)
            return false;

        if ((!Main.remixWorld && (p.Y < Layer3Top + 1 || p.Y >= Layer4Top - 5)) ||
            (Main.remixWorld && (p.Y > Layer3Top - 1 || p.Y <= Layer4Top + 5)))
            return false;

        // The squid den is always considered a part of the lumenyl zone.
        if (InsideOfLayer3SquidDen(p))
            return true;

        float verticalOffset = FractalBrownianMotion(p.X * Layer3CrystalCaveMagnificationFactor,
            p.Y * Layer3CrystalCaveMagnificationFactor, AbyssLayer3CavernSeed, 4) * 45f;

        // Bias towards crystal caves as they reach the fourth layer.
        return Utils.Remap(p.Y + verticalOffset, Layer4Top + Main.remixWorld.ToDirectionInt() * 126f,
            Layer4Top + Main.remixWorld.ToDirectionInt() * 104f, 1f, 0f) >= CrystalCaveNoiseThreshold;
    }

    public static bool InsideOfLayer3SquidDen(Point p)
    {
        if (p.X < Main.maxTilesX / 2)
            p.X = Main.maxTilesX - p.X;

        return p.ToVector2().WithinRange(SquidDenCenter.ToVector2(), Layer3SquidDenOuterRadius);
    }

    #endregion Utilities

    #region 新增方法

    public static void ChangeLavaToWater()
    {
        if (!Main.remixWorld) // 非颠倒世界跳过
        {
            return;
        }

        int minWidth = MinAbyssWidth;
        int maxWidth = MaxAbyssWidth;
        int top = AbyssTop;
        int bottom = AbyssBottom;

        for (int i = 1; i < maxWidth; i++)
        {
            int x = GetActualX(i);
            for (int y = top; y > bottom; y--)
            {
                Tile t = Framing.GetTileSafely(x, y);
                float yCompletion = Utils.GetLerpValue(top, bottom - 1f, y, true);
                if (i >= GetWidth(yCompletion, minWidth, maxWidth))
                {
                    continue;
                }

                // 将岩浆和黑曜石转换为水
                if (t.HasTile)
                {
                    if (t.TileType == TileID.Obsidian)
                    {
                        ResetToWater(new Point(x, y));
                    }
                }
                else
                {
                    if (t.LiquidType == LiquidID.Lava)
                    {
                        t.LiquidType = LiquidID.Water;
                    }

                    if (t.LiquidAmount < 255)
                    {
                        t.LiquidAmount = 255;
                    }
                }
            }
        }
    }

    public static bool IsInsideOfAbyss(Point p)
    {
        var verticalCheck = Main.remixWorld
            ? AbyssBottom <= p.Y && p.Y < AbyssTop + 34
            : AbyssTop - 34 < p.Y && p.Y <= AbyssBottom;
        var yCompletion = Utils.GetLerpValue(AbyssTop, AbyssBottom - 1f, p.Y / 16f, true);
        var abyssWidth = GetWidth(yCompletion, MinAbyssWidth, MaxAbyssWidth);
        var horizontalCheck = AtLeftSideOfWorld ? p.X < abyssWidth : p.X > Main.maxTilesX - abyssWidth;
        return verticalCheck && horizontalCheck;
    }

    #endregion 新增方法
}