using DeadCellsBossFight.Projectiles.EffectProj;
using DeadCellsBossFight.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using DeadCellsBossFight.Utils;
using DeadCellsBossFight.Core;
using DeadCellsBossFight.Projectiles.NPCsProj;
using Terraria.GameContent;
using DeadCellsBossFight.Contents.GlobalChanges;
using System.Collections.Generic;

namespace DeadCellsBossFight;

public class DeadCellsBossFight : Mod
{
    private static DeadCellsBossFight m_instance;
    public static DeadCellsBossFight Instance => m_instance;
    public DeadCellsBossFight()
    {
        m_instance = this;
    }

    public static RenderTarget2D render;
    public static List<Projectile> EffectProj = new List<Projectile>();

    public float twistStrength = 0f;
    public bool shoudTwist;

    public static bool IsDrawingScaledScreen; // 画布最后画Main.screenTargetSwap时如果画了改变大小的，这个设为true，用来避免因为UI不会变化大小的穿帮情况。
                                              // 传给禁用一些UI绘制的用

    public override void Load()
    {
        AssetsLoader.LoadAsset();
        AssetsLoader.LoadSound();

        On_FilterManager.EndCapture += FilterManager_EndCapture;
        Main.OnResolutionChanged += Main_OnResolutionChanged;
    }


    private void Main_OnResolutionChanged(Vector2 obj)
    { 
        render?.Dispose();
        render = null;
        render = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
    }
    // 味儿肘次元斩绝用的
    Vector2[] piecesPos = new Vector2[24];
    float[] piecesRotate = new float[24];
    float[] piecesScale = new float[24];
    Color[] piecesColor = new Color[24];
    private void FilterManager_EndCapture(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
    {
        if (EffectProj.Count > 0 || DCPlayer.TrapDamagedTimer > 0)
        {
            render ??= new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);

            GraphicsDevice gd = Main.instance.GraphicsDevice;
            SpriteBatch sb = Main.spriteBatch;

            gd.SetRenderTarget(Main.screenTargetSwap);
            gd.Clear(Color.Transparent);//用透明清除
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            /*
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active)
                {
                    if (proj.type == ModContent.ProjectileType<MirrorScreenBroken>() && proj.localAI[2] > 0)
                    {
                        if (proj.localAI[2] == 2)
                        {
                            AssetsLoader.filter.Parameters["filterRGB"].SetValue(new Vector3(0f, 0.03f, 0.15f));
                            AssetsLoader.filter.CurrentTechnique.Passes[0].Apply();
                        }
                        // 建议两个错开
                        if (proj.localAI[1] > 0)
                        {
                            AssetsLoader.RadialBlur.Parameters["center"].SetValue(new Vector2(0.5f, 0.5f));
                            AssetsLoader.RadialBlur.Parameters["strength"].SetValue(proj.localAI[1]);
                            AssetsLoader.RadialBlur.CurrentTechnique.Passes[0].Apply();

                        }
                    }
                }
            }
            */
            // ！确保所有弹幕在OnSpawn里都写了        DeadCellsBossFight.EffectProj.Add(Projectile);
            foreach (Projectile proj in EffectProj)
            {
                if (proj.active)
                {
                    if (proj.type == ModContent.ProjectileType<MirrorScreenBroken>() && proj.localAI[2] > 0)
                    {
                        if (proj.localAI[2] == 2)
                        {
                            AssetsLoader.filter.Parameters["filterRGB"].SetValue(new Vector3(0f, 0.03f, 0.15f));
                            AssetsLoader.filter.CurrentTechnique.Passes[0].Apply();
                        }
                        // 建议两个错开
                        if (proj.localAI[1] > 0)
                        {
                            AssetsLoader.RadialBlur.Parameters["center"].SetValue(new Vector2(0.5f, 0.5f));
                            AssetsLoader.RadialBlur.Parameters["strength"].SetValue(proj.localAI[1]);
                            AssetsLoader.RadialBlur.CurrentTechnique.Passes[0].Apply();

                        }
                    }
                }
            }
            sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);

            sb.End();


            gd.SetRenderTarget(render);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.Transform);

            // Main.NewText(hasPlayerMinionProj, Color.Green);

