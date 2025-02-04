using DeadCellsBossFight.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;

namespace DeadCellsBossFight.Contents.Biomes.BlackBridge;

public class BlackBridgeSky : DCBasicSky
{
    private Texture2D transparentTex;
    private Texture2D moonTexture;
    private Texture2D bgmoon;
    private Texture2D bgColorTex;
    private Texture2D bgWaterTex;
    private Texture2D waterShine;
    private Texture2D bgTop;
    private Texture2D bgCloud;
    private Texture2D bgboat1;
    private Texture2D bgboat2;
    private Texture2D bgboat3;
    private Texture2D bgbuilding1;
    private Texture2D bgbuilding2;
    private Texture2D bgbuilding3;
    private Texture2D bgbuilding4;
    private Texture2D bridgeTex;
    private Texture2D railingTex1;
    private Texture2D railingTex2;
    private Texture2D railingTex3;
    private Texture2D[] railingTex;
    private int[] railingDrawArray;
    private Texture2D lamppost;
    public override void OnLoad()
    {
        string path = "DeadCellsBossFight/Contents/Biomes/BlackBridge/BBElements/";
        transparentTex = GetTex(path + "empty");
        moonTexture = GetTex(path + "moon");
        bgmoon = GetTex(path + "bgmoon");
        bgColorTex = GetTex(path + "bridgeBg");
        bgWaterTex = GetTex(path + "bridgeWaterGradient");
        waterShine = GetTex(path + "waterShine");
        bgTop = GetTex(path + "bg1");
        bgCloud = GetTex(path + "bg2");
        bgboat1 = GetTex(path + "bgboat1");
        bgboat2 = GetTex(path + "bgboat2");
        bgboat3 = GetTex(path + "bgboat3");
        bgbuilding1 = GetTex(path + "bgbuilding1");
        bgbuilding2 = GetTex(path + "bgbuilding2")  ;
        bgbuilding3 = GetTex(path + "bgbuilding3");
        bgbuilding4 = GetTex(path + "bgbuilding4");
        bridgeTex = GetTex(path + "bridgeBase");
        lamppost = GetTex(path + "lamppost");
        railingTex1 = GetTex(path + "railing1");
        railingTex2 = GetTex(path + "railing2");
        railingTex3 = GetTex(path + "railing3");
        railingTex = new Texture2D[4] { transparentTex, railingTex1, railingTex2, railingTex3 };
        railingDrawArray = new int[16] { 0, 0, 1, 2, 2, 2, 2, 3, 0, 0, 1, 2, 2, 3, 0, 0 };

    }
    public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
    {
        base.Draw(spriteBatch, minDepth, maxDepth);


        float playerBorderDisCheck = Main.screenPosition.X - Main.LocalPlayer.position.X;
        // 检测玩家是否靠近地图边界，靠近时 Main.screenPosition 将会不再移动，仅玩家移动
        bool playerCloseBorder = -951 < playerBorderDisCheck && playerBorderDisCheck < -950;

        float middleDistanceX = playerCloseBorder ?
            Main.LocalPlayer.position.X - 2302 :  // 玩家距离世界中心的水平距离，范围：-1646 到 1646 
            Main.screenPosition.X - 2302 + 950;// 玩家处在边界范围内不再进行绘制的调整

        // 世界左上角位置：Vector2(656, 656) 中心点X: 2302

        // 黑色填充最内层
        spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * opacity);
        //绘制上黑下蓝最后层背景。
        float fadeIn = Math.Min(1f, Main.screenPosition.Y / 800f * opacity);
        spriteBatch.Draw(bgColorTex, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * fadeIn);
        //float k = ((float)Main.worldSurface / 2f - Main.screenPosition.Y);
        //Main.NewText(k);


        //绘制云彩
        float scale1 = 250 * 16 / bgTop.Width;
        spriteBatch.Draw(bgCloud, new Vector2(bgCloud.Width - middleDistanceX / 30f, Main.screenHeight * 0.55f + 0.01f * ((float)Main.worldSurface / 2f - Main.screenPosition.Y)), null, Color.White * fadeIn, 0, bgCloud.Size() / 2, scale1, SpriteEffects.None, 0);
        float scaleT = 1.2f;

