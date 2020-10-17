using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;

namespace DiegoG.Utilities
{
    public static class Other
    {
        [DllImport("shlwapi.dll")]
        public static extern int ColorHLSToRGB(int H, int L, int S);

        [Obsolete]
        public static SecureString ToSecureString(this string plainString)
        {
            if (plainString == null)
            {
                return null;
            }

            SecureString secureString = new SecureString();
            foreach (char c in plainString.ToCharArray())
            {
                secureString.AppendChar(c);
            }
            return secureString;
        }
        public static string GetColorName(Color c)
        {
            var str = (from KnownColor color in Enum.GetValues(typeof(KnownColor)) where c == Color.FromKnownColor(color) select Convert.ToString(color)).FirstOrDefault();
            if (String.IsNullOrEmpty(str))
                return DiegoGMath.FormatInt(c.ToArgb(), DiegoGMath.IntFormat.Hexadecimal);
            return str;
        }
        public static Color GetOppositeColor(Color c)
        {
            return Color.FromArgb(ColorHLSToRGB((int)((c.GetHue() + 180) % 360), (int)c.GetBrightness(), (int)c.GetSaturation()));
        }
        public static Color GetInverseColor(Color c)
        {
            return Color.FromArgb(int.MaxValue - c.ToArgb());
        }
    }
}
