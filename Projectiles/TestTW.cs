using DeadCellsBossFight.Contents.SubWorlds;
using DeadCellsBossFight.Core;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Projectiles;

public class TestTW : ModProjectile
{
    public override string Texture => AssetsLoader.TransparentImg;
    public override void SetDefaults()
    {
        Projectile.width = 64;
        Projectile.height = 800;
        Projectile.timeLeft = 50;
        Projectile.ignoreWater = true;
    }
    public override void AI()
    {
        Projectile.velocity *= 0f;
        
        if (Projectile.timeLeft == 30)
        {
            if (SubworldSystem.IsActive<PrisonWorld>())
                SubworldSystem.Exit();
            else
                SubworldSystem.Enter<PrisonWorld>();
        }
        
    }

    public override void PostDraw(Color lightColor)
    {
        Vector2 pos = Projectile.position - Main.screenPosition;
        Rectangle rectangle = new Rectangle((int)pos.X, (int)pos.Y, Projectile.width, Projectile.height);
        Color color = new(200, 200, 200, 200);
        // Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, rectangle, color);
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, Vector2.Zero, new Rectangle?(new Rectangle(0, 0, 2500, 1200)), new(200, 40, 30, 120), 0f, Vector2.Zero, 1f, 0, 0f);

        base.PostDraw(lightColor);
    }

}
