using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using DeadCellsBossFight.Core;
using DeadCellsBossFight.NPCs.ExtraBosses.Queen;

namespace DeadCellsBossFight.Projectiles.NPCsProj;

public class QueenRepelBullets : ModProjectile
{
    public override string Texture => AssetsLoader.TransparentImg;
    public override void SetDefaults()
    {
        Projectile.width = 140;
        Projectile.height = 200;
        Projectile.timeLeft = 132;
    }
    // 效果见DCGlobalProj.cs
    public override void PostDraw(Color lightColor)
    {
        /*
        Vector2 pos = Projectile.position - Main.screenPosition;
        Rectangle rectangle = new Rectangle((int)pos.X, (int)pos.Y, Projectile.width, Projectile.height);
        Color color = new(100, 200, 100, 200);
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, rectangle, color);
        */
        base.PostDraw(lightColor);
    }
    public override void AI()
    {
        foreach (NPC npc in Main.ActiveNPCs)
        {
            // 更新弹幕位置，史
            if (npc.type == ModContent.NPCType<Queen>())
            {
                Projectile.Center = (npc.direction > 0 ? npc.Right : npc.Left) + new Vector2(140, 0) * npc.direction;
            }
        }
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return false;
    }
    public override bool? CanDamage()
    {
        return false;
    }
    public override bool? CanCutTiles()
    {
        return false;
    }
}
