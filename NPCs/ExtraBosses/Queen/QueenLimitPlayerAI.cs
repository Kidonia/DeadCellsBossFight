using DeadCellsBossFight.Projectiles.NPCsProj;
using DeadCellsBossFight.Utils;
using Microsoft.Xna.Framework;
using System;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using DeadCellsBossFight.Contents.Tiles;
using DeadCellsBossFight.Core;

namespace DeadCellsBossFight.NPCs.ExtraBosses.Queen;

public partial class Queen : ModNPC
{
    /// <summary>
    /// 未完成
    /// </summary>
    public void QNAI_disableGrenade()
    {
        fxdelay = 36;
        ChooseCorrectFrame2(QueenAnims.bulletStop, QueenAnims.cutGrenade, Queenfx.fxCutGrenade);
    }
    public int bulletStopCD;
    public void QNAI_repelBullets()
    {
        noDashSkill = true;
        ChooseCorrectFrame2(QueenAnims.bulletStop, QueenAnims.throwBullets, Queenfx.None,
            projType1: ModContent.ProjectileType<QueenRepelBullets>(), projOffX: 140);
        /*
       // 更新弹幕位置，史，写在弹幕AI里面了
        for (int i = 0; i < Main.projectile.Length; i++)
        {
            if (Main.projectile[i].type == ModContent.ProjectileType<QueenRepelBullets>())
            {
                Main.projectile[i].Center = (NPC.direction > 0 ? NPC.Right : NPC.Left) + new Vector2(140, 0) * NPC.direction;
            }
        }
        */
    }

    public int killPetMinionCD;
    /// <summary>
    /// 未完成
    /// </summary>
    public void QNAI_killPet()
    {
        NPC.direction = ForceDirection;
        ChooseCorrectFrame2(QueenAnims.bulletStop, QueenAnims.killPet, Queenfx.None, ModContent.ProjectileType<QueenKillPetSummonArea>(), projOffX : 70, projOffY : -30);
        if (NPC.ai[3] < 4)
            DCWorldSystem.hasPlayerMinionProj = false;
    }

    public bool grabbed;
    public bool drawGrabFrames;
    /// <summary>
    /// 未完成
    /// </summary>
    public void QNAI_grabHero()
    {
        noDashSkill = true;
        NPC.direction = ForceDirection;
        drawHandSmokeSkill = true;
        //Main.NewText(NPC.ai[3]);
        if (NPC.ai[3] < 5 && isDrawingSecondDic && !isDrawingLoadDic && !drawGrabFrames)
        {
            if (Collision.CheckAABBvAABBCollision(handProj.position, handProj.Hitbox.Size(), player.position, player.Hitbox.Size()))
            {
                grabbed = true;
                // Main.NewText(handProj.Hitbox.Size(), new Color((int)Main.GameUpdateCount % 200 + 20, 20, 50));
            }
        }

        if (grabbed)
        {
            isDrawingLoadDic = false;
            isDrawingSecondDic = false;
            drawGrabFrames = true;
            grabbed = false;
            limitplayer.ActivateScreenPosFrozen();
            limitplayer.grabbedByQueen = true;

            // limitplayer.forceScreenPosition = Main.screenPosition;
        }

        if (drawGrabFrames)
        {
            ChooseCorrectFrame2(QueenAnims.grab, QueenAnims.grabThrow, Queenfx.None);


            if (isDrawingLoadDic || (NPC.ai[3] < 11 && isDrawingSecondDic))
            {
                player.position = handProj.position;
                if (Main.GameUpdateCount % 15 == 0)
                {
                    int heal = Math.Min(NPC.lifeMax - NPC.life, 1200);
                    if (heal > 0)
                    {
                        NPC.HealEffect(heal, true);
                        NPC.life += heal;
                    }
                }
            }
            if (NPC.ai[3] == 11 && !isDrawingLoadDic && isDrawingSecondDic)
            {
                limitplayer.StopScreenPosFrozen();
                limitplayer.grabbedByQueen = false;
                player.velocity = new Vector2(16f * -ForceDirection, -8f);
            }
        }
        else
            ChooseCorrectFrame2(QueenAnims.loadGrab, QueenAnims.grabMiss, Queenfx.None);
    }

