using System;
using Terraria.ModLoader;
using DeadCellsBossFight.Contents.Tiles;
using Terraria.Graphics;
using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.UI;
using Terraria.WorldBuilding;
using Terraria.ModLoader.IO;
using SubworldLibrary;
using DeadCellsBossFight.Contents.SubWorlds;
using Microsoft.Xna.Framework.Graphics;
using DeadCellsBossFight.NPCs;

namespace DeadCellsBossFight.Core;

public class DCWorldSystem : ModSystem
{
    public int BlackBridgeBlockCount;
    public bool PreventZoomChange;

    public static bool ChangeToPrisonSky2;
    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
    {
        BlackBridgeBlockCount = tileCounts[ModContent.TileType<DCNormalTile>()];
    }
    
    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        int shimmerIndex = tasks.FindIndex((genpass) => genpass.Name.Equals("Shimmer"));

        base.ModifyWorldGenTasks(tasks, ref totalWeight);
    }
    public static Vector2 shimmerPosition;
    public override void PostWorldGen()
    {
        shimmerPosition = new Vector2((float)GenVars.shimmerPosition.X + 45, (float)GenVars.shimmerPosition.Y - 10) * 16;
        base.PostWorldGen();
    }
    public override void PreUpdateWorld()
    {
        if (shimmerPosition == Vector2.Zero)
        {
            shimmerPosition = ReGenerateShimmer();
        }

        base.PreUpdateWorld();
    }

    public static bool BH_active = false;
    public static int BH_whoAmI = 0;
    public override void PostUpdateNPCs()
    {
        if(SubworldSystem.IsActive<PrisonWorld>() && BH_active)
        {
            if (Main.npc[BH_whoAmI].active && Main.npc[BH_whoAmI].type == ModContent.NPCType<BH>())
                return;
            foreach (NPC Npc in Main.ActiveNPCs)
            {
                if (Npc.type == ModContent.NPCType<BH>())
                {
                    return;
                }
            }

            BH_active = false;
        }
    }

    public static bool startCheckGrappleHookProj = false;
    public static bool hasPlayerGrappleHookProj = false;

    public static bool startCheckMinionProj = false;
    public static bool hasPlayerMinionProj = false;
    public override void PostUpdateProjectiles()
    {
        if (SubworldSystem.IsActive<QueenArenaWorld>())
        {
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                //顺手在这检测了，反正能省遍历就省
                if (startCheckGrappleHookProj && !hasPlayerGrappleHookProj)
                {
                    if (proj.aiStyle == 7 && proj.ai[0] == 2f)
                    {
                        hasPlayerGrappleHookProj = true;
                        break;
                    }
                }
                if (startCheckMinionProj && !hasPlayerMinionProj)
                {
                    if (proj.minion || Main.projPet[proj.type])
                    {
                        hasPlayerMinionProj = true;
                        break;
                    }
                }

            }
        }
        base.PostUpdateProjectiles();
    }
    public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
    {
        //Main.NewText(shimmerPosition);
        /*
        Main.NewText(Main.LocalPlayer.position, Color.BlueViolet);
        Main.NewText(Main.rightWorld);
        */
        // 固定屏幕为最小放大倍数
        if (PreventZoomChange)
        {

            Transform.Zoom = new Vector2(1, 1);
        }
        base.ModifyTransformMatrix(ref Transform);
    }

    // public static bool ActivateOnionSkinTrailDraw;
    public static List<OnionSkinTrail> onionSkinTrails = new List<OnionSkinTrail>();
    public override void PostDrawTiles()
    {
        if(onionSkinTrails.Count < 1)
            return;
        SpriteBatch sb = Main.spriteBatch;
        sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        foreach (var item in onionSkinTrails)
            item.DrawUpdateBHOnionSkinTrail();
        sb.End();

        onionSkinTrails.RemoveAll(obj => obj.active == false);
        base.PostDrawTiles();
    }


    //单独使用，为NPC对话用
    public static DialogueBox singleDialogueBox = new DialogueBox();
    public static int collectorNPCidx;
    //对话数组，后面写满屏对话特效用
    public DialogueBox[] dialogueBoxArray = new DialogueBox[200];
    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        // https://github.com/tModLoader/tModLoader/wiki/Vanilla-Interface-layers-values
        base.ModifyInterfaceLayers(layers);//鼠标放上去可以去wiki链接里看

        if (singleDialogueBox.active)
        {
            DialogueBox.UpdateDialogueBox(singleDialogueBox, Main.npc[collectorNPCidx].Center - new Vector2(0, 200) - Main.screenPosition);

            int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Capture Manager Check"));//这里是字符串匹配。
            if (resourceBarIndex != -1)
            {
                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "DialogueUI(not actually)",
                    delegate
                    {
                        singleDialogueBox.DrawDialogueBox(Main.spriteBatch);
                        return true;
                    },
                    InterfaceScaleType.Game)
                );
            }
        }
    }



    public override void SaveWorldData(TagCompound tag)
    {
        tag["shimmerPositionX"] = shimmerPosition.X;
        tag["shimmerPositionY"] = shimmerPosition.Y;

    }
    public override void LoadWorldData(TagCompound tag)
    {
        if (tag.TryGet("shimmerPositionX", out float shimmerPositionX) && tag.TryGet("shimmerPositionY", out float shimmerPositionY))
            shimmerPosition = new Vector2(shimmerPositionX, shimmerPositionY);
        else
            shimmerPosition = Vector2.Zero;
    }
    public Vector2 ReGenerateShimmer()
    {
        bool leftDungeon = Main.dungeonX - Main.rightWorld / 2 < 0;
        int num702 = 50;
        int num703 = (int)(Main.worldSurface + Main.rockLayer) / 2 + num702;
        int num704 = (int)((Main.maxTilesY - 250) * 2 + Main.rockLayer) / 3;
        if (num704 > Main.maxTilesY - 330 - 100 - 30)
        {
            num704 = Main.maxTilesY - 330 - 100 - 30;
        }
        if (num704 <= num703)
        {
            num704 = num703 + 50;
        }
        int num705 = WorldGen.genRand.Next(num703, num704);
        int num706 = leftDungeon ? WorldGen.genRand.Next((int)(Main.maxTilesX * 0.89), Main.maxTilesX - 200) : WorldGen.genRand.Next(200, (int)(Main.maxTilesX * 0.11));
        int num707 = (int)Main.worldSurface + 150;
        int num708 = (int)(Main.rockLayer + Main.worldSurface + 200.0) / 2;
        if (num708 <= num707)
        {
            num708 = num707 + 50;
        }
        if (Main.tenthAnniversaryWorld)
        {
            num705 = WorldGen.genRand.Next(num707, num708);
        }
        int num709 = 0;

        while (!WorldGen.ShimmerMakeBiome(num706, num705))
        {
            num709++;
            if (Main.tenthAnniversaryWorld && num709 < 10000)
            {
                num705 = WorldGen.genRand.Next(num707, num708);
                num706 = leftDungeon ? WorldGen.genRand.Next((int)(Main.maxTilesX * 0.89), Main.maxTilesX - 200) : WorldGen.genRand.Next(200, (int)(Main.maxTilesX * 0.11));
            }
            else if (num709 > 20000)
            {
                num705 = WorldGen.genRand.Next((int)Main.worldSurface + 100 + 20, num704);
                num706 = leftDungeon ? WorldGen.genRand.Next((int)(Main.maxTilesX * 0.8), Main.maxTilesX - 200) : WorldGen.genRand.Next(200, (int)(Main.maxTilesX * 0.2));
            }
            //Main.dungeonX;
            else
            {
                num705 = WorldGen.genRand.Next((int)(Main.worldSurface + Main.rockLayer) / 2 + 20, num704);
                num706 = leftDungeon ? WorldGen.genRand.Next((int)(Main.maxTilesX * 0.89), Main.maxTilesX - 200) : WorldGen.genRand.Next(200, (int)(Main.maxTilesX * 0.11));
            }
        }
        //GenVars.shimmerPosition = new Vector2D((double)num706, (double)num705);
        Main.NewText("微光之地重新绽出星流", new Color(165, 131, 239));
        return new Vector2(num706 + 45, num705 - 10) * 16;
    }
}
