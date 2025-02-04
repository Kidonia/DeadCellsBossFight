using DeadCellsBossFight.Contents.SubWorlds;
using DeadCellsBossFight.Core;
using DeadCellsBossFight.Projectiles.EffectProj;
using DeadCellsBossFight.Projectiles.NPCsProj;
using DeadCellsBossFight.Utils;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System;
using Terraria;
using Terraria.ModLoader;

namespace DeadCellsBossFight.NPCs.ExtraBosses.Queen;

public partial class Queen : ModNPC
{
    /// <summary>
    /// 玩家与女王的水平距离。玩家在女王左侧为负数
    /// </summary>
    public float distanceX;

    /// <summary>
    /// 释放技能时要固定的方向，一些技能无需限制（如抓取子弹） 
    /// </summary>
    public int ForceDirection;

    /// <summary>
    /// 子世界中间场地最左边界
    /// </summary>
    public const int FieldLeft = 1356;
    /// <summary>
    /// 子世界中间场地最右边界
    /// </summary>
    public const int FieldRight = 4832;

    /// <summary>
    /// 子世界中间场地地面纵坐标
    /// </summary>
    public const int FieldGround = 1504;
    /// <summary>
    /// 子世界虚空表面的纵坐标，女王落下来要扣血+传送
    /// </summary>
    public const int FieldBottom = 1888;

    private Vector2[] oldPosi = new Vector2[16]; //示例中记录16个坐标用于绘制
    private bool ShouldDrawTrail;
    /// <summary>
    /// 当前技能执行前无需靠近玩家
    /// </summary>
    public bool noDashSkill;

    /// <summary>
    /// 当前技能手也要有黑色烟雾
    /// </summary>
    public bool drawHandSmokeSkill;
    

    /// <summary>
    /// 处于次元斩阶段
    /// </summary>
    public bool IsDoingSplitAI;

