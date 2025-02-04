using DeadCellsBossFight.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Projectiles.EffectProj;

public class TeleportWhiteScreen : ModProjectile
{
    public override string Texture => AssetsLoader.TransparentImg;
    public override void SetDefaults()
    {
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = 32;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        base.SetDefaults();
    }
    public override void OnSpawn(IEntitySource source)
    {
        DeadCellsBossFight.EffectProj.Add(Projectile);
        base.OnSpawn(source);
    }
    public override void AI()
    {
        Projectile.position = Main.screenPosition;

        if (Projectile.timeLeft > 22)
            Projectile.ai[0] += 25;
        if (Projectile.timeLeft < 12)
            Projectile.ai[0] -= 20;
    }

}
