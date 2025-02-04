using DeadCellsBossFight.Contents.GlobalChanges; // => DCPlayer
using DeadCellsBossFight.Core; // AssetsLoader
using DeadCellsBossFight.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Projectiles;

public class YamatoHeldProj : ModProjectile
{
    // Projectile.ai[0] 控制竖直收刀, ai[1]是进入收刀后持续了多长时间（EndAnimTimer）
    public Player Owner => Main.player[Projectile.owner];
    private Item holdingItem => Owner.HeldItem; // 用了很普通的检测手持弹幕，没用你的
    private int yamadoEndIdx = -1; // 收刀的帧图索引
    private bool isBasicAnim; // 正常手持，不是收刀
    public int EndAnimTimer // 进入收刀后开始计时，每帧+1
    {
        get { return (int)Projectile.ai[1]; }
        set { Projectile.ai[1] = value; }
    }
    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 32;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 300;
        Projectile.hide = true;
    }

    public override bool? CanDamage() => false;

    public override bool ShouldUpdatePosition() => false;

    public override bool PreAI()
    {
        // 抄的你的
        bool heldBool2 = holdingItem.type != ModContent.ItemType<Yamato>();
        if (heldBool2 || Owner.mount.Active)
        {
            Projectile.Kill();
            return false;
        }
        Projectile.hide = true; // 更新状态
        Projectile.rotation = (0.6f + DCPlayer.yamadoExtraDrawRotation) * Owner.direction;
        Projectile.Center = Owner.MountedCenter.Floor() + new Vector2(0, Owner.gfxOffY) + new Vector2(0, 5) * Math.Sign(Owner.gravDir);
        Projectile.timeLeft = 2;
        Owner.heldProj = Projectile.whoAmI;


        return true;
    }
    public override void AI()
    {
        Projectile.direction = Owner.direction; // 其实没必要，我direction 都直接用的 Owner.direction
        isBasicAnim = (Projectile.ai[0] == 0); // 见开头
        if (Owner.direction == 1)
        {
            Projectile.hide = false;
        }

        if (!isBasicAnim) // 进入收刀后开始计时，每帧+1
        {
            EndAnimTimer++;
        }
        if (0 < EndAnimTimer && EndAnimTimer < 16) // 旋转15帧，
        {
            // Main.NewText(Projectile.ai[1]);
            DCPlayer.yamadoExtraDrawRotation += 0.21f;
        }
        switch (EndAnimTimer) // 按时间判用哪张图
        {
            case 15: // 0
            case 25: // 1
            case 38: // 2
            case 67: // 3
            case 71: // 4
            case 75: // 5
                yamadoEndIdx++;
                break;
            default:
                break;
        }

        // 抄的你的
        float armRotation = 0.1f;

        if (isBasicAnim)
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (armRotation + 4 * DCPlayer.yamadoExtraDrawRotation) * -Owner.direction * Math.Sign(Owner.gravDir));
        else
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (armRotation + DCPlayer.yamadoExtraDrawRotation) * -Owner.direction * Math.Sign(Owner.gravDir));

        Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, armRotation * -Owner.direction * Math.Sign(Owner.gravDir));

    }
    public override bool PreDraw(ref Color lightColor)
    {
        if(DCPlayer.playerHideDrawTime > 0)
            return false;

        // 抄的你的
        if (isBasicAnim)
        {
            string path = "DeadCellsBossFight/Items/yamato6"; // 基本手持是完全收刀，所以6
            Texture2D value = ModContent.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad).Value;
            Vector2 SpecialDrawPositionOffset = Main.OffsetsPlayerHeadgear[Owner.bodyFrame.Y / Owner.bodyFrame.Height] * Owner.Directions;
            SpecialDrawPositionOffset.Y -= 3;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + SpecialDrawPositionOffset;

            // Main.NewText(Projectile.rotation);
            Main.EntitySpriteDraw(value, drawPos, null, lightColor, Projectile.rotation
                , new Vector2(value.Width + Owner.direction * 3, value.Height) * 0.5f, Projectile.scale
                , Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            return false;
        }

        else // 收刀
        {
            Texture2D daoqiao = ModContent.Request<Texture2D>("DeadCellsBossFight/Items/yamatoN", AssetRequestMode.ImmediateLoad).Value;
            Texture2D value = (yamadoEndIdx == -1) ? ModContent.Request<Texture2D>("DeadCellsBossFight/Items/Yamato", AssetRequestMode.ImmediateLoad).Value : AssetsLoader.yamatoEnd[yamadoEndIdx];

            Vector2 SpecialDrawPositionOffset = Main.OffsetsPlayerHeadgear[Owner.bodyFrame.Y / Owner.bodyFrame.Height] * Owner.Directions
                 + new Vector2(Owner.direction * 5, -3);
            Vector2 drawPos = Projectile.Center - Main.screenPosition + SpecialDrawPositionOffset;


            // Main.NewText(Projectile.rotation);
            Main.EntitySpriteDraw(value, drawPos, null, lightColor, -Projectile.rotation
            , new Vector2(value.Width, value.Height) * 0.5f, Projectile.scale
                , Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

            if(yamadoEndIdx == -1)
            // 0.6084076
                //画刀鞘，旋转角度已算好，因为只旋转15帧，直接NewText出来了，就是Projectile.rotation最后的值
            Main.EntitySpriteDraw(daoqiao, drawPos, null, lightColor, -0.6084076f
           , new Vector2(daoqiao.Width, daoqiao.Height) * 0.5f, Projectile.scale
              , Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

        }
        return false;
    }
}
