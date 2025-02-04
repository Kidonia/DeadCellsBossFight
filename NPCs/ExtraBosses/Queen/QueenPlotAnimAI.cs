using Terraria;
using Terraria.ModLoader;

namespace DeadCellsBossFight.NPCs.ExtraBosses.Queen;

public partial class Queen : ModNPC
{
    //ai[2]用于存储当前进行的是第几段剧情。
    // -1战斗，0背身等待玩家，1背身转向举剑，2举剑面对玩家，3举剑转向伸剑，4伸剑指着玩家，5伸剑转向准备攻击
    // 6 站立到下跪，7下跪循环
    public bool IsDealingPlot;
    public bool hasDoneIntroPlot;
    public void QNAI_IntroPlot()
    {
        NPC.direction = -1;
        IsDealingPlot = true;


        handProj.ai[2] = 0;//不绘制手烟
        /*
        Main.blockInput = true;
        player.velocity /= 2;
        */

        ChooseCorrectFrame1Loop((QueenAnims)(44 + NPC.ai[2]));
        /*
        switch(NPC.ai[2])
        {
            case 0:
                ChooseCorrectFrame1Loop(QueenAnims.introLoop0);
                break;
            case 1:

             default:
                break;
        }
        */

    }
    public bool NowDie;
    public void QNAI_IntroDeath()
    {
        NPC.noGravity = false;
        NPC.direction = ForceDirection;
        NPC.velocity *= 0;
        if (NPC.ai[2] != 7) // 跪地循环
            IsDealingPlot = true;
        else
        {
            // 要添加死亡条件
            if (Main.rand.NextBool(60))
            {
                // Main.NewText("die");
                NowDie = true;
                NPC.StrikeInstantKill();
            }
        }

        handProj.ai[2] = 0;//不绘制手烟

        ChooseCorrectFrame1Loop((QueenAnims)(44 + NPC.ai[2]));


        ImmuneTime = 2;
    }

}
