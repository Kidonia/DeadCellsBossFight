using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;


namespace DeadCellsBossFight.Utils;

public static class PlayerExtensions
{
    public static bool IsLocal(this Player player)
    {
        return player.whoAmI == Main.myPlayer;
    }

    public static bool OnGround(this Player player)
    {
        return player.velocity.Y == 0f && Main.tile[player.Bottom.ToTileCoordinates()].HasTile;
    }

    public static bool WasOnGround(this Player player)
    {
        return player.oldVelocity.Y == 0f;
    }

    public static bool IsUnderwater(this Player player)
    {
        return Collision.DrownCollision(player.position, player.width, player.height, player.gravDir);
    }

    public static Vector2 KeyDirection(this Player player)
    {
        Vector2 result = default(Vector2);
        result.X = (player.controlRight ? 1f : 0f) - (player.controlLeft ? 1f : 0f);
        result.Y = (player.controlDown ? 1f : 0f) - (player.controlUp ? 1f : 0f);
        return result;
    }


    public static void RemoveBuffsOfType(this Player player, int type)
    {
        int buffIndex = player.FindBuffIndex(type);
        if (buffIndex >= 0)
        {
            player.DelBuff(buffIndex);
        }
    }

    public static bool HasEquipment(this Player player, int itemId)
    {
        for (int i = 0; i < 5; i++)
        {
            Item item = player.miscEquips[i];
            if(item != null && item.active && item.type == itemId)
            {
                return true;
            }
        }
        return false;
    }
    public static bool HasAccessory(this Player player, int itemId)
    {
        return player.EnumerateAccessories().Any(((Item item, int index) tuple) => tuple.item.type == itemId);
    }

    public static bool HasAccessory(this Player player, bool any, params int[] itemIds)
    {
        IEnumerable<(Item, int)> accessories = player.EnumerateAccessories();
        if (!any)
        {
            return accessories.All(Predicate);
        }
        return accessories.Any(Predicate);
        bool Predicate((Item, int) tuple)
        {
            return itemIds.Contains(tuple.Item1.type);
        }
    }

    public static IEnumerable<(Item item, int index)> EnumerateAccessories(this Player player)
    {
        for (int i = 3; i < 10; i++)
        {
            Item item = player.armor[i];
            if (item != null && item.active)
            {
                yield return (item, i);
            }
        }
    }

    public static void StopGrappling(this Player player, Projectile? exceptFor = null)
    {
        foreach (var (grapplingHook, hookIndex) in player.EnumerateGrapplingHooks())
        {
            if (grapplingHook != exceptFor && grapplingHook.ai[0] == 2f)
            {
                grapplingHook.Kill();
                if (hookIndex >= 0 && hookIndex < player.grappling.Length)
                {
                    player.grappling[hookIndex] = -1;
                }
            }
        }
    }

    public static IEnumerable<(Projectile projectile, int hookIndex)> EnumerateGrapplingHooks(this Player player)
    {
        for (int i = 0; i < Main.projectile.Length; i++)
        {
            Projectile proj = Main.projectile[i];
            if (proj != null && proj.active && proj.aiStyle == 7 && proj.owner == player.whoAmI)
            {
                yield return (proj, i);
            }
        }
    }

    /// <summary>
    /// 该段攻击为暴击，默认变成原来的两倍（即伤害*2）参数bonus为变成几倍，同时也有一定的击退效果
    /// </summary>
    /// <param name="modifiers"></param>
    /// <param name="bonus"></param>
    public static void SetCrit(this Player.HurtModifiers modifiers, float bonus = 2f)
    {
        modifiers.FinalDamage *= bonus;
        modifiers.KnockbackImmunityEffectiveness *= 0.9f;
        
    }
}
