using DeadCellsBossFight.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Projectiles.NPCsProj;

public class QueenccQuickHigh : ModProjectile
{
    public override string Texture => AssetsLoader.TransparentImg;
    public override void SetDefaults()
    {
        //Projectile.damage = 40;
        Projectile.width = 480;
        Projectile.height = 32;
        Projectile.hostile = true;
        Projectile.timeLeft = 4;
        Projectile.tileCollide = false;
    }
}

public class QueenccQuickLow : ModProjectile
{
    public override string Texture => AssetsLoader.TransparentImg;
    public override void SetDefaults()
    {
        //Projectile.damage = 40;
        Projectile.width = 480;
        Projectile.height = 64;
        Projectile.hostile = true;
        Projectile.timeLeft = 4;
        Projectile.tileCollide = false;
    }
}

public class QueenccQuickFull : ModProjectile
{
    public override string Texture => AssetsLoader.TransparentImg;
    public override void SetDefaults()
    {
        //Projectile.damage = 40;
        Projectile.width = 288;
        Projectile.height = 430;
        Projectile.hostile = true;
        Projectile.timeLeft = 4;
        Projectile.tileCollide = false;
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return base.Colliding(projHitbox, targetHitbox);
    }
}
//firewave用看守者的
public class QueenshockWave : ModProjectile
{
    public override string Texture => AssetsLoader.TransparentImg;
    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.hostile = true;
        Projectile.timeLeft = 4;
        Projectile.tileCollide = false;
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return (Projectile.Center - targetHitbox.Center.ToVector2()).Length() < 288; ;
    }
}
public class QueenccStrongComboA : ModProjectile
{
    public override string Texture => AssetsLoader.TransparentImg;
    public override void SetDefaults()
    {
        Projectile.width = 510;
        Projectile.height = 130;
        Projectile.hostile = true;
        Projectile.timeLeft = 4;
        Projectile.tileCollide = false;
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.immuneTime = 2;
        base.OnHitPlayer(target, info);
    }
}
//ComboB是前半劈砍
public class QueenccStrongOvershield : ModProjectile
{
    public override string Texture => AssetsLoader.TransparentImg;
    public override void SetDefaults()
    {
        //Projectile.damage = 40;
        Projectile.width = 72;
        Projectile.height = 540;
        Projectile.hostile = true;
        Projectile.timeLeft = 4;
        Projectile.tileCollide = false;
    }
}
public class QueendispelAOE : ModProjectile
{
    public Fire_tornado_particle[] Fire_Tornado_Particles = new Fire_tornado_particle[200];
    public override string Texture => AssetsLoader.TransparentImg;
    public override void SetDefaults()
    {
        //Projectile.damage = 40;
        Projectile.width = 96;
        Projectile.height = 1080;
        Projectile.hostile = true;
        Projectile.timeLeft = 800;////
        Projectile.tileCollide = false;
    }

    public override void OnSpawn(IEntitySource source)
    {
        for (int i = 0; i < Fire_Tornado_Particles.Length; i++)
        {
            Fire_Tornado_Particles[i] = new Fire_tornado_particle();
        }

        for (int i = 0; i < 80; i++)
        {
            //var proj = Projectile.NewProjectileDirect(source, Projectile.Bottom + new Vector2((int)(Math.Sin(Projectile.Bottom.Y - i * 0.55f) * 24), -i * 16), Vector2.Zero, ModContent.ProjectileType<testFirePtc>(), 0, 0, -1);
            //proj.timeLeft = Main.rand.Next(600, 700);

            NewFire(Projectile.Bottom + new Vector2((int)(Math.Sin(Projectile.Bottom.Y - i * 0.55f) * 24), -i * 16 - 32), Main.rand.NextFloat(2.1f, 2.6f), Main.rand.Next(600, 700), Main.rand.NextFloat(-1f, 1f));
        }
    }

    public override void AI()
    {
        if (Main.GameUpdateCount % 5 == 0)
        {
            //var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Bottom + new Vector2((int)(Math.Sin(Projectile.Bottom.Y - Main.rand.Next(0, 5)* 0.55f) * 24), 0), Vector2.Zero, ModContent.ProjectileType<testFirePtc>(), 0, 0, -1, Projectile.ai[0]);
            //proj.timeLeft = Main.rand.Next(600, 700);

            NewFire(Projectile.Bottom + new Vector2((int)(Math.Sin(Projectile.Bottom.Y - Main.rand.Next(0, 5) * 0.55f) * 24), -36), Main.rand.NextFloat(2.1f, 2.6f), Main.rand.Next(600, 700), Main.rand.NextFloat(-1f, 1f));

        }
        foreach (var fire in Fire_Tornado_Particles)
        {
            Fire_tornado_particle.UpdateFireP(fire, Projectile.velocity.X);
        }

        base.AI();
    }
    public override void PostDraw(Color lightColor)
    {
        //Vector2 pos = Projectile.position - Main.screenPosition;
        //Rectangle rectangle = new Rectangle((int)pos.X, (int)pos.Y, Projectile.width, Projectile.height);
        //Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, rectangle, new(0, 250, 0));//画碰撞箱
        DrawFire();
        base.PostDraw(lightColor);
    }

