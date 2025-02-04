using DeadCellsBossFight.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Projectiles.EffectProj;

public class TeleportScreenScale : ModProjectile
{
    public override string Texture => AssetsLoader.TransparentImg;
    public override void SetDefaults()
    {
        Projectile.width = 50;
        Projectile.height = 50;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = 122;
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

        if (Projectile.timeLeft > 100)
        { }
        else if (Projectile.timeLeft > 60)
            Projectile.ai[0] = MathHelper.Lerp(1f, 1.18f, 0.02f * (100 - Projectile.timeLeft));
        else if (Projectile.timeLeft > 28)
            Projectile.ai[0] += 0.022f;
        else if (Projectile.timeLeft > 12)
        {
            Projectile.ai[0] = MathHelper.Lerp(2f, 1f, (28 - Projectile.timeLeft) / 15f);
            Projectile.ai[1] = MathHelper.Lerp(0.005f, 0f, (28 - Projectile.timeLeft) / 16f);
        }

        if(Projectile.timeLeft >28)
            Projectile.ai[1] = MathHelper.Lerp(0f, 0.005f, 0.0106f * (122 - Projectile.timeLeft));

    }
    public override void OnKill(int timeLeft)
    {
        // 如果喝完药头晕之后
        if (Projectile.localAI[0] == 1)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Main.screenPosition, Vector2.Zero, ModContent.ProjectileType<DCScreenTerrifySentence>(), 0, 0);
        }
        base.OnKill(timeLeft);
    }
}