    public bool leftNoSpike;
    public bool rightNoSpike;
    /// <summary>摧毁玩家的勾爪，并给墙壁添加木桩尖刺</summary>
    public void QNAI_killGrappleHooks()
    {
        NPC.direction = ForceDirection;
        NPC.velocity.X = 0;
        ChooseCorrectFrame2(QueenAnims.loadDemine, QueenAnims.demine, Queenfx.fxDemine);

        if (isDrawingSecondDic)
        {
            if (leftNoSpike && player.Center.X < FieldLeft)
            {
                for (int j = 95; j < 100 + (int)NPC.ai[3]; j++)
                {
                    Tile tile2 = Main.tile[85, j];
                    if (tile2.HasTile)
                        tile2.ClearEverything();
                    tile2.HasTile = true;
                    tile2.TileType = TileID.WoodenSpikes;
                    WorldGen.SquareTileFrame(85, j);
                    WorldGen.paintTile(85, j, PaintID.BlackPaint);
                }

                for (int j = 95; j < 100 + (int)NPC.ai[3]; j++)
                {
                    Tile tile2 = Main.tile[84, j];
                    if ( j % 2 == 1)
                    {
                        if (tile2.HasTile)
                            tile2.ClearEverything();
                        tile2.HasTile = true;
                        tile2.TileType = TileID.WoodenSpikes;
                        WorldGen.SquareTileFrame(84, j);
                        WorldGen.paintTile(84, j, PaintID.BlackPaint);
                    }
                }
                if (NPC.ai[3] == 19)
                    leftNoSpike = false;
            }
            if (rightNoSpike && player.Center.X > FieldRight)
            {
                for (int j = 95; j < 100 + (int)NPC.ai[3]; j++)
                {
                    Tile tile2 = Main.tile[302, j];
                    if (tile2.HasTile)
                        tile2.ClearEverything();
                    tile2.HasTile = true;
                    tile2.TileType = TileID.WoodenSpikes;
                    WorldGen.SquareTileFrame(302, j);

                    WorldGen.paintTile(302, j, PaintID.BlackPaint);

                }

                for (int j = 95; j < 100 + (int)NPC.ai[3]; j++)
                {
                    Tile tile2 = Main.tile[303, j];
                    if (j % 2 == 1)
                    {
                        if (tile2.HasTile)
                            tile2.ClearEverything();
                        tile2.HasTile = true;
                        tile2.TileType = TileID.WoodenSpikes;
                        WorldGen.SquareTileFrame(303, j);
                        WorldGen.paintTile(303, j, PaintID.BlackPaint);
                    }
                }

                if (NPC.ai[3] == 19)
                    rightNoSpike = false;
            }
            if (NPC.ai[3] == 1 && isDrawingSecondDic)
            {
                player.StopGrappling();
                player.velocity += new Vector2(4 * ForceDirection, 4);
                DCWorldSystem.hasPlayerGrappleHookProj = false;
            }
        }

    }

