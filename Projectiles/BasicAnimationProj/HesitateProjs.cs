
using DeadCellsBossFight.Core;
using DeadCellsBossFight.NPCs;
using DeadCellsBossFight.Utils;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
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

public class ThinkStandProj : DC_WeaponAnimation
{
    public override string AnimName => "think";
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

public class TravoltaFullProj : DC_WeaponAnimation
{
    public override string AnimName => "travolta";
    public override string AnimName2 => "travoltaIdle";
    private Dictionary<int, DCAnimPic> AnimDic = new();
    public override int TotalFrame => AnimDic.Count;
    public override void SetDefaults()
    {
        var animDic1 = AssetsLoader.BHanimAtlas[AnimName];
        var animDic2 = AssetsLoader.BHanimAtlas[AnimName2];
        AnimDic = NormalUtils.MergeTwoAnimDictionaries(animDic1, animDic2);

        QuickSetDefault(2, 2, 0, DamageClass.Default, 0);
    }
    public override void OnSpawn(IEntitySource source)
    {
        GetBHNPC();
    }
    public override void AI()
    {
        Projectile.direction = npc.direction;
        if (Projectile.frame > 50) // 50帧往后快一点，直接一倍。
            Projectile.frame++;
        else if (Main.GameUpdateCount % 2 == 0)
            Projectile.frame++;
        //记得改位置
        Projectile.position = npc.Center - Projectile.Size / 2 + new Vector2(Projectile.direction, -5f) * Projectile.scale;
        Projectile.timeLeft = 2;

        if (Projectile.frame == TotalFrame)
        {
            Projectile.frame = 0;
            var BH = npc.ModNPC as BH;
            BH.standingtime = Main.rand.Next(40, 78);
            BH.ChangeMove(BHMoveType.Idle);
            return;
        }
        if (!npc.active)
            Projectile.Kill();
    }
    public override void PostDraw(Color lightColor)
    {
        DrawWeaponTexture(AnimDic, 0, -27);
    }

}
public class SayNoProj : DC_WeaponAnimation
{
    public override string AnimName => "no";
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
            BH.standingtime = Main.rand.Next(32, 60);
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
public class FuckOffProj : DC_WeaponAnimation
{
    public override string AnimName => "fuckOff";
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
            BH.standingtime = Main.rand.Next(32, 60);
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