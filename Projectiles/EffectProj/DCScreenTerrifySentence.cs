using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using ReLogic.Graphics;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using DeadCellsBossFight.Core;
using Terraria.GameInput;
using DeadCellsBossFight.Contents.GlobalChanges;
using System.Collections.Generic;

namespace DeadCellsBossFight.Projectiles.EffectProj;

public class DCScreenTerrifySentence : ModProjectile
{
    //localAI[0]用于绘制第几条语句，ai[0]用于声音的断断续续

    private SlotId earRingSound;
    private SoundStyle tinnitus;

    private SlotId whisphere;

    private SoundStyle crypt_loop;

    private SoundStyle scream;



    private SoundStyle fireOut;




    public override string Texture => AssetsLoader.TransparentImg;
    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.friendly = true;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = 300;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;

        tinnitus = new SoundStyle("DeadCellsBossFight/Assets/Sounds/PlayerSounds/tinnitus");
        crypt_loop = new SoundStyle("DeadCellsBossFight/Assets/Music/crypt_loop");
        crypt_loop.Pitch = 0.4f;
        fireOut = new SoundStyle("DeadCellsBossFight/Assets/Sounds/InterSounds/fireBurnOut");
        fireOut.Volume = 1.21f;

        scream = new SoundStyle("DeadCellsBossFight/Assets/Sounds/PlayerSounds/scream");

