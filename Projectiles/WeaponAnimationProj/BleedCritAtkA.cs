using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using System.Collections.Generic;
using DeadCellsBossFight.Contents.Buffs;
using DeadCellsBossFight.Utils;
using DeadCellsBossFight.Core;
using Terraria.DataStructures;

namespace DeadCellsBossFight.Projectiles.WeaponAnimationProj;

public class BleedCritAtkA : DC_WeaponAnimation
{
    public override string AnimName => "atkKnifeB";
    public override string fxName => "fxKnifeB";
    public override int HitFrame => 5;
    public override int fxStartFrame => 4;

    private Dictionary<int, DCAnimPic> WeaponDic = new();
    private Dictionary<int, DCAnimPic> fxDic = new();
    public override int TotalFrame => WeaponDic.Count;
    public override int fxFrames => fxDic.Count;
    public override void SetDefaults()
    {
        fxDic = AssetsLoader.fxAtlas[fxName];
        QuickSetDefault(72, 74, 18, DamageClass.Default, 0.16f, slowBeginFrame: 2, slowEndFrame: 4);
    }
    public override void OnSpawn(IEntitySource source)
    {
        SpawnSourceCheck(source, ref WeaponDic);
    }
    public override void AI()
    {
        base.AI();
        DrawTheAnimationInAI(14f, -11.4f);
        PlayWeaponSound(AssetsLoader.weapon_saber_release1, 4);
        CameraBump(1.65f, 1f, 16);
    }
    public override void PostDraw(Color lightColor)//绘制快刀贴图
    {
        DrawWeaponTexture(WeaponDic, 12, -28, true, new(255, 227, 77));
        DrawfxTexture(fxDic, 12, -28, true, new(5, 7, 7));
    }
    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
    {
        if (target.HasBuff(ModContent.BuffType<Bleed>()) || target.poisoned)
        {
            modifiers.SetCrit(2.15f);
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
