using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using ReLogic.Content;
using ReLogic.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Core;

public class AssetsLoader
{
    public static string EnemySound = "DeadCellsBossFight/Assets/Sounds/EnemySounds/";

    public static string PlayerSound = "DeadCellsBossFight/Assets/Sounds/PlayerSounds/";

    public static string WeaponUseSound = "DeadCellsBossFight/Assets/Sounds/WeaponSounds/";

    public static string SkillUseSound = "DeadCellsBossFight/Assets/Sounds/SkillSounds/";

    public static string InterSound = "DeadCellsBossFight/Assets/Sounds/InterSounds/";

    public static string TransparentImg = "DeadCellsBossFight/Assets/Backgrounds/Empty";

    public static string WhiteDotImg = "DeadCellsBossFight/Assets/Backgrounds/WhiteDot";



    public static Texture2D BlackDot;
    public static Texture2D WhiteDot;
    public static Texture2D TransparentDot;
    public static Texture2D BHHead;
    public static Texture2D HeadFlesh;
    public static Texture2D HeadFlesh2;
    public static Texture2D HeadFlesh3;
    public static Texture2D Headline;
    public static Texture2D QueenHead;
    public static Texture2D QueenHead_Back;
    public static Texture2D BlackSmoke;
    public static Texture2D BlackSmokeTrail;
    public static Texture2D HeadSpark;
    public static Texture2D HugeSpark;
    public static Texture2D Shield1;
    public static Texture2D Shield2;
    public static Texture2D Shield3;
    public static Texture2D boxCollecorDialog;
    public static Texture2D boxMain;

    public static Texture2D skillBg;
    public static Texture2D skillSlotBi;
    public static Texture2D skillSlotBot;
    public static Texture2D skillSlotTop;
    public static Texture2D skillSlotFull;
    public static Texture2D skillSlotColorless;
    public static Texture2D skillSlotLegendary;
    public static Texture2D skillSlotBg;

    public static Texture2D tri;
    public static Texture2D roundtry;
    public static Texture2D roundtry2;
    public static Texture2D linetry;
    public static Texture2D linetry2;
    public static Texture2D PrisonChain;
    public static Texture2D fxGlowWhite;

    public static Texture2D fxBrutality;
    public static Texture2D fxFinesse;
    public static Texture2D fxSurvival;
    public static Texture2D[] HaloTexture = new Texture2D[3];

    public static Texture2D ENDBG;
    public static Texture2D ENDWINDBLOW;

    public static Texture2D nsSmokeMask;
    public static Texture2D nsPerlinNoise;
    public static Texture2D nsRGBNoise;
    public static Texture2D fxFireBullet0;
    public static Texture2D fxFireBullet1;
    public static Texture2D fxFireBullet2;
    public static Texture2D[] fxFireBullet = new Texture2D[3];
    public static Texture2D[] yamatoEnd = new Texture2D[6];

    public static Texture2D fxWeapon0;//武器攻击特效图0
    public static Texture2D fxWeapon1;//武器攻击特效图1

    public static Texture2D QNfxQueen;//女王攻击特效贴图

    public static Texture2D animPic0;//武器动作图0
    public static Texture2D animPic1;//武器动作图1
    public static Texture2D animPic0_glow;//武器发光部位图0
    public static Texture2D animPic1_glow;//武器发光部位图1
    public static Texture2D BHanimPic0;//细胞人武器动作图0
    public static Texture2D BHanimPic1;//细胞人武器动作图1
    public static Texture2D BHanimPic2;//细胞人武器动作图2
    public static Texture2D BHanimPic0_glow;//细胞人武器发光部位图0
    public static Texture2D BHanimPic1_glow;//细胞人武器发光部位图1
    public static Texture2D BHanimPic2_glow;//细胞人武器发光部位图2

    public static Texture2D QNanimPic0;//女王武器动作图0
    public static Texture2D QNanimPic1;//女王武器动作图1
    public static Texture2D QNanimPic0_glow;//女王发光部位图0
    public static Texture2D QNanimPic1_glow;//女王发光部位图1

    private static int picNum = 0;//用来判断是第几张图片

    public static Color[] ThreeSectColor =
    // 三种卷轴的颜色
    [
        new Color(236, 36, 36), // 红
        new Color(160, 81, 255), // 紫
        new Color(23, 206, 107), // 绿
        new Color(51, 69, 106) // UI里没有物品时UI的颜色
    ];

    public static Dictionary<string, Dictionary<int, DCAnimPic>> AnimAtlas = new();//玩家武器动作的总字典

    public static Dictionary<string, Dictionary<int, DCAnimPic>> fxAtlas = new();//细胞人武器特效的总字典
    public static Dictionary<string, Dictionary<int, DCAnimPic>> BHanimAtlas = new();//细胞人动作、特效的总字典

    public static Dictionary<string, Dictionary<int, DCAnimPic>> QNfxAtlas = new();//女王武器特效的总字典
    public static Dictionary<string, Dictionary<int, DCAnimPic>> QNanimAtlas = new();//女王动作、发光的总字典
    //NPC部分
    public static Texture2D fxEnemy0;//NPC攻击特效图0
    public static Texture2D fxEnemy1;//NPC攻击特效图0
    public static Dictionary<string, Dictionary<int, DCAnimPic>> fxEnmAtlas = new();//NPC特效的总字典

    public static string behemothPath = "Assets/NPCTextures/behemoth.atlas";
    public static Texture2D behemothTex;
    public static Texture2D behemothGlowTex;
    public static Dictionary<string, Dictionary<int, DCAnimPic>> behemothDic = new();


    public static string rockLauncherPath = "Assets/NPCTextures/rockLauncher.atlas";
    public static Texture2D rockLauncherTex;
    public static Texture2D rockLauncherGlowTex;
    public static Dictionary<string, Dictionary<int, DCAnimPic>> rockLauncherDic = new();



