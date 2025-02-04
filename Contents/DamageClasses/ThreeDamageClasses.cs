using Terraria;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Contents.DamageClasses;

// 三种流派都在这里
public class BrutalityDamage : DamageClass
{
    internal static BrutalityDamage Instance;

    public override void Load()
    {
        Instance = this;
    }

    public override void Unload()
    {
        Instance = null;
    }
    public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
    {
        if (damageClass == Generic)
            return StatInheritanceData.Full;

        if (damageClass == Melee)
        {
            return new StatInheritanceData(
                damageInheritance: 0.4f,
                critChanceInheritance: 0f,
                attackSpeedInheritance: 0f,
                armorPenInheritance: 0.8f,
                knockbackInheritance: 0.1f
            );
        };

        if (damageClass == MeleeNoSpeed)
        {
            return new StatInheritanceData(
                damageInheritance: 0.4f,
                critChanceInheritance: 0f,
                attackSpeedInheritance: 0f,
                armorPenInheritance: 0.8f,
                knockbackInheritance: 0.1f
            );
        };
        return StatInheritanceData.None;
    }

    public override bool GetEffectInheritance(DamageClass damageClass)
    {
        if (damageClass == Melee)
            return true;
        if (damageClass == MeleeNoSpeed)
            return true;

        return false;
    }

    public override void SetDefaultStats(Player player)
    {
        player.GetCritChance<BrutalityDamage>() = 0;
    }

    public override bool UseStandardCritCalcs => false;
}
public class SurvivalDamage : DamageClass
{
    internal static SurvivalDamage Instance;

    public override void Load()
    {
        Instance = this;
    }

    public override void Unload()
    {
        Instance = null;
    }
    public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
    {
        if (damageClass == Generic)
            return StatInheritanceData.Full;

        if (damageClass == Melee)
        {
            return new StatInheritanceData(
                damageInheritance: 0.2f,
                critChanceInheritance: 0f,
                attackSpeedInheritance: 0f,
                armorPenInheritance: 0.6f,
                knockbackInheritance: 0f
            );
        };

        if (damageClass == DamageClass.Summon)
        {
            return new StatInheritanceData(
                damageInheritance: 0.8f,
                critChanceInheritance: 0f,
                attackSpeedInheritance: 0f,
                armorPenInheritance: 0.4f,
                knockbackInheritance: 0f
            );
        };
        return StatInheritanceData.None;
    }

    public override bool GetEffectInheritance(DamageClass damageClass)
    {
        return false;
    }

    public override void SetDefaultStats(Player player)
    {
        player.GetCritChance<SurvivalDamage>() = 0;
    }

    public override bool UseStandardCritCalcs => false;


}
public class TacticsDamage : DamageClass
{
    internal static TacticsDamage Instance;

    public override void Load()
    {
        Instance = this;
    }

    public override void Unload()
    {
        Instance = null;
    }
    public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
    {
        if (damageClass == Generic)
            return StatInheritanceData.Full;

        if (damageClass == Ranged)
        {
            return new StatInheritanceData(
                damageInheritance: 0.4f,
                critChanceInheritance: 0f,
                attackSpeedInheritance: 0f,
                armorPenInheritance: 0.6f,
                knockbackInheritance: 0.8f
            );
        };

        if (damageClass == DamageClass.Magic)
        {
            return new StatInheritanceData(
                damageInheritance: 0.2f,
                critChanceInheritance: 0f,
                attackSpeedInheritance: 0f,
                armorPenInheritance: 0.4f,
                knockbackInheritance: 0.1f
            );
        };
        return StatInheritanceData.None;
    }

    public override bool GetEffectInheritance(DamageClass damageClass)
    {
        if (damageClass == Ranged)
            return true;

        return false;
    }

    public override void SetDefaultStats(Player player)
    {
        player.GetCritChance<TacticsDamage>() = 0;
    }

    public override bool UseStandardCritCalcs => false;



}