        Vector2 vector = new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1); // 源码写法，相当于除以2，离谱
        Vector2 vector2 = 0.01f * (new Vector2(Main.maxTilesX * 8f, (float)Main.worldSurface / 2f) - Main.screenPosition);

        // 绘制基本月亮
        spriteBatch.Draw(bgmoon, vector + new Vector2(0f, -220f) + vector2, null, Color.White * 0.9f * opacity, 0f, new Vector2(moonTexture.Width >> 1, moonTexture.Height >> 1), 1f, SpriteEffects.None, 1f);

        Vector2 offsetX = new(18, 0);
        // 倒影
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        // 水波shader
        AssetsLoader.waterWaveEffect.Parameters["noiseTex"].SetValue(AssetsLoader.nsSmokeMask);
        AssetsLoader.waterWaveEffect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 3);
        AssetsLoader.waterWaveEffect.Parameters["Strength"].SetValue(0.05f);
        AssetsLoader.waterWaveEffect.CurrentTechnique.Passes[0].Apply();

        spriteBatch.Draw(bgboat1, offsetX + new Vector2(2302 - 252 + middleDistanceX / 2f, 360 + bgboat1.Height * scaleT) - new Vector2(Main.screenPosition.X, Main.screenPosition.Y * 0.2f), null, Color.White, 0, Vector2.Zero, scaleT, SpriteEffects.FlipVertically, 0);

        Vector2 bgbuilding4pos = new Vector2(2302 + 1500 + middleDistanceX / 2.5f, 306 + bgbuilding4.Height * scaleT) - new Vector2(Main.screenPosition.X, Main.screenPosition.Y * 0.35f);
        if (bgbuilding4pos.X - bgbuilding4.Width < Main.screenWidth)
            spriteBatch.Draw(bgbuilding4, offsetX + bgbuilding4pos, null, Color.White, 0, new Vector2(bgbuilding4.Width, 0), scaleT, SpriteEffects.FlipVertically, 0);

        spriteBatch.Draw(bgboat3, offsetX / 2 + new Vector2(2302 + 350 + middleDistanceX / 4.6f, 775 + bgboat3.Height * scaleT) - new Vector2(Main.screenPosition.X, Main.screenPosition.Y * 0.45f), null, Color.White, 0, Vector2.Zero, scaleT, SpriteEffects.FlipVertically, 0);
        spriteBatch.Draw(bgboat2, offsetX + new Vector2(2302 - 800 + middleDistanceX / 5f, 742 + bgboat2.Height * scaleT) - new Vector2(Main.screenPosition.X, Main.screenPosition.Y * 0.35f), null, Color.White, 0, Vector2.Zero, scaleT, SpriteEffects.FlipVertically, 0);

        Vector2 bgbuilding1pos = new Vector2(2302 + 1200 + middleDistanceX / 12f, 828 + bgbuilding1.Height * scaleT) - Main.screenPosition;
        if (bgbuilding1pos.X < Main.screenWidth)
            spriteBatch.Draw(bgbuilding1, offsetX + bgbuilding1pos, null, Color.White, 0, Vector2.Zero, scaleT, SpriteEffects.FlipVertically, 0);

        Vector2 bgbuilding3pos = new Vector2(860 + middleDistanceX / 9f, 542 + bgbuilding3.Height * scaleT) - new Vector2(Main.screenPosition.X, Main.screenPosition.Y * 0.85f);
        if (bgbuilding3pos.X + bgbuilding3.Width * scaleT > 0)
            spriteBatch.Draw(bgbuilding3, offsetX + bgbuilding3pos, null, Color.White, 0, Vector2.Zero, scaleT, SpriteEffects.FlipVertically, 0);

        Vector2 bgbuilding2pos = new Vector2(636, 656 + bgbuilding2.Height * scaleT) - Main.screenPosition;
        if (bgbuilding2pos.X + bgbuilding2.Width > 0)
            spriteBatch.Draw(bgbuilding2, offsetX + bgbuilding2pos, null, Color.White, 0, Vector2.Zero, scaleT, SpriteEffects.FlipVertically, 0);
        for (int i = 656; i < 4500; i += bridgeTex.Width)
        {
            Vector2 pos = new Vector2(i, 1540) - Main.screenPosition;
            if (pos.X + bridgeTex.Width > 0 && pos.X < Main.screenWidth)
                spriteBatch.Draw(bridgeTex, new Vector2(i + 10, 1540 + bridgeTex.Height) - Main.screenPosition, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipVertically, 0);
        }
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);


        //绘制水面
        spriteBatch.Draw(bgWaterTex, new Rectangle(0, (int)(vector + vector2).Y + 257, Main.screenWidth, Main.screenHeight / 2), Color.White * 0.5f);

        //绘制月亮 + 水面光效
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone); ;
        spriteBatch.Draw(waterShine, vector + new Vector2(0f, 444f) + vector2, null, new Color(255, 190, 130) * opacity, 0f, new Vector2(waterShine.Width >> 1, waterShine.Height >> 1), 1f, SpriteEffects.None, 1f);
        spriteBatch.Draw(moonTexture, vector + new Vector2(0f, -220f) + vector2, null, new Color(255, 190, 130) * opacity, 0f, new Vector2(moonTexture.Width >> 1, moonTexture.Height >> 1), 1f, SpriteEffects.None, 1f);
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

        // 绘制桥的大顶
        spriteBatch.Draw(bgTop, new Vector2(506, 506) - Main.screenPosition, null, Color.White, 0, Vector2.Zero, scale1, SpriteEffects.None, 0);

        //绘制桥顶前面的东西
        //绘制居中的船
        spriteBatch.Draw(bgboat1, new Vector2(2302 - 252 + middleDistanceX / 2f, 360) - new Vector2(Main.screenPosition.X, Main.screenPosition.Y * 0.2f), null, Color.White, 0, Vector2.Zero, scaleT, SpriteEffects.None, 0);

        //绘制右侧远建筑

        spriteBatch.Draw(bgbuilding4, bgbuilding4pos, null, Color.White, 0, bgbuilding4.Size(), scaleT, SpriteEffects.None, 0);

        //绘制两侧的船
        spriteBatch.Draw(bgboat2, new Vector2(2302 - 800 + middleDistanceX / 5f, 742) - new Vector2(Main.screenPosition.X, Main.screenPosition.Y * 0.35f), null, Color.White, 0, Vector2.Zero, scaleT, SpriteEffects.None, 0);
        spriteBatch.Draw(bgboat3, new Vector2(2302 + 350 + middleDistanceX / 4.6f, 775) - new Vector2(Main.screenPosition.X, Main.screenPosition.Y * 0.45f), null, Color.White, 0, Vector2.Zero, scaleT, SpriteEffects.None, 0);


        //绘制右侧近建筑
        if (bgbuilding1pos.X < Main.screenWidth)
            spriteBatch.Draw(bgbuilding1, bgbuilding1pos, null, Color.White, 0, new Vector2(0, bgbuilding1.Height), scaleT, SpriteEffects.None, 0);

        //绘制左侧近远建筑
        if (bgbuilding3pos.X + bgbuilding3.Width * scaleT > 0)
            spriteBatch.Draw(bgbuilding3, bgbuilding3pos, null, Color.White, 0, new Vector2(0, bgbuilding3.Height), scaleT, SpriteEffects.None, 0);

        if (bgbuilding2pos.X + bgbuilding2.Width > 0)
            spriteBatch.Draw(bgbuilding2, bgbuilding2pos, null, Color.White, 0, new Vector2(0, bgbuilding2.Height), scaleT, SpriteEffects.None, 0);

        // 绘制桥面
        for (int i = 656; i < 4500; i += bridgeTex.Width)
        {
            Vector2 pos = new Vector2(i, 1540) - Main.screenPosition;
            if (pos.X + bridgeTex.Width > 0 && pos.X < Main.screenWidth)
                spriteBatch.Draw(bridgeTex, pos, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }
        // 绘制栏杆
        for (int k = 0; k < 16; k++)
        {
            Vector2 pos = new Vector2(656 + k * railingTex1.Width, 1424) - Main.screenPosition;
            if (pos.X + 205 > 0 && pos.X < Main.screenWidth)
                spriteBatch.Draw(railingTex[railingDrawArray[k]], pos, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }
        spriteBatch.Draw(lamppost, new Vector2(1210, 1357) - Main.screenPosition, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        spriteBatch.Draw(lamppost, new Vector2(2049, 1357) - Main.screenPosition, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        spriteBatch.Draw(lamppost, new Vector2(3360, 1357) - Main.screenPosition, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        // 不画火光了，太麻烦了，奶奶个腿的

    }
}