using DeadCellsBossFight.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;
using System.Collections.Generic;
using DeadCellsBossFight.Utils;
using System;

namespace DeadCellsBossFight.Projectiles;

// ai[0]控制是哪个NPC的（女王还是细胞人）0细胞人，1女王背身，2女王正脸；ai[1]控制绘制偏移，ai[2]控制绘制与否，0不绘制，1绘制
public class DCHeadFire : ModProjectile
{
    public HeadBlackSmoke[] BlackSmoke = new HeadBlackSmoke[200];
    public override string Texture => AssetsLoader.TransparentImg;
    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = 2;
        Projectile.tileCollide = false;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 1;
        Projectile.penetrate = -1;
    }
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
    }
    public override void OnSpawn(IEntitySource source)
    {
        for(int i = 0; i < BlackSmoke.Length; i++)
        {
            BlackSmoke[i] = new HeadBlackSmoke();
        }
        base.OnSpawn(source);
    }
    public override void AI()
    {
        Projectile.timeLeft = 2;
        //Projectile.position = Main.MouseWorld;
        if (Projectile.ai[2] == 0)
        {
            return;
        }
        UpdateBlackFog();

        
        NewBlackFog(Projectile.position +Main.rand.NextVector2Circular(3, 1), Main.rand.NextFloat(2f, 2.5f), new Color(0, 0, 0, 200));
        NewBlackFog(Projectile.position + Main.rand.NextVector2Circular(3, 1), Main.rand.NextFloat(2.1f, 2.5f), new Color(0, 0, 0, 180));
        


        /*
        for (int l = 0; l < 2; l++)
        {
            int num54 = Dust.NewDust(new Vector2(Projectile.position.X + 3f, Projectile.position.Y + 4f), 14, 14, DustID.UltraBrightTorch);
            Main.dust[num54].velocity *= 0.2f;
            Main.dust[num54].noGravity = true;
            Main.dust[num54].scale = 1.25f;
        }
        */
    }
    public override bool PreDraw(ref Color lightColor)
    {
        // Main.NewText(Projectile.ai[2]);
        if (Projectile.ai[2] == 0)
        {
            return false;
        }

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        Main.spriteBatch.Draw(AssetsLoader.BlackSmoke, Projectile.position - Main.screenPosition, Color.Black);
        DrawBlackFog();

        // 细胞人的头图片放在它对应的弹幕里面绘制
        if (Projectile.ai[0] == 1)
            Main.spriteBatch.Draw(AssetsLoader.QueenHead_Back, Projectile.position + new Vector2(Projectile.ai[1] * Projectile.direction, 0) - Main.screenPosition, null, Color.White, 0, new Vector2(7.5f + Projectile.direction, 9), 2.4f, (SpriteEffects)((Projectile.direction > 0 ? 0 : 1)), 0);
        else if (Projectile.ai[0] == 2)
            Main.spriteBatch.Draw(AssetsLoader.QueenHead, Projectile.position + new Vector2(Projectile.ai[1] * Projectile.direction, 0) - Main.screenPosition, null, Color.White, 0, new Vector2(7.5f + Projectile.direction, 9), 2.4f, (SpriteEffects)((Projectile.direction > 0 ? 0 : 1)), 0);
        




        List<CustomVertexInfo> vertices = new();
        for (int i = 1; i < Projectile.oldPos.Length; ++i)
        {
            if (Projectile.oldPos[i] == Vector2.Zero) break;
            int width = 35;
            var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
            normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
            var factor = i / (float)Projectile.oldPos.Length;
            var color = Color.Lerp(Color.Black, Color.Black, factor);
            var w = MathHelper.Lerp(1f, 0.04f, factor);
            vertices.Add(new CustomVertexInfo(Projectile.oldPos[i] - Main.screenPosition + normalDir * width, color, new Vector3((float)Math.Sqrt(factor), 1, w)));
            vertices.Add(new CustomVertexInfo(Projectile.oldPos[i] - Main.screenPosition + normalDir * -width, color, new Vector3((float)Math.Sqrt(factor), 0, w)));
        }

        if (vertices.Count > 3)
        {
            Main.graphics.GraphicsDevice.Textures[0] = AssetsLoader.BlackSmokeTrail;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

        }

        return false;
    }
    public override void PostDraw(Color lightColor)
    {
        base.PostDraw(lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        Projectile.ai[2] = 0;
        foreach(var smoke in BlackSmoke)
        {
            smoke.active = false;
        }
    }

    public int NewBlackFog(Vector2 position, float scale, Color color)
    {
        for (int i = 0; i < 200; i++)
        {
            HeadBlackSmoke smoke = BlackSmoke[i];
            if (smoke.active)
                continue;
            else
            {
                smoke.position = position;
                smoke.scale = scale;
                smoke.color = color;
                smoke.active = true;

                // Main.NewText(i);
                return i;
            }
        }
        Main.NewText("黑雾数量过多，已越界！");
        return -1;
    }
    public void UpdateBlackFog()
    {
        for (int i = 0; i <200;  i++)
        {
            HeadBlackSmoke smoke = BlackSmoke[i];
            if (!smoke.active)
                continue;
            else
            {
                smoke.scale -= 0.06f;
                if(smoke.scale <0.3f)
                    smoke.active = false;
            }
        }
    }
    public void DrawBlackFog()
    {
        for (int i = 0; i < 200; i++)
        {
            HeadBlackSmoke smoke = BlackSmoke[i];
            if (!smoke.active)
                continue;
            else
            {
                
                Main.spriteBatch.Draw(AssetsLoader.BlackSmoke, smoke.position - Main.screenPosition, null, smoke.color, 0, new Vector2(10, 10), smoke.scale, SpriteEffects.None, 0);
            }
        }
    }
}
public class HeadBlackSmoke
{
    public bool active;
    public Vector2 position;
    public float scale;
    public Color color;

}
