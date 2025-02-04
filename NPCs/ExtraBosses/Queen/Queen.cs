using DeadCellsBossFight.Contents.Bosssbars;
using DeadCellsBossFight.Contents.GlobalChanges;
using DeadCellsBossFight.Core;
using DeadCellsBossFight.Projectiles;
using DeadCellsBossFight.Projectiles.NPCsProj;
using DeadCellsBossFight.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DeadCellsBossFight.NPCs.ExtraBosses.Queen;

public partial class Queen : ModNPC
{
    // ai[0]用于控制是否执行摧毁炮塔，宠物：
    // 1：炮塔，2：空中炮塔，3: 宠物和召唤物

    // ai[1]用于存储要摧毁的东西的index

    //ai[2]用于存储当前进行的是第几段剧情，见QueenPlotAnimAI.cs

    // ai[3]用于控制当前是当前动作的第几帧

    /// <summary>
    /// 当前动画的字典
    /// </summary>
    private Dictionary<int, DCAnimPic> dic = new();

    /// <summary>
    /// 当前动画的字典
    /// </summary>
    private Dictionary<int, DCAnimPic> fxdic = new();

    // 追击的玩家
    private Player player;
    // 给玩家添加的限制，如禁用矿车等
    private DCPlayer limitplayer;
    // 检测是否存在勾爪，宠物，召唤物
    // private DeadCellsBossFight sys;

    public Projectile headProj;
    public Projectile handProj;

    private Vector2 drawVec;
    // 你猜我为什么不继承DC_BasicNPC？你猜我为什么要加这两个？
    //AI 每帧更新一次，PostDraw每帧更新三次，因为两张图，所以一旦出现图片切换，寄，我阐释你的梦
    private Texture2D CurrentTex;
    private Texture2D CurrentGlowTex;
    private bool isDrawingLoadDic;
    private bool isDrawingSecondDic;
    /// <summary>
    /// 用于播放连贯的第二个动作时，fx索引接上第一个字典的长度继续使用
    /// </summary>
    private int lastLoadDicCount;
    private int lastPicIndex;
    private int fxindex;
    private int fxdelay;
    private int fxoffsetx;
    private int fxoffsety;
    /// <summary>
    /// 女王要使用的丝滑小连招
    /// </summary>
    public List<QueenStates> QueenSkillSlot_State1 = new();

    private int ImmuneTime;
    public int Combo2_CD;
    public int CollectorDashCD;

    /// <summary>
    /// 记录当前的状态
    /// </summary>
    public QueenStates CurrentState;

    /// <summary>
    /// 记录女王血量的阶段
    /// High = 0,高
    /// Medium = 1,中
    /// Low = 2,低
    /// MegaLow = 3,劲爆结尾
    /// </summary>
    public int HP_level;

    /// <summary>
    /// 女王的所有动作的名称
    /// </summary>
    public enum QueenAnims
    {
        /// <summary>
        /// 不使用动作
        /// </summary>
        None = 0,
        /// <summary>
        /// 戳胸前置动作
        /// </summary>
        loadEstocHead,
        /// <summary>
        /// 戳胸动作
        /// </summary>
        estocHead,
        /// <summary>
        /// 刺脚前置动作
        /// </summary>
        loadEstocFeet,
        /// <summary>
        /// 刺脚动作
        /// </summary>
        estocFeet,
        /// <summary>
        /// 前半劈砍前置动作
        /// </summary>
        loadEstocArc,
        /// <summary>
        /// 前半劈砍动作
        /// </summary>
        estocArc,
        /// <summary>
        /// 下戳刺前置动作
        /// </summary>
        loadOvershield,
        /// <summary>
        /// 下戳刺动作
        /// </summary>
        overshield,
        /// <summary>
        /// 大剑第一段前置动作
        /// </summary>
        loadThrust,
        /// <summary>
        /// 大剑第一段出手动作
        /// </summary>
        thrust,
        /// <summary>
        /// 尝试抓取玩家前置动作
        /// </summary>
        loadGrab,
        /// <summary>
        /// 成功抓到玩家，从地上起立
        /// </summary>
        grab,
        /// <summary>
        /// 没抓到玩家站起来
        /// </summary>
        grabMiss,
        /// <summary>
        /// 把玩家扔出去
        /// </summary>
        grabThrow,
        /// <summary>
        /// 站立转到收藏家突刺的前置动作
        /// </summary>
        idleLoadLunge,
        /// <summary>
        /// 收藏家突刺动作
        /// </summary>
        lunge,
        /// <summary>
        /// 地火前置动作
        /// </summary>
        loadFirewave,
        /// <summary>
        /// 地火释放动作
        /// </summary>
        firewave,
        /// <summary>
        /// 自爆前置动作
        /// </summary>
        loadShockWave,
        /// <summary>
        /// 自爆释放动作
        /// </summary>
        shockWave,
        /// <summary>
        /// 鞠躬前置动作
        /// </summary>
        loadTaunt,
        /// <summary>
        /// 鞠躬收尾动作
        /// </summary>
        taunt,
        /// <summary>
        /// 追赶玩家动作
        /// </summary>
        dash,
        /// <summary>
        /// 后撤步动作
        /// </summary>
        backDash,
        /// <summary>
        /// 次元斩前置动作
        /// </summary>
        idleMassivecuts,
        /// <summary>
        /// 次元斩循环动作
        /// </summary>
        massivecuts,
        /// <summary>
        /// 次元斩结束动作
        /// </summary>
        massivecutsIdle,
        /// <summary>
        /// 拍地排开周围前置动作
        /// </summary>
        loadAllAround,
        /// <summary>
        /// 拍地排开周围动作
        /// </summary>
        allAround,
        /// <summary>
        /// 站立转向招架动作
        /// </summary>
        idleIdleDefense,
        /// <summary>
        /// 待机，招架动作
        /// </summary>
        idleDEFENSE,
        /// <summary>
        /// 招架成功抖一下动作
        /// </summary>
        idleDEFENSEParry,
        /// <summary>
        /// 招架后反击前置动作，和招架成功抖一下很像
        /// </summary>
        loadGroundStomp,
        /// <summary>
        /// 招架后反击动作
        /// </summary>
        groundStomp,
        /// <summary>
        /// 扭身伸手动作
        /// </summary>
        bulletStop,
        /// <summary>
        /// 射出抓住的子弹动作
        /// </summary>
        throwBullets,
        /// <summary>
        /// 劈砍手雷动作
        /// </summary>
        cutGrenade,
        /// <summary>
        /// 杀死宠物动作
        /// </summary>
        killPet,
        /// <summary>
        /// 释放火龙卷前置动作
        /// </summary>
        loadPoisonCloud,
        /// <summary>
        /// 释放火龙卷动作
        /// </summary>
        poisonCloud,
        /// <summary>
        /// 摧毁炮塔前置动作
        /// </summary>
        loadDemine,
        /// <summary>
        /// 摧毁炮塔动作
        /// </summary>
        demine,
        /// <summary>
        /// 立正动作
        /// </summary>
        idleDEFENSEIdle,