    public int NewFire(Vector2 position, float scale, int timeLeft, float rotation)
    {
        for (int i = 0; i < 200; i++)
        {
            Fire_tornado_particle fire = Fire_Tornado_Particles[i];
            if (fire.active)
                continue;
            else
            {
                fire.active = true;
                fire.position = position;
                fire.velocity = Vector2.Zero;
                fire.scale = scale;
                fire.timeLeft = timeLeft;
                fire.color = Color.White;
                fire.rotation = rotation;
                // Main.NewText(i);
                return i;
            }
        }
        Main.NewText("火焰数量过多，已越界！");
        return -1;
    }
    public void DrawFire()
    {
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        foreach (var fire in Fire_Tornado_Particles)
        {
            if (!fire.active)
                continue;
            else
                Main.spriteBatch.Draw(AssetsLoader.fxFireBullet[Main.rand.Next(0, 3)], fire.position - Main.screenPosition, null, fire.color, fire.rotation, new Vector2(23, 23), fire.scale, SpriteEffects.None, 0);//画碰撞箱
        }
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

    }
    
    public override void OnKill(int timeLeft)
    {
        foreach (var fire in Fire_Tornado_Particles)
        {
            fire.active = false;
        }
    }
}
/*
public class testFirePtc : ModProjectile
{
    public override string Texture => AssetsLoader.TransparentImg;
    public override void SetDefaults()
    {
        Projectile.damage = 0;
        Projectile.width = 48;
        Projectile.height = 48;
        Projectile.hostile = true;
        Projectile.timeLeft = 360;////
        Projectile.tileCollide = false;
    }
    public override void PostDraw(Color lightColor)
    {
        Vector2 pos = Projectile.position - Main.screenPosition;
        Rectangle rectangle = new Rectangle((int)pos.X, (int)pos.Y, Projectile.width, Projectile.height);
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, rectangle, new(210, 160, 0, 70 + 60 * Projectile.velocity.X));//画碰撞箱

        base.PostDraw(lightColor);
    }

    public override void AI()
    {
        Projectile.velocity = Vector2.Zero;
        Projectile.velocity.X += (float)Math.Sin(Projectile.timeLeft/3.5f) * 8f;
        Projectile.velocity.X += (float)Math.Cos(Projectile.position.Y/29f) * 0.4f;
        Projectile.velocity.X += 18f*Projectile.ai[0];
        Projectile.velocity.Y = -2f;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
*/
public class Fire_tornado_particle
{
    public bool active;
    public int timeLeft;
    public Vector2 position;
    public Vector2 velocity;
    public float scale;
    public Color color;
    public float rotation;

    public static void UpdateFireP(Fire_tornado_particle particle, float attach_VelocityX)
    {
        if (particle.timeLeft < 0)
        {
            particle.active = false;
            return;
        }
        else
        {
            particle.timeLeft--;
            particle.velocity = Vector2.Zero;
            particle.velocity.X += (float)Math.Sin(particle.timeLeft / 3.5f) * 8f;
            particle.velocity.X += (float)Math.Cos(particle.position.Y / 29f) * 0.4f;
            particle.velocity.X += attach_VelocityX;
            particle.velocity.Y = -2f;

            particle.color = new(0.9f, 0.6f, 0.1f, 0.5f - (float)Math.Sin(particle.position.Y / 29f) / 8f);
            particle.position += particle.velocity;
        }
    }
}
//removeRoot在RoundTwist.cs里面，为RoundTwistQueen
public class QueenlungeAttack : ModProjectile
{
    public override string Texture => AssetsLoader.TransparentImg;
    public override void SetDefaults()
    {
        //Projectile.damage = 40;
        Projectile.width = 420;
        Projectile.height = 256;
        Projectile.hostile = true;
        Projectile.timeLeft = 60;
        Projectile.tileCollide = false;
        Projectile.restrikeDelay = 59;
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.velocity = new Vector2(Projectile.ai[0], -4f);
        Projectile.timeLeft = 2;////偷懒写法
    }
}
public class QueenstompAnswer : ModProjectile
{
    public override string Texture => AssetsLoader.TransparentImg;
    public override void SetDefaults()
    {
        //Projectile.damage = 40;
        Projectile.width = 460;
        Projectile.height = 212;
        Projectile.hostile = true;
        Projectile.timeLeft = 6;
        Projectile.tileCollide = false;
        Projectile.restrikeDelay = 5;
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        Projectile.timeLeft = 2;////偷懒
    }
}