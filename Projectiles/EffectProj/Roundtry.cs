using DeadCellsBossFight.Core; // => AssetsLoader
using DeadCellsBossFight.Utils; // => CustomVertexInfo，抄的，你需要我再发给你
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Projectiles.EffectProj;

public class Roundtry : ModProjectile
{
    // ai[0] 控制材质，0为完整，其他为黑暗残破的，生成时传入
    // ai[1] 控制半短轴
    // ai[2] 控制椭圆整体旋转角度
    // localAI[0] 控制旋转速度
    // localAI[1] 控制半长轴

    private Color drawColor;
    public override string Texture => AssetsLoader.TransparentImg; // 1x1纯透明贴图
    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.aiStyle = -1;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.timeLeft = Main.rand.Next(8, 20);
        drawColor = Color.White * (-1 * Main.rand.NextFloat(-1.0f, -0.5f));
        base.SetDefaults();
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override void AI()
    {
        Projectile.localAI[0] = Main.GlobalTimeWrappedHourly * 16;
        Projectile.localAI[1] = 120f / (int)(Main.LocalPlayer.Center - Projectile.Center).Length();
        Projectile.ai[1] = 80f / (int)(Main.LocalPlayer.Center - Projectile.Center).Length();
        Projectile.ai[2] = (Main.LocalPlayer.Center - Projectile.Center).ToRotation();

        base.AI();
    }
    public override void PostDraw(Color lightColor)
    {
        // 不会顶点绘制，以下内容照抄自：顶点绘制入门及简单实例2
        // Main.spriteBatch.Draw(AssetsLoader.roundtry, Projectile.Center - Main.screenPosition, Color.White);
        List<CustomVertexInfo> vertices = new List<CustomVertexInfo>();

        float a = Projectile.localAI[1];
        float b = Projectile.ai[1];
        float FlipRotation = Projectile.ai[2];

        Vector2 v1 = new Vector2(-300, -300).RotatedBy(Projectile.localAI[0]);
        vertices.Add(new CustomVertexInfo(Projectile.Center - Main.screenPosition + new Vector2(v1.X / a, v1.Y / b).RotatedBy(FlipRotation), drawColor, new Vector3(0, 0, 1)));

        v1 = new Vector2(300, -300).RotatedBy(Projectile.localAI[0]);
        vertices.Add(new CustomVertexInfo(Projectile.Center - Main.screenPosition + new Vector2(v1.X / a, v1.Y / b).RotatedBy(FlipRotation), drawColor, new Vector3(1, 0, 1)));

        v1 = new Vector2(-300, 300).RotatedBy(Projectile.localAI[0]);
        vertices.Add(new CustomVertexInfo(Projectile.Center - Main.screenPosition + new Vector2(v1.X / a, v1.Y / b).RotatedBy(FlipRotation), drawColor, new Vector3(0, 1, 1)));

        v1 = new Vector2(300, 300).RotatedBy(Projectile.localAI[0]);
        vertices.Add(new CustomVertexInfo(Projectile.Center - Main.screenPosition + new Vector2(v1.X / a, v1.Y / b).RotatedBy(FlipRotation), drawColor, new Vector3(1, 1, 1)));

        Main.graphics.GraphicsDevice.Textures[0] = Projectile.ai[0] == 0 ? AssetsLoader.roundtry : AssetsLoader.roundtry2; // 半圆形贴图

        if (vertices.Count > 3)
        {
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
        }
    }
}
