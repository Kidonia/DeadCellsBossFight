using System.Collections.Generic;
using Terraria.ModLoader;
using DeadCellsBossFight.Projectiles;
using DeadCellsBossFight.Projectiles.WeaponAnimationProj;

namespace DeadCellsBossFight.NPCs;


public enum BHMoveType
{
    None,
    SwordAtk, // 血刀，油刀，双匕首
    HeavyAtk, // 巨镰，死神巨镰，牙签，
    BowAtk, // 弓箭
    CrossbowAtk, // 弩箭
    HeadAtk, // 飞头
    ShieldBlock, // 盾反
    Stun, // 眩晕
    Heal, // 回血
    HideAndSummon, // 召唤别的Boss打架
    Roll, // 翻滚
    Walk, // 行走
    Jump, // 跳跃
    DoubleJump,
    Idle,
    SmashDown, // 下砸
    ClimbWallAndSmashDown
}
public enum BHState
{
    Begin,
    First,
    Second,
    Third,
    Fourth
}

public enum BHWeaponAtkMode
{
    None,
    SwordAtk, // 血刀，油刀，双匕首
    HeavyAtk, // 巨镰，死神巨镰，牙签，
    BowAtk, // 弓箭
    CrossbowAtk, // 弩箭
    HeadAtk // 飞头
}
/// <summary>
/// 一般会快速跳过0和1，一般展示的是下一段攻击是第几段。
/// </summary>
public enum BHATKChain
{
    NOATK,
    First,
    Second,
    Third,
    Fourth,
    Fifth,
    Sixth,
    Seventh//多写点避免出错
}
public enum BHWeaponType
{
    NoWeapon,
    // 剑
    StartSword,
    Bleeder,
    OilSword,
    BleedCrit,
    DualDaggers,
    LowHealth,
    QueenRapier,

    //重武
    HeavyAxe,
    BroadSword,
    KingsSpear,
    PerfectHalberd,
    TickScythe,
    AdeleScythe,

    //弓
    //弩
}
public enum BHSummonType
{
    Behemoth,
    Beholder,
    MamaTick,
    Queen
}
