using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.ModLoader;
using DeadCellsBossFight.Core;
using Terraria.DataStructures;

namespace DeadCellsBossFight.Projectiles.WeaponAnimationProj;

public class AdminAttack : DC_WeaponAnimation
{
    public override string AnimName => "atkPunchA";
    public override string fxName => "fxSwordAtkB";
    public override int HitFrame => 2;
    public override int fxStartFrame => 1;

    private Dictionary<int, DCAnimPic> WeaponDic = new();
    private Dictionary<int, DCAnimPic> fxDic = new();
    public override int TotalFrame => WeaponDic.Count;
    public override int fxFrames => fxDic.Count;
    public override void SetDefaults()
    {
        WeaponDic = AssetsLoader.BHanimAtlas[AnimName];
        fxDic = AssetsLoader.fxAtlas[fxName];
        Projectile.width = Main.screenWidth;
        Projectile.height = Main.screenHeight;
        Projectile.damage = 114514;
        Projectile.friendly = true;
        Projectile.scale = 3f;
        Projectile.knockBack = 0;
        Projectile.DamageType = DamageClass.Generic;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 2;
        Projectile.hide = false;
        Projectile.localNPCHitCooldown = -1;
    }
    public override void OnSpawn(IEntitySource source)
    {
        spawner = npc;
    }
    public override void AI()
    {
        base.AI();
        DrawTheAnimationInAI(0, 0);
    }
    public override void PostDraw(Color lightColor)//绘制血刀贴图
    {
        DrawWeaponTexture(WeaponDic, 5, -35);
        DrawfxTexture(fxDic, 8, -32, true, new(0, 0, 0));
    }
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.DisableCrit();
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        SoundEngine.PlaySound(AssetsLoader.hit_blade);
    }
}

