using DeadCellsBossFight.Contents.SubWorlds;
using SubworldLibrary;
using Terraria;
using Terraria.Graphics.Effects;

namespace DeadCellsBossFight.Contents.Biomes.Prison;

public class PrisonBiome : DCBasicBiome
{
    public override string SkyKey => "PrisonSky";

    // Select Music
    // public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/MysteriousMystery")
    public override void Load()
    {
        SkyManager.Instance[SkyKey] = new PrisonSky();
    }
    public override bool IsBiomeActive(Player player)
    {
        return SubworldSystem.IsActive<PrisonWorld>();
    }

}
