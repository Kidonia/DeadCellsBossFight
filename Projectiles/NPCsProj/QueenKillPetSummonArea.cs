using DeadCellsBossFight.Contents.GlobalChanges;
using DeadCellsBossFight.Core;
using DeadCellsBossFight.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Projectiles.NPCsProj;

public class QueenKillPetSummonArea : ModProjectile
{
    public override string Texture => AssetsLoader.TransparentImg;

    public override void SetDefaults()
    {
        Projectile.width = 4;
        Projectile.height = 4; 
        Projectile.tileCollide = false;//false就能让他穿墙,就算是不穿墙激光也不要设置不穿墙
        Projectile.timeLeft = 175;//消散时间
        Projectile.aiStyle = -1;//不使用原版AI
        Projectile.penetrate = -1;//表示能穿透几次怪物。-1是无限制
        Projectile.ignoreWater = true;//无视液体
        base.SetDefaults();
    }
    public override void AI()
    {

    }
    public override void PostAI()
    {
        Vector2 vector = Projectile.Center - Main.player[Projectile.owner].Center;
        Vector2 vector2 = vector.SafeNormalize(Vector2.UnitX) * 0.05f;
        Main.player[Projectile.owner].velocity += vector2;

        foreach (Projectile targetProj in Main.projectile)
        {
            if ((targetProj.minion || Main.projPet[targetProj.type] || NormalUtils.lightPetProj.Contains(targetProj.type)))
            {
                if (targetProj.type == ProjectileID.VoltBunny && targetProj.ai[0] == 1)
                    break;

                // 飞碟/台风/细胞 ProjAIStyleID.Hornet
                // 跟玩家的宠物，要改 ProjAIStyleID.FloatBehindPet || ProjAIStyleID.FloatInFrontPet
                // 双眼ProjAIStyleID.MiniTwins
                // 同伴方块，跳动型ProjAIStyleID.CommonFollow
                if (targetProj.aiStyle == ProjAIStyleID.FloatBehindPet || targetProj.aiStyle == ProjAIStyleID.FloatingFollow || targetProj.aiStyle == ProjAIStyleID.StardustGuardian || targetProj.type == ProjectileID.Wisp)
                {
                    targetProj.velocity = Vector2.Zero;
                    targetProj.position = Vector2.Lerp(targetProj.position, Projectile.Center, (175f - Projectile.timeLeft) / 175f);
                }
                else
                {
                    targetProj.damage = 0;
                    Vector2 line = (Projectile.Center - targetProj.Center) / 32;
                    float radian = MathHelper.Lerp(0.21f, 0.1f, line.Length() / 48);
                    targetProj.velocity = line.RotatedBy(radian);
                }


                if (Projectile.timeLeft == 2)
                {
                    if (Main.player[Projectile.owner].TryGetModPlayer(out DCPlayer limitplayer))
                    {
                        limitplayer.StopMinionPetUse = true;
                    }
                    foreach (var buff in Main.player[Projectile.owner].buffType)
                    {
                        if (buff > 0 && (Main.vanityPet[buff] || Main.lightPet[buff]))
                        {
                            Main.player[Projectile.owner].ClearBuff(buff);
                            Main.player[Projectile.owner].hideMisc[0] = Main.player[Projectile.owner].hideMisc[1] = true;
                        }
                    }
                    targetProj.active = false ;
                }

            }
        }
        base.PostAI();
    }
    public override bool? CanDamage()
    {
        return false;
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return false;
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
}
