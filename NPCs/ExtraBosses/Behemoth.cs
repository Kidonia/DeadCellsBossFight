using DeadCellsBossFight.Contents.Bosssbars;
using DeadCellsBossFight.Core;
using DeadCellsBossFight.Projectiles.NPCsProj;
using DeadCellsBossFight.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DeadCellsBossFight.NPCs.ExtraBosses;

// ai[0]用于控制NPC使用哪个动作贴图，
// 计划ai[1]用于控制血量不同时的阶段切换
// 
// ai[3]用于NPC贴图的帧数变化，一定小心！不要用到它。

[AutoloadBossHead]
public class Behemoth : DC_BasicNPC
{
    // public override LocalizedText DisplayName => "Concierge";

    private Dictionary<int, DCAnimPic> AtkADic = new();
    private Dictionary<int, DCAnimPic> AtkFireDic = new();
    private Dictionary<int, DCAnimPic> AtkCDic = new();
    private Dictionary<int, DCAnimPic> AtkLoadADic = new();
    private Dictionary<int, DCAnimPic> AtkLoadFireDic = new();
    private Dictionary<int, DCAnimPic> IdleDic = new();
    private Dictionary<int, DCAnimPic> ShoutDic = new();
    private Dictionary<int, DCAnimPic> WalkADic = new();
    private Dictionary<int, DCAnimPic> WalkBDic = new();
    private Dictionary<int, DCAnimPic> FallDic = new();
    private Dictionary<int, DCAnimPic> KillDic = new();
    private Dictionary<int, DCAnimPic> StunDic = new();

    private Dictionary<int, DCAnimPic> fxAtkADic = new();
    private Dictionary<int, DCAnimPic> fxAtkFireDic = new();
    private int cooldownBTW;
    private int AtkACoolDown;
    private int FireCoolDown;
    private int FireDelayTime;
    private int JumpCoolDown;
    private int JumpDelayTime;
    private int RedShieldCoolDown;

