using System.Collections.Generic;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Events;
using Terraria.Utilities;
using Terraria.DataStructures;
using Terraria.Localization;
using DeadCellsBossFight.Items;
using DeadCellsBossFight.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;

namespace DeadCellsBossFight.NPCs;

public class Collector : ModNPC
{
    int npcID = NPCID.Wizard;
    public override void SetStaticDefaults()
    {

        //总帧数，根据使用贴图的实际帧数进行填写，这里我们直接调用全部商人的数据
        Main.npcFrameCount[Type] = Main.npcFrameCount[npcID];

        //特殊交互帧（如坐下，攻击）的数量，其作用就是规划这个NPC的最大行走帧数为多少，
        //最大行走帧数即Main.npcFrameCount - NPCID.Sets.ExtraFramesCount
        NPCID.Sets.ExtraFramesCount[Type] = NPCID.Sets.ExtraFramesCount[npcID];

        //攻击帧的数量，取决于你的NPC属于哪种攻击类型，如何填写见上文的分类讲解
        NPCID.Sets.AttackFrameCount[Type] = NPCID.Sets.AttackFrameCount[npcID];

        //NPC的攻击方式，同样取决于你的NPC属于哪种攻击类型，投掷型填0，远程型填1，魔法型填2，近战型填3，
        //如果是宠物没有攻击手段那么这条将不产生影响
        NPCID.Sets.AttackType[Type] = 2;

        //NPC的帽子位置中Y坐标的偏移量，这里特指派对帽，
        //当你觉得帽子戴的太高或太低时使用这个做调整（所以为什么不给个X的）         
        //NPCID.Sets.HatOffsetY[Type] = NPCID.Sets.HatOffsetY[npcID];

        //这个名字比较抽象，可以理解为 [记录了NPC的某些帧带来的身体起伏量的数组] 的索引值，
        //而这个数组的名字叫 NPCID.Sets.TownNPCsFramingGroups ，详情请在源码的NPCID.cs与Main.cs内进行搜索。
        //举个例子：你应该注意到了派对帽或是机械师背后的扳手在NPC走动时是会不断起伏的，靠的就是用这个进行调整，
        //所以说在画帧图时最好比着原版NPC的帧图进行绘制，方便各种数据调用
        //补充：这个属性似乎是针对城镇NPC的。
        NPCID.Sets.NPCFramingGroup[Type] = NPCID.Sets.NPCFramingGroup[npcID];

        //魔法型NPC在攻击时产生的魔法光环的颜色，如果NPCID.Sets.AttackType不为2那就不会产生光环
        //如果NPCID.Sets.AttackType为2那么默认为白色
        NPCID.Sets.MagicAuraColor[Type] = Color.LightBlue;

        //NPC的单次攻击持续时间，如果你的NPC需要持续施法进行攻击可以把这里设置的很长，
        //比如树妖的这个值就高达600
        //补充说明一点：如果你的NPC的AttackType为3即近战型，
        //这里最好选择套用，因为近战型NPC的单次攻击时间是固定的
        NPCID.Sets.AttackTime[Type] = NPCID.Sets.AttackTime[npcID];

        //NPC的危险检测范围，以像素为单位，这个似乎是半径
        NPCID.Sets.DangerDetectRange[Type] = 800;

        //让一个NPC在AI、动画等方面表现的像一个城镇NPC，但是并非一个标准城镇NPC，
        //如果你想要实现骷髅商人那种可以对话并交易，但是不会出现在入住菜单里、没有幸福度选项、且不会出现在小地图上的NPC，
        //可以在 NPC.townNPC 设置为 false 时将此选项设置为true。
        //若 NPC.townNPC 设置为 true。则这条将不再需要设置。
        NPCID.Sets.ActsLikeTownNPC[Type] = true;

        //该NPC生成时是否会顺便生成一个GivenName，
        //如果设置了false，那么该NPC生成时将会和普通NPC一样只显示DisplayName
        NPCID.Sets.SpawnsWithCustomName[Type] = true;

        //NPC在遭遇敌人时发动攻击的概率，如果为0则该NPC不会进行攻击（待验证）
        //遇到危险时，该NPC在可以进攻的情况下每帧有 1 / (NPCID.Sets.AttackAverageChance * 2) 的概率发动攻击
        //注：每帧都判定
        NPCID.Sets.AttackAverageChance[Type] = 10;

        //图鉴设置部分
        //将该NPC划定为城镇NPC分类
        NPCID.Sets.BossBestiaryPriority.Add(Type);
        NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new()
        {
            //为NPC设置图鉴展示状态，赋予其Velocity即可展现出行走姿态
            Velocity = 0.5f,
        };
        //添加信息至图鉴
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

        NPCID.Sets.NoTownNPCHappiness[Type] = true; // Prevents the happiness button

        /*        //设置对邻居和环境的喜恶，也就是幸福度设置
                //幸福度相关对话需要写在hjson里，见下文所讲
                NPC.Happiness
                    .SetBiomeAffection<JungleBiome>(AffectionLevel.Hate)//憎恶丛林环境
                    .SetBiomeAffection<UndergroundBiome>(AffectionLevel.Dislike)//讨厌地下环境
                    .SetBiomeAffection<SnowBiome>(AffectionLevel.Like)//喜欢雪地环境
                    .SetBiomeAffection<OceanBiome>(AffectionLevel.Love)//最爱海洋环境
                    .SetNPCAffection(NPCID.Angler, AffectionLevel.Dislike)//讨厌与渔夫做邻居
                    .SetNPCAffection(NPCID.Guide, AffectionLevel.Like)//喜欢与向导做邻居
                                                                      //邻居的喜好级别和环境的AffectionLevel是一样的
                ;*/
    }

