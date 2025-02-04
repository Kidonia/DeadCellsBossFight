using DeadCellsBossFight.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using DeadCellsBossFight.Contents.GlobalChanges;

namespace DeadCellsBossFight.Contents.Buffs;

public  class Bleed : ModBuff
{
    public Player player => Main.player[Main.myPlayer];
    public DCPlayer dcplayer => player.GetModPlayer<DCPlayer>();
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        Main.buffNoSave[Type] = false;
        Main.pvpBuff[Type] = false;
    }
    public override void Update(Player player, ref int buffIndex)
    {
        int d = player.direction;
        //禁用生命偷取
        player.moonLeech = true;

        for (int i = 0; i < 2; i++)//身体周围生成粒子
        {
            int num2 = Dust.NewDust(player.Center, 1, 1, DustID.Blood, d * Main.rand.NextFloat(7f, 12f), Main.rand.NextFloat(-2f, 8f), 0, new Color(Main.rand.Next(230, 256), 0, 0), Main.rand.NextFloat(0.8f, 1.3f));
            Main.dust[num2].scale += Main.rand.Next(-6, 21) * 0.01f;
        }


        //player.lifeRegen相关写在DCPlayer里了
        if (dcplayer.BleedLevel >= 5)
        {
            int boomdmg = Main.rand.Next(55, 70);
            player.Hurt(PlayerDeathReason.ByCustomReason("血流成河"), boomdmg, 0, dodgeable: false, knockback: 0, armorPenetration: 0, scalingArmorPenetration: 0);
            int showBoomDmg = CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new(222, 11, 16), $"{boomdmg}", true, false);
            Main.combatText[showBoomDmg].lifeTime = 75;


            for (int i = 0; i < 4; i++)
            {
                int m = Main.rand.Next(2, 4);
                for (double r = 0f; r < MathHelper.TwoPi; r += MathHelper.TwoPi / (float)(5 + m))//血爆绘制外圆边
                {
                    float k = Main.rand.NextFloat(-0.4f, 0.4f);
                    Vector2 newposition = new Vector2((float)Math.Cos(r + k), (float)Math.Sin(r + k)) * 60f;
                    float rot = newposition.ToRotation() + (float)Math.PI / 2f;
                    float rotation = newposition.ToRotation();
                    if (rotation < 0)
                    {
                        rotation += MathHelper.TwoPi;
                    }
                    Vector2 Center = player.Center + Terraria.Utils.RotatedBy(new Vector2(60f + Main.rand.NextFloat(-10f, 3f), 0f), rotation);
                    Projectile.NewProjectile(player.GetSource_Buff(buffIndex), Center, Vector2.Zero, ModContent.ProjectileType<RedLightning>(), 0, 0f, default, rot);
                }
            };

            for (int k = 0; k < 160; k++)//血爆player身体周围生成粒子
            {
                int num2 = Dust.NewDust(player.position, player.width, player.height, DustID.Blood, Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, -0.2f), 210, new Color(255, 0, 0), 3f);
                Main.dust[num2].scale *= 1 - Main.rand.NextFloat(0.011f, 0.01f);
                Main.dust[num2].noGravity = true;
            }

            for (int i = 0; i < 20; i++)//血爆player身体溅血
            {
                int num2 = Dust.NewDust(player.Center, 1, 1, DustID.Blood, d * Main.rand.NextFloat(7f, 12f), Main.rand.NextFloat(-2f, 8f), 0, new Color(Main.rand.Next(230, 256), 0, 0), Main.rand.NextFloat(0.8f, 1.3f));
                Main.dust[num2].scale += Main.rand.Next(-3, 6) * 0.01f;
            }


            dcplayer.BleedLevel = 0;
            player.buffTime[buffIndex] = 2;
        }

        //层数随时间减少
        if (player.buffTime[buffIndex] < 20 && player.buffTime[buffIndex] > 0)
        {
            if (dcplayer.BleedLevel > 0)
            {
                dcplayer.BleedLevel--;
                player.buffTime[buffIndex] += 70;
            }

            if (dcplayer.BleedLevel == 0)
            {
                player.buffTime[buffIndex] = 0;
            }
        }


    }
    public override bool ReApply(Player player, int time, int buffIndex)
    {
        player.buffTime[buffIndex] = time;
        return true;
    }
}
