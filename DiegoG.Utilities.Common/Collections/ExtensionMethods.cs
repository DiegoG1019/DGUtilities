using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DiegoG.Utilities.Collections
{
    public static class ExtensionMethods
    {
        public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> enumerable)
            => new ObservableCollection<T>(enumerable);
        /// <summary>
        /// Takes all values until item index <= specified index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static IEnumerable<T> UpToIndex<T>(this IEnumerable<T> enumerable, int index)
            => enumerable.Select((v, i) => new { v, i }).Where(p => p.i <= index).Select(p => p.v);
        /// <summary>
        /// Takes all values until item index > specified index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static IEnumerable<T> StartingAtIndex<T>(this IEnumerable<T> enumerable, int index)
            => enumerable.Select((v, i) => new { v, i }).Where(p => p.i > index).Select(p => p.v);

        /// <summary>
        /// BeforeIndex contains all values of index <= specified index; while AfterIndex contains all values of index > specified index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static (IEnumerable<T> BeforeIndex, IEnumerable<T> AfterIndex) SplitAtIndex<T>(this IEnumerable<T> enumerable, int index)
            => (enumerable.UpToIndex(index), enumerable.StartingAtIndex(index));

        public static bool TryDequeue<T>(this Queue<T> queue, out T item)
        {
            var test = queue.Count > 0;
            item = default;
            if (test)
                item = queue.Dequeue();
            return test;
        }

        ///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
        ///<param name="enumerable">The enumerable to search.</param>
        ///<param name="predicate">The expression to test the items against.</param>
        ///<returns>The index of the first matching item, or -1 if no items match.</returns>
        public static int FindIndex<T>(this IEnumerable<T> enumerable, Predicate<T> predicate)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            int retVal = 0;
            foreach (var item in enumerable)
            {
                if (predicate(item)) return retVal;
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
                action(i);
        }
    }
}
