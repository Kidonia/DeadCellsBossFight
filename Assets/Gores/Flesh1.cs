using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DeadCellsBossFight.Assets.Gores
{
    public class Flesh1 : ModGore
    {
        private int color;
        public override string Texture => "DeadCellsBossFight/Assets/Gores/fxFlesh1";
        public override void SetStaticDefaults()
        {
            GoreID.Sets.DisappearSpeed[Type] = 2;
        }
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            //scale 用于传参
            color = (int)gore.scale;
            gore.scale = Main.rand.NextFloat(1.1f, 1.7f);
            base.OnSpawn(gore, source);
        }
        public override Color? GetAlpha(Gore gore, Color lightColor)
        {
            return DemicalToHexToColor(color);
        }
        private static Color DemicalToHexToColor(int input)
        {
            if (input < 16777215)
            {
                string hex = Convert.ToString(input, 16);
                hex = hex.PadLeft(6, '0'); // 如果不满6位，前面补0
                int r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                int g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                int b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                return new Color(r, g, b);
            }
            else
                return Color.White;
        }
    }
}
