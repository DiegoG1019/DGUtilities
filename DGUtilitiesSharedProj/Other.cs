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
            if (plainString is null)
                return null;
            SecureString secureString = new SecureString();
            foreach (char c in plainString.ToCharArray())
                secureString.AppendChar(c);
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
            => Color.FromArgb(ColorHLSToRGB((int)((c.GetHue() + 180) % 360), (int)c.GetBrightness(), (int)c.GetSaturation()));
        public static Color GetInverseColor(Color c)
            => Color.FromArgb(int.MaxValue - c.ToArgb());
        public static T CapNumber<T>(T number, T min, T max)
            where T : struct, IComparable, IConvertible, IFormattable, IComparable<T>, IEquatable<T>
        {
            if (number.CompareTo(max) > 0)
                return max;
            if (number.CompareTo(min) < 0)
                return min;
            return number;
        }
        public static void Cap<T>(ref this T number, T min, T max) where T : struct, IComparable, IConvertible, IFormattable, IComparable<T>, IEquatable<T> => number = CapNumber(number, min, max);
    }
}
