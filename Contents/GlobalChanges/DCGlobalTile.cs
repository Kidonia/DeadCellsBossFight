using Terraria.ModLoader;
using SubworldLibrary;
using Terraria;
using Microsoft.Xna.Framework.Graphics;

namespace DeadCellsBossFight.Contents.GlobalChanges;

public class DCGlobalTile : GlobalTile
{
    public override bool CanPlace(int i, int j, int type)
    {
        return IsNOTinSubworld();
    }
    public override bool CanExplode(int i, int j, int type)
    {
        return IsNOTinSubworld();
    }
    public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
    {
        return IsNOTinSubworld();
    }
    public override bool CanReplace(int i, int j, int type, int tileTypeBeingPlaced)
    {
        return IsNOTinSubworld();
    }
    public override bool CanDrop(int i, int j, int type)
    {
        return IsNOTinSubworld();
    }
    public override bool AutoSelect(int i, int j, int type, Item item)
    {
        return IsNOTinSubworld();
    }
    public override bool Slope(int i, int j, int type)
    {
        return IsNOTinSubworld();
    }

    // True = Tiles are unbreakable, False = Tiles are breakable.
    public bool IsNOTinSubworld()
    {
        if (SubworldSystem.AnyActive())
            return false;
        return true;
    }

    public override void PostDraw(int i, int j, int type, SpriteBatch spriteBatch)
    {
        base.PostDraw(i, j, type, spriteBatch);
    }
}
