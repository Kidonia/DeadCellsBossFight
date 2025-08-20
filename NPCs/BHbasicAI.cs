using DeadCellsBossFight.Core;
using DeadCellsBossFight.NPCs.ExtraBosses;
using DeadCellsBossFight.Projectiles.BasicAnimationProj;
using DeadCellsBossFight.Utils;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DeadCellsBossFight.NPCs;

public partial class BH : ModNPC
{
    /// <summary>
    /// 追逐玩家的时间。归零则直接接上下一段攻击。
    /// </summary>
    public int chasingTime;
    /// <summary>
    /// 执行跳跃动作的冷却，避免跳完落地紧接着一直跳个不停。
    /// </summary>
    public int jumpCooldown;
    /// <summary>
    /// 二段跳所需的间隔时间，即第一段跳之后的间隔。
    /// </summary>
    public int secondJumpWaitTime;
    /// <summary>
    /// 翻滚方向。
    /// </summary>
    public int rollDirection;
    /// <summary>
    /// 翻滚无敌时间。
    /// </summary>
    public int rollImmuneTime;
    /// <summary>
    /// 翻滚的冷却时间。后期可以适当缩小。
    /// </summary>
    public int rollCooldown;
    public int healPause;
    public int healCount;
    public int floatingTimeAfterHeal;
    /// <summary>
    /// 记录最后三个运动状态。一般用于在攻击之间衔接翻滚跳跃等动作。新动作会加在[2]，向前挤掉[0]
    /// </summary>
    public BHMoveType[] lastThreeMovement;
    /// <summary>
    /// 一些动作要固定方向，不朝着玩家（爬墙等）
    /// </summary>
    public int ForceDirection;
    /// <summary>
    /// 子世界中间场地左墙
    /// </summary>
    public const int FieldLeft = 704;
    /// <summary>
    /// 子世界中间场地最右边界
    /// </summary>
    public const int FieldRight = 2528;

    /// <summary>
    /// 瓶子底端位置
    /// </summary>
    public const int BottleBottom = 1360;

    /// <summary>
    /// 中轴线X
    /// </summary>
    public const int FieldMiddle = 1616;

    /// <summary>
    /// 子世界中间场地地面纵坐标
    /// </summary>
    public const int FieldGround = 1632;


    public int standingtime; // 强制站着不动的时间。
    //None使用，站立不动，重设一些属性
    public void AI_Idle()
    {
        if (standingtime > 0)
        {
            standingtime--;
            // 站着不动时，偶尔触发一些小动作 （拟人技）
            if (Main.rand.NextBool(180) && hesitateCD == 0)
                ChangeMove_Hesitate(BHHesitateType.determined); // type后续可以改成随机的
        }
        else if (X_abs_distance > 150f) // 距离较远，开始走向玩家
            if (Main.rand.NextBool(40))
            {
                walkbacktime = Main.rand.Next(36, 64);
                ChangeMove(BHMoveType.SmallWalkBack);
            }
            else
                ChangeMove(BHMoveType.Walk);
        else if (Main.rand.NextBool(20)) // 离玩家150码以内，可以有适当的迟疑拟人，即往回走一段距离
        {
            walkbacktime = Main.rand.Next(28, 50);
            ChangeMove(BHMoveType.SmallWalkBack);
        }


        NPC.velocity.X *= 0; // 站立不动
        NPC.noGravity = false; // 恢复重力影响
        NPC.noTileCollide = false; // 恢复碰撞
        NPC.dontTakeDamage = false; // 恢复可受伤
        CheckATK();//检测是否变换攻击

    }
    public BHHesitateType hesitateType;
    public void AI_Hesitate()
    {
        NPC.velocity.X *= 0; // 站立不动
        NPC.noGravity = false; // 恢复重力影响
        NPC.noTileCollide = false; // 恢复碰撞
        NPC.dontTakeDamage = false; // 恢复可受伤
        
        // 可以尝试添加DialogueBox ///////////////////////////////



        // 请注意！
        // 本 AI 里 ChangeMove(BHMoveType.Idle) 部分在每个Proj的AI中实现！
        // 参考：DeterminedProj 的 AI() 部分：
        // if (Projectile.frame == TotalFrame)
        // {
        //     Projectile.frame = 0;
        //     var BH = npc.ModNPC as BH;
        //     BH.standingtime = Main.rand.Next(36, 72);
        //     BH.ChangeMove(BHMoveType.Idle);
        // }
    }
    public int hesitateCD = 300;
    public void ChangeMove_Hesitate(BHHesitateType hesitatetype)
    {
        hesitateType = hesitatetype;
        hesitateCD = 300;
        ChangeMove(BHMoveType.Hesitate);
    }

