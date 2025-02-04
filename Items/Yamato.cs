using DeadCellsBossFight.Contents.GlobalChanges;
using DeadCellsBossFight.Projectiles; // => YamatoHeldProj
using DeadCellsBossFight.Projectiles.EffectProj; // => MirrorScreenBroken
using DeadCellsBossFight.Utils; // => OnGround写在下面了，简陋的很
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Items;

public class Yamato : ModItem
{
    public int heldProjType;
    public override void SetDefaults()
    {
        Item.damage = 59160153; // EZ常数
        Item.DamageType = DamageClass.Default;
        Item.width = 58;
        Item.height = 58;
        Item.useTime = 60;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.value = 10000;
        Item.rare = ItemRarityID.Cyan;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.useAnimation = 25;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.shoot = ModContent.ProjectileType<MirrorScreenBroken>();
        heldProjType = ModContent.ProjectileType<YamatoHeldProj>();
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectileDirect(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<MirrorScreenBroken>(), 0, knockback, -1, 1);
        return false;
    }
    public override void HoldItem(Player player)
    {
        if (heldProjType > 0)
        {
            if (player.ownedProjectileCounts[heldProjType] == 0 && Main.myPlayer == player.whoAmI)
            {
                DCPlayer.yamadoExtraDrawRotation = 0;
                DCPlayer.yamadoHeldProj = Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, heldProjType, 0, 0, player.whoAmI);
            }
        }
        // player.direction = Math.Sign(Main.MouseWorld.X - player.Center.X);
    }
    public override bool CanUseItem(Player player)
    {
        return (!player.mount.Active && player.OnGround() && !player.pulley && !player.CCed);
    }
    /*抄的
    public static bool OnGround(this Player player)
    {
        return player.velocity.Y == 0f;
    }
     */

}
