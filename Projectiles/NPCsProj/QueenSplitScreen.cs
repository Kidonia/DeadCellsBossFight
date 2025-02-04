using DeadCellsBossFight.Core;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using DeadCellsBossFight.Contents.GlobalChanges;

namespace DeadCellsBossFight.Projectiles.NPCsProj;
public class QueenSplitScreen : ModProjectile
{
    public override string Texture => AssetsLoader.TransparentImg;
    public Player player => Main.player[Projectile.owner];
    public DCPlayer playerHurt => player.GetModPlayer<DCPlayer>();

    private Dictionary<int, DCAnimPic> fxDic = new();

    private int fxFrame = 0;

    public override void SetDefaults()
    {
        fxDic = AssetsLoader.QNfxAtlas["fxMassiveCut"];
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.timeLeft = 40;
        Projectile.tileCollide = false;
        Projectile.hostile = true;
        Projectile.penetrate = -1;

    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override void OnSpawn(IEntitySource source)
    {
        // ai[0]用于控制屏幕偏移变化量，ai[1]用于控制旋转角度，ai[2]用于控制当前是否为最后一段
        /*
        if (Projectile.ai[2] > 0)
            Projectile.NewProjectile(source, Projectile.Center + Main.rand.NextVector2Circular(400, 300), Vector2.Zero, Projectile.type, Projectile.damage, 2, -1, 0, MathHelper.ToRadians(Main.rand.NextFloat(-90, 90)), Projectile.ai[2] - 1);
        */
        //Main.NewText(Projectile.ai[2]);
        Projectile.rotation = Projectile.ai[1];
        Projectile.scale = 1.8f;
        DeadCellsBossFight.EffectProj.Add(Projectile);
    }
    public override void AI()
    {
        if (fxFrame < fxDic.Count - 1)
            fxFrame++;

        if (Projectile.timeLeft > 36)
            Projectile.ai[0] += 0.25f;
        if (Projectile.timeLeft < 16)
            Projectile.ai[0] -= 0.067f;
        //Projectile.ai[0] = 20;

    }
    public override void OnKill(int timeLeft)
    {
        // 最后一段
        if (Projectile.ai[2] == 1)
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.type == ModContent.ProjectileType<QueenSplitBlueLine>())
                    proj.timeLeft = 1;
                else if (proj.type == ModContent.ProjectileType<QueenSplitScreen>() && proj.ai[2] != 1)
                    proj.timeLeft = 1;
                /*
                else if (proj.sentry && (bool)Colliding(Projectile.Hitbox, proj.Hitbox))
                    proj.Kill();
                */
            }
        }
        base.OnKill(timeLeft);
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        int length = (int)(Math.Sqrt(Main.screenWidth * Main.screenWidth + Main.screenHeight * Main.screenHeight) * 4f);
        Vector2 lineStart = Projectile.Center + (Projectile.Left - Projectile.Center).RotatedBy(Projectile.rotation) * length / 2;
        Vector2 lineEnd = Projectile.Center + (Projectile.Right - Projectile.Center).RotatedBy(Projectile.rotation) * length / 2;

        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), lineStart, lineEnd) && Projectile.timeLeft > 36;
    }
    public override void PostDraw(Color lightColor)
    {

        SpriteBatch sb = Main.spriteBatch;
        DrawfxEffect(fxDic, sb);

    }
    public void DrawfxEffect(Dictionary<int, DCAnimPic> fxDic, SpriteBatch sb)
    {

        Rectangle rectangle = new(fxDic[fxFrame].x, fxDic[fxFrame].y,
                                                    fxDic[fxFrame].width, fxDic[fxFrame].height);
        Vector2 vector = new Vector2(fxDic[fxFrame].originalWidth / 2 //参考解包图片如果在大图里是如何绘制的
                                                        - fxDic[fxFrame].offsetX,

                                                         fxDic[fxFrame].originalHeight / 2
                                                         - fxDic[fxFrame].offsetY);


        sb.End();
        sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

        sb.Draw(AssetsLoader.QNfxQueen,
                Projectile.Center - Main.screenPosition,
                rectangle,
                Color.White,
                Projectile.rotation,
                vector,
                Projectile.scale,
                0,
                0f);

        sb.End();
        sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

    }
}