    /// <summary>摧毁玩家呆在的绳子上，并添加绳索类物品使用冷却</summary>
    public void QNAI_killRopes()
    {
        ChooseCorrectFrame2(QueenAnims.bulletStop, QueenAnims.killPet, Queenfx.None);
        //Main.NewText(NPC.ai[3]);
        int centerX = (int)NPC.Center.X / 16;
        int centerY = (int)NPC.Center.Y / 16;
        if (isDrawingLoadDic && Main.GameUpdateCount % 8 == 1)
        {
            for (int k = 1; k < NPC.ai[3] *1.2; k++)
            {
                for (int m = -k; m < k; m++)
                {
                    int j = m + centerY;
                    if (j > 0)
                        for (int n = -k; n < k; n++)
                        {
                            int i = n + centerX;
                            if (i > 0 && Main.tile[i, j].HasTile && Main.tile[i, j].TileType != ModContent.TileType<TransparentTile>() && Main.tile[i, j].TileType != ModContent.TileType<DCNormalTile>())
                            {
                                Tile tile = TilemapExtensions.GetTileSafely(i, j);
                                if (tile.TileType == TileID.Rope || tile.TileType == TileID.SilkRope || tile.TileType == TileID.VineRope || tile.TileType == TileID.WebRope || tile.TileType == TileID.MysticSnakeRope)
                                {
                                    if (tile.TileColor != PaintID.BlackPaint)
                                    {
                                        WorldGen.paintTile(i, j, PaintID.BlackPaint);
                                        Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, DustID.Rope, Main.rand.NextFloat(-1f, 1f), -Main.rand.NextFloat(4f), 0, Color.Black);
                                    }
                                }
                            }
                        }
                }
            }
        }
        if (isDrawingSecondDic && NPC.ai[3] == 14 && Main.GameUpdateCount % 3 == 1)
        {
            for (int m = -48; m < 48; m++)
            {
                int j = m + centerY;
                if (j > 0)
                    for (int n = -48; n < 48; n++)
                    {
                        int i = n + centerX;
                        if (i > 0 && Main.tile[i, j].HasTile && Main.tile[i, j].TileType != ModContent.TileType<TransparentTile>() && Main.tile[i, j].TileType != ModContent.TileType<DCNormalTile>())
                        {
                            Tile tile = TilemapExtensions.GetTileSafely(i, j);
                            if (tile.TileType == TileID.Rope || tile.TileType == TileID.SilkRope || tile.TileType == TileID.VineRope || tile.TileType == TileID.WebRope || tile.TileType == TileID.MysticSnakeRope)
                            {
                                tile.ClearEverything();
                                Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, DustID.Rope, Main.rand.NextFloat(-1f, 1f), -Main.rand.NextFloat(4f), 0, Color.Black);
                                Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, DustID.Rope, Main.rand.NextFloat(-1f, 1f), -Main.rand.NextFloat(4f), 0, Color.Black);
                                Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, DustID.Rope, Main.rand.NextFloat(-1f, 1f), -Main.rand.NextFloat(4f), 0, Color.Black);

                            }
                        }
                    }
            }


        }
    }


    /// <summary>要摧毁的炮塔实体，并添加炮塔类物品使用冷却</summary>
    public Projectile sentryProj = new();
    public bool ableToDestroy;
    public void QNAI_destroyTurret()
    {
        if (!sentryProj.sentry)
        {
            NPC.ai[0] = -1;
            NPC.ai[1] = -1;
            limitplayer.StopSentryItemUse = false;
            waitingTime += 20;
            ChangeState(QueenStates.waiting);
            return;
        }
        float projNpcDistance = sentryProj.Center.X - NPC.Center.X;
        bool shouldToKill = false;
        NPC.direction = Math.Sign(projNpcDistance);
        //Main.NewText("要添加追踪炮塔");
        // 走到炮塔处

        if ((NPC.Left.X <= FieldLeft && projNpcDistance < -110) || (NPC.Right.X >= FieldRight && projNpcDistance > 110))
        {
            //Main.NewText("bord");
            NPC.velocity.X = 0;
            shouldToKill = true;
        }
        if (Math.Abs(projNpcDistance) > 110 && sentryProj.active && !shouldToKill)
        {
            NPC.velocity.X = NPC.direction * (10f + HP_level * 1.8f);
            ChooseCorrectFrame1Loop(QueenAnims.dash);
        }
        else
        {
            shouldToKill = true;
        }
        if (shouldToKill)
        {
            NPC.velocity *= 0f;
            ChooseCorrectFrame2(QueenAnims.loadDemine, QueenAnims.demine, Queenfx.fxDemine);
            if (NPC.ai[3] == 1 && isDrawingSecondDic && Main.GameUpdateCount % 3 == 1)
            {
                if (NPC.ai[0] == 1)
                {
                    if (sentryProj.active)
                        sentryProj.Kill();

                }
                if (NPC.ai[0] == 2)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), sentryProj.Center, Vector2.Zero, ModContent.ProjectileType<QueenSplitBlueLine>(), 0, 2.5f, -1, 1, MathHelper.ToRadians(Main.rand.NextFloat(80, 100)), 4);
                    sentryProj.aiStyle = 0;
                    // 控制和次元斩出现同时消失
                    sentryProj.timeLeft = 58;
                }
                NPC.ai[0] = -1;
                NPC.ai[1] = -1;
                limitplayer.StopSentryItemUseTime += 12 * 60 + HP_level * 40;
            }
        }

    }


}
