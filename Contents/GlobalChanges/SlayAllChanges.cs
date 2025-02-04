using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using DeadCellsBossFight.Projectiles.EffectProj;
using Microsoft.Xna.Framework.Graphics;
using DeadCellsBossFight.Projectiles;

namespace DeadCellsBossFight.Contents.GlobalChanges;
// 时停，也是抄的（）

public class TimeStopPlayer : ModPlayer
{
    public override void PostUpdate()
    {
        if (TimeFrozen)
        {
            Player.creativeGodMode = true;
            Main.dayRate = 0.0;
            Main.time -= 1.0;
            for (int i = 0; i < Player.buffType.Length; i++)
            {
                int buffType = Player.buffType[i];
                if (buffType != 0 && Main.debuff[buffType])
                {
                    Player.buffTime[i] = 0;
                    Player.DelBuff(i);
                }
            }
        }
    }
    public bool TimeFrozen;
}
public class TimeStoppedNPC : GlobalNPC
{
    public override bool InstancePerEntity
    {
        get
        {
            return true;
        }
    }

    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
    {
        if (Main.LocalPlayer.GetModPlayer<TimeStopPlayer>().TimeFrozen)
        {
            spawnRate = 0;
            maxSpawns = 0;
        }
    }

    public override bool PreAI(NPC npc)
    {
        if (Main.LocalPlayer.GetModPlayer<TimeStopPlayer>().TimeFrozen)
        {
            npc.position = npc.oldPosition;
            npc.direction = npc.oldDirection;
            npc.velocity = Vector2.Zero;
            npc.frameCounter = 0.0;
            npc.aiAction = 0;
            npc.timeLeft++;
            return false;
        }
        return true;
    }

    public override void AI(NPC npc)
    {
        if (Main.LocalPlayer.GetModPlayer<TimeStopPlayer>().TimeFrozen)
        {
            npc.position = npc.oldPosition;
            npc.direction = npc.oldDirection;
            npc.velocity = Vector2.Zero;
            npc.frameCounter = 0.0;
            npc.aiAction = 0;
            npc.timeLeft++;
            return;
        }
    }

    public override void PostAI(NPC npc)
    {
        if (Main.LocalPlayer.GetModPlayer<TimeStopPlayer>().TimeFrozen)
        {
            npc.position = npc.oldPosition;
            npc.direction = npc.oldDirection;
            npc.velocity = Vector2.Zero;
            npc.frameCounter = 0.0;
            npc.aiAction = 0;
            npc.timeLeft++;
            return;
        }
    }
    public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
    {
        return !target.GetModPlayer<TimeStopPlayer>().TimeFrozen && base.CanHitPlayer(npc, target, ref cooldownSlot);
    }

    // 以下为屏幕缩放时不绘制这些UI，因为它们不会跟着缩放，不想用Main.hideUI的时候用这些

    public override void ChatBubblePosition(NPC npc, ref Vector2 position, ref SpriteEffects spriteEffects)
    {
        if (DeadCellsBossFight.IsDrawingScaledScreen)
            position = new Vector2(-200, -200);
        base.ChatBubblePosition(npc, ref position, ref spriteEffects);
    }
    public override void EmoteBubblePosition(NPC npc, ref Vector2 position, ref SpriteEffects spriteEffects)
    {
        if (DeadCellsBossFight.IsDrawingScaledScreen)
            position = new Vector2(-200, -200);
        base.EmoteBubblePosition(npc, ref position, ref spriteEffects);
    }
    public override bool? DrawHealthBar(NPC npc, byte hbPosition, ref float scale, ref Vector2 position)
    {
        if(DeadCellsBossFight.IsDrawingScaledScreen)
            return false;
        return base.DrawHealthBar(npc, hbPosition, ref scale, ref position); 
    }

}



public class TimeStoppedProjectile : GlobalProjectile
{
    public override bool InstancePerEntity
    {
        get
        {
            return true;
        }
    }
    public override bool PreAI(Projectile projectile)
    {
        if (Main.LocalPlayer.GetModPlayer<TimeStopPlayer>().TimeFrozen &&
            !Main.projPet[projectile.type] &&
            !projectile.minion &&
            !Main.projHook[projectile.type] &&
            projectile.type != 61 &&
            projectile.type != ModContent.ProjectileType<MirrorScreenBroken>() && // 让用到的弹幕不受影响
            projectile.type != ModContent.ProjectileType<Roundtry>() &&
            projectile.type != ModContent.ProjectileType<Linetry>() &&
            projectile.type != ModContent.ProjectileType<YamatoHeldProj>()
            )
        {
            int num = delayCounter; // 抄的，不是我写的，反正就是判断
            delayCounter = num + 1;
            if (num >= 1)
            {
                projectile.position = projectile.oldPosition;
                projectile.timeLeft++;
                return false;
            }
        }
        else
        {
            delayCounter = 0;
        }
        return true;

    }
    public override bool? CanHitNPC(Projectile projectile, NPC target)
    {
        if (Main.LocalPlayer.GetModPlayer<TimeStopPlayer>().TimeFrozen)
        {
            return new bool?(false);
        }
        return base.CanHitNPC(projectile, target);
    }
    private int delayCounter;
}

