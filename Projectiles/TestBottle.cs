using DeadCellsBossFight.Core;
using DeadCellsBossFight.Utils;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Projectiles;

public class TestBottle : ModProjectile
{
    public List<Rope_Point> rp1 = new List<Rope_Point>();
    public List<Rope_Line> rl1 = new List<Rope_Line>();
    public Player player => Main.player[Projectile.owner];
    public override string Texture => AssetsLoader.TransparentImg;
    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 360;
        base.SetDefaults();
    }
    public override void AI()
    {
        base.AI();
    }
    public override void PostDraw(Color lightColor)
    {
        base.PostDraw(lightColor);
    }
    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
    }
}
