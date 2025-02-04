using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ID;
using DeadCellsBossFight.Core;
using Terraria.DataStructures;

namespace DeadCellsBossFight.Projectiles.WeaponAnimationProj;

public class AdeleScytheAtkB : DC_WeaponAnimation
{
    public override string AnimName => "AtkReaperToolB";
    public override string fxName => "FXAtkReaperToolB";
    public override int HitFrame => 3;
    public override int fxStartFrame => 0;

    private Dictionary<int, DCAnimPic> WeaponDic = new();
    private Dictionary<int, DCAnimPic> fxDic = new();
    public override int TotalFrame => WeaponDic.Count;
    public override int fxFrames => fxDic.Count;
    public override void SetDefaults()
    {
        fxDic = AssetsLoader.fxAtlas[fxName];
        QuickSetDefault(116, 128, 18, DamageClass.Default, 0.16f, slowBeginFrame : 4, slowEndFrame :4);
    }
    public override void OnSpawn(IEntitySource source)
    {
        SpawnSourceCheck(source, ref WeaponDic);
    }
    public override void AI()
    {
        base.AI();
        DrawTheAnimationInAI(42f, -24.8f);
        PlayWeaponSound(AssetsLoader.purpleDLC_scythe_AtkB_release, 3);
        PlayChargeSound(AssetsLoader.purpleDLC_scythe_charge);
        CameraBump(2.6f, 4f, 18, Vector2.UnitY);
    }
    public override void PostDraw(Color lightColor)//绘制快刀贴图
    {
        DrawWeaponTexture(WeaponDic, 12, -28, true, new(81, 192, 255), true);
        DrawfxTexture(fxDic, 8, -28);
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (!target.boss && target.life <= 0)
        {
            float k = target.type < NPCID.Count ? 1 : 2;
            Projectile.NewProjectile(Entity.GetSource_FromAI(), target.position, Vector2.Zero, ModContent.ProjectileType<SoulProj>(), Projectile.damage, 3f, player.whoAmI, target.type, k);
        }
        SoundEngine.PlaySound(AssetsLoader.purpleDLC_scythe_hit);
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        SoundEngine.PlaySound(AssetsLoader.purpleDLC_scythe_hit);
    }

}
