using DeadCellsBossFight.Contents.SubWorlds;
using DeadCellsBossFight.Core;
using DeadCellsBossFight.NPCs;
using DeadCellsBossFight.NPCs.ExtraBosses.Queen;
using DeadCellsBossFight.Projectiles.NPCsProj;
using DeadCellsBossFight.Utils;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Contents.GlobalChanges;

public class DCGlobalProj : GlobalProjectile
{
    public override bool InstancePerEntity => true;

    public bool HasQueenGrabProj;
    public int timeBetween;
    public bool grabbed;
    public bool OnCollide;
    public bool isTileColideProj;
    public Projectile QueenGrabProj;
    public Vector2 shootPos = new Vector2();

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        //Main.NewText(NormalUtils.QNKillPetSentryEtcList.Count);
        /*
        if (NormalUtils.QNKillPetSentryEtcList.Contains(projectile.type))
        {
            Main.NewText("给我好好战斗！躲起来依仗那些可悲的仆从和低贱的器械来作战，真是可悲。", Color.Aquamarine);
            projectile.active = false;
            projectile.Kill();
        }
        */
        base.OnSpawn(projectile, source);
    }
    // 以下内容来自灾厄附属Mod：无上光神花园，很细致，所以全抄了
    public override bool PreAI(Projectile projectile)
    {
        /*
        if (Collision.CheckAABBvAABBCollision(Main.MouseWorld, Vector2.Zero, projectile.position, projectile.Size))
            Main.NewText(projectile.type);
        */
        //Main.NewText(TextureAssets.Projectile[projectile.type].ToString());
        if(QueenGrabProj != null && !QueenGrabProj.active)
        {
            HasQueenGrabProj = false;
            QueenGrabProj = null;
        }
        
        if(grabbed && projectile.aiStyle != ProjAIStyleID.ColdBolt && projectile.aiStyle != ProjAIStyleID.GemStaffBolt && (projectile.aiStyle != ProjAIStyleID.Arrow))
            return false;


        if (!SubworldSystem.AnyActive())
        {
            return base.PreAI(projectile);
        }
        else
        {
            if (SubworldSystem.IsActive<PrisonWorld>() && BottleSystem.ActivateBottleSystem)
            {
                if (projectile.active && projectile.Top.Y < BH.BottleBottom) // npc 在瓶子底端位置往上
                {
                    if (!BottleSystem.collisionCheckProj.Contains(projectile))
                        BottleSystem.collisionCheckProj.Add(projectile);
                }
                else
                    if (BottleSystem.collisionCheckProj.Contains(projectile))
                        BottleSystem.collisionCheckProj.Remove(projectile);
            }


            if (SubworldSystem.IsActive<QueenArenaWorld>())
            {
            
                if (Main.LocalPlayer.TryGetModPlayer(out DCPlayer dcplayer) && dcplayer.StopMinionPetUse)
                    if (projectile.type == ProjectileID.StardustGuardian)
                        projectile.Kill();
            

                // 检测炮塔存在，女王摧毁
                if (projectile.sentry)
                {

                    foreach (NPC npc in Main.npc)
                    {
                        // 女王
                        if (npc.type == ModContent.NPCType<Queen>() && npc.active && npc.ai[0] == -1 && projectile.aiStyle > 0)
                        {
                            //Main.NewText(projectile.Center.Y);
                            // ai[0]控制摧毁炮塔
                            npc.ai[0] = 1;
                            // 俩空中炮塔，要生成屏砍摧毁
                            if (projectile.aiStyle == ProjAIStyleID.LunarSentry && (projectile.Center.Y < 1300 || projectile.Center.X < 1356 || projectile.Center.X > 4832))
                            {
                                npc.ai[0] = 2;
                            }

                            npc.ai[1] = projectile.whoAmI;
                            break;

                        }

                    }

                }

                // 检测在女王塔顶里女王是否处于吸取弹幕AI
                if (!HasQueenGrabProj || QueenGrabProj == null)
                {
                    foreach (Projectile projInWorld in Main.projectile)
                    {
                        if (projInWorld.type == ModContent.ProjectileType<QueenRepelBullets>() && projInWorld.active)// 后续改成女王弹幕
                        {
                            HasQueenGrabProj = true;
                            QueenGrabProj = projInWorld;
                            break;
                        }
                    }
                }
            }




            // This apparently causes shader issues in the garden.
            // This projectile is notably used by the Shattered Community when leveling up.
            if (projectile.type == ProjectileID.DD2ElderWins)
            {
                projectile.active = false;
                return false;
            }

            // Prevent tombs from cluttering things up in the garden.
            bool isTomb = projectile.type is ProjectileID.Tombstone or ProjectileID.Gravestone or ProjectileID.RichGravestone1 or ProjectileID.RichGravestone2 or
                ProjectileID.RichGravestone3 or ProjectileID.RichGravestone4 or ProjectileID.RichGravestone4 or ProjectileID.Headstone or ProjectileID.Obelisk or
                ProjectileID.GraveMarker or ProjectileID.CrossGraveMarker or ProjectileID.Headstone;
            if (isTomb)
                projectile.active = false;

            // Prevent tile-manipulating items like the sandgun from working in the garden and messing up tiles.
            if (projectile.type == ProjectileID.DirtBomb || projectile.type == ProjectileID.DirtStickyBomb)
                projectile.active = false;
            if (projectile.type == ProjectileID.SandBallGun || projectile.type == ProjectileID.SandBallGun)
                projectile.active = false;
            if (projectile.type == ProjectileID.SandBallFalling || projectile.type == ProjectileID.PearlSandBallFalling)
                projectile.active = false;
            if (projectile.type == ProjectileID.EbonsandBallFalling || projectile.type == ProjectileID.EbonsandBallGun)
                projectile.active = false;
            if (projectile.type == ProjectileID.CrimsandBallFalling || projectile.type == ProjectileID.CrimsandBallGun)
                projectile.active = false;

            // From the Dirt Rod. Kill is used instead of active = false to ensure that the dirt doesn't just vanish and gets placed down again in its original location.
            if (projectile.type == ProjectileID.DirtBall)
                projectile.Kill();

            // No explosives.
            // MAN rocket code is evil!
            bool dryRocket = projectile.type == ProjectileID.DryRocket || projectile.type == ProjectileID.DrySnowmanRocket;
            bool wetRocket = projectile.type == ProjectileID.WetRocket || projectile.type == ProjectileID.WetSnowmanRocket;
            bool honeyRocket = projectile.type == ProjectileID.HoneyRocket || projectile.type == ProjectileID.HoneySnowmanRocket;
            bool lavaRocket = projectile.type == ProjectileID.LavaRocket || projectile.type == ProjectileID.LavaSnowmanRocket;
            bool rocket = dryRocket || wetRocket || honeyRocket || lavaRocket;

            bool dryMisc = projectile.type == ProjectileID.DryGrenade || projectile.type == ProjectileID.DryMine;
            bool wetMisc = projectile.type == ProjectileID.WetGrenade || projectile.type == ProjectileID.WetMine;
            bool honeyMisc = projectile.type == ProjectileID.HoneyGrenade || projectile.type == ProjectileID.HoneyMine;
            bool lavaMisc = projectile.type == ProjectileID.LavaGrenade || projectile.type == ProjectileID.LavaMine;
            bool miscExplosive = dryMisc || wetMisc || honeyMisc || lavaMisc;

            if (rocket || miscExplosive)
                projectile.active = false;

            return true;

        }

    }


    
    public override void PostAI(Projectile projectile)
    {
        base.PostAI(projectile);
        // 女王抓取弹幕时改变AI
        //Main.NewText(timeBetween);
        if (HasQueenGrabProj && QueenGrabProj != null)
        {
            //Main.NewText(timeBetween);
            

            // 确保当前弹幕是能伤害NPC的，不考虑其他 且 二者相互碰撞
            if (projectile.friendly && 
                Collision.CheckAABBvAABBCollision(QueenGrabProj.position, QueenGrabProj.Size, projectile.position, projectile.Size) && 
                projectile != QueenGrabProj && 
                (!ProjectileID.Sets.LightPet[projectile.type]) && 
                !Main.projPet[projectile.type] &&
                !projectile.minion &&
                projectile.aiStyle != ProjAIStyleID.Yoyo && // 悠悠球爬
                projectile.aiStyle != ProjAIStyleID.ThickLaser && // 激光，别
                projectile.aiStyle != ProjAIStyleID.HeldProjectile && // 手持弹幕死
                projectile.aiStyle != ProjAIStyleID.Hook && // 有的mod给钩子加伤害，你也死
                projectile.aiStyle != ProjAIStyleID.Pet && // 有的mod给宠物加伤害，你也死
                projectile.aiStyle != ProjAIStyleID.ShortSword && // 缩头乌龟，死
                projectile.aiStyle != ProjAIStyleID.Drill && // 钻头，死
                projectile.aiStyle != ProjAIStyleID.Explosive&& // 炸弹？轮不到你，死
                projectile.aiStyle != ProjAIStyleID.Spear && // 突刺长枪，死
                projectile.aiStyle != ProjAIStyleID.Whip && // 鞭子，死
                projectile.aiStyle != ProjAIStyleID.Flail && // 流星锤，死
                projectile.aiStyle != ProjAIStyleID.Harpoon && // 链刀，死
                projectile.aiStyle != ProjAIStyleID.Vilethorn // 魔法荆棘，死
                )
            {
                //执行第一次碰撞后的处理
                if (!grabbed)
                {
                    OnCollide = true;
                    // 标记为被抓去的弹幕
                    grabbed = true;
                    if(projectile.tileCollide)
                    {
                        isTileColideProj = true;
                        projectile.tileCollide = false;
                    }
                }
            }

            // 弹幕滞留时间
            if (QueenGrabProj.timeLeft > 2)
            {

                //只处理被抓去的弹幕，需要大大的补充
                if (grabbed)
                {

                    bool specialProj = false;
                    //  别似好吗
                    projectile.timeLeft = 250;

                    // 箭矢
                    if (projectile.arrow)
                    {
                        if (OnCollide)// 字段只执行一次
                        {
                            projectile.friendly = false;
                            OnCollide = false;
                            shootPos = projectile.position;
                            if(projectile.extraUpdates == 0)
                                projectile.extraUpdates = 1;
                            projectile.velocity *= 0.02f;

                            // 蜜蜂箭直接爆蜜蜂，抄自源码
                            if (projectile.type == 469)
                            {
                                projectile.Kill();
                                if (projectile.owner == Main.myPlayer)
                                {
                                    for (int k = 0; k < 6; k++)
                                    {
                                        if (k % 2 != 1 || Main.rand.NextBool(3))
                                        {
                                            Vector2 vector60 = projectile.position;
                                            Vector2 vector61 = projectile.oldVelocity;
                                            vector61.Normalize();
                                            vector61 *= 8f;
                                            float num752 = Main.rand.Next(-35, 36) * 0.01f;
                                            float num753 = Main.rand.Next(-35, 36) * 0.01f;
                                            vector60 -= vector61 * k;
                                            num752 += projectile.oldVelocity.X / 6f;
                                            num753 += projectile.oldVelocity.Y / 6f;
                                            int num754 = Projectile.NewProjectile(projectile.GetSource_FromAI(), vector60, new Vector2(num752, num753), Main.player[projectile.owner].beeType(), Main.player[projectile.owner].beeDamage(projectile.damage / 3), Main.player[projectile.owner].beeKB(0f), Main.myPlayer);
                                            Main.projectile[num754].penetrate = 2;
                                            Main.projectile[num754].hostile = true;
                                            Main.projectile[num754].timeLeft = 300 + QueenGrabProj.timeLeft;
                                        }
                                    }
                                }
                            }
                        }

                        projectile.velocity = projectile.velocity.RotatedBy(0.06f);

                        if (projectile.type != ProjectileID.MoonlordArrow || projectile.type != ProjectileID.PhantasmArrow || projectile.type != ProjectileID.ShadowFlameArrow)
                            projectile.velocity.Y -= 0.1f;
                        if (projectile.type ==  ProjectileID.ShimmerArrow)
                            projectile.velocity.Y += 0.2f;
                        

                        projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
                        specialProj = true;
                    }


                    if (projectile.type == 933)// 天顶剑弹幕
                    {
                        projectile.aiStyle = 0;
                        projectile.rotation += 0.06f;
                        specialProj = true;
                    }

                    if(projectile.type == 856) // 星星吉他，什么b武器
                    {
                        projectile.aiStyle = 0;
                        projectile.velocity = projectile.velocity.RotatedBy(0.06f);
                        projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
                        specialProj = true;
                    }


                    if (!specialProj)
                    {
                        if (OnCollide)// 字段只执行一次
                        {
                            OnCollide = false;
                            shootPos = projectile.position;
                            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
                            projectile.velocity *= 0.01f;
                        }
                        //Main.NewText(Main.GlobalTimeWrappedHourly / 50000f);
                        projectile.rotation += 0.01f;
                        if (projectile.aiStyle == ProjAIStyleID.Boomerang)// 回旋镖++
                            projectile.rotation += 0.05f;
                        if (projectile.aiStyle == ProjAIStyleID.TrueNightsEdge)// 永夜++
                            projectile.rotation += 0.05f;
                        if(projectile.type == 636)//破晓之光++
                        {
                            projectile.rotation += 0.01f;
                        }
                        //specialProj = true;
                    }

                    //Main.NewText(projectile.type);
                }
            }
            // 滞留结束，射向玩家
            if (QueenGrabProj.timeLeft <= 2)
            {
                if (isTileColideProj)
                {
                    projectile.tileCollide = true;
                }
                if (grabbed)
                {

                    //其他弹幕
                    if (projectile.type != 933)
                    {
                        projectile.velocity = Vector2.Normalize(Main.player[projectile.owner].Center - projectile.Center).RotatedByRandom(0.02f) * 8.8f;
                        projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;


                        // 永夜加速度
                        if (projectile.aiStyle == ProjAIStyleID.TrueNightsEdge || projectile.aiStyle == ProjAIStyleID.NightsEdge)
                        {
                            projectile.velocity *= 2.1f;
                        }
                    }
                    else
                    {
                        projectile.friendly = false;// 不再伤害敌怪，不然会秒杀外挂太强了
                        projectile.timeLeft = 240; // 减少寿命，不然会死的很爽
                    }
                    projectile.friendly = false;// 不再伤害敌怪（转身导致的）
                    projectile.hostile = true;
                }
            }

        }
        else // 不存在女王抓弹幕，或者说也处于抓完弹幕后，处理一些弹幕的运动
        {
            if (grabbed)
            {
                // 永夜加速度
                if (projectile.aiStyle == ProjAIStyleID.TrueNightsEdge || projectile.aiStyle == ProjAIStyleID.NightsEdge)
                {

                    projectile.rotation += 0.42f;
                }
                // 回旋镖当然没伤害
                if (projectile.aiStyle  == ProjAIStyleID.Boomerang)
                {
                    Vector2 playerDir = Main.player[projectile.owner].Center - projectile.Center;
                    projectile.velocity = Vector2.Normalize(playerDir) * 8.5f;
                    projectile.rotation += 0.07f;
                    projectile.hostile = false;
                    if (playerDir.Length() < 12f)
                        projectile.Kill();
                }

                if (projectile.arrow || projectile.type == 636) // 调整贴图旋转方向
                {
                    projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }

                if ((projectile.type == 933 || projectile.type == 856)&& projectile.aiStyle == 0) // 天顶剑
                {
                    Vector2 playerLine = Main.player[projectile.owner].Center - projectile.Center;
                    projectile.position += Vector2.Normalize(playerLine) * (projectile.type / 130f);// 我想怎么写就怎么写
                    //projectile.velocity = 0.0001f * projectile.position - projectile.oldPosition;
                    projectile.rotation += 0.06f;
                    if (playerLine.Length() < 6f)
                        projectile.Kill();

                }
            }
        }

    }

}
