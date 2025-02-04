using DeadCellsBossFight.Contents.DamageClasses;
using DeadCellsBossFight.Projectiles.WeaponAnimationProj;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace DeadCellsBossFight.Items.DCWeaponItems;

public class StartSword : DeadCellsItem
{

    public override void SetDefaults()
    {
        iconX = 18;
        iconY = 0;
        SetWeaponDefaults(DamageClass.Melee, 7, 2f, 2, 2, 1);
    }
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (FirstAttack())
        {
            type = ModContent.ProjectileType<StartSwordAtkA>();
            InitialNextComboAttack(40, 2); //可接上第二段攻击时间间隔， 第二段攻击的前摇
        }
        if (CanNextAttack(2))//能进行第二段攻击
        {
            type = ModContent.ProjectileType<StartSwordAtkB>();
            damage = DamageMul(1.12f);
            InitialNextComboAttack(50, 8);
        }
        if (CanNextAttack(3))
        {
            type = ModContent.ProjectileType<StartSwordAtkC>();
            damage = DamageMul(1.3f);
            FinalComboAttack(26); //后摇
        }
        Item.shoot = type;
    }
}
public class QuickSword : DeadCellsItem
{
    public override void SetDefaults()
    {
        iconX = 4;
        iconY = 6;
        SetWeaponDefaults(BrutalityDamage.Instance, 4, 2f, 2, 2, 1);
    }
    /*
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (FirstAttack())
        {
            type = ModContent.ProjectileType<QuickSwordAtkA>();
            damage = DamageMul(playerComboAttack.QuickSwordAtkComboNum * 0.1f + 1.1f);
            InitialNextComboAttack(40, 12);
        }
        if (CanNextAttack(2))
        {
            type = ModContent.ProjectileType<QuickSwordAtkB>();
            damage = DamageMul(playerComboAttack.QuickSwordAtkComboNum * 0.1f + 1.4f);
            InitialNextComboAttack(50, 15);
        }
        if (CanNextAttack(3))
        {
            type = ModContent.ProjectileType<QuickSwordAtkA>();
            damage = DamageMul(playerComboAttack.QuickSwordAtkComboNum * 0.1f + 1.2f);
            InitialNextComboAttack(50, 12);
        }
        if (CanNextAttack(4))
        {
            type = ModContent.ProjectileType<QuickSwordAtkB>();
            damage = DamageMul(playerComboAttack.QuickSwordAtkComboNum * 0.1f + 1.4f);
            InitialNextComboAttack(50, 17);
        }
        if (CanNextAttack(5))
        {
            type = ModContent.ProjectileType<QuickSwordAtkC>();
            damage = DamageMul(playerComboAttack.QuickSwordAtkComboNum * 0.1f + 1f);
            InitialNextComboAttack(50, 10);
        }
        if (CanNextAttack(6))
        {
            type = ModContent.ProjectileType<QuickSwordAtkD>();
            damage = DamageMul(playerComboAttack.QuickSwordAtkComboNum * 0.1f + 1.6f);
            FinalComboAttack(30);
        }
    }
    */
}////
public class Bleeder : DeadCellsItem
{
    public override void SetDefaults()
    {
        iconX = 4;
        iconY = 1;
        SetWeaponDefaults(BrutalityDamage.Instance, 12, 2f, 2, 6, 1500);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        //大概就这么写。
        if (FirstAttack())
        {
            type = ModContent.ProjectileType<BleederAtkA>();
            InitialNextComboAttack(80, 4); //可接上第二段攻击时间间隔， 第二段攻击的前摇
        }
        if (CanNextAttack(2))//能进行第二段攻击
        {
            type = ModContent.ProjectileType<BleederAtkB>();
            damage = DamageMul(1.05f);
            FinalComboAttack(6); //后摇
        }
    }

}
public class DualDaggers : DeadCellsItem
{
    public override void SetDefaults()
    {
        iconX = 5;
        iconY = 1;
        SetWeaponDefaults(BrutalityDamage.Instance, 15, 2.4f, 2, 6, 1800);
    }
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (FirstAttack())
        {
            type = ModContent.ProjectileType<DualDaggersAtkA>();
            InitialNextComboAttack(30, 1); //可接上第二段攻击时间间隔， 第二段攻击的前摇
        }
        if (CanNextAttack(2))//能进行第二段攻击
        {
            type = ModContent.ProjectileType<DualDaggersAtkB>();
            InitialNextComboAttack(30, 1);
        }
        if (CanNextAttack(3))//能进行第二段攻击
        {
            type = ModContent.ProjectileType<DualDaggersAtkC>();
            damage = DamageMul(1.2f);
            FinalComboAttack(34); //后摇
        }
    }
}
public class BroadSword : DeadCellsItem
{
    public override void SetDefaults()
    {
        iconX = 7;
        iconY = 1;
        SetWeaponDefaults(SurvivalDamage.Instance, 16, 2.4f, 2, 6, 1750);
    }
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (FirstAttack())
        {
            type = ModContent.ProjectileType<BroadSwordAtkA>();
            InitialNextComboAttack(90, 18); //可接上第二段攻击时间间隔， 第二段攻击的前摇
        }
        if (CanNextAttack(2))//能进行第二段攻击
        {
            type = ModContent.ProjectileType<BroadSwordAtkB>();
            InitialNextComboAttack(85, 26);
        }
        if (CanNextAttack(3))//能进行第二段攻击
        {
            type = ModContent.ProjectileType<BroadSwordAtkC>();
            FinalComboAttack(48); //后摇
        }
    }
}

