using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.ModLoader;
using DeadCellsBossFight.Utils;
using DeadCellsBossFight.Core;
using Terraria.DataStructures;

namespace DeadCellsBossFight.Projectiles.WeaponAnimationProj;

public class PerfectHalberdA : DC_WeaponAnimation
{
    public override string AnimName => "perfectHalberdAtkA";
    public override string fxName => "fxPerfectHalberdAtkA";
    public override int HitFrame => 13;
    public override int fxStartFrame => 11;

    private Dictionary<int, DCAnimPic> WeaponDic = new();
    private Dictionary<int, DCAnimPic> fxDic = new();
    public override int TotalFrame => WeaponDic.Count;
    public override int fxFrames => fxDic.Count;
    public override void SetDefaults()
    {
        fxDic = AssetsLoader.fxAtlas[fxName];
        QuickSetDefault(108, 128, 16, DamageClass.Default, 1.4f, slowBeginFrame: 6, slowEndFrame: 8);
    }
    public override void OnSpawn(IEntitySource source)
    {
        SpawnSourceCheck(source, ref WeaponDic);
    }
    public override void AI()
    {
        base.AI();
        DrawTheAnimationInAI(28.4f, -21.8f);
        PlayChargeSound(AssetsLoader.weapon_broadsword_charge3);
        PlayWeaponSound(AssetsLoader.weapon_perfectsw_release1, 8);
        CameraBump(2.4f, 1f, 19);//屏幕震动
    }
    public override void PostDraw(Color lightColor)
    {
        DrawWeaponTexture(WeaponDic, 8, -25, true, new(255, 255, 255), true);
        DrawfxTexture(fxDic, 8, -25);
    }
    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
    {
        if (playerHurt.PerfectHalberdHurtTime == 0)//可暴击
        {
            modifiers.SetCrit(0.85f);
            ThisATKShouldCritSound();
        }
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        CheckAndPlayCritSound(info);
        SoundEngine.PlaySound(AssetsLoader.hit_broadsword);
    }
}