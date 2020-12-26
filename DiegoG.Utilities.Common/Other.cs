using DiegoG.Utilities.Enumerations;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Collections.Generic;

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

        public static string Format(this string str, params object[] args)
            => string.Format(str, args);
        public static string Format(this string str, IFormatProvider provider, params object[] args)
            => string.Format(provider, str, args);

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
                _ => throw new NotSupportedException(),
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

        public static string RemoveSubstring(this string str, string substring)
        {
            while (str.Contains(substring))
                str = str.Remove(str.IndexOf(substring), substring.Length);
            return str;
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

        /// <summary>
        /// Checks if all of the values within bool[] b are true
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool AllTrue(this bool[] b) => b.All(a => a);
        /// <summary>
        /// Checks if any of the values within bool[] b are true
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool AnyTrue(this bool[] b) => b.Any(a => a);

        /// <summary>
        /// For bool Fields
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Toggle(ref this bool b) { b = !b; return b; }
        /// <summary>
        /// For bool Properties
        /// </summary>
        /// <param name="b"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool Toggle(this bool b, Action<bool> action) { action(!b); return !b; }

        public static bool TryDispose(this IDisposable disposable)
        {
            if (disposable is not null)
                disposable.Dispose();
            else
                return false;
            return true;
        }

        public static MethodInfo GetMethod(this object objectToCheck, string methodName, Type[] parameters)
        {
            var type = objectToCheck.GetType();
            return type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance, null, parameters, null);
        }
        /// <summary>
        /// Only works for public instance methods.
        /// </summary>
        /// <param name="objectToCheck"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static bool HasMethod(this object objectToCheck, string methodName)
            => objectToCheck.HasMethod(methodName, Type.EmptyTypes);
        /// <summary>
        /// Only works for public instance methods.
        /// </summary>
        /// <param name="objectToCheck"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static bool HasMethod(this object objectToCheck, string methodName, params Type[] parameters)
            => objectToCheck.GetMethod(methodName, parameters) is not null;
        /// <summary>
        /// Only works for public instance methods.
        /// </summary>
        /// <param name="objectToCheck"></param>
        /// <param name="methodName"></param>
        /// <param name="returnType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static bool HasMethod(this object objectToCheck, string methodName, Type returnType, params Type[] parameters)
        {
            var mi = objectToCheck.GetMethod(methodName, parameters);
            return mi is not null && mi.ReturnType == returnType;
        }

        public static IEnumerable<int> GetIndexOfMatches(this string[] str, string compare)
        {
            for (int i = 0; i < str.Length; i++)
                if (str[i] == compare)
                    yield return i;
        }
        public static int CountMatches<T>(this IEnumerable<T> e, T match) where T : IEquatable<T>
            => CountMatches(e, match, d => d.Equals(match));
        public static int CountMatches<T>(this IEnumerable<T> e, T match, Func<T,bool> predicate)
        {
            int count = 0;
            foreach (var i in e)
                if (predicate(i))
                    count++;
            return count;
        }
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> e, T match) where T : IEquatable<T>
            => Split(e, (d) => d.Equals(match));
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> e, Func<T, bool> predicate)
        {
            List<T> current = new();
            List<List<T>> list = new() { current };
            foreach(var element in e)
            {
                if (predicate(element))
                {
                    current = new();
                    list.Add(current);
                    continue;
                }
                current.Add(element);
            }
            return list;
        }

        public static IEnumerable<string> ToLower(this IEnumerable<string> strarr)
        {
            var narr = new List<string>(strarr.Count());
            foreach (var s in strarr)
                narr.Add(s.ToLower());
            return narr;
        }
        public static string Flatten(this IEnumerable<string> strarr, string spacing = " ", bool trim = true)
        {
            var rs = "";
            foreach (var s in strarr)
                rs = rs + s + spacing;
            return trim ? rs[0..^0].Trim() : rs[0..^0];
        }

        public static IEnumerable<(TKey, TValue)> GetKVTuple<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            foreach (var k in dict.Keys)
                yield return (k, dict[k]);
        } 

    }
}