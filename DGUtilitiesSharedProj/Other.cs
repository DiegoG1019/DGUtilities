using DiegoG.Utilities.Enumerations;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Media;

namespace DiegoG.Utilities
{
    public static class Other
    {
        [Obsolete("SecureString itself is obsolete")]
        public static SecureString ToSecureString(this string plainString)
        {
            if (plainString is null)
                return null;
            SecureString secureString = new SecureString();
            foreach (char c in plainString.ToCharArray())
                secureString.AppendChar(c);
            return secureString;
        }

        public static object AutoCast(object number, NumberTypes type)
        {
            return type switch
            {
                NumberTypes.Byte => Convert.ToByte(number),
                NumberTypes.SByte => Convert.ToSByte(number),
                NumberTypes.Int16 => Convert.ToInt16(number),
                NumberTypes.UInt16 => Convert.ToUInt16(number),
                NumberTypes.Int32 => Convert.ToInt32(number),
                NumberTypes.UInt32 => Convert.ToUInt32(number),
                NumberTypes.Int64 => Convert.ToInt64(number),
                NumberTypes.UInt64 => Convert.ToUInt64(number),
                NumberTypes.Half => throw new NotSupportedException(),
                NumberTypes.Single => Convert.ToSingle(number),
                NumberTypes.Double => Convert.ToDouble(number),
                NumberTypes.Decimal => Convert.ToDecimal(number),
                _ => throw new NotImplementedException(),
            };
        }
        public static bool GenericTryParse<T>(string input, out T result)
            where T : struct, IComparable, IConvertible, IFormattable, IComparable<T>, IEquatable<T>
        {
            result = default;
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter == null)
                return false;
            try
            {
                result = (T)converter.ConvertFromString(input);
                return true;
            }
            catch (NotSupportedException)
            {
                return false;
            }
        }
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
