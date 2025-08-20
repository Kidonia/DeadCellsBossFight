
using DeadCellsBossFight.Core;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Projectiles.BasicAnimationProj;

public class DeterminedProj : DC_WeaponAnimation
{
    public override string AnimName => "determined";
    private Dictionary<int, DCAnimPic> WeaponDic = new();
    public override int TotalFrame => WeaponDic.Count;
    public override void SetDefaults()
    {
        WeaponDic = AssetsLoader.BHanimAtlas[AnimName];
        QuickSetDefault(2, 2, 0, DamageClass.Default, 0);
    }


}
