using SubworldLibrary;
using System.Collections.Generic;
using Terraria.WorldBuilding;
using Terraria;
using Microsoft.Xna.Framework;
using DeadCellsBossFight.Core;
using Terraria.Audio;

namespace DeadCellsBossFight.Contents.SubWorlds;

public class PrisonWorld : Subworld
{
    //世界宽度
    public override int Width => 203;
    //世界高度
    public override int Height => 160;

    //世界生成的进程。直接按下面这样写
    public override List<GenPass> Tasks => new()
    {
        // new ExampleGenPass()
        new DCWorldGenPasses.PrisonGenPass()
    };
    //进入子世界的设置
    public override void OnLoad()
    {
        Main.dayTime = true;
        Main.time = 27000;
    }
    public override void Update()
    {
        // 禁用玩家放大缩小
        Main.GameViewMatrix.Zoom = new(1, 1);

        // 停止时间流逝
        Main.time = 27000;
    }
    public override bool ChangeAudio()
    {
        // Get rid of the jarring title screen music when moving between subworlds.
        if (Main.gameMenu)
        {
            Main.newMusic = 0;
            return true;
        }

        return false;
    }
    public override void DrawSetup(GameTime gameTime)
    {
        base.DrawSetup(gameTime);
    }
    public override void OnEnter()
    {

        SoundEngine.PlaySound(AssetsLoader.portal_use2);
        base.OnEnter();
    }
}