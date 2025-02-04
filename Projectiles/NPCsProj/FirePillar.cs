using DeadCellsBossFight.Core;
using DeadCellsBossFight.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Projectiles.NPCsProj;

public class FirePillar : ModProjectile
{
    // ai[0]控制绘图
    // ai[1]控制向左生成弹幕
    // ai[2]控制向右生成弹幕
    public override void SetDefaults()
    {
        Projectile.width = 36;
        Projectile.height = 28;
        Projectile.tileCollide = false;
        Projectile.hostile = true;
        Projectile.timeLeft = 70;
    }

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.rotation = Main.rand.NextFloat(-0.12f, 0.12f);
        if (Projectile.ai[1] == 1)
        {
            Point left = Projectile.Left.ToTileCoordinates();
            Point bottomleft = Projectile.BottomLeft.ToTileCoordinates();
            for (int j = 0; j < 2; j++)
            {
                Tile tileAtPosition = NormalUtils.CheckTileInPosition(left.X - j, left.Y);
                if (tileAtPosition.HasTile && !Main.tileAxe[tileAtPosition.TileType] && !WorldGen.CanKillTile(left.X - j, left.Y)) // 弹幕左面有物块
                {
                    Projectile.ai[1] = 0;
                }
                Tile tileAtPosition2 = NormalUtils.CheckTileInPosition(bottomleft.X - j, bottomleft.Y + 1);
                if (!tileAtPosition2.HasTile) // 弹幕左下角没有物块
                {
                    Projectile.ai[1] = 0;
                }
            }
        }

        if (Projectile.ai[2] == 1)
        {
            Point right = Projectile.Right.ToTileCoordinates();
            Point bottomright = Projectile.BottomRight.ToTileCoordinates();
            for (int j = 0; j < 2; j++)
            {
                Tile tileAtPosition = NormalUtils.CheckTileInPosition(right.X + j, right.Y);
                if (tileAtPosition.HasTile && !Main.tileAxe[tileAtPosition.TileType] && !WorldGen.CanKillTile(right.X + j, right.Y))  // 弹幕右面有物块
                {
                    Projectile.ai[2] = 0;
                }
                Tile tileAtPosition2 = NormalUtils.CheckTileInPosition(bottomright.X + j, bottomright.Y + 1);
                if (!tileAtPosition2.HasTile) // 弹幕右下角没有物块
                {
                    Projectile.ai[2] = 0;
                }
            }
        }
    }
    public override void AI()
    {
        Lighting.AddLight(Projectile.Top, 0.5f, 0.5f, 0.2f);

        if (Projectile.timeLeft < 54)
            Projectile.ai[0] = 8 - Projectile.timeLeft / 6;
        if (Projectile.timeLeft == 65)
        {
            if (Projectile.ai[1] == 1)
            {
                Vector2 pos = Projectile.Left - new Vector2(16, 0);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, Vector2.Zero, Type, 40, 2, -1, 0, 1, 0);
            }

            if (Projectile.ai[2] == 1)
            {

                Vector2 pos = Projectile.Right + new Vector2(16, 0);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, Vector2.Zero, Type, 40, 2, -1, 0, 0, 1);
            }
        }
    }
    public override bool CanHitPlayer(Player target)
    {
        return Projectile.timeLeft > 35;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }
    public override void PostDraw(Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Rectangle rectangle = new((int)Projectile.ai[0] * 100, 0, 100, 169);
        SpriteBatch sb = Main.spriteBatch;

        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
        sb.Draw(texture, Projectile.Bottom - Main.screenPosition, rectangle, Color.White, Projectile.rotation, new(50f, 169f), 0.75f, SpriteEffects.None, 0f);
        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);


    }

}
