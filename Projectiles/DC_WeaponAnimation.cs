using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.ModLoader;
using Terraria.ID;
using ReLogic.Utilities;
using System.Collections.Generic;
using System;
using DeadCellsBossFight.NPCs;
using DeadCellsBossFight.Core;
using DeadCellsBossFight.Contents.GlobalChanges;
using DeadCellsBossFight.Utils;

namespace DeadCellsBossFight.Projectiles;

public abstract class DC_WeaponAnimation : ModProjectile
{
    // Projectile.velocity.X 值为 1 和 -1，用于判断方向 （！！重要！！）
    // Boss的话估计不能用velocity.X，改用Projectile.direction.
    // ai[0]用来控制是否绘制细胞人移动的残影（不是自带的额外洋葱皮残影）0是不绘制，1是自动添加并绘制
    public override string Texture => AssetsLoader.TransparentImg;
    public Player player => Main.LocalPlayer;
    public DCPlayer playerHurt => player.GetModPlayer<DCPlayer>();
    public NPC npc;
    public Entity spawner;

    public SlotId WeaponChargeSound;

    private int slowdownbegin;//最开始哪些帧需要放慢播放速度
    private int slowdownend;//最后哪些帧需要放慢播放速度
    private bool canslow = true;
    private bool slow = false;

    private bool drawfx = false;//是否该绘制fx特效贴图了

    private Vector2 OnionPos = Vector2.Zero;

    /// <summary>
    /// 动作名称。见cdb
    /// </summary>
    public virtual string AnimName => "";

    /// <summary>
    /// fx特效贴图名称。见cdb
    /// </summary>
    public virtual string fxName => "";

    /// <summary>
    /// 第几帧造成判定（看cdb）
    /// </summary>
    public virtual int HitFrame => 1;

    /// <summary>
    /// fx特效贴图第几帧开始播放
    /// </summary>
    public virtual int fxStartFrame => 1;

    /// <summary>
    /// 总共有几张帧图，用dic.Count获取。
    /// </summary>
    public virtual int TotalFrame => 1;

    /// <summary>
    /// 总共有几张fx特效贴图，用dic.Count获取。
    /// </summary>
    public virtual int fxFrames => 1;

    /// <summary>
    /// 绘制一帧延迟的洋葱皮残影
    /// </summary>
    public virtual int OnionSkinFrame => -1;

    /// <summary>
    /// 残影的横向偏移
    /// </summary>
    public virtual float OnionSkinOffX => 0;

    /// <summary>
    /// ※重要※记得加上
    /// WeaponDic = AssetLoader.AnimAtlas[AnimName];
    /// fxDic = AssetLoader.fxAtlas[fxName];
    /// 碰撞箱宽、碰撞箱高、伤害(给自己提个醒)、伤害类型、击退大小、最开始哪些帧需要放慢播放速度（默认0）、最后哪些帧需要放慢播放速度（默认0）、能伤害敌人（默认能）、穿透数量（默认无限（即-1））、贴图大小（※默认放大3.5倍）、剩余时间（没啥用）、停止绘制（默认开启就对了）、撞墙不消失（默认开启）、无视水减速（默认开启）、一次攻击只能击中当前敌人一次（即-1）、监测能不能击中（默认监测，似乎没用）
    /// </summary>
    public void QuickSetDefault( int width, int height, int damage, DamageClass damageClass, float knockBack, int slowBeginFrame = 0, int slowEndFrame = 0, bool friendly = true, int penetrate = -1, float scale = 2.4f, int timeLeft = 2, bool hide = false, bool tileCollide = false, bool ignoreWater = true, int localNPCHitCooldown = -1, bool ownerHitCheck = true, bool hostile = true)
    {
        Projectile.width = width;
        Projectile.height = height;
        Projectile.damage = damage;
        Projectile.DamageType = damageClass;
        Projectile.knockBack = knockBack;
        slowdownbegin = slowBeginFrame;
        slowdownend = slowEndFrame;
        Projectile.friendly = friendly;
        Projectile.penetrate = penetrate;
        Projectile.scale = scale;
        Projectile.timeLeft = timeLeft;
        Projectile.hide = hide;
        Projectile.tileCollide = tileCollide;
        Projectile.ignoreWater = ignoreWater;
        Projectile.localNPCHitCooldown = localNPCHitCooldown;
        Projectile.ownerHitCheck = ownerHitCheck;
        Projectile.hostile = hostile;
    }

    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = TotalFrame;
    }
