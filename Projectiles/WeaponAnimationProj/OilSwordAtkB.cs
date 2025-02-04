using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using System.Collections.Generic;
using DeadCellsBossFight.Contents.Buffs;
using DeadCellsBossFight.Core;
using Terraria.DataStructures;

namespace DeadCellsBossFight.Projectiles.WeaponAnimationProj;
public class OilSwordAtkB : DC_WeaponAnimation
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
        QuickSetDefault(105, 70, 22, DamageClass.Default, knockBack: 1.8f, slowBeginFrame : 2 , slowEndFrame: 6);
    }
    public override void OnSpawn(IEntitySource source)
    {
        SpawnSourceCheck(source, ref WeaponDic);
    }
    public override void AI()
    {
        base.AI();
        DrawTheAnimationInAI(20f, -1.8f);
        PlayWeaponSound(AssetsLoader.weapon_dualdg_release2, 4);
        CameraBump(1.6f, 1f, 17);
    }
    public override void PostDraw(Color lightColor)//绘制油刀贴图
    {
        if (playerHurt.OilSwordHitFireTargetTime > 0)//暴击绘制
        {
            DrawWeaponTexture(WeaponDic, 12, -28, true, new(132, 100, 186), true);
            DrawfxTexture(fxDic , 12, -28, true, new(194, 165, 255));
        }
        else//不暴击绘制
        {
            DrawWeaponTexture(WeaponDic, 12, -28, true, new(101, 79, 219));
            DrawfxTexture(fxDic, 12, -28, true ,new(36, 43, 76));
        }
    }
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (target.onFire || target.onFire2 || target.onFire3 || target.onFrostBurn || target.onFrostBurn2)
            playerHurt.OilSwordHitFireTargetTime = 600;
        if (playerHurt.OilSwordHitFireTargetTime > 0)
        {
            modifiers.SetCrit();
            modifiers.CritDamage -= 0.06f;
        }
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(ModContent.BuffType<Oil>(), 1500);
        SoundEngine.PlaySound(AssetsLoader.hit_blade);
    }
}
