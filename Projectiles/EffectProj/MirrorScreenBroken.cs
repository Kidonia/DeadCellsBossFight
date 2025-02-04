using DeadCellsBossFight.Contents.GlobalChanges;
using DeadCellsBossFight.Contents.Tiles;
using DeadCellsBossFight.Core;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.CameraModifiers;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Projectiles.EffectProj;

public class MirrorScreenBroken : ModProjectile
{
    public override string Texture => AssetsLoader.TransparentImg;
    public DCPlayer dcplayer => Main.LocalPlayer.GetModPlayer<DCPlayer>(); // Content/GlobalChanges里
    public TimeStopPlayer timestop => Main.LocalPlayer.GetModPlayer<TimeStopPlayer>(); // Content/GlobalChanges/SlayAllChanges里

    // ai[0]控制屏幕缩放
    // ai[1]控制几帧的rgb错位
    // ai[2]控制屏幕的灰暗程度
    // localAI[0]控制是否绘制屏幕碎裂，0用于一开始初始化，1正在绘制，2结束绘制，3不绘制
    // localAI[1]控制径向模糊强度，0不绘制
    // localAI[2]控制蓝色滤镜（？

    public override void OnSpawn(IEntitySource source)
    {
        Main.blockInput = true; // 直接禁用输入，奶奶的
        //dcplayer.ActivateScreenPosFrozen();
        SoundEngine.PlaySound(AssetsLoader.slayAll, Main.LocalPlayer.Center); // 放送音效
        Projectile.ai[0] = 1f; // 屏幕正常大小（scale = 1f）
        Projectile.ai[2] = 40; // 灰暗40
        Projectile.localAI[0] = 3; // 无碎屏
        DeadCellsBossFight.EffectProj.Add(Projectile);
        base.OnSpawn(source);
    }
    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 380;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override void AI()
    {
        int adjust = 28; // 因为音效加的晚，所以额外给时间来个调整，只有下面一行用了
        int timeSinceSpawn = 380 - Projectile.timeLeft - adjust; // 整个次元斩绝的时间线
        int graylimit = 285 < timeSinceSpawn ? 0 : 50; // 控制灰暗范围
        // 没错，都是时间特判
        if (timeSinceSpawn == -4 || timeSinceSpawn == 38 || timeSinceSpawn == 66)
        {
            Projectile.ai[2] += Main.rand.Next(50, 80); // 屏幕闪烁，指随机改变灰色的程度
            DCPlayer.playerDrawLightningTime += Main.rand.Next(10, 25); // 身上随机画闪电效果
            // 屏幕震动，抄的
            var bump = new PunchCameraModifier(Main.LocalPlayer.Center, Main.rand.NextVector2Unit(), 7 + timeSinceSpawn / 70, 12 + timeSinceSpawn / 80, 20 + timeSinceSpawn / 80);
            Main.instance.CameraModifiers.Add(bump);
        }

        if (timeSinceSpawn < 90) // 一开始阎魔刀手持弹幕微微旋转
        {
            // 阎魔刀手持弹幕微微旋转
            DCPlayer.yamadoExtraDrawRotation += 0.002f;
            // 屏幕放大
            Projectile.ai[0] += (0.6f / (90 + adjust));
        }
        if (90 < timeSinceSpawn && timeSinceSpawn < 111)
        {
            // 屏幕震动，抄的
            var bump = new PunchCameraModifier(Main.LocalPlayer.Center, Main.rand.NextVector2Unit(), 6, 10, 15);
            Main.instance.CameraModifiers.Add(bump);

            Projectile.ai[1] = 0f; // 初始化rgb错位
            Projectile.ai[0] -= 0.04f; // 屏幕迅速开始缩小
            timestop.TimeFrozen = true; // 开始时停
            Main.hideUI = true; // 不绘制UI
            if (timeSinceSpawn < 100) // 时间范围(90, 100)
            {
                // 生成圆弧
                float rotation = MathHelper.TwoPi / 8f * (timeSinceSpawn % 8) + Main.rand.NextFloat(-1f, 1f);
                Vector2 vec = new Vector2(0, Main.rand.NextFloat(45f, Main.screenHeight / 5f)).RotatedBy(rotation);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Main.LocalPlayer.Center + vec, Vector2.Zero, ModContent.ProjectileType<Roundtry>(), 0, 0, -1, ai0:0);
                if (Main.rand.NextBool()) // 随机多来点
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Main.LocalPlayer.Center + new Vector2(0, Main.rand.NextFloat(45f, Main.screenHeight / 5f)).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)), Vector2.Zero, ModContent.ProjectileType<Roundtry>(), 0, 0, -1, ai0:1f);
                }


                // 不绘制玩家，时间慢慢调
                DCPlayer.playerHideDrawTime += 13;
                // rgb错位强度
                Projectile.ai[1] = 0.005f;
                // 变灰变灰再变灰
                Projectile.ai[2] += 17;
            }
            else // 时间范围(100, 111)
            {
                // 生成白线，后续会变成黑影，因为 ai[0] == 1，看Linetry.cs
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Main.LocalPlayer.Center + Main.rand.NextVector2Circular(Main.screenWidth / 10f, Main.screenHeight / 10f) * 3.2f, Vector2.Zero, ModContent.ProjectileType<Linetry>(), 0, 0, -1, ai0:1);
                if (Main.rand.NextBool(5)) // 来点随机多的弹幕
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Main.LocalPlayer.Center + Main.rand.NextVector2Circular(Main.screenWidth / 10f, Main.screenHeight / 10f) * 3.2f, Vector2.Zero, ModContent.ProjectileType<Linetry>(), 0, 0, -1, ai0:1);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Main.LocalPlayer.Center + Main.rand.NextVector2Circular(Main.screenWidth / 10f, Main.screenHeight / 10f) * 4, Vector2.Zero, ModContent.ProjectileType<Linetry>(), 0, 0, -1, ai0: 0);
                }
            }
        }
        if(timeSinceSpawn == 111)
        {
            Projectile.localAI[0] = 0; // 初始化屏幕碎裂扭曲
            Projectile.localAI[1] = 0.2f; // 进入径向模糊
            Projectile.localAI[2] = 2; // 开启灰色滤镜
        }
        if (111 < timeSinceSpawn && timeSinceSpawn < 132 )
        {
            // 生成白线，不留黑影，因为 ai[0] == 0
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Main.LocalPlayer.Center + Main.rand.NextVector2Circular(Main.screenWidth / 10f, Main.screenHeight / 10f) * 4, Vector2.Zero, ModContent.ProjectileType<Linetry>(), 0, 0, -1, ai0: 0);

            Projectile.localAI[1] -= 0.01f; // 径向模糊变弱
        }
        if (timeSinceSpawn == 132)
        {
            Projectile.localAI[1] = 0; // 径向模糊结束
        }
        if (160 < timeSinceSpawn && timeSinceSpawn < 191)
        {
            // 屏幕微微放大，玩家差不多开始绘制
            Projectile.ai[0] += 0.00667f;
        }
        if (210 < timeSinceSpawn && timeSinceSpawn < 281)
        {
            // 屏幕放大，玩家差不多开始收刀
            Projectile.ai[0] += 0.003f;
        }
        if (280 < timeSinceSpawn && timeSinceSpawn < 286)
        {
            if (timeSinceSpawn == 282)
            {
                //Projectile.ai[2] += Main.rand.Next(50, 80);
                // 添加屏幕震动
                var bump = new PunchCameraModifier(Main.LocalPlayer.Center, Main.rand.NextVector2Unit(), 7 + timeSinceSpawn / 70, 12 + timeSinceSpawn / 80, 20 + timeSinceSpawn / 80);
                Main.instance.CameraModifiers.Add(bump);


                Main.hideUI = false; // 画UI
                Main.blockInput = false;
                timestop.TimeFrozen = false; // 停止时停

                DCPlayer.playerDrawLightningTime += 60; // 绘制闪电效果
                Projectile.localAI[0] = 2; // 碎屏结束
                Projectile.localAI[1] = 0.38f; // 再次进入径向模糊
                Projectile.localAI[2] = 0.54f; // 灰色滤镜结束
                DCPlayer.yamadoExtraDrawRotation = 0; // 清除之前的手臂/刀旋转
                DCPlayer.yamadoHeldProj.Kill(); // 删了弹幕，让Item重新生成一个，暴力但是有效
                /////////////////////////////////////////////////////
                // 用的一刀秒，你可以改成别的受击
                foreach (NPC npc in Main.npc)
                {
                    if(npc.active && !npc.townNPC)
                    {
                        if (!npc.boss)
                        {
                            npc.HitEffect(0, 114514, true);
                            npc.StrikeInstantKill();
                        }
                        if(npc.boss)
                        {
                            npc.StrikeInstantKill();
                        }
                    }
                }
                /*
                /////////////////////////////////////////////////////
                // 清空屏幕内所有砖块，墙，
                int posX = (int)Main.screenPosition.X / 16;
                int posY = (int)Main.screenPosition.Y / 16;
                for (int i = posX; i < posX + Main.screenWidth / 16; i++)
                {
                    for (int j = posY; j < posY + Main.screenHeight / 16; j++)
                    {
                        if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType != ModContent.TileType<TransparentTile>() && Main.tile[i, j].TileType != ModContent.TileType<DCNormalTile>())
                        {
                            Tile tile = TilemapExtensions.GetTileSafely(i, j);
                            tile.ClearEverything();
                        }
                        if (Main.tile[i, j].WallType > 0)
                            Main.tile[i, j].WallType = 0;
                    }
                }
                /////////////////////////////////////////////////////
                */
                // 生成Particle，向玩家两侧散开，增强收刀表现
                for (int i = 0; i < 8; i++)
                {
                    ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings
                    {
                        PositionInWorld = Main.LocalPlayer.Center,
                        MovementVector = new Vector2(Main.rand.NextFloat(-6, 6), -Main.rand.NextFloat(-1, 1))
                    });
                }
            }
            // 屏幕迅速放大，增强收刀表现力
            Projectile.ai[0] += 0.025f;
        }


        if (285 < timeSinceSpawn && timeSinceSpawn < 335)
        {
            Projectile.localAI[2] -= 0.02f;  // 结束时的滤镜，但是懒，直接去画个矩形改颜色透明度了
            Projectile.localAI[1] -= 0.04f; // 径向模糊减弱到0
            Projectile.ai[0] -= 0.00685f;// 屏幕缩小到正常
        }
        Projectile.ai[2] = Math.Max(graylimit, Projectile.ai[2] - 2); // 限制灰暗度
    }
}
