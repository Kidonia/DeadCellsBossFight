using System.Collections.Generic;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using DeadCellsBossFight.Core;
using Terraria.DataStructures;

namespace DeadCellsBossFight.Projectiles.BasicAnimationProj;

public class RollAProj : DC_WeaponAnimation
{
    public override string AnimName => "rollStart";
    private Dictionary<int, DCAnimPic> WeaponDic = new();
    public override int TotalFrame => WeaponDic.Count;
    public override void SetDefaults()
    {
        WeaponDic = AssetsLoader.BHanimAtlas[AnimName];
        QuickSetDefault(2, 2, 0, DamageClass.Default, 0, slowBeginFrame : 4);
    }
    public override void OnSpawn(IEntitySource source)
    {
        GetBHNPC();
    }
    public override bool? CanDamage()
    {
        return false;
    }
    public override void AI()
    {
        base.AI();
        DrawTheAnimationInAI(0f, 15f);
        PlayWeaponSound(AssetsLoader.roll, 1);
    }
    public override void PostDraw(Color lightColor)
    {
        Projectile.direction = Math.Sign(Projectile.velocity.X);
        SpriteBatch sb = Main.spriteBatch;
        Rectangle rectangle = new(WeaponDic[Projectile.frame].x, WeaponDic[Projectile.frame].y, WeaponDic[Projectile.frame].width, WeaponDic[Projectile.frame].height);
        Vector2 vector = new Vector2(WeaponDic[Projectile.frame].originalWidth / 2 * Projectile.direction //参考解包图片如果在大图里是如何绘制的
                                                        - WeaponDic[Projectile.frame].offsetX * Projectile.direction //
                                                        - (Projectile.direction - 1) * WeaponDic[Projectile.frame].width / 2,//+为0，-为width

                                                         WeaponDic[Projectile.frame].originalHeight / 2
                                                         - WeaponDic[Projectile.frame].offsetY)
            //位置要修改！！！！
            + new Vector2(Projectile.direction, -28);


        sb.Draw(AssetsLoader.ChooseCorrectAnimPic(WeaponDic[Projectile.frame].index, BH: true),
            npc.Center - Main.screenPosition,
            rectangle,
            Color.White,
            Projectile.rotation,
            vector,
            Projectile.scale,
            (SpriteEffects)(Projectile.direction > 0 ? 0 : 1),
            0f);
        if (Projectile.ai[0] == 1 && Main.GameUpdateCount % 2 == 0) // 绘制额外移动的连贯残影们
        {
            DCWorldSystem.onionSkinTrails.Add(new OnionSkinTrail(npc.Center, Projectile.direction, Projectile.frame, Projectile.scale, WeaponDic, rectangle, vector));
        }

        Vector2 vectorhead = new Vector2(WeaponDic[Projectile.frame].originalWidth / 2 * Projectile.direction //参考解包图片如果在大图里是如何绘制的
                                        - WeaponDic[Projectile.frame].headPos[0] * Projectile.direction //
                                        - (Projectile.direction - 1) * AssetsLoader.BHHead.Width / 2,//+为0，-为width

                                         WeaponDic[Projectile.frame].originalHeight / 2
                                         - WeaponDic[Projectile.frame].headPos[1])

                                        + new Vector2(Projectile.direction *  6, -16);
        sb.Draw(AssetsLoader.BHHead,
                      npc.Center - Main.screenPosition,
                      null,
                      Color.White,
                      Projectile.rotation,
                      vectorhead,
                      Projectile.scale,
                      (SpriteEffects)(Projectile.direction > 0 ? 0 : 1),
                      0f);

    }
    public override bool PreKill(int timeLeft)
    {
        Projectile.NewProjectile(Projectile.GetSource_FromAI(), npc.Center, npc.velocity, ModContent.ProjectileType<RollBProj>(), 0, 0, -1, Projectile.ai[0]);
        return true;
    }
}
