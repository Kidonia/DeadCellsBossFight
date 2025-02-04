using DeadCellsBossFight.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace DeadCellsBossFight.Core;

public class DialogueBox
{
    public DialogueBox() { }


    public DialogueBox(string Text, Vector2 centerPosition, Texture2D boxTexture, int extraDrawLength, Color textColor, Color UIcolor, DynamicSpriteFont font = null)
    {
        active = true;
        texture = boxTexture;
        TextToShow = Text;
        CenterPos = centerPosition;
        ExtraDrawLength = extraDrawLength;
        TextColor = textColor;
        UIColor = UIcolor;
        if (font != null)
            Font = font;
        else
            Font = FontAssets.MouseText.Value;
        TextSize = ChatManager.GetStringSize(Font, NormalUtils.InsertInEachRangePixel(Text, Font), Vector2.One, 0f);
        timeLeft = TextToShow.Length * 3 + 30;
    }

    public bool active = false;
    public int timeLeft = 0;

    /// <summary>
    /// 对话框样式
    /// </summary>
    public Texture2D texture = TextureAssets.MagicPixel.Value;

    /// <summary>
    /// 要展示的话
    /// </summary>
    public string TextToShow = "";

    /// <summary>
    /// 当前这句话展示的进度
    /// </summary>
    public string TextShowing = "";

    public int ExtraDrawLength = 0;
    public Color TextColor = Color.White;
    public Color UIColor = Color.White;

    public DynamicSpriteFont Font = FontAssets.MouseText.Value;
    /// <summary>
    /// 记录文本变长时最后一个字符，当为'.'时要变慢
    /// </summary>
    private char lastWord;
    //计时器，爱来自DDMod
    private int TL = 0;
    private int TLTime = 0;

    public Vector2 TextSize = Vector2.Zero;
    public Vector2 CenterPos = Vector2.Zero;

