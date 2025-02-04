using Terraria.ID;
using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeadCellsBossFight.Contents.Tiles;

namespace DeadCellsBossFight.Core.TexGenWorld
{
    //读图生成世界案例
    public partial class CoraliteWorld
    {
        /// <summary>
        /// 冰龙巢穴中心点
        /// </summary>
        public static Point NestCenter;

        public static void GenIceDragonNest(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "正在制作冰龙巢穴";
            progress.Set(0);
            //位置
            int nestCenter_x = 145;
            int nestCenter_y = 60;

            Tile tile = Framing.GetTileSafely(nestCenter_x, nestCenter_y);
            if (!tile.HasTile)
            {
                tile.HasTile = true;
                tile.TileType = TileID.Dirt;
            }

            nestCenter_y += 5;

            Texture2D nestTex = ModContent.Request<Texture2D>("Coralite/Assets/Textures/IceNest1", AssetRequestMode.ImmediateLoad).Value;
            Texture2D clearTex = ModContent.Request<Texture2D>("Coralite/Assets/Textures/IceNestClear1", AssetRequestMode.ImmediateLoad).Value;

            int genOrigin_x = nestCenter_x - clearTex.Width / 2;
            int genOrigin_y = nestCenter_y - clearTex.Height / 2;

            NestCenter = new Point(genOrigin_x + 52, genOrigin_y + 20);

            Dictionary<Color, int> clearDic = new Dictionary<Color, int>()
            {
                [Color.White] = -2,
                [Color.Black] = -1
            };
            Dictionary<Color, int> nestDic = new Dictionary<Color, int>()
            {
                [new Color(95, 205, 228)] = ModContent.TileType<DCNormalTile>(),
                [new Color(215, 123, 186)] = TileID.IceBrick,
                [new Color(99, 155, 255)] = TileID.SnowBlock,
                [new Color(63, 63, 116)] = TileID.BreakableIce,
                [Color.Black] = -1
            };

            Task.Run(async () =>
            {
                await GenIceNestWithTex(clearTex, nestTex, clearDic, nestDic, genOrigin_x, genOrigin_y);
            }).Wait();

            progress.Set(0.75);
            //生成装饰物
            WorldGenHelper.PlaceOnTopDecorations(genOrigin_x, genOrigin_y, 0, 0, nestTex.Width, nestTex.Height, TileID.Stalactite, 10, 0);
            WorldGenHelper.PlaceOnGroundDecorations(genOrigin_x, genOrigin_y, 0, 0, nestTex.Width, nestTex.Height, TileID.SmallPiles, 10, 5);
            WorldGenHelper.PlaceOnGroundDecorations(genOrigin_x, genOrigin_y, 0, 0, nestTex.Width, nestTex.Height, TileID.SmallPiles, 10, 6);
            WorldGenHelper.PlaceOnGroundDecorations(genOrigin_x, genOrigin_y, 0, 0, nestTex.Width, nestTex.Height, TileID.SmallPiles, 3, 24);
            WorldGenHelper.PlaceOnGroundDecorations(genOrigin_x, genOrigin_y, 0, 0, nestTex.Width, nestTex.Height, TileID.LargePiles, 3, 8);

            //添加斜坡
            WorldGenHelper.SmoothSlope(genOrigin_x,genOrigin_y, 0, 0, nestTex.Width, nestTex.Height, TileID.IceBlock, 5);
            WorldGenHelper.SmoothSlope(genOrigin_x, genOrigin_y, 0, 0, nestTex.Width, nestTex.Height, TileID.SnowBlock, 5);
        }

        private static Task GenIceNestWithTex(Texture2D clearTex, Texture2D nestTex, Dictionary<Color, int> clearDic, Dictionary<Color, int> nestDic, int genOrigin_x, int genOrigin_y)
        {
            bool genned = false;
            bool placed = false;
            while (!genned)
            {
                if (placed)
                    continue;

                Main.QueueMainThreadAction(() =>
                {
                    //清理范围
                    Texture2TileGenerator clearGenerator = TextureGeneratorDatas.GetTex2TileGenerator(clearTex, clearDic);
                    clearGenerator.Generate(genOrigin_x, genOrigin_y, true);

                    //生成主体地形
                    Texture2TileGenerator nestGenerator = TextureGeneratorDatas.GetTex2TileGenerator(nestTex, nestDic);
                    nestGenerator.Generate(genOrigin_x, genOrigin_y, true);
                    genned = true;
                });
                placed = true;
            }

            return Task.CompletedTask;
        }
    }
}
