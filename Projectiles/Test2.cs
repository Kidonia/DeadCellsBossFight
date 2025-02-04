using DeadCellsBossFight.Contents.Biomes.QueenArena;
using DeadCellsBossFight.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace DeadCellsBossFight.Projectiles;

public class Test2 : ModProjectile
{
    public float myScale = 1;
    public override string Texture => AssetsLoader.TransparentImg; // 1x1纯透明贴图
    public override void SetDefaults()
    {
        for (int i = 0; i < 14; i++)
            torchFireTex[i] = AssetsLoader.GetTex("DeadCellsBossFight/Contents/Biomes/QueenArena/QAElements/TorchFireTex/" + i.ToString());
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 360;
        Projectile.hide = true;
        float scaleX = Main.screenWidth / AssetsLoader.ENDBG.Size().X;
        float scaleY = Main.screenHeight / AssetsLoader.ENDBG.Size().Y;
        myScale = Math.Max(scaleX, scaleY);
        base.SetDefaults();
    }
    public override void AI()
    {
        base.AI();
    }
    private Texture2D[] torchFireTex = new Texture2D[14]; // 懒，end场景也用试试
    public override void Load()
    {
        for (int i = 0; i < 14; i++)
            torchFireTex[i] = AssetsLoader.GetTex("DeadCellsBossFight/Contents/Biomes/QueenArena/QAElements/TorchFireTex/" + i.ToString());
        base.Load();
    }
    public override void PostDraw(Color lightColor)
    {
        Vector2 screenTop = new Vector2(Main.screenWidth / 2, 0);
        Main.spriteBatch.Draw(AssetsLoader.ENDBG, screenTop, null, Color.White, 0, new Vector2(AssetsLoader.ENDBG.Size().X / 2, 0), myScale, SpriteEffects.None, 0);
        Vector2 BlowCentre = new Vector2(72, 31);

        Main.spriteBatch.Draw(AssetsLoader.ENDWINDBLOW, screenTop + BlowCentre * myScale, null, Color.White, Main.GlobalTimeWrappedHourly * 0.7f, new Vector2(291, 286), myScale, SpriteEffects.None, 0);

        Vector2 torchPos = screenTop + new Vector2(-315, 413) * myScale;
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        Main.spriteBatch.Draw(torchFireTex[Main.GameUpdateCount % 28 / 2], torchPos, null, new Color(220, 160, 140), 0, new Vector2(49, 63), myScale * 2, SpriteEffects.None, 0);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

        // Main.spriteBatch.Draw(AssetsLoader.BHHead, new Vector2(Main.screenWidth / 2, 0) + BlowCentre * myScale, Color.White);
    }
    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        behindNPCs.Add(index);
        base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
    }
}