    public bool preSplit;
    public bool Spliting;
    public int SplitingTime;
    public bool postSplit;
    public void QNAI_splitScreen()
    {
        NPC.direction = ForceDirection;
        UpdateDrawTrail();
        if (preSplit)
        {
            ChooseCorrectFrame1(QueenAnims.idleMassivecuts);
            NPC.noGravity = true; // 免受重力影响
            NPC.velocity.Y = -1.9f; // 向上飘浮
            //进入切割
            if (NPC.ai[3] == 20)
            {
                NPC.velocity *= 0f;
                preSplit = false;
                Spliting = true;
                SplitingTime = 450 - HP_level / 3 * 360;
                postSplit = false;
                //Main.NewText(HP_level.ToString());
                if(HP_level == 1)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center, Vector2.Zero, 
                        ModContent.ProjectileType<QueenSplitBlueLine>(), 70, 2f, -1, 5, MathHelper.ToRadians(Main.rand.NextFloat(12, 168)), 1);
                else if(HP_level == 2)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center, Vector2.Zero,
                        ModContent.ProjectileType<QueenSplitBlueLine>(), 110, 2.5f, -1, 10, MathHelper.ToRadians(Main.rand.NextFloat(12, 168)), 2);
                else if(HP_level == 3)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center, Vector2.Zero,
                        ModContent.ProjectileType<QueenSplitBlueLine>(), 240, 2.5f, -1, 20, MathHelper.ToRadians(Main.rand.NextFloat(12, 168)), 3);
            }
        }
        else if (Spliting)
        {
            ChooseCorrectFrame1Loop(QueenAnims.massivecuts);
            float shakeT = (float)Math.Sin(Main.GameUpdateCount / 36f) / 2;
            NPC.velocity.Y = shakeT; // 上下浮动效果。
            if (SplitingTime > 0) SplitingTime--;
            if(SplitingTime <= 0)
            {
                NPC.velocity.Y = 1.9f;
                preSplit = false;
                Spliting = false;
                SplitingTime = 0;
                postSplit = true;
                ReplayFrame();
            }

        }
        else if(postSplit)
        {
            ChooseCorrectFrame1(QueenAnims.massivecutsIdle);
            if (NPC.ai[3] == 20)
            {
                NPC.velocity *= 0f;
                NPC.noGravity = false;
                preSplit = false;
                Spliting = false;
                postSplit = false;
                ReplayFrame();
                EndDrawTrail();
                IsDoingSplitAI = false;

                if (Math.Abs(distanceX) < 560)
                {
                    CheckAddDash(farAwayDash : true);
                }
                else
                    ChangeState(QueenStates.waiting);
                fxdic = null;
                waitingTime += 50 - 15 * HP_level;
            }
        }
    }
    public bool HPLevelChanged;
    public bool shouldStandUp;
    /// <summary>
    /// 起立，然后进入次元斩
    /// </summary>
    public void QNAI_StandUp()
    {
        NPC.direction = ForceDirection;
        NPC.velocity *= 0;
        ChooseCorrectFrame1(QueenAnims.idleDEFENSEIdle);
        // 次元斩前的站立

        if (waitingTime > 0) waitingTime--;
        if (waitingTime <= 24)
        {
            if (IsDoingSplitAI)
            {
                CheckAddDash(middleDash: true);
                return;
            }

            // 清理地面炮塔
            if ((NPC.ai[0] == 1 || NPC.ai[0] == 2) && NPC.ai[1] >= 0)
            {
                sentryProj = Main.projectile[(int)NPC.ai[1]];
                if (!sentryProj.active)
                {
                    NPC.ai[0] = -1;
                    NPC.ai[1] = -1;
                    limitplayer.StopSentryItemUse = false;
                    return;
                }
                waitingTime = 0;
                // 禁用炮塔召唤，防止召唤新的替换掉要处理的炮塔
                limitplayer.StopSentryItemUse = true;
                ChangeState(QueenStates.destroyTurret);
                return;
            }



            // 煞笔检测
            // 玩家在绳子上
            if (player.pulley && Math.Abs(distanceX) < 800)
            {
                Main.NewText("别再躲躲藏藏！给我过来好好战斗！", Color.Aquamarine);
                ChangeState(QueenStates.killRopes);
                limitplayer.StopRopeItemUse = true;
                limitplayer.StopRopeItemUseTime += 12 * 60 + HP_level * 40;
                player.pulley = false;
                return;
            }

            // 清理仆从
            if (DCWorldSystem.hasPlayerMinionProj && killPetMinionCD == 0 && Main.rand.NextBool(20 - HP_level))
            {
                Main.NewText("瞧这些愚昧无知的生灵与器械为你苦苦卖命......真是碍眼。", Color.Aquamarine);
                ChangeState(QueenStates.killPet);
                player.velocity += new Vector2(ForceDirection * 2, 0);
                limitplayer.StopMinionPetUseTime = 420 + HP_level * 20;
                killPetMinionCD = 600 - HP_level * 30;
                return;
            }
            // 玩家在场地下方
            if (player.Bottom.Y > FieldGround)
            {
                //Main.NewText("under");
            }

        }

        // 玩家来到另一侧
        if(distanceX * ForceDirection < 0)
        {
            ChangeState(QueenStates.waiting);
        }

    }

    public int parryTime;
    public int parryCD;
    public int parryProjIndex;
    public void QNAI_parry()
    {
        NPC.direction = ForceDirection;
        noDashSkill = true;
        NPC.velocity.X = 0;
        ChooseCorrectFrame1Loop(QueenAnims.idleDEFENSE);
        if (parryTime > 0)
        {
            parryTime--;
            if(NPC.velocity.Y > 0 && parryProjIndex > -1 && Main.projectile[parryProjIndex].active && Main.projectile[parryProjIndex].type == ModContent.ProjectileType<QueenParryArea>())
            {
                Main.projectile[parryProjIndex].position = NPC.Center + new Vector2(0, 40);
            }
        }
        if(parryTime <= 0)
        {
            ChangeToNextSkill();
        }
        // 切换状态去OnHitByItem()里面写
    }
    /// <summary>
    /// 这个是招架近战的反击，不要再记错了
    /// </summary>
    public void QNAI_stompAnswer()
    {
        NPC.direction = ForceDirection;
        noDashSkill = true;
        fxdelay = 2;
        ChooseCorrectFrame2(QueenAnims.idleDEFENSEParry, QueenAnims.groundStomp, Queenfx.fxGroundStomp, 0, ModContent.ProjectileType<QueenstompAnswer>(), 32, 96, 50 + 30 * HP_level);
    }

    /// <summary>突刺前的延迟时间</summary>
    public int CollectorDashDelay;

    Projectile lungeProj;
    public void QNAI_lungeAttack()
    {
        fxdelay = 14;
        noDashSkill = true;
        NPC.direction = ForceDirection;
        ChooseCorrectFrame2(QueenAnims.idleLoadLunge, QueenAnims.lunge, Queenfx.fxCollectorDash);
        // 特判，添加突刺
        
        if(isDrawingSecondDic)
        {
            if (NPC.ai[3] == 0)
            {
                NPC.velocity += new Vector2(76 * ForceDirection, 0);
                lungeProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<QueenlungeAttack>(), 65 + HP_level * 25, 0, -1, NPC.direction * 18f);

                NPC.ai[3]++;

            }
            if(NPC.velocity.X * ForceDirection <0)
                NPC.velocity.X *= 0;
            if (NPC.Left.X < FieldLeft || NPC.Right.X > FieldRight)
            {
                NPC.velocity.X *= 0;
            }

            int offX = NPC.direction > 0 ? 0 : NPC.width - lungeProj.width;
            lungeProj.position = NPC.TopLeft + new Vector2(108 * NPC.direction + offX, 64);
        }
        
    }
    public void QNAI_removeRoot()
    {
        noDashSkill = false;
        NPC.direction = ForceDirection;
        ChooseCorrectFrame2(QueenAnims.loadAllAround, QueenAnims.allAround, Queenfx.fxAllAround, 0, ModContent.ProjectileType<RoundTwistQueen>(), 0, NPC.height, 75 + HP_level * 15);
    }
    public int downAtkCD;
    public void QNAI_ccStrongOvershield()
    {
        fxdelay = 26;
        fxoffsety = -64;
        noDashSkill = false;
        NPC.direction = ForceDirection;
        ChooseCorrectFrame2(QueenAnims.loadOvershield, QueenAnims.overshield, Queenfx.fxOvershield);
        
        // 狗日的，就你搞特殊
        if(NPC.ai[3] == 14 && downAtkCD == 0)
        {
            downAtkCD += 60;////
            Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<QueenccStrongOvershield>(), 100 + HP_level * 25, 0);

            int offX = NPC.direction > 0 ? 0 : NPC.width - proj.width;
            proj.position = NPC.TopLeft + new Vector2(64 * NPC.direction + offX, -10);

        }


    }
    public int fireCloudCD;
    public void QNAI_dispelAOE()
    {
        noDashSkill = true;
        NPC.direction = ForceDirection;
        ChooseCorrectFrame2(QueenAnims.loadPoisonCloud, QueenAnims.poisonCloud, Queenfx.None);

        //Main.NewText(NPC.ai[3]);
        if (NPC.ai[3] == 0 && !isDrawingLoadDic && isDrawingSecondDic && fireCloudCD == 0)// 前置动作结束
        {

            fireCloudCD += 60;////
            int offX = NPC.direction > 0 ? 0 : NPC.width - 48;
            Vector2 pos = NPC.TopLeft + new Vector2(120 * NPC.direction + offX, -220); 
            Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, new Vector2(NPC.direction * 18, 0), ModContent.ProjectileType<QueendispelAOE>(), 60 + HP_level * 20, 8, -1, NPC.direction);
        }
    }
    public int fireCD;
    public void QNAI_firewave()
    {
        noDashSkill = true;
        NPC.direction = ForceDirection;
        
        if (NPC.ai[3] == 9 && isDrawingLoadDic && !isDrawingSecondDic && fireCD == 0)// 前置动作结束
        {
            fireCD += 60;////
            Vector2 btmpos = NPC.direction > 0 ? NPC.BottomRight : NPC.BottomLeft;
            Point bottom = btmpos.ToTileCoordinates();
            for (int j = 0; j < 3; j++)// 检测脚下砖块，避免空砖
            {
                Tile spawnTile = NormalUtils.CheckTileInPosition(bottom.X, bottom.Y + j);
                if (spawnTile.HasTile)  // 有物块，不要检测树什么的，生成便是，碰撞交给弹幕AI
                {
                    Point posPoint = new(bottom.X, bottom.Y + j);
                    Vector2 pos = posPoint.ToWorldCoordinates() + new Vector2(0, -18);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, Vector2.Zero, ModContent.ProjectileType<FirePillar>(), 65 + HP_level * 20, 2, -1, 0, 1, 0);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, Vector2.Zero, ModContent.ProjectileType<FirePillar>(), 65 + HP_level * 20, 2, -1, 0, 0, 1);
                    break;
                }
            }
        }

        ChooseCorrectFrame2(QueenAnims.loadFirewave, QueenAnims.firewave, Queenfx.None);
    }
    public void QNAI_shockWave()
    {
        noDashSkill = true;
        NPC.direction = ForceDirection;
        ChooseCorrectFrame2(QueenAnims.loadShockWave, QueenAnims.shockWave, Queenfx.fxShockwave, 0, ModContent.ProjectileType<QueenshockWave>(), NPC.width / 2, NPC.height / 2, 85 + HP_level * 15);
    }
    public void QNAI_ccStrongComboA()
    {
        noDashSkill = true;
        NPC.direction = ForceDirection;
        shouldToComboB = true;
        ChooseCorrectFrame2(QueenAnims.loadThrust, QueenAnims.thrust, Queenfx.fxThrust, 0, ModContent.ProjectileType<QueenccStrongComboA>(), 40, 120, 65 + HP_level * 15);
    }
    private bool shouldToComboB;
    public void QNAI_ccStrongComboB()
    {
        //Main.NewText("BB");
        if (shouldToComboB)
        {
            shouldToComboB = false;
            noDashSkill = true;
            EndDrawTrail();
            isDrawingLoadDic = false;
            isDrawingSecondDic = true;
            ChooseCurrentDic(AssetsLoader.QNanimAtlas[QueenAnims.estocArc.ToString()], AssetsLoader.QNfxAtlas[Queenfx.fxEstocArc.ToString()]);// 切换到load动画

            Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<QueenccQuickFull>(), 50 + HP_level * 5, 0);

            int offX = NPC.direction > 0 ? 0 : NPC.width - proj.width;
            proj.position = NPC.TopLeft + new Vector2(188 * NPC.direction + offX, -100);

        }
        //

        NPC.direction = ForceDirection;
        ChooseCorrectFrame2(QueenAnims.loadEstocArc, QueenAnims.estocArc, Queenfx.fxEstocArc);

    }
    public void QNAI_ccQuickHigh()
    {
        fxdelay = 6;
        noDashSkill = false;
        NPC.direction = ForceDirection;
        ChooseCorrectFrame2(QueenAnims.loadEstocHead, QueenAnims.estocHead, Queenfx.fxEstocHead, 0, ModContent.ProjectileType<QueenccQuickHigh>(), 50, 172, 70 + HP_level * 10);
        //Main.NewText(NPC.ai[3]);
    }

    public void QNAI_ccQuickLow()
    {
        fxdelay = 6;
        noDashSkill = false;
        NPC.direction = ForceDirection;
        ChooseCorrectFrame2(QueenAnims.loadEstocFeet, QueenAnims.estocFeet, Queenfx.fxEstocFeet, 0, ModContent.ProjectileType<QueenccQuickLow>(), 108, 250, 55 + HP_level * 10);
    }

    public void QNAI_ccQuickFull()
    {
        fxdelay = 6;
        noDashSkill = false;
        NPC.direction = ForceDirection;
        ChooseCorrectFrame2(QueenAnims.loadEstocArc, QueenAnims.estocArc, Queenfx.fxEstocArc, 0, ModContent.ProjectileType<QueenccQuickFull>(), 188, -100, 60 + HP_level * 10);
    }

    public void QNAI_taunt()
    {
        noDashSkill = true;
        NPC.direction = ForceDirection;
        ChooseCorrectFrame2(QueenAnims.loadTaunt, QueenAnims.taunt, Queenfx.None);

    }

    /// <summary>等待的时间</summary>
    public int waitingTime;
    /// <summary>用完技能撤离后的按兵不动</summary>
    public void QNAI_wait()
    {
        //Main.NewText(NPC.ai[3]);
        ChooseCorrectFrame1Loop(QueenAnims.idleDEFENSE);
        //CurrentState = QueenSkills.gapCloser;
        if (waitingTime < 20)
        {
            // 清理地面炮塔
            if ((NPC.ai[0] == 1 || NPC.ai[0] == 2) && NPC.ai[1] >= 0)
            {
                sentryProj = Main.projectile[(int)NPC.ai[1]];
                if (!sentryProj.active)
                {
                    NPC.ai[0] = -1;
                    NPC.ai[1] = -1;
                    limitplayer.StopSentryItemUse = false;
                    return;
                }
                waitingTime = 0;
                // 禁用炮塔召唤，防止召唤新的替换掉要处理的炮塔
                limitplayer.StopSentryItemUse = true;
                ChangeState(QueenStates.destroyTurret);
                return;
            }


            if(DCWorldSystem.hasPlayerMinionProj && killPetMinionCD == 0 && Main.rand.NextBool(20 - HP_level))
            {
                Main.NewText("瞧这些愚昧无知的生灵与器械为你苦苦卖命......真是碍眼。", Color.Aquamarine);
                ChangeState(QueenStates.killPet);
                player.velocity += new Vector2(ForceDirection * 2, 0);
                limitplayer.StopMinionPetUseTime = 420 + HP_level * 20;
                killPetMinionCD = 600 - HP_level * 30;
                return;
            }
        }
        justFarAwayBorder = false;
        if (waitingTime > 0) waitingTime--;
        if (waitingTime <= 0)
        {
            //waitingTime = 50;
            ChangeState(QueenStates.gapCloser);
        }
        if (SubworldSystem.IsActive<QueenArenaWorld>() && (NPC.Left.X < FieldLeft || NPC.Right.X > FieldRight))
        {
            CheckAddDash(farAwayBorder:true);
        }
    }

    /// <summary> 正常走向玩家，距离、速度随阶段增强</summary>
    public void QNAI_gapCloser()
    {
        ChooseCorrectFrame1Loop(QueenAnims.dash);
        // 检测到血量变化，进入次元斩
        if (HPLevelChanged)
        {
            HPLevelChanged = false;
            // 准备次元斩阶段
            IsDoingSplitAI = true;
            //先立正
            ChangeState(QueenStates.standUp);
            // 一起用
            waitingTime += 40 - 6 * HP_level;

            ReplayFrame();
            //清空技能列表
            QueenSkillSlot_State1.Clear();
            return;
        }

        // 正常走向玩家，距离、速度随阶段增强
        if (Math.Abs(distanceX) > 180 - HP_level * 10)
        {
            NPC.velocity.X = Math.Sign(distanceX) * (9.1f + HP_level * 1.8f);
            if(NPC.Left.X < FieldLeft || NPC.Right.X > FieldRight)
            {
                NPC.velocity.X = 0;
                // 准备次元斩阶段
                // IsDoingSplitAI = true;

                //先立正
                ChangeState(QueenStates.standUp);
                // 一起用
                waitingTime += 60 - 10 * HP_level;
            }
        }
        else
        {
            // 追上玩家，开展攻击
            // 速度清零
            NPC.velocity.X *= 0;
            //Main.NewText("change");
            //ChangeToNextSkill();
            if (QueenSkillSlot_State1.Count > 0)
            {
                ChangeState(QueenSkillSlot_State1[0]);
            }
        }

    }
    /// <summary>次元斩时女王处于的世界中间位置</summary>
    public float gapMiddlePosX;

    /// <summary>瞬移的持续时间</summary>
    private int dashTime;
    /// <summary>瞬移了几帧了</summary>
    private int dashProgress;

    /// <summary>瞬移的起点，由女王位置确定</summary>
    public float DashStartX;

    /// <summary> 瞬移的终点，由玩家位置确定</summary>
    public float DashPosX;
    public bool FarAwayDash;
    public bool justFarAwayBorder;
    /// <summary>
    /// 瞬身到玩家附近
    /// </summary>
    /// <param name="backdash"></param>
    public void QNAI_Dash(bool backdash, bool middledash = false)
    {
        NPC.direction = ForceDirection;// 限制朝向
        NPC.position = new(MathHelper.Lerp(DashStartX, DashPosX, 1.5f - 12f / (dashProgress + 8)) - 0.5f * NPC.width, NPC.oldPosition.Y);
        UpdateDrawTrail();
        if (backdash)
            ChooseCorrectFrame1Loop(QueenAnims.backDash);
        else
            ChooseCorrectFrame1Loop(QueenAnims.dash);

        dashProgress++;
        // 冲刺结束
        if (dashProgress > dashTime)
        {
            dashProgress = 0;
            DashStartX = 0;
            DashPosX = 0; 
            if(middledash)
            {
                ChangeState(QueenStates.splitScreenTest);
                preSplit = true;
                Spliting = false;
                postSplit = false;
                ReplayFrame();
                //Main.NewText("结束");
            }
            else
                ChangeToNextSkill();
        }


    }

    /// <summary>
    /// 检测是否冲刺到玩家面前或身后，三连击等等攻击衔接时用，一帧限制
    /// </summary>
    public void CheckAddDash(bool middleDash = false, bool farAwayDash = false, bool farAwayBorder = false)
    {
        dashTime = 16 - HP_level * 2;
        //Main.NewText(QueenSkillSlot_State1.Count);
        if (middleDash)// 冲到世界中央
        {
            //追赶动作
            ChangeState(QueenStates.middleDash);
            /////
            dashTime = 20;
            ForceDirection = Math.Sign(gapMiddlePosX - NPC.Center.X);
            DashPosX = gapMiddlePosX;
        }


        else if(farAwayBorder)
        {
            ChangeState(QueenStates.backDash);

            int middleCheck = Math.Sign(NPC.Center.X - (FieldRight - FieldLeft)/2);
            DashPosX = NPC.position.X + middleCheck *480;

        }
        // 玩家在女王左侧 且 往左跑，或，玩家在女王右侧 且 往右跑
        else if (NPC.direction * (player.velocity.X + player.direction) > 0)
        {
            // 远离玩家的撤步
            if (farAwayDash)
            {
                ChangeState(QueenStates.backDash);
                //后撤步要改变朝向

                DashPosX = player.Center.X - 520 * ForceDirection;
            }
            // 80%概率后撤冲刺
            else if (Main.rand.Next(10) < 8)
            {
                ChangeState(QueenStates.backDash);
                //后撤步要改变朝向
                ForceDirection = -ForceDirection;
                DashPosX = player.Center.X - 220 * ForceDirection;
            }
            else
            {
                //20%概率追赶冲刺
                ChangeState(QueenStates.gapDash);
                /////
                DashPosX = player.Center.X - 180 * ForceDirection;
            }
        }
        
        // 玩家在女王左侧 且 往左跑，或，玩家在女王右侧 且 往右跑
        else if (NPC.direction * (player.velocity.X + player.direction) < 0)
        {

            // 远离玩家的撤步
            if (farAwayDash)
            {
                ChangeState(QueenStates.backDash);
                DashPosX = player.Center.X - 520 * ForceDirection;
            }
            // 60%概率后撤冲刺
            else if (Main.rand.Next(10) < 6)
            {
                ChangeState(QueenStates.backDash);
                ////
                ForceDirection = -ForceDirection;
                DashPosX = player.Center.X - 220 * ForceDirection;
            }
            else
            {
                //40%概率追赶冲刺
                ChangeState(QueenStates.gapDash);
                /////
                DashPosX = player.Center.X - 180 * ForceDirection;
            }
        }

        DashStartX = NPC.Center.X;

        // 别冲到灯塔外面
        if (SubworldSystem.IsActive<QueenArenaWorld>())
        {
            if (DashPosX < FieldLeft)
                DashPosX = FieldLeft + 150;
            else if (DashPosX > FieldRight)
                DashPosX = FieldRight - 150;
        }
        dashProgress = 0;
        // 冲刺需要绘制残影 
        ActivateDrawTrail();
    }

    /// <summary> 开启绘制残影，一帧限制</summary>
    public void ActivateDrawTrail()
    {
        // 需要绘制残影
        ShouldDrawTrail = true;
        // 清零存残影位置的向量数组
        oldPosi = new Vector2[16];
    }

    /// <summary>每帧更新残影位置，写在AI里</summary>
    public void UpdateDrawTrail()
    {
        if (ShouldDrawTrail)
        {
            for (int i = 15; i > 0; i--) //计数器，绘制拖尾用
            {
                oldPosi[i] = oldPosi[i - 1];
            }
            oldPosi[0] = NPC.Bottom;
        }
    }

    /// <summary>结束绘制残影，一帧限制</summary>
    public void EndDrawTrail()
    {
        ShouldDrawTrail = false;
        oldPosi = new Vector2[16];
    }

    /// <summary>
    /// 进入下一个技能，一帧限制
    /// </summary>
    public void ChangeToNextSkill()
    {
        ReplayFrame();
        EndDrawTrail();
        fxdic = null;
        fxdelay = 0;
        fxoffsetx = 0;
        fxoffsety = 0;
        if (grabbed)
        {
            grabbed = false;
        }
        //Main.NewText(QueenSkillSlot_State1.Count);

        // 先检测是否HP阶段切换，切换了进入次元斩
        if (HPLevelChanged)
        {
            HPLevelChanged = false;
            // 准备次元斩阶段
            IsDoingSplitAI = true;
            //先立正
            ChangeState(QueenStates.standUp);
            // 一起用
            waitingTime += 50 - 10 * HP_level;
            ReplayFrame();
            //清空技能列表
            QueenSkillSlot_State1.Clear();
            return;
        }

        // 玩家躲在绳子上
        if (player.pulley && Math.Abs(distanceX) < 800)
        {
            Main.NewText("别再躲躲藏藏！给我过来好好战斗！", Color.Aquamarine);
            ChangeState(QueenStates.killRopes);
            limitplayer.StopRopeItemUse = true;
            limitplayer.StopRopeItemUseTime = 12 * 60 + HP_level * 40;
            player.pulley = false;
            return;
        }

        // 玩家在女王位置下
        if (player.Bottom.Y > NPC.Bottom.Y)
        {
            // Main.NewText("low !");

            if (DCWorldSystem.hasPlayerGrappleHookProj && (leftNoSpike || rightNoSpike))
            {
                Main.NewText("挺灵巧的工具......不过别想逃避战斗！", Color.Aquamarine);
                ChangeState(QueenStates.killGrappleHooks);
                return;
            }

            if(player.mount.Active)
            {
                Main.NewText("杂鱼体力，你就靠着那可悲的坐骑逃吧。", Color.Aquamarine);
                player.mount.Dismount(player);
            }
        }

        if (QueenSkillSlot_State1.Count > 1)
        {
            QueenSkillSlot_State1.RemoveAt(0);
            ChangeState(QueenSkillSlot_State1[0]);
        }
        // 如果是最后一段攻击
        else if(QueenSkillSlot_State1.Count == 1)
        {
            //Main.NewText("最后一段结束！");

            QueenSkillSlot_State1.Clear();
            
            if(NPC.Left.X < FieldLeft + 100 || NPC.Right.X > FieldRight - 100)
            {
                CheckAddDash(farAwayBorder: true);
            }
            else if(Math.Abs(distanceX) < 560)
            {
                //if(NPC.Left.X - FieldLeft)
                
                CheckAddDash(farAwayDash:true);
            }
            else
                ChangeState(QueenStates.waiting);
            fxdic = null;
            waitingTime += 70 - 12 * HP_level;
        }
        else
        {
            //Main.NewText("ERROR:技能列表为空！");
            ChangeState(QueenStates.waiting);
            fxdic = null;
            waitingTime += 80 - 12 * HP_level;
        }
    }


    /// <summary>
    /// 改变状态时发生的事，应该注意限制只执行一帧
    /// </summary>
    /// <param name="state"></param>
    public void ChangeState(QueenStates state)
    {
        CurrentState = state;
        // 更改限制的朝向
        ForceDirection = Math.Sign(distanceX);
    }

}