    private bool drawShield;
    private bool drawfxAtkA;
    private bool drawfxAtkFire;
    private int bonus;
    public override string Texture => "DeadCellsBossFight/Assets/NPCTextures/behemoth0";
    public override void SetDefaults()
    {
        NPC.width = 85;
        NPC.height = 109;
        //80000血
        NPC.lifeMax = 80000;
        NPC.damage = 0;
        NPC.defense = 10;
        NPC.knockBackResist = 0f;
        NPC.value = 0;
        NPC.npcSlots = 10f;
        NPC.lavaImmune = false;
        NPC.boss = true;
        NPC.scale = 2f;
        NPC.BossBar = ModContent.GetInstance<BetemothBossbar>();
        NPC.aiStyle = 0;

        AtkADic = AssetsLoader.behemothDic["behemothAtkA"]; 
        AtkFireDic = AssetsLoader.behemothDic["behemothAtkB"];
        AtkCDic = AssetsLoader.behemothDic["behemothCast"];
        AtkLoadADic = AssetsLoader.behemothDic["behemothAtkLoadA"];
        AtkLoadFireDic = AssetsLoader.behemothDic["behemothAtkLoadB"];
        IdleDic = AssetsLoader.behemothDic["behemothWalkIdleA"];
        ShoutDic = AssetsLoader.behemothDic["behemothShout"];
        WalkADic = AssetsLoader.behemothDic["behemothWalkA"];
        WalkBDic = AssetsLoader.behemothDic["behemothWalkB"];
        KillDic = AssetsLoader.behemothDic["lethalHit"]; 
        FallDic = AssetsLoader.behemothDic["behemothFall"];
        StunDic = AssetsLoader.behemothDic["stun"];

        fxAtkADic = AssetsLoader.fxEnmAtlas["fxBeheAtkA"];
        fxAtkFireDic = AssetsLoader.fxEnmAtlas["fxBeheAtkB"];
    }
public override void OnSpawn(IEntitySource source)
    {
        NPC.ai[0] = 9;
        cooldownBTW = 72;
        AtkACoolDown = 0;
        FireCoolDown = 0;
        FireDelayTime = 0;
        JumpCoolDown = 0;
        JumpDelayTime = 0;
        RedShieldCoolDown = 0;
        drawShield = false;
    }
    public override void AI()
    {
        //ai[0]用于控制当前动作播放，ai[1]用于控制行为

        if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
        {
            NPC.TargetClosest(true);
        }
        // 阶段带来的强化
        bonus = 24 * (int)NPC.ai[1];
        drawfxAtkA = false;
        drawfxAtkFire = false;
        //Main.NewText(NPC.ai[3]);
        if (NPC.ai[0] < 6 || NPC.ai[0] > 8)
            NPC.direction = NPC.oldDirection;

        switch (NPC.ai[0])
        {
            case 0: // 近战攻击
                AtkA_AI();
                ChooseCorrectFrame(AtkADic);
                if (NPC.ai[3] > 7)
                    drawfxAtkA = true;
                break;
            case 1: // 近战攻击待机动作
                AtkA_AI();
                ChooseCorrectFrame(AtkLoadADic);
                break;
            case 2: // 火焰攻击
                FireAtk_AI();
                ChooseCorrectFrame(AtkFireDic);

                if (NPC.ai[3] > 2)
                    drawfxAtkFire = true;
                break;
            case 3: // 火焰攻击待机动作
                FireAtk_AI();
                ChooseCorrectFrame(AtkLoadFireDic);
                break;
            case 4: // 红盾攻击前摇
                ChooseCorrectFrame(WalkBDic);
                if (NPC.ai[3] == 17) // 动作播放结束，进入红盾展开
                {
                    NPC.ai[0] = 5;
                    ReplayFrame();
                }
                break;
            case 5: // 红盾攻击
                ChooseRedShieldFrame(AtkCDic);
                //////////////////////////////////////////////////////////////////////////////
                break;
            case 6: // 站立
                ChooseCorrectFrame(IdleDic);
                Walk_AI();
                break;
            case 7: // 走路动画1
                ChooseWalkFrame(WalkADic);
                Walk_AI();
                break;
            case 8: // 走路动画2
                ChooseWalkFrame(WalkBDic);
                Walk_AI();
                break;
            case 9: // 降落
                ChooseCorrectFrame(FallDic);
                Fall_AI();
                break;
            case 10: // 跳跃前摇攻击，要完善
                JumpAtk_AI();
                JumpLoadDraw();
                break;
            case 11:
                ChooseCorrectFrame(ShoutDic);
                ShoutShield_AI();
                break;
            case 12: // 死亡
                ChooseCorrectFrame(KillDic);
                Die_AI();
                break;
            default: break;
        }





        //血量减到临界值时，触发阶段切换。
        if (NPC.life < NPC.lifeMax * 7 / 10 && NPC.ai[1] == 0)
        {
            NPC.ai[1] = 1; // 阶段加一
            drawfxAtkA = false;
            drawfxAtkFire = false;
            ReplayFrame(); // 动作清零
            NPC.ai[0] = 11; // 播放喊叫动画
            NPC.dontTakeDamage = true;
        }
        if (NPC.life < NPC.lifeMax * 11 / 20 && NPC.ai[1] == 1)
        {
            NPC.ai[1] = 2; // 阶段加一
            drawfxAtkA = false;
            drawfxAtkFire = false;
            ReplayFrame(); // 动作清零
            NPC.ai[0] = 11; // 播放喊叫动画
            NPC.dontTakeDamage = true;
        }
        if (NPC.life < NPC.lifeMax * 2 / 5 && NPC.ai[1] == 2)
        {
            NPC.ai[1] = 3; // 阶段加一
            drawfxAtkA = false;
            drawfxAtkFire = false;
            ReplayFrame(); // 动作清零
            NPC.ai[0] = 11; // 播放喊叫动画
            NPC.dontTakeDamage = true;
        }

        // 计时器
        if (AtkACoolDown > 0) AtkACoolDown--;
        if (FireCoolDown > 0) FireCoolDown--;
        if (JumpCoolDown > 0) JumpCoolDown--;
        if (RedShieldCoolDown > 0) RedShieldCoolDown--;
        if (cooldownBTW > 0) cooldownBTW--;
    }




    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Vector2 pos = NPC.position - Main.screenPosition;
        Rectangle rectangle = new Rectangle((int)pos.X, (int)pos.Y, NPC.width, NPC.height);
        //spriteBatch.Draw(TextureAssets.MagicPixel.Value, rectangle, new(50, 50, 50, 50));//画碰撞箱
        DrawNpcTexture(spriteBatch);
        DrawBeheTexture(AssetsLoader.behemothGlowTex, spriteBatch, new(255, 169, 0), rectangle); // 不用basic的，因为顺便画盾，爱护你的电脑！
        if (drawfxAtkA)
            DrawBehefxTexture(fxAtkADic, 8, -46, 25);
        if (drawfxAtkFire)
            DrawBehefxTexture(fxAtkFireDic, 3, -46, 25);
    }

    public override bool PreKill()
    {
        foreach (NPC npc in Main.npc)
        {
            if (npc.type == ModContent.NPCType<BH>())
                npc.ai[1] = 1;
        }
        return base.PreKill();
    }
    public override bool? CanFallThroughPlatforms()
    {
        return Main.player[NPC.target].Bottom.Y < NPC.Bottom.Y;
    }
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return NPC.ai[0] == 9 && NPC.velocity.X != 0;
    }
    public override bool CheckDead()
    {
        NPC.dontTakeDamage = true; // 免伤

        if (NPC.ai[3] == 55)
        {
            NPC.life = 0;
            return true;
        }
        NPC.life = 1; // 锁血
        NPC.velocity.X *= 0; // 水平速度清零
        NPC.ai[0] = 12; // 切换到死亡动画状态
        return false;
    }



    /// <summary>
    /// 处理走路的速度
    /// </summary>
    public void Walk_AI()
    {
        NPC.damage = 0;

        float distance = Math.Abs((player.Center - NPC.Center).X);
        // NormalUtils.EazyNewText(distance, "distance");
        // 在走路或站立
        if ((NPC.ai[0] == 6 || NPC.ai[0] == 7 || NPC.ai[0] == 8))
        {
            //站立或走路时，速度往下，更改状态：降落
            if (NPC.velocity.Y > 0)
            {
                NPC.ai[0] = 9;
                ReplayFrame();
            }
            if (cooldownBTW <= 0) // 攻击间隔结束
            {
                // 距离小于200
                if (distance < 200f && AtkACoolDown == 0)
                {
                    AtkACoolDown = 420 - bonus;
                    cooldownBTW = 150 - bonus;
                    NPC.ai[0] = 1; // 进入近战攻击状态
                    ReplayFrame();
                    return;
                }

                //距离超过250，先检测火焰攻击
                if (distance > 250f - bonus && FireCoolDown == 0)
                {
                    FireCoolDown = 420 - bonus;
                    cooldownBTW = 150 - bonus;
                    FireDelayTime = 32 - bonus / 6;
                    NPC.ai[0] = 3; // 进入火焰攻击状态
                    ReplayFrame();
                    return;
                }

                //距离超过250，后检测跳跃攻击，二阶段后解锁
                if (distance > 250f - bonus && JumpCoolDown == 0 && NPC.ai[1] > 0)
                {
                    JumpCoolDown = 420 - bonus;
                    cooldownBTW = 150 - bonus;
                    JumpDelayTime = 30;
                    NPC.ai[0] = 10; // 进入跳跃准备状态
                    ReplayFrame();
                    return;
                }

                //盾好了直接用
                if (RedShieldCoolDown == 0)
                {
                    RedShieldCoolDown = 580 - (int)(bonus * 1.3f);
                    cooldownBTW = 150 - bonus;
                    NPC.ai[0] = 4;
                    ReplayFrame();
                    return;
                }
            }
            // 检测完攻击条件后
            if (NPC.ai[0] == 7 || NPC.ai[0] == 8) //
            {
                if(distance < 50f) // 距离过近
                {
                    NPC.ai[0] = 6; // 进入站立状态
                }
                else // 正常移动
                {
                    float maxSpeed = 2.4f + 0.2f * NPC.ai[1];
                    NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * maxSpeed, 0.35f);
                }
            }
            if (NPC.ai[0] == 6 && distance > 50f)
            {
                if (distance > 50f) // 距离过远
                {
                    NPC.ai[0] = 7; // 进入走路状态
                    ReplayFrame();
                }
                else // 正常站立
                {
                    NPC.velocity.X *= 0;
                }
            }
        }
    }
    public void AtkA_AI()
    {
        NPC.velocity.X *= 0;
        if (NPC.ai[0] == 1) // 攻击待机状态
        { 
            if(NPC.ai[3] == 9) // 动作结束
            {
                NPC.ai[0] = 0; // 进入攻击状态
                //NPC.ai[3] = 0; // 为什么不清零？因为细胞里就这样才显得连贯，正好直接跳过前面那些帧
            }
        }
        if (NPC.ai[0] == 0) // 攻击状态
        {
            /////////////////////////////////////////////////////////////////////////////////////弹幕生成
            if ((NPC.ai[3]) == 9 && Main.GameUpdateCount % 3 == 0)
            {
                Vector2 pos = NPC.Center + new Vector2(NPC.direction * 80, 48);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, Vector2.Zero, ModContent.ProjectileType<BeheAtkArea>(), 32 + bonus, 2 + NPC.ai[1]);
            }


            if (NPC.ai[3] == 19) // 动作结束
            {
                if (FireCoolDown == 0) // 直接接上火焰攻击
                {
                    FireCoolDown = 310 - bonus;
                    cooldownBTW = 80 - bonus;
                    FireDelayTime = 32 - bonus / 6;
                    NPC.ai[0] = 3; // 进入火焰攻击状态
                }
                else
                    NPC.ai[0] = 7; // 回到走路状态
            }
        }
    }
    public void FireAtk_AI()
    {
        NPC.velocity.X *= 0;
        if (NPC.ai[0] == 3) // 火焰攻击待机状态
        {
            if (FireDelayTime > 0) // 用它控制动作播放时间
            {
                FireDelayTime--;
            }
            else // 时间结束
            {
                NPC.ai[0] = 2; // 进入火焰攻击状态
                ReplayFrame();
            }
        }
    
        if (NPC.ai[0] == 2) // 火焰攻击状态
        {
            if (NPC.ai[3] == 4 && Main.GameUpdateCount % 3 == 0) // 生成两侧延伸火柱，% 3 == 0保证只生成一次
            {
                Vector2 btmpos = NPC.direction > 0 ? NPC.BottomRight : NPC.BottomLeft;
                Point bottom = btmpos.ToTileCoordinates(); 
                for (int j = 0; j < 3; j++)// 检测脚下砖块，避免空砖
                {
                    Tile spawnTile = NormalUtils.CheckTileInPosition(bottom.X, bottom.Y + j);
                    if (spawnTile.HasTile)  // 有物块，不要检测树什么的，生成便是，碰撞交给弹幕AI
                    {
                        Point posPoint = new(bottom.X, bottom.Y + j);
                        Vector2 pos = posPoint.ToWorldCoordinates() + new Vector2(0, -18);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, Vector2.Zero, ModContent.ProjectileType<FirePillar>(), 40, 2, -1, 0, 1, 0);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, Vector2.Zero, ModContent.ProjectileType<FirePillar>(), 40, 2, -1, 0, 0, 1);
                        break;
                    }
                }
            }


            /////////////////////////////////////////////////////////////////////////////////////弹幕生成
            if (NPC.ai[3] == 11) // 动作结束
            {
                NPC.ai[0] = 7; // 回到走路状态
            }
        }
    }
    public void JumpAtk_AI()
    {
        if (JumpDelayTime > 0)
        {
            JumpDelayTime--;
        }
        else
        {
            NPC.damage = 120 + bonus;
            float velBonus = 0.2f * NPC.ai[1];
            NPC.velocity = new Vector2(NPC.direction * 14.8f + velBonus, -6.8f - velBonus); // 跳跃速度要调
            NPC.oldVelocity = new Vector2(NPC.direction * 14.8f + velBonus, -6.8f - velBonus); // 跳跃速度要调
            NPC.ai[0] = 9; // 回到跳跃
        }
    }
    public void JumpLoadDraw()
    {
        NPC.frame = new(StunDic[0].x, StunDic[0].y, StunDic[0].width, StunDic[0].height);

        drawVec = new Vector2(StunDic[0].originalWidth / 2 * NPC.direction //参考解包图片如果在大图里是如何绘制的
                                                - StunDic[0].offsetX * NPC.direction
                                                - (NPC.direction - 1) * StunDic[0].width / 2,//+为0，-为width

                                                 StunDic[0].originalHeight / 2
                                                 - StunDic[0].offsetY)

    + new Vector2(NPC.direction * drawStartOffsetX, drawStartOffsetY);
    }
    public void Fall_AI()
    {
        // 下落过程
        if (NPC.ai[0] == 9)
        {
            NPC.velocity.X = NPC.oldVelocity.X;
            // 下落状态落地后
            if (NPC.velocity.Y == 0 && NPC.oldVelocity.Y >= 0)
            {
                NPC.velocity.X /= 10f; // 迅速减速，刹住车
                NPC.ai[0] = 7; // 回到走路
            }
        }
    }
    public void ShoutShield_AI()
    {
        NPC.velocity.X *= 0; // 速度清零
        if (NPC.ai[0] == 11) // 再判一次
        {
            drawShield = true; // 绘制盾
            if (NPC.ai[3] == 60 - bonus / 6) // 动画结束
            { 
                ReplayFrame();
                drawShield = false;
                NPC.dontTakeDamage = false;
                RedShieldCoolDown = 480 - bonus; // 重设盾冷却
                NPC.ai[0] = 4; // 直接衔接盾
            }
        }
    }
    public void Die_AI()
    {
        if (NPC.ai[0] == 12 && NPC.ai[3] == 55) // 死亡动画播放结束
        {
            NPC.dontTakeDamage = false;
            NPC.life = 0;
            NPC.checkDead(); // 执行死亡
        }
    }
    
    public void ChooseWalkFrame(Dictionary<int, DCAnimPic> dic)
    {
        int cframe = (int)NPC.ai[3];
        if (cframe >= dic.Count)
        {
            cframe = 0;
            ReplayFrame();
        }
        NPC.frame = new(dic[cframe].x, dic[cframe].y, dic[cframe].width, dic[cframe].height);

        drawVec = new Vector2(dic[cframe].originalWidth / 2 * NPC.direction //参考解包图片如果在大图里是如何绘制的
                                                - dic[cframe].offsetX * NPC.direction
                                                - (NPC.direction - 1) * dic[cframe].width / 2,//+为0，-为width

                                                 dic[cframe].originalHeight / 2
                                                 - dic[cframe].offsetY)

    + new Vector2(NPC.direction * drawStartOffsetX, drawStartOffsetY);


        if (NPC.ai[0] == 7)
        {
            if (Main.GameUpdateCount % 4 == 0) // 因为抽帧，所以动作播放也要随之放慢一点
                NPC.ai[3] += 3; // 抽帧播放，细胞看起来就是这么搞得
            if (NPC.ai[3] >= 19) // 动作结束
            {
                NPC.ai[0] = 8; // 另一条腿，非得分成两个吗
                ReplayFrame();
            }
        }
        if (NPC.ai[0] == 8)
        {
            if (Main.GameUpdateCount % 4 == 0)
                NPC.ai[3] += 2; // 抽帧播放，细胞看起来就是这么搞得
            if (NPC.ai[3] >= 9) // 动作结束
            {
                NPC.ai[0] = 7; // 另一条腿，非得分成两个吗
                ReplayFrame();
            }
        }

        if (NPC.ai[3] >= dic.Count)
            ReplayFrame();

    }

    public void ChooseRedShieldFrame(Dictionary<int, DCAnimPic> dic)
    {
        int cframe = (int)NPC.ai[3];
        if (cframe >= dic.Count)
        {
            cframe = 0;
            ReplayFrame();
        }
        NPC.frame = new(dic[cframe].x, dic[cframe].y, dic[cframe].width, dic[cframe].height);

        drawVec = new Vector2(dic[cframe].originalWidth / 2 * NPC.direction //参考解包图片如果在大图里是如何绘制的
                                                - dic[cframe].offsetX * NPC.direction
                                                - (NPC.direction - 1) * dic[cframe].width / 2,//+为0，-为width

                                                 dic[cframe].originalHeight / 2
                                                 - dic[cframe].offsetY)

    + new Vector2(NPC.direction * drawStartOffsetX, drawStartOffsetY);


        if (NPC.ai[0] == 5) // 保险起见，检测一下
        {
            if (Main.GameUpdateCount % 4 == 0) // 因为抽帧，所以动作播放也要随之放慢一点
                NPC.ai[3] += 2; // 抽帧播放，细胞看起来就是这么搞得
            if (NPC.ai[3] >= 18) // 动作结束
            {
                NPC.ai[0] = 7; // 进入走路
            }
        }
        /*
        if (NPC.ai[0] == 8)
        {
            if (Main.GameUpdateCount % 4 == 0)
                NPC.ai[3] += 2; // 抽帧播放，细胞看起来就是这么搞得
            if (NPC.ai[3] >= 9) // 动作结束
            {
                NPC.ai[0] = 7; // 另一条腿，非得分成两个吗
                ReplayFrame();
            }
        }
        */


    }

    public void DrawBeheTexture(Texture2D glowTex, SpriteBatch spriteBatch, Color drawColor, Rectangle rectangle)
    {
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

        if (drawShield)
        {
            int alpha = 160 - 12 * ((int)NPC.ai[3] % 3) - (int)(NPC.ai[3] * 2.2f);
            spriteBatch.Draw(AssetsLoader.Shield1, rectangle, new(255, 185, 135, alpha));
        }

        AssetsLoader.glowEffect.Parameters["InputR"].SetValue(drawColor.R);
        AssetsLoader.glowEffect.Parameters["InputG"].SetValue(drawColor.G / 255f);
        AssetsLoader.glowEffect.Parameters["InputB"].SetValue(drawColor.B / 255f);
        AssetsLoader.glowEffect.CurrentTechnique.Passes["fx"].Apply();
        spriteBatch.Draw(glowTex,
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

    public void DrawBehefxTexture(Dictionary<int, DCAnimPic> fxDic, int beginIndex = 0, float drawStartOffsetX = 0, float drawStartOffsetY = 0, bool shader = false, Color fxColor = default)
    {
        SpriteBatch sb = Main.spriteBatch;
        int index = (int)NPC.ai[3] - beginIndex;
        Rectangle rectangle = new(fxDic[index].x, fxDic[index].y,
                                                    fxDic[index].width, fxDic[index].height);

        Vector2 vector = new Vector2(fxDic[index].originalWidth / 2 * NPC.direction //参考解包图片如果在大图里是如何绘制的
                                                        - fxDic[index].offsetX * NPC.direction
                                                        - (NPC.direction - 1) * fxDic[index].width / 2,//+为0，-为width

                                                         fxDic[index].originalHeight / 2
                                                         - fxDic[index].offsetY)

            + new Vector2(NPC.direction * drawStartOffsetX, drawStartOffsetY);



        sb.End();
        sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

        sb.Draw(AssetsLoader.ChooseCorrectAnimPic(fxDic[index].index, fxEnemy: true),
                NPC.Center - Main.screenPosition,
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
}
