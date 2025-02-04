using Terraria.Audio;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ID;
using System.Collections.Generic;
using DeadCellsBossFight.Utils;
using DeadCellsBossFight.Projectiles.EffectProj;
using DeadCellsBossFight.Core;

namespace DeadCellsBossFight.Projectiles.WeaponAnimationProj;

public class TickScytheAtkA2 : DC_WeaponAnimation
{
    public override string AnimName => "atkScytheA2";
    public override string fxName => "fxAtkScytheA2";
    public override int HitFrame => 35;
    public override int fxStartFrame => 34;
    private Dictionary<int, DCAnimPic> WeaponDic = new();
    private Dictionary<int, DCAnimPic> fxDic = new();
    public override int TotalFrame => WeaponDic.Count;
    public override int fxFrames => fxDic.Count;
    private bool allTargetNoCrit;
    public override void SetDefaults()
    {
        fxDic = AssetsLoader.fxAtlas[fxName];
        QuickSetDefault(190, 76, 20, DamageClass.Default, 3f, slowBeginFrame : 8);
    }
    public override void OnSpawn(IEntitySource source)
    {
        SpawnSourceCheck(source, ref WeaponDic);
        if (!playerHurt.TickScytheCanCrit)
            allTargetNoCrit = true;

        playerHurt.TickScytheCheckHit = false;
    }
    public override void AI()
    {
        base.AI();
        DrawTheAnimationInAI(65f, 8f);
        PlayChargeSound(AssetsLoader.weapon_tickscythe_charge2);
        PlayWeaponSound(AssetsLoader.weapon_tickscythe_release1, HitFrame);
        CameraBump(8f, 4.2f, 20);
        Bump(2.6f);

        if (Projectile.frame == HitFrame + 2 && npc != null)
        {
            for (int i = 0; i < 32; i++)
                Dust.NewDustDirect((Projectile.velocity.X > 0 ? Projectile.Right - new Vector2(200, 10) : Projectile.Left + new Vector2(60, -10)), 210, 45, DustID.Dirt, Projectile.velocity.X * 2.8f, 0.8f, Scale : Main.rand.NextFloat(1f, 1.7f));
        }

        if (Projectile.frame == TotalFrame - 1 && !playerHurt.TickScytheCheckHit)
            playerHurt.TickScytheCanCrit = false;
    }
    public override void PostDraw(Color lightColor)
    {
        DrawWeaponTexture(WeaponDic, 6, -27, true, new(65, 169, 251), true);
        DrawfxTexture(fxDic, 6, -27);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (playerHurt.TickScytheCanCrit && !allTargetNoCrit)
        {
            modifiers.SetCrit();
            modifiers.CritDamage += 3.2f;
        }
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        SoundEngine.PlaySound(AssetsLoader.hit_broadsword);
        playerHurt.TickScytheCheckHit = true;
        playerHurt.TickScytheCanCrit = true;

        if (hit.Crit)//改为暴击时生成放大效果
        {
            IEntitySource source = player.GetSource_FromAI();
            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<EffectProj.RoundTwist>(), 0, 0);
        }
    }
    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
    {
        if (playerHurt.TickScytheCanCrit && !allTargetNoCrit)
        {
            ThisATKShouldCritSound();
            modifiers.SetCrit(4.3f);
        }
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        SoundEngine.PlaySound(AssetsLoader.hit_broadsword);
        playerHurt.TickScytheCheckHit = true;
        playerHurt.TickScytheCanCrit = true;

        if (playerHurt.ShouldPlayCritSound)
        {
            IEntitySource source = npc.GetSource_FromAI();
            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<RoundTwist>(), 0, 0);
            PlayCritSound(info);
            playerHurt.ShouldPlayCritSound = false;
        }

    }
}
