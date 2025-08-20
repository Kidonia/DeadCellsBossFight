using DeadCellsBossFight.Projectiles.WeaponAnimationProj;
using DeadCellsBossFight.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;
using DeadCellsBossFight.Projectiles.BasicAnimationProj;
using System.Text;
using Terraria.UI.Chat;
using ReLogic.Graphics;
using DeadCellsBossFight.NPCs;

namespace DeadCellsBossFight.Utils;

public struct CustomVertexInfo : IVertexType
{
    private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[3]
    {
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0)
    });
    /// <summary>
    /// 绘制位置(世界坐标)
    /// </summary>
    public Vector2 Position;
    /// <summary>
    /// 绘制的颜色
    /// </summary>
    public Color Color;
    /// <summary>
    /// 前两个是纹理坐标，最后一个是自定义的
    /// </summary>
    public Vector3 TexCoord;

    public CustomVertexInfo(Vector2 position, Color color, Vector3 texCoord)
    {
        this.Position = position;
        this.Color = color;
        this.TexCoord = texCoord;
    }

    public VertexDeclaration VertexDeclaration => _vertexDeclaration;
}
public class Rope_Point
{
    public Vector2 pos, oldpos;
    public Rope_Point(Vector2 vector)
    {
        pos = vector;
        oldpos = vector;
    }

    public bool locked;
}
public class Rope_Line
{
    public Rope_Point startPoint, endPoint;
    public float Length;
    public Rope_Line(Rope_Point startPoint, Rope_Point endPoint, float length)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        this.Length = length;
    }
}

public class NormalUtils
{
    public static int Rand1or_1()
    {
        return Main.rand.Next(0, 2) * 2 - 1;
    }

