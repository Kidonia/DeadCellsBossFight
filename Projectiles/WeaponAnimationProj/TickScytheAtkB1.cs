using Terraria.Audio;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ID;
using System.Collections.Generic;
using DeadCellsBossFight.Projectiles.EffectProj;
using DeadCellsBossFight.Utils;
using DeadCellsBossFight.Core;

namespace DeadCellsBossFight.Projectiles.WeaponAnimationProj;

public class TickScytheAtkB1 : DC_WeaponAnimation
{
    public override string AnimName => "atkScytheB1";
    public override string fxName => "fxAtkScytheB2";
    public override int HitFrame => 18;
    public override int fxStartFrame => 17;
    private Dictionary<int, DCAnimPic> WeaponDic = new();
    private Dictionary<int, DCAnimPic> fxDic = new();
    public override int TotalFrame => WeaponDic.Count;
    public override int fxFrames => fxDic.Count;
    private bool allTargetNoCrit;//检测当前攻击是否会对第二个以及以后的敌人造成暴击，把这页里用到这个的全删了就是镀金弓的。

    //生成：           如果不能暴击 !playerHurt.TickScytheCanCrit，则 这段攻击里所有击中的敌人都不会暴击 allTargetNoCrit = true;
    //击中前检测：如果 能暴击playerHurt.TickScytheCanCrit ，且，这段攻击不是不能暴击的  !allTargetNoCrit，添加暴击伤害
    //击中时触发：击中到敌人playerHurt.TickScytheCheckHit = true;      下段攻击可暴击 playerHurt.TickScytheCanCrit = true;
    //AI即将结束时：如果 未击中敌人 !playerHurt.TickScytheCheckHit， 下段攻击不能暴击 playerHurt.TickScytheCanCrit = false;
    public override void SetDefaults()
    {
        fxDic = AssetsLoader.fxAtlas[fxName];
        QuickSetDefault(158, 175, 20, DamageClass.Default, 3f, slowBeginFrame: 12, slowEndFrame: 9);
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
        DrawTheAnimationInAI(72f, -55f);
        PlayChargeSound(AssetsLoader.weapon_tickscythe_charge1);
        PlayWeaponSound(AssetsLoader.weapon_tickscythe_release1, HitFrame);
        CameraBump(8.4f, 5f, 16, Vector2.UnitY);

        if (Projectile.frame == HitFrame + 2 && npc != null)
        {
            for (int i = 0; i < 20; i++)//刀尖生成粒子效果
                Dust.NewDustDirect(Projectile.Top + Projectile.velocity * 52, 64, 260, DustID.Dirt, Projectile.velocity.X, -5f, Scale: Main.rand.NextFloat(1f, 1.3f));
        }

        if (Projectile.frame == TotalFrame - 1 && !playerHurt.TickScytheCheckHit)//如果武器使用完毕，且，没有击中敌人
            playerHurt.TickScytheCanCrit = false;//不能暴击
    }
    public override void PostDraw(Color lightColor)
    {
        DrawWeaponTexture(WeaponDic, 0, -28, true, new(255, 212, 45), true);
        DrawfxTexture(fxDic, 0, -28);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (playerHurt.TickScytheCanCrit && !allTargetNoCrit)//如果 能暴击，且，这段攻击不是不能暴击的
        {
            modifiers.SetCrit();
            modifiers.CritDamage += 0.55f;
        }
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        SoundEngine.PlaySound(AssetsLoader.hit_broadsword);
        playerHurt.TickScytheCheckHit = true;//检测到击中敌人
        playerHurt.TickScytheCanCrit = true;//击中敌人，下段攻击可暴击

        if (hit.Crit)//改为暴击时生成放大效果
        {
            IEntitySource source = player.GetSource_FromAI();
            Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<RoundTwist>(), 0, 0);
        }
    }
    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
    {
        if (playerHurt.TickScytheCanCrit && !allTargetNoCrit)
        {
            ThisATKShouldCritSound();
            modifiers.SetCrit(2.55f);
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
