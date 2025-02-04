using DeadCellsBossFight.Contents.Biomes.BlackBridge;
using DeadCellsBossFight.Contents.SubWorlds;
using SubworldLibrary;
using Terraria.Graphics.Effects;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace DeadCellsBossFight.Contents.Biomes;

public abstract class DCBasicBiome : ModBiome
{
    /// <summary>
    /// 天空的名称。
    /// </summary>
    public virtual string SkyKey => "";
    public abstract override bool IsBiomeActive(Player player);
    //加载天空
    //重写时加上类似 SkyManager.Instance[SkyKey] = new BlackBridgeSky();
    public abstract override void Load();

    // public override ModWaterStyle WaterStyle => ;
    //使用空白的地表。
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<EmptyBG>();
    // public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<BlackBridgeBG>();

    // 优先级最高
    public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

    // 音乐选择
    // public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/MysteriousMystery")
    public override string MapBackground => BackgroundPath;
    public override Color? BackgroundColor => Color.White;

    public override void SpecialVisuals(Player player, bool isActive)
    {
        if (SkyManager.Instance[SkyKey] is not null && isActive != SkyManager.Instance[SkyKey].IsActive())
        {
            if (isActive)
                SkyManager.Instance.Activate(SkyKey);
            else
                SkyManager.Instance.Deactivate(SkyKey);
        }
    }
}
