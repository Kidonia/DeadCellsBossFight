using DeadCellsBossFight.Contents.Buffs;
using DeadCellsBossFight.Contents.SubWorlds;
using DeadCellsBossFight.Core;
using DeadCellsBossFight.NPCs;
using DeadCellsBossFight.Utils;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Contents.GlobalChanges;

public class DCPlayer : ModPlayer
{
    //public int FallDownWorldTime;
    public bool QueenPlot1;
    public static int TrapDamagedTimer;
    public  int drugTime;


    public bool ShouldPlayCritSound;

    public bool StopMoving;
    public bool grabbedByQueen;

    public bool StopSentryItemUse;
    public int StopSentryItemUseTime;

    public bool StopRopeItemUse;
    public int StopRopeItemUseTime;

    public bool StopMinionPetUse;
    public int StopMinionPetUseTime;

    public int BleedLevel;

    public int TimeCanConsistentAttack = 0;
    public int ConsistentLockCtrlAfter = 0;
    public int NextStrikeChainNum = 1;
    public int WeaponCoolDown = 0;

    public int QuickSwordAtkComboNum = 0;
    public int QuickSwordComboBetweenTime = 0;

    public int OilSwordHitFireTargetTime = 0;

    public int KingsSpearComboKillNum = 0;
    public int KingsSpearKillShortPerioud = 0;
    public int KingsSpearCritTime = 0;

    public int PerfectHalberdHurtTime = 0;

    public bool TickScytheCheckHit = false;
    public bool TickScytheCanCrit = false;
    public int TickScytheCanCritTime = 0;

    public static float yamadoExtraDrawRotation = 0;
    public static Projectile yamadoHeldProj;

    public Projectile DCweaponProj;

    public int CollectorIdx = -1;
    public override void UpdateBadLifeRegen()
    {
        if (Player.HasBuff<Bleed>())
        {
            if (Player.lifeRegen > 0)
                Player.lifeRegen = 0;

            Player.lifeRegenTime = 22 - BleedLevel * 2;
            Player.lifeRegen = -BleedLevel * 3;
        }

    }
    public override void PostItemCheck()
    {
        base.PostItemCheck();
    }
    public override void OnEnterWorld()
    {
        DCweaponProj = Projectile.NewProjectileDirect(Player.GetSource_None(), Vector2.Zero, Vector2.Zero, 10, 0, 0);
        DCweaponProj.active = false;
    }
    public override void PreUpdate()
    {
        //Main.NewText(Main.mouseItem.shoot);

        //计时器
        if (TimeFrozeScreenPos > 0)
            TimeFrozeScreenPos--;
        if(TimeFrozeScreenPos == 0)
        {
            ShouldFrozeScreenPos = false;
            forceScreenPosition = default;
            TimeFrozeScreenPos = -1;

            ShouldStepToNoFrozenPos = true;
        }

        if (drugTime > 1)
            drugTime--;
        else if (drugTime == 1)
        {
            //Player.position = DCWorldSystem.shimmerPosition;
            
            Player.velocity = Vector2.Zero;
            Player.fallStart = (int)(Player.position.Y / 16f);
            drugTime = 0;
        }
        // 隐藏绘制
        if(playerHideDrawTime > 0)
            playerHideDrawTime--;
        if(playerHideDrawTime == 1)
        {
            if(yamadoHeldProj != null && yamadoHeldProj.active)
            {
                // yamadoHeldProj.ai[0] = 1; // 进入收刀动作
                Main.projectile[yamadoHeldProj.whoAmI].ai[0] = 1;
                DCPlayer.yamadoExtraDrawRotation = -MathHelper.Pi;
                // Main.NewText(yamadoHeldProj.ai[0]);
            }

            for (int i = 0; i < 5; i++)
            {
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings
                {
                    PositionInWorld = Main.LocalPlayer.Top - new Vector2(Main.rand.NextFloat(-4, 4), 20),
                    MovementVector = new Vector2(Main.rand.NextFloat(-1, 1), -Main.rand.NextFloat(-8, -3))
                });
            }

        }
        
        // 画闪电
        if (playerDrawLightningTime > 0)
            playerDrawLightningTime--;

        //两段攻击间隔剩余时间，用于判断武器能否进行连贯攻击
        if (TimeCanConsistentAttack > 0)
            TimeCanConsistentAttack--;
        //剩余时间为1帧，刷新使武器回到第一段攻击
        if (TimeCanConsistentAttack == 1)
        {
            NextStrikeChainNum = 1;
        }

