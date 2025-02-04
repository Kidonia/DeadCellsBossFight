using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;

namespace DeadCellsBossFight.Projectiles;

public class RedLightning : ModProjectile
{
    public Vector2 SpawnCenter;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.width = 52;
        Projectile.height = 34;
        Projectile.damage = 0;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 2;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;

    }



    public override bool PreDraw(ref Color lightColor)
    {
        lightColor = new Color(Main.rand.Next(51, 75), Main.rand.Next(42, 55), 31, 0);
        return true;
    }
    public override void OnSpawn(IEntitySource source)
    {
        /*这些写在了Bleed效果里面，避免了npc.Center的使用（恼）
         
            Projectile.rotation = Projectile.Center.ToRotation() + (float)Math.PI / 2f;

            rotation = Projectile.Center.ToRotation();
            if (rotation < 0)
            {
                rotation += MathHelper.TwoPi;
            }
            if (Projectile.ai[0] > 5f)//npc射出
            {
                Projectile.timeLeft += (int)Projectile.ai[0];
                Projectile.Center = npc.Center + Terraria.Utils.RotatedBy(new Vector2(Projectile.ai[0] + Main.rand.NextFloat(-10f, 3f), 0f), rotation);
            }
        */
        Projectile.rotation = Projectile.ai[0];
        Projectile.timeLeft += Main.rand.Next(160, 205);
        Projectile.scale *= Main.rand.NextFloat(0.9f, 1.1f);
        
    }

    public override void AI()
    {
        Projectile.scale *= 0.97f;
    }

}