    public int NewDialogueBoxInArray(DialogueBox[] boxarray, string Text, Vector2 centerPosition, Texture2D boxTexture, int extraDrawLength, Color TextColor, Color UIcolor, DynamicSpriteFont font = null)
    {
        for (int i = 0; i < boxarray.Length; i++)
        {
            if (boxarray[i] == null)
                boxarray[i] = new DialogueBox();
            else if (boxarray[i].active)
                continue;
            else
            {
                boxarray[i] = new DialogueBox(Text, centerPosition, boxTexture, extraDrawLength, TextColor, UIcolor, font);
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 写在每帧更新的函数里
    /// </summary>
    /// <param name="dialogueBox"></param>
    public static void UpdateDialogueBox(DialogueBox dialogueBox, Vector2 centerpos)
    {
        dialogueBox.CenterPos = centerpos;
        if (dialogueBox == null || dialogueBox.active == false) return;
        else
        {
            dialogueBox.timeLeft--;
            if (dialogueBox.timeLeft <= 0)
            {
                dialogueBox.active = false;
                return;
            }

            string Text = dialogueBox.TextToShow;

            if (Text.Length > dialogueBox.TL)
            {
                dialogueBox.TLTime++;
                // if (TLTime % 5 == 0)
                if (dialogueBox.lastWord == '.')
                {
                    if (dialogueBox.TLTime % 16 == 0)
                    {
                        dialogueBox.TL++;
                        dialogueBox.timeLeft += 15;
                    }
                }
                else
                    if (dialogueBox.TLTime % 3 == 0)
                {
                    dialogueBox.TL++;
                }
            }
            else
            {
                if (Text.Length < dialogueBox.TL)
                {
                    dialogueBox.TL = 0;
                    dialogueBox.TLTime = 0;
                }
            }
            Text = Text.Remove(dialogueBox.TL);
            if (Text.Length > 0)
            {

                dialogueBox.lastWord = Text.Last();
                dialogueBox.TextShowing = NormalUtils.InsertInEachRangePixel(Text, dialogueBox.Font);

            }
        }
    }

    public void DrawDialogueBox(SpriteBatch spriteBatch)
    {
        if (active == false) return;
        else
        {
            //DrawDCDialogueTextUI(spriteBatch, texture, FontAssets.MouseText.Value, TextShowing, CenterPos, TextSize, ExtraDrawLength, TextColor, UIColor, 2f, true);
            DrawDCDialogueTextUI(spriteBatch, texture, Font, TextShowing, CenterPos, TextSize, ExtraDrawLength, TextColor, UIColor, 2f, true);
        }
    }
    public static void DrawDialogueBox(DialogueBox dialogueBox, SpriteBatch spriteBatch, int extraDrawLength, Color TextColor, Color UIcolor, bool shadow = true)
    {
        if (dialogueBox == null || dialogueBox.active == false) return;
        else
            DrawDCDialogueTextUI(spriteBatch, dialogueBox.texture, dialogueBox.Font, dialogueBox.TextShowing, dialogueBox.CenterPos, dialogueBox.TextSize, extraDrawLength, TextColor, UIcolor, 2f, shadow);

    }
    /// <summary>
    /// 绘制死亡细胞风格收藏家对话框
    /// </summary>
    /// <param name="spriteBatch">sb</param>
    /// <param name="texture">对话框材质</param>
    /// <param name="font">字体，一般为FontAssets.MouseText.Value</param>
    /// <param name="Text">文本内容，注意在细胞中是不断变长的一句话</param>
    /// <param name="middleCenter">对话框的中心位置，要记得减去Main.screenPosition</param>
    /// <param name="textLineSize">整段文本的大小，因为文本在不断变化，大小应在一开始获得一次并传入</param>
    /// <param name="extraDrawLength">四个边中间部分额外多画一些，避免不必要的裂缝</param>
    /// <param name="UIscale">！默认2f，不然会有错误，但我不想改辣</param>h
    public static void DrawDCDialogueTextUI(SpriteBatch spriteBatch, Texture2D texture, DynamicSpriteFont font, string Text, Vector2 middleCenter, Vector2 textLineSize, int extraDrawLength, Color TextColor, Color UIcolor, float UIscale = 2f, bool shadow = true)
    {
        
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        
        int cornerSize = texture.Width / 2;
        spriteBatch.Draw(texture, middleCenter - textLineSize / 2f + new Vector2(-texture.Width, -texture.Height), new Rectangle(0, 0, cornerSize, cornerSize), UIcolor, 0f, Vector2.Zero, UIscale, SpriteEffects.None, 0);
        spriteBatch.Draw(texture, middleCenter - textLineSize / 2f + new Vector2(-texture.Width, textLineSize.Y + 1), new Rectangle(0, cornerSize + 1, cornerSize, cornerSize), UIcolor, 0f, Vector2.Zero, UIscale, SpriteEffects.None, 0);
        spriteBatch.Draw(texture, middleCenter - textLineSize / 2f + new Vector2(textLineSize.X + 1, -texture.Height), new Rectangle(cornerSize + 1, 0, cornerSize, cornerSize), UIcolor, 0f, Vector2.Zero, UIscale, SpriteEffects.None, 0);
        spriteBatch.Draw(texture, middleCenter - textLineSize / 2f + new Vector2(textLineSize.X + 1, textLineSize.Y + 1), new Rectangle(cornerSize + 1, cornerSize + 1, cornerSize, cornerSize), UIcolor, 0f, Vector2.Zero, UIscale, SpriteEffects.None, 0);

        spriteBatch.Draw(texture, NormalUtils.CreateRectangleFromVectors(middleCenter - textLineSize / 2f + new Vector2(-extraDrawLength, -texture.Height), (int)textLineSize.X + 2 * extraDrawLength, texture.Height), new Rectangle(cornerSize, 0, 1, cornerSize), UIcolor, 0f, Vector2.Zero, SpriteEffects.None, 0);
        spriteBatch.Draw(texture, NormalUtils.CreateRectangleFromVectors(middleCenter + new Vector2(-textLineSize.X, textLineSize.Y) / 2f + new Vector2(-extraDrawLength, 0), (int)textLineSize.X + 2 * extraDrawLength, texture.Height), new Rectangle(cornerSize, cornerSize + 1, 1, cornerSize), UIcolor, 0f, Vector2.Zero, SpriteEffects.None, 0);
        spriteBatch.Draw(texture, NormalUtils.CreateRectangleFromVectors(middleCenter - textLineSize / 2f + new Vector2(-texture.Width, -extraDrawLength), texture.Width, (int)textLineSize.Y + 2 * extraDrawLength), new Rectangle(0, cornerSize, cornerSize, 1), UIcolor, 0f, Vector2.Zero, SpriteEffects.None, 0);
        spriteBatch.Draw(texture, NormalUtils.CreateRectangleFromVectors(middleCenter + new Vector2(textLineSize.X, -textLineSize.Y) / 2f + new Vector2(0, -extraDrawLength), texture.Width, (int)textLineSize.Y + 2 * extraDrawLength), new Rectangle(cornerSize + 1, cornerSize, cornerSize, 1), UIcolor, 0f, Vector2.Zero, SpriteEffects.None, 0);

        spriteBatch.Draw(texture, NormalUtils.CreateRectangleFromVectors(middleCenter - textLineSize / 2f, (int)textLineSize.X, (int)textLineSize.Y), new Rectangle(cornerSize, cornerSize, 1, 1), UIcolor, 0f, Vector2.Zero, SpriteEffects.None, 0);
        if (Text != "")
        {
            if (shadow)
                spriteBatch.DrawString(font, Text, middleCenter + new Vector2(1f, 1f), new(20, 20, 20), 0f, textLineSize / 2f, 1f, SpriteEffects.None, 32);
            spriteBatch.DrawString(font, Text, middleCenter, TextColor, 0f, textLineSize / 2f, 1f, SpriteEffects.None, 0);
        }
        /*
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
        */
    }

}