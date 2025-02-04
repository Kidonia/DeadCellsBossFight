using DeadCellsBossFight.Contents.Biomes;
using DeadCellsBossFight.Contents.GlobalChanges;
using DeadCellsBossFight.Contents.SubWorlds;
using DeadCellsBossFight.Core;
using DeadCellsBossFight.Utils;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Projectiles.EffectProj;

// 刚喝药出现的头晕炫彩效果
public class DCScreenDrug : ModProjectile
{
    public override string Texture => AssetsLoader.TransparentImg;
    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 1440;

    }
    public override void OnSpawn(IEntitySource source)
    {
        if (NormalUtils.FindFirstProjectile(Projectile.type) != Projectile.whoAmI || SubworldSystem.AnyActive())
        {
            Projectile.active = false;
            return;
        }
        var dcplayer = Main.LocalPlayer.GetModPlayer<DCPlayer>();
        dcplayer.drugTime = 1360;
        for(int i = 0; i < Player.MaxBuffs; i++)
        {
            if (Main.LocalPlayer.buffType[i] < 1 || Main.LocalPlayer.buffTime[i] < 1)
                continue;
            else
            {
                Main.LocalPlayer.buffType[i] = 0;
                Main.LocalPlayer.buffTime[i] = 0;
            }
        }
        Main.LocalPlayer.AddBuff(BuffID.Confused, 600);
        Main.LocalPlayer.AddBuff(BuffID.Suffocation , 600);
        DeadCellsBossFight.EffectProj.Add(Projectile);
        base.OnSpawn(source);
    }
    public override void AI()
    {
        if (SubworldSystem.AnyActive())
        {
            Projectile.active = false;
        }

        Projectile.ai[0] = 10f / (1490f - 3 * ((1443 - Projectile.timeLeft) % 1443) / 3);
        if (Projectile.timeLeft < 40)
            Projectile.ai[1] += 0.03f;

    }
    public override void OnKill(int timeLeft)
    {
        ////
        var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<TeleportScreenScale>(), 0, 0, -1, Projectile.direction);
        proj.timeLeft = 29;
        proj.localAI[0] = 1;


        Main.GameViewMatrix.Zoom = new(1, 1);
        for (int i = 0; i < Player.MaxBuffs; i++)
        {
            if (Main.LocalPlayer.buffType[i] < 1 || Main.LocalPlayer.buffTime[i] < 1)
                continue;
            else
            {
                Main.LocalPlayer.buffType[i] = 0;
                Main.LocalPlayer.buffTime[i] = 0;
            }
        }
    }
}
