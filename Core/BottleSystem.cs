using DeadCellsBossFight.Utils;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using SubworldLibrary;
using DeadCellsBossFight.Contents.SubWorlds;
using DeadCellsBossFight.Items;
using System.Linq;

namespace DeadCellsBossFight.Core;

public class BottleSystem : ModSystem
{
    public static bool ActivateBottleSystem = false;
    public static int AddBottle = 0; ////////////////////////////////////////////////////要更新
    public List<Bottle> bottles = new List<Bottle>();
    public List<Vector2> allBottleBottomPos = new List<Vector2>() { new Vector2(720, 780) };
    // public List<VerletSegment> testSegments = new List<VerletSegment>();
    public static Dictionary<BottleItemLable, Texture2D> bottleTexA = new Dictionary<BottleItemLable, Texture2D>();
    public static Dictionary<BottleItemLable, Texture2D> bottleTexB = new Dictionary<BottleItemLable, Texture2D>();
    public static Dictionary<BottleItemLable, Texture2D> bottleTexC = new Dictionary<BottleItemLable, Texture2D>();
    public static Texture2D PrisonChain;
    public static Texture2D cardIcons;

    public static List<Vector2> IconPos = new List<Vector2>();
    // public static List<string> bottleItemNames = new List<string>();
    public static List<int> bottleItemTypes = new List<int>();
    public static List<int> bottleItemLabels = new List<int>();
    // public Texture2D fioleA;
    public static List<NPC> collisionCheckNPC = new List<NPC>();
    public static List<Projectile> collisionCheckProj = new List<Projectile>();

