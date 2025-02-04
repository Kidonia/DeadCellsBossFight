using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;

namespace DeadCellsBossFight.Contents.Biomes.QueenArena;

public class QueenArenaSky : DCBasicSky
{
    private Texture2D QA_bg;
    private Texture2D QA_bgclouds1;
    private Texture2D QA_bgclouds2;
    private Texture2D stars;
    private Texture2D moon;
    private Texture2D moonglow;
    private Texture2D clouds1;
    private Texture2D clouds2;
    private Texture2D clouds3;
    private Texture2D clouds4;
    private Texture2D backFar;
    private Texture2D[] hangerTex = new Texture2D[6];
    private int[] hangerX;
    private int[] hangerY;
    private Texture2D rempartBase;
    private Texture2D rempart;
    private Texture2D stone0;
    private Texture2D stone1;
    private Texture2D floorStamp;
    private Texture2D sideWall;
    private Texture2D window;
    private Texture2D frontBase;
    private Texture2D frontMiddle;
    private Texture2D[] fronthangerTex = new Texture2D[3];
    private Texture2D[] shootstarsTex = new Texture2D[22];
    private Texture2D[] torchFireTex = new Texture2D[14];
    private Texture2D[] fireTex = new Texture2D[18];
    private List<MapParticle> shootstarsList = new List<MapParticle>();
    private List<MapParticle> fireList = new List<MapParticle>();
    private List<MapParticle> fireListLeft = new List<MapParticle>();
    private List<MapParticle> fireListRight = new List<MapParticle>();
    public override void OnLoad()
    {
        string path = "DeadCellsBossFight/Contents/Biomes/QueenArena/QAElements/";
        QA_bg = GetTex(path + "QA_bg");
        QA_bgclouds1 = GetTex(path + "QA_bgclouds1");
        QA_bgclouds2 = GetTex(path + "QA_bgclouds2");
        stars = GetTex(path + "stars");
        moon = GetTex(path + "moon");
        moonglow = GetTex(path + "moonglow");
        clouds1 = GetTex(path + "clouds1");
        clouds2 = GetTex(path + "clouds2");
        clouds3 = GetTex(path + "clouds3");
        clouds4 = GetTex(path + "clouds4");
        backFar = GetTex(path + "backFar");
        hangerX = new int[6] {1210, 1076, 1342, 1088, 1266, 1450};
        hangerY = new int[6] {1486, 1355, 1264, 528, 1138, 910};
        rempartBase = GetTex(path + "rempartBase");
        rempart = GetTex(path + "rempart");
        stone0 = GetTex(path + "stone0");
        stone1 = GetTex(path + "stone1");
        floorStamp = GetTex(path + "floorStamp");
        sideWall = GetTex(path + "sideWall");
        window = GetTex(path + "window");
        frontBase = GetTex(path + "frontBase");
        frontMiddle = GetTex(path + "frontMiddle");
        for (int i  = 0; i < 3; i++)
            fronthangerTex[i] = GetTex(path + "HangerTex/front" + (i + 1).ToString());
        for (int i = 0; i < 6; i++)
            hangerTex[i] = GetTex(path + "HangerTex/hanger" +  (i+1).ToString());
        for (int i = 0; i < 22; i++)
            shootstarsTex[i] = GetTex(path + "ShootStarsTex/" + i.ToString());
        for (int i=0; i <14;  i++)
            torchFireTex[i] = GetTex(path + "TorchFireTex/" + i.ToString());
        for (int i = 0; i < 18; i++)
            fireTex[i] = GetTex(path + "FireTex/" + i.ToString());
        for (int i = 0;i < 25; i++)
            fireList.Add(new MapParticle(new Vector2(1720 + i * 122, 1416),  Main.rand.NextFloat(2.7f, 3.2f), Main.rand.Next(18)));
        for (int i = 0; i < 5; i++)
        {
            fireListLeft.Add(new MapParticle(new Vector2(1550, 1420 + i * 80), Main.rand.NextFloat(2.7f, 3.2f), Main.rand.Next(18)));
            fireListRight.Add(new MapParticle(new Vector2(4662, 1420 + i * 80), Main.rand.NextFloat(2.7f, 3.2f), Main.rand.Next(18)));
        }
        for(int i = 0; i < 3; i++)
        {
            fireListLeft.Add(new MapParticle(new Vector2(1670, 1420 + i * 80), Main.rand.NextFloat(2.7f, 3.2f), Main.rand.Next(18)));
            fireListLeft.Add(new MapParticle(new Vector2(1610, 1580 + i * 70), Main.rand.NextFloat(2.7f, 3.2f), Main.rand.Next(18)));
            fireListRight.Add(new MapParticle(new Vector2(4528, 1500 + i * 80), Main.rand.NextFloat(2.7f, 3.2f), Main.rand.Next(18)));
        }
    }
    public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
    {
        // 抽象类重写，别漏了
        base.Draw(spriteBatch, minDepth, maxDepth);
        if (!Main.BackgroundEnabled)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
        float playerBorderDisCheck = Main.screenPosition.X - Main.LocalPlayer.position.X;
        // 检测玩家是否靠近地图边界，靠近时 Main.screenPosition 将会不再移动，仅玩家移动
        bool playerCloseBorder = -951 < playerBorderDisCheck && playerBorderDisCheck < -950;

        float middleDistanceX = playerCloseBorder ?
            Main.LocalPlayer.position.X - 3102 :  // 玩家距离世界中心的水平距离，范围：-2446 到 2446 
            Main.screenPosition.X - 3102 + 950;// 玩家处在边界范围内不再进行绘制的调整


        //middleDistance 除的数越大，变化率越小
        //因此越远的地图元素，除的数要越大，以此构成视差效果
        //对于月亮和一些云彩，用原版的四柱月亮偏移画法，不使用这个字段

        //对于减去的Main.screenPosition.Y乘的数，乘的越小，变化率越小
        //越远的元素，乘的数越小

        //你问我怎么知道这些数据的？当然是一个一个格子数的啦（bushi）
        // 世界左上角位置：Vector2(656, 656) 中心点X : 3102

        Vector2 vector = new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1); // 源码写法，相当于除以2，离谱
        Vector2 vector2 = 0.01f * (new Vector2(Main.maxTilesX * 8f, (float)Main.worldSurface / 2f) - Main.screenPosition); // 玩家移动产生的微小偏移
        float scale1 = Main.screenWidth / 480f;
        float scalebase = 3f;
        //Main.NewText(middleDistanceX);

