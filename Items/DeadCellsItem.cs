using DeadCellsBossFight.Contents.DamageClasses;
using DeadCellsBossFight.Contents.GlobalChanges;
using DeadCellsBossFight.Contents.SubWorlds;
using DeadCellsBossFight.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DeadCellsBossFight.Items;


public abstract class DeadCellsItem : ModItem
{
    public override string Texture => "DeadCellsBossFight/Assets/cardIcons";
    public Player player => Main.player[Main.myPlayer];
    public DCPlayer playerComboAttack => player.GetModPlayer<DCPlayer>();
    /// <summary>
    /// 图标在cardIcons里的水平位置（每24像素），通过json传入。其被赋值于<see cref="BottleSystem.LoadBottleJSON"/>
    /// </summary>
    public int iconX;
    /// <summary>
    /// 图标在cardIcons里的垂直位置（每24像素），通过json传入。其被赋值于<see cref="BottleSystem.LoadBottleJSON"/>
    /// </summary>
    public int iconY;

    /// <summary>
    /// 物体的流派，包括双卷轴，每个流派（红，紫绿，无色）都有一个，详细另见：<see cref="BottleItemLable"/>。其被赋值于<see cref="BottleSystem.LoadBottleJSON"/>
    /// </summary>
    public int ItemLabel = -1;

    /// <summary>
    /// 存放物体的流派颜色，影响地上光效材质，数值范围0,1,2,3。其被赋值于<see cref="BottleSystem.LoadBottleJSON"/>
    /// </summary>
    public int colorIdx1;

    /// <summary>
    /// 存放物体的流派颜色，数值范围0,1,2,3。其被赋值于<see cref="BottleSystem.LoadBottleJSON"/>
    /// </summary>
    public int colorIdx2;

    public bool DualWeaponBase;
    public bool DualWeaponOffhand;
    public int DualWeaponOffhandItemType;

    public bool IsWeapon = false;
    public bool IsSkill = false;
    public bool IsMutation = false;

