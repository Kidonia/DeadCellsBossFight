using Terraria.Audio;
using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;
using DeadCellsBossFight.Utils;
using DeadCellsBossFight.Core;
using Terraria.DataStructures;

namespace DeadCellsBossFight.Projectiles.WeaponAnimationProj;

public class LowHealthAtkC : DC_WeaponAnimation
{
    public override string AnimName => "atkBackStabberA";
    public override string fxName => "fxAtkBackstabberA";
    public override int HitFrame => 7;
    public override int fxStartFrame => 6;

    private Dictionary<int, DCAnimPic> WeaponDic = new();
    private Dictionary<int, DCAnimPic> fxDic = new();
    public override int TotalFrame => WeaponDic.Count;
    public override int fxFrames => fxDic.Count;
    public override void SetDefaults()
    {
        fxDic = AssetsLoader.fxAtlas[fxName];
        QuickSetDefault(54, 54, 20, DamageClass.Default, 0.12f);
    }
    public override void OnSpawn(IEntitySource source)
    {
        SpawnSourceCheck(source, ref WeaponDic);
    }
    public override void AI()
    {
        base.AI();
        DrawTheAnimationInAI(12.4f, -2f);
        PlayWeaponSound(AssetsLoader.weapon_shortsw_release, 3);
        CameraBump(1.65f, 1f, 16);
    }
    public override void PostDraw(Color lightColor)
    {
        DrawWeaponTexture(WeaponDic, 18, -28, true, new(80, 187, 255), true);
        DrawfxTexture(fxDic, 18, -28, true, new(153, 225, 80));
    }
    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
    {
        if (npc.life < npc.lifeMax / 2)
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