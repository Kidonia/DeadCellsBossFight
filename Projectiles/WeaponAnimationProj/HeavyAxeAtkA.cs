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

public class HeavyAxeAtkA : DC_WeaponAnimation
{
    public override string AnimName => "AtkOvenAxeA";
    public override int HitFrame => 18;

    private Dictionary<int, DCAnimPic> WeaponDic = new();
    public override int TotalFrame => WeaponDic.Count;
    public override void SetDefaults()
    {
        QuickSetDefault(118, 58, 16, DamageClass.Default, 1.4f, slowBeginFrame: 15);
    }
    public override void OnSpawn(IEntitySource source)
    {
        SpawnSourceCheck(source, ref WeaponDic);
    }
    public override void AI()
    {
        base.AI();
        DrawTheAnimationInAI(55f, -12.8f);
        PlayChargeSound(AssetsLoader.weapon_axe_charge1);
        PlayWeaponSound(AssetsLoader.weapon_axe_release1, 16);
        CameraBump(2.4f, 1f, 19);//屏幕震动
        Bump(2.1f);
        if (Projectile.frame == TotalFrame - 17 && npc != null)
        {
            SoundEngine.PlaySound(AssetsLoader.unstable_platform_break);
            for (int i = 0; i < 24; i++)
                Dust.NewDustDirect((Projectile.direction > 0 ? Projectile.BottomLeft + new Vector2(-300, -80) : Projectile.BottomRight + new Vector2(190, -80)), 100, 45, DustID.Dirt,
                    Projectile.direction * Main.rand.NextFloat(-3.8f, -1.6f), Main.rand.NextFloat(-0.5f, -1.1f), Scale : Main.rand.NextFloat(1.1f, 1.5f));

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