public class EvilSword : DeadCellsItem
{
    public override void SetDefaults()
    {
        iconX = 2;
        iconY = 3;
        SetWeaponDefaults(BrutalityDamage.Instance, 25, 2.1f, 2, 6, 2500);
    }
    public override void HoldItem(Player player)
    {
        // player.AddBuff(ModContent.BuffType<BeCursed>(), 2);
        base.HoldItem(player);
    }
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (FirstAttack())
        {
            // type = ModContent.ProjectileType<EvilSwordAtkA>();
            InitialNextComboAttack(30, 6); //可接上第二段攻击时间间隔， 第二段攻击的前摇
        }
        if (CanNextAttack(2))//能进行第二段攻击
        {
            // type = ModContent.ProjectileType<EvilSwordAtkB>();
            InitialNextComboAttack(30, 1);
        }
        if (CanNextAttack(3))//能进行第二段攻击
        {
            // type = ModContent.ProjectileType<EvilSwordAtkA>();
            damage = DamageMul(1.48f);
            FinalComboAttack(20); //后摇
        }
    }
}////
public class BleedCrit : DeadCellsItem
{
    public override void SetDefaults()
    {
        iconX = 3;
        iconY = 1;
        SetWeaponDefaults(BrutalityDamage.Instance, 20, 2f, 2, 6, 1500);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (FirstAttack())
        {
            type = ModContent.ProjectileType<BleedCritAtkA>();
            damage = DamageMul(0.8f);
            InitialNextComboAttack(74, 18); //可接上第二段攻击时间间隔， 第二段攻击的前摇
        }
        if (CanNextAttack(2))//能进行第二段攻击
        {
            type = ModContent.ProjectileType<BleedCritAtkB>();
            damage = DamageMul(0.65f);
            InitialNextComboAttack(40, 10);
        }
        if (CanNextAttack(3))
        {
            type = ModContent.ProjectileType<BleedCritAtkB>();
            damage = DamageMul(0.65f);
            InitialNextComboAttack(60, 15);
        }
        if (CanNextAttack(4))
        {
            type = ModContent.ProjectileType<BleedCritAtkC>();
            damage = DamageMul(1.2f);
            FinalComboAttack(30); //后摇
        }
    }


}

