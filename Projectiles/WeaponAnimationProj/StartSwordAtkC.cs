using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using System.Collections.Generic;
using Terraria.ModLoader;
using DeadCellsBossFight.Core;
using Terraria.DataStructures;

namespace DeadCellsBossFight.Projectiles.WeaponAnimationProj;

public class StartSwordAtkC : DC_WeaponAnimation
{
    public override string AnimName => "atkSaberA";
    public override string fxName => "fxSaberA";
    public override int HitFrame => 7;
    public override int fxStartFrame => 6;

    private Dictionary<int, DCAnimPic> WeaponDic = new();
    private Dictionary<int, DCAnimPic> fxDic = new();
    public override int TotalFrame => WeaponDic.Count;
    public override int fxFrames => fxDic.Count;
    public override void SetDefaults()
    {
        fxDic = AssetsLoader.fxAtlas[fxName];
        QuickSetDefault(132, 72, 20, DamageClass.Default, 1.5f, slowBeginFrame: 3, slowEndFrame : 2);
        Projectile.CritChance = 0;
    }
    public override void OnSpawn(IEntitySource source)
    {
        SpawnSourceCheck(source, ref WeaponDic);
    }
    public override void AI()
    {
        base.AI();
        DrawTheAnimationInAI(18f, 2.89f);
        PlayWeaponSound(AssetsLoader.weapon_shortsw_release, 4);
        CameraBump(1.7f, 1f, 16);
    }
    public override void PostDraw(Color lightColor)
    {
        DrawWeaponTexture(WeaponDic, 12, -28, true, new(187, 198, 231), true);
        DrawfxTexture(fxDic, 12, -28, true, new(250, 255, 73));
        
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        SoundEngine.PlaySound(AssetsLoader.hit_blade);
    }
}
