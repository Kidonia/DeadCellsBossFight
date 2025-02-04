using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace DeadCellsBossFight.Core;

public class OnionSkinTrail
{
    public bool active;
    public int timeLeft = 30;
    public Vector2 position;
    public int direction;
    public int frame;
    public float scale;
    public Dictionary<int, DCAnimPic> dic;
    Rectangle onionrect;
    Vector2 onionvect;
    public OnionSkinTrail(Vector2 position, int direction, int frame, float scale, Dictionary<int, DCAnimPic> dic, float drawStartOffsetX, float drawStartOffsetY, int timeLeft = 15)
    {
        this.active = true;
        this.timeLeft = 20;
        this.position = position;
        this.direction = direction;
        this.frame = frame;
        this.scale = scale;
        this.dic = dic;
        this.onionrect = new(dic[frame].x, dic[frame].y, dic[frame].width, dic[frame].height);
        this.onionvect = new Vector2(dic[frame].originalWidth / 2 * direction //参考解包图片如果在大图里是如何绘制的
                                                            - dic[frame].offsetX * direction //
                                                            - (direction - 1) * dic[frame].width / 2,//+为0，-为width

                                                             dic[frame].originalHeight / 2
                                                             - dic[frame].offsetY)
                + new Vector2(direction * drawStartOffsetX, drawStartOffsetY);
    }
    public OnionSkinTrail(Vector2 position, int direction, int frame, float scale, Dictionary<int, DCAnimPic> dic, Rectangle onionrect, Vector2 onionvect, int timeLeft = 15)
    {
        {
            this.active = true;
            this.timeLeft = 20;
            this.position = position;
            this.direction = direction;
            this.frame = frame;
            this.scale = scale;
            this.dic = dic;
            this.onionrect = onionrect;
            this.onionvect = onionvect;
        }
    }
    public void DrawUpdateBHOnionSkinTrail()
    {
        if (this.active == false)
            return;
        if (this.timeLeft > 0)
            this.timeLeft--;
        if(this.timeLeft == 0)
        {
            this.active = false;
            return;
        }
          Main.spriteBatch.Draw(AssetsLoader.ChooseCorrectAnimPic(dic[frame].index, BH : true),
            this.position - Main.screenPosition,
            this.onionrect,
            new Color(255, 237, 19, 150 - 8 * (20 - this.timeLeft)),
            0f,
            this.onionvect,
            this.scale,
            (SpriteEffects)(this.direction > 0 ? 0 : 1),
            0f);
    }


}
