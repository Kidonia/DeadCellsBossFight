using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using DeadCellsBossFight.Projectiles;
using DeadCellsBossFight.Projectiles.WeaponAnimationProj;

namespace DeadCellsBossFight.NPCs;

public partial class BH : ModNPC
{
    /// <summary>
    /// 处于攻击状态。
    /// </summary>
    public int atkTiming;

    /// <summary>
    /// 攻击之间的间隔时间。
    /// </summary>
    public int atkCoolDown;

    /// <summary>
    /// 阶段不同提高的效果
    /// </summary>
    public int stateBonus;

    /// <summary>
    /// 半近战重武攻击AI
    /// </summary>
    public void AI_HeavyAtk()
    {
        NPC.defense = 18;
        switch (CurrentWeaponType)
        {
            case BHWeaponType.HeavyAxe:
                HATK_HeavyAxe();
                break;
            case BHWeaponType.BroadSword:
                HATK_BroadSword();
                break;
            case BHWeaponType.KingsSpear: 
                HATK_KingsSpear();
                break;
            case BHWeaponType.PerfectHalberd:
                HATK_PerfectHalberd();
                break;
            case BHWeaponType.TickScythe:
                HATK_TickScythe();
                break;
            case BHWeaponType.AdeleScythe:
                HATK_AdeleScythe();
                break;
            default: 
                break;
        }

    }
    #region HeavyATKs中程重武的攻击方式
    public void HATK_HeavyAxe()
    {
        CheckAndActivateATKChain();
        if (CheckCanATK())
        {
            if (CurrentState == BHState.Fourth)
            {
                switch (CurrentATKChain)
                {
                    case BHATKChain.First:
                        AddAnimTimeAndCoolDown(85, 105, 170, ModContent.ProjectileType<HeavyAxeAtkD>(), 45, 4f);
                        break;
                    case BHATKChain.Second:
                        AddAnimTimeAndCoolDown(85, 105, 0, ModContent.ProjectileType<HeavyAxeAtkD>(), 45, 4f);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (CurrentATKChain)
                {
                    case BHATKChain.First:
                        AddAnimTimeAndCoolDown(55, 80, 140, ModContent.ProjectileType<HeavyAxeAtkA>(), 30, 2.8f);
                        break;
                    case BHATKChain.Second:
                        AddAnimTimeAndCoolDown(70, 85, 160, ModContent.ProjectileType<HeavyAxeAtkB>(), 28, 2.5f);
                        break;
                    case BHATKChain.Third:
                        AddAnimTimeAndCoolDown(80, 100, 210, ModContent.ProjectileType<HeavyAxeAtkC>(), 45, 4f);
                        break;
                    case BHATKChain.Fourth:
                        AddAnimTimeAndCoolDown(76, 110, 0, ModContent.ProjectileType<HeavyAxeAtkD>(), 45, 4f);
                        break;
                    default:
                        break;
                }

            }
        }
        if (CurrentState == BHState.Fourth)
        {
            CheckFinalWeaponState(BHATKChain.Second);
        }
        else
        {
            CheckFinalWeaponState(BHATKChain.Fourth);
        }

        AddBump(1.2f);
        CoolDownToWalk();
    }

    public void HATK_BroadSword()
    {
        CheckAndActivateATKChain();
        // 可以进行攻击
        if (CheckCanATK())
        {
            switch (CurrentATKChain)
            {
                case BHATKChain.First:
                    AddAnimTimeAndCoolDown(26, 55, 140, ModContent.ProjectileType<BroadSwordAtkA>(), 30, 3f);
                    break;
                case BHATKChain.Second:
                    AddAnimTimeAndCoolDown(32, 100, 200, ModContent.ProjectileType<BroadSwordAtkB>(), 45, 3.2f);
                    break;
                case BHATKChain.Third:
                    AddAnimTimeAndCoolDown(50, 130, 0, ModContent.ProjectileType<BroadSwordAtkC>(), 65, 4f);
                    break;
                default:
                    break;
            }
        }
        CheckFinalWeaponState(BHATKChain.Third);
        AddBump(0.85f);
        CoolDownToWalk();
    }

    public void HATK_KingsSpear()
    {
        CheckAndActivateATKChain();
        // 可以进行攻击
        if (CheckCanATK())
        {
            switch (CurrentATKChain)
            {
                case BHATKChain.First:
                    AddAnimTimeAndCoolDown(40, 70, 120, ModContent.ProjectileType<KingsSpearAtkA>(), 35, 3.2f);
                    break;
                case BHATKChain.Second:
                    AddAnimTimeAndCoolDown(21, 60, 200, ModContent.ProjectileType<KingsSpearAtkB>(), 30, 3f);
                    break;
                case BHATKChain.Third:
                    AddAnimTimeAndCoolDown(40, 130, 0, ModContent.ProjectileType<KingsSpearAtkC>(), 55, 3.8f);
                    break;
                default:
                    break;
            }
        }
        CheckFinalWeaponState(BHATKChain.Third);
        AddBump(1.2f);
        CoolDownToWalk();
    }

    public void HATK_PerfectHalberd()
    {
        CheckAndActivateATKChain();
        // 可以进行攻击
        if (CheckCanATK())
        {
            switch (CurrentATKChain)
            {
                case BHATKChain.First:
                    AddAnimTimeAndCoolDown(34, 60, 80, ModContent.ProjectileType<PerfectHalberdA>(), 50, 2.8f);
                    break;
                case BHATKChain.Second:
                    AddAnimTimeAndCoolDown(16, 70, 60, ModContent.ProjectileType<PerfectHalberdB>(), 45, 3.4f);
                    break;
                case BHATKChain.Third:
                    AddAnimTimeAndCoolDown(42, 60, 60, ModContent.ProjectileType<PerfectHalberdC>(), 45, 2.8f);
                    break;
                case BHATKChain.Fourth:
                    AddAnimTimeAndCoolDown(25, 125, 0, ModContent.ProjectileType<PerfectHalberdD>(), 60, 3.4f);
                    break;
                default:
                    break;
            }
        }
        CheckFinalWeaponState(BHATKChain.Fourth);
        AddBump(1f);
        CoolDownToWalk();
    }

    public void HATK_TickScythe()
    {
        CheckAndActivateATKChain();
        // 可以进行攻击
        if (CheckCanATK())
        {
            switch (CurrentATKChain)
            {
                case BHATKChain.First:
                    AddAnimTimeAndCoolDown(49, 100, 120, ModContent.ProjectileType<TickScytheAtkB1>(), 30, 3f);
                    break;
                case BHATKChain.Second:
                    AddAnimTimeAndCoolDown(77, 120, 160, ModContent.ProjectileType<TickScytheAtkA2>(), 85, 4f);
                    break;
                case BHATKChain.Third:
                    AddAnimTimeAndCoolDown(55, 100, 120, ModContent.ProjectileType<TickScytheAtkB2>(), 45, 3f);
                    break;
                case BHATKChain.Fourth:
                    AddAnimTimeAndCoolDown(69, 150, 0, ModContent.ProjectileType<TickScytheAtkA1>(), 100, 4f);
                    break;
                default:
                    break;
            }
        }
        CheckFinalWeaponState(BHATKChain.Fourth);
        AddBump(1.4f);
        CoolDownToWalk();
    }

    public void HATK_AdeleScythe()
    {
        CheckAndActivateATKChain();
        // 可以进行攻击
        if (CheckCanATK())
        {
            switch (CurrentATKChain)
            {
                case BHATKChain.First:
                    AddAnimTimeAndCoolDown(30, 60, 90, ModContent.ProjectileType<AdeleScytheAtkA>(), 30, 2f);
                    break;
                case BHATKChain.Second:
                    AddAnimTimeAndCoolDown(30, 60, 90, ModContent.ProjectileType<AdeleScytheAtkB>(), 28, 2f);
                    break;
                case BHATKChain.Third:
                    AddAnimTimeAndCoolDown(40, 70, 130, ModContent.ProjectileType<AdeleScytheAtkC>(), 32, 2.3f);
                    break;
                case BHATKChain.Fourth:
                    AddAnimTimeAndCoolDown(45, 150, 0, ModContent.ProjectileType<AdeleScytheAtkD>(), 30, 0.8f);
                    break;
                default:
                    break;
            }
        }
        CheckFinalWeaponState(BHATKChain.Fourth);
        AddBump(0.8f);
        CoolDownToWalk();
    }

    #endregion

    /// <summary>
    /// 近战剑类攻击AI
    /// </summary>
    public void AI_SwordAtk()
    {
        NPC.defense = 15;
        switch (CurrentWeaponType)
        {
            case BHWeaponType.StartSword:
                ATK_StartSword();
                break;
            case BHWeaponType.Bleeder:
                ATK_Bleeder();
                break;
            case BHWeaponType.OilSword:
                ATK_OilSword();
                break;
            case BHWeaponType.BleedCrit:
                ATK_BleedCrit();
                break;
            case BHWeaponType.DualDaggers:
                ATK_DualDaggers();
                break;
            case BHWeaponType.LowHealth:
                ATK_LowHealth();
                break;
            case BHWeaponType.QueenRapier:
                ATK_QueenRapier();
                break;
            default:
                ATK_StartSword();
                break;
        }
        NPC.defense = 3;
    }
    #region SwordATKs近战剑类武器的攻击方式

    public void ATK_StartSword()
    {
        CheckAndActivateATKChain();
        // 可以进行攻击
        if (CheckCanATK())
        {
            switch (CurrentATKChain)
            {
                case BHATKChain.First:
                    AddAnimTimeAndCoolDown(15, 45, 55, ModContent.ProjectileType<StartSwordAtkA>(), 12, 1f);
                    break;
                case BHATKChain.Second:
                    AddAnimTimeAndCoolDown(20, 50, 60, ModContent.ProjectileType<StartSwordAtkB>(), 10, 0.8f);
                    break;
                case BHATKChain.Third:
                    AddAnimTimeAndCoolDown(20, 105, 0, ModContent.ProjectileType<StartSwordAtkC>(), 25, 1.8f);
                    break;
                default:
                    break;
            }
        }
        CheckFinalWeaponState(BHATKChain.Third);

        // 攻击时添加微小位移
        AddBump(0.9f);

        // 处于攻击完成后的冷却状态，转为行走模式
        CoolDownToWalk();
    }

    public void ATK_Bleeder()
    {
        CheckAndActivateATKChain();
        // 可以进行攻击
        if (CheckCanATK())
        {
            switch (CurrentATKChain)
            {
                case BHATKChain.First:
                    AddAnimTimeAndCoolDown(20, 55, 85, ModContent.ProjectileType<BleederAtkA>(), 26, 1.8f);
                    break;
                case BHATKChain.Second:
                    AddAnimTimeAndCoolDown(15, 110, 0, ModContent.ProjectileType<BleederAtkB>(), 30, 2f);
                    break;
                default:
                    break;
            }
        }
        CheckFinalWeaponState(BHATKChain.Second);
        // 攻击时添加微小位移
        AddBump(1.6f);
        // 处于攻击完成后的冷却状态，转为行走模式
        CoolDownToWalk();
    }

    public void ATK_OilSword()
    {
        CheckAndActivateATKChain();
        // 可以进行攻击
        if (CheckCanATK())
        {
            switch (CurrentATKChain)
            {
                case BHATKChain.First:
                    AddAnimTimeAndCoolDown(20, 55, 80, ModContent.ProjectileType<OilSwordAtkA>(), 55, 2);
                    break;
                case BHATKChain.Second:
                    AddAnimTimeAndCoolDown(20, 115, 0, ModContent.ProjectileType<OilSwordAtkB>(), 60, 2);
                    break;
                default:
                    break;
            }
        }
        CheckFinalWeaponState(BHATKChain.Second);
        // 攻击时添加微小位移
        AddBump(1.6f);
        // 处于攻击完成后的冷却状态，转为行走模式
        CoolDownToWalk();
    }

    public void ATK_BleedCrit()
    {
        CheckAndActivateATKChain();
        // 可以进行攻击
        if (CheckCanATK())
        {
            switch (CurrentATKChain)
            {
                case BHATKChain.First:
                    AddAnimTimeAndCoolDown(18, 50, 80, ModContent.ProjectileType<BleedCritAtkA>(), 50, 2);
                    break;
                case BHATKChain.Second:
                    AddAnimTimeAndCoolDown(12, 15, 15, ModContent.ProjectileType<BleedCritAtkB>(), 42, 2);
                    break;
                case BHATKChain.Third:
                    AddAnimTimeAndCoolDown(12, 40, 60, ModContent.ProjectileType<BleedCritAtkB>(), 42, 2);
                    break;
                case BHATKChain.Fourth:
                    AddAnimTimeAndCoolDown(22, 115, 0, ModContent.ProjectileType<BleedCritAtkC>(), 60, 3.6f);
                    break;
                default:
                    break;
            }
        }
        CheckFinalWeaponState(BHATKChain.Fourth);
        // 攻击时添加微小位移
        AddBump(0.4f);
        // 处于攻击完成后的冷却状态，转为行走模式
        CoolDownToWalk();
    }

    public void ATK_DualDaggers()
    {
        CheckAndActivateATKChain();
        // 可以进行攻击
        if (CheckCanATK())
        {
            switch (CurrentATKChain)
            {
                case BHATKChain.First:
                    AddAnimTimeAndCoolDown(20, 45, 60, ModContent.ProjectileType<DualDaggersAtkA>(), 18, 1f);
                    break;
                case BHATKChain.Second:
                    AddAnimTimeAndCoolDown(20, 50, 55, ModContent.ProjectileType<DualDaggersAtkB>(), 15, 0.8f);
                    break;
                case BHATKChain.Third:
                    AddAnimTimeAndCoolDown(25, 105, 0, ModContent.ProjectileType<DualDaggersAtkC>(), 26, 2f);
                    break;
                default:
                    break;
            }
        }
        CheckFinalWeaponState(BHATKChain.Third);
        AddBump(0.9f);//较大的bump写在Projectile的AI里了
        CoolDownToWalk();
    }

    public void ATK_LowHealth()
    {
        CheckAndActivateATKChain();
        // 可以进行攻击
        if (CheckCanATK())
        {
            switch (CurrentATKChain)
            {
                case BHATKChain.First:
                    AddAnimTimeAndCoolDown(20, 45, 50, ModContent.ProjectileType<LowHealthAtkA>(), 28, 1f);
                    break;
                case BHATKChain.Second:
                    AddAnimTimeAndCoolDown(18, 35, 45, ModContent.ProjectileType<LowHealthAtkB>(), 20, 0.8f);
                    break;
                case BHATKChain.Third:
                    AddAnimTimeAndCoolDown(15, 30, 40, ModContent.ProjectileType<LowHealthAtkC>(), 18, 2f);
                    break;
                case BHATKChain.Fourth:
                    AddAnimTimeAndCoolDown(15, 35, 80, ModContent.ProjectileType<LowHealthAtkD>(), 24, 2f);
                    break;
                case BHATKChain.Fifth:
                    AddAnimTimeAndCoolDown(20, 120, 0, ModContent.ProjectileType<LowHealthAtkE>(), 30, 2f);
                    break;
                default:
                    break;
            }
        }
        CheckFinalWeaponState(BHATKChain.Fifth);
        AddBump(0.6f);
        CoolDownToWalk();
    }

    public void ATK_QueenRapier()
    {
        CheckAndActivateATKChain();
        // 可以进行攻击
        if (CheckCanATK())
        {
            switch (CurrentATKChain)
            {
                case BHATKChain.First:
                    AddAnimTimeAndCoolDown(26, 60, 90, ModContent.ProjectileType<QueenRapierAtkA>(), 20, 0);
                    break;
                case BHATKChain.Second:
                    AddAnimTimeAndCoolDown(26, 60, 90, ModContent.ProjectileType<QueenRapierAtkB>(), 20, 0);
                    break;
                case BHATKChain.Third:
                    AddAnimTimeAndCoolDown(26, 125, 0, ModContent.ProjectileType<QueenRapierAtkC>(), 20, 0);
                    break;
                default:
                    break;
            }
        }
        CheckFinalWeaponState(BHATKChain.Third);
        CoolDownToWalk();
    }

    #endregion


    /// <summary>
    /// 确定该次攻击时间，下次攻击间隔，追逐玩家时间上限（最后一段攻击不要加）,生成弹幕类型，弹幕伤害， 弹幕击退，攻击段数增加
    /// </summary>
    /// <param name="atkTimingAdd"></param>
    /// <param name="atkCoolDownAdd"></param>
    /// <param name="projectile"></param>
    public void AddAnimTimeAndCoolDown(int atkTimingAdd, int atkCoolDownAdd, int chaseTimeAdd = 0, int projectile = 0, int damage = 0, float knockback = 0)
    {
        //随机连续攻击
        if ((distance < 100 && Main.rand.NextFloat(0, 1f) < 0.8f) || (distance < 160 && Main.rand.NextFloat(0, 1f) < 0.38f) || Main.rand.NextFloat(0, 1f) < 0.15f / (float)CurrentATKChain)
        {
            atkCoolDownAdd = atkTimingAdd;
            chaseTimeAdd = atkTimingAdd;
        }
        //添加攻击持续时间
        atkTiming += atkTimingAdd;

        //添加攻击间隔冷却
        atkCoolDown += (atkCoolDownAdd - stateBonus);

        //添加追赶玩家时间上限
        chasingTime += chaseTimeAdd;

        //生成弹幕
        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, projectile, damage, knockback, -1, DrawTrail);

        CurrentATKChain++;
    }