    public bool AlphaDrawIconRequired = false;
    public override void OnSpawn(IEntitySource source)
    {
        if(DualWeaponOffhand)
            Main.item[Item.whoAmI].active = false;
        base.OnSpawn(source);
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public virtual void SetWeaponDefaults(DamageClass damageType, int damage, float knockback, int usetime, int useAnimation, int sellpricefromCDB, int useStyle = 1, int crit = 0, int rare = 10, int shoot = 10, float shootSpeed = 1f, int width = 48, int height = 48, bool material = false, bool noMelee = true, bool autoReuse = false)
    {
        IsWeapon = true;
        Item.DamageType = damageType;//流派
        Item.damage = damage;
        Item.knockBack = knockback;
        Item.useTime = usetime;
        Item.useAnimation = useAnimation;
        Item.value = sellpricefromCDB * 100;
        Item.useStyle = useStyle;//1，即 ItemUseStyleID.Swing    剑挥舞

        Item.crit = crit;
        Item.rare = rare;//10，即 ItemRarityID.Red
        Item.shoot = shoot;//10，即 ProjectileID.PurificationPowder   手持弹幕都用这个，没有为什么。
        Item.shootSpeed = shootSpeed;
        Item.width = width;//icon默认48宽
        Item.height = height;//icon默认48高
        
        Item.material = material;
        Item.noMelee = noMelee;
        Item.autoReuse = autoReuse;//自动使用
        Item.noUseGraphic = true;//使用时不展示Icon，肯定
    }
    public virtual void SetSkillDefaults(DamageClass damageType, int useTime, int useAnimation, int sellpricefromCDB, int width = 48, int height = 48, int useStyle = 10, int rare = 10)
    {
        IsSkill = true;
        Item.DamageType = damageType;//流派
        Item.useTime = useTime;
        Item.useAnimation = useAnimation;
        Item.value = sellpricefromCDB * 100;

        Item.width = width;
        Item.height = height;
        Item.useStyle = useStyle;//10，即 ItemUseStyleID.HiddenAnimation
        Item.rare = rare;//10，即 ItemRarityID.Red
        Item.noUseGraphic = true;//使用时不展示Icon，肯定
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////                                           图标部分                                                //////////////    
    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        Texture2D texture = TextureAssets.Item[Item.type].Value;
        Rectangle sourceRectangle = new Rectangle(iconX * 24, iconY * 24, 24, 24);
        origin = new Vector2(12, 12);
        if (AlphaDrawIconRequired)
        {
            Color myColor = new Color(255, 255, 255, 0);
            spriteBatch.Draw(texture, position, sourceRectangle, myColor, 0f, origin, 1f, SpriteEffects.None, 0f);
            return false;
        }
        else
        {
            spriteBatch.Draw(texture, position, sourceRectangle, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
            return false;
        }
    }
    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        float basicScale = 2f;
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        Texture2D texture = TextureAssets.Item[Item.type].Value;
        Rectangle sourceRectangle = new Rectangle(iconX * 24, iconY * 24, 24, 24);
        float yOffset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2) * 4;
        Vector2 drawPos = Item.Bottom - Main.screenPosition + new Vector2(0, yOffset);
        Vector2 origin = new Vector2(12, 24);

        Vector2 haloDrawPosition = Item.Center - Main.screenPosition + new Vector2(0, yOffset) + (Item.Center - Item.Bottom).RotatedBy(rotation) - (Item.Center - Item.Bottom);
        Vector2 haloOrigin = new Vector2(0, 5);
        Vector2 haloOrigin2 = new Vector2(4, 5);

        // 一圈光环
        for (int i = 0; i < 12; i++)
        {

            float haloRotation = (MathHelper.Pi * i) / 6 + Main.GlobalTimeWrappedHourly / 2; // 外圈
            Color haloDrawColor = HaloColorLerp(AssetsLoader.ThreeSectColor[colorIdx1], AssetsLoader.ThreeSectColor[colorIdx2], haloRotation);

            // 使用第一个颜色，未排序的数组确保随机性。
            Texture2D HaloTex = AssetsLoader.HaloTexture[colorIdx1];

            spriteBatch.Draw(HaloTex, haloDrawPosition, null, haloDrawColor, haloRotation, haloOrigin, basicScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(HaloTex, haloDrawPosition, null, haloDrawColor, haloRotation - Main.rand.NextFloat(MathHelper.Pi / 45), haloOrigin, basicScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(HaloTex, haloDrawPosition, null, haloDrawColor, haloRotation - Main.rand.NextFloat(MathHelper.Pi / 30), haloOrigin, basicScale, SpriteEffects.None, 0f);

            float haloRotation2 = haloRotation - MathHelper.Pi / 12; // 内圈

            Color haloDrawColor2 = HaloColorLerp(AssetsLoader.ThreeSectColor[colorIdx1], AssetsLoader.ThreeSectColor[colorIdx2], haloRotation2);
            haloDrawColor2.A /= 2;

            spriteBatch.Draw(HaloTex, haloDrawPosition, null, haloDrawColor2, haloRotation2, haloOrigin2, basicScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(HaloTex, haloDrawPosition, null, haloDrawColor2, haloRotation2 - Main.rand.NextFloat(MathHelper.Pi / 30), haloOrigin2, basicScale, SpriteEffects.None, 0f);
        }

        // 物品
        spriteBatch.Draw(texture, drawPos, sourceRectangle, Color.White, rotation, origin, basicScale, SpriteEffects.None, 0f);

        spriteBatch.End();
        spriteBatch.Begin();
        return false;

    }
    private static Color HaloColorLerp(Color color1, Color color2, float haloRotation)
    {
        if(color2 == color1)
            return color1;
        double value = Math.Abs(Math.Sin(haloRotation / 2));
        if (value < 0.642787640) // Math.Sin(MathHelper.Pi * 40 / 180)
            return color1;
        else if (value > 0.766044446) // Math.Sin(MathHelper.Pi * 50 / 180)
            return color2;
        else
        {
            Color color3 = Color.Lerp(color1, color2, (float)((value - 0.642787640) / 0.123256806));
            return color3;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////
    //                                               吸引物品，受细胞人影响

    public override bool GrabStyle(Player player)
    {
        if(DCWorldSystem.BH_active && SubworldSystem.IsActive<PrisonWorld>())
        {
            NPC beheaded = Main.npc[DCWorldSystem.BH_whoAmI];
            Main.NewText((beheaded.Center - Item.Center).Length());
            Item.velocity = PullItem_Pickup(player, Item, 7f, 1);
            if ((beheaded.Center - Item.Center).Length() < 180 )
            {
                Item.velocity += PullItem_Pickup(beheaded, Item, 2.8f, 1);
            }
            return true;
        }
        return false;
    }
    // 改自源码
    private static Vector2 PullItem_Pickup(Entity entity, Item itemToPickUp, float speed, int acc)
    {
        float num = entity.Center.X - itemToPickUp.Center.X;
        float num2 = entity.Center.Y - itemToPickUp.Center.Y;
        float num3 = (float)Math.Sqrt(num * num + num2 * num2);
        num3 = speed / num3;
        num *= num3;
        num2 *= num3;
        return new Vector2(num, num2);
    }





    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////                                           武器部分                                                //////////////      
    public override bool CanUseItem(Player player)
    {
        //玩家后摇结束，且世界中没有存留上一次攻击
        return player.ownedProjectileCounts[Item.shoot] < 1 && playerComboAttack.WeaponCoolDown == 0 && playerComboAttack.ConsistentLockCtrlAfter == 0;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Main.NewText(type);
        Projectile.NewProjectile(player.GetSource_FromAI(), Main.MouseWorld, Vector2.Zero, type, damage, knockback, -1, 1);
        return false;
    }

    /// <summary>
    /// coolDownTime 武器的冷却时间，该时间过后可再次使用该武器。最后一段攻击使用。攻击间隔清零，攻击段数回退1。
    /// </summary>
    public void FinalComboAttack(int coolDownTime = 0)
    {
        //最终那段攻击后执行
        //攻击间隔清零，攻击段数回退1
        playerComboAttack.TimeCanConsistentAttack = 0;
        playerComboAttack.NextStrikeChainNum = 1;
        playerComboAttack.WeaponCoolDown = coolDownTime;
    }

    /// <summary>
    /// 初始化下一段攻击，包括：① timebetween 初始化玩家两段攻击间隔剩余时间。② lockCtrlBetween 下一段攻击的前摇。
    /// 假设为（60, 14），则表示接下来60帧内可进行第二次攻击，但是14帧以后才能进行。
    /// </summary>
    /// <param name="timebetween"></param>
    /// <param name="lockCtrlBetween"></param>
    public void InitialNextComboAttack(int timebetween, int lockCtrlBetween)
    {
        //初始化下一段攻击，包括：
        //初始化玩家两段攻击间隔剩余时间
        //攻击段数加一，即进行下一段攻击
        //下一段攻击的前摇
        playerComboAttack.TimeCanConsistentAttack = timebetween;//两段攻击间隔剩余时间， 60 == 1秒
        playerComboAttack.ConsistentLockCtrlAfter = lockCtrlBetween;
        playerComboAttack.NextStrikeChainNum++;//段数加一
    }

    /// <summary>
    /// NextAttackChain 下一段攻击是第几段攻击。如果玩家两段攻击间隔剩余时间大于零，且，下一段攻击对得上号（正常都能对的上号），且，间隔冷却时间为零，则返回真
    /// </summary>
    /// <param name="NextAttackChain"></param>
    public bool CanNextAttack(int NextAttackChain)
    {
        //如果玩家两段攻击间隔剩余时间大于零，且，下一段攻击对得上号（正常都能对的上号），且，间隔冷却时间为零，则返回真
        return playerComboAttack.TimeCanConsistentAttack > 0 && playerComboAttack.NextStrikeChainNum == NextAttackChain && playerComboAttack.ConsistentLockCtrlAfter == 0;
    }

    public bool FirstAttack()
    {
        return (playerComboAttack.NextStrikeChainNum == 1);
    }


    /// <summary>
    /// 伤害放大倍数，即伤害为物品基础伤害的对少倍。填的数要大于1，不然伤害会缩小。一定记得写清流派。
    /// </summary>
    /// <param name="mul"></param>
    /// <returns></returns>
    public int DamageMul(float mul = 1f)
    {
        if (Item.DamageType == BrutalityDamage.Instance)
            return (int)player.GetTotalDamage<BrutalityDamage>().ApplyTo(Item.damage * mul);

        if (Item.DamageType == TacticsDamage.Instance)
            return (int)player.GetTotalDamage<TacticsDamage>().ApplyTo(Item.damage * mul);

        if (Item.DamageType == SurvivalDamage.Instance)
            return (int)player.GetTotalDamage<SurvivalDamage>().ApplyTo(Item.damage * mul);

        return (int)(Item.damage * mul);
    }
    //  这是给多流派判断用的
    /*
    public virtual DamageClass CheckDamageClass()
    {
        if(playerScroll.BrutalityNum > playerScroll.TacticsNum)
            if(playerScroll.BrutalityNum > playerScroll.SurvivalNum)
                return BrutalityDamage.Instance;



        return BrutalityDamage.Instance;
    }
    */
    public override void SaveData(TagCompound tag)
    {
        base.SaveData(tag);
    }
    public override void LoadData(TagCompound tag) 
    {
        base.LoadData(tag);
    }
}

