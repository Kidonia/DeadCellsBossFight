using DeadCellsBossFight.Items;
using DeadCellsBossFight.NPCs.ExtraBosses.Queen;
using DeadCellsBossFight.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;

namespace DeadCellsBossFight.Core;

/// <summary>
/// 物品的卷轴颜色，也就是那些int itemLabel，只不过用enum看着清晰，别动它，哦内盖
/// </summary>
public enum BottleItemLable : int
{
    Empty,
    Broken_0,
    Broken_1,
    Green,
    GreenPurple,
    Meta,
    NoColor,
    Purple,
    Red,
    RedGreen,
    RedPurple
}
public class Bottle
{
    public bool active;
    public List<Rope_Point> rp = new List<Rope_Point>();
    public List<Rope_Line> rl = new List<Rope_Line>();
    public Vector2 hangPos;
    public int ropeCount;
    /// <summary>
    /// 0:U瓶，1:方瓶，2:圆瓶
    /// </summary>
    public int bottleTextureType = 0;

    /// <summary>
    /// 瓶内是否有物品
    /// </summary>
    public bool hasItemInside = false;
    /// <summary>
    /// 0:空瓶，1:碎瓶1，2:碎瓶2
    /// </summary>
    public int noItemBottleType = 0; 

    /// <summary>
    /// 第几个瓶子，一共二百多
    /// </summary>
    public int bottleIndex;
    /// <summary>
    /// 瓶内物体在泰拉模组里的 type，未完成则默认 -1
    /// </summary>
    public int itemType = -1;

    public bool ShouldFallAndBreak;
    public Vector2 gravity = new Vector2(0, 30);

    public int TimeShouldUpdate = 240;

    public Bottle()
    {
        this.active = false;
    }
    public Bottle(Vector2 bottleBottomPos, bool hasItemInside, int bottleTextureType = 0, int itemInsideIdx = -1, int itemType = -1)
    {
        this.rp = new List<Rope_Point>();
        this.rl = new List<Rope_Line>();
        getHangPosSpawn(bottleBottomPos);
        for (int i = 0; i < ropeCount; i++)//7个差不多
        {
            BottlePhysicalEffects.AddVerletObj(this.rp, this.rl, this.hangPos + new Vector2(0, i * 48 * 3), 48 * 3);
        }
        rp[^1].pos += Vector2.UnitX * 4;

        this.bottleTextureType = bottleTextureType;


        this.hasItemInside = hasItemInside;
        if ( ! hasItemInside)
        {
            this.noItemBottleType = Main.rand.Next(3);
        }
        this.bottleIndex = itemInsideIdx;
        this.itemType = itemType;

        this.active = true;
    }
    public void UpdateBottle()
    {
        // Main.NewText(Main.MouseWorld);
        if (!this.active) return;
        if (this.rp.Count > 0 && this.rl.Count > 0)
        {
            if (this.ShouldFallAndBreak)
            {
                // this.rp[0].locked = false;
                this.TimeShouldUpdate = 2;
                this.gravity.Y += 0.9f;
                if (this.rp[^1].pos.Y > 1668) // 牢房地面往下36
                {
                    ////////////////////////////////////////////////////
                    if (this.itemType > 0)
                    {
                        int k = Item.NewItem(new EntitySource_DebugCommand("bottlebreak"), this.rp[^2].pos, this.itemType);

                        if (DeadCellsBossFight.Instance.TryFind<DeadCellsItem>(Main.item[k].ModItem.Name, out DeadCellsItem aa))
                        {
                            Main.NewText(aa.Name);
                        }
                        if (Main.item[k].ModItem is DeadCellsItem DCitem)
                        {
                            if (DCitem.DualWeaponOffhand)
                                Main.item[k].active = false;
                        }
                    }

                    this.rp.Clear();
                    this.rl.Clear();
                    this.active = false;
                    // Main.NewText("钢管落地.mp3");
                    return;
                }
            }
            if (this.TimeShouldUpdate > 0)
            {
                this.TimeShouldUpdate--;
                BottlePhysicalEffects.VerletObjPosiUpdate(this.rp, this.rl, this.hangPos, this.gravity, this.ShouldFallAndBreak);
            }
        }
    }
    /// <summary>
    /// 获取物体掉在地上时环绕光环的材质，注意对于双卷轴物品返回两个流派中的随机一项。0：暴虐，1：战术，2：生存，3：透明其他
    /// </summary>
    /// <param name="bottleItemLabel"></param>
    /// <returns></returns>
    public static int GetHaloTextureIndex(int bottleItemLabel)
    {
        return bottleItemLabel switch
        {
            3 => 2,
            4 => Main.rand.Next(1, 3),
            7 => 1,
            8 => 0,
            9 => Main.rand.Next(2) * 2,
            10 => Main.rand.Next(2),
            _ => 3,
        };
    }

    public void getHangPosSpawn(Vector2 bottleBottomPos)
    {
        // 确保是在牢房里
        float ex = (bottleBottomPos.Y - 656) / (1800 - 656) * 30;
        this.ropeCount = Math.Max(2, (int)((bottleBottomPos.Y - 656) / (145 + ex)) + 1);
        this.hangPos = new Vector2(bottleBottomPos.X, bottleBottomPos.Y - ropeCount * (145 + ex));
    }
    public void AddBottleCollidePhysic(int direction)
    {
        if (this.TimeShouldUpdate > 0 || this.rp.Count < 0 || this.rl.Count < 0 || this.ShouldFallAndBreak)
            return;
        this.TimeShouldUpdate += 180; //// 后续要更改，减短
        this.rp[^1].pos += new Vector2(-55 * direction, -5);
    }
    public Texture2D GetBottleTex()
    {
        if (hasItemInside)
            this.noItemBottleType = 0;
        switch (this.bottleTextureType)
        {
            case 0:
                return BottleSystem.bottleTexA[(BottleItemLable)this.noItemBottleType];
            case 1:
                return BottleSystem.bottleTexB[(BottleItemLable)this.noItemBottleType];
            case 2:
                return BottleSystem.bottleTexC[(BottleItemLable)this.noItemBottleType];
            default:
                return BottleSystem.bottleTexA[BottleItemLable.Empty];
        }
    }
    public Texture2D GetWaterTex()
    {
        if (!hasItemInside) // 无物
            return AssetsLoader.TransparentDot;
        else
        {
            switch (this.bottleTextureType)
            {
                case 0:
                    return BottleSystem.bottleTexA[(BottleItemLable)BottleSystem.bottleItemLabels[this.bottleIndex]];
                case 1:
                    return BottleSystem.bottleTexB[(BottleItemLable)BottleSystem.bottleItemLabels[this.bottleIndex]];
                case 2:
                    return BottleSystem.bottleTexC[(BottleItemLable)BottleSystem.bottleItemLabels[this.bottleIndex]];
                default:
                    return AssetsLoader.TransparentDot;
            }
        }
    }
    /// <summary>
    /// 获取图标的位置
    /// </summary>
    /// <returns></returns>
    public Rectangle GetIconRectangle()
    {
        if (!hasItemInside) // 无物
            return new(0, 0, 1, 1);
        else
        {
            Vector2 iconPos = BottleSystem.IconPos[this.bottleIndex];
            return new((int)iconPos.X * 24, (int)iconPos.Y * 24, 24, 24);
        }
    }
    public void KillBottle()
    {
        this.active = false;
        this.rp.Clear();
        this.rl.Clear();
    }
}