public class KingsSpear : DeadCellsItem
{
    public override void SetDefaults()
    {
        iconX = 11;
        iconY = 11;
        SetWeaponDefaults(SurvivalDamage.Instance, 18, 2f, 2, 6, 2000);
    }
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (FirstAttack())
        {
            type = ModContent.ProjectileType<KingsSpearAtkA>();
            InitialNextComboAttack(80, 12); //可接上第二段攻击时间间隔， 第二段攻击的前摇
        }
        if (CanNextAttack(2))//能进行第二段攻击
        {
            type = ModContent.ProjectileType<KingsSpearAtkB>();
            damage = DamageMul(1.3f);
            InitialNextComboAttack(80, 14);
        }
        if (CanNextAttack(3))
        {
            type = ModContent.ProjectileType<KingsSpearAtkC>();
            damage = DamageMul(1.95f);
            FinalComboAttack(48); //后摇
        }
    }
}

public class OilSword : DeadCellsItem
{
    public override void SetDefaults()
    {
        iconX = 7;
        iconY = 2;
        SetWeaponDefaults(BrutalityDamage.Instance, 30, 2f, 2, 6, 1500);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        //大概就这么写，一段一段从下往上。
        if (FirstAttack())
        {
            type = ModContent.ProjectileType<OilSwordAtkA>();
            InitialNextComboAttack(80, 14); //可接上第二段攻击时间间隔， 第二段攻击的前摇
        }
        if (CanNextAttack(2))//能进行第二段攻击
        {
            type = ModContent.ProjectileType<OilSwordAtkB>();
            damage = DamageMul(1.1f);
            FinalComboAttack(24);
        }
    }
}


public class LowHealth : DeadCellsItem
{
    public override void SetDefaults()
    {
        iconX = 17;
        iconY = 4;
        SetWeaponDefaults(BrutalityDamage.Instance, 14, 2f, 2, 6, 1500);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        //大概就这么写，一段一段从下往上。
        if (FirstAttack())
        {
            type = ModContent.ProjectileType<LowHealthAtkA>();
            InitialNextComboAttack(80, 14); //可接上第二段攻击时间间隔， 第二段攻击的前摇
        }
        if (CanNextAttack(2))//能进行第二段攻击
        {
            type = ModContent.ProjectileType<LowHealthAtkB>();
            InitialNextComboAttack(80, 20);
        }
        if (CanNextAttack(3))
        {
            type = ModContent.ProjectileType<LowHealthAtkC>();
            damage = DamageMul(1.25f);
            InitialNextComboAttack(80, 17);
        }
        if (CanNextAttack(4))
        {
            type = ModContent.ProjectileType<LowHealthAtkD>();
            damage = DamageMul(0.875f);
            InitialNextComboAttack(80, 18);
        }
        if (CanNextAttack(5))
        {
            type = ModContent.ProjectileType<LowHealthAtkE>();
            damage = DamageMul(1.6f);
            FinalComboAttack(24); //后摇
        }
    }
}
public class PerfectHalberd : DeadCellsItem
{
    public override void SetDefaults()
    {
        iconX = 11;
        iconY = 14;
        SetWeaponDefaults(SurvivalDamage.Instance, 18, 1.8f, 2, 6, 1750);
    }
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (FirstAttack())
        {
            type = ModContent.ProjectileType<PerfectHalberdA>();
            InitialNextComboAttack(90, 8); //可接上第二段攻击时间间隔， 第二段攻击的前摇
        }
        if (CanNextAttack(2))//能进行第二段攻击
        {
            type = ModContent.ProjectileType<PerfectHalberdB>();
            damage = DamageMul(1.05f);
            InitialNextComboAttack(90, 14);
        }
        if (CanNextAttack(3))
        {
            type = ModContent.ProjectileType<PerfectHalberdC>();
            damage = DamageMul(1.2f);
            InitialNextComboAttack(90, 10);
        }
        if (CanNextAttack(4))
        {
            type = ModContent.ProjectileType<PerfectHalberdD>();
            damage = DamageMul(2f);
            FinalComboAttack(36); //后摇
        }
    }

}