    public static void ProjKillTree(Projectile projectile)
    {
        Point16 topLeft = projectile.TopLeft.ToTileCoordinates16();
        Point16 bottomRight = projectile.BottomRight.ToTileCoordinates16();
        for (int i = topLeft.X; i < bottomRight.X; i++)
            for (int j = topLeft.Y; j < bottomRight.Y; j++)
            {
                KillTrees(i, j);
            }
    }
    public static void KillTrees(int x, int y)
    {
        Tile tileAtPosition = CheckTileInPosition(x, y);
        if (tileAtPosition.HasTile && Main.tileAxe[tileAtPosition.TileType] && WorldGen.CanKillTile(x, y))
        {
            AchievementsHelper.CurrentlyMining = true;
            WorldGen.KillTile(x, y);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y);
            }
            AchievementsHelper.CurrentlyMining = false;
        }
    }

    public static Rectangle CreateRectangleFromVectors(Vector2 topLeft, Vector2 bottomRight)
    {
        int x = (int)topLeft.X;
        int y = (int)topLeft.Y;
        int width = (int)(bottomRight.X - topLeft.X);
        int height = (int)(bottomRight.Y - topLeft.Y);

        return new Rectangle(x, y, width, height);
    }
    public static Rectangle CreateRectangleFromVectors(Vector2 topLeft, int width, int height)
    {
        int x = (int)topLeft.X;
        int y = (int)topLeft.Y;

        return new Rectangle(x, y, width, height);
    }
    /// <summary>
    /// Main.screenPosition, Main.screenWidth, Main.screenHeight 的 Rectangle
    /// </summary>
    public static Rectangle screenRectangle = CreateRectangleFromVectors(Main.screenPosition, Main.screenWidth, Main.screenHeight);
    public static Rectangle Minus(Rectangle a, Rectangle b)
    {
        return new Rectangle(a.X - b.X, a.Y - b.Y, a.Width - b.Width, a.Height - b.Height);
    }
    public static Tile CheckTileInPosition(int x, int y)
    {
        if (!WorldGen.InWorld(x, y, 0))
        {
            return default;
        }
        return Main.tile[x, y];
    }
    public static int FindFirstProjectile(int Type)
    {
        for (int i = 0; i < Main.projectile.Length; i++)
        {
            if (Main.projectile[i].active && Main.projectile[i].type == Type)
                return i;
        }

        return -1;
    }

    public static float GetCorrectRadian(float minusRadian)
    {
        if (minusRadian < 0)
        {
            return (MathHelper.TwoPi + minusRadian) / MathHelper.TwoPi;
        }
        else
            return minusRadian / MathHelper.TwoPi;
    }

    /// <summary>
    /// 生成一个数组，其中每个数为0,1,2之一，且相邻两个互相不一样
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static int[] GenerateQNArray(int length)
    {
        int[] array = new int[length];

        for (int i = 0; i < array.Length; i++)
        {
            // 生成0, 1, 2的随机数
            int num = Main.rand.Next(3);
            // 确保当前数和前一个数不同
            while (i > 0 && num == array[i - 1])
            {
                num = Main.rand.Next(3);
            }
            array[i] = num;
        }

        return array;
    }



    /// <summary>
    /// 随机打乱数组
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    public static void Shuffle<T>(T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = Main.rand.Next(n + 1);
            T temp = array[k];
            array[k] = array[n];
            array[n] = temp;
        }
    }

    /// <summary>
    /// 随机打乱列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Main.rand.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static string InsertInEachRangePixel(string input, DynamicSpriteFont font)
    {
        StringBuilder result = new StringBuilder();
        int pixelCount = 0;

        for (int i = 0; i < input.Length; i++)
        {
            // 检查是否遇到 "..."
            if (input[i] == '.' && i + 2 < input.Length && input[i + 1] == '.' && input[i + 2] == '.')
            {
                pixelCount+=(int)(ChatManager.GetStringSize(font, ".", Vector2.One, 0f)).X * 3;
                result.Append("...");
                i += 2; // 跳过 ".."
            }
            else
            {
                pixelCount+= (int)(ChatManager.GetStringSize(font, input[i].ToString(), Vector2.One, 0f)).X;
                result.Append(input[i]);
            }

            if (pixelCount >= Main.screenWidth / 4 && (input[i] == ' ' || IsChineseChar(input[i])))
            {
                result.Append("\n");
                pixelCount = 0;
            }
        }

        return result.ToString();
    }

    public static string InsertInEach18Cn(string input)
    {
        StringBuilder result = new StringBuilder();
        int count = 0;

        for (int i = 0; i < input.Length; i++)
        {
            // 检查是否遇到 "..."
            if (input[i] == '.' && i + 2 < input.Length && input[i + 1] == '.' && input[i + 2] == '.')
            {
                count++;
                result.Append("...");
                i += 2; // 跳过 ".."
            }
            else if (IsChineseChar(input[i]))
            {
                count++;
                result.Append(input[i]);
            }
            else
            {
                result.Append(input[i]);
            }

            if (count >= 18)
            {
                result.Append("\n");
                count = 0;
            }
        }

        return result.ToString();
    }

    public static bool IsChineseChar(char c)
    {
        // 汉字的 Unicode 范围是 4E00-9FFF
        return c >= 0x4E00 && c <= 0x9FFF;
    }
    public static void EazyNewText(object o, string name, Color? color = null)
    {
        if (o is string)
        {
            Main.NewText(o, color);
            return;
        }
        Main.NewText($"{name}:{o}", color);
    }

    public static List<int> DrugBuffsToAdd = new List<int>()
    {
        BuffID.Poisoned,
        BuffID.OnFire,
        BuffID.Tipsy,
        BuffID.Bleeding,
        BuffID.Slow,
        BuffID.WellFed,
        BuffID.CursedInferno,
        BuffID.Frostburn,
        BuffID.Regeneration,
        BuffID.ObsidianSkin,
        BuffID.Ironskin,
        BuffID.ManaRegeneration,
        BuffID.Spelunker,
        BuffID.Chilled,
        BuffID.Frozen,
        BuffID.RapidHealing,
        BuffID.Burning,
        BuffID.Suffocation,
        BuffID.Venom,
        BuffID.ManaSickness,
        BuffID.Wet,
        BuffID.Dangersense,
        BuffID.Lifeforce,
        BuffID.Endurance,
        BuffID.Rage,
        BuffID.Inferno,
        BuffID.Wrath,
        BuffID.Stinky,
        BuffID.Electrified,
        BuffID.MoonLeech,
        BuffID.Rabies,
        BuffID.Stoned,
        BuffID.VortexDebuff,
        BuffID.NebulaUpDmg1,
        BuffID.NebulaUpDmg2,
        BuffID.NebulaUpDmg3,
        BuffID.NebulaUpLife1,
        BuffID.NebulaUpLife2,
        BuffID.NebulaUpLife3,
        BuffID.NebulaUpMana1,
        BuffID.NebulaUpMana2,
        BuffID.NebulaUpMana3,
        BuffID.WitheredArmor,
        BuffID.WitheredWeapon,
        BuffID.OgreSpit,
        BuffID.NoBuilding,
        BuffID.Dazed,
        BuffID.Panic,
        BuffID.Gills,
        BuffID.PotionSickness
    };

    /// <summary>
    /// 用于DCGlobalProjectile要被杀掉的宠物和召唤物，
    /// </summary>
    public static List<int> QNKillPetMinionIndexList = new();
    /// <summary>
    /// 炮塔后禁用那把武器
    /// </summary>
    public static List<int> QNForbiddenItemList = new();

    public static Dictionary<BHHesitateType, int> hesitate2proj = new Dictionary<BHHesitateType, int>
    {
        { BHHesitateType.None, ModContent.ProjectileType<IdleProj>() },
        { BHHesitateType.determined, ModContent.ProjectileType<DeterminedProj>() },

    };

    public static Dictionary<BHWeaponType, int> WeaponFirstAtkProjType = new Dictionary<BHWeaponType, int>()
    {
        { BHWeaponType.NoWeapon, ModContent.ProjectileType<StartSwordAtkA>() },
        { BHWeaponType.StartSword, ModContent.ProjectileType<StartSwordAtkA>() },
        { BHWeaponType.OilSword, ModContent.ProjectileType<OilSwordAtkA>() },
        { BHWeaponType.KingsSpear, ModContent.ProjectileType<KingsSpearAtkA>() },
        { BHWeaponType.BroadSword, ModContent.ProjectileType<BroadSwordAtkA>() },
        { BHWeaponType.AdeleScythe, ModContent.ProjectileType<AdeleScytheAtkA>() },
        { BHWeaponType.BleedCrit, ModContent.ProjectileType<BleedCritAtkA>() },
        { BHWeaponType.Bleeder, ModContent.ProjectileType<BleederAtkA>() },
        { BHWeaponType.DualDaggers, ModContent.ProjectileType<DualDaggersAtkA>() },
        { BHWeaponType.QueenRapier, ModContent.ProjectileType<QueenRapierAtkA>() },
        { BHWeaponType.TickScythe, ModContent.ProjectileType<TickScytheAtkB1>() },
        { BHWeaponType.PerfectHalberd, ModContent.ProjectileType<PerfectHalberdA>() },
        { BHWeaponType.HeavyAxe, ModContent.ProjectileType<HeavyAxeAtkA>() },
        { BHWeaponType.LowHealth, ModContent.ProjectileType<LowHealthAtkA>() },

    };


    public static Dictionary<BHWeaponType, int> WeaponNameItemType = new Dictionary<BHWeaponType, int>()
    {
        // ModContent.TryFind<ModItem>()
        /*
        { BHWeaponType.NoWeapon, ModContent.ItemType<StartSword>() },
        { BHWeaponType.StartSword, ModContent.ProjectileType<StartSwordAtkA>() },
        { BHWeaponType.OilSword, ModContent.ProjectileType<OilSwordAtkA>() },
        { BHWeaponType.KingsSpear, ModContent.ProjectileType<KingsSpearAtkA>() },
        { BHWeaponType.BroadSword, ModContent.ProjectileType<BroadSwordAtkA>() },
        { BHWeaponType.AdeleScythe, ModContent.ProjectileType<AdeleScytheAtkA>() },
        { BHWeaponType.BleedCrit, ModContent.ProjectileType<BleedCritAtkA>() },
        { BHWeaponType.Bleeder, ModContent.ProjectileType<BleederAtkA>() },
        { BHWeaponType.DualDaggers, ModContent.ProjectileType<DualDaggersAtkA>() },
        { BHWeaponType.QueenRapier, ModContent.ProjectileType<QueenRapierAtkA>() },
        { BHWeaponType.TickScythe, ModContent.ProjectileType<TickScytheAtkB1>() },
        { BHWeaponType.PerfectHalberd, ModContent.ProjectileType<PerfectHalberdA>() },
        { BHWeaponType.HeavyAxe, ModContent.ProjectileType<HeavyAxeAtkA>() },
        { BHWeaponType.LowHealth, ModContent.ProjectileType<LowHealthAtkA>() },
        */

    };


    /// <summary>
    /// 包含有所有攻击动作弹幕的列表
    /// </summary>
    public static List<int> modBHProjTypeList = new()
    {
        ModContent.ProjectileType<TestTW>(),
        ModContent.ProjectileType<AdeleScytheAtkA>(),
        ModContent.ProjectileType<AdeleScytheAtkB>(),
        ModContent.ProjectileType<AdeleScytheAtkC>(),
        ModContent.ProjectileType<AdeleScytheAtkD>(),
        ModContent.ProjectileType<BleedCritAtkA>(),
        ModContent.ProjectileType<BleedCritAtkB>(),
        ModContent.ProjectileType<BleedCritAtkC>(),
        ModContent.ProjectileType<BleederAtkA>(),
        ModContent.ProjectileType<BleederAtkB>(),
        ModContent.ProjectileType<BroadSwordAtkA>(),
        ModContent.ProjectileType<BroadSwordAtkB>(),
        ModContent.ProjectileType<BroadSwordAtkC>(),
        ModContent.ProjectileType<DualDaggersAtkA>(),
        ModContent.ProjectileType<DualDaggersAtkB>(),
        ModContent.ProjectileType<DualDaggersAtkC>(),
        ModContent.ProjectileType<HeavyAxeAtkA>(),
        ModContent.ProjectileType<HeavyAxeAtkB>(),
        ModContent.ProjectileType<HeavyAxeAtkC>(),
        ModContent.ProjectileType<HeavyAxeAtkD>(),
        ModContent.ProjectileType<KingsSpearAtkA>(),
        ModContent.ProjectileType<KingsSpearAtkB>(),
        ModContent.ProjectileType<KingsSpearAtkC>(),
        ModContent.ProjectileType<LowHealthAtkA>(),
        ModContent.ProjectileType<LowHealthAtkB>(),
        ModContent.ProjectileType<LowHealthAtkC>(),
        ModContent.ProjectileType<LowHealthAtkD>(),
        ModContent.ProjectileType<LowHealthAtkE>(),
        ModContent.ProjectileType<OilSwordAtkA>(),
        ModContent.ProjectileType<OilSwordAtkB>(),
        ModContent.ProjectileType<PerfectHalberdA>(),
        ModContent.ProjectileType<PerfectHalberdB>(),
        ModContent.ProjectileType<PerfectHalberdC>(),
        ModContent.ProjectileType<PerfectHalberdD>(),
        ModContent.ProjectileType<QueenRapierAtkA>(),
        ModContent.ProjectileType<QueenRapierAtkB>(),
        ModContent.ProjectileType<QueenRapierAtkC>(),
        ModContent.ProjectileType<QueenRapierCut>(),
        ModContent.ProjectileType<QueenRapierCritCut>(),
        ModContent.ProjectileType<StartSwordAtkA>(),
        ModContent.ProjectileType<StartSwordAtkB>(),
        ModContent.ProjectileType<StartSwordAtkC>(),
        ModContent.ProjectileType<TickScytheAtkA1>(),
        ModContent.ProjectileType<TickScytheAtkA2>(),
        ModContent.ProjectileType<TickScytheAtkB1>(),
        ModContent.ProjectileType<TickScytheAtkB2>()

    };
    public static List<int> modAnimProjList = new()
    {
        ModContent.ProjectileType<RollAProj>(),
        ModContent.ProjectileType<RollBProj>(),
        ModContent.ProjectileType<RollCProj>(),
        ModContent.ProjectileType<WalkProj>(),
        ModContent.ProjectileType<IdleProj>(),
        ModContent.ProjectileType<JumpUpProj>(),
        ModContent.ProjectileType<JumpDownProj>()
    };

    public static List<int> lightPetProj = new() { 18, 500, 492, 72, 86, 87, 702, 211, 650, 896, 891, 895 };
    

    

    /// <summary>
    /// 清除BH移动动画的弹幕
    /// </summary>
    public static void ClearMoveProjs()
    {
        foreach (Projectile projectile in Main.ActiveProjectiles)//清除移动弹幕
        {
            if (modAnimProjList.Contains(projectile.type))
                projectile.Kill();
        }
    }
}
