using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media;

namespace DiegoG.WPF
{
    public static class ColorExtensions
    {
        [DllImport("shlwapi.dll")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible", Justification = "Does not represent a potential security breach")]
        public static extern int ColorHLSToRGB(int H, int L, int S);

        public static Color FromArgbUInt32(uint value)
        {
            var ColorArray = BitConverter.GetBytes(value);
            return Color.FromArgb(ColorArray[3], ColorArray[2], ColorArray[1], ColorArray[0]);
        }
        public static uint ToArgb(this Color c)
            => BitConverter.ToUInt32(new byte[] { c.B, c.G, c.R, c.A });
        public static string GetName(this Color c)
        {
            var str = (from color in ((TypeInfo)typeof(Colors)).GetProperties() where c == (Color)color.GetValue(null) select color.Name).FirstOrDefault();
            if (string.IsNullOrEmpty(str))
                return Convert.ToString(c.ToArgb(), toBase: 16);
            return str;
        }
        public static Color Opposite(this Color c)
            => Colors.White - c;
        public static Color Inverse(this Color c)
            => Colors.Black - c;

        /// <summary>
        /// Setting any of the values to -1 will set them to the color's previous value
        /// </summary>
        /// <param name="A">(-1)-255</param>
        /// <param name="R">(-1)-255</param>
        /// <param name="G">(-1)-255</param>
        /// <param name="B">(-1)-255</param>
        /// <returns>The modified color</returns>
        public static Color Modify(this Color c, short A, short R, short G, short B)
        {
            if (A < 0)
                A = c.A;
            if (R < 0)
                R = c.R;
            if (G < 0)
                G = c.G;
            if (B < 0)
                B = c.B;
            return Color.FromArgb((byte)A, (byte)R, (byte)G, (byte)B);
        }
        /// <summary>
        /// Setting any of the values to -1 will set them to the color's previous value
        /// </summary>
        /// <param name="A">(-1)-255</param>
        /// <returns>The modified color</returns>
        public static Color Modify(this Color c, short A)
            => Modify(c, A, -1, -1, -1);
    }
}
