using DeadCellsBossFight.Core;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Projectiles.BasicAnimationProj;

public class JumpDownProj : DC_WeaponAnimation
{
    public override string AnimName => "jumpDown";
    private Dictionary<int, DCAnimPic> WeaponDic = new();
    public override int TotalFrame => WeaponDic.Count;
    public override void SetDefaults()
    {
        WeaponDic = AssetsLoader.BHanimAtlas[AnimName];
        QuickSetDefault(2, 2, 0, DamageClass.Default, 0, slowBeginFrame: 2);
    }
    public override void OnSpawn(IEntitySource source)
    {
        GetBHNPC();
    }
    public override bool? CanDamage()
    {
        return false;
    }
    public override void AI()
    {
        if (!npc.active)
            Projectile.Kill();
        Projectile.direction = npc.direction;

        if (Main.GameUpdateCount % 3 == 0)
            Projectile.frame++;
        //记得改位置
        Projectile.position = npc.Center - Projectile.Size / 2 + new Vector2(Projectile.direction, -5f) * Projectile.scale;
        Projectile.timeLeft = 2;


        if (Projectile.frame == TotalFrame)
        {
            Projectile.frame = 0;
        }
    }
    public override void PostDraw(Color lightColor)
    {
        DrawWeaponTexture(WeaponDic, 0, -27);
    }
}