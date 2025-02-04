using Microsoft.Xna.Framework;
using SubworldLibrary;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Contents.GlobalChanges;

public class DCGlobalItem :GlobalItem
{
    public override void ModifyHitNPC(Item item, Player player, NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(item, player, target, ref modifiers);
    }
    public override bool CanUseItem(Item item, Player player)
    {
        if (SubworldSystem.AnyActive())
        {
            int itemID = item.type;
            bool isSponge = itemID == ItemID.SuperAbsorbantSponge || itemID == ItemID.LavaAbsorbantSponge || itemID == ItemID.HoneyAbsorbantSponge || itemID == ItemID.UltraAbsorbantSponge;
            bool isRegularBucket = itemID == ItemID.EmptyBucket || itemID == ItemID.WaterBucket || itemID == ItemID.LavaBucket || itemID == ItemID.HoneyBucket;
            bool isSpecialBucket = itemID == ItemID.BottomlessBucket || itemID == ItemID.BottomlessLavaBucket || itemID == ItemID.BottomlessHoneyBucket || itemID == ItemID.BottomlessShimmerBucket;
            return !isSponge && !isRegularBucket && !isSpecialBucket && itemID != ItemID.CelestialSigil;
        }
        return base.CanUseItem(item, player);
    }

    public override bool CanPickup(Item item, Player player)
    {
        return base.CanPickup(item, player);
    }
}
