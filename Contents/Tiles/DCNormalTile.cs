using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Contents.Tiles;

public class DCNormalTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileNoAttach[Type] = true;
        DustType = DustID.Corruption;

        AddMapEntry(new Color(20, 40, 60));
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = 1;
    }


}