        if (TickScytheCanCritTime > 0)
            TickScytheCanCritTime--;

        if (TickScytheCanCritTime == 1)
        {
            TickScytheCanCrit = false;//巨镰下段攻击不可暴击。
            TickScytheCheckHit = false;//巨镰击中敌人的判定刷新。
        }

        //可进行下一段攻击的剩余时间，或者说多少时间后能进行下一段攻击
        if (ConsistentLockCtrlAfter > 0)
            ConsistentLockCtrlAfter--;
        //武器冷却时间，能够重新使用该武器的时间
        if (WeaponCoolDown > 0)
            WeaponCoolDown--;

        if (NextStrikeChainNum == 0)
            NextStrikeChainNum++;//避免便乘0的情况，正常不应该出现


        //油刀
        if (OilSwordHitFireTargetTime > 0)
            OilSwordHitFireTargetTime--;

        //化境
        if (PerfectHalberdHurtTime > 0)
            PerfectHalberdHurtTime--;



        //对称长枪
        if (KingsSpearKillShortPerioud > 0)
            KingsSpearKillShortPerioud--;
        if (KingsSpearCritTime > 0)
            KingsSpearCritTime--;

        if (KingsSpearKillShortPerioud == 1)//如果迅速击杀计时器耗尽
            KingsSpearComboKillNum = 0;//击杀数清零

        if (KingsSpearComboKillNum == 1 && KingsSpearKillShortPerioud == 0)//如果只杀了一个，且，迅速击杀计时器为0
            KingsSpearKillShortPerioud = 75;//迅速击杀计时器设为48
        if (KingsSpearComboKillNum > 1)//如果杀了超过一个
        {
            KingsSpearComboKillNum = 0;//击杀计数清零
            KingsSpearCritTime = 360;//可暴击时间设为360（6秒）
        }
        if (StopSentryItemUseTime > 0)
            StopSentryItemUseTime--;
        if (StopSentryItemUseTime == 0)
        {
            StopSentryItemUseTime--;
            StopSentryItemUse = false;
        }

        if (StopRopeItemUseTime > 0)
            StopRopeItemUseTime--;
        if (StopRopeItemUseTime == 0)
        {
            StopRopeItemUseTime--;
            StopRopeItemUse = false;
        }

        if (StopMinionPetUseTime > 0)
        {
            StopMinionPetUseTime--;
        }
        if(StopMinionPetUseTime == 0)
        {
            StopMinionPetUseTime--;
            StopMinionPetUse = false;
        }

        if (SubworldSystem.IsActive<QueenArenaWorld>())
        {
            if (Player.position.Y > 1800 && TrapDamagedTimer == 0)
            {
                Player.immune = false;
                Player.immuneTime = 0;
                //Main.rand.Next(100, 120)
                Player.RemoveAllGrapplingHooks();
                Player.Hurt(PlayerDeathReason.ByCustomReason(""), 80, 0, false, false, -1, false, 0f, 0f, 4.5f);
                Player.immuneTime += 100;
                Player.immune = true;
                TrapDamagedTimer = 61;
            }
            // Main.NewText(Player.Center);
            if (TrapDamagedTimer > 0)
            {
                TrapDamagedTimer--;

                if (TrapDamagedTimer > 30)
                    Main.blockInput = true;

                if (TrapDamagedTimer == 30)
                {
                    Main.blockInput = false;
                    Player.velocity *= 0f;
                    if (Player.Center.X < 3000)
                    {
                        Player.Center = new Vector2(1400f, 1462f);
                        Player.direction = 1;
                    }
                    else
                    {
                        Player.Center = new Vector2(4790f, 1462f);
                        Player.direction = -1;
                    }
                }

            }
        }



        if (Player.ZoneShimmer)
        {
            if (CollectorIdx < 0 || CollectorIdx > 200 || NPC.FindFirstNPC(ModContent.NPCType<Collector>()) == -1)
                CollectorIdx = SpawnCollectorNPC();
            else if (Main.npc[CollectorIdx].active == false || Main.npc[CollectorIdx].type != ModContent.NPCType<Collector>())
                CollectorIdx = SpawnCollectorNPC();

        }