        base.SetDefaults();
    }
    public override void OnSpawn(IEntitySource source)
    {
        var system = ModContent.GetInstance<DCWorldSystem>();
        system.PreventZoomChange = true;

        Main.blockInput = true;
        for (int i = 0; i < Main.projectile.Length; i++)
        {
            if (Main.projectile[i].active == true && Main.projectile[i].type == ModContent.ProjectileType<DCScreenDrug>())
                Projectile.active = false;
        }

        Main.LocalPlayer.wingTime = 0;
        Main.LocalPlayer.rocketTime = 0;
        Main.LocalPlayer.sandStorm = false;
        Main.LocalPlayer.dash = 0;
        Main.LocalPlayer.dashType = 0;
        Main.LocalPlayer.noKnockback = true;
        Main.LocalPlayer.RemoveAllGrapplingHooks();
        Main.LocalPlayer.StopVanityActions();
        Main.LocalPlayer.StopExtraJumpInProgress();

        if (Main.LocalPlayer.mount.Active)
            Main.LocalPlayer.mount.Dismount(Main.LocalPlayer);

        if (Main.LocalPlayer.pulley)
            Main.LocalPlayer.pulley = false;

        // Disable rain.
        Main.StopRain();
        for (int i = 0; i < Main.maxRain; i++)
            Main.rain[i].active = false;

        DeadCellsBossFight.EffectProj.Add(Projectile);
        base.OnSpawn(source);
    }
    public override void AI()
    {

        // 禁用小地图
        Main.mapStyle = 0;

        Main.LocalPlayer.velocity *= 0;

        Projectile.position = Main.LocalPlayer.Center;





        if (Projectile.timeLeft == 250)
        {
            earRingSound = SoundEngine.PlaySound(tinnitus);
            whisphere = SoundEngine.PlaySound(crypt_loop);

            /*
            if (SoundEngine.TryGetActiveSound(earSound, out ActiveSound result2))
                result2.Volume = 0f;
            */
        }
        if (SoundEngine.TryGetActiveSound(whisphere, out ActiveSound result1))
        {
            float volume = MathHelper.Clamp((250 - Projectile.timeLeft) / 250f * 0.9f, 0, 0.9f);
            if (volume > 0.6f) { volume = 1f; }
            result1.Volume = volume;
        }


        if(Projectile.timeLeft == 20)
        {
            SoundEngine.PlaySound(scream);
        }

        int k = (300 - Projectile.timeLeft) / 60;


        if (Projectile.ai[0] > 0)
        {
            Projectile.ai[0]--;
            Main.audioSystem.PauseAll();
                Main.hideUI = true;
            if (Projectile.ai[0] <= 0)
            {
                Projectile.ai[1] += Main.rand.Next(20 - k);
            }
        }
        if (Projectile.ai[1] > 0)
        {
            Projectile.ai[1]--;
            Main.audioSystem.ResumeAll();
            if (k < 4)
                Main.hideUI = false;
            if (Projectile.ai[1] <= 0)
            {
                Projectile.ai[0] += Main.rand.Next(20 + k * 3);
            }
        }
        if (Projectile.ai[0] == 0 && Projectile.ai[1] == 0)
            Projectile.ai[0] += Main.rand.Next(20 + k * 3);
        //Projectile.timeLeft = 10;

        if (Projectile.timeLeft == 70)
        {
            SoundEngine.PlaySound(fireOut);
            Main.hideUI = true;
        }


        if (Projectile.timeLeft % 5 == 0)
            Projectile.localAI[0]++;

        if (Projectile.localAI[0] > 50) Projectile.localAI[0] = 50;
        //Main.NewText(Projectile.localAI[0]);
    }
    public override void PostDraw(Color lightColor)
    {
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(0, 0, 0, 450 - (int)(Projectile.timeLeft * 1.5f)));
        //Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black);

        string[] sentence = {
            "药物依赖、同理心缺失、渴求暴力、疯狂、无尽的死亡。",
            "“你不配。”",
            "“我好痒，肚子好痛，我伸手去抓却抓得一手血......头很痛，视线也模糊了......好消沉，好痒......”",
            "你熟悉万灵药的传说吗？",
            "“但凡行为举止异常、或容貌身姿明显恶化者，当立即处以监禁。”",
            "什么？！出什么事了？！",
            "“国王的新挚友下令说要抓到一个......还要抓活的......”",
            "“为什么要下令让我们把村民们关进牢房里？这些可怜的人们需要的是治疗啊！”",
            "“若牢房空间不够，则使用露天牢房或密牢。决不可有任何疏忽放任。”",
            "“见鬼的，我还要拨给你多少钱你才能做好工作？立即把桥彻底封闭！这不是你一个人要掉脑袋的问题！”",

            "真是个不可救药的混蛋！",
        "疫病让这个王国变成这番模样，你也该明白我们决不能让任何东西离开这座岛。不过看起来和你是不能讲道理了。",
            "那是能够治愈所有疾病的灵药......",
            "我们一定要立即采取和平一点的手段才行！我已经和炼金师聊......”",
            "之后我会第一时间赶来和大家汇合，只等我的身体不适有所好转......和我的手臂消肿之后。",
            "“......可以用......我们的身体进行......你的实验......求求你......救救我们......”",
            "“不想......被感染......逃出去......再次见到你。我希望......",
            "“你这个可怜的蠢材，为什么还要继续这样的实验？",
            "这么多尸体......这么多条性命......",
        "我很抱歉，但我必须就此阻止这番愚行。还请理解，杀死你这件事对我来说没有任何愉悦可言。",

            "“国王骗了我们！杀了国王！杀了国王！”",
            "但容我僭越地说一句，即使是时间，也不能‘永远’关住疫病。”",
        "抱歉，亲爱的，我不能让你重新点亮灯塔。哪怕要我杀死自己的夫君和国王，我也一定会阻止你。",
            "你这个蠢货究竟做了什么......",
            "一直以来我都拼命想要拯救这个扭曲的王国......",
            "拯救这个国家......免受你这个混蛋的伤害啊！",
            "“巨人已经不再是......守卫的一分子。把他的尸体带到牢......",
            "“传令给你们属下的所有士兵，如有违反一律掉脑袋。",
            "......我连脑袋都没有，甚至都不知道自己是从哪里来的。",
        "我也希望能有不同的结局......诅咒这见鬼的疫病！",

        "真是可悲......",
            "“请注意，泰拉瑞亚的冒险者们：你们必须把向导巫毒娃娃扔进岩浆中献祭，才能召唤出血肉墙。”" ,
        "了结她。",
            "“你闻起来像是腐臭的尸体......还散发出血腥杀戮和漠视生命的味道......”",
            "“国王绑架然后烧死了我们的孩子们！",
            "救命啊！！",
            "我们不正是这样被命运所带领，在这名为生命的玄妙循环之中，要么适应、要么死亡？",
            "“......一律处以监禁并绞首至死。”",
            "“我不会让疫病触及我们的。我会保护你们......",
            "“为什么......牢里全都是无辜的......”",

            "献上祭品......为你的救赎祈祷吧。证明你的忠诚！",
            "话说回来了，我现在这副样子也不怎么样。又有什么资格去说别人呢？",
        "看起来......骰子已经掷出了吗......",
            "结果我还剩下什么选择呢？只有这堆童话里的虚假故事吗......",
        "而现在到来的是我亲爱的夫君，又一次费劲心力想要救他自己一个人的命。",
            "“疫病、瘟疫、黑暗病、绿之毒......你们给这场诅咒起再多名字，都只是在浪费时间！",
            "“这里是建筑师们的安息之地。”",
            "你是我们所有人的榜样......，究竟是什么驱使你造成了如此多的毁灭？",
            "等到这些瓶子全都亮起来，一定会是一番盛况吧。",
            "这个王国是不可能得救了......但至少我不会被困在这里。",
            ""
        };

        float[] fontSizes = { 1.3f, 0.65f, 0.5f, 0.88f, 0.55f, 0.7f, 0.56f, 0.56f, 0.5f, 0.48f,
                                        0.9f, 0.55f, 0.7f, 0.57f, 0.57f, 0.6f, 0.62f, 0.6f, 0.7f, 0.55f,
                                        0.72f, 0.55f, 0.6f, 0.7f, 0.68f, 0.73f, 0.55f, 0.5f, 0.61f, 0.66f,
                                        0.8f, 0.45f, 0.9f, 0.62f, 0.8f, 0.9f, 0.5f, 0.69f, 0.54f, 0.58f,
                                        0.78f, 0.6f, 0.7f, 0.6f, 0.58f, 0.59f, 0.6f, 0.64f, 0.62f, 0.6f};

        int[] offsetXs = { -640, -670, -60, -260, -860, 80, -740, -960, 200, -650,
                                    160, -740, -810, -100, -700, 10, -760, -320, 150, -452,
                                    260, -700, -340, 40, -550, -740, -510, 100, -520, -920,
                                    -260, -270, -120, -920, -200, 320, 40, -600, -700, 120,
                                    10, 30, -800, -50, -880, -920, -430, 30, 120, -950};

        int[] offsetYs = { -200, 160, 240, -340, -235, 160, 320, -10, -450, 205,
                                    -420, 28, 280, -135, -373, -70, 120, -295, 360, -485,
                                    60, 350, 270, -240, -410, -55, 480, -265, 60, 378,
                                   -130, 298, -255, -90, 400, -310, 3, 235, -265, 100,
                                    320, -102, -300, 130, 450, 90, 162, -36, 450, -130};

        Color[] colors = { Color.Red, Color.LightSalmon , Color.Bisque , Color.Cyan, Color.Salmon, Color.LightCoral, Color.Olive, Color.MediumVioletRed, Color.Peru, Color.Brown,
                                    Color.Maroon, Color.MediumAquamarine, Color.LightSkyBlue, Color.SandyBrown, Color.DarkSalmon, Color.DarkRed, Color.Crimson, Color.DarkOrange, Color.Firebrick, Color.MediumTurquoise,
                                    Color.OrangeRed, Color.IndianRed, Color.MediumOrchid, Color.DarkMagenta, Color.MediumVioletRed, Color.Crimson, Color.Chocolate, Color.Brown, Color.Red, Color.Crimson,
                                    Color.MediumVioletRed, new(114, 81, 56), Color.White, Color.Firebrick, Color.OrangeRed, Color.Red, Color.Red, Color.Peru, Color.DarkRed, Color.DarkSalmon,
                                    Color.DarkOrange, Color.Red, Color.Crimson, Color.Red, Color.MediumVioletRed, Color.Salmon, Color.BlueViolet, Color.OrangeRed, Color.SandyBrown, Color.Red};
        //DynamicSpriteFont[] fonts = { FontAssets.ItemStack.Value, FontAssets.MouseText.Value, FontAssets.CombatText[0].Value, FontAssets.CombatText[1].Value , FontAssets.DeathText.Value };

        //int index = (int)Projectile.localAI[0];
        //Vector2 middle = new Vector2(Main.screenWidth, Main.screenHeight) / 2;

        Vector2 middle = Main.ScreenSize.ToVector2() / 2;
        for (int i = 0; i < Projectile.localAI[0]; i++)
        {
            Vector2 off2 = Main.rand.NextVector2Circular((300 - Projectile.timeLeft) / 25, (300 - Projectile.timeLeft) / 25);
            if (Projectile.timeLeft < 20)
            {
                colors[i] = Color.Red;
                off2 = Main.rand.NextVector2Circular(50, 50);
            }
            Main.spriteBatch.DrawString(FontAssets.DeathText.Value, sentence[i], middle + new Vector2(offsetXs[i], offsetYs[i]) + off2, colors[i], 0, Vector2.Zero, fontSizes[i], SpriteEffects.None, 0);

        }
        //Main.spriteBatch.DrawString(FontAssets.DeathText.Value, sentence[index], new Vector2(Main.screenWidth * index / 50, Main.screenHeight * index / 50), new Color(250, 0, 0));


    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
    }

    public override void OnKill(int timeLeft)
    {
        Main.musicFade[Main.curMusic] = 0.02f;
        if (SoundEngine.TryGetActiveSound(whisphere, out ActiveSound result1))
            result1.Volume = 0f;

        if (SoundEngine.TryGetActiveSound(earRingSound, out ActiveSound result2))
            result2.Volume = 1f;


        // 保证伤害为0，不然没有效果
        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Main.LocalPlayer.Center, Vector2.Zero, ModContent.ProjectileType<DCScreenFire>(), 0, 0);
        base.OnKill(timeLeft);
    }
}
