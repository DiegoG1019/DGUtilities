using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DiegoG.Utilities.Collections
{
    public static class CollectionExtensionMethods
    {
        public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> enumerable) => new ObservableCollection<T>(enumerable);

        /// <summary>
        /// Takes all values until item index <= specified index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static IEnumerable<T> UpToIndex<T>(this IEnumerable<T> enumerable, int index) => enumerable.Select((v, i) => new { v, i }).Where(p => p.i <= index).Select(p => p.v);

        /// <summary>
        /// Takes all values until item index > specified index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static IEnumerable<T> StartingAtIndex<T>(this IEnumerable<T> enumerable, int index) => enumerable.Select((v, i) => new { v, i }).Where(p => p.i > index).Select(p => p.v);

        /// <summary>
        /// BeforeIndex contains all values of index <= specified index; while AfterIndex contains all values of index > specified index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static (IEnumerable<T> BeforeIndex, IEnumerable<T> AtAndAfterIndex) SplitAtIndex<T>(this IEnumerable<T> enumerable, int index) => (enumerable.UpToIndex(index), enumerable.StartingAtIndex(index));

        public static bool TryDequeue<T>(this Queue<T> queue, out T item)
        {
            var test = queue.Count > 0;
            item = default;
            if (test)
            {
                item = queue.Dequeue();
            }

            return test;
        }

        ///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
        ///<param name="enumerable">The enumerable to search.</param>
        ///<param name="predicate">The expression to test the items against.</param>
        ///<returns>The index of the first matching item, or -1 if no items match.</returns>
        public static int FindIndex<T>(this IEnumerable<T> enumerable, Predicate<T> predicate)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            int retVal = 0;
            foreach (var item in enumerable)
            {
                if (predicate(item))
                {
                    return retVal;
                }

                retVal++;
            }
            return -1;
        }
        ///<summary>Finds the index of the first occurrence of an item in an enumerable.</summary>
        ///<param name="enumerable">The enumerable to search.</param>
        ///<param name="item">The item to find.</param>
        ///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
        public static int IndexOf<T>(this IEnumerable<T> enumerable, T item) => enumerable.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i));

        /// <summary>
        /// Deprecated. This method is considered (by me) as poor practice because it's a method used only for its side-effects, and is better used in statement form. However, it can be used to shorten amount of lines where appropriate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var i in enumerable)
            {
                action(i);
            }
        }

        public static IEnumerable<int> GetIndexOfMatches(this string[] str, string compare)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == compare)
                {
                    yield return i;
                }
            }
        }
        public static int CountMatches<T>(this IEnumerable<T> e, T match) where T : IEquatable<T> => CountMatches(e, match, d => d.Equals(match));

        public static int CountMatches<T>(this IEnumerable<T> e, T match, Func<T, bool> predicate)
        {
            int count = 0;
            foreach (var i in e)
            {
                if (predicate(i))
                {
                    count++;
                }
            }

            return count;
        }
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> e, T match) where T : IEquatable<T> => Split(e, (d) => d.Equals(match));

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> e, Func<T, bool> predicate)
        {
            List<T> current = new();
            List<List<T>> list = new() { current };
            foreach (var element in e)
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
            {
                narr.Add(s.ToLower());
            }

            return narr;
        }

        public static IEnumerable<string> MembersToString<T>(this IEnumerable<T> ienum)
        {
            var res = new List<string>(ienum.Count());
            foreach (var i in ienum)
                res.Add(i.ToString());
            return res;
        }

        public static string Flatten(this IEnumerable<string> strarr, string spacing = " ", bool trim = true)
        {
            var rs = "";
            foreach (var s in strarr)
                rs += s + spacing;
            return trim ? rs.Trim() : rs;
        }

        public static IEnumerable<(TKey, TValue)> GetKVTuple<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            foreach (var k in dict.Keys)
            {
                yield return (k, dict[k]);
            }
        }

        public static IEnumerable<T> GetEnumValues<T>() where T : struct, Enum => Enum.GetValues(typeof(T)).Cast<T>();
    }
}
