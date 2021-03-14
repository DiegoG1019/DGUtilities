using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;
using System.Timers;

namespace DiegoG.Utilities
{
    public static class Other
    {
        [Obsolete("SecureString itself is obsolete")]
        public static SecureString ToSecureString(this string plainString)
        {
            if (plainString is null)
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

        public static bool GenericTryParse<T>(string input, out T result)
            where T : struct, IComparable, IConvertible, IFormattable, IComparable<T>, IEquatable<T>
        {
            result = default;
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter == null)
            {
                return false;
            }

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
            {
                str = str.Remove(str.IndexOf(substring), substring.Length);
            }

            return str;
        }

        public static void Deconstruct(this Rectangle rect, out int x, out int y, out int width, out int height) 
        { 
            x = rect.X;
            y = rect.Y;
            width = rect.Width;
            height = rect.Height;
        }

        public static T CapNumber<T>(T number, T min, T max)
            where T : struct, IComparable, IConvertible, IFormattable, IComparable<T>, IEquatable<T> 
            => number.CompareTo(max) > 0 ? max : number.CompareTo(min) < 0 ? min : number;
        public static T Cap<T>(ref this T number, T min, T max) where T : struct, IComparable, IConvertible, IFormattable, IComparable<T>, IEquatable<T>
        {
            number = CapNumber(number, min, max);
            return number;
        }
        public static T Cap<T>(ref this T number, (T min, T max) mm) where T : struct, IComparable, IConvertible, IFormattable, IComparable<T>, IEquatable<T> 
            => number.Cap(mm.min, mm.max);

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

        /// <summary>
        /// Inverts the value of the given bool, and returns the previous one
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool CheckToggle(ref this bool b)
        {
            if (b)
            {
                b = false;
                return true;
            }
            b = true;
            return false;
        }

        /// <summary>
        /// Sets b to true, then returns its previous value
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool CheckSetTrue(ref this bool b)
        {
            var a = b;
            b = true;
            return a;
        }

        /// <summary>
        /// Sets b to false, then returns its previous value
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool CheckSetFalse(ref this bool b)
        {
            var a = b;
            b = false;
            return a;
        }

        public static bool TryDispose(this IDisposable disposable)
        {
            if (disposable is not null)
            {
                disposable.Dispose();
            }
            else
            {
                return false;
            }

            return true;
        }

        public static void ThrowIfNull(params (object obj, string name)[] objs)
        {
            foreach (var (obj, name) in objs)
            {
                if (obj is null)
                {
                    throw new ArgumentNullException(name);
                }
            }
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
        public static bool HasMethod(this object objectToCheck, string methodName) => objectToCheck.HasMethod(methodName, Type.EmptyTypes);

        /// <summary>
        /// Only works for public instance methods.
        /// </summary>
        /// <param name="objectToCheck"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static bool HasMethod(this object objectToCheck, string methodName, params Type[] parameters) => objectToCheck.GetMethod(methodName, parameters) is not null;

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


        public static void Reset(this Timer timer) { timer.Stop(); timer.Start(); }

        public static object GetProperty(object instance, Type thistype, params string[] address)
        {
            foreach (string s in address)
            {
                var prop = thistype.GetProperty(s);
                if (prop is null)
                {
                    throw new ArgumentOutOfRangeException($"{thistype.Name} does not contain a property named {s}.");
                }

                thistype = prop.PropertyType;
                instance = prop.GetValue(instance);
            }
            return instance;
        }

        public static Stream GetEmbeddedResource(string resource)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
            return stream is not null ? stream : throw new FileNotFoundException("Unable to find embedded resource", resource);
        }
        public static bool TryGetEmbeddedResource(string resource, [MaybeNullWhen(false)]out Stream stream)
        {
#if DEBUG
            var s = Assembly.GetExecutingAssembly().GetManifestResourceNames(); //This retrieves a list that can be seen in local variable watch
#endif
            stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
            return stream is not null;
        }

        public static async Task AwaitWithTimeout(this Task task, int timeout, Action onSuccess = null, Action ifError = null)
        {
            if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
                onSuccess?.Invoke();
            else
                ifError?.Invoke();
        }
        public static async Task<T> AwaitWithTimeout<T>(this Task<T> task, int timeout, Action onSuccess = null, Action ifError = null)
        {
            bool success;
            if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {
                onSuccess?.Invoke();
                success = true;
            }
            else
            {
                ifError?.Invoke();
                success = false;    
            }
            return success ? task.Result : default;
        }
        public static IEnumerable<string> Split(this string str, Func<char, bool> controller)
        {
            int nextPiece = 0;

            for (int c = 0; c < str.Length; c++)
            {
                if (controller(str[c]))
                {
                    yield return str[nextPiece..c];
                    nextPiece = c + 1;
                }
            }
            yield return str[nextPiece..];
        }
        public static string TrimMatchingQuotes(this string input, char quote) 
            => input.Length >= 2 && input[0] == quote && input[^1] == quote ? input[1..^1] : input;

        /// <summary>
        /// Outputs a substring of the input that starts at the first instance of 'start' and ends at the first instance of 'end' after 'start'
        /// </summary>
        /// <param name="input"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="trimkeys">Set to true to remove both 'start' and 'end' from the beggining and end of the string respectively</param>
        /// <returns></returns>
        public static string MatchSubstring(this string input, char start, char end, bool trimkeys = false)
        {
            if (input.Length < 2)
                throw new ArgumentException("input string must be longer than 2 characters", nameof(input));
            if (trimkeys && input.Length < 3)
                throw new ArgumentException("input string must be longer than 3 characters, because trimkeys is set to true", nameof(input));

            int indexstart = input.IndexOf(start);
            int indexend = input.IndexOf(end, indexstart + 1);
            return indexstart == -1
                ? throw new KeyNotFoundException($"Could not find '{nameof(start)}' ({start}) in the given string")
                : indexend == -1
                ? throw new KeyNotFoundException($"Could not find '{nameof(end)}' ({end}) in the given string")
                : trimkeys
                ? input[(indexstart + 1)..(indexend - 1)]
                : input[indexstart..indexend];
        }
        /// <summary>
        /// Outputs a substring of the input that starts at the first instance of 'start' and ends at the first instance of 'end' after 'start'
        /// </summary>
        /// <param name="input"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="trimkeys">Set to true to remove both 'start' and 'end' from the beggining and end of the string respectively</param>
        /// <returns></returns>
        public static string MatchSubstring(this string input, string start, string end, bool trimkeys = false)
        {
            if (input.Length < 2)
                throw new ArgumentException("input string must be longer than 2 characters", nameof(input));
            if (trimkeys && input.Length < 3)
                throw new ArgumentException("input string must be longer than 3 characters, because trimkeys is set to true", nameof(input));

            int indexstart = input.IndexOf(start);
            int indexend = input.IndexOf(end, indexstart + 1);
            return indexstart == -1
                ? throw new KeyNotFoundException($"Could not find '{nameof(start)}' ({start}) in the given string")
                : indexend == -1
                ? throw new KeyNotFoundException($"Could not find '{nameof(end)}' ({end}) in the given string")
                : trimkeys
                ? input[(indexstart + start.Length)..(indexend - end.Length)]
                : input[indexstart..indexend];
        }

        public static string TruncateString(this string input, int max, string replacement = "...") 
            => input.Length <= max ? input : input[..(max - replacement.Length)] + replacement;
    }
}