
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Projectiles.NPCsProj;

public class QueenParryArea : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 32;//长宽无所谓，我们需要改写碰撞箱了
                                                  //注意细长形弹幕千万不能照葫芦画瓢把长宽按贴图设置因为碰撞箱是固定的，不会随着贴图的旋转而旋转
        Projectile.tileCollide = false;//false就能让他穿墙,就算是不穿墙激光也不要设置不穿墙
        Projectile.timeLeft = 90;//消散时间
        Projectile.scale = 3.6f;
        Projectile.aiStyle = -1;//不使用原版AI
        Projectile.penetrate = -1;//表示能穿透几次怪物。-1是无限制
        Projectile.ignoreWater = true;//无视液体
        base.SetDefaults();
    }
    public override bool ShouldUpdatePosition()//决定这个弹幕的速度是否控制他的位置变化
    {
        return false;
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        //你得配
        return false;
    }
    public override bool PreDraw(ref Color lightColor)//predraw返回false即可禁用原版绘制
    {
        SpriteBatch sb = Main.spriteBatch;
        //下面是激光身体的绘制
        Texture2D tex = TextureAssets.Projectile[Type].Value;//获取材质


        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

        sb.Draw(tex,
            Projectile.Center - Main.screenPosition,
            null,//在高度不变的基础上，X轴延长到length
            Color.White,//修改后的颜色
            0f,
            tex.Size() / 2f,//参考原点选择图片左边中点
            Projectile.scale,//为使得激光更加自然，调整激光宽度
            (SpriteEffects)((Projectile.ai[0] + 1) / 2), 0);
        return false;//return false阻止自动绘制
    }

}
