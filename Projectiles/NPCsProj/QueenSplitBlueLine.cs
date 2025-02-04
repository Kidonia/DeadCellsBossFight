using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using DeadCellsBossFight.Utils;
using Terraria.DataStructures;

namespace DeadCellsBossFight.Projectiles.NPCsProj;

public class QueenSplitBlueLine : ModProjectile
{
    public Player player => Main.player[Projectile.owner];
    public override void SetStaticDefaults()//本函数每次加载模组时执行一次，用于分配静态属性
    {
        Main.projFrames[Type] = 1;//你的帧图有多少帧就填多少
                                  //  ProjectileID.Sets.TrailingMode[Type] = 2;//这一项赋值2可以记录运动轨迹和方向（用于制作拖尾）
                                  //  ProjectileID.Sets.TrailCacheLength[Type] = 10;//这一项代表记录的轨迹最多能追溯到多少帧以前(注意最大值取不到)
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 4000;//这一项代表弹幕超过屏幕外多少距离以内可以绘制
                                                            //用于长条形弹幕绘制
                                                            //激光弹幕建议4000左右
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 2;//长宽无所谓，我们需要改写碰撞箱了
                                                  //注意细长形弹幕千万不能照葫芦画瓢把长宽按贴图设置因为碰撞箱是固定的，不会随着贴图的旋转而旋转
        Projectile.tileCollide = false;//false就能让他穿墙,就算是不穿墙激光也不要设置不穿墙
        Projectile.timeLeft = 449;//消散时间
        Projectile.scale = 7.2f;
        Projectile.aiStyle = -1;//不使用原版AI
        Projectile.penetrate = -1;//表示能穿透几次怪物。-1是无限制
        Projectile.ignoreWater = true;//无视液体
        base.SetDefaults();
    }
    public override bool ShouldUpdatePosition()//决定这个弹幕的速度是否控制他的位置变化
    {
        return false;
        //注意，激光类弹幕要返回false,速度只是用来赋予激光方向和击退力的，要修改位置请直接动center
    }
    public override void OnSpawn(IEntitySource source)
    {
        //Projectile.timeLeft = (int)Projectile.ai[2];
        base.OnSpawn(source);
    }
    public override void AI()//激光AI主要是控制方向和源点位置
    {

        //uint k = Main.GameUpdateCount / 90;
        //ai[0]控制攻击段数，ai[1]控制弹幕角度，ai[2]控制弹幕是否为第一次（5个），第二次（10个），第三次（20个），第一发为1,2,3，后续均为4，保证连贯的弹幕攻击均由第一发弹幕产生
        int num1 = (int)(90 / Projectile.ai[2]);
        int num2 = (int)(35 / (Projectile.ai[2] % 3));
        //Main.NewText(num1);
        //num1 = 45;
        // 前两次女王阶段切换产生的5/10个弹幕
        if (Projectile.ai[2] < 3 && Projectile.ai[2] > 0)
        {
            if (Projectile.timeLeft % num1 == 0 && Projectile.ai[0] > 0)// 每隔一小段时间生成一次弹幕，一共生成不超过ai[0]个
            {
                // rotation要大改
                //NormalUtils.EazyNewText(Projectile.ai[0], "ai[0]");
                Projectile.ai[0]--;// 减少一段
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, Vector2.Zero, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0], MathHelper.ToRadians(Main.rand.NextFloat(12, 168)), Projectile.ai[2] + 3);
                //Projectile.NewProjectile(Owner.parent(), spanPos, vr, ModContent.ProjectileType<MurasamaEndSkillOrbOnSpan>(), Projectile.damage, 0, Owner.whoAmI, vr.ToRotation(), Main.rand.Next(100));
            }

            if (Projectile.timeLeft % num1 - num2 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<QueenSplitScreen>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, Projectile.ai[1], Projectile.ai[0]);
            }
        }
        else if (Projectile.ai[2] == 3)// 全屏攻击
        {
            if (Projectile.timeLeft % 2 == 0 && Projectile.ai[0] > 0) // 每三十分之一秒生成一个弹幕，一共生成不超过ai[0]个
            {
                // rotation要大改
                //NormalUtils.EazyNewText(Projectile.ai[0], "ai[0]");
                Projectile.ai[0]--;// 减少一段
                Vector2 pos = Main.rand.NextVector2FromRectangle(NormalUtils.CreateRectangleFromVectors(Main.screenPosition, Main.screenWidth, Main.screenHeight));
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, Vector2.Zero, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0], MathHelper.ToRadians(Main.rand.NextFloat(12, 168)), Projectile.ai[2] + 3);
                


            }
            if(Projectile.timeLeft == 385 && Projectile.ai[2] < 4)
            {
                foreach(Projectile projectile in Main.projectile)
                {
                    if(projectile.type == Projectile.type)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), projectile.Center, Vector2.Zero, ModContent.ProjectileType<QueenSplitScreen>(), Projectile.damage, Projectile.knockBack, projectile.owner, 0, projectile.ai[1], projectile.ai[0]);
                }
            }
        }
        else
        {
            num1 = (int)(90 / (Projectile.ai[2] % 3));
            if (Projectile.timeLeft % num1 - num2 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<QueenSplitScreen>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, Projectile.ai[1], Projectile.ai[0]);
            }
        }

        //这一段是为了视觉效果设置的AI,localai0将被用来控制激光透明度渐变
        if (Projectile.localAI[0] < 9)//弹幕出现时增加
            Projectile.localAI[0]++;
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        //你得配
        return false;
    }

    public override bool PreDraw(ref Color lightColor)//predraw返回false即可禁用原版绘制
    {
        SpriteBatch sb = Main.spriteBatch;
        int length = (int)(Math.Sqrt(Main.screenWidth * Main.screenWidth + Main.screenHeight * Main.screenHeight) * 1.2f);//定义激光长度
                                      //黑色背景的图片如果不对A值赋予0，或者启动Additive模式的话，画出来是黑色，效果很差
                                      //接下来是简单的延长绘制

        //下面是激光身体的绘制
        Texture2D tex = TextureAssets.Projectile[Type].Value;//获取材质，这是激光中部
        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

        sb.Draw(tex,
            Projectile.Center + Vector2.Normalize((Projectile.Left - Projectile.Center).RotatedBy(Projectile.ai[1])) * length / 2 * Projectile.scale - Main.screenPosition, 
            new Rectangle(0, 0, length, tex.Height),//在高度不变的基础上，X轴延长到length
            new (255, 255, 255, (int)Projectile.localAI[0] * 32),//修改后的颜色
            Projectile.ai[1],
            new Vector2(0, tex.Height / 2),//参考原点选择图片左边中点
            Projectile.scale,//为使得激光更加自然，调整激光宽度
            SpriteEffects.None, 0);

        return false;//return false阻止自动绘制
    }
}
