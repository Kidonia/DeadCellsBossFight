using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ModLoader;
using DeadCellsBossFight.Contents.Bosssbars;
using DeadCellsBossFight.Utils;
using DeadCellsBossFight.Projectiles.BasicAnimationProj;
using System;
using DeadCellsBossFight.Core;

namespace DeadCellsBossFight.NPCs;
[AutoloadBossHead]
public partial class BH : ModNPC
{
    //ai[1]用于检测是否要阶段切换，目前在白给的AI里更改

    //添加一个新AI：
    //1. 给BHMoveType添加新的一项
    //2. 在本页 AI() 函数的 switch (CurrentMove) 中添加该项的case
    //3. 在 BHbasicAI 或其他地方写上对应的AI函数
    //4. 留意调整 BHbasicAI 的 ChangeMove() 函数


    #region 字段/变量
    /// <summary>
    /// 记录当前的运动状态。一般每个状态对应一个AI。
    /// </summary>
    public BHMoveType CurrentMove;

    /// <summary>
    /// 记录当前细胞人是第几阶段
    /// </summary>
    public BHState CurrentState;

    /// <summary>
    /// 记录当前是第几段攻击，与CurrentWeaponAtkMode，CurrentWeaponType结合使用确定每一段攻击是什么
    /// </summary>
    public BHATKChain CurrentATKChain;

    /// <summary>
    /// 记录当前使用的武器的攻击模式，与CurrentATKChain，CurrentWeaponType结合使用确定每一段攻击是什么
    /// </summary>
    public BHWeaponAtkMode CurrentWeaponAtkMode;

    /// <summary>
    /// 记录当前使用的武器类型，与CurrentATKChain，CurrentWeaponAtkMode结合使用确定每一段攻击是什么
    /// </summary>
    public BHWeaponType CurrentWeaponType;
    /// <summary>
    /// 也是记录当前使用的武器类型，不为NoWeapon
    /// </summary>
    public BHWeaponType LastWeaponType;

    /// <summary>
    /// 测试用细胞人碰撞箱填充色。
    /// </summary>
    public Color testDrawStateColor;

    /// <summary>
    /// 玩家与NPC距离
    /// </summary>
    public float distance;

    /// <summary>
    /// 玩家与NPC水平距离（绝对值）
    /// </summary>
    public float X_abs_distance;

    /// <summary>
    /// 绘制残影，0不绘制，1绘制。用int方便传参给Proj.ai[]
    /// </summary>
    public int DrawTrail = 0;

    /// <summary>
    /// 状态
    /// </summary>
    public bool standing, walking, uping, downing;

    /// <summary>
    /// 头。火焰头的弹幕。不是飞头实体。
    /// </summary>
    public Projectile headProj;

    /// <summary>
    /// 玩家。
    /// </summary>
    public Player player;

    /// <summary>
    /// 存储基本移动动作弹幕。
    /// ai[0]用来控制是否绘制细胞人移动的残影（不是自带的额外洋葱皮残影）0是不绘制，1是自动添加并绘制。
    /// </summary>
    public Projectile basicMoveProj;
    #endregion

