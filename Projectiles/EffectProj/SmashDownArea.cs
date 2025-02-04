using Terraria.Audio;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;
using DeadCellsBossFight.Core;
using DeadCellsBossFight.Utils;

namespace DeadCellsBossFight.Projectiles.EffectProj;

public class SmashDownArea : ModProjectile
{
    public override string Texture => AssetsLoader.TransparentImg;

    public override void SetDefaults()
    {
        Projectile.damage = 6;
        Projectile.DamageType = DamageClass.Default;
        Projectile.knockBack = 5.8f;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 2;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.ownerHitCheck = true;
    }
    public override void AI()
    {
        Projectile.width = (int)Projectile.ai[0];
        Projectile.height = (int)Projectile.ai[1];
        base.AI();
        //砸门
        var tilePos = Projectile.Hitbox.TopLeft().ToTileCoordinates16();
        for (int x = tilePos.X; x < tilePos.X + Projectile.width / 16 + 2; x++)
        {
            for (int y = tilePos.Y; y < tilePos.Y + Projectile.height / 16; y++)
            {
                if (Main.tile.TryGet(x, y, out var tile) && tile.TileType == 10)
                {
                    WorldGen.OpenDoor(x, y, (int)Projectile.velocity.X);
                    for (int i = 0; i < 20; i++)//破门粒子效果
                    {
                        int m = Dust.NewDust(new Point16(x, y + Main.rand.Next(-1, 2)).ToVector2() * 16, 8, 14, DustID.WoodFurniture, Projectile.velocity.X * Main.rand.NextFloat(8f, 10f), Scale: Main.rand.NextFloat(0.8f, 1.2f));
                        Main.dust[m].alpha -= i * 6;
                    }
                    SoundEngine.PlaySound(AssetsLoader.door_break);
                }
            }
        }
    }
}
