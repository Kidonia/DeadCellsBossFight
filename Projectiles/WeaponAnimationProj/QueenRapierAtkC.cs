using Terraria.Audio;
using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;
using DeadCellsBossFight.Core;
using Terraria.DataStructures;

namespace DeadCellsBossFight.Projectiles.WeaponAnimationProj;

public class QueenRapierAtkC : DC_WeaponAnimation
{
    public override string AnimName => "AtkQueenRapierC";
    public override int HitFrame => 9;

    private Dictionary<int, DCAnimPic> WeaponDic = new();
    public override int TotalFrame => WeaponDic.Count;
    public override void SetDefaults()
    {
        QuickSetDefault(132, 28, 10, DamageClass.Default, 1.4f, slowBeginFrame: 6, slowEndFrame: 4);
    }
    public override void OnSpawn(IEntitySource source)
    {
        SpawnSourceCheck(source, ref WeaponDic);
    }
    public override void AI()
    {
        base.AI();
        DrawTheAnimationInAI(50f, 0);
        PlayWeaponSound(AssetsLoader.weapon_queensw_release1, 5);
        CameraBump(2.4f, 1f, 19);
        if (Projectile.frame == HitFrame)
        {
            Projectile.NewProjectile(spawner.GetSource_FromAI(), npc.Center + new Vector2(Projectile.velocity.X * 160, 0), Vector2.Zero, ModContent.ProjectileType<QueenRapierCut>(), 0, 0, player.whoAmI, 0);
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