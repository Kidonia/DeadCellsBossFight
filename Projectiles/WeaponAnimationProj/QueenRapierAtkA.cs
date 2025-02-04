using Terraria.Audio;
using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;
using DeadCellsBossFight.Core;
using Terraria.DataStructures;

namespace DeadCellsBossFight.Projectiles.WeaponAnimationProj;

public class QueenRapierAtkA : DC_WeaponAnimation
{
    public override string AnimName => "AtkQueenRapierA";
    public override int HitFrame => 8;

    private Dictionary<int, DCAnimPic> WeaponDic = new();
    public override int TotalFrame => WeaponDic.Count;
    public override void SetDefaults()
    {
        QuickSetDefault(118, 56, 10, DamageClass.Default, 1.4f, slowBeginFrame : 5, slowEndFrame : 5);
    }
    public override void OnSpawn(IEntitySource source)
    {
        SpawnSourceCheck(source, ref WeaponDic);
    }
    public override void AI()
    {
        base.AI();
        DrawTheAnimationInAI(40f, 8f);
        PlayWeaponSound(AssetsLoader.weapon_queensw_release1, 5);
        CameraBump(2.4f, 1f, 19);
        if (Projectile.frame == HitFrame )
        {  
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), npc.Center + new Vector2(Projectile.velocity.X * 130, 40), Projectile.velocity, ModContent.ProjectileType<QueenRapierCut>(), 0, 0, player.whoAmI, MathHelper.ToRadians(Projectile.direction * 22.5f));  
        }
    }
    public override void PostDraw(Color lightColor)
    {
        DrawWeaponTexture(WeaponDic, 8, -29, true, new(161, 71, 0), true);
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        SoundEngine.PlaySound(AssetsLoader.hit_blade);
    }
}