    /// <summary>
    /// 处于攻击完成后的冷却状态，转为行走模式
    /// </summary>
    public void CoolDownToWalk()
    {
        if (atkTiming == 0 && atkCoolDown > 0)
        {
            if (X_distance < 64f)
                ChangeMove(BHMoveType.Idle);
            else
                ChangeMove(BHMoveType.Walk);
        }
    }

    /// <summary>
    /// 处理攻击的最后一段，清零攻击状态
    /// </summary>
    /// <param name="finalChain"></param>
    public void CheckFinalWeaponState(BHATKChain finalChain)
    {
        if (CurrentATKChain > finalChain && atkTiming == 0)
            ClearWeaponState();
    }
    public void CheckAndActivateATKChain()
    {
        if (CheckCanATK())
        {
            if (CurrentATKChain == BHATKChain.NOATK)
            {
                CurrentATKChain = BHATKChain.First;
            }
        }

    }


    /// <summary>
    /// 添加微小位移，受当前细胞人阶段影响
    /// </summary>
    /// <param name="strength"></param>
    public void AddBump(float strength)
    {
        if(atkTiming > 0)
            NPC.velocity.X = strength * NPC.direction * (int)CurrentState;
    }

    /// <summary>
    /// 检测能否进行攻击。
    /// 攻击状态时间为0，攻击间隔冷却为0，
    /// </summary>
    /// <returns></returns>
    public bool CheckCanATK()
    {
        return atkTiming == 0 && atkCoolDown == 0;
    }

    /// <summary>
    /// 清除细胞人攻击状态
    /// </summary>
    public void ClearWeaponState()
    {
        if (atkTiming == 0)
        {
            CurrentWeaponType = BHWeaponType.NoWeapon;
            CurrentWeaponAtkMode = BHWeaponAtkMode.None;
            CurrentATKChain = BHATKChain.NOATK;

            //清除攻击状态，可以走路跳跃啥的，不要动武器的冷却，让它自己流逝
            if (X_distance < 50f)
                ChangeMove(BHMoveType.Idle);
            else
                ChangeMove(BHMoveType.Walk);
        }
    }
    public void ShowATKMessage()
    {
        Main.NewText($"当前攻击模式为{CurrentWeaponAtkMode}，使用的武器为{CurrentWeaponType}，为第{(int)CurrentATKChain - 1}段攻击。");
    }

    public int addTestProj()
    {
        return Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TestTW>(), 5, 0);
    }
}