    /// <summary>
    ///    shader    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    public static Effect offsetEffect; // 空间扭曲效果 巨镰
    public static Effect glowEffect; // 颜色叠加效果
    public static Effect screenColorMess; // 传送时屏幕颜色错乱
    public static Effect waterWaveEffect; // 水面扭曲效果
    public static Effect Fire; // 屏幕大火焰细胞头绘制 
    public static Effect ScreenFault; // 屏幕方块碎裂+颜色错乱效果
    public static Effect drugEffect; // 屏幕扭曲，喝药头晕
    public static Effect RadialBlur; // 径向模糊
    public static Effect filter; // 滤镜
    public static Effect SkillUIColor; // 装备&技能的UI颜色添加

    /// <summary>
    ///    textures   /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>


    //数组长度为图片张数

    /*示例 加载一大组图片
    public static Texture2D[] Test = (Texture2D[])(object)new Texture2D[6];
    */


    /// <summary>
    ///    sounds    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    public static SoundStyle stun;
    public static SoundStyle roll;
    public static SoundStyle absorb;
    public static SoundStyle hurt;
    public static SoundStyle die;
    public static SoundStyle jump;
    public static SoundStyle stomp_air;
    public static SoundStyle stomp_char1;
    public static SoundStyle stomp_char2;
    public static SoundStyle curse_end;
    public static SoundStyle homunculus_ready;
    public static SoundStyle homunculus_release;
    public static SoundStyle homunculus_comeback;
    public static SoundStyle intro_slime_move1;
    public static SoundStyle intro_slime_move2;
    public static SoundStyle intro_slime_move3;
    public static SoundStyle intro_slime_land;

    public static SoundStyle hit_blade;
    public static SoundStyle hit_crit;
    public static SoundStyle hit_broadsword;
    public static SoundStyle hit_poison;

    public static SoundStyle weapon_axe_charge1;
    public static SoundStyle weapon_axe_charge2;
    public static SoundStyle weapon_axe_charge3;
    public static SoundStyle weapon_axe_charge4;
    public static SoundStyle weapon_axe_hit;
    public static SoundStyle weapon_axe_release1;
    public static SoundStyle weapon_axe_release2;
    public static SoundStyle weapon_axe_release3;
    public static SoundStyle weapon_axe_release4;


    public static SoundStyle weapon_saber_release1;
    public static SoundStyle weapon_saber_release2;
    public static SoundStyle weapon_dualdg_release1;
    public static SoundStyle weapon_dualdg_release2;
    public static SoundStyle weapon_dualdg_release3;
    public static SoundStyle weapon_dualdg_charge1;
    public static SoundStyle weapon_dualdg_charge2;
    public static SoundStyle weapon_dualdg_charge3;
    public static SoundStyle weapon_perfectsw_release1;
    public static SoundStyle weapon_perfectsw_release2;
    public static SoundStyle weapon_perfectsw_release3;
    public static SoundStyle weapon_perfectsw_release4;
    public static SoundStyle weapon_doublelance_release1;
    public static SoundStyle weapon_doublelance_release2;
    public static SoundStyle weapon_doublelance_release3;
    public static SoundStyle weapon_spear_charge1;
    public static SoundStyle weapon_stunmace_charge1;

    public static SoundStyle weapon_tickscythe_charge1;
    public static SoundStyle weapon_tickscythe_charge2;
    public static SoundStyle weapon_tickscythe_charge3;
    public static SoundStyle weapon_tickscythe_release1;
    public static SoundStyle weapon_broadsword_charge1;
    public static SoundStyle weapon_broadsword_charge2;
    public static SoundStyle weapon_broadsword_charge3;
    public static SoundStyle weapon_broadsword_release1;
    public static SoundStyle weapon_broadsword_release2;
    public static SoundStyle weapon_broadsword_release3;

    public static SoundStyle active_laceration;
    public static SoundStyle active_laceration_end;
    public static SoundStyle weapon_shortsw_release;
    public static SoundStyle weapon_kunai_release;

    public static SoundStyle weapon_queensw_release1;

    public static SoundStyle weapon_shield_block1;
    public static SoundStyle weapon_shield_block2;
    public static SoundStyle weapon_shield_block3;
    public static SoundStyle weapon_shield_charge;

    public static SoundStyle purpleDLC_scythe_AtkA_release;
    public static SoundStyle purpleDLC_scythe_AtkB_release;
    public static SoundStyle purpleDLC_scythe_AtkC_release;
    public static SoundStyle purpleDLC_scythe_AtkD_release;
    public static SoundStyle purpleDLC_scythe_charge;
    public static SoundStyle purpleDLC_scythe_hit;

    public static SoundStyle purpleDLC_scytheGhost_charge;
    public static SoundStyle purpleDLC_scytheGhost_explosion_hit;
    public static SoundStyle purpleDLC_scytheGhost_spawn;
    public static SoundStyle purpleDLC_scytheGhost_teleport_release;

    public static SoundStyle enm_bat_trigger;
    public static SoundStyle enm_fly_charge;
    public static SoundStyle enm_fly_fly;
    public static SoundStyle enm_fly_release;
    public static SoundStyle enm_zmb_die;

    public static SoundStyle door_break;
    public static SoundStyle unstable_platform_break;
    public static SoundStyle portal_full;
    public static SoundStyle portal_use1;
    public static SoundStyle portal_use2;

    public static SoundStyle slayAll;

    public static DynamicSpriteFont TryTextFont;
    public static DynamicSpriteFont CastleVaniaFont;

