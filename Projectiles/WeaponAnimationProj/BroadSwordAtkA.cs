using Terraria.Audio;
using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;
using DeadCellsBossFight.Core;
using Terraria.DataStructures;

namespace DeadCellsBossFight.Projectiles.WeaponAnimationProj;

public class BroadSwordAtkA : DC_WeaponAnimation
{
    public override string AnimName => "atkBroadSwordA";
    public override string fxName => "fxAtkBroadSwordA";
    public override int HitFrame => 9;
    public override int fxStartFrame => 5;

    private Dictionary<int, DCAnimPic> WeaponDic = new();
    private Dictionary<int, DCAnimPic> fxDic = new();
    public override int TotalFrame => WeaponDic.Count;
    public override int fxFrames => fxDic.Count;
    public override void SetDefaults()
    {
        fxDic = AssetsLoader.fxAtlas[fxName];
        QuickSetDefault(168, 65, 16, DamageClass.Default, 1.4f, slowBeginFrame: 4, slowEndFrame : 6);
    }
    public override void OnSpawn(IEntitySource source)
    {
        SpawnSourceCheck(source, ref WeaponDic);
    }
    public override void AI()
    {
        base.AI();
        DrawTheAnimationInAI(5.4f, -5.2f);
        PlayChargeSound(AssetsLoader.weapon_broadsword_charge1);
        PlayWeaponSound(AssetsLoader.weapon_broadsword_release1, 13);
        CameraBump(4.1f, 8.9f, 24);
        Bump(1.6f);
    }
    public override void PostDraw(Color lightColor)
    {
        DrawWeaponTexture(WeaponDic, 6, -28, true, new(118, 112, 217), true);
        DrawfxTexture(fxDic, 6, -28, true, new(0, 159, 255));
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        SoundEngine.PlaySound(AssetsLoader.hit_broadsword);
    }
}

