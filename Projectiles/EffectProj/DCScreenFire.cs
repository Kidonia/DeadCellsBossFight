using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using DeadCellsBossFight.Core;
using DeadCellsBossFight.Contents.SubWorlds;
using SubworldLibrary;
using DeadCellsBossFight.Contents.GlobalChanges;
using Terraria.Audio;

namespace DeadCellsBossFight.Projectiles.EffectProj;

public class DCScreenFire : ModProjectile
{
    public override string Texture => AssetsLoader.TransparentImg;
    public override void SetDefaults()
    {
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.friendly = true;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = 200;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        base.SetDefaults();
    }
    public override void OnSpawn(IEntitySource source)
    {
        // 隐藏UI
        Main.hideUI = true;
        Main.blockInput = true;
        DeadCellsBossFight.EffectProj.Add(Projectile);
        base.OnSpawn(source);
    }
    public override void AI()
    {
        /*
        int timeBegin = 200 - Projectile.timeLeft;
        if(timeBegin > 50) timeBegin = 50;
        Projectile.ai[0] = timeBegin / 50f;
        */
        // 极小声播放BGM
        Main.musicFade[Main.curMusic] = 0.02f;
        Projectile.ai[0] = MathHelper.Clamp((200 - Projectile.timeLeft) / 50f, 0, 1f);
        if (Projectile.timeLeft < 50)
            Projectile.ai[1] = (50 - Projectile.timeLeft) / 50f;

        /*
        if (Main.audioSystem is LegacyAudioSystem MusicSystem)
        {
            //MusicSystem.AudioTracks[Main.curMusic].SetVariable("Volume", -0.01f * ((50 - Projectile.timeLeft)));
            MusicSystem.AudioTracks[2].Resume();
        }
        */


    }
    public override void OnKill(int timeLeft)
    {
        // 不再隐藏UI
        Main.hideUI = false;

        
        if (SubworldSystem.IsActive<QueenArenaWorld>())
            SubworldSystem.Exit();
        else
            SubworldSystem.Enter<QueenArenaWorld>();
        

        // 可以接受键盘输入
        Main.blockInput = false;
        //不再限制大小缩放
        var system = ModContent.GetInstance<DCWorldSystem>();
        system.PreventZoomChange = false;

        base.OnKill(timeLeft);
    }
}
