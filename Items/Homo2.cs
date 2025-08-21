using DeadCellsBossFight.Contents.Biomes.Prison;
using DeadCellsBossFight.Core;
using DeadCellsBossFight.Projectiles;
using DeadCellsBossFight.Projectiles.NPCsProj;
using DeadCellsBossFight.Utils;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
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

            //var dic1 = AssetsLoader.BHanimAtlas["travolta"];
            //var dic2 = AssetsLoader.BHanimAtlas["travoltaIdle"];
            //var dic = NormalUtils.MergeTwoAnimDictionaries(dic1, dic2);
            //showdickey(dic1);
            //showdickey(dic2);
            //Main.NewText("");
            //showdickey(dic);

            //Main.NewText("---end---");
            //int idx2 = 7;
            //Main.NewText(dic2[idx2].name.ToString() +" " +  dic[idx2+dic1.Keys.Max()+1].name.ToString());

        }
        //public void showdickey(Dictionary<int, DCAnimPic> dic)
        //{
        //    Main.NewText(dic.Count);
        //    string aa = "";
        //    int k = 60;
        //    foreach (int n in dic.Keys)
        //    {
        //        k--;
        //        aa += $" {n}";
        //        if (k == 0)
        //        {
        //            Main.NewText($"{aa}");
        //            k = 60;
        //            aa = "";
        //        }
        //    }
        //    Main.NewText($"{aa}");
        //}
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}