    public void AI_Heal()
    {

        NPC.velocity.X *= 0; // 站立不动
        NPC.dontTakeDamage = true; // 免伤
        if(healPause > 0)
        {
            Main.NewText("heal");
            healPause--;
        }
        else if (Main.GameUpdateCount % 12 == 0 && healCount > 0) // 每次回血10%，共6次
        {
            for (int i = 0; i < NPC.buffTime.Length; i++) // 清除负面效果
            {
                NPC.buffTime[i] *= 0;
            }
            healCount--;
            NPC.life += 25000;
        }
        if(healCount==0 && CurrentState != BHState.Fourth)//回完血，且 不是最后阶段，进入召唤BOSS躲起来阶段
        {
            Player player = Main.player[NPC.target];

            if (CurrentState == BHState.Begin)
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)player.Center.X, (int)player.Center.Y - 550, ModContent.NPCType<Behemoth>());
            ChangeMove(BHMoveType.HideAndSummon);
        }
    }
    
    /// <summary>
    /// 好点子：这部分可以试试阶段换皮，‘；放到阶段换皮，换皮王手之后再使用
    /// </summary>
    public void AI_HideAndSummon()
    {
        NPC.dontTakeDamage = true; // 参考王手，此阶段空中正弦漂浮，免疫伤害

        if (floatingTimeAfterHeal > 66)//停一会
        {
            floatingTimeAfterHeal--;
        }
        else if (floatingTimeAfterHeal > 0) // 漂浮
        {
            floatingTimeAfterHeal--;
            NPC.noGravity = true; // 免受重力影响
            NPC.velocity.Y = -2f; // 向上飘浮
        }
        if(floatingTimeAfterHeal == 0) // 漂浮结束，召唤并正弦上下飘
        {
            NPC.noGravity = true;
            float shakeT = (float)Math.Sin(Main.GameUpdateCount / 24f);
            NPC.velocity.Y = shakeT; // 上下浮动效果。
        }
    }


    //翻滚
    public void AI_Roll()
    {
        if (rollImmuneTime > 0) // 翻滚时无敌
        {
            rollImmuneTime --;
            NPC.dontTakeDamage = true; // 翻滚时无敌
        }
        NPC.velocity.X = rollDirection * 5.2f; // 速度要调!!!
        if (rollImmuneTime <= 0)
        {
            SoundEngine.PlaySound(AssetsLoader.hit_crit);
            NPC.dontTakeDamage = false; // 翻滚结束，可受伤害
            rollCooldown += 120; // 翻滚冷却
            if(lastThreeMovement[1] == BHMoveType.SmashDown) // 砸地衔接翻滚，则不回到砸地，不然一直翻滚
            {
                ChangeMove(BHMoveType.Walk);
                return;
            }
            ChangeMove(lastThreeMovement[1]); // 回到上一段动作
        }
    }


    //走路
    public void AI_Walk()
    {
        CheckATK();   
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * 5.5f, 0.35f); // 走路速度要调
        if (Main.rand.NextBool(20))
        {
            AddClimbWallThenSmashDown();
        }
        if (X_abs_distance < 50f)
            ChangeMove(BHMoveType.Idle);
    }




    //一段跳
    public void AI_Jump()
    {
        if (X_abs_distance > 40f)
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * 5.4f, 0.35f); // 速度要调


        NPC.velocity.Y -= 6.2f; // 直接跳起来
        jumpCooldown += 100; // 跳跃冷却

        ChangeMove(lastThreeMovement[1]); // 回到上一段动作
    }
    public void AI_DoubleJump()
    {
        if (X_abs_distance > 40f)
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * 5.4f, 0.35f); // 速度要调


        if (secondJumpWaitTime == 28) // 第一段跳跃
            NPC.velocity.Y -= 6.4f;

        secondJumpWaitTime--;
        if (secondJumpWaitTime <= 0) // 第二段跳跃
        {
            NPC.velocity.Y -= 6f;
            jumpCooldown += 120; // 添加冷却
            secondJumpWaitTime = 28; // 重置第二段跳所需的间隔时间
            ChangeMove(lastThreeMovement[1]); // 回到上一段动作
        }
    }
    /// <summary>
    /// 爬哪一侧的墙，左-1，右1，默认0
    /// </summary>
    public int ClimbSide = 0;
    public bool isClimbingWall;
    /// <summary>
    /// 爬墙的时间
    /// </summary>
    public int ClimbTime = 40;
    /// <summary>
    /// 从墙上跳开的时间，时间结束进行下砸
    /// </summary>
    public int jumpWallTime = 0;
    /// <summary>
    /// 别的动作需要检测使用移动弹幕的，加上这个，改为true
    /// </summary>
    public bool extraMoveCheck;

    public bool ClimbWalking;
    private int smallAnimClimbPause = 0;
    /// <summary>
    /// 爬墙时使用攻击键重置爬墙技巧，拟人技。此弹幕为那一帧攻击的弹幕。
    /// </summary>
    private Projectile animBetweenClimb;
    public void AI_ClimbWallAndSmashDown()
    {
        if(ClimbSide == 0)
        {
            ClimbSide = -1;
        }
        NPC.direction = ClimbSide;

        if(jumpWallTime > 0)
        {
            NPC.direction = -ClimbSide;
            extraMoveCheck = true;
            jumpWallTime--;
            NPC.velocity = new Vector2(ClimbSide * -12f, -10f);
            if (jumpWallTime == 0 || X_abs_distance < 30)
            {
                jumpWallTime = 0;
                extraMoveCheck = false;
                isClimbingWall = false;
                ClimbTime = 50;
                ChangeMove(BHMoveType.SmashDown);
            }
            return;
        }
        if ( ! isClimbingWall) // 走路&跳跃
        {
            extraMoveCheck = true;
            if(ClimbWalking && basicMoveProj.type != ModContent.ProjectileType<WalkProj>())
            {
                basicMoveProj.Kill();
                basicMoveProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<WalkProj>(), 0, 0, -1, DrawTrail);
            }
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, ClimbSide * 5.6f, 0.35f); // 走路速度要调
            switch (ClimbSide)
            {
                case -1:
                    if (NPC.Center.X < FieldMiddle - 730)
                    {
                        if (secondJumpWaitTime == 28) // 第一段跳跃
                        {
                            ClimbWalking = false;
                            NPC.velocity.Y -= 6.4f;
                        }

                        secondJumpWaitTime--;
                        if (secondJumpWaitTime <= 0) // 第二段跳跃
                        {
                            NPC.velocity.Y -= 6f;
                            secondJumpWaitTime = 999; // 暂缓
                        }
                    }
                    Point16 bottom = (NPC.Left + new Vector2(-2, 0)).ToTileCoordinates16();
                    if (Main.tile.TryGet(bottom, out Tile tile) && tile.HasTile)
                    {
                        isClimbingWall = true;
                        secondJumpWaitTime = 28;
                        ClimbTime = 50;
                        extraMoveCheck = false;
                        basicMoveProj.Kill();
                        basicMoveProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<WallRunProj>(), 0, 0, -1, 1); // 必定残影
                    }
                    break;
                case 1:
                    if (NPC.Center.X > FieldMiddle + 730)
                    {
                        if (secondJumpWaitTime == 28) // 第一段跳跃
                        {
                            ClimbWalking = false;
                            NPC.velocity.Y -= 6.4f;
                        }

                        secondJumpWaitTime--;
                        if (secondJumpWaitTime <= 0) // 第二段跳跃
                        {
                            NPC.velocity.Y -= 6f;
                            secondJumpWaitTime = 999; // 暂缓
                        }
                    }
                    Point16 bottom2 = (NPC.Right + new Vector2(+2, 0)).ToTileCoordinates16();
                    if (Main.tile.TryGet(bottom2, out Tile tile2) && tile2.HasTile)
                    {
                        isClimbingWall = true;
                        secondJumpWaitTime = 28;
                        ClimbTime = 50;
                        extraMoveCheck = false;
                        basicMoveProj.Kill();
                        basicMoveProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<WallRunProj>(), 0, 0, -1, 1); // 必定残影
                    }
                    break;
            }
        }
        else // 爬墙ing
        {
            DrawTrail = 1; // 开启残影
            NPC.velocity = new Vector2(ClimbSide, -8.6f);
            ClimbTime--;
            if (ClimbTime % 20 == 0) // 爬墙相关的速度，时机尽量都不再变。墙上一共使用两次攻击重置
            {
                basicMoveProj.localAI[2] = 1; // 详见WallRunProj的PostDraw。1 为不进行绘制，其他正常绘制。
                smallAnimClimbPause = 4;
                if (CurrentWeaponAtkMode == BHWeaponAtkMode.HeavyAtk) // 重武多停留一帧
                    smallAnimClimbPause++;
                animBetweenClimb = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, NormalUtils.WeaponFirstAtkProjType[CurrentWeaponType], 0, 0, -1, DrawTrail);
            }
            if(smallAnimClimbPause > 0)
            {
                smallAnimClimbPause--;
                if (smallAnimClimbPause == 0)
                {
                    basicMoveProj.localAI[2] = 0;
                    animBetweenClimb.Kill();
                }
            }
            if(ClimbTime == 0)
            {
                animBetweenClimb.Kill();
                jumpWallTime = 18;
                extraMoveCheck = true;
            }
        }

    }

    public int walkbacktime;
    public void AI_SmallWalkBack()
    {
        CheckATK();
        NPC.direction = ForceDirection;
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, ForceDirection * 5.5f, 0.35f); // 走路速度要调
        walkbacktime--;
        if ((NPC.Left.X - FieldLeft < 16) || (FieldRight - NPC.Right.X < 16))
        {
            walkbacktime = 0;
            Main.NewText("沾边");
        } 
        if (walkbacktime == 0)
        {
            standingtime = Main.rand.Next(12, 60); // 走回去小站一会。
            ChangeMove(BHMoveType.Idle);
        }
    }
    public void AddClimbWallThenSmashDown()
    {
        if (NPC.Center.X < FieldMiddle - 350 && player.velocity.X < 0)
        {
            ClimbSide = -1;
            ChangeMove(BHMoveType.ClimbWallAndSmashDown);
        }
        else if (NPC.Center.X > FieldMiddle + 350 && player.velocity.X > 0)
        {
            ClimbSide = 1;
            ChangeMove(BHMoveType.ClimbWallAndSmashDown);
        }
    }
    public void AI_SmashDown()
    {
        DrawTrail = 1;
        NPC.velocity.X *= 0;
        NPC.velocity.Y = 22f;
        // NPC.GravityMultiplier *= 2f;
        NPC.MaxFallSpeedMultiplier *= 2.2f;
        //// 添加洋葱皮√
        NPC.damage = 45 + (int)CurrentState * 10;
        NPC.defense = 40;
        Point16 bottom = (NPC.Bottom + new Vector2(0, 2)).ToTileCoordinates16();
        if (Main.tile.TryGet(bottom, out Tile tile) && tile.HasTile)
        {
            //// newproj
            rollCooldown = 0;
            NPC.damage = 0;
            ChangeMove(BHMoveType.Roll);
        }
    }

    /// <summary>
    /// ！！！非常重要！！！要重写！！
    /// 检测并进行状态的变化，Walk和Idle使用这个。
    /// </summary>
    public void CheckATK()
    {
        // 表明这个move是因为攻击间隔产生的。
        if (CurrentWeaponAtkMode != 0 || CurrentATKChain != 0 || CurrentWeaponType != 0)
        {
            if (atkCoolDown == 0) // 武器攻击间隔冷却结束
            {
                if (chasingTime == 0 || distance < 220f || Main.rand.NextFloat(0, 1) < 0.05f)
                {
                    ChangeMove((BHMoveType)(int)CurrentWeaponAtkMode); // 回到那个攻击模式去，武器和段数都没变，不用改
                    chasingTime = 0; // 直接清零，避免问题
                }
            }

        }
        else if (atkCoolDown == 0)
        {
            ////////////////////////////////////////////////////////////// 需要大改 ///////////////////////////////////////////////////////
            //Main.NewText(distance);
            if (distance > 240f && distance < 400f && Main.rand.NextBool(60)) // 距离220~400码内，判定可以进行攻击动作
            {
                // Main.NewText("range");
                /*
                switch (Main.rand.Next(0, 3))
                {
                    case 0:
                        ChangeMove(BHMoveType.BowAtk);
                        break;
                    case 1:
                        ChangeMove(BHMoveType.CrossbowAtk);
                        break;
                }
                return;
                */
            }
            else if (distance > 140f && distance < 280f && Main.rand.NextBool(60)) // 距离180 ~280码内，判定可以进行重武飞头动作
            {
                // Main.NewText("closer");
                ChangeMove(BHMoveType.HeavyAtk);
                CurrentWeaponType = (BHWeaponType)Main.rand.Next(8, 14);
                /*
                switch (Main.rand.Next(0, 3))
                {
                    case 0:
                        ChangeMove(BHMoveType.HeavyAtk);
                        break;
                    case 1:
                        ChangeMove(BHMoveType.CrossbowAtk);
                        break;
                    case 2:
                        ChangeMove(BHMoveType.HeadAtk);
                        break;
                }
                return;
                */
            }
            else if (distance < 180f && Main.rand.NextBool(45)) // 距离180码内，判定可以进行攻击动作
            {
                // Main.NewText("near");
                ChangeMove(BHMoveType.SwordAtk);
                CurrentWeaponType = (BHWeaponType)Main.rand.Next(1, 8);
                /*
                switch (Main.rand.Next(0, 4))
                {
                    case 0:
                        ChangeMove(BHMoveType.HeavyAtk);
                        break;
                    case 1:
                    case 2:
                        ChangeMove(BHMoveType.SwordAtk);
                        break;
                    case 3:
                        ChangeMove(BHMoveType.BowAtk);
                        break;
                }
                return;
                */
                //Main.NewText("close");
            }
            else
            {

            }
        }
        if (jumpCooldown == 0)
        {
            if (Main.LocalPlayer.Bottom.Y >NPC.Bottom.Y && Main.rand.NextBool(40)) // 避免卡墙，一种糟糕低能的写法，要修正
            {
                ChangeMove_ChooseRandomJump(); // 随机一段跳还是二段跳
                return;
            }
            if (Main.rand.NextBool(70))
            {
                ChangeMove_ChooseRandomJump(); // 随机一段跳还是二段跳
                return;
                //Main.NewText("jump");
            }
        }
        if (rollCooldown == 0 && Main.rand.NextBool(90)) // 随机翻滚
        {
            ChangeMove(BHMoveType.Roll);
            //Main.NewText("roll");
            return;
        }
    }





    /// <summary>
    /// 更改运动状态，记录更改后最新三次运动的状态。
    /// </summary>
    /// <param name="move"></param>
    public void ChangeMove(BHMoveType move)
    {
        // NormalUtils.ClearMoveProjs();
        basicMoveProj.Kill();
        ForceDirection = Math.Sign(player.Center.X - NPC.Center.X);
        if (move == BHMoveType.Idle) // 将换成Idle，生成动画弹幕
        {
            basicMoveProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<IdleProj>(), 0, 0, -1, DrawTrail);
        }
        else if (move < (BHMoveType)7) // 将换成攻击，对应切换状态
        {
            CurrentWeaponAtkMode = (BHWeaponAtkMode)(int)move;
        }
        if(move == BHMoveType.Roll) // 将换成翻滚，生成动画弹幕，添加计时器
        {
            basicMoveProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<RollAProj>(), 0, 0, -1, DrawTrail);
            rollImmuneTime = 48;
            rollDirection = NPC.direction;
        }
        if(move == BHMoveType.SmashDown)
        {
            NPC.velocity.X *= 0;
            basicMoveProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<JumpDownProj>(), 0, 0, -1, 1); // 必定开启残影 
        }
        if(move == BHMoveType.Heal) // 将换成回血，生成动画弹幕，要完善
        {
            healPause = 300;
            healCount = 6;
        }
        if(move == BHMoveType.HideAndSummon)
        {
            floatingTimeAfterHeal = 100;
        }
        if(move == BHMoveType.ClimbWallAndSmashDown)
        {
            ClimbWalking = true;
            basicMoveProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<WalkProj>(), 0, 0, -1, DrawTrail);
        }
        if(move == BHMoveType.SmallWalkBack)
        {
            ForceDirection = -ForceDirection; // 往反方向走
            basicMoveProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, -NPC.velocity, ModContent.ProjectileType<WalkProj>(), 0, 0, -1, DrawTrail);
        }
        if(move == BHMoveType.Hesitate) // 不应使用ChangeMove，使用 ChangeMove_Hesitate(enum_type)
        {
            // hesitateType 在 ChangeMove_Hesitate() 中改变
            basicMoveProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, NormalUtils.hesitate2proj[hesitateType], 0, 0, -1, DrawTrail);

        }

        CurrentMove = move; // 切换当前状态

        (lastThreeMovement[0], lastThreeMovement[1], lastThreeMovement[2]) // 更新最后三个状态
            = (lastThreeMovement[1], lastThreeMovement[2], move);
 
        //dic clear

    }




    /// <summary>
    /// 随机选择一段跳或二段跳
    /// </summary>
    public void ChangeMove_ChooseRandomJump()
    {
        
        // basicMoveProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<JumpUpProj>(), 0, 0);
        if (Main.rand.NextBool()) ChangeMove(BHMoveType.Jump);
        else ChangeMove(BHMoveType.DoubleJump);
    }


}
