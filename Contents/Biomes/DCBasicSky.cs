using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Contents.Biomes;

public abstract class DCBasicSky : CustomSky
{
    public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
    {
        // 禁用玩家放大缩小
        Main.GameViewMatrix.Zoom = new(1, 1);

        if (Main.gameMenu)
            return;

        // Disable ambient sky objects like wyverns and eyes appearing in the background.
        if (skyActive)
        {
            SkyManager.Instance["Ambience"].Deactivate();
            SkyManager.Instance["Slime"].Opacity = -1f;
            SkyManager.Instance["Slime"].Deactivate();
        }
    }
    public bool skyActive;
    public float opacity;

    public override float GetCloudAlpha() => 0f;
    public override void Deactivate(params object[] args)
    {
        skyActive = false;
    }

    public override void Reset()
    {
        skyActive = false;
    }

    public override bool IsActive()
    {
        return skyActive || opacity > 0f;
    }
    public override void Activate(Vector2 position, params object[] args)
    {
        skyActive = true;
    }
    public override void Update(GameTime gameTime)
    {
        if (Main.gameMenu)
            skyActive = false;

        if (skyActive && opacity < 1f)
            opacity += 0.02f;
        else if (!skyActive && opacity > 0f)
            opacity -= 0.02f;
    }

    public Texture2D GetTex(string path)
    {
        return ModContent.Request<Texture2D>(path, (AssetRequestMode)1).Value;
    }
}
