using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using DeadCellsBossFight.Utils;
using DeadCellsBossFight.Core;
using Terraria.DataStructures;

namespace DeadCellsBossFight.Projectiles.WeaponAnimationProj;

public class HeavyAxeAtkC : DC_WeaponAnimation
{
    public override string AnimName => "AtkOvenAxeC";
    public override int HitFrame => 29;

    private Dictionary<int, DCAnimPic> WeaponDic = new();
    public override int TotalFrame => WeaponDic.Count;
    public override void SetDefaults()
    {
        QuickSetDefault(208, 58, 16, DamageClass.Default, 1.4f, slowBeginFrame: 14, slowEndFrame: 21);
    }
    public override void OnSpawn(IEntitySource source)
    {
        SpawnSourceCheck(source, ref WeaponDic);
    }
    public override void AI()
    {
        base.AI();
        DrawTheAnimationInAI(0, -2.8f);
        PlayChargeSound(AssetsLoader.weapon_axe_charge3);
        PlayWeaponSound(AssetsLoader.weapon_axe_release3, 30);
        CameraBump(2.4f, 1f, 19);//屏幕震动
        Bump(4.5f);
        if (Projectile.frame == 34 && npc != null)
        {
            SoundEngine.PlaySound(AssetsLoader.unstable_platform_break);
            for (int i = 0; i < 18; i++)
                Dust.NewDustDirect(npc.Bottom + new Vector2(Projectile.velocity.X *  -170, -40), 100, 50, DustID.Dirt,
                    Projectile.velocity.X * Main.rand.NextFloat(-2.2f, -1.6f), Main.rand.NextFloat(-1.1f, -0.5f), Scale: Main.rand.NextFloat(1.2f, 1.5f));

            NormalUtils.ProjKillTree(Projectile);
        }
    }
    public override void PostDraw(Color lightColor)
    {
        DrawWeaponTexture(WeaponDic, 2, -28, true, new(80, 51, 162), true);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        SoundEngine.PlaySound(AssetsLoader.weapon_axe_hit);
    }
}