        // 随机生成流星
        if (Main.rand.NextBool(140))
        {
            shootstarsList.Add(new MapParticle(new Vector2(Main.screenWidth / 2, Main.screenHeight / 4) + Main.rand.NextVector2Circular(600, 400), Main.rand.NextFloat(2f) * 2.5f));
        }

        // 黑色填充最内层
        spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * opacity);
        //绘制最后层背景。
        spriteBatch.Draw(QA_bg, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
        // 绘制背景星星图
        spriteBatch.Draw(stars, new Vector2(230f, 20f) + vector2, null, Color.White, 0f, Vector2.Zero, scalebase, SpriteEffects.None, 0);

        // 绘制所有的流星，改变帧图
        for (int i = 0; i < shootstarsList.Count; i++)
        {
            var shooting = shootstarsList[i];

            spriteBatch.Draw(shootstarsTex[shooting.PicIndex], shooting.Position, Color.White);
            if (Main.GameUpdateCount % 6 == 0)
                shooting.PicIndex++;
        }

        /*
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        */

        // 绘制月亮
        spriteBatch.Draw(moon, new Vector2(230f, 20f) + vector2, null, Color.White, 0f, Vector2.Zero, scalebase, SpriteEffects.None, 0);
        spriteBatch.Draw(moonglow, new Vector2(230f, 20f) + vector2, null, Color.White, 0f, Vector2.Zero, scalebase, SpriteEffects.None, 0);

        //绘制四层云彩
        spriteBatch.Draw(clouds4, vector + new Vector2(0f, -680f) + vector2, null, Color.White, Main.GlobalTimeWrappedHourly / 128f, new Vector2(clouds4.Width / 2, clouds4.Height / 2), scalebase, SpriteEffects.None, 1f);
        spriteBatch.Draw(clouds3, vector + new Vector2(0f, -680f) + vector2, null, Color.White, Main.GlobalTimeWrappedHourly / 64f, new Vector2(clouds3.Width / 2, clouds3.Height / 2), scalebase, SpriteEffects.None, 1f);
        spriteBatch.Draw(clouds2, vector + new Vector2(0f, -680f) + vector2, null, Color.White, Main.GlobalTimeWrappedHourly / 64f, new Vector2(clouds2.Width / 2, clouds2.Height / 2), scalebase, SpriteEffects.None, 1f);
        //加一层滤镜
        spriteBatch.Draw(QA_bgclouds2, Vector2.Zero, null, Color.White, 0, Vector2.Zero, scale1, SpriteEffects.None, 0);
        spriteBatch.Draw(clouds1, vector + new Vector2(0f, -680f) + vector2, null, Color.White, Main.GlobalTimeWrappedHourly / 32f, new Vector2(clouds1.Width / 2, clouds1.Height / 2), scalebase, SpriteEffects.None, 1f);
        //第二层滤镜，Additive效果更好
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        spriteBatch.Draw(QA_bgclouds1, new Vector2(0f, 808 + Main.screenHeight - Main.screenPosition.Y), null, Color.White, 0, new Vector2(0, QA_bgclouds1.Height), scale1, SpriteEffects.None, 0);
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        //绘制最后一层的墙杆雕像
        spriteBatch.Draw(backFar, new Vector2(3102 + middleDistanceX / 4f, -152) - new Vector2(Main.screenPosition.X, Main.screenPosition.Y * 0.35f), null, Color.White, 0, new Vector2(backFar.Width / 2, 0), 3f, SpriteEffects.None, 0);

        // 用于绘制灯塔左右侧的那些外凸柱子(火焰后)。玩家靠近地图中间就不画，节约性能
        if (middleDistanceX < -720)
            spriteBatch.Draw(hangerTex[5], new Vector2(hangerX[5] + middleDistanceX * 6 / 21f, hangerY[5] - 24) - new Vector2(Main.screenPosition.X, Main.screenPosition.Y * (1 - 0.1f * 6)), null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.None, 0);
        if (middleDistanceX > 720)
            spriteBatch.Draw(hangerTex[5], new Vector2(hangerX[5] + (3102 - hangerX[5]) * 2 + middleDistanceX * 6 / 21f, hangerY[5]) - new Vector2(Main.screenPosition.X, Main.screenPosition.Y * (1 - 0.1f * 6)), null, Color.White, 0, new Vector2(hangerTex[5].Width, 0), scalebase, SpriteEffects.FlipHorizontally, 0);

        // 绘制所有的火焰，改变帧图

        for (int i = 0; i < fireList.Count; i++)
        {
            var fire = fireList[i];

            Vector2 pos = fire.Position - new Vector2(Main.screenPosition.X - middleDistanceX / 12f, 250 + Main.screenPosition.Y * 0.7f);
            // 屏幕外不绘制
            if (pos.X + 84 * fire.Scale > 0 && pos.X - 84 * fire.Scale < Main.screenWidth)
                spriteBatch.Draw(fireTex[fire.PicIndex], pos, null, Color.White, 0, new Vector2(84, 72), fire.Scale, SpriteEffects.None, 0);
            if (Main.GameUpdateCount % 6 == 0)
                fire.PicIndex++;
            if (fire.PicIndex >= 18)
            {
                fire.PicIndex = 0;
            }
        }
        // 两侧裂缝里的火焰
        // 左
        for (int i = 0; i < fireListLeft.Count; i++)
        {
            var fire = fireListLeft[i];
            // 屏幕外不绘制
            if (middleDistanceX < -620)
                spriteBatch.Draw(fireTex[fire.PicIndex], fire.Position - new Vector2(Main.screenPosition.X - middleDistanceX / 6f, 250 + Main.screenPosition.Y * 0.7f), null, Color.White, 0, new Vector2(84, 72), fire.Scale, SpriteEffects.None, 0);
            if (Main.GameUpdateCount % 6 == 0)
                fire.PicIndex++;
            if (fire.PicIndex >= 18)
            {
                fire.PicIndex = 0;
            }
        }
        // 右
        for (int i = 0; i < fireListRight.Count; i++)
        {
            var fire = fireListRight[i];
            // 屏幕外不绘制
            if (middleDistanceX > 620)
                spriteBatch.Draw(fireTex[fire.PicIndex], fire.Position - new Vector2(Main.screenPosition.X - middleDistanceX / 6f, 250 + Main.screenPosition.Y * 0.7f), null, Color.White, 0, new Vector2(84, 72), fire.Scale, SpriteEffects.None, 0);
            if (Main.GameUpdateCount % 6 == 0)
                fire.PicIndex++;
            if (fire.PicIndex >= 18)
            {
                fire.PicIndex = 0;
            }
        }

        // 绘制灯塔左右侧的那些外凸柱子（火焰前）。玩家靠近地图中间就不画，节约性能

        for (int i = 4; i > 0; i--)
        {
            int j = i + 1;
            if (middleDistanceX < -720)
                spriteBatch.Draw(hangerTex[i], new Vector2(hangerX[i] + middleDistanceX * j / 21f, hangerY[i] - 24) - new Vector2(Main.screenPosition.X, Main.screenPosition.Y * (1 - 0.1f * j)), null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.None, 0);
            if (middleDistanceX > 720)
                spriteBatch.Draw(hangerTex[i], new Vector2(hangerX[i] + (3102 - hangerX[i]) * 2 + middleDistanceX * j / 21f, hangerY[i]) - new Vector2(Main.screenPosition.X, Main.screenPosition.Y * (1 - 0.1f * j)), null, Color.White, 0, new Vector2(hangerTex[i].Width, 0), scalebase, SpriteEffects.FlipHorizontally, 0);
        }

        // 路两边
        if (middleDistanceX < -720)
            spriteBatch.Draw(sideWall, new Vector2(1356, 1506) - Main.screenPosition, null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.None, 0);
        if (middleDistanceX > 720)
            spriteBatch.Draw(sideWall, new Vector2(4830, 1506) - Main.screenPosition, null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.FlipHorizontally, 0);

        // 绘制进侧城头
        // 方块
        for (int i = 0; i < 13; i++)
        {
            // 屏幕外不绘制。
            Vector2 pos = new Vector2(1512 + 240 * i, 1310) - Main.screenPosition;
            if (pos.X > -rempart.Width * scalebase && pos.X < Main.screenWidth)
                spriteBatch.Draw(rempart, pos, null, Color.White, 0, Vector2.Zero, 2.7f, SpriteEffects.None, 0);
        }
        // 横排
        for (int i = 0; i < 22; i++)
        {
            // 屏幕外不绘制。
            Vector2 pos = new Vector2(1446 + i * rempartBase.Width * scalebase, 1400) - Main.screenPosition;
            if (pos.X > -rempartBase.Width * scalebase && pos.X < Main.screenWidth)
                spriteBatch.Draw(rempartBase, pos, null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.None, 0);
        }
        // 石墩子
        if (middleDistanceX < -630)
            spriteBatch.Draw(stone1, new Vector2(1410, 1400) - Main.screenPosition, null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.None, 0);
        if (middleDistanceX > 510)
            spriteBatch.Draw(stone1, new Vector2(4580, 1400) - Main.screenPosition, null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.FlipHorizontally, 0);
        if (middleDistanceX < -320)
            spriteBatch.Draw(stone0, new Vector2(1720, 1326) - Main.screenPosition, null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.None, 0);
        if (middleDistanceX > 200)
            spriteBatch.Draw(stone0, new Vector2(4270, 1326) - Main.screenPosition, null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.FlipHorizontally, 0);

        // 路
        for (int i = 0; i < 16; i++)
        {
            // 屏幕外不绘制。
            Vector2 pos = new Vector2(1375 + i * floorStamp.Width * scalebase, 1504) - Main.screenPosition;
            // 216 是 floorStamp.Width * scalebase，后续判断语句同理，贴图宽×基础放大系数
            if (pos.X > -216 && pos.X < Main.screenWidth)
                spriteBatch.Draw(floorStamp, pos, null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.None, 0);
        }

        //Main.NewText(Main.GameUpdateCount % 84 / 6);
        // 窗花
        for (int i = 0; i < 11; i++)
        {
            // 在中心位置画，方便火苗的绘画
            Vector2 pos = new Vector2(1521 + i * 316, 1640) - Main.screenPosition;
            if (pos.X > -165 && pos.X < Main.screenWidth + 165)
            {
                spriteBatch.Draw(window, pos, null, Color.White, 0, new Vector2(window.Width / 2, 0), scalebase, SpriteEffects.None, 0);
                spriteBatch.Draw(torchFireTex[Main.GameUpdateCount % 28 / 2], pos + new Vector2(3, -94), null, Color.White, 0, new Vector2(50, 0), scalebase, SpriteEffects.None, 0);
            }
        }
        // 窗上悬挂笼
        // 基座
        for (int i = 0; i < 8; i++)
        {
            // 屏幕外不绘制。
            Vector2 pos = new Vector2(1410 + i * 438, 1564) - Main.screenPosition;
            if (pos.X > -84 && pos.X < Main.screenWidth)
            {
                spriteBatch.Draw(frontBase, pos, null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.None, 0);

                // 绘制柱子和挂的灯笼，不用for循环了，不想用。
                // 离谱的是，窗户上面那一圈柱子还不是根据世界中心坐标来的
                // 而是根据这个，屏幕中心位置。
                // 其他同理。
                float screenMiddleDistance = pos.X - Main.screenWidth / 2;
                spriteBatch.Draw(frontMiddle, pos + new Vector2(2 + screenMiddleDistance / 45f, 28 - Main.screenPosition.Y * 0.03f), null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.None, 0);
                spriteBatch.Draw(frontMiddle, pos + new Vector2(2 + screenMiddleDistance / 22.5f, 56 - Main.screenPosition.Y * 0.06f), null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.None, 0);
                spriteBatch.Draw(frontMiddle, pos + new Vector2(2 + screenMiddleDistance / 15f, 84 - Main.screenPosition.Y * 0.09f), null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.None, 0);

                spriteBatch.Draw(fronthangerTex[i % 3], pos + new Vector2(2 + screenMiddleDistance / 11.25f, 112 - Main.screenPosition.Y * 0.12f), null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.None, 0);
            }
        }

        shootstarsList.RemoveAll((MapParticle s) => s.PicIndex > shootstarsTex.Length - 1);
    }
    public override void Deactivate(params object[] args)
    {
        base.Deactivate(args);
        shootstarsList.Clear();
    }

    public class MapParticle
    {
        public MapParticle(Vector2 startingPosition, float scale = 1f, int picindex = 0)
        {
            Position = startingPosition;
            Scale = scale;
            PicIndex = picindex;
        }

        public int PicIndex;

        public float Scale;

        public Vector2 Position;

    }
}
