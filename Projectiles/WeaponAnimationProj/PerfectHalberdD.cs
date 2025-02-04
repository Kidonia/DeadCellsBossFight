using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.ModLoader;
using DeadCellsBossFight.Utils;
using DeadCellsBossFight.Core;
using Terraria.DataStructures;

namespace DeadCellsBossFight.Projectiles.WeaponAnimationProj;

public class PerfectHalberdD : DC_WeaponAnimation
{
    public override string AnimName => "perfectHalberdAtkD";
    public override string fxName => "fxPerfectHalberdAtkD";
    public override int HitFrame => 5;
    public override int fxStartFrame => 5;

    private Dictionary<int, DCAnimPic> WeaponDic = new();
    private Dictionary<int, DCAnimPic> fxDic = new();
    public override int TotalFrame => WeaponDic.Count;
    public override int fxFrames => fxDic.Count;
    public override int OnionSkinFrame => 5;
    public override float OnionSkinOffX => 10.2f;
    public override void SetDefaults()
    {
        fxDic = AssetsLoader.fxAtlas[fxName];
        QuickSetDefault(156, 70, 16, DamageClass.Default, 1.4f, slowBeginFrame: 9);
    }
    public override void OnSpawn(IEntitySource source)
    {
        SpawnSourceCheck(source, ref WeaponDic);
    }
    public override void AI()
    {
        base.AI();
        DrawTheAnimationInAI(53f, -4f);
        PlayWeaponSound(AssetsLoader.weapon_perfectsw_release4, 4);
        CameraBump(2.4f, 1f, 19);//屏幕震动
        Bump(1.8f);
    }
    public override void PostDraw(Color lightColor)
    {
        DrawWeaponTexture(WeaponDic, 6, -25, true, new(255, 207, 92), true);
        DrawfxTexture(fxDic, 6, -25);
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