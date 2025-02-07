﻿namespace DeadCellsBossFight.Core.TexGenWorld
{
    public class TileInfo
    {
        public int tileID = -1;
        public int tileStyle;
        public int liquidAmt;

        public TileInfo(int tileID, int style)
        {
            this.tileID = tileID;
            tileStyle = style;
        }
    }

    public class WallInfo
    {
        public int wallID = -1;

        public WallInfo(int wallID)
        {
            this.wallID = wallID;
        }
    }
}