        //Main.NewText(Player.stardustMinion, Color.Red);
        /*
        if (Main.playerInventory)
            Main.NewText("");
        else
            Main.NewText("");
        */
    }

    /*
    public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
    {
        TickScytheCanCrit = false;//巨镰下一段不可暴击
        PerfectHalberdHurtTime = 900;//化境15秒不暴击
        base.OnHitByNPC(npc, hurtInfo);
    }
    */
    public override void UpdateEquips()
    {
        /*
        for(int i = 0; i < Player.miscEquips.Length; i++)
        {
            if (Player.hideMisc[i])
                Main.NewText(i);
        }
        */
        base.UpdateEquips();
    }

    public override void PreUpdateBuffs()
    {
        if (drugTime > 0)
        {
            if (Player.statLife < 60)
            {
                Player.Heal(Main.rand.Next(64, 18500) * 10);
                //Player.HealEffect(1600, true);
            }
            // Main.NewText(NormalUtils.DrugBuffsToAdd.Count);
            //NormalUtils.DrugBuffsToAdd.Count==49
            if (Main.rand.NextBool(4))
            {
                Player.AddBuff(NormalUtils.DrugBuffsToAdd[Main.rand.Next(0, 50)], Main.rand.Next(60, 180));
            }
            else if (Main.rand.NextBool(40))
            {
                Player.TeleportationPotion();//随机传送
            }
            if (Main.rand.NextBool(30))
            {
                Player.Heal(Main.rand.Next(700, 11500) * 10);
            }
        }
        base.PreUpdateBuffs();
    }

    public override void PostUpdateBuffs()
    {
        base.PostUpdateBuffs();
    }

    public override void PostUpdate()
    {
        if (StopMinionPetUse && Player.HasBuff(BuffID.StardustGuardianMinion))
        {
            Player.ClearBuff(BuffID.StardustGuardianMinion);
        }

        if (grabbedByQueen)
            if (Main.GameUpdateCount % 15 == 0)
            {
                Player.Hurt(PlayerDeathReason.ByCustomReason("你被吸干了灵魂..."), 70, 0, false, false, -1, false, 0f, 0f, 0f);
                Player.immuneTime = 0;
            }
        base.PostUpdate();
    }
    public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
    {
        if (NormalUtils.modAnimProjList.Contains(proj.type))
        {
            TickScytheCanCritTime = 600;//巨镰砍到10秒内暴击
            TickScytheCanCrit = true;
        }
        //巨镰下一段不可暴击
        //PerfectHalberdHurtTime = 900;//化境15秒不暴击

        base.OnHitByProjectile(proj, hurtInfo);
    }
    public override bool CanUseItem(Item item)
    {
        //Main.NewText(item.type);
        if(StopSentryItemUse)
        {
            //UI用
            /*
            if(StopSentryItemUseTime > 0)
            {
                string t = StopSentryItemUseTime > 60 ? (StopSentryItemUseTime / 60).ToString() : (StopSentryItemUseTime / 60f).ToString("0.0");
                Main.NewText($"炮塔使用冷却中！剩余" + t + "秒");
            }
            */
            if(item.sentry)
            {
                string t = (StopSentryItemUseTime / 60f).ToString("0.0");
                Main.NewText($"炮塔使用冷却中！剩余" + t + "秒");
                return false;
            }
        }
        if(StopRopeItemUse)
        {
            if (item.type == ItemID.Rope || item.type == ItemID.RopeCoil ||
                item.type == ItemID.SilkRope || item.type == ItemID.SilkRopeCoil ||
                item.type == ItemID.VineRope || item.type == ItemID.VineRopeCoil ||
                item.type == ItemID.WebRope || item.type == ItemID.WebRopeCoil || item.type == ItemID.MysticCoilSnake)
            {
                string t = (StopRopeItemUseTime / 60f).ToString("0.0");
                Main.NewText($"绳索物品使用冷却中！剩余" + t + "秒");
                return false;
            }
        }
        if(StopMinionPetUseTime > 0)
        {
            if((item.DamageType == DamageClass.Summon) || Main.vanityPet[item.buffType] || Main.lightPet[item.buffType])
            {
                string t =  (StopMinionPetUseTime / 60f).ToString("0.0");
                Main.NewText($"召唤物使用冷却中！剩余" + t + "秒");
                return false;
            }
        }
        return base.CanUseItem(item);
    }
    public override void SetControls()
    {
        if (StopMoving || grabbedByQueen)
        {
            Player.controlInv = false;
            Player.controlMap = false;
            Player.controlMount = false;
            Player.controlSmart = false;
            Player.controlTorch = false;
            Player.controlHook = false;
            Player.controlJump = false;
            Player.controlDown = false;
            Player.controlLeft = false;
            Player.controlRight = false;
            Player.controlUp = false;
            Player.controlUseItem = false;
            Player.controlUseTile = false;
            Player.controlThrow = false;

            Player.wingTime = 0;
            Player.rocketTime = 0;
            Player.sandStorm = false;
            Player.dash = 0;
            Player.dashType = 0;
            Player.noKnockback = true;
            Player.RemoveAllGrapplingHooks();
            Player.StopVanityActions();
            Player.StopExtraJumpInProgress();

            if (Player.mount.Active)
                Player.mount.Dismount(Player);

            if (Player.pulley)
                Player.pulley = false;

        }
    }
    public override void ResetEffects()
    {
        if(SubworldSystem.AnyActive())
        {
            drugTime = 0;

        }
    }
    public static int playerDrawLightningTime;

    public static int playerHideDrawTime;
    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        if (playerDrawLightningTime > 0 || Main.LocalPlayer.electrified)
            drawInfo.drawPlayer.electrified = true;
        else
            drawInfo.drawPlayer.electrified = false;
        base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
    }
    public override void HideDrawLayers(PlayerDrawSet drawInfo)
    {
        //Main.NewText(Player.position);
        if(playerHideDrawTime > 0)
            foreach (PlayerDrawLayer playerDrawLayer in PlayerDrawLayerLoader.Layers)
            {
                playerDrawLayer.Hide();
            }

        if (SubworldSystem.IsActive<QueenArenaWorld>() && Player.position.Y > 1800)
        {

            // 隐藏所有绘制
            foreach (PlayerDrawLayer playerDrawLayer in PlayerDrawLayerLoader.Layers)
            {
                playerDrawLayer.Hide();
            }
        }
        base.HideDrawLayers(drawInfo);
    }

    public static Vector2 forceScreenPosition;
    public bool ShouldFrozeScreenPos;
    public int TimeFrozeScreenPos;

    public bool ShouldStepToNoFrozenPos;
    public int TimeStepToNoFrozenPos;
    private int TimeStepped;
    private Vector2 stepToThisPos;
    public override void ModifyScreenPosition()
    {
        if(ShouldStepToNoFrozenPos)
        {
            stepToThisPos = Main.screenPosition;
            TimeStepped++;
            if(TimeStepped > TimeStepToNoFrozenPos)
            {
                TimeStepped = 0;
                TimeStepToNoFrozenPos = 0;
                ShouldStepToNoFrozenPos = false;
                return;
            }
            forceScreenPosition = Vector2.Lerp(forceScreenPosition, stepToThisPos, (float)TimeStepped / TimeStepToNoFrozenPos);
            Main.screenPosition = forceScreenPosition;


        }
        else if (ShouldFrozeScreenPos)
        {
            if (forceScreenPosition == default)
            {
                Main.NewText("ERROR ScreenPos");
                return;
            }
            Main.screenPosition = forceScreenPosition;
        }
        else
            base.ModifyScreenPosition();
    }
    public void ActivateScreenPosFrozen(int time = -1, Vector2 screenPositon = default)
    {
        ShouldStepToNoFrozenPos = false;
        TimeStepped = 0;
        TimeStepToNoFrozenPos = 0;

        forceScreenPosition = (screenPositon == default) ? Main.screenPosition : screenPositon;

        TimeFrozeScreenPos = time;
        ShouldFrozeScreenPos = true;
    }

    public void StopScreenPosFrozen(int steptime = 60)
    {
        TimeFrozeScreenPos = -1;
        ShouldFrozeScreenPos = false;

        ShouldStepToNoFrozenPos = true;
        TimeStepped = 0;
        TimeStepToNoFrozenPos = steptime;
    }

    public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
    {
        return QueenPlot1 || base.ImmuneTo(damageSource, cooldownCounter, dodgeable);
    }

    public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
    {
        if (drugTime > 0 || QueenPlot1)
            return false;

        return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
    }

    public static  int SpawnCollectorNPC()
    {
        return NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (int)DCWorldSystem.shimmerPosition.X, (int)DCWorldSystem.shimmerPosition.Y, ModContent.NPCType<Collector>());
    }
}
