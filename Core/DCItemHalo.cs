
using Microsoft.Xna.Framework;

namespace DeadCellsBossFight.Core;

public class DCItemHalo
{
    /// <summary>
    /// 0:暴虐，1:战术，2:生存
    /// </summary>
    public int HaloTextureType = 0;

    public bool active;
    public Vector2 ItemCenter;


    public DCItemHalo(int haloTextureType)
    {
        active = true;
        HaloTextureType = haloTextureType;
    }

    private void DrawItemHalo()
    {
        if (active)
        {

        }
    }
}
