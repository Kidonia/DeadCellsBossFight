using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using System.Collections.Generic;
using Terraria.Audio;
using DeadCellsBossFight.Projectiles.EffectProj;
using DeadCellsBossFight.Core;

namespace DeadCellsBossFight.Projectiles;

public class SoulProj : ModProjectile
{
    public Player player => Main.player[Projectile.owner];
    private Entity target = null;//追踪的目标
    private bool ShouldExtraMove = true;
    private Dictionary<int, DCAnimPic> GhostTransfmDic = new();//生成时的变形特效字典
    private int GhostTransfmFrame = 0;//变形特效的帧数
    private bool startDrawTRnpc = false;
    private bool startDrawDCnpc = false;
    private bool delay = false;
    private bool shouldTeleport;
    private int TeleportAlpha = 255;//渐变时控制透明度
    public override string Texture => AssetsLoader.TransparentImg;
    public override void SetDefaults()
    {
        GhostTransfmDic = AssetsLoader.fxAtlas["FXghostTransformation"];
        Projectile.width = 30;
        Projectile.height = 30;
        Projectile.tileCollide = false;
        Projectile.hostile = true;
        Projectile.timeLeft = 450;
        Projectile.ignoreWater = true;
    }

    public override void OnSpawn(IEntitySource source)
    {
        SoundEngine.PlaySound(AssetsLoader.purpleDLC_scytheGhost_spawn);
        Projectile.velocity.X = (Main.rand.Next(0, 2) * 2 - 1) * Main.rand.NextFloat(1.49f, 1.94f);
        Projectile.velocity.Y = Main.rand.NextFloat(-0.3f, 0.31f);
    }

    public override void AI()
    {
        if (GhostTransfmFrame < GhostTransfmDic.Count && delay)
            GhostTransfmFrame++;
        delay = !delay;//放慢一倍绘制变身特效


        // 最大寻敌距离为1000像素
        float distanceMax = 1000f;

            float currentDistance = Vector2.Distance(Projectile.Center, player.Center);
            // 如果npc距离比当前最大距离小
            if (currentDistance < distanceMax)
            {
                // 就把最大距离设置为npc和玩家的距离
                // 并且暂时选取这个npc为距离最近npc
                distanceMax = currentDistance;
                target = player;
            }
        // 如果找到符合条件的npc 且 变鬼动画结束
        if (target != null && startDrawTRnpc)
        {
            Projectile.friendly = true;
            Vector2 targetVel = Vector2.Normalize(target.Center - Projectile.Center);
            if (ShouldExtraMove)//发现时位移一段距离
            {
                SoundEngine.PlaySound(AssetsLoader.purpleDLC_scytheGhost_charge);
                Projectile.velocity = (-10 * targetVel).RotateRandom(MathHelper.Pi / 4);
                ShouldExtraMove = false;
            }
            targetVel *= 7f;
            // X分量的加速度
            float accX = 0.36f;
            // Y分量的加速度
            float accY = 0.36f;
            Projectile.velocity.X += (Projectile.velocity.X < targetVel.X ? 1.15f : -1.15f) * accX;
            Projectile.velocity.Y += (Projectile.velocity.Y < targetVel.Y ? 1.15f : -1.15f) * accY;
            Projectile.timeLeft++;


            if (!target.active)//死了清零
            {
                target = null;
                ShouldExtraMove = true;
            }
        }
        if (target == null)
        {
            ShouldExtraMove = true;
            if (Projectile.velocity.Length() > 1.78f)
            {
                Projectile.velocity *= 0.56f;
            }
            if ((Projectile.Center - player.Center).Length() > Main.screenHeight - 280)
            {
                shouldTeleport = true;
            }
        }
        if(shouldTeleport)
        {
            TeleportAlpha -= 28;
            if(TeleportAlpha < 20)
            {
                SoundEngine.PlaySound(AssetsLoader.purpleDLC_scytheGhost_teleport_release);
                Projectile.position = player.Center + 32 * new Vector2(Main.rand.Next(3, 8) * (Main.rand.Next(0, 2) * 2 - 1), Main.rand.Next(3, 8) * (Main.rand.Next(0, 2) * 2 - 1));
                Projectile.velocity.X = (Main.rand.Next(0, 2) * 2 - 1) * Main.rand.NextFloat(1.49f, 1.94f);
                Projectile.velocity.Y = Main.rand.NextFloat(-0.4f, 0.3f);
                shouldTeleport = false;
            }
        }
        else
        {
            if (TeleportAlpha < 255)
                TeleportAlpha += 28;
        }

        if(TeleportAlpha > 255)
            TeleportAlpha = 255;
    }

