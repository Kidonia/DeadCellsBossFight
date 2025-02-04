using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.ModLoader;
using DeadCellsBossFight.Utils;
using DeadCellsBossFight.Core;
using Terraria.DataStructures;

namespace DeadCellsBossFight.Projectiles.WeaponAnimationProj;

public class KingsSpearAtkA : DC_WeaponAnimation
{
    public override string AnimName => "halberdAtkA";
    public override string fxName => "halberdFxAtkA";
    public override int HitFrame => 18;
    public override int fxStartFrame => 17;

    private Dictionary<int, DCAnimPic> WeaponDic = new();
    private Dictionary<int, DCAnimPic> fxDic = new();
    public override int TotalFrame => WeaponDic.Count;
    public override int fxFrames => fxDic.Count;

    public override int OnionSkinFrame => 18;
    public override float OnionSkinOffX => 5f;

    public override void SetDefaults()
    {
        fxDic = AssetsLoader.fxAtlas[fxName];
        QuickSetDefault(148, 72, 16, DamageClass.Default, 1.4f, slowBeginFrame: 9, slowEndFrame: 6);
    }
    public override void OnSpawn(IEntitySource source)
    {
        SpawnSourceCheck(source, ref WeaponDic);
    }
    public override void AI()
    {
        base.AI();
        DrawTheAnimationInAI(68f, -4f);
        PlayChargeSound(AssetsLoader.weapon_spear_charge1);
        PlayWeaponSound(AssetsLoader.weapon_doublelance_release1, 18);
        CameraBump(2.4f, 1f, 19);//屏幕震动
        Bump(2f);
    }
    public override void PostDraw(Color lightColor)
    {
        if (playerHurt.KingsSpearCritTime > 0)
            DrawWeaponTexture(WeaponDic, 6, -28, true, new(217, 67, 0), true);
        else
            DrawWeaponTexture(WeaponDic, 6, -28, true, new(137, 151, 176), true);
     
        DrawfxTexture(fxDic, 6, -28, true, new(244, 255, 73));
    }
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (playerHurt.KingsSpearCritTime > 0)//可暴击
            modifiers.SetCrit();
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        SoundEngine.PlaySound(AssetsLoader.hit_blade);
        //该函数在击中敌人减少生命后执行
        if (target.life <= 0)//判断杀死敌人
            playerHurt.KingsSpearComboKillNum++;//每杀一个击杀计数加一
    }
    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
    {
        if (playerHurt.KingsSpearCritTime > 0)//可暴击
        {
            ThisATKShouldCritSound();
            modifiers.SetCrit();
        }
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        CheckAndPlayCritSound(info);
        SoundEngine.PlaySound(AssetsLoader.hit_blade);
    }

}
