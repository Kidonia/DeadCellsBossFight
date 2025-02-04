using Terraria.Audio;
using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using DeadCellsBossFight.Utils;
using DeadCellsBossFight.Core;
using Terraria.DataStructures;

namespace DeadCellsBossFight.Projectiles.WeaponAnimationProj;

public class BroadSwordAtkC : DC_WeaponAnimation
{
    public override string AnimName => "atkBroadSwordC";
    public override string fxName => "fxAtkBroadSwordC";
    public override int HitFrame => 16;
    public override int fxStartFrame => 17;

    private Dictionary<int, DCAnimPic> WeaponDic = new();
    private Dictionary<int, DCAnimPic> fxDic = new();
    public override int TotalFrame => WeaponDic.Count;
    public override int fxFrames => fxDic.Count;
    public override void SetDefaults()
    {
        fxDic = AssetsLoader.fxAtlas[fxName];
        QuickSetDefault(172, 140, 18, DamageClass.Default, 1.4f, slowBeginFrame: 10, slowEndFrame: 10);
    }
    public override void OnSpawn(IEntitySource source)
    {
        SpawnSourceCheck(source, ref WeaponDic);
    }
    public override void AI()
    {
        base.AI();
        DrawTheAnimationInAI(8.3f, -22.2f);
        PlayChargeSound(AssetsLoader.weapon_broadsword_charge3);
        PlayWeaponSound(AssetsLoader.weapon_broadsword_release3, 11);
        CameraBump(5.8f, 10.8f, 24, - Vector2.UnitY);
        Bump(1.6f);

        if (Projectile.frame == HitFrame)
        {
            for (int i = 0; i < 18; i++)
                Dust.NewDustDirect((Projectile.velocity.X > 0 ? Projectile.Right - new Vector2(36, -48) : Projectile.Left + new Vector2(22, 48)), 70, 60, DustID.Dirt, Projectile.velocity.X, -3.8f, Scale: Main.rand.NextFloat(1.3f, 2.4f));
        }
    }
    public override void PostDraw(Color lightColor)
    {
        DrawWeaponTexture(WeaponDic, 6, -28, true, new(255, 157, 0), true);
        DrawfxTexture(fxDic, 6, -28, true, new(255, 249, 180));
    }
    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
    {
        modifiers.SetCrit(2.8f);
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        SoundEngine.PlaySound(AssetsLoader.hit_broadsword);
    }
}