/*    public override void OnSpawn(IEntitySource source)
    {

    }*/
    public void SpawnSourceCheck(IEntitySource source, ref Dictionary<int, DCAnimPic> weaponDic)
    {
        if (source is EntitySource_Parent)
        {
            // 在这里先创建好一个EntitySource_Parent实例，方便后面调用
            EntitySource_Parent parentSource = source as EntitySource_Parent;
            // 如果生成这个source的是一个NPC
            // 而且这个NPC是一个Boss
            // parentSource.Entity as NPC 用于获取这个NPC，这样我们才能用boss这个字段
            if (parentSource.Entity is NPC)
            {
                Main.NewText("npc");

                GetBHNPC();
                spawner = npc;
                weaponDic = AssetsLoader.BHanimAtlas[AnimName];
                return;
            }
            if (parentSource.Entity is Player)
            {
                Main.NewText("player");
                spawner = player;
                weaponDic = AssetsLoader.AnimAtlas[AnimName];
                Projectile.hostile = false;
                return;
            }
            else
                GetBHNPC();
        }
    }
    public override bool? CanDamage()
    {
        return Projectile.frame == HitFrame && Projectile.frame < TotalFrame - 1;
    }


    /// <summary>
    /// posX,Yoffset是向右攻击时，碰撞箱中心向右偏移的X，向下偏移的Y值。(0, 0)表示碰撞箱中心在目标中心。写在AI()里。
    /// </summary>
    public void DrawTheAnimationInAI(float posXoffset, float posYoffset)
    {
        Projectile.direction = spawner.direction;
        if (slowdownbegin > 0 && canslow)
        {
            slowdownbegin--;
            slow = true;
            canslow = false;
        }
        if (Projectile.frame == TotalFrame - slowdownend - 1)
        {
            if (slowdownend == 0)
            {
                Projectile.Kill();
                return;
            }
            if (slowdownend > 0 && canslow)
            {
                slowdownend--;
                slow = true;
                canslow = false;
            }
        }
        if (slow)
            slow = false;
        else
        {
            Projectile.frame++;
            canslow = true;

            if (Projectile.frame >= fxStartFrame && Projectile.frameCounter < fxFrames)
            {
                Projectile.frameCounter++;
                drawfx = true;
            }
            else drawfx = false;
        }

        if (Projectile.frame == HitFrame)
        {
            var tilePos = Projectile.Hitbox.TopLeft().ToTileCoordinates16();
            for (int x = tilePos.X; x < tilePos.X + Projectile.width / 16 + 2; x++)
            {
                for (int y = tilePos.Y; y < tilePos.Y + Projectile.height / 16; y++)
                {
                    if (Main.tile.TryGet(x, y, out var tile) && tile.TileType == 10)
                    {
                        WorldGen.OpenDoor(x, y, (int)Projectile.velocity.X);
                        for (int i = 0; i < 20; i++)//破门粒子效果
                        {
                            int m = Dust.NewDust(new Point16(x, y + Main.rand.Next(-1, 2)).ToVector2() * 16, 8, 14, DustID.WoodFurniture, Projectile.direction * Main.rand.NextFloat(8f, 10f), Scale: Main.rand.NextFloat(0.8f, 1.2f));
                            Main.dust[m].alpha -= i * 6;
                        }
                        SoundEngine.PlaySound(AssetsLoader.door_break);
                    }
                }
            }
        }


        if (Projectile.frame - OnionSkinFrame == 0)//记录洋葱皮残影的位置，有问题
        {
            OnionPos = spawner.Center - Main.screenPosition;
        }

        Projectile.position = spawner.Center - Projectile.Size / 2 + new Vector2(Projectile.direction * posXoffset, posYoffset) * Projectile.scale;
        Projectile.timeLeft = 2;

        if (!spawner.active)
            Projectile.Kill();
    }




    /// <summary>
    /// 玩家使用武器时加的一点点移动速度。有点破坏游戏体验，建议重武可以试试加这个。
    /// </summary>
    /// <param name="bump"></param>
    public void Bump(float bump = 0)
    {
        if (Projectile.frame == HitFrame - 2 && Math.Abs(spawner.velocity.X) < 10.2f)
            spawner.velocity += new Vector2(bump * Projectile.direction, 0);
    }


    /// <summary>
    /// 如你所学。A是振幅，f是频率，正常剑类都是一，像巨镰能有个二三。time是持续几帧。慢慢调，挺乱的。 Vector2 direction是震动方向。不填默认射弹朝向。
    /// </summary>
    /// <param name="A">振幅</param>
    /// <param name="f">频率</param>
    /// <param name="time">时间</param>
    public void CameraBump(float A, float f, int time, Vector2 direction = default)
    {
        //default是Vector2.Zero，所以传参时用传的值，不传时用射弹朝向。
        direction = direction == Vector2.Zero ? new(Projectile.direction, 0) : direction;

        if (Projectile.frame == HitFrame)
        {
            var bump = new PunchCameraModifier(player.Center, direction, A, f, time);
            Main.instance.CameraModifiers.Add(bump);
        }
    }


    /// <summary>
    /// dic是武器贴图序号的字典。基本就填WeaponDic。
    /// drawStartOffsetX相当于玩家位置相对图片中点的横向偏移量，在图片中偏几像素就是几，左负右正。
    /// drawStartOffsetY相当于玩家位置相对图片中点的纵向偏移量，在图片中偏几像素就是几，上负下正。
    /// hasglow说明有发光颜色。默认否。
    /// glowColor是发光的颜色。见cdb。
    /// brighterGlow说明采用更为明亮的发光模式(Additive)。默认否。
    /// </summary>
    /// <param name="dic"></param>
    /// <param name="drawStartOffsetX"></param>
    /// <param name="drawStartOffsetY"></param>
    /// <param name="hasglow"></param>
    /// <param name="glowColor"></param>
    /// <param name="brighterGlow"></param>
    public void DrawWeaponTexture(Dictionary<int, DCAnimPic> dic, float drawStartOffsetX = 0, float drawStartOffsetY = 0, bool hasglow = false, Color glowColor = default, bool brighterGlow = false)
    {
        Projectile.direction = spawner.direction;

        SpriteBatch sb = Main.spriteBatch;
        Rectangle rectangle = new(dic[Projectile.frame].x, dic[Projectile.frame].y, dic[Projectile.frame].width, dic[Projectile.frame].height);
        Vector2 vector = new Vector2(dic[Projectile.frame].originalWidth / 2 * Projectile.direction //参考解包图片如果在大图里是如何绘制的
                                                        - dic[Projectile.frame].offsetX * Projectile.direction //
                                                        - (Projectile.direction - 1) * dic[Projectile.frame].width / 2,//+为0，-为width

                                                         dic[Projectile.frame].originalHeight / 2
                                                         - dic[Projectile.frame].offsetY)

            + new Vector2(Projectile.direction * drawStartOffsetX, drawStartOffsetY);


        //Main.NewText(Projectile.direction);
        bool useBHimg = spawner == npc;
        sb.Draw(AssetsLoader.ChooseCorrectAnimPic(dic[Projectile.frame].index, BH: useBHimg, player: !useBHimg),
            spawner.Center - Main.screenPosition,
            rectangle,
            Color.White,
            Projectile.rotation,
            vector,
            Projectile.scale,
            (SpriteEffects)(Projectile.direction > 0 ? 0 : 1),
            0f);


        if (OnionSkinFrame > 0 && Projectile.frame - OnionSkinFrame >= 0 && Projectile.frame - OnionSkinFrame <= 9)
        {
            Rectangle onionrect = new(dic[OnionSkinFrame].x, dic[OnionSkinFrame].y, dic[OnionSkinFrame].width, dic[OnionSkinFrame].height);
            Vector2 onionvect = new Vector2(dic[OnionSkinFrame].originalWidth / 2 * Projectile.direction //参考解包图片如果在大图里是如何绘制的
                                                            - dic[OnionSkinFrame].offsetX * Projectile.direction //
                                                            - (Projectile.direction - 1) * dic[OnionSkinFrame].width / 2,//+为0，-为width

                                                             dic[OnionSkinFrame].originalHeight / 2
                                                             - dic[OnionSkinFrame].offsetY)

                + new Vector2(Projectile.direction * (OnionSkinOffX + drawStartOffsetX), drawStartOffsetY);


            sb.Draw(AssetsLoader.ChooseCorrectAnimPic(dic[OnionSkinFrame].index, BH: useBHimg, player: !useBHimg),
            OnionPos,
            onionrect,
            new(255, 237, 19, 150 - 15 * (Projectile.frame - OnionSkinFrame)),
            Projectile.rotation,
            onionvect,
            Projectile.scale,
            (SpriteEffects)(Projectile.direction > 0 ? 0 : 1),
            0f);
        }

        if (useBHimg && Projectile.ai[0] == 1 && Main.GameUpdateCount % 2 == 0) // 绘制额外移动的连贯残影们
        {
            DCWorldSystem.onionSkinTrails.Add(new OnionSkinTrail(spawner.Center, Projectile.direction, Projectile.frame, Projectile.scale, dic, rectangle, vector));
        }


        if (hasglow)
        {
            sb.End();

            if (brighterGlow)
                sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
            else
                sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

            AssetsLoader.glowEffect.Parameters["InputR"].SetValue(glowColor.R / 255f);
            AssetsLoader.glowEffect.Parameters["InputG"].SetValue(glowColor.G / 255f);
            AssetsLoader.glowEffect.Parameters["InputB"].SetValue(glowColor.B / 255f);
            AssetsLoader.glowEffect.CurrentTechnique.Passes["Test"].Apply();

            sb.Draw(AssetsLoader.ChooseCorrectAnimPic(dic[Projectile.frame].index, BHglow : useBHimg, playerGlow: !useBHimg),
                spawner.Center - Main.screenPosition,
                rectangle,
                Color.White,
                Projectile.rotation,
                 vector,
                Projectile.scale,
                (SpriteEffects)(Projectile.direction > 0 ? 0 : 1),
                0f);
        }

        Vector2 vectorhead = new Vector2(dic[Projectile.frame].originalWidth / 2 * Projectile.direction //参考解包图片如果在大图里是如何绘制的
                                                - dic[Projectile.frame].headPos[0] * Projectile.direction //
                                                - (Projectile.direction - 1) * AssetsLoader.BHHead.Width / 2,//+为0，-为width

                                                 dic[Projectile.frame].originalHeight / 2
                                                 - dic[Projectile.frame].headPos[1])

    + new Vector2(Projectile.direction * (drawStartOffsetX + 6), (drawStartOffsetY + 12));


        //Main.NewText(Projectile.direction);


        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
        
        if(useBHimg)
            sb.Draw(AssetsLoader.BHHead,
                spawner.Center - Main.screenPosition,
                null,
                Color.White,
                Projectile.rotation,
                vectorhead,
                Projectile.scale,
                (SpriteEffects)(Projectile.direction > 0 ? 0 : 1),
                0f);
    }

    public void DrawHeadTexture(Dictionary<int, DCAnimPic> dic, float drawStartOffsetX = 0, float drawStartOffsetY = 0)
    {
        Projectile.direction = spawner.direction;

        SpriteBatch sb = Main.spriteBatch;
        Vector2 vectorhead = new Vector2(dic[Projectile.frame].originalWidth / 2 * Projectile.direction //参考解包图片如果在大图里是如何绘制的
                                                        - dic[Projectile.frame].headPos[0] * Projectile.direction //
                                                        - (Projectile.direction - 1) * AssetsLoader.BHHead.Width / 2,//+为0，-为width

                                                         dic[Projectile.frame].originalHeight / 2
                                                         - dic[Projectile.frame].headPos[1])

            + new Vector2(Projectile.direction * drawStartOffsetX, drawStartOffsetY);


        //Main.NewText(Projectile.direction);
        sb.Draw(AssetsLoader.BHHead,
            spawner.Center - Main.screenPosition,
            null,
            Color.White,
            Projectile.rotation,
            vectorhead,
            Projectile.scale,
            (SpriteEffects)(Projectile.direction > 0 ? 0 : 1),
            0f);

    }


    /// <summary>
    /// fxDic是武器贴图序号的字典。基本就填fxDic。
    /// drawStartOffsetX相当于玩家位置相对图片中点的横向偏移量，在图片中偏几像素就是几，左负右正。和DrawWeaponTexture()的一样。
    /// drawStartOffsetY相当于玩家位置相对图片中点的纵向偏移量，在图片中偏几像素就是几，上负下正。和DrawWeaponTexture()的一样。
    /// shader说明使用颜色叠加。默认否。
    /// fxWeaponColor是发光的颜色。见cdb。
    /// </summary>
    /// <param name="fxDic"></param>
    /// <param name="drawStartOffsetX"></param>
    /// <param name="drawStartOffsetY"></param>
    /// <param name="shader"></param>
    /// <param name="fxWeaponColor"></param>
    public void DrawfxTexture(Dictionary<int, DCAnimPic> fxDic, float drawStartOffsetX = 0, float drawStartOffsetY = 0, bool shader = false, Color fxWeaponColor = default)
    {
        if (drawfx)
        {
            SpriteBatch sb = Main.spriteBatch;
            Rectangle rectangle = new(fxDic[Projectile.frameCounter - 1].x, fxDic[Projectile.frameCounter - 1].y, 
                                                        fxDic[Projectile.frameCounter - 1].width, fxDic[Projectile.frameCounter - 1].height);
            Vector2 vector = new Vector2(fxDic[Projectile.frameCounter - 1].originalWidth / 2 * Projectile.direction //参考解包图片如果在大图里是如何绘制的
                                                            - fxDic[Projectile.frameCounter - 1].offsetX * Projectile.direction //
                                                            - (Projectile.direction - 1) * fxDic[Projectile.frameCounter - 1].width / 2,//+为0，-为width

                                                             fxDic[Projectile.frameCounter - 1].originalHeight / 2
                                                             - fxDic[Projectile.frameCounter - 1].offsetY)

                + new Vector2(Projectile.direction * drawStartOffsetX, drawStartOffsetY);



            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
            
            if (shader)
            {

                AssetsLoader.glowEffect.Parameters["InputR"].SetValue(fxWeaponColor.R / 255f);
                AssetsLoader.glowEffect.Parameters["InputG"].SetValue(fxWeaponColor.G / 255f);
                AssetsLoader.glowEffect.Parameters["InputB"].SetValue(fxWeaponColor.B / 255f);
                AssetsLoader.glowEffect.CurrentTechnique.Passes["fx"].Apply();
            }

            sb.Draw(AssetsLoader.ChooseCorrectAnimPic(fxDic[Projectile.frameCounter - 1].index, fxWeapon: true),
                    spawner.Center - Main.screenPosition,
                    rectangle,
                    Color.White,
                    Projectile.rotation,
                    vector,
                    Projectile.scale,
                    (SpriteEffects)(Projectile.direction > 0 ? 0 : 1),
                    0f);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);


        }
    }



    /// <summary>
    /// 使用武器前挥舞的声音。第0帧放送。写在AI()里，写在PlayWeaponSound()前。
    /// </summary>
    /// <param name="sound"></param>
    public void PlayChargeSound(SoundStyle sound)
    {
        if (Projectile.frame == 0)
            WeaponChargeSound = SoundEngine.PlaySound(sound, spawner.Center);

        if (!Projectile.active && SoundEngine.TryGetActiveSound(WeaponChargeSound, out ActiveSound result))
            result.Stop();
    }

    /// <summary>
    /// 使用武器的声音，在第几帧发出（默认0）。写在AI()里。
    /// </summary>
    /// <param name="sound"></param>
    /// <param name="frame"></param>
    public void PlayWeaponSound(SoundStyle sound, int frame = 0)
    {
        if (Projectile.frame == frame)
            SoundEngine.PlaySound(sound, spawner.Center);
    }



    /// <summary>
    /// 下次攻击暴击。
    /// </summary>
    public void ThisATKShouldCritSound()
    {
        if (playerHurt.ShouldPlayCritSound)
            Main.NewText("下次攻击已经要暴击!");

        playerHurt.ShouldPlayCritSound = true;
    }

    /// <summary>
    /// 因为是细胞人打的，所以不能判暴击，得手动加。一般暴击用这个就好。记得ModifierHitPlayer()要对应加上ThisATKShouldCritSound()。
    /// </summary>
    public void CheckAndPlayCritSound(Player.HurtInfo info)
    {
        if(playerHurt.ShouldPlayCritSound)
        {
            PlayCritSound(info);
            // SoundEngine.PlaySound(AssetsLoader.hit_crit);
            playerHurt.ShouldPlayCritSound = false;
        }
    }


    /// <summary>
    /// 因为是细胞人打的，所以不能判暴击，得手动加。写在OnHitPlayer里
    /// </summary>
    /// <param name="info"></param>
    public static void PlayCritSound(Player.HurtInfo info)
    {
        info.SoundDisabled = true;
        SoundEngine.PlaySound(AssetsLoader.hit_crit);
    }


    public override void AI()
    {
        if(spawner == npc || spawner == null)
            GetBHNPC();
    }
    public void GetBHNPC()
    {
        if(npc == null || !npc.active || npc.type != ModContent.NPCType<BH>())
        {
            foreach (NPC Npc in Main.npc)
            {
                if (Npc.type == ModContent.NPCType<BH>() && Npc.active)
                {
                    npc = Npc;
                    spawner ??= npc;
                    return;
                }
            }
            Main.NewText("细胞人已经死了...");
            Projectile.timeLeft = 0;
            Projectile.active = false;
        }
    }
}
