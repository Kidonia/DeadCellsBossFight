using DeadCellsBossFight.Contents.SubWorlds;
using SubworldLibrary;
using Terraria;
using Terraria.Graphics.Effects;

namespace DeadCellsBossFight.Contents.Biomes.QueenArena;

public class QueenArenaBiome : DCBasicBiome
{
    public override string SkyKey => "QueenArenaSky";

    // Select Music
    // public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/MysteriousMystery")

    public override void Load()
    {
        SkyManager.Instance[SkyKey] = new QueenArenaSky();
    }
    public override bool IsBiomeActive(Player player)
    { 
        return SubworldSystem.IsActive<QueenArenaWorld>();
    }
}
