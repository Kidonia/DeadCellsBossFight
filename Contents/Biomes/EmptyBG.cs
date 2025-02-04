using DeadCellsBossFight.Core;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Contents.Biomes;

public class EmptyBG : ModSurfaceBackgroundStyle
{
    //  All the Drawing methods, see BlackBridgeSky.cs，...
    // 仅做一个环境占位，所有的绘制见 BlackBridgeSky.cs 等
    public override void ModifyFarFades(float[] fades, float transitionSpeed)
    {
        return;
    }
    public override int ChooseFarTexture()
    {
        return BackgroundTextureLoader.GetBackgroundSlot(AssetsLoader.TransparentImg);
    }
    public override int ChooseMiddleTexture()
    {
        return BackgroundTextureLoader.GetBackgroundSlot(AssetsLoader.TransparentImg);
    }

    public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
    {
        return BackgroundTextureLoader.GetBackgroundSlot(AssetsLoader.TransparentImg);
    }
}
