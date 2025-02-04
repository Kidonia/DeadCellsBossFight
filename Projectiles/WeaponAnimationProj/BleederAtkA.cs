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

public class BleederAtkA : DC_WeaponAnimation
{
    //以此为模板。
    public override string AnimName => "atkSaberA";//武器动画名称
    public override string fxName => "fxSaberA";//fx特效名称
    public override int HitFrame => 7;//击中帧
    public override int fxStartFrame => 6;//fx特效开始播放帧

    private Dictionary<int, DCAnimPic> WeaponDic = new();//新建字典，存放武器动画
    private Dictionary<int, DCAnimPic> fxDic = new();//新建字典，存放fx特效
    public override int TotalFrame => WeaponDic.Count;//总帧图图数，直接复制粘贴
    public override int fxFrames => fxDic.Count;//总fx特效图图数，直接复制粘贴
    public override void SetDefaults()
    {
        fxDic = AssetsLoader.fxAtlas[fxName];//初始化fx特效字典，直接复制粘贴
        QuickSetDefault(128, 68, 1, DamageClass.Default, 1.5f, slowBeginFrame : 4, slowEndFrame : 3);
    }
    public override void OnSpawn(IEntitySource source)
    {
        SpawnSourceCheck(source, ref WeaponDic);
    }
    public override void AI()
    {
        base.AI();
        DrawTheAnimationInAI(18f, 2.89f);//碰撞箱位置
        PlayWeaponSound(AssetsLoader.weapon_saber_release1, 4);//播放攻击声音
        CameraBump(2.4f, 1f, 19);//屏幕震动
        // Main.NewText(AssetsLoader.BHanimAtlas["kick02B"][2].headPos[0]);
    }
    public override void PostDraw(Color lightColor)//绘制血刀贴图
    {
        DrawWeaponTexture(WeaponDic, 12, -28, true, new(255, 246, 173));//帧图绘制武器动画
        DrawfxTexture(fxDic, 12, -28, true, new(163, 0, 22));//帧图绘制fx特效图
        // DrawHeadTexture(WeaponDic, 18, -20);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {

        var dcplayer = target.GetModPlayer<DCPlayer>();
        dcplayer.BleedLevel++;//流血层数加一
        target.AddBuff(ModContent.BuffType<Bleed>(), 360);
        SoundEngine.PlaySound(AssetsLoader.hit_blade);
    }

}
