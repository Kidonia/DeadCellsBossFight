using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace DeadCellsBossFight.Utils;

public static class TilemapExtensions
{
    public static bool TryGet(this Tilemap tilemap, Point16 pos, out Tile tile)
    {
        return tilemap.TryGet(pos.X, pos.Y, out tile);
    }
    public static bool TryGet(this Tilemap tilemap, int x, int y, out Tile tile)
    {
        if (x >= 0 && y >= 0 && x < Main.maxTilesX && y < Main.maxTilesY)
        {
            tile = tilemap[x, y];
            return true;
        }
        tile = default;
        return false;
    }
    /// <summary>
    /// Performs an index-safe tile retrieval. If this mistakenly attempts to access a tile outside of the world, it returns a default, empty tile rather than throwing an <see cref="IndexOutOfRangeException"/>.
    /// </summary>
    /// <param name="x">The X position of the tile.</param>
    /// <param name="y">The Y position of the tile.</param>
    public static Tile GetTileSafely(int x, int y)
    {
        if (!WorldGen.InWorld(x, y))
            return new();

        return Main.tile[x, y];
    }

    public static Vector2 Normalized(this Vector2 vector)
    {
        return Vector2.Normalize(vector);
    }
}
