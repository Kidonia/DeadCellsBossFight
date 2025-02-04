using DeadCellsBossFight.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Projectiles;

public class QueenRapierCut : ModProjectile
{
    //ai[0]是旋转角度，ai[1]是朝向1和-1
    public override string Texture => AssetsLoader.TransparentImg;
    public Player player => Main.player[Projectile.owner];

    public override void SetDefaults()
    {
        Projectile.width = 420;
        Projectile.height = 14;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 20;
        Projectile.ignoreWater = true;
    }
    public override void OnSpawn(IEntitySource source)
    {
        Projectile.rotation = Projectile.ai[0];
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override void AI()
    {
        //Main.NewText(Projectile.timeLeft);
        if (Projectile.timeLeft == 8)
        {
            /*s
            foreach (NPC npc in Main.npc)
            {
                if (Collision.CheckAABBvLineCollision(npc.TopLeft, npc.Size, Projectile.Center + (Projectile.Left - Projectile.Center).RotatedBy(Projectile.ai[0]), Projectile.Center + (Projectile.Right - Projectile.Center).RotatedBy(Projectile.ai[0])))
                {
                    Projectile.NewProjectile(Entity.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<QueenRapierCritCut>(), 14, 0, player.whoAmI, 0, Projectile.ai[0]);
                    
                }
            }
            */
            if (Collision.CheckAABBvLineCollision(player.TopLeft, player.Size, Projectile.Center + (Projectile.Left - Projectile.Center).RotatedBy(Projectile.ai[0]), Projectile.Center + (Projectile.Right - Projectile.Center).RotatedBy(Projectile.ai[0])))
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<QueenRapierCritCut>(), 100, 0, player.whoAmI, 0, Projectile.ai[0]);

            }
        }

        if (Projectile.timeLeft < 8)
            Projectile.alpha += 22;
        base.AI();
    }
    public override void PostDraw(Color lightColor)
    {
        SpriteBatch sb = Main.spriteBatch;
        Texture2D texture = ModContent.Request<Texture2D>("DeadCellsBossFight/Projectiles/QueenRapierLine", (AssetRequestMode)1).Value;
        sb.End();
        sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

        sb.Draw(texture,
                    Projectile.Center - Main.screenPosition,
                    new Rectangle(0, 0, 16, 7),
                    Color.White,
                    Projectile.rotation,
                    new Vector2(8, 3.5f),
                    new Vector2(25, 1),
                    SpriteEffects.None,
                    0);
        sb.End();
        sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
    }
}