        /// <summary>
        /// 背身等待玩家
        /// </summary>
        introLoop0,
        /// <summary>
        /// 背身转向举剑
        /// </summary>
        introLoop0toLoop1,
        /// <summary>
        /// 举剑面对玩家
        /// </summary>
        introLoop1,
        /// <summary>
        /// 举剑转向伸剑
        /// </summary>
        introLoop1toLoop2,
        /// <summary>
        /// 伸剑指着玩家
        /// </summary>
        introLoop2,
        /// <summary>
        /// 伸剑转向准备攻击
        /// </summary>
        introLoop2toIdle,
        /// <summary>
        /// 站立到死亡动画
        /// </summary>
        death,
        /// <summary>
        /// 死亡跪地循环动画
        /// </summary>
        deathLoop
    }

    /// <summary>
    /// 女王所有fx特效的名称
    /// </summary>
    public enum Queenfx
    {
        None,
        fxAllAround,
        fxCollectorDash,
        fxCutGrenade,
        fxDemine,
        fxEstocArc,
        fxEstocFeet,
        fxEstocHead,
        fxGroundStomp,
        fxMassiveCut,
        fxOvershield,
        fxShockwave,
        fxThrust
    }

    /// <summary>
    /// 女王的所有技能、状态的名称
    /// </summary>
    public enum QueenStates
    {
        /// <summary>
        /// 戳胸
        /// </summary>
        ccQuickHigh,
        /// <summary>
        /// 刺脚
        /// </summary>
        ccQuickLow,
        /// <summary>
        /// 前半劈砍
        /// </summary>
        ccQuickFull,
        /// <summary>
        /// 下戳刺
        /// </summary>
        ccStrongOvershield,
        /// <summary>
        /// 大剑第一段
        /// </summary>
        ccStrongComboA,
        /// <summary>
        /// 大剑第二段，即为前半劈砍
        /// </summary>
        ccStrongComboB,
        /// <summary>
        /// 抓取玩家
        /// </summary>
        grabHero,
        /// <summary>
        /// 扔出去玩家
        /// </summary>
        throwHero,
        /// <summary>
        /// 收藏家突刺
        /// </summary>
        lungeAttack,
        /// <summary>
        /// 看守者：我们是地火！
        /// </summary>
        firewave,
        /// <summary>
        /// 自身往外爆炸
        /// </summary>
        shockWave,
        /// <summary>
        /// 鞠躬
        /// </summary>
        taunt,
        /// <summary>
        /// 正常跑路，去屏幕中央次元斩，开局奔向玩家
        /// </summary>
        gapCloser,
        /// <summary>
        /// 原地按兵不动（swap）
        /// </summary>
        waiting,
        /// <summary>
        /// I AM THE STORM THAT IS APROOOOOOOOOOOOOOOOOOOOOOOOOOCHING!!!!!
        /// </summary>
        splitScreenTest,
        /// <summary>
        /// 拍地排开周围
        /// </summary>
        removeRoot,
        /// <summary>
        /// 后撤步，衔接使用
        /// </summary>
        backDash,
        /// <summary>
        /// 正瞬步，衔接使用，作为后撤步的另一选择
        /// </summary>
        gapDash,
        /// <summary>
        /// 招架近战攻击
        /// </summary>
        parry,
        /// <summary>
        /// 招架成功播放的音效，真的离谱
        /// </summary>
        parryCounterAtk,
        /// <summary>
        /// 吸取子弹，让其反击
        /// </summary>
        repelBullets,
        /// <summary>
        /// 生成火龙卷
        /// </summary>
        dispelAOE,
        /// <summary>
        /// 摧毁炮塔
        /// </summary>
        destroyTurret,
        /// <summary>
        /// 摧毁手雷（？特判地狱）
        /// </summary>
        disableGrenade,
        /// <summary>
        /// 摧毁宠物（Main.projPet[type]）
        /// </summary>
        killPet,
        /// <summary>
        /// 招架成功进行的反击
        /// </summary>
        stompAnswer,
        /// <summary>
        /// 后撤步②，连贯技能结束进入等待使用
        /// </summary>
        dashFar,

        /// <summary>
        /// 女王立正了（自建）
        /// </summary>
        standUp,

        /// <summary>
        /// 去世界中间（自建）
        /// </summary>
        middleDash,

        /// <summary>
        /// 清理玩家的勾爪（自建）
        /// </summary>
        killGrappleHooks,

        /// <summary>
        /// 清理玩家的绳索
        /// </summary>
        killRopes,

        /// <summary>
        /// 玩家进场后的动画
        /// </summary>
        IntroPlot,

        /// <summary>
        /// 死亡动画
        /// </summary>
        death
    }




