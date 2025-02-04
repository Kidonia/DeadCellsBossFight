using Terraria;
using SubworldLibrary;
using DeadCellsBossFight.Contents.SubWorlds;
using Terraria.Graphics.Effects;

namespace DeadCellsBossFight.Contents.Biomes.BlackBridge;

public class BlackBridgeBiome : DCBasicBiome
{
    public override string SkyKey => "BlackBridgeSky";
    // Select Music
    // public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/MysteriousMystery")

    public override void Load()
    {
        SkyManager.Instance[SkyKey] = new BlackBridgeSky();
    }
    // Calculate when the biome is active.
    public override bool IsBiomeActive(Player player)
    {
        return SubworldSystem.IsActive<BlackBridgeWorld>();
    }

}