    public override void SetDefaults()
    {
        //判断该NPC是否为城镇NPC，决定了这个NPC是否拥有幸福度对话，是否可以对话以及是否会被地图保存
        //当然以上这些属性也可以靠其他的方式开启或关闭，我们日后再说
        NPC.townNPC = false;


        //该NPC为友好NPC，不会被友方弹幕伤害且会被敌对NPC伤害
        NPC.friendly = true;
        //碰撞箱宽，不做过多解释，此处为标准城镇NPC数据
        NPC.width = 18;
        //碰撞箱高，不做过多解释，此处为标准城镇NPC数据
        NPC.height = 40;
        //套用原版城镇NPC的AIStyle，这样我们就不用自己费劲写AI了，
        //同时根据我以往的观测结果发现这个属性也决定了NPC是否会出现在入住列表里，还请大佬求证
        NPC.aiStyle = NPCAIStyleID.Passive;
        //伤害，由于城镇NPC没有体术所以这里特指弹幕伤害（虽然弹幕伤害也是单独设置的所以理论上这个可以不写？）
        NPC.damage = 666;
        //防御力
        NPC.defense = 66;
        //最大生命值，此处为标准城镇NPC数据
        NPC.lifeMax = 666666;
        //受击音效
        NPC.HitSound = SoundID.NPCHit1;
        //死亡音效
        NPC.DeathSound = SoundID.NPCDeath1;
        //抗击退性，数据越小抗性越高
        NPC.knockBackResist = 0.3f;
        //模仿的动画类型，这样就不用自己费劲写动画播放了
        AnimationType = npcID;
        TownNPCStayingHomeless = true;

        NPC.homeTileX = (int)DCWorldSystem.shimmerPosition.X / 16;
        NPC.homeTileY = (int)DCWorldSystem.shimmerPosition.Y / 16;
    }

    //设置姓名
    public override List<string> SetNPCNameList()
    {
        //所有可能出现的名字
        return new List<string>() {
        "Collector",
        "Robot",
        "AggressiveStomper",
        "Bird",
        "Mooner",
        "Le Collecteur",
        "#13eaff",
        "#42c7db",
        "Flasker"
        };


    }
    //设置对话按钮的文本
    public override void SetChatButtons(ref string button, ref string button2)
    {
        //直接引用原版的“商店”文本
        button = Language.GetTextValue("LegacyInterface.28");
        //设置第二个按钮
        button2 = "调查";
    }
    public override void OnChatButtonClicked(bool firstButton, ref string shopName)
    {
        if (firstButton)
        {
            shopName = "混乱与融合";
        }
        else
        {
            string a = "How pathetic...";
            DCWorldSystem.singleDialogueBox = new(a
                , NPC.Center - new Vector2(0, 200) - Main.screenPosition
                , AssetsLoader.boxCollecorDialog
                , 2
                , new(0, 0.8f, 1)
                , Color.White
                , AssetsLoader.CastleVaniaFont
                );
            Main.CloseNPCChatOrSign();
        }


        base.OnChatButtonClicked(firstButton, ref shopName);
    }