    /// <summary>
    ///  end  /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    public static void LoadAsset()
    {
        // TryTextFont = ModContent.Request<DynamicSpriteFont>("DeadCellsBossFight/Assets/Fonts/TryFont", AssetRequestMode.ImmediateLoad).Value;
        CastleVaniaFont = ModContent.Request<DynamicSpriteFont>("DeadCellsBossFight/Assets/Fonts/CastleVaniaFont-en", AssetRequestMode.ImmediateLoad).Value;

        AnimAtlas = Expand(AnimAtlas, "Assets/beheadedModHelper.atlas");
        fxAtlas = Expand(fxAtlas, "Assets/fxWeapon.atlas");
        fxEnmAtlas = Expand(fxEnmAtlas, "Assets/fxEnemy.atlas");
        BHanimAtlas = Expand(BHanimAtlas, "Assets/NPCTextures/beheaded.atlas");
        BHanimAtlas = AddJointPosToDic(BHanimAtlas, "Assets/NPCTextures/beheaded_tracks.json", "headBone");

        QNfxAtlas = Expand(QNfxAtlas, "Assets/fxQueen.atlas");
        QNanimAtlas = Expand(QNanimAtlas, "Assets/NPCTextures/Queen.atlas");
        QNanimAtlas = AddJointPosToDic(QNanimAtlas, "Assets/NPCTextures/Queen_tracks.json", "Bip001 Head");
        QNanimAtlas = AddJointPosToDic(QNanimAtlas, "Assets/NPCTextures/Queen_tracks.json", "Bip001 R Hand", isExtraJoint:true);

        offsetEffect = GetEffect("DeadCellsBossFight/Effects/Offset");
        glowEffect = GetEffect("DeadCellsBossFight/Effects/Glow");
        screenColorMess = GetEffect("DeadCellsBossFight/Effects/ScreenColorMess");
        waterWaveEffect = GetEffect("DeadCellsBossFight/Effects/Wave");
        Fire = GetEffect("DeadCellsBossFight/Effects/Fire");
        ScreenFault = GetEffect("DeadCellsBossFight/Effects/ScreenFault");
        drugEffect = GetEffect("DeadCellsBossFight/Effects/drug");
        RadialBlur = GetEffect("DeadCellsBossFight/Effects/RadialBlur");
        filter = GetEffect("DeadCellsBossFight/Effects/filter");
        SkillUIColor = GetEffect("DeadCellsBossFight/Effects/SkillUIColor");

        nsSmokeMask = GetTex("DeadCellsBossFight/Assets/Cool/smokeMask");
        nsPerlinNoise = GetTex("DeadCellsBossFight/Assets/Cool/perlinNoise");
        nsRGBNoise = GetTex("DeadCellsBossFight/Assets/Cool/rgbNoise");

        fxFireBullet0 = GetTex("DeadCellsBossFight/Assets/fxFireBullet0");
        fxFireBullet1 = GetTex("DeadCellsBossFight/Assets/fxFireBullet1");
        fxFireBullet2 = GetTex("DeadCellsBossFight/Assets/fxFireBullet2");
        fxFireBullet = [fxFireBullet0, fxFireBullet1, fxFireBullet2];
        for (int i = 1; i < 7; i++)
        {
            yamatoEnd[i-1] = GetTex("DeadCellsBossFight/Items/yamato" + i.ToString());
        }

        BlackDot = GetTex("DeadCellsBossFight/Assets/Backgrounds/BlackDot");
        WhiteDot = GetTex(WhiteDotImg);
        TransparentDot = GetTex(TransparentImg);
        BHHead = GetTex("DeadCellsBossFight/Assets/head");
        HeadFlesh = GetTex("DeadCellsBossFight/Assets/HeadFlesh");
        HeadFlesh2 = GetTex("DeadCellsBossFight/Assets/HeadFlesh2");
        HeadFlesh3 = GetTex("DeadCellsBossFight/Assets/HeadFlesh3");
        QueenHead = GetTex("DeadCellsBossFight/Assets/QueenHead");
        QueenHead_Back = GetTex("DeadCellsBossFight/Assets/QueenHead_Back");
        Headline = GetTex("DeadCellsBossFight/Assets/ceilTurretLink");
        BlackSmoke = GetTex("DeadCellsBossFight/Assets/fxDotLarge");
        BlackSmokeTrail = GetTex("DeadCellsBossFight/Assets/BlackSmokeTrail");
        HeadSpark = GetTex("DeadCellsBossFight/Assets/HeadSpark");
        HugeSpark = GetTex("DeadCellsBossFight/Assets/HugeSpark");
        Shield1 = GetTex("DeadCellsBossFight/Assets/fxNova");
        Shield2 = GetTex("DeadCellsBossFight/Assets/shieldBubble");
        Shield3 = GetTex("DeadCellsBossFight/Assets/fxShieldBubble");

        boxCollecorDialog = GetTex("DeadCellsBossFight/Assets/UIAssets/boxCollecorDialog");
        boxMain = GetTex("DeadCellsBossFight/Assets/UIAssets/boxMain");

        skillBg = GetTex("DeadCellsBossFight/Assets/UIAssets/skillBg");
        skillSlotBi = GetTex("DeadCellsBossFight/Assets/UIAssets/skillSlotBi");
        skillSlotBot = GetTex("DeadCellsBossFight/Assets/UIAssets/skillSlotBot");
        skillSlotTop = GetTex("DeadCellsBossFight/Assets/UIAssets/skillSlotTop");
        skillSlotFull = GetTex("DeadCellsBossFight/Assets/UIAssets/skillSlotFull");
        skillSlotColorless = GetTex("DeadCellsBossFight/Assets/UIAssets/skillSlotColorless");
        skillSlotLegendary = GetTex("DeadCellsBossFight/Assets/UIAssets/skillSlotLegendary");
        skillSlotBg = GetTex("DeadCellsBossFight/Assets/UIAssets/skillSlotBg");


        tri = GetTex("DeadCellsBossFight/Assets/tri");
        roundtry = GetTex("DeadCellsBossFight/Assets/roundtry");
        roundtry2 = GetTex("DeadCellsBossFight/Assets/roundtry2");
        linetry = GetTex("DeadCellsBossFight/Assets/linetry");
        linetry2 = GetTex("DeadCellsBossFight/Assets/linetry2");

        PrisonChain = GetTex("DeadCellsBossFight/Contents/Biomes/Prison/PrisonElements/PrisonChain");
        fxGlowWhite = GetTex("DeadCellsBossFight/Assets/Cool/fxGlowWhite");

        fxBrutality = GetTex("DeadCellsBossFight/Assets/fxBrutality");
        fxFinesse = GetTex("DeadCellsBossFight/Assets/fxFinesse");
        fxSurvival = GetTex("DeadCellsBossFight/Assets/fxSurvival");
        HaloTexture = [fxBrutality, fxFinesse, fxSurvival, TransparentDot];

        ENDBG = GetTex("DeadCellsBossFight/Assets/Backgrounds/ENDBG");
        ENDWINDBLOW = GetTex("DeadCellsBossFight/Assets/Backgrounds/ENDWINDBLOW");


        animPic0 = GetTex("DeadCellsBossFight/Assets/beheadedModHelper0");
        animPic1 = GetTex("DeadCellsBossFight/Assets/beheadedModHelper1");
        animPic0_glow = GetTex("DeadCellsBossFight/Assets/beheadedModHelper0_glow");
        animPic1_glow = GetTex("DeadCellsBossFight/Assets/beheadedModHelper1_glow");

        BHanimPic0 = GetTex("DeadCellsBossFight/Assets/NPCTextures/beheaded0");
        BHanimPic0_glow = GetTex("DeadCellsBossFight/Assets/NPCTextures/beheaded0_glow");
        BHanimPic1 = GetTex("DeadCellsBossFight/Assets/NPCTextures/beheaded1");
        BHanimPic1_glow = GetTex("DeadCellsBossFight/Assets/NPCTextures/beheaded1_glow");
        BHanimPic2 = GetTex("DeadCellsBossFight/Assets/NPCTextures/beheaded2");
        BHanimPic2_glow = GetTex("DeadCellsBossFight/Assets/NPCTextures/beheaded2_glow");

        QNfxQueen = GetTex("DeadCellsBossFight/Assets/fxQueen");

        QNanimPic0 = GetTex("DeadCellsBossFight/Assets/NPCTextures/Queen0");
        QNanimPic0_glow = GetTex("DeadCellsBossFight/Assets/NPCTextures/Queen0_glow");
        QNanimPic1 = GetTex("DeadCellsBossFight/Assets/NPCTextures/Queen1");
        QNanimPic1_glow = GetTex("DeadCellsBossFight/Assets/NPCTextures/Queen1_glow");

        fxWeapon0 = GetTex("DeadCellsBossFight/Assets/fxWeapon-0");
        fxWeapon1 = GetTex("DeadCellsBossFight/Assets/fxWeapon-1");

        fxEnemy0 = GetTex("DeadCellsBossFight/Assets/fxEnemy-0");
        fxEnemy1 = GetTex("DeadCellsBossFight/Assets/fxEnemy-1");

        //weapon
        /*示例 加载一大组图片
        LoadTextures(Test, 6, WeaponImagePath, "Test/atkSaberA_");
        */

        //NPC
        behemothDic = Expand(behemothDic, behemothPath);
        behemothTex = GetTex("DeadCellsBossFight/Assets/NPCTextures/behemoth0");
        behemothGlowTex = GetTex("DeadCellsBossFight/Assets/NPCTextures/behemoth0_glow");

        rockLauncherDic = Expand(rockLauncherDic, rockLauncherPath);
        rockLauncherTex = GetTex("DeadCellsBossFight/Assets/NPCTextures/rockLauncher");
        rockLauncherGlowTex = GetTex("DeadCellsBossFight/Assets/NPCTextures/rockLauncher_glow");
    }
    public static void LoadSound()
    {
        //player
        stun = LoadModSound(PlayerSound + "stun", 0.82f);
        roll = LoadModSound(PlayerSound + "roll", 0.65f, 0.18f);
        absorb = LoadModSound(PlayerSound + "absorb", 1.1f, 0.02f);
        hurt = LoadModSound(PlayerSound + "hurt", 0.72f, 0.16f);
        die = LoadModSound(PlayerSound + "die", 0.48f, 0.22f);
        jump = LoadModSound(PlayerSound + "jump", 0.48f, 0.23f);
        stomp_air = LoadModSound(PlayerSound + "stomp_air", 0.7f);
        stomp_char1 = LoadModSound(PlayerSound + "stomp_char1", 0.62f);
        stomp_char2 = LoadModSound(PlayerSound + "stomp_char2", 0.6f);
        curse_end = LoadModSound(PlayerSound + "curse_end", 0.6f);
        homunculus_ready = LoadModSound(PlayerSound + "homunculus_ready", 0.58f);
        homunculus_release = LoadModSound(PlayerSound + "homunculus_release", 0.62f);
        homunculus_comeback = LoadModSound(PlayerSound + "homunculus_comeback", 0.62f);
        intro_slime_move1 = LoadModSound(PlayerSound + "intro_slime_move1", 0.62f, 0.02f);
        intro_slime_move2 = LoadModSound(PlayerSound + "intro_slime_move2", 0.62f, 0.02f);
        intro_slime_move3 = LoadModSound(PlayerSound + "intro_slime_move3", 0.62f, 0.02f);
        intro_slime_land = LoadModSound(PlayerSound + "intro_slime_land", 0.64f);
        //weapon
        hit_crit = LoadModSound(WeaponUseSound + "hit_crit", 0.45f, 0.12f);
        hit_blade = LoadModSound(WeaponUseSound + "hit_blade", 0.56f);
        hit_broadsword = LoadModSound(WeaponUseSound + "hit_broadsword", 0.56f);
        hit_poison = LoadModSound(WeaponUseSound + "hit_poison", 0.5f, 0.12f);

        weapon_axe_charge1 = LoadModSound(WeaponUseSound + "weapon_axe_charge1", 0.68f, 0.26f);
        weapon_axe_charge2 = LoadModSound(WeaponUseSound + "weapon_axe_charge2", 0.7f, 0.26f);
        weapon_axe_charge3 = LoadModSound(WeaponUseSound + "weapon_axe_charge3", 0.64f, 0.26f);
        weapon_axe_charge4 = LoadModSound(WeaponUseSound + "weapon_axe_charge4", 0.64f, 0.26f);
        weapon_axe_hit = LoadModSound(WeaponUseSound + "weapon_axe_hit", 0.68f, 0.26f);
        weapon_axe_release1 = LoadModSound(WeaponUseSound + "weapon_axe_release1", 0.68f, 0.26f);
        weapon_axe_release2 = LoadModSound(WeaponUseSound + "weapon_axe_release2", 0.68f, 0.26f);
        weapon_axe_release3 = LoadModSound(WeaponUseSound + "weapon_axe_release3", 0.6f, 0.26f);
        weapon_axe_release4 = LoadModSound(WeaponUseSound + "weapon_axe_release4", 0.62f, 0.26f);


        weapon_saber_release1 = LoadModSound(WeaponUseSound + "weapon_saber_release1", 0.52f, 0.12f);
        weapon_saber_release2 = LoadModSound(WeaponUseSound + "weapon_saber_release2", 0.52f, 0.14f);

        weapon_dualdg_release1 = LoadModSound(WeaponUseSound + "weapon_dualdg_release1", 0.68f, 0.26f);
        weapon_dualdg_release2 = LoadModSound(WeaponUseSound + "weapon_dualdg_release2", 0.68f, 0.26f);
        weapon_dualdg_release3 = LoadModSound(WeaponUseSound + "weapon_dualdg_release3", 0.74f, 0.2f);
        weapon_dualdg_charge1 = LoadModSound(WeaponUseSound + "weapon_dualdg_charge1", 0.73f, 0.26f);
        weapon_dualdg_charge2 = LoadModSound(WeaponUseSound + "weapon_dualdg_charge2", 0.74f, 0.26f);
        weapon_dualdg_charge3 = LoadModSound(WeaponUseSound + "weapon_dualdg_charge3", 0.94f, 0.2f);

        weapon_broadsword_charge1 = LoadModSound(WeaponUseSound + "weapon_broadsword_charge1", 0.88f, 0.22f);
        weapon_broadsword_charge2 = LoadModSound(WeaponUseSound + "weapon_broadsword_charge2", 0.88f, 0.22f);
        weapon_broadsword_charge3 = LoadModSound(WeaponUseSound + "weapon_broadsword_charge3", 0.88f, 0.22f);
        weapon_broadsword_release1 = LoadModSound(WeaponUseSound + "weapon_broadsword_release1", 0.88f, 0.22f);
        weapon_broadsword_release2 = LoadModSound(WeaponUseSound + "weapon_broadsword_release2", 0.88f, 0.22f);
        weapon_broadsword_release3 = LoadModSound(WeaponUseSound + "weapon_broadsword_release3", 0.88f, 0.16f);

        weapon_doublelance_release1 = LoadModSound(WeaponUseSound + "weapon_doublelance_release1", 0.88f, 0.22f);
        weapon_doublelance_release2 = LoadModSound(WeaponUseSound + "weapon_doublelance_release2", 0.88f, 0.22f);
        weapon_doublelance_release3 = LoadModSound(WeaponUseSound + "weapon_doublelance_release3", 0.88f, 0.22f);

        weapon_spear_charge1 = LoadModSound(WeaponUseSound + "weapon_spear_charge1", 0.72f, 0.23f);
        weapon_stunmace_charge1 = LoadModSound(WeaponUseSound + "weapon_stunmace_charge1", 0.7f, 0.22f);

        weapon_perfectsw_release1 = LoadModSound(WeaponUseSound + "weapon_perfectsw_release1", 0.62f, 0.26f);
        weapon_perfectsw_release2 = LoadModSound(WeaponUseSound + "weapon_perfectsw_release2", 0.62f, 0.26f);
        weapon_perfectsw_release3 = LoadModSound(WeaponUseSound + "weapon_perfectsw_release3", 0.62f, 0.26f);
        weapon_perfectsw_release4 = LoadModSound(WeaponUseSound + "weapon_perfectsw_release4", 0.62f, 0.26f);

        weapon_shortsw_release = LoadModSound(WeaponUseSound + "weapon_shortsw_release", 0.88f, 0.17f);
        weapon_kunai_release = LoadModSound(WeaponUseSound + "weapon_kunai_release", 0.66f, 0.14f);
        weapon_queensw_release1 = LoadModSound(WeaponUseSound + "weapon_queensw_release1", 0.62f, 0.24f);
        weapon_tickscythe_charge1 = LoadModSound(WeaponUseSound + "weapon_tickscythe_charge1", 0.56f);
        weapon_tickscythe_charge2 = LoadModSound(WeaponUseSound + "weapon_tickscythe_charge2", 0.54f);
        weapon_tickscythe_charge3 = LoadModSound(WeaponUseSound + "weapon_tickscythe_charge3", 0.54f);
        weapon_tickscythe_release1 = LoadModSound(WeaponUseSound + "weapon_tickscythe_release1", 0.52f);

        purpleDLC_scythe_AtkA_release = LoadModSound(WeaponUseSound + "purpleDLC_scythe_AtkA_release", 0.54f);
        purpleDLC_scythe_AtkB_release = LoadModSound(WeaponUseSound + "purpleDLC_scythe_AtkB_release", 0.54f);
        purpleDLC_scythe_AtkC_release = LoadModSound(WeaponUseSound + "purpleDLC_scythe_AtkC_release", 0.48f);
        purpleDLC_scythe_AtkD_release = LoadModSound(WeaponUseSound + "purpleDLC_scythe_AtkD_release", 0.54f);
        purpleDLC_scythe_charge = LoadModSound(WeaponUseSound + "purpleDLC_scythe_charge", 0.66f);
        purpleDLC_scythe_hit = LoadModSound(WeaponUseSound + "purpleDLC_scythe_hit", 0.4f);

        purpleDLC_scytheGhost_charge = LoadModSound(EnemySound + "purpleDLC_scytheGhost_charge", 0.4f);
        purpleDLC_scytheGhost_explosion_hit = LoadModSound(EnemySound + "purpleDLC_scytheGhost_explosion_hit", 0.42f);
        purpleDLC_scytheGhost_spawn = LoadModSound(EnemySound + "purpleDLC_scytheGhost_spawn", 0.56f);
        purpleDLC_scytheGhost_teleport_release = LoadModSound(EnemySound + "purpleDLC_scytheGhost_teleport_release", 0.45f);
        //shield
        weapon_shield_block1 = LoadModSound(WeaponUseSound + "weapon_shield_block1", 0.6f);
        weapon_shield_block2 = LoadModSound(WeaponUseSound + "weapon_shield_block2", 0.6f);
        weapon_shield_block3 = LoadModSound(WeaponUseSound + "weapon_shield_block3", 0.6f);
        weapon_shield_charge = LoadModSound(WeaponUseSound + "weapon_shield_charge", 0.6f, 0.12f);

        //skill
        active_laceration = LoadModSound(SkillUseSound + "active_laceration", 0.82f);
        active_laceration_end = LoadModSound(SkillUseSound + "active_laceration_end", 0.27f);

        //inter
        door_break = LoadModSound(InterSound + "door_break", 0.52f, 0.15f);
        unstable_platform_break = LoadModSound(InterSound + "unstable_platform_break", 0.64f, 0.12f);
        portal_full = LoadModSound(InterSound + "portal_full", 0.64f);
        portal_use1 = LoadModSound(InterSound + "portal_use1", 0.62f);
        portal_use2 = LoadModSound(InterSound + "portal_use2", 0.58f);


        //enemy
        enm_bat_trigger = LoadModSound(EnemySound + "enm_bat_trigger", 0.6f);
        enm_fly_charge = LoadModSound(EnemySound + "enm_fly_charge", 0.6f);
        enm_fly_fly = LoadModSound(EnemySound + "enm_fly_fly", 0.6f);
        enm_fly_release = LoadModSound(EnemySound + "enm_fly_release", 0.6f);
        enm_zmb_die = LoadModSound(EnemySound + "enm_zmb_die", 0.7f);

        slayAll = LoadModSound("DeadCellsBossFight/Assets/Sounds/slayAll", 1f);
    }
    // 示例 加载一大组图片
    /*
    /// <summary>
    /// textures是上面定义数组的名称。
    /// totalFrame是总共有多少帧，与上面定义数组后面的数字一样。
    /// imgPath填最上面定义的前置文件夹名称。
    /// nextPath记得用引号""！里面是前置文件夹里面图片的名称
    /// </summary>
    /// <param name="textures"></param>
    /// <param name="totalFrame"></param>
    /// <param name="imgPath"></param>
    /// <param name="nextPath"></param>
    public static void LoadTextures(Texture2D[] textures, int totalFrame, string imgPath, string nextPath)
    {
        for (int j = 0; j < totalFrame; j++)//小于和数组一样的数
        {
            Texture2D[] loadTex = textures;//对上号
            int num = j;
            int stringnum = nextPath.Length;
            DefaultInterpolatedStringHandler Path = new(stringnum, 1);//左边是下面那行的字符串的""里面的字符总数，包括斜杠；右边不用管
            Path.AppendLiteral(nextPath);
            if (j < 10)//00到09的贴图
            {
                Path.AppendFormatted(0);
                Path.AppendFormatted(j);
                loadTex[num] = GetTex(imgPath + Path.ToStringAndClear());
            }
            else//10以及再往后的贴图
            {
                Path.AppendFormatted(j);
                loadTex[num] = GetTex(imgPath + Path.ToStringAndClear());
            }
        }
    }
    */
    
