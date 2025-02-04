using DeadCellsBossFight.Contents.Tiles;
using DeadCellsBossFight.Core.TexGenWorld;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using DeadCellsBossFight.Contents.Biomes.Prison;

namespace DeadCellsBossFight.Contents.SubWorlds;

public class DCWorldGenPasses
{
    //以下是示例地形生成
    // 冰龙巢穴中心点
    public static Point NestCenter;
    public class CoraliteGenpass : GenPass
    {
        public CoraliteGenpass() : base("Terrain", 1) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Generating terrain"; // Sets the text displayed for this pass
            Main.worldSurface = Main.maxTilesY - 8;
            Main.rockLayer = Main.maxTilesY - 9;

            CoraliteWorld.GenIceDragonNest(progress, configuration);
        }
    }

    /// <summary>
    /// 生成黑色大桥子世界
    /// </summary>
    public class BBGenPass : GenPass
    {
        public BBGenPass() : base("Terrain", 1f) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            // 界面要更改
            // Set the progress text.
            progress.Message = "Forming the Black Bridge.";

            // Define the position of the world lines.
            Main.worldSurface = Main.maxTilesY - 8;
            Main.rockLayer = Main.maxTilesY - 9;

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                progress.Set((96 - i * Main.maxTilesY) + (float)(Main.maxTilesX) + (float)(Main.maxTilesY)); // Controls the progress bar, should only be set between 0f and 1f
                Tile tile = Main.tile[i, 96];
                tile.HasTile = true;
                tile.TileType = (ushort)ModContent.TileType<DCNormalTile>();
                // 底部
                for (int j = 97; j < 113; j++)
                {
                    Tile tile2 = Main.tile[i, j];
                    tile2.HasTile = true;
                    tile2.TileType = (ushort)ModContent.TileType<TransparentTile>();
                }
            }

            // 生成位置：世界左侧
            Main.spawnTileX = 74;
        }
    }
    public class QAGenPass : GenPass
    {
        public QAGenPass() : base("Terrain", 1f) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            // 界面要更改
            // Set the progress text.
            progress.Message = "Forming the Queen Arena.";

            // Define the position of the world lines.
            Main.worldSurface = Main.maxTilesY - 8;
            Main.rockLayer = Main.maxTilesY - 9;

            for (int i = 86; i < Main.maxTilesX - 88; i++)
            {
                // 顶端
                progress.Set((94 - i * Main.maxTilesY) + (float)(Main.maxTilesX) + (float)(Main.maxTilesY)); // Controls the progress bar, should only be set between 0f and 1f
                Tile tile = Main.tile[i, 94];
                tile.HasTile = true;
                tile.TileType = (ushort)ModContent.TileType<DCNormalTile>();

                // 底部
                for (int j = 95;  j < Main.maxTilesY; j++)
                {
                    Tile tile2 = Main.tile[i, j];
                    tile2.HasTile = true;
                    tile2.TileType = (ushort)ModContent.TileType<TransparentTile>();
                }
            }

            // 生成位置：世界左侧
            Main.spawnTileX = 100;
        }

    }
    public class PrisonGenPass : GenPass
    {
        public PrisonGenPass() : base("Terrain", 1f) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            // 界面要更改
            // Set the progress text.
            progress.Message = "Forming the Prison.";

            // Define the position of the world lines.
            Main.worldSurface = Main.maxTilesY - 8;
            Main.rockLayer = Main.maxTilesY - 9;

            for (int i = 40; i < 44; i++)
            {
                int k = 201 - i;
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Tile tile = Main.tile[i, j];
                    Tile tile2 = Main.tile[k, j];
                    tile.HasTile = true;
                    tile2.HasTile = true;
                    tile.TileType = TileID.Dirt;
                    tile2.TileType = TileID.Dirt;
                }
            }

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                progress.Set((102 - i * Main.maxTilesY) + (float)(Main.maxTilesX) + (float)(Main.maxTilesY)); // Controls the progress bar, should only be set between 0f and 1f
                Tile tile = Main.tile[i, 102];
                tile.HasTile = true;
                tile.TileType = (ushort)ModContent.TileType<DCNormalTile>();
                // 底部
                for (int j = 103; j < 120; j++)
                {
                    Tile tile2 = Main.tile[i, j];
                    tile2.HasTile = true;
                    tile2.TileType = (ushort)ModContent.TileType<TransparentTile>();
                }
            }

            // 生成位置：世界左侧
            Main.spawnTileX = 74;
        }
    }

    public static void AddNineBlock(int i, int j, ushort type)
    {
        for (int k = i; k < i + 3; k++)
        {
            for (int m = j; m < j + 3; m++)
            {
                Tile tile = Main.tile[k, m];
                tile.HasTile = true;
                tile.TileType = type;
            }
        }
    }
}