    public override void SetDefaults()
    {
        NPC.width = 80;
        NPC.height = 128;
        NPC.lifeMax = 160000;
        NPC.damage = 0;
        NPC.scale = 2.4f;
        NPC.defense = 3;
        NPC.knockBackResist = 0f;
        NPC.value = 0;
        NPC.npcSlots = 10f;
        NPC.lavaImmune = false;
        NPC.boss = true;
        NPC.BossBar = ModContent.GetInstance<QueenBossbar>();
        NPC.aiStyle = 0;
    }
    public override void OnSpawn(IEntitySource source)
    {
        NPC.ai[0] = -1;
        NPC.ai[1] = -1;
        parryProjIndex = -1;
        leftNoSpike = true;
        rightNoSpike = true;
        // 开始检测世界上是否有玩家的勾爪，宠物召唤物
        DCWorldSystem.startCheckGrappleHookProj = true;
        DCWorldSystem.startCheckMinionProj = true;

        hasDoneIntroPlot = false;
        CurrentState = QueenStates.IntroPlot;
        IsDealingPlot = true;
        //ActivateDrawTrail();

        foreach(Projectile projectile in Main.projectile)
        {
            if(projectile.active && projectile.type == ModContent.ProjectileType<DCHeadFire>())
            {
                projectile.Kill();
            }
        }
        headProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.position, Vector2.Zero, ModContent.ProjectileType<DCHeadFire>(), 0, 0, -1, ai0:1, ai2:1);
        handProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.position, Vector2.Zero, ModContent.ProjectileType<DCHeadFire>(), 0, 0, -1, ai0:3, ai2:1);
        
        fireCloudCD = 360;
        downAtkCD = 400;
        fireCD = 360;
        bulletStopCD = 300;
        parryCD = 300;
        killPetMinionCD = 30;
        waitingTime = 120;
        gapMiddlePosX = Main.maxTilesX * 8; //世界中点水平位置
        
        base.OnSpawn(source);
    }
    public override void AI()
    {
        if (NPC.life > 112000)
        {
            HP_level = 0;
        }
        else if (NPC.life > 80000 && HP_level == 0)
        {
            HP_level = 1;
            HPLevelChanged = true;
        }
        if(NPC.life > 45000 && NPC.life < 80000 && HP_level == 1)
        {
            HP_level = 2;
            HPLevelChanged = true;
        }
        if (NPC.life > 0 && NPC.life < 45000 && HP_level == 2)
        {
            HP_level = 3;
            HPLevelChanged = true;
        }

        if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
        {
            NPC.TargetClosest(true); //面朝玩家
        }

        if (player == null || !player.active || player.dead || !NPC.HasPlayerTarget)
        {
             player = Main.player[NPC.target];
            limitplayer = player.GetModPlayer<DCPlayer>();

        }
        //限制移动
        if (player.wingTime > 40)
            player.wingTime = 40;

        if (player.carpetTime > 30)
            player.carpetTime = 30;
        if (player.rocketTime > 50)
            player.rocketTime = 50;
        //玩家装备无限飞行
        //player.HasAccessory(ItemID.EmpressFlightBooster)

        //Main.NewText(player.HasAccessory(ItemID.EmpressFlightBooster));
        // 更新玩家与女王的水平距离，在女王左侧为负数
        distanceX = player.Center.X - NPC.Center.X;


        player.wingTimeMax = 60;
        // Do not despawn.
        NPC.timeLeft = 7200;
        /*
        ChooseCorrectFrame2(QueenAnims.loadDemine,
                                                QueenAnims.demine,
                                                Queenfx.fxDemine, 0);
        */

        //Main.NewText(distanceX);
        //Main.NewText(QueenSkillSlot_State1.Count);

        ////////////////////////////////////////
        //AI基础
        switch (CurrentState)
        {
            case QueenStates.waiting:
                QNAI_wait();
                break;
            case QueenStates.gapCloser: 
                QNAI_gapCloser(); 
                break;
            case QueenStates.backDash:
            case QueenStates.dashFar:
                QNAI_Dash(backdash: true);
                break;
            case QueenStates.gapDash:
                QNAI_Dash(backdash: false);
                break;
            case QueenStates.middleDash:
                QNAI_Dash(backdash: false, middledash: true);
                break;
            case QueenStates.ccQuickHigh:
                QNAI_ccQuickHigh();
                break;
            case QueenStates.ccQuickLow:
                QNAI_ccQuickLow();
                break;
            case QueenStates.ccQuickFull:
                QNAI_ccQuickFull();
                break;
            case QueenStates.ccStrongComboA:
                QNAI_ccStrongComboA();
                break;
            case QueenStates.ccStrongComboB:
                QNAI_ccStrongComboB();
                break;
            case QueenStates.firewave:
                QNAI_firewave();
                break;
            case QueenStates.shockWave:
                QNAI_shockWave();
                break;
            case QueenStates.dispelAOE:
                QNAI_dispelAOE();
                break;
            case QueenStates.ccStrongOvershield:
                QNAI_ccStrongOvershield();
                break;
            case QueenStates.removeRoot: 
                QNAI_removeRoot();
                break;
            case QueenStates.lungeAttack:
                QNAI_lungeAttack();
                break;
            case QueenStates.grabHero:
                QNAI_grabHero();
                break;
            case QueenStates.destroyTurret:
                QNAI_destroyTurret();
                break;
            case QueenStates.killPet:
                QNAI_killPet();
                break;
            case QueenStates.repelBullets:
                QNAI_repelBullets();
                break;
            case QueenStates.disableGrenade:
                QNAI_disableGrenade();
                break;
            case QueenStates.standUp:
                QNAI_StandUp();
                break;
            case QueenStates.splitScreenTest:
                QNAI_splitScreen();
                break;
            case QueenStates.parry:
                QNAI_parry(); 
                break;
            case QueenStates.stompAnswer:
                QNAI_stompAnswer();
                break;
            case QueenStates.killGrappleHooks:
                QNAI_killGrappleHooks();
                break;
            case QueenStates.killRopes:
                QNAI_killRopes();
                break;
            case QueenStates.taunt:
                QNAI_taunt();
                break;
            case QueenStates.IntroPlot:
                QNAI_IntroPlot();
                break;
            case QueenStates.death:
                QNAI_IntroDeath();
                break;
            default:
                Main.NewText($"未完成{CurrentState}");
                break;
        }


        headProj.velocity.X = NPC.direction;
        handProj.velocity.X = NPC.direction;


        fxindex = (int)NPC.ai[3] + lastLoadDicCount;

        if(IsDealingPlot)
        {
            NPC.dontTakeDamage = true;

            //NPC.velocity *= 0;
        }



        //Main.NewText(CurrentState);
        ///////////////////////////////////////////////////////////
        //技能用完且等待状态结束 且 没有准备次元斩，添加新的连招
        if (QueenSkillSlot_State1.Count == 0 && waitingTime == 0 && !IsDoingSplitAI && !IsDealingPlot)
        {
            //基本劈砍连招S
            if (true)// 条件先不写
            {

                /*
                //三种劈砍的次数和
                int k = 3 + HP_level * Main.rand.Next(5) / 2;
                int[] skillindex;
                if (k > 3)
                    skillindex = NormalUtils.GenerateQNArray(k);
                else
                {
                    skillindex = [0, 1, 2];
                    NormalUtils.Shuffle(skillindex);
                }
                for (int i = 0; i < skillindex.Length; i++)
                {
                    //NormalUtils.EazyNewText(i, "i : ");
                    QueenSkillSlot_State1.Add((QueenStates)skillindex[i]);
                }
                */


                switch (Main.rand.Next(7))
                {
                    case 0:
                        QueenSkillSlot_State1.Add(QueenStates.ccStrongComboA); //三段后接上第四段攻击
                        QueenSkillSlot_State1.Add(QueenStates.ccStrongComboB); //三段后接上第四段攻击
                        break;
                    case 1:
                        QueenSkillSlot_State1.Add(QueenStates.shockWave);
                        break;
                    case 2:
                        QueenSkillSlot_State1.Add(QueenStates.firewave);
                        break; 
                    case 3:
                        QueenSkillSlot_State1.Add(QueenStates.ccStrongOvershield);
                        break;
                    case 4:
                        QueenSkillSlot_State1.Add(QueenStates.dispelAOE);
                        break; 
                    case 5:
                        QueenSkillSlot_State1.Add(QueenStates.removeRoot);
                        break;
                    case 6:
                        QueenSkillSlot_State1.Add(QueenStates.lungeAttack);
                        break; 
                    default:
                        QueenSkillSlot_State1.Add(QueenStates.firewave); 
                        break;
                }

                //进入第一段攻击
                ChangeState(QueenStates.waiting);
            }
            /*
            if(Combo2_CD == 0 && Main.rand.NextBool(3))
            {
                QueenSkillSlot_State1.Add()
            }
            */

        }



        if (fireCloudCD > 0) fireCloudCD--;
        if (downAtkCD > 0) downAtkCD --;
        if (fireCD > 0) fireCD--;
        if (parryCD > 0) parryCD--;
        if (bulletStopCD > 0) bulletStopCD--;
        if (killPetMinionCD > 0) killPetMinionCD--;

        if (ImmuneTime > 0)
        {
            ImmuneTime--;
            NPC.dontTakeDamage = true;
        }
        else
            NPC.dontTakeDamage = false;

        // Main.NewText(Main.maxTilesX * 16 / 2);
        //Main.NewText(sentryProj.active);
        //Main.NewText(player.grappling[1]);
        //NormalUtils.EazyNewText(IsDoingSplitAI, "IsDoingSplitAI");
        //NormalUtils.EazyNewText(NPC.ai[0], "ai0");
        //NormalUtils.EazyNewText(NPC.ai[1], "ai1");
    }



    public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
    {
        if (parryTime > 0)
        {
            modifiers.HideCombatText();
            modifiers.FinalDamage *= 0f;
            SoundEngine.PlaySound(SoundID.GuitarAm);//////////////////////////////////////////////////////////////
            ImmuneTime = 40;
            parryTime = 0;
            int showBoomDmg = CombatText.NewText(NPC.targetRect, new Color(12, 218, 255), "招架", true, false);
            player.velocity = (player.Center - NPC.Bottom).Normalized() * 6f;
            Main.combatText[showBoomDmg].lifeTime = 70;
            foreach (Projectile projectile in Main.projectile)
                if(projectile.type == ModContent.ProjectileType<QueenParryArea>())
                    projectile.Kill();
            QueenSkillSlot_State1.Clear();
            QueenSkillSlot_State1.Add(QueenStates.stompAnswer);
            ChangeState(QueenStates.stompAnswer);
        }
        base.ModifyHitByItem(player, item, ref modifiers);
    }
    public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
    {
        // 还有一段微小位移，不想加了
        if ((CurrentState == QueenStates.waiting || CurrentState == QueenStates.standUp) && parryCD == 0 && QueenSkillSlot_State1.Count == 0 && !IsDoingSplitAI)
        {
            EndDrawTrail();
            QueenSkillSlot_State1.Add(QueenStates.parry);
            ChangeState(QueenStates.parry);
            parryProjIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, 40),
                Vector2.Zero, ModContent.ProjectileType<QueenParryArea>(), 0, 0, -1, NPC.direction);
            parryTime = 90;
            parryCD = 900 - HP_level * 30;
        }
        //NormalUtils.EazyNewText(item.Name, "name");
        base.OnHitByItem(player, item, hit, damageDone);
    }






    public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
    {
        //NormalUtils.EazyNewText(projectile.type, "type");
        //NormalUtils.EazyNewText(NPC.ai[0], "ai0");

        // 格挡这次近战伤害
        if (parryTime > 0 && (
                projectile.aiStyle == ProjAIStyleID.Yoyo || // 悠悠球
                projectile.aiStyle == ProjAIStyleID.HeldProjectile || // 手持弹幕死
                projectile.aiStyle == ProjAIStyleID.ShortSword || // 缩头乌龟，死
                projectile.aiStyle == ProjAIStyleID.Drill || // 钻头，死
                projectile.aiStyle == ProjAIStyleID.Spear || // 突刺长枪，死
                projectile.aiStyle == ProjAIStyleID.Whip || // 鞭子，死
                projectile.aiStyle == ProjAIStyleID.Flail || // 流星锤，死
                projectile.aiStyle == ProjAIStyleID.Harpoon || // 链刀，死
                projectile.aiStyle == ProjAIStyleID.Vilethorn // 魔法荆棘，死
            ))
        {
            modifiers.HideCombatText();
            modifiers.FinalDamage *= 0f;
            SoundEngine.PlaySound(SoundID.GuitarAm);//////////////////////////////////////////////////////////////
            ImmuneTime = 40;
            parryTime = 0;
            int showBoomDmg = CombatText.NewText(NPC.targetRect, new Color(12, 218, 255), "招架", true, false);
            Main.combatText[showBoomDmg].lifeTime = 70;
            foreach (Projectile proj in Main.projectile)
                if (proj.type == ModContent.ProjectileType<QueenParryArea>())
                    proj.Kill();

            QueenSkillSlot_State1.Clear();
            QueenSkillSlot_State1.Add(QueenStates.stompAnswer);
            ChangeState(QueenStates.stompAnswer);
        }
        base.ModifyHitByProjectile(projectile, ref modifiers);
    }
    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
    {

        int k = projectile.aiStyle;
        bool myMeleeProj = k == ProjAIStyleID.HeldProjectile || // 手持弹幕死
                k == ProjAIStyleID.ShortSword || // 缩头乌龟，死
                k == ProjAIStyleID.Drill || // 钻头，死
                k == ProjAIStyleID.Spear || // 突刺长枪，死
                k == ProjAIStyleID.Whip || // 鞭子，死
                k == ProjAIStyleID.Flail || // 流星锤，死
                k == ProjAIStyleID.Harpoon || // 链刀，死
                k == ProjAIStyleID.Vilethorn;// 魔法荆棘，死
        //等待状态时进入
        if ((CurrentState == QueenStates.waiting || CurrentState == QueenStates.standUp) && QueenSkillSlot_State1.Count == 0 && !IsDoingSplitAI)
        {
            // 近战的招架
            if (myMeleeProj)
            {
                if (parryCD == 0)
                {
                    QueenSkillSlot_State1.Add(QueenStates.parry);
                    ChangeState(QueenStates.parry);
                    EndDrawTrail();
                    parryProjIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, 40),
                        Vector2.Zero, ModContent.ProjectileType<QueenParryArea>(), 0, 0, -1, NPC.direction);
                    parryTime = 90;
                    parryCD = 900 - HP_level * 30;

                }
            }
            else if (bulletStopCD == 0 && k != ProjAIStyleID.ThickLaser)
            {
                EndDrawTrail();
                QueenSkillSlot_State1.Add(QueenStates.repelBullets);
                ChangeState(QueenStates.repelBullets);
                bulletStopCD = 1000 - HP_level * 30;
            }
        }
        base.OnHitByProjectile(projectile, hit, damageDone);
    }






    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        DrawQNTexture(spriteBatch);
        DrawQNGlowTexture(spriteBatch, new(255, 147, 17));
        DrawQNfxTexture(fxdic, fxdelay, fxoffsetx, fxoffsety);

        handProj.ai[2] = dic[(int)NPC.ai[3]].extraJointPos[2];
        /*
        Vector2 pos = handProj.position - Main.screenPosition;
        Rectangle rectangle = new Rectangle((int)pos.X, (int)pos.Y, handProj.width, handProj.height);
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, rectangle, new(0, 220, 0, 70));//画碰撞箱
        */
        //handProj.ai[2] = 0;
    }
    public override bool PreKill()
    {
        DCWorldSystem.startCheckGrappleHookProj = false;
        DCWorldSystem.startCheckMinionProj = false;

        headProj.Kill();
        handProj.Kill();
        return base.PreKill();
    }
    public override bool CheckDead()
    {
        if (NowDie)
        {
            NPC.life = 0;
            return true;
        }
        NPC.dontTakeDamage = true; // 免伤
        NPC.ai[2] = 6;
        ChangeState(QueenStates.death);
        NPC.life = 1; // 锁血
        NPC.velocity.X *= 0; // 水平速度清零

        return false;
    }

    /// <summary>
    /// 用来处理循环播放的动作的图像信息。
    /// </summary>
    /// <param name="AnimName"></param>
    public void ChooseCorrectFrame1Loop(QueenAnims AnimName)
    {
        ChooseCurrentDic(AssetsLoader.QNanimAtlas[AnimName.ToString()]);
        int cframe = (int)NPC.ai[3];
        if (cframe >= dic.Count)
        {
            cframe = 0;
            ReplayFrame();
        }

        CurrentTex = AssetsLoader.ChooseCorrectAnimPic(dic[cframe].index, QN: true);
        CurrentGlowTex = AssetsLoader.ChooseCorrectAnimPic(dic[cframe].index, QNglow: true);


        NPC.frame = new(dic[cframe].x, dic[cframe].y, dic[cframe].width, dic[cframe].height);

        drawVec = new Vector2(dic[cframe].originalWidth / 2 * NPC.direction //参考解包图片如果在大图里是如何绘制的
                                                - dic[cframe].offsetX * NPC.direction
                                                - (NPC.direction - 1) * dic[cframe].width / 2,//+为0，-为width

                                                 dic[cframe].originalHeight / 2+
                                                 - dic[cframe].offsetY);


        headProj.position = NPC.Bottom - new Vector2(dic[cframe].originalWidth / 2 * NPC.direction //参考解包图片如果在大图里是如何绘制的
                                        - dic[cframe].headPos[0] * NPC.direction,//+为0，-为width

                                            dic[cframe].originalHeight / 2
                                            - dic[cframe].headPos[1] + 6) * NPC.scale;

        handProj.position = NPC.Bottom - new Vector2(dic[cframe].originalWidth / 2 * NPC.direction //参考解包图片如果在大图里是如何绘制的
                                - dic[cframe].extraJointPos[0] * NPC.direction,//+为0，-为width

                                    dic[cframe].originalHeight / 2
                                    - dic[cframe].extraJointPos[1] + 6) * NPC.scale;


        if (CurrentState == QueenStates.gapCloser)
            headProj.ai[1] = 20;
        else headProj.ai[1] = 0;

        if(AnimName == QueenAnims.introLoop0toLoop1 && NPC.ai[3] == 16)
        {


        }
        switch (AnimName)
        {
            case QueenAnims.introLoop0toLoop1:
                if ((int)(Main.GameUpdateCount * 1.2f) % 3 == 0)
                    NPC.ai[3]++;
                if (NPC.ai[3] == 17)
                    headProj.ai[0] = 2;// 绘制正脸
                break;

            case QueenAnims.introLoop1:
                NPC.ai[3]++;
                break;
            case QueenAnims.introLoop1toLoop2:
            case QueenAnims.introLoop2:
            case QueenAnims.introLoop2toIdle:
                if (Main.GameUpdateCount % 2 == 0)
                    NPC.ai[3]++;
                break;



            default:
                if (Main.GameUpdateCount % 3 == 0)

                    NPC.ai[3]++;
                break;
        }

        if (NPC.ai[3] == dic.Count)
            ReplayFrame();
    }

    /// <summary>
    /// 用来处理播放那些动作做完就卡在最后一帧的动画。动作的改变用额外的字段去控制（其实那些二段动作的第一段也应该是卡住的，懒）
    /// </summary>
    /// <param name="AnimName"></param>
    /// <param name="fxName"></param>
    public void ChooseCorrectFrame1(QueenAnims AnimName)
    {
        /*
        if (fxName != Queenfx.None)
            ChooseCurrentDic(AssetsLoader.QNanimAtlas[AnimName.ToString()], AssetsLoader.QNfxAtlas[fxName.ToString()]);
        else
            */
            ChooseCurrentDic(AssetsLoader.QNanimAtlas[AnimName.ToString()]);

        int cframe = (int)NPC.ai[3];
        if (cframe >= dic.Count)
        {
            cframe = 0;
            ReplayFrame();
            if (Math.Abs(distanceX) > 300 && QueenSkillSlot_State1.Count > 2 && !noDashSkill)
            {
                CheckAddDash();
            }
            else
            {
                ChangeToNextSkill();
            }
        }

        if (lastPicIndex != dic[cframe].index)
        {
            lastPicIndex = dic[cframe].index;
            CurrentTex = AssetsLoader.ChooseCorrectAnimPic(dic[cframe].index, QN: true);
            CurrentGlowTex = AssetsLoader.ChooseCorrectAnimPic(dic[cframe].index, QNglow: true);
        }

        NPC.frame = new(dic[cframe].x, dic[cframe].y, dic[cframe].width, dic[cframe].height);

        drawVec = new Vector2(dic[cframe].originalWidth / 2 * NPC.direction //参考解包图片如果在大图里是如何绘制的
                                                - dic[cframe].offsetX * NPC.direction
                                                - (NPC.direction - 1) * dic[cframe].width / 2,//+为0，-为width

                                                 dic[cframe].originalHeight / 2
                                                 - dic[cframe].offsetY);

        headProj.position = NPC.Bottom - new Vector2(dic[cframe].originalWidth / 2 * NPC.direction //参考解包图片如果在大图里是如何绘制的
                                        - dic[cframe].headPos[0] * NPC.direction,//+为0，-为width

                                            dic[cframe].originalHeight / 2
                                            - dic[cframe].headPos[1] + 6) * NPC.scale;

        handProj.position = NPC.Bottom - new Vector2(dic[cframe].originalWidth / 2 * NPC.direction //参考解包图片如果在大图里是如何绘制的
                        - dic[cframe].extraJointPos[0] * NPC.direction,//+为0，-为width

                            dic[cframe].originalHeight / 2
                            - dic[cframe].extraJointPos[1] + 6) * NPC.scale;

        headProj.ai[1] = 0;

        if (Main.GameUpdateCount % 3 == 0 && NPC.ai[3] < dic.Count - 1)
        {
            NPC.ai[3]++;
        }
        /*
        if (NPC.ai[3] == dic.Count)
        {
            ReplayFrame();
            if (Math.Abs(distanceX) > 300 && QueenSkillSlot_State1.Count > 2 && !noDashSkill)
            {
                CheckAddDash();
            }
            else
            {
                ChangeToNextSkill();
            }
        }
        */
    }

    /// <summary>
    /// 用来处理由load动作到释放动作（2个），其中使用了fx贯穿其中的那些动作
    /// </summary>
    /// <param name="loadAnimName">前置加载动画的名称</param>
    /// <param name="AnimName">后续动画的名称</param>
    /// <param name="fxName">特效的名称，没有填Queenfx.None</param>
    /// <param name="projType1">动作一开始就生成的弹幕，没有不填</param>
    /// <param name="projType2">动作一切换就生成的弹幕，几个刺戳都用，没有不填</param>
    /// <param name="projOffX">弹幕生成时的水平偏移量</param>
    /// <param name="projOffY">弹幕生成时的垂直偏移量</param>
    /// <param name="damage">弹幕伤害</param>
    public void ChooseCorrectFrame2(QueenAnims loadAnimName, QueenAnims AnimName, Queenfx fxName, int projType1 = 0, int projType2 = 0, int projOffX = 0, int projOffY = 0, int damage = 0)
    {

        //NormalUtils.EazyNewText(isDrawingLoadDic, "isDrawingLoadDic");
        //NormalUtils.EazyNewText(isDrawingSecondDic, "isDrawingSecondDic");

        if (!isDrawingLoadDic && !isDrawingSecondDic) // 一开始，两个都没画，一帧
        {
            if (fxName != Queenfx.None)
                ChooseCurrentDic(AssetsLoader.QNanimAtlas[loadAnimName.ToString()], AssetsLoader.QNfxAtlas[fxName.ToString()]);
            else
                ChooseCurrentDic(AssetsLoader.QNanimAtlas[loadAnimName.ToString()]); 
            ReplayFrame();
            isDrawingLoadDic = true;
            isDrawingSecondDic = false;// 保险
            lastLoadDicCount = 0;
            // Projectile.NewProjectile(); // 生成当前攻击的弹幕
            //Main.NewText(projType);
            if (projType1 != 0)
            {
                Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, projType1, damage, 0);
                int offX = NPC.direction > 0 ? 0 : NPC.width - proj.width;
                proj.position = NPC.TopLeft + new Vector2(projOffX * NPC.direction + offX, projOffY);
            }
        }

        int cframe = (int)NPC.ai[3];// 当前是动作第几帧

        // 多写一遍下面两个检测，避免报错
        if (cframe >= dic.Count && isDrawingLoadDic && !isDrawingSecondDic)// 前置动作结束
        {
            lastLoadDicCount = cframe;
            cframe = 0;
            isDrawingLoadDic = false;
            isDrawingSecondDic = true;
            // 不再绘制残影
            EndDrawTrail();
            ReplayFrame();
            if (fxName != Queenfx.None)
                ChooseCurrentDic(AssetsLoader.QNanimAtlas[AnimName.ToString()], AssetsLoader.QNfxAtlas[fxName.ToString()]);
            else
                ChooseCurrentDic(AssetsLoader.QNanimAtlas[AnimName.ToString()]);
            if(projType2 != 0)
            {
                Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, projType2, damage, 0);

                int offX = NPC.direction > 0 ? 0 : NPC.width - proj.width;
                proj.position = NPC.TopLeft + new Vector2(projOffX * NPC.direction + offX, projOffY);
            }
            // Projectile.NewProjectile();
        }

        if (cframe >= dic.Count && !isDrawingLoadDic && isDrawingSecondDic)// 后续动作结束
        {
            isDrawingLoadDic = false;
            isDrawingSecondDic = false;
            if(drawGrabFrames) drawGrabFrames = false;
            cframe = 0;
            lastLoadDicCount = 0;
            ReplayFrame();
            //距离远且不是最后一个技能，进入瞬步追赶
            if (Math.Abs(distanceX) > 300 && QueenSkillSlot_State1.Count > 2 && !noDashSkill)
            {
                CheckAddDash();
            }
            else
            {
                ChangeToNextSkill();
            }
            //要更改
            //ChooseCurrentDic(AssetsLoader.QNanimAtlas[QueenAnims.idleDEFENSE.ToString()]);
        }

        // 避免更新贴图导致的错乱
        if (lastPicIndex != dic[cframe].index)
        {
            lastPicIndex = dic[cframe].index;
            CurrentTex = AssetsLoader.ChooseCorrectAnimPic(dic[cframe].index, QN: true);
            CurrentGlowTex = AssetsLoader.ChooseCorrectAnimPic(dic[cframe].index, QNglow: true);
        }

        NPC.frame = new(dic[cframe].x, dic[cframe].y, dic[cframe].width, dic[cframe].height);

        drawVec = new Vector2(dic[cframe].originalWidth / 2 * NPC.direction //参考解包图片如果在大图里是如何绘制的
                                                - dic[cframe].offsetX * NPC.direction
                                                - (NPC.direction - 1) * dic[cframe].width / 2,//+为0，-为width

                                                 dic[cframe].originalHeight / 2
                                                 - dic[cframe].offsetY);

        headProj.position = NPC.Bottom - new Vector2(dic[cframe].originalWidth / 2 * NPC.direction //参考解包图片如果在大图里是如何绘制的
                                        - dic[cframe].headPos[0] * NPC.direction,//+为0，-为width

                                            dic[cframe].originalHeight / 2
                                            - dic[cframe].headPos[1] + 6) * NPC.scale;

        handProj.position = NPC.Bottom - new Vector2(dic[cframe].originalWidth / 2 * NPC.direction //参考解包图片如果在大图里是如何绘制的
                        - dic[cframe].extraJointPos[0] * NPC.direction,//+为0，-为width

                            dic[cframe].originalHeight / 2
                            - dic[cframe].extraJointPos[1] + 6) * NPC.scale;


        if (isDrawingLoadDic) // 第二段特判要改变播放速度的动作
        {
            switch (loadAnimName)
            {
                case QueenAnims.idleLoadLunge:

                    if (NPC.ai[3] == dic.Count - 2)
                        CollectorDashDelay = 13;
                    if (NPC.ai[3] == dic.Count - 1 && CollectorDashDelay > 0)
                        CollectorDashDelay--;
                    else if (Main.GameUpdateCount % 4 == 0)
                        NPC.ai[3]++;

                    break;
                case QueenAnims.loadFirewave:
                case QueenAnims.loadGrab:
                    if (Main.GameUpdateCount % 4 == 0)
                        NPC.ai[3]++;
                    break;
                case QueenAnims.loadTaunt:
                case QueenAnims.loadOvershield:
                case QueenAnims.loadPoisonCloud:
                    if (Main.GameUpdateCount % 2 == 0)
                        NPC.ai[3]++;
                    break;
                case QueenAnims.bulletStop:
                    if (CurrentState == QueenStates.killRopes)
                    {
                        if (Main.GameUpdateCount % 2 == 0)
                            NPC.ai[3]++;
                    }
                    else
                        if (Main.GameUpdateCount % 3 == 0)
                            NPC.ai[3]++;
                    break;
                default:
                    if (Main.GameUpdateCount % 3 == 0)
                        NPC.ai[3]++;
                    break;
            }
        }
        else if (isDrawingSecondDic) // 第二段特判要改变播放速度的动作
        {
            switch (AnimName)
            {
                case QueenAnims.overshield:
                case QueenAnims.thrust:
                case QueenAnims.shockWave:
                case QueenAnims.poisonCloud:
                    if ((int)(Main.GameUpdateCount * 1.2f) % 3 == 0)
                        NPC.ai[3]++;
                    break;
                case QueenAnims.taunt:
                    if ((int)(Main.GameUpdateCount * 1.2f) % 2 == 0)
                        NPC.ai[3]++;
                    break;
                case QueenAnims.throwBullets:
                    if (Main.GameUpdateCount % 4 == 0)
                        NPC.ai[3]++;
                    break;
                default:
                    if (Main.GameUpdateCount % 3 == 0)
                        NPC.ai[3]++;
                    break;
            }
        }
        else //以上均不符合，回归本源
        {
            if (Main.GameUpdateCount % 3 == 0)
                NPC.ai[3]++;
        }
    



        if (NPC.ai[3] >= dic.Count && isDrawingLoadDic && !isDrawingSecondDic)// 前置动作结束，一帧
        {
            lastLoadDicCount = (int)NPC.ai[3];
            isDrawingLoadDic = false;
            isDrawingSecondDic = true;
            // 不再绘制残影
            EndDrawTrail();
            ReplayFrame();
            if (fxName != Queenfx.None)
                ChooseCurrentDic(AssetsLoader.QNanimAtlas[AnimName.ToString()], AssetsLoader.QNfxAtlas[fxName.ToString()]);
            else
                ChooseCurrentDic(AssetsLoader.QNanimAtlas[AnimName.ToString()]);

            if (projType2 != 0)
            {
                Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, projType2, damage, 0);

                int offX = NPC.direction > 0 ? 0 : NPC.width - proj.width;
                proj.position = NPC.TopLeft + new Vector2(projOffX * NPC.direction + offX, projOffY);
            }
        }
        if (NPC.ai[3] >= dic.Count && !isDrawingLoadDic && isDrawingSecondDic)// 后续动作结束，一帧
        {
            isDrawingLoadDic = false;
            isDrawingSecondDic = false;
            if (drawGrabFrames) drawGrabFrames = false;
            lastLoadDicCount = 0;
            ReplayFrame();

            if (Math.Abs(distanceX) > 300 && QueenSkillSlot_State1.Count > 2 && !noDashSkill)
            {
                CheckAddDash();
            }
            else
            {
                ChangeToNextSkill();
            }
            // 要更改
            //ChooseCurrentDic(AssetsLoader.QNanimAtlas[QueenAnims.idleDEFENSE.ToString()]);
        }
    }

    /// <summary>
    /// 改变当前使用的动画字典
    /// 示例：        ChooseCurrentDic(AssetsLoader.QNanimAtlas[QueenAnims.loadDemine.ToString()]);
    /// </summary>
    /// <param name="dict"></param>
    public void ChooseCurrentDic(Dictionary<int, DCAnimPic> dict, Dictionary<int, DCAnimPic> fxDict = null)
    {
        if (dict != null)
            dic = dict;

        fxdic = fxDict;

    }

    public void DrawQNTexture(SpriteBatch spriteBatch)
    {
        if (ShouldDrawTrail)
        {
            for (int i = oldPosi.Length - 1; i > 0; i--)//绘制拖尾
            {
                if (oldPosi[i] != Vector2.Zero)
                {
                    spriteBatch.Draw(CurrentTex, 
                        oldPosi[i] - Main.screenPosition,
                        NPC.frame, 
                        new Color(230, 230, 230, 0) * 1 * (1 - 0.067f * i),
                        0f,
                        drawVec,
                        NPC.scale, 
                        (SpriteEffects)(-(NPC.direction - 1) / 2),
                        0);
                }
            }
        }
        spriteBatch.Draw(CurrentTex,
            NPC.Bottom - Main.screenPosition,
            NPC.frame,
            Color.White,
            0f,
            drawVec,
            NPC.scale,
            (SpriteEffects)(-(NPC.direction - 1) / 2),
            0);
    }
    public void DrawQNGlowTexture(SpriteBatch spriteBatch, Color glowColor)
    {
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

        AssetsLoader.glowEffect.Parameters["InputR"].SetValue(glowColor.R);
        AssetsLoader.glowEffect.Parameters["InputG"].SetValue(glowColor.G / 255f);
        AssetsLoader.glowEffect.Parameters["InputB"].SetValue(glowColor.B / 255f);
        AssetsLoader.glowEffect.CurrentTechnique.Passes["fx"].Apply();
        spriteBatch.Draw(CurrentGlowTex,
            NPC.Bottom - Main.screenPosition,
            NPC.frame,
            Color.White,
            0f,
            drawVec,
            NPC.scale,
            (SpriteEffects)(-(NPC.direction - 1) / 2),
            0);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
    }
    public void DrawQNfxTexture(Dictionary<int, DCAnimPic> fxDic, int beginIndex = 0, float drawStartOffsetX = 0, float drawStartOffsetY = 0)
    {
        if (fxdic == null)
        {
            //Main.NewText("特效字典为空", Color.LightBlue);
            return;
        }
        SpriteBatch sb = Main.spriteBatch;
        int index = fxindex - beginIndex;
        if(index < 0)
        {
            //Main.NewText("动作可能处于延迟中", Color.LightBlue);
            return;
        }
        if(index >= fxdic.Count)
        {
            //Main.NewText("fx索引越界！", Color.LightBlue);
            return;
        }
        Rectangle rectangle = new(fxDic[index].x, fxDic[index].y,
                                                    fxDic[index].width, fxDic[index].height);

        Vector2 vector = new Vector2(fxDic[index].originalWidth / 2 * NPC.direction //参考解包图片如果在大图里是如何绘制的
                                                        - fxDic[index].offsetX * NPC.direction
                                                        - (NPC.direction - 1) * fxDic[index].width / 2,//+为0，-为width

                                                         fxDic[index].originalHeight / 2
                                                         - fxDic[index].offsetY)

            + new Vector2(NPC.direction * drawStartOffsetX, drawStartOffsetY);



        sb.End();

        sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

        sb.Draw(AssetsLoader.QNfxQueen,
                NPC.Bottom - Main.screenPosition,
                rectangle,
                Color.White,
                NPC.rotation,
                vector,
                NPC.scale,
                (SpriteEffects)(-(NPC.direction - 1) / 2),
                0f);

        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);


    }
    public void ReplayFrame()
    {

        if (IsDealingPlot)
        {
            if (NPC.ai[2] == 5)        //第一段剧情结束
            {
                IsDealingPlot = false;


                QueenSkillSlot_State1.Add(QueenStates.ccQuickHigh);
                QueenSkillSlot_State1.Add(QueenStates.ccQuickLow);
                QueenSkillSlot_State1.Add(QueenStates.ccQuickFull);
                QueenSkillSlot_State1.Add(QueenStates.ccStrongComboA); //三段后接上第四段攻击
                QueenSkillSlot_State1.Add(QueenStates.ccStrongComboB); //三段后接上第四段攻击
                NPC.ai[3] = 0;
                ChangeState(QueenStates.gapCloser);
                return;
            }
            if (NPC.ai[2] == 6)        //下跪结束，进入跪地循环
            {
                IsDealingPlot = false; // 等死就可以啦
                NPC.ai[2]++; // 进入跪地循环
                NPC.ai[3] = 0;
                return;
            }
            NPC.ai[2]++;
        }
        NPC.ai[3] = 0;
    }
}
