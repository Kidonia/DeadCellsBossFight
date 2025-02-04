using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ObjectData;

namespace DeadCellsBossFight.Core.TexGenWorld
{
    // Token: 0x020002A5 RID: 677
    public static class TileHelper
    {
        // Token: 0x06000E8A RID: 3722 RVA: 0x000637E8 File Offset: 0x000619E8
        public static bool topSlope(this Tile tile)
        {
            byte b = (byte)tile.Slope;
            return b == 1 || b == 2;
        }

        public static bool HasSolidTile(this Tile tile)
        {
            return tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType];
        }

        public static Vector2 FindTopLeft(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (tile == null)
            {
                return new Vector2(x, y);
            }
            TileObjectData data = TileObjectData.GetTileData((int)(tile.TileType), 0, 0);
            x -= tile.TileFrameX / 18 % data.Width;
            y -= tile.TileFrameY / 18 % data.Height;
            return new Vector2(x, y);
        }
    }
}