    public override void AddShops()
    {

        var npcShop = new NPCShop(Type, "混乱与融合")// chaos and mix
            .Add(new Item(3001) { shopCustomPrice = Item.buyPrice(silver: 2) })// 诡药
            .Add(new Item(227) { shopCustomPrice = Item.buyPrice(silver: 3) })// 恢复药

            .Add(new Item(313) { shopCustomPrice = Item.buyPrice(silver: 1) }, Condition.NotDownedKingSlime)// 太阳花
            .Add(new Item(314) { shopCustomPrice = Item.buyPrice(silver: 1) }, Condition.NotDownedQueenBee)// 月光草
            .Add(new Item(315) { shopCustomPrice = Item.buyPrice(silver: 1) }, Condition.NotDownedEyeOfCthulhu)// 闪耀根
            .Add(new Item(316) { shopCustomPrice = Item.buyPrice(silver: 1) }, Condition.NotDownedEowOrBoc)// 死亡草
            .Add(new Item(317) { shopCustomPrice = Item.buyPrice(silver: 1) }, Condition.NotDownedKingSlime)// 水叶草
            .Add(new Item(318) { shopCustomPrice = Item.buyPrice(silver: 1) }, Condition.NotDownedSkeletron)// 火焰花
            .Add(new Item(2358) { shopCustomPrice = Item.buyPrice(silver: 1)}, Condition.NotDownedGoblinArmy)// 寒颤棘

            .Add(new Item(295) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.DownedKingSlime)// 羽落药水
            .Add(new Item(302) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.DownedKingSlime)// 水上漂药水
            .Add(new Item(2327) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.DownedKingSlime)// 脚蹼药水
            .Add(new Item(2324) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.SmashedShadowOrb)// 镇静药水
            .Add(new Item(300) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.DownedEyeOfCthulhu)// 战斗药水
            .Add(new Item(304) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.DownedEyeOfCthulhu)// 狩猎药水
            .Add(new Item(2329) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.DownedEyeOfCthulhu)// 危险感药水
            .Add(new Item(289) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.DownedGoblinArmy)// 再生药水
            .Add(new Item(296) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.DownedGoblinArmy)// 洞穴探险药水
            .Add(new Item(2323) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.DownedEowOrBoc)// 拾心药水
            .Add(new Item(294) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.DownedEowOrBoc)// 魔能伤害药水
            .Add(new Item(291) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.DownedEowOrBoc)// 鱼鳃药水
            .Add(new Item(290) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.DownedQueenBee)// 敏捷药水
            .Add(new Item(2345) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.DownedQueenBee)// 生命力药水
            .Add(new Item(2326) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.DownedSkeletron)// 泰坦药水
            .Add(new Item(4870) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.Hardmode)// 返回药水
            .Add(new Item(5211) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.Hardmode)// 生物群系药水
            .Add(new Item(2351) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.Hardmode)// 随机传送药水
            .Add(new Item(2348) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.Hardmode)// 狱火药水
            .Add(new Item(288) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.Hardmode)// 黑曜石皮药水
            .Add(new Item(292) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.DownedMechBossAny)// 铁皮药水
            .Add(new Item(2328) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.DownedMechBossAny)// 召唤药水
            .Add(new Item(2347) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.DownedPirates)// 暴怒药水
            .Add(new Item(2349) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.DownedPirates)// 怒气药水
            .Add(new Item(2346) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.DownedMechBossAll)// 耐力药水
            .Add(new Item(2756) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.DownedClown)// 变性药水
            .Add(new Item(2359) { shopCustomPrice = Item.buyPrice(silver: 2) }, Condition.DownedDeerclops)// 保暖药水
            ;


        /*
        .Add(new Item(ModContent.ItemType<Items.Placeable.Furniture.ExampleWorkbench>()) { shopCustomPrice = Item.buyPrice(copper: 15) }) // This example sets a custom price, ExampleNPCShop.cs has more info on custom prices and currency. 
        .Add<Items.Placeable.Furniture.ExampleChair>()
        .Add<Items.Tools.ExampleHamaxe>()
        .Add<Items.Consumables.ExampleHealingPotion>(new Condition("Mods.ExampleMod.Conditions.PlayerHasLifeforceBuff", () => Main.LocalPlayer.HasBuff(BuffID.Lifeforce)))
        .Add<Items.Weapons.ExampleSword>(Condition.MoonPhasesQuarter0)
        //.Add<ExampleGun>(Condition.MoonPhasesQuarter1)
        .Add<Items.Ammo.ExampleBullet>(Condition.MoonPhasesQuarter1)
        .Add<Items.Weapons.ExampleStaff>(ExampleConditions.DownedMinionBoss)
        .Add<ExampleOnBuyItem>()
        .Add<Items.Weapons.ExampleYoyo>(Condition.IsNpcShimmered); // Let's sell an yoyo if this NPC is shimmered!
        */
        npcShop.Register(); // Name of this shop tab
        base.AddShops();
    }
    public override void ModifyActiveShop(string shopName, Item[] items)
    {
        if (Main.bloodMoon)
        {
            for (int i = 0; i < items.Length; i++)
            {
                // Skip 'air' items and null items.
                /*
                if (item == null || item.type == ItemID.None)
                    continue;
                else
                    item.type = ItemID.None;
                */

                items[i] = new Item(ModContent.ItemType<homo>());
                // If NPC is shimmered then reduce all prices by 50%.


                /*                if (NPC.IsShimmerVariant)
                                {
                                    int value = item.shopCustomPrice ?? item.value;
                                    item.shopCustomPrice = value / 2;
                                }*/

            }
        }
    }


    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
    }
    public int underwaterTime = 180;
    public override void AI()
    {
        Main.NewText(Main.npcChatFocus1);
        DCWorldSystem.collectorNPCidx = NPC.whoAmI;
        //Main.NewText(NPC.position);
        NPC.homeless = true;
        Lighting.AddLight(NPC.Center, 0, 0.8f, 1);
        if (NPC.shimmerWet)
        {
            if (underwaterTime > 0)
                underwaterTime--;
            else
            {
                NPC.position = DCWorldSystem.shimmerPosition;
                underwaterTime = 180;
            }
        }
        




        //杀死松露人
        if (Main.rand.NextBool(300))
        {
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == NPCID.Truffle)
                {
                    TruffleIdx = i;
                    break;
                }
            }
            if (TruffleIdx > 0)
            {
                TimeForKillTruffle = 150;
            }

        }

        if (TimeForKillTruffle > 0 && TruffleIdx > -1)
        {
            TimeForKillTruffle--;
            // 生成传送蓝光particle
            if (Main.GameUpdateCount % 12 == 0)
                for (int i = 0; i < 8; i++)
                {
                    ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings
                    {
                        PositionInWorld = NPC.Center + new Vector2(Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-12, 36)),
                        MovementVector = new Vector2(Main.rand.NextFloat(-1, 1), -Main.rand.NextFloat(2, 5))
                    });
                }
            if (TimeForKillTruffle == 0)
            {
                NPC.Teleport(Main.npc[TruffleIdx].position, 1);
                Main.npc[TruffleIdx].StrikeInstantKill();
                TruffleIdx = -1;
                TimeForKillTruffle = -240;
            }

        }
        else if (TimeForKillTruffle < 0)
        {
            TimeForKillTruffle++;
            if(TimeForKillTruffle > -120)
            {
                if (Main.GameUpdateCount % 12 == 0)
                    for (int i = 0; i < 8; i++)
                    {
                        ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings
                        {
                            PositionInWorld = NPC.Center + new Vector2(Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-12, 36)),
                            MovementVector = new Vector2(Main.rand.NextFloat(-1, 1), -Main.rand.NextFloat(2, 5))
                        });
                    }
            }
            if(TimeForKillTruffle == 0)
            {
                NPC.Teleport(DCWorldSystem.shimmerPosition, 1);

            }
        }
        //Main.NewText(NPC.shimmerWet);
    }
    public int TruffleIdx = -1;
    public int TimeForKillTruffle = 0;


    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        //Main.NewText(NPC.frame);
        // 攻击的两帧



        Rectangle rectangle = new Rectangle(0, 1344 + 64 * ((int)Main.GameUpdateCount % 14 / 7), 40, 64);
        spriteBatch.Draw(TextureAssets.Npc[Type].Value, NPC.position + new Vector2(-100, 100) - screenPos, rectangle, Color.White);
        base.PostDraw(spriteBatch, screenPos, drawColor);
    }

    //设置NPC的攻击力
    public override void TownNPCAttackStrength(ref int damage, ref float knockback)
    {
        //伤害，直接调用NPC本体的伤害
        damage = NPC.damage;
        //击退力，中规中矩的数据
        knockback = 8f;
    }
    //设置每次攻击完毕后的冷却时间
    public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
    {
        //基础冷却时间
        cooldown = 60;
        //额外冷却时间
        randExtraCooldown = 15;
    }
    //设置发射的弹幕
    public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
    {
        //射弹种类，这里用火枪子弹的弹幕
        projType = 253;
        //弹幕发射延迟，最好只给魔法型NPC设置较高数据
        attackDelay = 10;
    }
    //设置发射弹幕的向量
    public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
    {
        //发射速度
        multiplier = 8f;
        //射击角度额外向上偏移的量
        gravityCorrection = 0f;
        //射击时产生的最大额外向量偏差
        randomOffset = 0.2f;
    }
    //设置NPC施法时魔法光环的亮度
    public override void TownNPCAttackMagic(ref float auraLightMultiplier)
    {
        //光亮程度
        auraLightMultiplier = 0.9f;
    }

    public override bool CanGoToStatue(bool toKingStatue)
    {
        return true;
    }
    //当NPC在被雕像传送时会发生什么
    public override void OnGoToStatue(bool toKingStatue)
    {
        base.OnGoToStatue (toKingStatue);
    }

    //NPCID.Sets.ActsLikeTownNPC不会自动设置这里
    //仍然需要手动设置为true
    public override bool CanChat()
    {
        return true;
    }
    //设置对话
    public override string GetChat()
    {
        //声明一个int类型变量，查找一个whoAmI最靠前的、种类为向导的NPC并返回他的whoAmI
        /*
        int guide = NPC.FindFirstNPC(NPCID.Guide);
        int MushroomIdx = NPC.FindFirstNPC(NPCID.Truffle);*/

        WeightedRandom<string> chat = new WeightedRandom<string>();
        {

            //当血月和日食都没有发生时
            if (Main.hardMode) 
            {
                chat.Add("这个世界似乎在苦苦支撑着被打破的平衡...");
                chat.Add("我有着从污秽生灵提取纯洁精华的技术...但在这个世界里哪怕灵魂似乎都受到了诅咒般的污染");
                chat.Add("我过去提取生命精华的技术在最近被唤醒的生灵身上有了用武之地...这真的很有趣...");
                chat.Add("我能感受到这块大地的愤怒...这块大陆的重重庇护与诅咒都在被不断打破与重构...");
            }

            if (!Main.bloodMoon && !Main.eclipse)
            {
                //无家可归时
                if (NPC.homeless)
                {
                    chat.Add("呵呵...你肯定就是这个世界的...冒险者吧...");
                    chat.Add("我已经在这个世界做了一些实验...然而这个世界比我想象的更加深奥...也许问题的答案就在开头");
                    chat.Add("我的赖以生存之物在这个世界杳无音信...真是令人失望...或许不久后我便也无从查证了吧");
                    chat.Add("注意这片水域...它有着超脱这个世界之外的魔力...它不属于这个世界的任何一部分...却能够对这个世界的物质产生巨大的作用");
                    chat.Add("我来自...一个蕴育强大魔力与诅咒的岛屿...但眼下这个世界似乎有着更为强烈的反应...");
                    chat.Add("这个混乱又原始的世界维持着微妙的平衡...也许更为强大的对立都能被其包容");
                    chat.Add("与这里不同，我的国家有着一位...药物依赖、同理心缺失、渴求暴力、疯狂、无尽的死亡......当然，我不是在说你");
                    chat.Add("我在一片混乱与撕扯中来到此处...我已对这个世界展开调查...但层层拨开问题又将我领回此处");


                    chat.Add("我感受到了...你身上那纯粹的麻木，或许你没有半点称得上恐惧或自责的情感。");
                    chat.Add("这里的生物有着令人着迷的生物强度......配得上它们愚笨的行为");
                    chat.Add("请别害怕...我对你这渺小而愚钝的身体毫无兴趣");
                    chat.Add("可惜我不能给你提供一些...实验性的物品...我的研究在你的世界毫无出路");
                    chat.Add("这个充满生机与诅咒的世界...似乎隐藏着我要的答案");



                    chat.Add("这片水域...蕴含着强有力的魔力...但它似乎对我有着厌恶般的排斥...");
                    chat.Add("就算这片水域如此排斥我...我身上的每颗细胞都从中汲取到了......");
                    chat.Add("我已经观测到足够多有趣的现象...但我却始终无法将其应用在...没什么...");
                    chat.Add("我采取了一些激进的实验...现在我似乎在这充满迷幻的水中窥探到了星系的影子");

                    chat.Add("如果你身上的每颗细胞都被更新掉...你还是你自己吗?");
                    chat.Add("我的岛上也有着神明的存在...但祂似乎更容易被忤逆");



                    chat.Add("星系的连接...所谓星座...不过是弦论的古代意志体现");
                    chat.Add("水域映照星系...放大了不稳定性对时空裂痕的影响...便将我带至此处");
                    chat.Add("这片水域...它既是带我来到此处的原因...也是我要寻找的答案...只可惜...");
                    
                }
                else
                {
                    //自我介绍，NPC.FullName就是带上称呼的姓名，比如“测试小伙Den-o”

                    //当查找到向导NPC时
/*                    if (guide != -1)
                    {
                        //GivenName上面有提
                        chat.Add($"{Main.npc[guide].GivenName}博学多识，是个人才。");
                    }*/
                    //正在举行派对时
                    if (BirthdayParty.PartyIsUp)
                    {
                        chat.Add("我感到一种热烈的气氛...一种下水道里的老鼠们永远不会融入的气氛。");
                    }
                }
            }
            //日食时
            if (Main.eclipse)
            {

                chat.Add("这片水域...对你们所谓神明投下之阴影并无反应...有趣...真是有趣");
                chat.Add("所见皆为黑暗...故汝无处可逃");
            }
            //血月时
            if (Main.bloodMoon)
            {
                chat.Add("区区血色暴动之夜...我对其早已...绰绰有余...");
                chat.Add("我闻了...一股熟悉的...充满暴动的腥臭");

                chat.Add("疫病...月光......不......");
                chat.Add("即使深藏地底...我仍感到...月光对我身心的烧灼...");
                chat.Add("滚开！！！");
            }
            return chat;
        }
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        //当 玩家处于微光生成

        if (spawnInfo.Player.ZoneShimmer)
        {
            foreach (NPC npc in Main.ActiveNPCs)
                if (npc.type == ModContent.NPCType<Collector>())
                    return 0f;

            return 1f;
        }

        return 0f;
    }
    public override bool CheckActive()
    {
        return false;
    }

    public override bool CheckDead()
    {
        //写喝药AI，不会死
        return false;
    }

    public override bool UsesPartyHat()
    {
        return false;
    }
    public override bool NeedSaving()
    {
        return false;
    }
    public override bool CanHitNPC(NPC target)
    {
        return true;
    }
    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
    {
        return false;
    }
    public override void ChatBubblePosition(ref Vector2 position, ref SpriteEffects spriteEffects)
    {
        position = new Vector2(-200, -200);
    }
    public override void EmoteBubblePosition(ref Vector2 position, ref SpriteEffects spriteEffects)
    {
        position = new Vector2(-200, -200);
    }
}
