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

public class BroadSwordAtkB : DC_WeaponAnimation
{
    public override string AnimName => "atkBroadSwordB";
    public override string fxName => "fxAtkBroadSwordB";
    public override int HitFrame => 12;
    public override int fxStartFrame => 13;

    private Dictionary<int, DCAnimPic> WeaponDic = new();
    private Dictionary<int, DCAnimPic> fxDic = new();
    public override int TotalFrame => WeaponDic.Count;
    public override int fxFrames => fxDic.Count;
    public override void SetDefaults()
    {
        fxDic = AssetsLoader.fxAtlas[fxName];
        QuickSetDefault(124, 140, 17, DamageClass.Default, 1.4f, slowBeginFrame: 5, slowEndFrame : 10);
    }
    public override void OnSpawn(IEntitySource source)
    {
        SpawnSourceCheck(source, ref WeaponDic);
    }
    public override void AI()
    {
        base.AI();
        DrawTheAnimationInAI(19.4f, -19.2f);
        PlayChargeSound(AssetsLoader.weapon_broadsword_charge2);
        PlayWeaponSound(AssetsLoader.weapon_broadsword_release2, 12);
        CameraBump(4.4f, 8.6f, 23, Vector2.UnitY);
        Bump(1.6f);

        if (Projectile.frame == HitFrame)
        {
            for (int i = 0; i < 8; i++)
                Dust.NewDustDirect((Projectile.velocity.X > 0 ? Projectile.Right - new Vector2(36, -40) : Projectile.Left + new Vector2(22, 40)), 40, 30, DustID.Dirt, Projectile.velocity.X, -1.2f, Scale: Main.rand.NextFloat(1f, 1.4f));
        }
    }
    public override void PostDraw(Color lightColor)
    {
        DrawWeaponTexture(WeaponDic, 6, -28, true, new(255, 157, 0), true);
        DrawfxTexture(fxDic, 6, -28, true, new(255, 249, 180));
    }
    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
    {
        modifiers.SetCrit(1.8f);
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        SoundEngine.PlaySound(AssetsLoader.hit_broadsword);
    }
}
