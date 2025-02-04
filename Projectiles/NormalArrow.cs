using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ID;

namespace DeadCellsBossFight.Projectiles;

public class NormalArrow : ModProjectile
{
    private Texture2D tex;


    public override void SetDefaults()
    {
        Projectile.width = 30;
        Projectile.height = 30;
        Projectile.scale = 1.1f;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 240;
        Projectile.friendly = true;
        tex = ModContent.Request<Texture2D>(Texture, (AssetRequestMode)2).Value;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpriteBatch sb = Main.spriteBatch;
        sb.End();
        sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
        sb.Draw(tex,
            Projectile.Center - Main.screenPosition,
            null,
            Color.White,
            Projectile.velocity.ToRotation() + MathHelper.Pi / 2,
            tex.Size() * 0.5f,
            1f,
            SpriteEffects.None,
            0);
        return false;
    }

    public override void AI()
    {
        Lighting.AddLight(Projectile.Center, 0.08f, 0.66f, 0.8f);

            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.BlueTorch, 0f, 0f, 90, default, 2.4f);
            dust.noGravity = true;
            // 让粒子默认的运动速度归零
            dust.velocity *= 0;
            // 让粒子始终处于弹幕的中心位置
            dust.position = Projectile.Center;

        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi / 2;  //弹幕贴图方向朝上的情况
    }
    public override void PostDraw(Color lightColor)
    {

        //SpriteBatch sb = Main.spriteBatch;

       
        //sb.End();
        //sb.Begin();
    }

}

