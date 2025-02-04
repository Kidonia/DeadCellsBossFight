using DeadCellsBossFight.Assets.Gores;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using DeadCellsBossFight.Core;
using DeadCellsBossFight.Contents.GlobalChanges;

namespace DeadCellsBossFight.NPCs.ExtraBosses;

public abstract class DC_BasicNPC : ModNPC
{
    //ai[3]用于NPC贴图的帧数变化，一定小心！不要用到它。
    public Player player => Main.player[NPC.target];
    public DCPlayer playerHurt => player.GetModPlayer<DCPlayer>();
    public int drawStartOffsetX, drawStartOffsetY;
    public Vector2 drawVec;

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public void ReplayFrame()
    {
        NPC.ai[3] = 0;
    }
    public void DrawNpcTexture(SpriteBatch spriteBatch)
    {

        spriteBatch.Draw(TextureAssets.Npc[Type].Value,
            NPC.Bottom - Main.screenPosition,
            NPC.frame,
            Color.White,
            0f,
            drawVec,
            NPC.scale,
            (SpriteEffects)(-(NPC.direction - 1) / 2),
            0);
    }
    //为什么不用这些函数？因为这些只是壳子，一堆NPC都要特判特写，你有意见？
    public void DrawGlowTexture(Texture2D glowTex, SpriteBatch spriteBatch, Color drawColor)
    {
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

        AssetsLoader.glowEffect.Parameters["InputR"].SetValue(drawColor.R);
        AssetsLoader.glowEffect.Parameters["InputG"].SetValue(drawColor.G / 255f);
        AssetsLoader.glowEffect.Parameters["InputB"].SetValue(drawColor.B / 255f);
        AssetsLoader.glowEffect.CurrentTechnique.Passes["fx"].Apply();
        spriteBatch.Draw(glowTex,
            NPC.Bottom - Main.screenPosition,
            NPC.frame,
            Color.White,
            0f,
            drawVec,
            NPC.scale,
            (SpriteEffects)(-(NPC.direction - 1) / 2),
            0);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
    }

    public void DrawfxTexture(Dictionary<int, DCAnimPic> fxDic, float drawStartOffsetX = 0, float drawStartOffsetY = 0, bool shader = false, Color fxColor = default)
    {
        SpriteBatch sb = Main.spriteBatch;
        int index = (int)NPC.ai[3];
        Rectangle rectangle = new(fxDic[index].x, fxDic[index].y,
                                                    fxDic[index].width, fxDic[index].height);

        Vector2 vector = new Vector2(fxDic[index].originalWidth / 2 * NPC.direction //参考解包图片如果在大图里是如何绘制的
                                                        - fxDic[index].offsetX * NPC.direction
                                                        - (NPC.direction - 1) * fxDic[index].width / 2,//+为0，-为width

                                                         fxDic[index].originalHeight / 2
                                                         - fxDic[index].offsetY)

            + new Vector2(NPC.direction * drawStartOffsetX, drawStartOffsetY);



        sb.End();
        sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

        if (shader)
        {

            AssetsLoader.glowEffect.Parameters["InputR"].SetValue(fxColor.R / 255f);
            AssetsLoader.glowEffect.Parameters["InputG"].SetValue(fxColor.G / 255f);
            AssetsLoader.glowEffect.Parameters["InputB"].SetValue(fxColor.B / 255f);
            AssetsLoader.glowEffect.CurrentTechnique.Passes["fx"].Apply();
        }

        sb.Draw(AssetsLoader.ChooseCorrectAnimPic(fxDic[index].index, fxEnemy: true),
                NPC.Center - Main.screenPosition,
                rectangle,
                Color.White,
                NPC.rotation,
                vector,
                NPC.scale,
                (SpriteEffects)(-(NPC.direction - 1) / 2),
                0f);

        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);


    }
    //报应来了，改得好痛苦
    public void ChooseCorrectFrame(Dictionary<int, DCAnimPic> dic)
    {
        int cframe = (int)NPC.ai[3];
        if (cframe >= dic.Count)
        {
            cframe = 0;
            ReplayFrame();
        }
        //Main.NewText(dic[cframe].index);
        NPC.frame = new(dic[cframe].x, dic[cframe].y, dic[cframe].width, dic[cframe].height);

        drawVec = new Vector2(dic[cframe].originalWidth / 2 * NPC.direction //参考解包图片如果在大图里是如何绘制的
                                                - dic[cframe].offsetX * NPC.direction
                                                - (NPC.direction - 1) * dic[cframe].width / 2,//+为0，-为width

                                                 dic[cframe].originalHeight / 2
                                                 - dic[cframe].offsetY)

    + new Vector2(NPC.direction * drawStartOffsetX, drawStartOffsetY);

        if (Main.GameUpdateCount % 3 == 0)
        {
            NPC.ai[3]++;
        }
        if (NPC.ai[3] == dic.Count)
            ReplayFrame();
    }


    public void AddDCNPCHitEffect(SoundStyle dieSound, int color1ToInt, int color2ToInt = 0)
    {
        if (Main.netMode == NetmodeID.Server)
        {
            return;
        }
        if (NPC.life <= 0)
        {
            SpawnGore(dieSound, color1ToInt, color2ToInt);
        }

    }
    public void SpawnGore(SoundStyle dieSound, int color1, int color2 = 0)
    {
        var entitySource = NPC.GetSource_Death();
        int k = NPC.width * NPC.height / 512;
        for (int i = 0; i < k; i++)
        {
            Vector2 pos = NPC.position + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(0, NPC.height));
            Gore.NewGore(entitySource, pos, new Vector2(0, Main.rand.Next(3, 6)), ModContent.GoreType<Flesh1>(), color1);
            if (color2 > 0)
                Gore.NewGore(entitySource, pos, new Vector2(0, Main.rand.Next(3, 6)), ModContent.GoreType<Flesh2>(), color2);
        }
        SoundEngine.PlaySound(dieSound, NPC.Center);
    }
}