    public override void Load()
    {
        PrisonChain = ModContent.Request<Texture2D>("DeadCellsBossFight/Contents/Biomes/Prison/PrisonElements/PrisonChain", AssetRequestMode.ImmediateLoad).Value;
        cardIcons = ModContent.Request<Texture2D>("DeadCellsBossFight/Assets/cardIcons", AssetRequestMode.ImmediateLoad).Value;

        for (int i = 0; i < 11; i++)
        {
            string folderPath = "DeadCellsBossFight/Contents/Biomes/Prison/PrisonElements/BottleTex/fiole";
            bottleTexA.Add((BottleItemLable)i, AssetsLoader.GetTex(folderPath + "A" + "liquid" + ((BottleItemLable)i).ToString()));
            bottleTexB.Add((BottleItemLable)i, AssetsLoader.GetTex(folderPath + "B" + "liquid" + ((BottleItemLable)i).ToString()));
            bottleTexC.Add((BottleItemLable)i, AssetsLoader.GetTex(folderPath + "C" + "liquid" + ((BottleItemLable)i).ToString()));
        }
        base.Load();
    }
    public override void SetStaticDefaults() // 为什么不写在Load里？因为类没加载完，想要获取各个物品的Type并记录，就在这里写
    {
        LoadBottleJSON("Assets/icon.json");
        base.SetStaticDefaults();
    }
    public override void Unload()
    {
        PrisonChain = null;
        cardIcons = null;

        bottles.Clear();
        allBottleBottomPos.Clear();
        bottleTexA.Clear();
        bottleTexB.Clear();
        bottleTexC.Clear();
        IconPos.Clear();
        bottleItemLabels.Clear();
        bottleItemTypes.Clear();
        collisionCheckNPC.Clear();
        collisionCheckProj.Clear();
        base.Unload();
    }
    public override void PostUpdateNPCs()
    {
        // Player player = Main.LocalPlayer;
        // Main.NewText(Main.MouseWorld);
        ActivateBottleSystem = SubworldSystem.IsActive<PrisonWorld>();

        if (!ActivateBottleSystem)
            return;

        if (AddBottle == 2)
        {
            GetAllBottleBottomPos(allBottleBottomPos, 300); // 没有282也无所谓，少几个无人在意
            // Main.NewText(bottleItemLabels.Count);
            // Main.NewText(allBottleBottomPos.Count);

            bottles.Clear();
            for (int i = 0; i < allBottleBottomPos.Count; i++)
            {
                bool hasItemInside = ! Main.rand.NextBool(60);
                if ( i > 282)
                {
                    hasItemInside = false;
                }
                else if ( ! hasItemInside)
                {
                    i--;
                }
                bottles.Add(new Bottle(allBottleBottomPos[i], hasItemInside, Main.rand.Next(3), i, bottleItemTypes[i]));

            }
            AddBottle = 1;
        }
        if (AddBottle == 3)
        {
            if (bottles.Count > 0)
            {
                foreach (var bottle in bottles)
                {
                    bottle.AddBottleCollidePhysic(NormalUtils.Rand1or_1());
                }
            }
            AddBottle = 4;
        }
        if (AddBottle < 0)
        {
            if (bottles.Count > 0)
            {
                foreach (var bottle in bottles)
                {
                    bottle.ShouldFallAndBreak = true;
                    bottle.gravity = new Vector2(0, 9f);
                }
            }
            AddBottle = 0;
            return;
        }
        if (bottles.Count > 0)
        {

            // Main.NewText(collisionCheckNPC.Count);
            // Main.NewText(collisionCheckProj.Count);

            foreach (var bottle in bottles)
            {
                bottle.UpdateBottle();
                if(bottle.TimeShouldUpdate == 0)
                {
                    if (collisionCheckNPC.Count > 0)
                    {
                        foreach (var npc in collisionCheckNPC)
                        {
                            var X_distance = npc.Center.X - bottle.rl[0].startPoint.pos.X;
                            bool Y_check = npc.Top.Y > bottle.rl[^1].startPoint.pos.Y && npc.Top.Y < bottle.rl[^1].endPoint.pos.Y;
                            if (Y_check && Math.Abs(X_distance) < npc.width / 2)
                            {
                                bottle.AddBottleCollidePhysic(Math.Sign(X_distance));
                                goto EndLoop;  // 直接跳出所有循环
                            }
                            // Collision.CheckAABBvLineCollision(npc.position, npc.Size, bottle.rl[0].startPoint.pos, bottle.rl[^1].endPoint.pos);
                        }
                    }
                    if (collisionCheckProj.Count > 0)
                    {
                        foreach (var proj in collisionCheckProj)
                        {
                            var X_distance = proj.Center.X - bottle.rl[0].startPoint.pos.X;
                            bool Y_check = proj.Top.Y > bottle.rl[^1].startPoint.pos.Y && proj.Top.Y < bottle.rl[^1].endPoint.pos.Y;
                            if (Y_check && Math.Abs(X_distance) < proj.width / 2)
                            {
                                bottle.AddBottleCollidePhysic(Math.Sign(X_distance));
                                goto EndLoop;  // 直接跳出所有循环
                            }
                            // Collision.CheckAABBvLineCollision(npc.position, npc.Size, bottle.rl[0].startPoint.pos, bottle.rl[^1].endPoint.pos);
                        }
                    }
                    var X_distanceP = Main.LocalPlayer.Center.X - bottle.rl[0].startPoint.pos.X;
                    bool Y_checkP = Main.LocalPlayer.Top.Y > bottle.rl[^1].startPoint.pos.Y && Main.LocalPlayer.Top.Y < bottle.rl[^1].endPoint.pos.Y;
                    if (Y_checkP && Math.Abs(X_distanceP) < Main.LocalPlayer.width / 2)
                    {
                        bottle.AddBottleCollidePhysic(Math.Sign(X_distanceP));
                    }

                }
                EndLoop:;
            }
            collisionCheckNPC.RemoveAll(npc => npc.active == false);
            collisionCheckProj.RemoveAll(proj => proj.active == false);
        }
        base.PostUpdateNPCs();
    }

    public override void PostDrawTiles()
    {
        if (!ActivateBottleSystem)
            return;
        if (bottles.Count < 1)
            return;
        SpriteBatch sb = Main.spriteBatch;
        sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


        BottlePhysicalEffects.DrawChainObj(bottles, PrisonChain);
        BottlePhysicalEffects.DrawBottleObj(bottles, PrisonChain);

        sb.End();
        base.PostDrawTiles();
    }
    public void GetAllBottleBottomPos(List<Vector2> allBottleBottomPos, int bottleMaxCount)
    {
        int rect_x = 710;
        int rect_y = 760;
        int rect_width = 1860;
        int rect_height = 780;

        // 正方形的参数
        int square_size = 50;
        int num_squares = bottleMaxCount;
        allBottleBottomPos.Clear();
        allBottleBottomPos.Add(new Vector2(720, 780));
        // allBottleBottomPos = new List<Vector2>();
        for (int i = 0; i < num_squares; i++)
        {
            int tryCount = 50;
            while (tryCount > 0)
            {
                tryCount--;
                int x = Main.rand.Next(rect_x, rect_x + rect_width - square_size);
                int y = Main.rand.Next(rect_y, rect_y + rect_height - square_size);

                bool is_overlapping = false;
                foreach (Vector2 pos in allBottleBottomPos)
                {
                    if (Math.Abs(x - pos.X) < square_size && Math.Abs(y - pos.Y) < square_size)
                    {
                        is_overlapping = true;
                        break;
                    }
                }
                if (!is_overlapping)
                {
                    allBottleBottomPos.Add(new Vector2(x, y));
                    break;
                }

            }
        }
    }