            foreach (Projectile proj in EffectProj)
            {
                if (proj.active)
                {
/*                    //顺手在这检测了，反正能省遍历就省
                    if (startCheckGrappleHookProj && !hasPlayerGrappleHookProj)
                    {
                        if (proj.aiStyle == 7 && proj.ai[0] == 2f)
                        {
                            hasPlayerGrappleHookProj = true;
                        }
                    }
                    if (startCheckMinionProj && !hasPlayerMinionProj)
                    {
                        if (proj.minion || Main.projPet[proj.type])
                        {
                            hasPlayerMinionProj = true;
                        }
                    }*/
                    if (proj.type == ModContent.ProjectileType<RoundTwist>())
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("DeadCellsBossFight/Effects/imgtwist", (AssetRequestMode)1).Value;
                        sb.Draw(texture, proj.position - Main.screenPosition, null, Color.White, 0, texture.Size() / 2, proj.ai[0], SpriteEffects.None, 0);
                        twistStrength = 0.012f;
                        shoudTwist = true;
                    }
                    if (proj.type == ModContent.ProjectileType<RoundTwist2>() || proj.type == ModContent.ProjectileType<RoundTwistQueen>())
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("DeadCellsBossFight/Effects/imgtwist2", (AssetRequestMode)1).Value;
                        sb.Draw(texture, proj.position - Main.screenPosition, null, Color.White, 0, texture.Size() / 2, proj.ai[0], SpriteEffects.None, 0);
                        shoudTwist = true;
                        twistStrength = 0.012f;
                    }
                    if (proj.type == ModContent.ProjectileType<QueenRapierCritCut>())
                    {
                        Texture2D texture = ModContent.Request<Texture2D>(AssetsLoader.WhiteDotImg, (AssetRequestMode)1).Value;
                        sb.Draw(texture, proj.Center + (proj.Left - proj.Center).RotatedBy(proj.rotation) - Main.screenPosition, new Rectangle(0, 0, 1, 1), new(NormalUtils.GetCorrectRadian(proj.rotation), proj.ai[0], 0f), proj.rotation, Vector2.Zero, new Vector2(400, 400), SpriteEffects.None, 0);
                        sb.Draw(texture, proj.Center + (proj.Left - proj.Center).RotatedBy(proj.rotation) - Main.screenPosition, new Rectangle(0, 0, 1, 1), new(NormalUtils.GetCorrectRadian(proj.rotation) + Math.Sign(proj.rotation + 0.001f) * 0.5f, proj.ai[0], 0f), proj.rotation, new Vector2(0, 1), new Vector2(400, 400), SpriteEffects.None, 0);
                        shoudTwist = true;
                        twistStrength = 0.004f;
                    }
                    if (proj.type == ModContent.ProjectileType<QueenSplitScreen>())
                    {
                        sb.End();
                        sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.Transform);
                        Texture2D texture = ModContent.Request<Texture2D>(AssetsLoader.WhiteDotImg, (AssetRequestMode)1).Value;
                        int length = (int)(Math.Sqrt(Main.screenWidth * Main.screenWidth + Main.screenHeight * Main.screenHeight) * 4f);

                        sb.Draw(texture,
                            proj.Center + Vector2.Normalize((proj.Left - proj.Center).RotatedBy(proj.rotation)) * length / 2 - Main.screenPosition,
                            new Rectangle(0, 0, 1, 1),
                            new(NormalUtils.GetCorrectRadian(proj.rotation), proj.ai[0], 0f, 0.2f),
                            proj.rotation,
                            Vector2.Zero,
                            length, SpriteEffects.None, 0);


