using DeadCellsBossFight.Contents.Biomes.Prison;
using DeadCellsBossFight.Core;
using DeadCellsBossFight.Projectiles;
using DeadCellsBossFight.Projectiles.NPCsProj;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Items
{
    public class Homo2 : ModItem
    {
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.DeadCellsBossFight.hjson file.
        public override string Texture => AssetsLoader.WhiteDotImg;

        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 60;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            //Item.shoot = ModContent.ProjectileType<TestTW>();
            Item.shoot = ModContent.ProjectileType<QueenParryArea>();
            Item.autoReuse = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<testglow>(), 0, knockback, -1, 1);
            DCWorldSystem.ChangeToPrisonSky2 = !DCWorldSystem.ChangeToPrisonSky2;
            return false;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}