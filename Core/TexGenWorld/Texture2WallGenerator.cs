namespace DeadCellsBossFight.Core.TexGenWorld;

public class Texture2WallGenerator : TextureGenerator
{
    public Texture2WallGenerator(int width, int height) : base(width, height)
    {
        this.wallGen = new WallInfo[width, height];
    }

    public void Generate(int x, int y, bool sync)
    {
        for (int x2 = 0; x2 < this.width; x2++)
        {
            for (int y2 = 0; y2 < this.height; y2++)
            {
                int current_x = x + x2;
                int current_y = y + y2;
                WallInfo info = this.wallGen[x2, y2];
                if (info.wallID != -1)
                {
                    WorldGenHelper.Texture2WallGenerate(current_x, current_y, info.wallID);
                }
            }
        }
    }

    // Token: 0x04000172 RID: 370
    public WallInfo[,] wallGen;
}
