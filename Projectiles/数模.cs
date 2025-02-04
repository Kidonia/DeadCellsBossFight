using DeadCellsBossFight.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Projectiles;

public class 数模 : ModProjectile
{
    // ai[0]为R，ai[1]为r，ai[2]为l
    // localAI[0]内切还是外切，在AI()里会改
    // localAI[1]画图结束后图片存在时间
    // localAI[2]参数n, k=n*原始比例系数*t

    public Vector2 圆心;
    public int r1 { get { return (int)Projectile.ai[0]; } set { Projectile.ai[0] = value; } } // 大圆半径
    public int r2 { get { return (int)Projectile.ai[1]; } set { Projectile.ai[1] = value; } } // 小圆半径
    public float k { get { return Projectile.ai[2]; } set { Projectile.ai[2] = value; } } // 比例系数（可变化）
    public int t;
    public int 小圆转圈圈数_周期;
    public Vector2[][] 落点轨迹;
    public Vector2 小圆圆心;
    public Vector2[] 小圆轮廓 = new Vector2[360];
    int index = -1;// 最新一个点的索引
    public bool finished = false;// 运动完成
    public float 原始比例系数 = 0;
    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.aiStyle = -1;

        base.SetDefaults();
    }
    public override void OnSpawn(IEntitySource source)
    {
        圆心 = Projectile.Center;
        t = 0;
        小圆转圈圈数_周期 = Get_SmallCircle_Rotate_Count();
        落点轨迹 = new Vector2[360 * 小圆转圈圈数_周期][];

        Main.NewText(小圆转圈圈数_周期);

        // 初始化每个内层数组，大小为3
        for (int i = 0; i < 落点轨迹.Length; i++)
        {
            落点轨迹[i] = new Vector2[4];
        }

        // 结束后存在时间
        Projectile.localAI[1] = 180;

        原始比例系数 = k;
    }
    public override void OnKill(int timeLeft)
    {
        return;
        //Main.NewText(Projectile.localAI[2]);
        /*
         //k改变
        if (Projectile.localAI[0] == -1)
        {
            Main.NewText("kill");
            if (Projectile.ai[2] + adjust >= 1)
            {
                return;
            }
            else
            {

                var proj2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.position, Vector2.Zero, ModContent.ProjectileType<数模二>(), 0, 0, -1, Projectile.ai[0] - 2 * Projectile.ai[1], Projectile.ai[1], Projectile.ai[2] + adjust);
                // 0 为外切，1为内切
                proj2.localAI[0] = 0;
                proj2.localAI[2] = Projectile.localAI[2];


                return;
            }
        }
        else if (Projectile.localAI[0] == 1)
        {
            if (Projectile.ai[2] + adjust >= 1)
            {
                return;
            }
            else
            {

                var proj2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.position, Vector2.Zero, ModContent.ProjectileType<数模二>(), 0, 0, -1, Projectile.ai[0], Projectile.ai[1], Projectile.ai[2] + adjust);
                // 0 为外切，1为内切
                proj2.localAI[0] = 1;
                proj2.localAI[2] = Projectile.localAI[2];


                return;
            }
        }
        */
        /*
        // 半径比例改变
        if (Projectile.localAI[0] == 0)
        {
            if (Projectile.ai[1] + adjust >= 70)
            {
                return;
            }
            else
            {

                var proj2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.position, Vector2.Zero, ModContent.ProjectileType<数模二>(), 0, 0, -1, Projectile.ai[0] - 2 * Projectile.ai[1], Projectile.ai[1] + adjust, Projectile.ai[2]);
                // 0 为外切，1为内切
                proj2.localAI[0] = 0;
                proj2.localAI[2] = Projectile.localAI[2];


                return;
            }
        }
        else if (Projectile.localAI[0] == 1)
        {
            if (Projectile.ai[1] + adjust >= 65)
            {
                return;
            }
            else
            {

                var proj2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.position, Vector2.Zero, ModContent.ProjectileType<数模二>(), 0, 0, -1, Projectile.ai[0], Projectile.ai[1] + adjust, Projectile.ai[2]);
                // 0 为外切，1为内切
                proj2.localAI[0] = 1;
                proj2.localAI[2] = Projectile.localAI[2];


                return;
            }
        }
        */
        /*
        //r1:r2 k改变
        if (Projectile.localAI[0] == -1)
        {
            if (Projectile.ai[2] + adjust >= 1)
            {
                return;
            }
            else
            {

                var proj2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.position, Vector2.Zero, ModContent.ProjectileType<数模二>(), 0, 0, -1, Projectile.ai[0] - 2 * Projectile.ai[1], Projectile.ai[1] + adjust * 50, Projectile.ai[2] + adjust);
                // 0 为外切，1为内切
                proj2.localAI[0] = 0;
                proj2.localAI[2] = Projectile.localAI[2];


                return;
            }
        }
        else if (Projectile.localAI[0] == 1)
        {
            if (Projectile.ai[2] + adjust >= 1)
            {
                return;
            }
            else
            {

                var proj2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.position, Vector2.Zero, ModContent.ProjectileType<数模二>(), 0, 0, -1, Projectile.ai[0], Projectile.ai[1] + adjust * 50, Projectile.ai[2] + adjust);
                // 0 为外切，1为内切
                proj2.localAI[0] = 1;
                proj2.localAI[2] = Projectile.localAI[2];


                return;
            }
        }
        */
    }
    public override void AI()
    {
        //沟槽的OnSpawn

        // 0 为外切，1为内切
        if (Projectile.localAI[0] == 0)
        {
            r1 += 2 * r2;
            Projectile.localAI[0] = -1;

            小圆转圈圈数_周期 = Get_SmallCircle_Rotate_Count();
            落点轨迹 = new Vector2[360 * 小圆转圈圈数_周期][];


            // 初始化每个内层数组，大小为3
            for (int i = 0; i < 落点轨迹.Length; i++)
            {
                落点轨迹[i] = new Vector2[4];
            }
        }
        //Projectile.Kill();
        Projectile.timeLeft = 4;
        if (t < 小圆转圈圈数_周期 * 360)
        {
            
            //Main.NewText(小圆转圈圈数_周期 * 360 - t);
            if (Projectile.localAI[2] > 0)
            {
                k = 原始比例系数 * Projectile.localAI[2] * 0.01f * t/60f;

            }
            

            落点轨迹[t][0] = GetPos_Flowers_Curve(t, Projectile.localAI[0]);
            落点轨迹[t][1] = GetPos_Flowers_Curve(t + 0.25f, Projectile.localAI[0]);
            落点轨迹[t][2] = GetPos_Flowers_Curve(t + 0.5f, Projectile.localAI[0]);
            落点轨迹[t][3] = GetPos_Flowers_Curve(t + 0.75f, Projectile.localAI[0]);
            小圆圆心 = GetPos_SmallCircleO(t);

            for (int i = 0; i < 360; i++)
            {
                小圆轮廓[i] = 小圆圆心 + GetCircleDrawPos(r2, i);
            }
            t++;

            
            // 直接画完
            while (t < 小圆转圈圈数_周期 * 360 - 1)
             {
                落点轨迹[t][0] = GetPos_Flowers_Curve(t, Projectile.localAI[0]);
                落点轨迹[t][1] = GetPos_Flowers_Curve(t + 0.25f, Projectile.localAI[0]);
                落点轨迹[t][2] = GetPos_Flowers_Curve(t + 0.5f, Projectile.localAI[0]);
                落点轨迹[t][3] = GetPos_Flowers_Curve(t + 0.75f, Projectile.localAI[0]);
                t++;
                if (Projectile.localAI[2] > 0)
                {
                    k = 原始比例系数 * Projectile.localAI[2] * 0.01f * t / 60f;


                }
            }
            Main.NewText(k);
            Main.NewText(t);

        }
        if (t >= 小圆转圈圈数_周期 * 360)
        {
            //Projectile.Kill();
            finished = true;
        }
        if (finished)
        {
            Projectile.localAI[1]--;
            if (Projectile.localAI[1] < 0)
                Projectile.timeLeft = 0;
        }
        base.AI();
    }
    public override string Texture => AssetsLoader.TransparentImg;
    public override bool PreDraw(ref Color lightColor)
    {
        Color lineColor = Projectile.localAI[0] == -1 ? Color.Yellow : new Color(0, 255, 255);
        
        if (!finished && Projectile.localAI[1] > 60)
        {
            // 参数在AI已经改变
            // -1 为外切，1为内切
            if (Projectile.localAI[0] == -1)
                for (int i = 0; i < 360; i++)
                {
                    Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, 圆心 + new Vector2(r1 - 2 * r2, 0).RotatedBy(DegreeToRadian(i)) - Main.screenPosition, new(0, 0, 1, 1), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                }
            //画外圆
            else
                for (int i = 0; i < 360; i++)
                {
                    Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, 圆心 + new Vector2(r1, 0).RotatedBy(DegreeToRadian(i)) - Main.screenPosition, new(0, 0, 1, 1), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                }
            // 画滚动小圆
            for (int i = 0; i < 360; i++)
            {
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, 圆心 + 小圆轮廓[i] - Main.screenPosition, new(0, 0, 1, 1), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);

            }
        }
        

        for (int i = 1; i < 落点轨迹.Length; i++)
        {
            lineColor = HSVtoRGB(255-(int)落点轨迹[i][0].Length(), 200, 200);


            if (落点轨迹[i][0] == Vector2.Zero)
            {
                index = i - 1;
                break;
            }
            else
            {

                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, 圆心 + 落点轨迹[i][0] - Main.screenPosition, new(0, 0, 1, 1), lineColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                Vector2 pos1 = 落点轨迹[i - 1][3];
                Vector2 pos2 = 落点轨迹[i][0];
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, 圆心 + pos1 - Main.screenPosition, new Rectangle(0, 0, 1, (int)(pos2 - pos1).Length()), lineColor, (pos2 - pos1).ToRotation() - MathHelper.PiOver2, Vector2.Zero, 1f, SpriteEffects.None, 0);

                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, 圆心 + 落点轨迹[i][1] - Main.screenPosition, new(0, 0, 1, 1), lineColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                pos1 = pos2;
                pos2 = 落点轨迹[i][1];
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, 圆心 + pos1 - Main.screenPosition, new Rectangle(0, 0, 1, (int)(pos2 - pos1).Length()), lineColor, (pos2 - pos1).ToRotation() - MathHelper.PiOver2, Vector2.Zero, 1f, SpriteEffects.None, 0);

                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, 圆心 + 落点轨迹[i][2] - Main.screenPosition, new(0, 0, 1, 1), lineColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                pos1 = pos2;
                pos2 = 落点轨迹[i][2];
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, 圆心 + pos1 - Main.screenPosition, new Rectangle(0, 0, 1, (int)(pos2 - pos1).Length()), lineColor, (pos2 - pos1).ToRotation() - MathHelper.PiOver2, Vector2.Zero, 1f, SpriteEffects.None, 0);

                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, 圆心 + 落点轨迹[i][3] - Main.screenPosition, new(0, 0, 1, 1), lineColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                pos1 = pos2;
                pos2 = 落点轨迹[i][3];
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, 圆心 + pos1 - Main.screenPosition, new Rectangle(0, 0, 1, (int)(pos2 - pos1).Length()), lineColor, (pos2 - pos1).ToRotation() - MathHelper.PiOver2, Vector2.Zero, 1f, SpriteEffects.None, 0);

            }
        }
        lineColor = HSVtoRGB(255 - (int)落点轨迹[index][0].Length(), 200, 200);
        if (!finished && Projectile.localAI[1] > 60)
        {
            //Main.NewText((落点轨迹[index] - 小圆圆心).ToRotation());
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, 圆心 + 小圆圆心 - Main.screenPosition, new(0, 0, 1, r2), Color.White, (落点轨迹[index][0] - 小圆圆心).ToRotation() - MathHelper.PiOver2, Vector2.Zero, 1f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, 圆心 + 小圆圆心 - Main.screenPosition, new(0, 0, 1, (int)(r2 * k)), lineColor, (落点轨迹[index][0] - 小圆圆心).ToRotation() - MathHelper.PiOver2, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }
        
        return false;
    }

    /// <summary>
    /// 圆上一点相对圆心坐标
    /// </summary>
    /// <param name="r">圆的半径</param>
    /// <param name="theta">圆的角度（向右为0）（不是弧度）</param>
    /// <returns>返回圆上一点相对圆心坐标</returns>
    public Vector2 GetCircleDrawPos(int r, int theta)
    {
        float rad = DegreeToRadian(theta);

        float x = r * (float)Math.Cos(rad);
        float y = r * (float)Math.Sin(rad);

        return new Vector2(x, y);
    }
    /// <summary>
    /// 计算小圆圆心
    /// </summary>
    /// <param name="theta">角度值</param>
    /// <returns>返回小圆圆心相对于大圆圆心位置</returns>
    public Vector2 GetPos_SmallCircleO(int theta)
    {
        float rad = DegreeToRadian(theta);
        float xc = (r1 - r2) * (float)Math.Cos(rad);
        float yc = (r1 - r2) * (float)Math.Sin(rad);
        return new Vector2(xc, yc);
    }
    /// <summary>
    /// 根据繁花曲线参数方程，计算当前时刻落点相对圆心位置
    /// </summary>
    /// <param name="theta">角度值</param>
    /// <param name="outside">内切为1，外切为-1</param>
    /// <returns>当前时刻落点相对圆心位置</returns>
    public Vector2 GetPos_Flowers_Curve(float theta, float outside = 1)
    {
        float b = (float)r2 / r1;
        float ef = 1 - b;

        float rad = DegreeToRadian(theta);
        float x = (float)(r1 * (ef * Math.Cos(rad) + outside * k * b * Math.Cos(ef / b * rad)));
        float y = (float)(r1 * (ef * Math.Sin(rad) - k * b * Math.Sin(ef / b * rad)));

        return new Vector2(x, y);

    }


    /// <summary>
    /// 角度转弧度
    /// </summary>
    /// <param name="degree">角度(0~360°)</param>
    /// <returns>弧度制</returns>
    public float DegreeToRadian(float degree)
    {
        return degree * MathHelper.Pi / 180f;
    }
    /// <summary>
    /// 计算最大公约数，辗转相除法
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int Greatest_Common_Divisor(int x, int y)
    {
        int result;
        if (x == y)
            result = x;
        else if (x > y)
            result = Greatest_Common_Divisor(x - y, y);
        else
            result = Greatest_Common_Divisor(x, y - x);

        return result;
    }
    /// <summary>
    /// 计算小圆在大圆里要自转几圈
    /// </summary>
    /// <returns>圈数</returns>
    public int Get_SmallCircle_Rotate_Count()
    {
        int div = Greatest_Common_Divisor(r1, r2);
        return r2 / div;
    }

    public static Color HSVtoRGB(int h, int s, int v)
    {
        float hue = h / 255f * 360f;
        float saturation = s / 255f;
        float value = v / 255f;

        int hi = (int)Math.Floor(hue / 60) % 6;
        float f = hue / 60 - (float)Math.Floor(hue / 60);

        float p = value * (1 - saturation);
        float q = value * (1 - f * saturation);
        float t = value * (1 - (1 - f) * saturation);

        float r = 0, g = 0, b = 0;

        switch (hi)
        {
            case 0:
                r = value; g = t; b = p;
                break;
            case 1:
                r = q; g = value; b = p;
                break;
            case 2:
                r = p; g = value; b = t;
                break;
            case 3:
                r = p; g = q; b = value;
                break;
            case 4:
                r = t; g = p; b = value;
                break;
            case 5:
                r = value; g = p; b = q;
                break;
        }

        return new Color((int)(r * 255), (int)(g * 255), (int)(b * 255));
    }
}
