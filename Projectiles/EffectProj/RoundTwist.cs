using DeadCellsBossFight.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using DeadCellsBossFight.Utils;
using Terraria.DataStructures;
namespace DeadCellsBossFight.Projectiles.EffectProj;

public class RoundTwist : ModProjectile
{
    public override string Texture => AssetsLoader.TransparentImg;
    public override void SetDefaults()
    {
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = 24;
        Projectile.tileCollide = false;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 1;
        Projectile.penetrate = -1;
    }
    public override void OnSpawn(IEntitySource source)
    {
        DeadCellsBossFight.EffectProj.Add(Projectile);
        base.OnSpawn(source);
    }

    public override void AI()
    {
        if (Projectile.timeLeft > 10)
            Projectile.ai[0] += 0.2f;
        else
        {
            Projectile.ai[0] -= 0.3f;
        }
    }
}
public class RoundTwist2 : ModProjectile
{
    public override string Texture => AssetsLoader.TransparentImg;
    public override void SetDefaults()
    {
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = 22;
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
        Projectile.ai[0] += 0.21f;
    }
}
public class RoundTwistQueen : ModProjectile
{
    public override string Texture => AssetsLoader.TransparentImg;
    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = 28;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.restrikeDelay = 27;
        base.SetDefaults();
    }
    public override void OnSpawn(IEntitySource source)
    {
        DeadCellsBossFight.EffectProj.Add(Projectile);
        base.OnSpawn(source);
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return (Projectile.Center - targetHitbox.Center.ToVector2()).Length() < 288 + (28 - Projectile.timeLeft) * 4f;
    }
    public override void AI()
    {
        Projectile.ai[0] += 0.21f;
    }
    
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.velocity = (target.Center - Projectile.Center).Normalized() * 14f;
        base.OnHitPlayer(target, info);
    }
    
}