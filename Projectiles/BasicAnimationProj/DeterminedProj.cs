
using DeadCellsBossFight.Core;
using DeadCellsBossFight.NPCs;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Projectiles.BasicAnimationProj;

public class DeterminedProj : DC_WeaponAnimation
{
    public override string AnimName => "determined";
    private Dictionary<int, DCAnimPic> WeaponDic = new();
    public override int TotalFrame => WeaponDic.Count;
    public override void SetDefaults()
    {
        WeaponDic = AssetsLoader.BHanimAtlas[AnimName];
        QuickSetDefault(2, 2, 0, DamageClass.Default, 0);
    }
    public override void AI()
    {
        base.AI();
        Projectile.direction = npc.direction;
        if (Main.GameUpdateCount % 2 == 0)
            Projectile.frame++;
        //记得改位置
        Projectile.position = npc.Center - Projectile.Size / 2 + new Vector2(Projectile.direction, -5f) * Projectile.scale;
        Projectile.timeLeft = 2;

        if (Projectile.frame == TotalFrame)
        {
            Projectile.frame = 0;
            var BH = npc.ModNPC as BH;
            BH.standingtime = Main.rand.Next(36, 72);
            BH.ChangeMove(BHMoveType.Idle);
            return;
        }
        if (!npc.active)
            Projectile.Kill();
    }
    public override void PostDraw(Color lightColor)
    {
        DrawWeaponTexture(WeaponDic, 0, -27);
    }

}