    public static void LoadBottleJSON(string filePath)
    {
        // 读取 JSON 文件
        // string json = File.ReadAllText(filePath);
        Stream stream = DeadCellsBossFight.Instance.GetFileStream(filePath);
        using StreamReader sr = new StreamReader(stream);
        // 读取整个文件内容为字符串
        string json = sr.ReadToEnd();

        // 使用 JArray 解析 JSON 数组
        JArray dataArray = JArray.Parse(json);

        // 遍历每一条数据，将 x, y 存入 positions，将 BottleItemLable 存入 bottleItemLabels
        
        foreach (JObject item in dataArray.Cast<JObject>())
        {
            int x = item["x"].Value<int>();
            int y = item["y"].Value<int>();
            int bottleItemLabel = item["BottleItemLable"].Value<int>();

            string name = item["id"].Value<string>();
            Console.WriteLine(name);
            if (ModContent.TryFind<ModItem>(name, out ModItem modItem))
                {
                Console.WriteLine();
                Console.WriteLine(modItem.Type);
                if (ModContent.TryFind<DeadCellsItem>(name, out DeadCellsItem aa))
                    Console.WriteLine("yes!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                else
                    Console.WriteLine("no!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                bottleItemTypes.Add(modItem.Type); // 游戏内Item的type
                if (modItem is DeadCellsItem DCitem)
                {
                    // 赋值给对应武器。相当于setdefault
                    DCitem.iconX = x;
                    DCitem.iconY = y;
                    DCitem.ItemLabel = bottleItemLabel;
                    int index = Bottle.GetHaloTextureIndex(bottleItemLabel);
                    DCitem.colorIdx1 = index;
                    // 对于多流派次要颜色进行赋值
                    DCitem.colorIdx2 = DCitem.ItemLabel switch
                    {
                        4 => 3 - index,
                        9 => 2 - index,
                        10 => 1 - index,
                        _ => index,
                    };
                }
            }
            else
                bottleItemTypes.Add(-1);

            // bottleItemNames.Add(name);
            IconPos.Add(new Vector2(x, y));
            bottleItemLabels.Add(bottleItemLabel);
        }
    }

}
public class BottlePhysicalEffects
{
    public static int RopeRigidity = 16; //绳刚性
    // public static Vector2 gravity = new(0, 40);
    public static void VerletObjPosiUpdate(List<Rope_Point> rp, List<Rope_Line> rl, Vector2 headPos, Vector2 gravity, bool ShouldFallAndBreak = false)
    {
        for (int k = 1; k < rp.Count - 1; k++)
        {
            rp[k].locked = false;
        }


        if (rp.Count > 1)
        {
            if (!ShouldFallAndBreak)
            {
                rp[0].pos = headPos;
            }
            rp[0].oldpos = rp[0].pos;
        }

        for (int i = 0; i < rp.Count; i++)
        {
            Rope_Point p = rp[i];
            if (ShouldFallAndBreak)
            {
                p.pos += gravity;
                continue;
            }
            if (!p.locked)
            {
                Vector2 vector = p.pos;
                p.pos += p.pos - p.oldpos + gravity;
                p.oldpos = vector;
            }
        }
        if (ShouldFallAndBreak) return;
        for (int k = 0; k < RopeRigidity; k++)
        {
            for (int i = 0; i < rl.Count; i++)
            {
                Rope_Line rope = rl[i];
                Vector2 endToStart = rope.endPoint.pos - rope.startPoint.pos;
                float length = endToStart.Length();
                length = (length - rope.Length) / length;

                if (!rope.startPoint.locked)
                {
                    rope.startPoint.pos.X += 1.5f * endToStart.X * length;
                    rope.startPoint.pos.Y += 0.5f * endToStart.Y * length;
                }
                if (!rope.endPoint.locked)
                {
                    rope.endPoint.pos.X -= 1.5f * endToStart.X * length;
                    rope.endPoint.pos.Y -= 0.5f * endToStart.Y * length;
                }

            }
        }
    }
    public static void DrawSingleChainObj(List<Rope_Line> rl, Texture2D texture)
    {
        Vector2 origin = new Vector2(texture.Width, texture.Height) / 2f;

        for (int i = 0; i < rl.Count; i++)
        {
            Rope_Line line = rl[i];
            Vector2 lineVec = line.endPoint.pos - line.startPoint.pos;
            int length = (int)lineVec.Length();
            float rotation = lineVec.ToRotation() - MathHelper.PiOver2;
            Vector2 drawPos = line.endPoint.pos - Main.screenPosition - lineVec / 2;

            Rectangle destinationRectangle = new Rectangle((int)drawPos.X, (int)drawPos.Y, length, length);
            Rectangle iconDestinationRectangle = new Rectangle((int)drawPos.X, (int)drawPos.Y, length / 3, length / 3);

            Main.spriteBatch.Draw(texture, destinationRectangle, null, Color.White, rotation, origin, 0, 0);

        }

    }
    public static void DrawChainObj(List<Bottle> bottles, Texture2D texture)
    {
        foreach (Bottle bottle in bottles)
        {
            Vector2 origin = new Vector2(texture.Width, texture.Height) / 2f;
            var rl = bottle.rl;

            for (int i = 0; i < rl.Count - 1; i++)
            {
                Rope_Line line = rl[i];
                Vector2 lineVec = line.endPoint.pos - line.startPoint.pos;
                int length = (int)lineVec.Length();
                float rotation = lineVec.ToRotation() - MathHelper.PiOver2;
                Vector2 drawPos = line.endPoint.pos - Main.screenPosition - lineVec / 2;

                Rectangle destinationRectangle = new Rectangle((int)drawPos.X, (int)drawPos.Y, length, length);
                Rectangle iconDestinationRectangle = new Rectangle((int)drawPos.X, (int)drawPos.Y, length / 3, length / 3);

                Main.spriteBatch.Draw(texture, destinationRectangle, null, Color.White, rotation, origin, 0, 0);

            }

        }
    }
    public static void DrawBottleObj(List<Bottle> bottles, Texture2D texture)
    {
        foreach (Bottle bottle in bottles)
        {
            Vector2 origin = new Vector2(texture.Width, texture.Height) / 2f;
            var rl = bottle.rl;
            if (rl.Count > 0)
            {
                Texture2D bottleTex = bottle.GetBottleTex();
                Texture2D waterTex = bottle.GetWaterTex();
                Rectangle iconRectangle = bottle.GetIconRectangle();
                Rope_Line line = rl[^1];
                Vector2 lineVec = line.endPoint.pos - line.startPoint.pos;
                int length = (int)lineVec.Length();
                float rotation = lineVec.ToRotation() - MathHelper.PiOver2;
                Vector2 drawPos = line.endPoint.pos - Main.screenPosition - lineVec / 2;

                Rectangle destinationRectangle = new Rectangle((int)drawPos.X, (int)drawPos.Y, length, length);
                Rectangle iconDestinationRectangle = new Rectangle((int)drawPos.X, (int)drawPos.Y, length / 3, length / 3);


                Main.spriteBatch.Draw(BottleSystem.cardIcons, iconDestinationRectangle, iconRectangle, Color.White, rotation, new Vector2(12, 12), SpriteEffects.None, 0);
                Main.spriteBatch.Draw(waterTex, destinationRectangle, null, Color.White, rotation, origin, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(bottleTex, destinationRectangle, null, Color.White, rotation, origin, SpriteEffects.None, 0);
            }
        }
    }
    public static void AddVerletObj(List<Rope_Point> rp, List<Rope_Line> rl, Vector2 spawnPos, float LineLength = 5f)
    {

        if (rp.Count == 0)
        {
            Rope_Point startPoint = new Rope_Point(spawnPos);
            startPoint.locked = true;
            rp.Add(startPoint);
        }
        if (rp.Count > 0)
        {
            Rope_Point point = new Rope_Point(spawnPos);
            rp.Add(point);
            Rope_Line line = new Rope_Line(rp[rp.Count - 2], rp[rp.Count - 1], LineLength);
            rl.Add(line);
        }

    }
}