using System;
using System.Linq;

namespace DiegoG.Utilities
{
    public static class DiegoGMath
    {
        public static double NthRoot(double A, int N) => Math.Pow(A, 1.0 / N);
        public static ulong GreatestCommonDivisor(ulong a, ulong b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }
            return a == 0 ? b : a;
        }
        public static string IntToString(int value, char[] baseChars, IntFormat fm)
        {
            // 32 is the worst cast buffer size for base 2 and int.MaxValue
            int i = 32;
            char[] buffer = new char[i];
            int targetBase = baseChars.Length;

            do
            {
                buffer[--i] = baseChars[value % targetBase];
                value /= targetBase;
            }
            while (value > 0);

            char[] result = new char[32 - i];
            Array.Copy(buffer, i, result, 0, 32 - i);

            string newstring = new string(result);

            for (int b = Digits[(int)fm] - newstring.Length; b > 0; b--)
                newstring += "0";
            return newstring;
        }

        private static readonly byte[] Digits = { 10, 11, 8, 32, 7, 6 };
        public static string FormatInt(int value, IntFormat ifm)
        {
            return ifm switch
            {
                IntFormat.Hexadecimal => value.ToString("X" + Digits[(int)ifm]),
                IntFormat.Decimal => value.ToString("D" + Digits[(int)ifm]),
                IntFormat.Binary => IntToString(value, new char[] { '0', '1' }, ifm),
                IntFormat.Hexavigesimal => IntToString(value, Enumerable.Range('A', 26).Select(x => (char)x).ToArray(), ifm),
                IntFormat.Octal => IntToString(value, new char[] { '0', '1', '2', '3', '4', '5', '6', '7' }, ifm),
                IntFormat.Sexagesimal => IntToString(value, new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x' }, ifm),
                _ => throw new InvalidOperationException("Invalid StringFormat for int"),
            };
        }
        public enum IntFormat
        {
            Decimal, Octal, Hexadecimal, Binary, Hexavigesimal, Sexagesimal
        }
    }
}
