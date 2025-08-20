using Microsoft.Build.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace DeadCellsBossFight.Core;
/// <summary>
/// 细胞人的武器、技能UI槽
/// </summary>
public  class WeaponSkillUISlot
{
    public int DCWeaponType;
    public int icon_X;
    public int icon_Y;
    /// <summary>
    /// 颜色索引，0：暴虐，1：战术，2：生存，3：空（浅蓝））
    /// </summary>
    public int colorIdx1;
    /// <summary>
    /// 颜色索引，0：暴虐，1：战术，2：生存，3：空（浅蓝）
    /// </summary>
    public int colorIdx2;

    public Vector2 drawCenterPos;

    public WeaponSkillUISlot(Vector2 drawCenterPosition, int iconX = 0, int iconY = 0, int colorindex1 = 3, int colorindex2 = 3) 
    {
        drawCenterPos = drawCenterPosition;
        icon_X = iconX;
        icon_Y = iconY;
        colorIdx1 = colorindex1;
        colorIdx2 = colorindex2;
    }
    public void DrawSelf(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(AssetsLoader.skillBg, drawCenterPos, null, new Color(60, 60, 60, 160), 0, new Vector2(16, 0), 3f, SpriteEffects.None, 0);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        Rectangle iconRectangle = new Rectangle(icon_X * 24, icon_Y * 24, 24, 24);

        if (colorIdx1 == 3 || colorIdx1 == colorIdx2) // 没有物品 || 单流派
        {
            AssetsLoader.SkillUIColor.Parameters["Input1RGB"].SetValue((AssetsLoader.ThreeSectColor[colorIdx1].ToVector3() / 255f));
            AssetsLoader.SkillUIColor.CurrentTechnique.Passes["UI1Color"].Apply();
            spriteBatch.Draw(AssetsLoader.skillSlotFull, drawCenterPos, null, Color.White, 0, new Vector2(15, 0), 3f, SpriteEffects.None, 0);
        }
        else
        {
            AssetsLoader.SkillUIColor.Parameters["Input1RGB"].SetValue((AssetsLoader.ThreeSectColor[colorIdx1].ToVector3() / 255f));
            AssetsLoader.SkillUIColor.Parameters["Input2RGB"].SetValue((AssetsLoader.ThreeSectColor[colorIdx2].ToVector3() / 255f));
            AssetsLoader.SkillUIColor.CurrentTechnique.Passes["UI2Color"].Apply();
            spriteBatch.Draw(AssetsLoader.skillSlotFull, drawCenterPos, null, Color.White, 0, new Vector2(15, 0), 3f, SpriteEffects.None, 0);
        }


        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);

        Main.NewText(Main.MouseWorld);


    }

}
