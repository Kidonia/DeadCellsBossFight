using DeadCellsBossFight.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Projectiles.NPCsProj;
public class BeheAtkArea : ModProjectile
{
    public override string Texture => AssetsLoader.TransparentImg;
    public override void SetDefaults()
    {
        Projectile.width = 180;
        Projectile.height = 120;
        Projectile.hostile = true;
        Projectile.timeLeft = 4;
    }
    public override void PostDraw(Color lightColor)
    {
        /*
        Vector2 pos = Projectile.position - Main.screenPosition;
        Rectangle rectangle = new Rectangle((int)pos.X, (int)pos.Y, Projectile.width, Projectile.height);
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, rectangle, new(0, 250, 0));//画碰撞箱
        */
        base.PostDraw(lightColor);
    }


}