                        sb.Draw(texture,
                            proj.Center + Vector2.Normalize((proj.Left - proj.Center).RotatedBy(proj.rotation)) * length / 2 - Main.screenPosition,
                            new Rectangle(0, 0, 1, 1),
                            new(NormalUtils.GetCorrectRadian(proj.rotation) + Math.Sign(proj.rotation + 0.001f) * 0.5f, proj.ai[0], 0f, 0.2f),
                            proj.rotation,
                            new Vector2(0, 1),
                            length,
                            SpriteEffects.None, 0);
                        shoudTwist = true;
                        twistStrength = 0.025f;
                    }

                    if (proj.type == ModContent.ProjectileType<MirrorScreenBroken>() && proj.localAI[0] != 3)
                    {
                        if (proj.localAI[0] == 0)
                        {

                            for (int i = 0; i < piecesPos.Length; i++)
                            {
                                piecesPos[i] = Main.LocalPlayer.Center + Main.rand.NextVector2Circular(Main.screenWidth / 2.4f, Main.screenHeight / 2);
                                piecesRotate[i] = Main.rand.NextFloat(0, MathHelper.TwoPi);
                                piecesScale[i] = Main.rand.NextFloat(1f, 3f);
                                piecesColor[i] = new Color(Main.rand.Next(40, 256), Main.rand.Next(40, 256), 0);
                            }
                            proj.localAI[0] = 1;
                        }
                        for (int i = 0; i < piecesPos.Length; i++)
                        {

                            sb.Draw(AssetsLoader.tri, piecesPos[i] - Main.screenPosition, null, piecesColor[i], piecesRotate[i], AssetsLoader.tri.Size() / 2f, piecesScale[i], SpriteEffects.None, 0);
                            if (proj.localAI[0] == 2)
                            {
                                piecesScale[i] = Math.Max(piecesScale[i] - 0.17f, 0f);
                            }
                        }

                        shoudTwist = true;
                        twistStrength = 0.04f;

                    }

                }
            }
            sb.End();


            gd.SetRenderTarget(Main.screenTarget);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            // 开启扭曲shader， 有需要其他的以后再说
            if (shoudTwist)
            {
                AssetsLoader.offsetEffect.Parameters["tex0"].SetValue(render);
                AssetsLoader.offsetEffect.Parameters["i"].SetValue(twistStrength);
                AssetsLoader.offsetEffect.CurrentTechnique.Passes[0].Apply();
            }
            // 正常的屏幕
            sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
            IsDrawingScaledScreen = false;
            // 这个遍历可以理解为最后的PostDraw，除了UI以外的，并且可以对屏幕进行效果添加
            foreach (Projectile proj in EffectProj)
            {
                if (proj.active && proj.damage == 0)
                {
                    if (proj.type == ModContent.ProjectileType<MirrorScreenBroken>())
                    {

                        Vector2 middle = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
                        sb.Draw(AssetsLoader.BlackDot, new Rectangle(0, 0, (int)Main.screenWidth, (int)Main.screenHeight), Color.White);
                        if (proj.ai[1] > 0)
                        {
                            AssetsLoader.screenColorMess.Parameters["offsetStrength"].SetValue(proj.ai[1]);
                            AssetsLoader.screenColorMess.CurrentTechnique.Passes[0].Apply();
                        }

                        sb.Draw(Main.screenTargetSwap, middle, null, Color.White, 0, middle, proj.ai[0], SpriteEffects.None, 0);
                        IsDrawingScaledScreen = true;
                        sb.End();

                        sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
                        sb.Draw(AssetsLoader.BlackDot, new Rectangle(0, 0, (int)Main.screenWidth, (int)Main.screenHeight), new Color(42, 73, 120, (int)proj.ai[2]));
                        if (proj.localAI[2] > 0 && proj.localAI[2] < 2)
                            sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, (int)Main.screenWidth, (int)Main.screenHeight), new Color(191, 172, 255) * proj.localAI[2]);

                    }


                    if (proj.type == ModContent.ProjectileType<TeleportScreenScale>())
                    {
                        Vector2 middle = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
                        AssetsLoader.screenColorMess.Parameters["offsetStrength"].SetValue(proj.ai[1]);
                        AssetsLoader.screenColorMess.CurrentTechnique.Passes[0].Apply();
                        sb.Draw(Main.screenTargetSwap, middle, null, Color.White, 0, middle, proj.ai[0], SpriteEffects.None, 0);
                        IsDrawingScaledScreen = true;
                    }

                    if (proj.type == ModContent.ProjectileType<TeleportWhiteScreen>())
                    {
                        Texture2D texture = ModContent.Request<Texture2D>(AssetsLoader.WhiteDotImg, (AssetRequestMode)1).Value;
                        sb.End();
                        sb.Begin(SpriteSortMode.Immediate, BlendState.Additive);
                        sb.Draw(texture, Vector2.Zero, null, new Color(255, 255, 255, (int)proj.ai[0]), 0, Vector2.Zero, new Vector2(Main.screenWidth, Main.screenHeight), SpriteEffects.None, 0);
                    }

                    if (proj.type == ModContent.ProjectileType<DCScreenDrug>())
                    {
                        // 禁用放大，会卡爆
                        Main.GameViewMatrix.Zoom = new(1, 1);
                        sb.End();
                        sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                        AssetsLoader.drugEffect.Parameters["t"].SetValue(Main.GlobalTimeWrappedHourly);
                        AssetsLoader.drugEffect.Parameters["strength"].SetValue(proj.ai[0]);
                        AssetsLoader.drugEffect.CurrentTechnique.Passes[0].Apply();
                        float lerp = MathHelper.Lerp(1f, 2f, (1440f - proj.timeLeft) / 1440f);
                        Main.musicFade[Main.curMusic] = 2.2f - lerp;
                        sb.Draw(Main.screenTargetSwap, new Vector2(Main.screenWidth, Main.screenHeight) / 2f, null, Color.White, 0f, new Vector2(Main.screenWidth, Main.screenHeight) / 2f, lerp, SpriteEffects.None, 0);
                        IsDrawingScaledScreen = true;
                        sb.End();
                        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                        sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, (int)Main.screenWidth, (int)Main.screenHeight), null, Main.DiscoColor * proj.ai[0] * 4);

                        sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, (int)Main.screenWidth, (int)Main.screenHeight), null, new Color(0, 0, 0, proj.ai[1]));

                    }
                    // 屏幕女王次元斩效果展示
                    /*
                    if (proj.type == ModContent.ProjectileType<QueenSplitScreen>() && proj.active)
                    {
                        sb.End();
                        sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.Transform);



                        Texture2D texture = ModContent.Request<Texture2D>(AssetsLoader.WhiteDotImg, (AssetRequestMode)1).Value;
                        int length = (int)(Math.Sqrt(Main.screenWidth * Main.screenWidth + Main.screenHeight * Main.screenHeight) * 4f);
                        sb.Draw(texture,
                            proj.Center + Vector2.Normalize((proj.Left - proj.Center).RotatedBy(proj.rotation)) * length / 2 - Main.screenPosition,
                            new(0, 0, 1, 1),
                            new(NormalUtils.GetCorrectRadian(proj.rotation), proj.ai[0], 0f, 0.2f),
                            proj.rotation,
                            Vector2.Zero,
                            new Vector2(length, length), SpriteEffects.None, 0);
                        sb.Draw(texture,
                            proj.Center + Vector2.Normalize((proj.Left - proj.Center).RotatedBy(proj.rotation)) * length / 2 - Main.screenPosition,
                            new(0, 0, 1, 1),
                            new(NormalUtils.GetCorrectRadian(proj.rotation) + Math.Sign(proj.rotation + 0.001f) * 0.5f, proj.ai[0], 0f, 0.2f),
                            proj.rotation,
                            new(0, 1),
                            new Vector2(length, length),
                            SpriteEffects.None, 0);
                    }
                    */
                    if (proj.type == ModContent.ProjectileType<DCScreenFire>())
                    {
                        // 禁用放大，会卡爆
                        Main.GameViewMatrix.Zoom = new(1, 1);

                        float k = proj.ai[0];
                        sb.End();
                        sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
                        AssetsLoader.Fire.Parameters["aspectRatio"].SetValue(Main.screenWidth / Main.screenHeight);
                        AssetsLoader.Fire.Parameters["fireWidth"].SetValue(1.6f);
                        AssetsLoader.Fire.Parameters["iTime"].SetValue(Main.GlobalTimeWrappedHourly);
                        AssetsLoader.Fire.Parameters["fireStrength"].SetValue(MathHelper.Lerp(2, 0.5f, k));
                        AssetsLoader.Fire.Parameters["fireStrength2"].SetValue(MathHelper.Lerp(0.7f, 1f, k));
                        AssetsLoader.Fire.Parameters["fireLight"].SetValue(MathHelper.Lerp(1.2f, 1f, k));
                        AssetsLoader.Fire.Parameters["sunken"].SetValue(3f / 20f * (float)Math.Sin(5 * Main.GlobalTimeWrappedHourly) + 3.05f);
                        AssetsLoader.Fire.Parameters["broken"].SetValue(1f / 10f * (float)Math.Sin(4 * Main.GlobalTimeWrappedHourly) + 0.4f);
                        AssetsLoader.Fire.Parameters["twist"].SetValue(1f / 25f * (float)Math.Sin(3 * Main.GlobalTimeWrappedHourly) + 0.28f);
                        AssetsLoader.Fire.Parameters["disappear"].SetValue(MathHelper.Lerp(-0.9f, -4f, proj.ai[1]));

                        AssetsLoader.Fire.CurrentTechnique.Passes[0].Apply();

                        //sb.Draw(blackTex, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipVertically, 0);
                        sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), null, Color.Black, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0);
                        sb.End();
                        sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
                        sb.Draw(AssetsLoader.HugeSpark, new Vector2(Main.screenWidth / 2, Main.screenHeight - 230) + Main.rand.NextVector2Circular(4, 4) * (800 - proj.timeLeft) / 200, null, new Color(1, 1, 1, proj.ai[0] - proj.ai[1] * 6), 0, AssetsLoader.HugeSpark.Size() / 2, MathHelper.Clamp(proj.ai[0] * 1.7f, 0, 1f) - proj.ai[1] * 3f, SpriteEffects.None, 0);
                        IsDrawingScaledScreen = true;
                    }
                    if (proj.type == ModContent.ProjectileType<DCScreenTerrifySentence>())
                    {

                        sb.End();
                        sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
                        AssetsLoader.ScreenFault.Parameters["Speed"].SetValue(11f + (300 - proj.timeLeft) / 60);
                        AssetsLoader.ScreenFault.Parameters["BlockSize"].SetValue(7f + (300 - proj.timeLeft) / 120);
                        AssetsLoader.ScreenFault.Parameters["iTime"].SetValue(Main.GlobalTimeWrappedHourly);
                        AssetsLoader.ScreenFault.CurrentTechnique.Passes[0].Apply();
                        IsDrawingScaledScreen = true;
                        sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                    }



                }
            }



            if (DCPlayer.TrapDamagedTimer > 0)
            {
                sb.End();
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                if (DCPlayer.TrapDamagedTimer > 30)
                {
                    float f = Math.Min(1f, (float)Math.Sin((DCPlayer.TrapDamagedTimer / 25f * 3.1415926f)) * 1.35f) * 0.35f;
                    sb.Draw(TextureAssets.MagicPixel.Value, Vector2.Zero, new Rectangle?(new Rectangle(0, 0, 2500, 1200)), Color.Red * f, 0f, Vector2.Zero, 1f, 0, 0f);
                }

                if (DCPlayer.TrapDamagedTimer < 30)
                {
                    float f = Math.Min(1f, (float)Math.Sin((DCPlayer.TrapDamagedTimer / 60f * 3.1415926f) - 0.48f) * 1.25f);
                    sb.Draw(TextureAssets.MagicPixel.Value, Vector2.Zero, new Rectangle?(new Rectangle(0, 0, 2500, 1200)), Color.Black * f, 0f, Vector2.Zero, 1f, 0, 0f);
                }
            }
            /*
    sb.End();

    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
    float strength = 10f / (1490f - 3 * (Main.GameUpdateCount % 1443) / 3);
    if (strength < 0.005f) strength = 0.005f;
    Main.NewText(strength);
    Main.NewText(Main.GameUpdateCount );
    //strength = 0.02f;
    AssetsLoader.drugEffect.Parameters["t"].SetValue(Main.GlobalTimeWrappedHourly);
    AssetsLoader.drugEffect.Parameters["strength"].SetValue(strength);
    AssetsLoader.drugEffect.CurrentTechnique.Passes[0].Apply();
    sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
    sb.End();
    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

    sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, (int)Main.screenWidth, (int)Main.screenHeight), null, Main.DiscoColor * strength * 4);

    */

            //sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, (int)Main.screenWidth, (int)Main.screenHeight), new Color(255, 255, 160, 30));

            sb.End();
            if (EffectProj.Count > 0)
            {
                EffectProj.RemoveAll(proj => proj.active == false);
            }
        }

        orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);

    }
    public override void Unload()
    {
        m_instance = null;
        AssetsLoader.UnloadAsset();

        On_FilterManager.EndCapture -= FilterManager_EndCapture;
        Main.OnResolutionChanged -= Main_OnResolutionChanged;
        base.Unload();
    }

}