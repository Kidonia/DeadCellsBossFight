using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Contents.Buffs;

public class Oil : ModBuff
{

    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        Main.buffNoSave[Type] = false;
        Main.pvpBuff[Type] = false;
    }
    public override void Update(Player player, ref int buffIndex)
    {
        for (int i = 0; i < 1 + player.width * player.height / 1200; i++)
        {
            int num2 = Dust.NewDust(player.TopLeft, player.width + Main.rand.Next(-2, 2), player.height + Main.rand.Next(-2, 2), DustID.Ash, 0, 0, 0, default, 1.5f);
            Main.dust[num2].alpha += Main.rand.Next(120, 140);
            Main.dust[num2].velocity.X = 0;
            Main.dust[num2].velocity.Y = 0.1f;
            Main.dust[num2].noGravity = true;
        }
    }
    public override bool ReApply(Player player, int time, int buffIndex)
    {
        if (player.onFire || player.onFire2 || player.onFire3 || player.onFrostBurn || player.onFrostBurn2)
        {
            player.buffTime[buffIndex] = 300;
        }
        else
            player.buffTime[buffIndex] = time;
        return true;
    }

}
