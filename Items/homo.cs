using DeadCellsBossFight.Core;
using DeadCellsBossFight.Projectiles;
using DeadCellsBossFight.Projectiles.BasicAnimationProj;
using DeadCellsBossFight.Projectiles.EffectProj;
using DeadCellsBossFight.Projectiles.NPCsProj;
using DeadCellsBossFight.Utils;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Items
{
	public class homo : ModItem
	{
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.DeadCellsBossFight.hjson file.

		public override void SetDefaults()
		{
			Item.damage = 1;
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
			//��һ��
			//Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, -1, 5, MathHelper.ToRadians(Main.rand.NextFloat(12, 168)), 1);
			//�ڶ���
			//Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, -1, 10, MathHelper.ToRadians(Main.rand.NextFloat(12, 168)), 2);
			//������ 
			//Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, -1, 20, MathHelper.ToRadians(Main.rand.NextFloat(12, 168)), 3);

/*			if (DeadCellsBossFight.Instance.TryFind<ModItem>("StartSworsd", out ModItem modItem))
			{
				int a = modItem.Type;
				Main.NewText(a);
				Main.NewText("StartSword");
			}
			else
			{
				var p = BottleSystem.bottleItemTypes.Where(k => k > 0);
				Main.NewText(p.Count());

				foreach(var t in p)
					Main.NewText(t);
			}*/
            Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<TestTW>(), 0, knockback, -1, 1);

            //��Ļ�� + �ж� + ���� + �� + ����Ů��
            //Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<DCScreenDrug>(), 0, knockback, -1, player.direction);

            /*
			var proj = Projectile.NewProjectileDirect(source, Main.MouseWorld - new Vector2(300, 300), Vector2.Zero, ModContent.ProjectileType<��ģ��>(), 0, knockback, -1, 157, 31, 0.8f);
			// 0 Ϊ���У�1Ϊ����
			proj.localAI[0] = 0;
			proj.localAI[2] = n;

            float n = 1f;
            var proj2 = Projectile.NewProjectileDirect(source, Main.MouseWorld - new Vector2(300, 300), Vector2.Zero, ModContent.ProjectileType<��ģ>(), 0, knockback, -1, 157, 53, 0.7f);
			// 0 Ϊ���У�1Ϊ����
			proj2.localAI[0] = 1;
            proj2.localAI[2] = n;
			

			for (; n < 0.5f;  n += 0.1f )
			{
                var proj = Projectile.NewProjectileDirect(source, Main.MouseWorld + new Vector2((n ) * 50 * 78, 300), Vector2.Zero, ModContent.ProjectileType<��ģ��>(), 0, knockback, -1, 157, 53, 0.09f + n);
                // 0 Ϊ���У�1Ϊ����
                proj.localAI[0] = 1;
                proj.localAI[2] = n;
            }
			*/
            /*
            string a = "As for how you die... You're going to be by someone who looks almost the same as you... Piercing with a spear from behind... Or in front of your most important people... How pathetic...";
			string b = "�����������...��ᱻһ�����㳤�ü���һ������...�ӱ����ó�ǹ�ᴩ����...������������Ҫ֮����ǰ...��ô�ɱ�...";
			a = "How pathetic...";
            DCWorldSystem.singleDialogueBox = new(a
                , Main.MouseWorld - Main.screenPosition
				, AssetsLoader.boxCollecorDialog
				, 2
				, new(0, 0.8f, 1)
				, Color.White
				,AssetsLoader.CastleVaniaFont
				);
			*/

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