    public override void PostDraw(Color lightColor)
    {
        SpriteBatch sb = Main.spriteBatch;
        DrawGhostTrasformEffect(GhostTransfmDic, sb);
        DrawTRnpc(sb);
        
    }
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.SetCrit();
        SoundEngine.PlaySound(AssetsLoader.purpleDLC_scytheGhost_explosion_hit);
        Projectile.NewProjectileDirect(Entity.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RoundTwist2>(), 0, 0, player.whoAmI);
    }

    private void DrawGhostTrasformEffect(Dictionary<int, DCAnimPic> fxDic, SpriteBatch sb)
    {
        if (!startDrawTRnpc && !startDrawDCnpc && GhostTransfmFrame < fxDic.Count)
        {
            Rectangle rectangle = new(fxDic[GhostTransfmFrame].x, fxDic[GhostTransfmFrame].y,
                                                        fxDic[GhostTransfmFrame].width, fxDic[GhostTransfmFrame].height);
            Vector2 vector = new Vector2(fxDic[GhostTransfmFrame].originalWidth / 2 //参考解包图片如果在大图里是如何绘制的
                                                            - fxDic[GhostTransfmFrame].offsetX,

                                                             fxDic[GhostTransfmFrame].originalHeight / 2
                                                             - fxDic[GhostTransfmFrame].offsetY);


            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

            sb.Draw(AssetsLoader.ChooseCorrectAnimPic(fxDic[GhostTransfmFrame].index, fxWeapon: true),
                    Projectile.Center - Main.screenPosition,
                    rectangle,
                    Color.White,
                    Projectile.rotation,
                    vector,
                    Projectile.scale,
                    0,
                    0f);

            sb.End();
            sb.Begin();
        }
        if (GhostTransfmFrame >= 15)
        {
            if (Projectile.ai[1] == 1)
                startDrawTRnpc = true;
            else if (Projectile.ai[1] == 2)
                startDrawDCnpc = true;
        }
    }
    
    private void DrawTRnpc(SpriteBatch sb)
    {
        if (startDrawTRnpc)
        {
            int type = (int)Projectile.ai[0];
            Texture2D texture = TextureAssets.Npc[type].Value;
            int frameNum = Main.npcFrameCount[type];
            Rectangle framecut = texture.Frame(verticalFrames: frameNum, frameY: (int)Main.GameUpdateCount / 6 % frameNum);

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

            sb.Draw(texture,
                Projectile.TopLeft - Main.screenPosition,
                framecut,
                new(255, 255, 255, TeleportAlpha),
                0f,
                texture.Size() / new Vector2(1, frameNum) * 0.5f,
                1f,
                SpriteEffects.None,
                0f);

            sb.End();
            sb.Begin();
        }
        else if(startDrawDCnpc)//需要大补充，绘制DCNPC
        {
            int type = (int)Projectile.ai[0];
            Texture2D texture = TextureAssets.Npc[type].Value;
            int frameNum = Main.npcFrameCount[type];
            Rectangle framecut = texture.Frame(verticalFrames: frameNum, frameY: (int)Main.GameUpdateCount / 6 % frameNum);

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

            sb.Draw(texture,
                Projectile.TopLeft - Main.screenPosition,
                framecut,
                new(255, 255, 255, TeleportAlpha),
                0f,
                texture.Size() / new Vector2(1, frameNum) * 0.5f,
                1f,
                SpriteEffects.None,
                0f);

            sb.End();
            sb.Begin();
        }
    }
}