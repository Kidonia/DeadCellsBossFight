using Terraria.Audio;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using DeadCellsBossFight.Contents.Buffs;
using DeadCellsBossFight.Utils;
using DeadCellsBossFight.Core;
using Terraria.DataStructures;

namespace DeadCellsBossFight.Projectiles.WeaponAnimationProj;

public class BleedCritAtkB : DC_WeaponAnimation
{

    public override string AnimName => "atkKnifeA";
    public override string fxName => "fxKnifeA";
    public override int HitFrame => 5;
    public override int fxStartFrame => 5;

    private Dictionary<int, DCAnimPic> WeaponDic = new();
    private Dictionary<int, DCAnimPic> fxDic = new();
    public override int TotalFrame => WeaponDic.Count;
    public override int fxFrames => fxDic.Count;
    public override void SetDefaults()
    {
        fxDic = AssetsLoader.fxAtlas[fxName];
        QuickSetDefault(70, 72, 16, DamageClass.Default, 0.12f);
    }
    public override void OnSpawn(IEntitySource source)
    {
        SpawnSourceCheck(source, ref WeaponDic);
    }
    public override void AI()
    {
        base.AI();
        DrawTheAnimationInAI(13.8f, -11.2f);
        PlayWeaponSound(AssetsLoader.weapon_saber_release1, 4);
        CameraBump(1.38f, 1f, 16);
    }
    public override void PostDraw(Color lightColor)//绘制快刀贴图
    {
        DrawWeaponTexture(WeaponDic, 8, -28, true, new(255, 227, 77));
        DrawfxTexture(fxDic, 8, -28, true, new(5, 7, 7));
    }
    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
    {
        if (target.HasBuff(ModContent.BuffType<Bleed>()) || target.poisoned)
        {
            modifiers.SetCrit(2.1f);
        }
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {

        if (target.HasBuff(ModContent.BuffType<Bleed>()) || target.poisoned)
        {
            PlayCritSound(info);
        }

        SoundEngine.PlaySound(AssetsLoader.hit_blade);
    }

}
