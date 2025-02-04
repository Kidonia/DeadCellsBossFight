using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using System.Collections.Generic;
using DeadCellsBossFight.Contents.Buffs;
using DeadCellsBossFight.Core;
using DeadCellsBossFight.Contents.GlobalChanges;
using Terraria.DataStructures;

namespace DeadCellsBossFight.Projectiles.WeaponAnimationProj;
public class BleederAtkB : DC_WeaponAnimation
{
    public override string AnimName => "atkSaberB";
    public override string fxName => "fxSaberB";
    public override int HitFrame => 5;
    public override int fxStartFrame => 2;
    private Dictionary<int, DCAnimPic> WeaponDic = new();
    private Dictionary<int, DCAnimPic> fxDic = new();
    public override int TotalFrame => WeaponDic.Count;
    public override int fxFrames => fxDic.Count;
    public override void SetDefaults()
    {
        fxDic = AssetsLoader.fxAtlas[fxName];
        QuickSetDefault(108, 70, 22, DamageClass.Default, knockBack : 1.7f, slowEndFrame : 4);
    }
    public override void OnSpawn(IEntitySource source)
    {
        SpawnSourceCheck(source, ref WeaponDic);
    }
    public override void AI()
    {
        base.AI();
        DrawTheAnimationInAI(20f, -1.8f);
        PlayWeaponSound(AssetsLoader.weapon_saber_release2, 3);
        CameraBump(1.6f, 1f, 17);
    }
    public override void PostDraw(Color lightColor)//绘制血刀贴图
    {
        DrawWeaponTexture(WeaponDic, 12, -29, true, new(255, 246, 173));
        DrawfxTexture(fxDic, 12, -29, true, new(163, 0, 22));
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        var dcplayer = target.GetModPlayer<DCPlayer>();
        dcplayer.BleedLevel++;//流血层数加一
        target.AddBuff(ModContent.BuffType<Bleed>(), 360);
        SoundEngine.PlaySound(AssetsLoader.hit_blade);
    }
}
