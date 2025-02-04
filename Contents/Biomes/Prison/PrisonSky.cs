using DeadCellsBossFight.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace DeadCellsBossFight.Contents.Biomes.Prison;

public class PrisonSky : DCBasicSky
{
    private Texture2D[] wallTex = new Texture2D[10];
    private Texture2D skeleton;
    private Texture2D window;
    private Texture2D windowGlow;
    private Texture2D WindowElseShadowTry;
    private Texture2D WindowElseShadowTry2;
    private Texture2D shadow2;

    private Texture2D basepic;


    private Texture2D billot;
    private Texture2D chainTile;
    private Texture2D grass;
    private Texture2D homonculusFlush;
    private Texture2D alchemyAlcove;
    private Texture2D groundCells;
    private Texture2D lamppost;
    private Texture2D[] torchFireTex = new Texture2D[14];
    public override void OnLoad()
    {
        string path = "DeadCellsBossFight/Contents/Biomes/Prison/PrisonElements/";
        for (int i = 0; i < 10; i++)
            wallTex[i] = GetTex(path + "WallTex/" + i.ToString());

        skeleton = GetTex(path + "skeletonSpawn");
        billot = GetTex(path + "billot");
        grass = GetTex(path + "grass");
        homonculusFlush = GetTex(path + "homonculusFlush");
        window = GetTex(path + "spawnerWindow");
        windowGlow = GetTex(path + "spawnWindowglow");
        WindowElseShadowTry = GetTex(path + "WindowElseShadowTry");
        WindowElseShadowTry2 = GetTex(path + "WindowElseShadowTry2");



        basepic = GetTex(path + "base");
        chainTile = GetTex(path + "chainTile");
        alchemyAlcove = GetTex(path + "alchemyAlcove");
        groundCells = GetTex(path + "groundCells");
        shadow2 = GetTex(path + "shadow2");
        lamppost = GetTex("DeadCellsBossFight/Contents/Biomes/BlackBridge/BBElements/lamppost");
        for (int i = 0; i < 14; i++)
            torchFireTex[i] = GetTex(path + "TorchFireTex/" + i.ToString());
    }
    public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
    {
        //Main.NewText(Main.maxTilesX);
        // 抽象类重写，别漏了
        base.Draw(spriteBatch, minDepth, maxDepth);
        //Lighting.AddLight(Main.MouseWorld, new Vector3(3, 3, 3));

        float scalebase = 3f;
        for (int i = 0; i < Main.screenWidth; i += 72)
            for (int j = 0; j < Main.screenHeight + 145; j += 72)
            {
                Vector2 pos = new Vector2(i, j - Main.screenPosition.Y + 656);
                Random seekRand = new Random(i + j * 30);
                spriteBatch.Draw(wallTex[seekRand.Next() % 10], pos, null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.None, 0);
            }
        if ( ! DCWorldSystem.ChangeToPrisonSky2)
        {

            spriteBatch.Draw(window, new Vector2(410, 732 - Main.screenPosition.Y), null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.None, 0);
            spriteBatch.Draw(WindowElseShadowTry, new Vector2(600, 776 - Main.screenPosition.Y), null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.None, 0);
    
            spriteBatch.Draw(skeleton, new Vector2(46, 960 - Main.screenPosition.Y), null, Color.White, 0, Vector2.Zero, 2.8f, SpriteEffects.None, 0);
            spriteBatch.Draw(grass, new Vector2(780, 1620 - Main.screenPosition.Y), null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.None, 0);
            spriteBatch.Draw(billot, new Vector2(830, 1420 - Main.screenPosition.Y), null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.None, 0);
            spriteBatch.Draw(homonculusFlush, new Vector2(320, 600 - Main.screenPosition.Y), null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.None, 0);

            // spriteBatch.Draw(AssetsLoader.WhiteDot, NormalUtils.Minus(NormalUtils.screenRectangle, new Rectangle(656, 808, 0, 0)), new Color(16, 48, 92, 100));

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(windowGlow, new Vector2(600, 776 - Main.screenPosition.Y), null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.None, 0);
            spriteBatch.Draw(windowGlow, new Vector2(600, 776 - Main.screenPosition.Y), null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.None, 0);
            
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
        else
        {


            // spriteBatch.Draw(basepic, new Vector2(55, 0), Color.White);
            spriteBatch.Draw(alchemyAlcove, new Vector2(442, 1132 - Main.screenPosition.Y), null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.None, 0);
            spriteBatch.Draw(groundCells, new Vector2(152, 1558 - Main.screenPosition.Y), null, Color.White, 0, Vector2.Zero, scalebase, SpriteEffects.None, 0);

            spriteBatch.Draw(lamppost, new Vector2(300, 1452 - Main.screenPosition.Y), null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.Draw(lamppost, new Vector2(1520, 1452 - Main.screenPosition.Y), null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.Draw(torchFireTex[Main.GameUpdateCount % 28 / 2], new Vector2(352, 1296 - Main.screenPosition.Y), null, Color.White, 0, new Vector2(50, 0), scalebase, SpriteEffects.None, 0);
            spriteBatch.Draw(torchFireTex[Main.GameUpdateCount % 28 / 2], new Vector2(1572, 1296 - Main.screenPosition.Y), null, Color.White, 0, new Vector2(50, 0), scalebase, SpriteEffects.None, 0);

            // Main.NewText(Main.MouseWorld);
            spriteBatch.Draw(shadow2, new Vector2(Main.screenWidth / 2, 656 - Main.screenPosition.Y), null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.Draw(shadow2, new Vector2(0, 656 - Main.screenPosition.Y), null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
/*            spriteBatch.Draw(shadow2, new Vector2(Main.screenWidth / 2, 656 - Main.screenPosition.Y), null, new Color(255, 255, 255, 100), 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.Draw(shadow2, new Vector2(0, 656 - Main.screenPosition.Y), null, new Color(255, 255, 255, 100), 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
*/

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        }
    }

}