    public static Texture2D GetTex(string path)
    {
        return ModContent.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad).Value;
    }

    public static Effect GetEffect(string path)
    {
        return ModContent.Request<Effect>(path, AssetRequestMode.ImmediateLoad).Value;
    }


    /// <summary>
    /// path是声音的完整路径，volume是音量， pitchVariance是音调变调的范围
    /// </summary>
    /// <param name="path"></param>
    /// <param name="volume"></param>
    /// <param name="pitchVariance"></param>
    /// <returns></returns>
    public static SoundStyle LoadModSound(string path, float volume = 1f, float pitchVariance = 0f)
    {
        SoundStyle sound = new SoundStyle(path);
        sound.Volume = volume;
        sound.PitchVariance = pitchVariance;
        return sound;
    }


    /// <summary>
    /// 用于根据index选择正确的图片去切帧图。默认使用动作贴图，其他图片请标明。
    /// 0就是beheadedModHelper0。
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static Texture2D ChooseCorrectAnimPic(int index, bool fxWeapon = false, bool fxEnemy = false, bool BH = false, bool BHglow = false, bool QN = false, bool QNglow = false, bool player = false, bool playerGlow = false)
    {
        /*
        if (glow)
        {
            if (index == 0) return animPic0_glow;
            if (index == 1) return animPic1_glow;
        }
        */
        if (fxWeapon)
        {
            if (index == 0) return fxWeapon0;
            if (index == 1) return fxWeapon1;
        }

        if (fxEnemy)
        {
            if (index == 0) return fxEnemy0;
            if (index == 1) return fxEnemy1;
        }

        if (BH)
        {
            if (index == 0) return BHanimPic0;
            if (index == 1) return BHanimPic1;
            if (index == 2) return BHanimPic2;
        }

        if(player)
        {
            if (index == 0) return animPic0;
            if (index == 1) return animPic1;
        }

        if (QN)
        {
            if (index == 0) return QNanimPic0;
            if (index == 1) return QNanimPic1;
        }

        if(QNglow)
        {
            if (index == 0) return QNanimPic0_glow;
            if (index == 1) return QNanimPic1_glow;
        }

        if (BHglow)
        {
            if (index == 0) return BHanimPic0_glow;
            if (index == 1) return BHanimPic1_glow;
            if (index == 2) return BHanimPic2_glow;
        }

        if (playerGlow)
        {
            if (index == 0) return animPic0_glow;
            if (index == 1) return animPic1_glow;
        }

        if (index == 0) return BHanimPic0;
        if (index == 1) return BHanimPic1;
        if (index == 2) return BHanimPic2;

        Main.NewText("出错！");
        return BHanimPic0;
    }


    /// <summary>
    /// 解包特定资源使用。返回序号字典，键 为动作贴图的序号，值 为相关的数据。
    /// </summary>
    /// <param name="PicsDict"></param>
    /// <param name="inName">动作名称</param>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static Dictionary<int, DCAnimPic> ExpandMatch(Dictionary<int, DCAnimPic> PicsDict, string inName, string filePath)
    {
        PicsDict.Clear();
        picNum = 0;
        if (DeadCellsBossFight.Instance.FileExists(filePath))
        {
            //Stream stream = File.OpenRead("beheadedModHelper.atlas");
            Stream stream = DeadCellsBossFight.Instance.GetFileStream(filePath);

            using BinaryReader binaryReader = new BinaryReader(stream);
            new string(binaryReader.ReadChars(4));//读取前四个字母BATL
            while (true)//循环读取
            {
                string atlasName = ReadString(binaryReader);//读取图片的名字
                if (atlasName == "")//检测到文件末尾终止！！有问题！！
                    break;

                while (true)//循环读取数据
                {
                    DCAnimPic pic = new DCAnimPic();
                    pic.name = ReadString(binaryReader); //读取动作图名称，如 atkA_01

                    if (pic.name == "") //检测到空行时停止
                    {
                        picNum++;//图片张数加一。因为文件里只有更换图片才会在名称前加一个空行。
                        break; //跳出循环，读取图片名称
                    }
                    pic.index = binaryReader.ReadUInt16();//读两位，以下一样。
                    pic.x = binaryReader.ReadUInt16();
                    pic.y = binaryReader.ReadUInt16();
                    pic.width = binaryReader.ReadUInt16();
                    pic.height = binaryReader.ReadUInt16();
                    pic.offsetX = binaryReader.ReadUInt16();
                    pic.offsetY = binaryReader.ReadUInt16();
                    pic.originalWidth = binaryReader.ReadUInt16();
                    pic.originalHeight = binaryReader.ReadUInt16();

                    pic.index = picNum;//重新利用数据，匹配是哪张图片。

                    string[] array = pic.name.Split('/');
                    string finalName = array[^1]; //即 string finalName = array[array.Length - 1];

                    string prefix = finalName.Split("_")[0].Trim();//获取名称的前半部分。如 atkA_01 就是 atkA
                    if (prefix == inName)
                    {
                        string number = finalName.Split("_")[1].Trim();//获取名称的后半部分。如 atkA_01 就是 01
                        if (int.TryParse(number, out int key))//将字符串01换成整数1
                        {
                            PicsDict[key] = pic;//将名称匹配的pic存到字典中去
                        }
                    }
                }
            }
            binaryReader.Close();
            stream.Close();

            return PicsDict;
        }
        else return default;
    }


    /// <summary>
    ///  解包.atlas文件使用。返回总字典<名称，序号字典>。其中序号字典<序号，图片相关信息类>。
    ///  示例：解包 武器动画AnimAtlas =  Expand2(AnimAtlas, WeaponAnimPath);
    ///  示例：获取 dic = AnimAtlas["atkA"]
    /// </summary>
    /// <param name="atlasdic"></param>
    /// <param name="filePath"></param>
    /// <returns>返回总字典<名称，序号字典>，其中序号字典<序号，图片相关信息类>。</returns>
    public static Dictionary<string, Dictionary<int, DCAnimPic>> Expand(Dictionary<string, Dictionary<int, DCAnimPic>> atlasdic, string filePath)
    {
        atlasdic.Clear();
        picNum = 0;
        if (DeadCellsBossFight.Instance.FileExists(filePath))
        {
            //Stream stream = File.OpenRead("beheadedModHelper.atlas");
            Stream stream = DeadCellsBossFight.Instance.GetFileStream(filePath);

            using BinaryReader binaryReader = new BinaryReader(stream);
            new string(binaryReader.ReadChars(4));//读取前四个字母BATL

            while (true)//循环读取
            {
                string atlasName = ReadString(binaryReader);//读取图片的名字
                if (atlasName == "")//检测到文件末尾终止！！有问题！！
                    break;

                //Console.WriteLine(atlasName);
                //Console.WriteLine(picNum);
                while (true)//循环读取数据
                {
                    DCAnimPic pic = new();
                    pic.name = ReadString(binaryReader); //读取动作图名称，如 atkA_01

                    if (pic.name == "") //检测到空行时停止
                    {
                        picNum++;//图片张数加一。因为文件里只有更换图片才会在名称前加一个空行。
                        break; //跳出循环，读取图片名称
                    }
                    _ = binaryReader.ReadUInt16();//读两位，以下一样。
                    pic.x = binaryReader.ReadUInt16();
                    pic.y = binaryReader.ReadUInt16();
                    pic.width = binaryReader.ReadUInt16();
                    pic.height = binaryReader.ReadUInt16();
                    pic.offsetX = binaryReader.ReadUInt16();
                    pic.offsetY = binaryReader.ReadUInt16();
                    pic.originalWidth = binaryReader.ReadUInt16();
                    pic.originalHeight = binaryReader.ReadUInt16();

                    pic.index = picNum;//重新利用数据，匹配是哪张图片。

                    string[] array = pic.name.Split('/');
                    string finalName = array[^1]; //即 string finalName = array[array.Length - 1];

                    string prefix;
                    string number;

                    if (finalName.Contains('_'))
                    {
                        int lastIndex = finalName.LastIndexOf("_");
                        prefix = finalName[..lastIndex];//获取名称的前半部分。如 atkA_01 就是 atkA   即 finalName.Substring(0, lastIndex);
                        number = finalName[(lastIndex + 1)..];//获取名称的后半部分。如 atkA_01 就是 01   即 finalName.Substring(lastIndex + 1);
                    }
                    else if (finalName.Length > 2 && finalName.Any(c => char.IsLetter(c)))//NPC
                    {
                        prefix = finalName[..^2];//即 finalName.Substring(0, finalName.Length - 2);
                        number = finalName[^2..]; //即 finalName.Substring(finalName.Length - 2);
                    }
                    else
                    {
                        prefix = null;
                        number = null;
                    }
                    if (int.TryParse(number, out int key))
                    {
                        if (atlasdic.TryGetValue(prefix, out var innerDict))
                        {
                            innerDict.Add(key, pic);
                        }
                        else
                        {
                            atlasdic.Add(prefix, new Dictionary<int, DCAnimPic> { { key, pic } });
                        }
                    }


                }
            }
            binaryReader.Close();
            stream.Close();
        }
        return atlasdic;
    }


    private static string ReadString(BinaryReader _reader)//有问题
    {
        try
        {
            int num = _reader.ReadByte();
            if (num != 0)
            {
                return new string(_reader.ReadChars(num));
            }
            else
                return "";
        }
        catch
        {
            // 读取到流的末尾会报错※
            return "";
        }
    }

    /// <summary>
    /// 给解包过的.atlas文件得到的字典添加头的位置。返回总字典<名称，序号字典>。其中序号字典<序号，图片相关信息类>
    /// <param name="dic">已经得到的字典</param>
    /// <param name="filePath">_track.json文件路径</param>
    /// <param name="headName">头（关节）的名称</param>
    /// </summary>
    public static Dictionary<string, Dictionary<int, DCAnimPic>> AddJointPosToDic(Dictionary<string, Dictionary<int, DCAnimPic>> dic, string filePath, string headName, bool isExtraJoint = false)
    {
        Stream stream = DeadCellsBossFight.Instance.GetFileStream(filePath);
        StreamReader streamReader = new StreamReader(stream);
        // 读取JSON文件内容
        string jsonText = streamReader.ReadToEnd();

        // 解析JSON数据
        JObject jsonData = JObject.Parse(jsonText);

        // 遍历每个顶层属性，即动作的名称，如 "bulletStop"
        foreach (JProperty topLevelProperty in jsonData.Properties())
        {
            string key = topLevelProperty.Name;
            // 遍历每个子属性，包括各个关节的名称，如 "Bip001 Head"
            foreach (JProperty subProperty in topLevelProperty.Value.Children<JProperty>())
            {
                // 检测第二级名称是否是我们需要的头（关节）的名称
                if (subProperty.Name == headName)
                {
                    int[] infos = new int[3]; // 创建一个用于存储三个元素的数组
                    int count = 0;// 记录每三个数字为一组，进行更新
                    int group = 0;// 每经过三个数字，组数加一
                    foreach (var value in subProperty.Value)
                    {
                        infos[count % 3] = (int)value; // 将元素存入数组

                        if (count % 3 == 2)
                        {
                            // 跳过那些解包时就搞错了没加进字典的动作
                            if (dic.ContainsKey(key))
                            {
                                if (isExtraJoint)
                                {
                                    dic[key][group].extraJointPos[0] = infos[0];
                                    dic[key][group].extraJointPos[1] = infos[1];
                                    dic[key][group].extraJointPos[2] = infos[2];
                                }
                                else
                                {
                                    dic[key][group].headPos[0] = infos[0];
                                    dic[key][group].headPos[1] = infos[1];
                                    dic[key][group].headPos[2] = infos[2];
                                }
                            }
                            group++; //组数加一、
                            infos = new int[3]; // 重新创建一个用于存储三个元素的数组
                        }
                        count++;
                    }
                }
            }
        }
        stream.Close();
        return dic;
    }


    public static void UnloadAsset()
    {
        //shader
        offsetEffect = null;
        glowEffect = null;
        Fire = null;
        screenColorMess = null;
        waterWaveEffect = null;
        ScreenFault = null;
        drugEffect = null;
        RadialBlur = null;
        filter = null;
        SkillUIColor = null;

        BlackDot = null;
        WhiteDot = null;
        TransparentDot = null;
        BHHead = null;
        HeadFlesh = null;
        HeadFlesh2 = null;
        HeadFlesh3 = null;
        Headline = null;
        QueenHead = null;
        QueenHead_Back = null;
        BlackSmoke = null;
        BlackSmokeTrail = null;
        HeadSpark = null;
        HugeSpark = null;
        Shield1 = null;
        Shield2 = null;
        Shield3 = null;
        boxCollecorDialog = null;
        boxMain = null;
        tri = null;
        roundtry = null;
        roundtry2 = null;
        linetry = null;
        linetry2 = null;
        yamatoEnd = null;
        PrisonChain = null;
        fxGlowWhite = null;

        fxBrutality = null;
        fxFinesse = null;
        fxSurvival = null;

        ENDBG = null;
        ENDWINDBLOW = null;

        //animPic0 = null;
        //animPic0_glow = null;
        //animPic1_glow = null;
        BHanimPic0 = null;
        BHanimPic0_glow = null;
        BHanimPic1 = null;
        BHanimPic1_glow = null;
        BHanimPic2 = null;
        BHanimPic2_glow = null;
        QNanimPic0 = null;
        QNanimPic0_glow = null;
        QNanimPic1 = null;
        QNanimPic1_glow = null;
        fxWeapon0 = null;
        fxWeapon1 = null;
        fxEnemy0 = null;
        fxEnemy1 = null;
        QNfxQueen = null;
        //AnimAtlas = null;
        fxAtlas = null;
        fxEnmAtlas = null;
        BHanimAtlas = null;
        QNanimAtlas = null;
        /*示例 加载一大组图片
        Test = null;
        */

        EnemySound = null;
        PlayerSound = null;
        WeaponUseSound = null;
        SkillUseSound = null;
        InterSound = null;
        TransparentImg = null;
        WhiteDotImg = null;

        //NPC
        behemothDic = null;
        behemothTex = null;

        CastleVaniaFont = null;
    }

}
