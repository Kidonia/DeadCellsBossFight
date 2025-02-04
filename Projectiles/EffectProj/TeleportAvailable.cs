using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using DeadCellsBossFight.Core;

namespace DeadCellsBossFight.Projectiles.EffectProj;

public class TeleportAvailable : ModProjectile
{
    public Vector2 pos = new Vector2();
    public override string Texture => AssetsLoader.TransparentImg;

    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = 48;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
    }
    public override void OnSpawn(IEntitySource source)
    {
         Projectile.velocity = Vector2.Zero;
        SoundEngine.PlaySound(AssetsLoader.portal_full, Projectile.position);

    }
    public override void AI()
    {
        Projectile.velocity = Vector2.Zero;

        if (Projectile.timeLeft > 20)
        {
            int distance = Main.rand.Next(32, 80);
            float mul = Main.rand.NextFloat(0.75f, 0.9f);
            Vector2 offsetRadians = Vector2.UnitX.RotateRandom(MathHelper.TwoPi);

            int type = Dust.NewDust(Projectile.position + offsetRadians * distance,
                1, 1, DustID.Pixie, 0, 0, 0, default, mul);

            Main.dust[type].velocity = new Vector2(-offsetRadians.X, -offsetRadians.Y) * mul;
        }

    }

    public override void OnKill(int timeLeft)
    {
        for (float r = 0f; r < MathHelper.TwoPi; r += MathHelper.TwoPi / 28f)
        {
            Vector2 vector = Vector2.UnitX.RotatedBy(r);
            int type = Dust.NewDust(Projectile.position + new Vector2(-2, -8) + vector * 20, 1, 1, DustID.OrangeTorch, 0, 0, 0, default, 1.2f);
            Main.dust[type].noGravity = true;
            Main.dust[type].velocity = vector * 1.2f;
        };
    }
}
