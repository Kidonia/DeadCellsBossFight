using DeadCellsBossFight.Core; // => AssetsLoader
using DeadCellsBossFight.Utils; // => CustomVertexInfo
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Projectiles.EffectProj;

public class Linetry : ModProjectile
{
    // 直接拿半圆改的，你可以Dragonlens直接生成在不同位置看看效果
    // ai[0] 控制颜色，0为白色马上消散，其他为黑暗残破的，生成时传入
    // ai[1] 控制半短轴
    // ai[2] 控制椭圆整体旋转角度
    // localAI[0] 控制旋转速度 => 0
    // localAI[1] 控制半长轴
    private Color drawColor;
    private float distance;
    private int timeSinceSpawn = 0;
    public override string Texture => AssetsLoader.TransparentImg; // 1x1纯透明贴图
    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 120;
        drawColor = Color.White;

        base.SetDefaults();
    }
    public override void OnSpawn(IEntitySource source)
    {
        distance = (Main.LocalPlayer.Center - Projectile.Center).Length();
        Projectile.ai[2] = (Main.LocalPlayer.Center - Projectile.Center).ToRotation();
        Projectile.localAI[1] = 420f / distance;
        Projectile.ai[1] = 2f / distance;
        Projectile.timeLeft = 80 + (int)Projectile.ai[0] * 110 + (int)distance / 40;
        base.OnSpawn(source);
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override void AI()
    {
        timeSinceSpawn++;
        if (30 < timeSinceSpawn && timeSinceSpawn < 51)
        {
            drawColor = Color.White * ((50 - timeSinceSpawn) * 0.05f);
            if (Projectile.ai[0] > 0)
            {
                drawColor.A = 255;
            }
        }
        if(160 + (int)distance / 40 < timeSinceSpawn)
        {
            drawColor *= 0.7f;
        }
        Projectile.localAI[1] += (8f) / distance;


    }
    public override void PostDraw(Color lightColor)
    {
        // 不会顶点绘制，以下内容照抄自：顶点绘制入门及简单实例2
        // Main.spriteBatch.Draw(AssetsLoader.linetry, Projectile.Center - Main.screenPosition, Color.White);
        List<CustomVertexInfo> vertices = new List<CustomVertexInfo>();

        float a = Projectile.localAI[1];
        float b = Projectile.ai[1];
        float FlipRotation = Projectile.ai[2];

        Vector2 v1 = new Vector2(-25, -600);
        vertices.Add(new CustomVertexInfo(Projectile.Center - Main.screenPosition + new Vector2(v1.X / a, v1.Y / b).RotatedBy(FlipRotation), drawColor, new Vector3(0, 0, 1)));

        v1 = new Vector2(25, -600);
        vertices.Add(new CustomVertexInfo(Projectile.Center - Main.screenPosition + new Vector2(v1.X / a, v1.Y / b).RotatedBy(FlipRotation), drawColor, new Vector3(1, 0, 1)));

        v1 = new Vector2(-25, 600);
        vertices.Add(new CustomVertexInfo(Projectile.Center - Main.screenPosition + new Vector2(v1.X / a, v1.Y / b).RotatedBy(FlipRotation), drawColor, new Vector3(0, 1, 1)));

        v1 = new Vector2(25, 600);
        vertices.Add(new CustomVertexInfo(Projectile.Center - Main.screenPosition + new Vector2(v1.X / a, v1.Y / b).RotatedBy(FlipRotation), drawColor, new Vector3(1, 1, 1)));

        Main.graphics.GraphicsDevice.Textures[0] = AssetsLoader.linetry; // 就是那张简陋的细长贴图

        if (vertices.Count > 3)
        {
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
        }
    }
}