    public override void SetDefaults()
    {
        NPC.width = 80;
        NPC.height = 128;
        NPC.lifeMax = 258147;
        NPC.damage = 0;
        NPC.defense = 3;
        NPC.knockBackResist = 0f;
        NPC.value = 0;
        NPC.npcSlots = 10f;
        NPC.lavaImmune = false;
        NPC.boss = true;
        NPC.BossBar = ModContent.GetInstance<BHBossbar>();
        NPC.aiStyle = 0;

        base.SetDefaults();
    }
    public override void OnSpawn(IEntitySource source)
    {
        // headProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.position, Vector2.Zero, ModContent.ProjectileType<DCHeadFire>(), 0, 0, -1, ai0:0, ai2:1);
        // 禁用沙尘暴
        Sandstorm.StopSandstorm();

        DrawTrail = 0;
        CurrentATKChain = BHATKChain.NOATK;
        CurrentWeaponAtkMode = BHWeaponAtkMode.None;
        testDrawStateColor = Color.White;
        jumpCooldown = 0;
        rollCooldown = 0;
        secondJumpWaitTime = 28;
        rollImmuneTime = 48;
        lastThreeMovement = new BHMoveType[3] {0, 0, 0};
        extraMoveCheck = false;
        ClimbSide = 0;
        ClimbTime = 50;
        isClimbingWall = false;
        jumpWallTime = 0;
        ClimbWalking = false;
        // basicMoveProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<JumpDownProj>(), 0, 0, -1, DrawTrail);
        basicMoveProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<JumpDownProj>(), 0, 0, -1, DrawTrail);
        player = Main.LocalPlayer;
        DCWorldSystem.BH_active = true;
        DCWorldSystem.BH_whoAmI = NPC.whoAmI;
        ChangeMove(BHMoveType.SmashDown);
    }
    public override bool PreAI()
    {
        /*
        NPC.GravityMultiplier *= 6f;
        Main.NewText(NPC.GravityMultiplier.Value);
        */
        if(NPC.Bottom.Y > FieldGround)
            NPC.position.Y = FieldGround - NPC.height - 2;
        return base.PreAI();
    }

    public override void AI()
    {
        // Main.NewText(Main.MouseWorld);
        if (player == null || !player.active || player.dead || !NPC.HasPlayerTarget)
        {
            player = Main.player[NPC.target];
        }

        if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
        {
            NPC.TargetClosest(true);
        }


        
        distance = (player.Center - NPC.Center).Length();
        X_abs_distance = Math.Abs((player.Center - NPC.Center).X);
        // 禁用玩家飞行
        player.wingTime = 0;
        //添加创意震撼
        player.AddBuff(199, 7200);

        // Do not despawn.
        NPC.timeLeft = 7200;
        // return;

        if(CurrentMove == BHMoveType.HideAndSummon && NPC.ai[1] > 0) // 检测到阶段切换，且 正弦漂浮空中时
        {
            NPC.velocity.X *= 0;
            NPC.velocity.Y *= 0;
            NPC.noGravity = false;
            NPC.dontTakeDamage = false;
            CurrentState += 1; // 阶段加一
            ChangeMove(BHMoveType.Walk);

        }


        // EazyNewText(CurrentState, "CurrentState");
        switch (CurrentMove)
        {
            case BHMoveType.None:
                break;
            case BHMoveType.SwordAtk:
                AI_SwordAtk();
                testDrawStateColor = Color.DarkRed;
                break;
            case BHMoveType.HeavyAtk:
                AI_HeavyAtk();
                testDrawStateColor = Color.Green;
                break;
            case BHMoveType.BowAtk: 
                testDrawStateColor = Color.Blue;
                break;
            case BHMoveType.CrossbowAtk:
                testDrawStateColor = Color.Purple;
                break;
            case BHMoveType.HeadAtk:
                testDrawStateColor = Color.DarkGreen;
                break;
            case BHMoveType.ShieldBlock:
                testDrawStateColor = Color.AliceBlue;
                break;
            case BHMoveType.Stun:
                testDrawStateColor = Color.Gray;
                break;
            case BHMoveType.Heal:
                AI_Heal();
                testDrawStateColor = Color.Yellow;
                break;
            case BHMoveType.HideAndSummon:
                AI_HideAndSummon();
                testDrawStateColor = Color.Orange;
                break;
            case BHMoveType.Roll:
                testDrawStateColor = Color.Pink;
                AI_Roll();
                break;
            case BHMoveType.Walk:
                testDrawStateColor = Color.DeepSkyBlue;
                AI_Walk();
                break;
            case BHMoveType.Jump:
                testDrawStateColor = Color.DarkSalmon;
                AI_Jump();
                break;
            case BHMoveType.DoubleJump:
                testDrawStateColor = Color.DarkSalmon;
                AI_DoubleJump();
                break;
            case BHMoveType.SmashDown:
                AI_SmashDown();
                testDrawStateColor = Color.DarkRed;
                break;
            case BHMoveType.Idle:
                AI_Idle();
                testDrawStateColor = Color.White;
                break;
            case BHMoveType.ClimbWallAndSmashDown:
                AI_ClimbWallAndSmashDown();
                testDrawStateColor = Color.Red;
                break;
            case BHMoveType.SmallWalkBack:
                AI_SmallWalkBack();
                testDrawStateColor = Color.OrangeRed;
                break;
            case BHMoveType.Hesitate:
                AI_Hesitate();
                break;
            default:
                break;
        }

        /*
        for(int i = 0;i<3;i++)
            Main.NewText($"{i}:{lastThreeMovement[i].ToString()}");
        */

        //EazyNewText((Main.player[NPC.target].Center - NPC.Center).X, "Xdistance");
        //EazyNewText(atkTiming, "atkTiming");
        player.accDreamCatcher = true;
        //if(Main.updatesCountedForFPS % 30 == 0)
            //player.dpsStarted = true;
        //EazyNewText(Main.updatesCountedForFPS, "Main.updatesCountedForFPS");
        //EazyNewText(player.getDPS(), "dps");

        if (jumpCooldown > 0) jumpCooldown--;
        if (rollCooldown > 0) rollCooldown--;
        if(atkCoolDown > 0) atkCoolDown--;
        if(atkTiming > 0) atkTiming--;

        //更新阶段效果
        stateBonus = (int)CurrentState * 3;

        //ShowATKMessage();
        //前四阶段
        if (CurrentState != BHState.Fourth)
        {
            //血量低于100000，触发回血。
            if (NPC.life < 100000 && CurrentMove != BHMoveType.Heal)
            {
                if (atkTiming == 0)
                {
                    ClearWeaponState();
                    ChangeMove(BHMoveType.Heal);
                }
            }

            //锁血，除非最后阶段，不然免疫伤害。
            if (NPC.life < 10000)
            {
                NPC.dontTakeDamage = true;
            }
            // 行走 跳跃 二段跳 站立
            if ((CurrentMove >= (BHMoveType)11 && CurrentMove <= (BHMoveType)15) || extraMoveCheck)
            {
                walking = false;
                uping = false;
                downing = false;
                standing = false;

                if (basicMoveProj.type == ModContent.ProjectileType<WalkProj>())
                {
                    walking = true;
                }
                else if (basicMoveProj.type == ModContent.ProjectileType<JumpUpProj>())
                {
                    uping = true;
                }
                else if (basicMoveProj.type == ModContent.ProjectileType<JumpDownProj>())
                {
                    downing = true;
                }
                else if (basicMoveProj.type == ModContent.ProjectileType<IdleProj>())
                {
                    standing = true;
                }
                /*
                foreach (Projectile projectile in Main.projectile)
                {
                    if (projectile.type == ModContent.ProjectileType<WalkProj>())
                    {
                        walking = true;
                        break;
                    }
                    if (projectile.type == ModContent.ProjectileType<JumpUpProj>())
                    {
                        uping = true;
                        break;
                    }
                    if (projectile.type == ModContent.ProjectileType<JumpDownProj>())
                    {
                        downing = true;
                        break;
                    }
                    if (projectile.type == ModContent.ProjectileType<IdleProj>())
                    {
                        standing = true;
                        break;
                    }
                  
                }
                  */

                /*                if (walking) Main.NewText("walking");
                                if (uping) Main.NewText("uping");
                                if (downing) Main.NewText("downing");
                                if (standing) Main.NewText("standing");*/

                // Main.NewText(basicMoveProj.active);
                if (NPC.velocity.Y == 0 && !walking && CurrentMove == BHMoveType.Walk)
                {
                    basicMoveProj.Kill();
                    basicMoveProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<WalkProj>(), 0, 0, -1, DrawTrail);
                }
                else if (NPC.velocity.Y > 0 && !downing)
                {
                    basicMoveProj.Kill();
                    basicMoveProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<JumpDownProj>(), 0, 0, -1, DrawTrail);
                }
                else if(NPC.velocity.Y < 0 && !uping)
                {
                    basicMoveProj.Kill();
                    basicMoveProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<JumpUpProj>(), 0, 0, -1, DrawTrail);

                }

                else if (NPC.velocity.Y == 0 && !standing && CurrentMove == BHMoveType.Idle)
                {
                    basicMoveProj.Kill();
                    basicMoveProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<IdleProj>(), 0, 0, -1, DrawTrail);
                }
            }


        }
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        /*
        Vector2 pos = NPC.position - Main.screenPosition;
        Rectangle rectangle = new Rectangle((int)pos.X, (int)pos.Y, NPC.width, NPC.height);
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, rectangle, testDrawStateColor);
        */
        base.PostDraw(spriteBatch, screenPos, drawColor);
    }

    public override bool? CanBeHitByProjectile(Projectile projectile)
    {
        return ! NormalUtils.modBHProjTypeList.Contains(projectile.type);
    }
    public override bool? CanFallThroughPlatforms()
    {
        return Main.player[NPC.target].position.Y < NPC.position.Y;
    }
    public void EazyNewText(object o, string name, Color? color = null)
    {
        if (o is string)
        {
            Main.NewText(o, color);
            return;
        }
        Main.NewText($"{name}:{o}", color);
    }
}