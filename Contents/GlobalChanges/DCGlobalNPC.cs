using DeadCellsBossFight.Contents.SubWorlds;
using DeadCellsBossFight.Core;
using DeadCellsBossFight.NPCs;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Contents.GlobalChanges;

public class DCGlobalNPC : GlobalNPC
{
    public override bool PreAI(NPC npc)
    {
        if (SubworldSystem.IsActive<PrisonWorld>() && BottleSystem.ActivateBottleSystem)
        {
            if(npc.active && npc.Top.Y < BH.BottleBottom) // npc 在瓶子底端位置往上
            {
                if( ! BottleSystem.collisionCheckNPC.Contains(npc))
                    BottleSystem.collisionCheckNPC.Add(npc);
            }
            else
                if (BottleSystem.collisionCheckNPC.Contains(npc))
                    BottleSystem.collisionCheckNPC.Remove(npc);
        }
        return base.PreAI(npc);
    }
    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
    {
        if (SubworldSystem.AnyActive())
        {
            spawnRate = 0;
            maxSpawns = 0;
        }
    }
}