public class HeavyAxe : DeadCellsItem
{
    public override void SetDefaults()
    {
        iconX = 30;
        iconY = 6;
        SetWeaponDefaults(SurvivalDamage.Instance, 24, 2f, 2, 6, 1750);
    }
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (FirstAttack())
        {
            type = ModContent.ProjectileType<HeavyAxeAtkA>();
            InitialNextComboAttack(108, 4); //可接上第二段攻击时间间隔， 第二段攻击的前摇
        }
        if (CanNextAttack(2))//能进行第二段攻击
        {
            type = ModContent.ProjectileType<HeavyAxeAtkB>();
            damage = DamageMul(1.05f);
            InitialNextComboAttack(120, 16);
        }
        if (CanNextAttack(3))
        {
            type = ModContent.ProjectileType<HeavyAxeAtkC>();
            damage = DamageMul(1.1f);
            InitialNextComboAttack(124, 12);
        }
        if (CanNextAttack(4))
        {
            type = ModContent.ProjectileType<HeavyAxeAtkD>();
            damage = DamageMul(1.2f);
            playerComboAttack.TimeCanConsistentAttack = 124;
            playerComboAttack.ConsistentLockCtrlAfter = 2;

        }
    }
}

public class QueenRapier : DeadCellsItem
{
    public override void SetDefaults()
    {
        iconX = 33;
        iconY = 8;
        SetWeaponDefaults(BrutalityDamage.Instance, 25, 2.1f, 2, 6, 2500);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (FirstAttack())
        {
            type = ModContent.ProjectileType<QueenRapierAtkA>();
            InitialNextComboAttack(66, 22); //可接上第二段攻击时间间隔， 第二段攻击的前摇
        }
        if (CanNextAttack(2))//能进行第二段攻击
        {
            type = ModContent.ProjectileType<QueenRapierAtkB>();
            InitialNextComboAttack(66, 22);
        }
        if (CanNextAttack(3))//能进行第二段攻击
        {
            type = ModContent.ProjectileType<QueenRapierAtkC>();
            damage = DamageMul(1.48f);
            FinalComboAttack(38); //后摇
        }
    }
}

public class AdeleScythe : DeadCellsItem
{
    public override void SetDefaults()
    {
        iconX = 25;
        iconY = 10;
        SetWeaponDefaults(SurvivalDamage.Instance, 14, 1.6f, 2, 6, 2000);
    }
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (FirstAttack())
        {
            type = ModContent.ProjectileType<AdeleScytheAtkA>();
            InitialNextComboAttack(90, 8); //可接上第二段攻击时间间隔， 第二段攻击的前摇
        }
        if (CanNextAttack(2))//能进行第二段攻击
        {
            type = ModContent.ProjectileType<AdeleScytheAtkB>();
            damage = DamageMul(1.05f);
            InitialNextComboAttack(90, 10);
        }
        if (CanNextAttack(3))
        {
            type = ModContent.ProjectileType<AdeleScytheAtkC>();
            damage = DamageMul(1.2f);
            InitialNextComboAttack(90, 12);
        }
        if (CanNextAttack(4))
        {
            type = ModContent.ProjectileType<AdeleScytheAtkD>();
            damage = DamageMul(2f);
            FinalComboAttack(36); //后摇
        }
    }
}

public class CombinedTickScythe : DeadCellsItem
{
    public override void SetDefaults()
    {
        iconX = 32;
        iconY = 1;
        SetWeaponDefaults(SurvivalDamage.Instance, 24, 2f, 2, 6, 1500);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        //大概就这么写。
        if (FirstAttack())
        {
            type = ModContent.ProjectileType<TickScytheAtkB1>();//暴击原理写在B1里了
            damage = DamageMul(0.75f);
            InitialNextComboAttack(120, 8); //可接上第二段攻击时间间隔， 第二段攻击的前摇
        }
        if (CanNextAttack(2))//能进行第二段攻击
        {
            type = ModContent.ProjectileType<TickScytheAtkA2>();
            InitialNextComboAttack(160, 4);
        }
        if (CanNextAttack(3))
        {
            type = ModContent.ProjectileType<TickScytheAtkB2>();
            damage = DamageMul(0.88f);
            InitialNextComboAttack(120, 4);
        }
        if (CanNextAttack(4))
        {
            type = ModContent.ProjectileType<TickScytheAtkA1>();
            damage = DamageMul(1.2f);
            FinalComboAttack(8); //后摇
        }
    }


}
