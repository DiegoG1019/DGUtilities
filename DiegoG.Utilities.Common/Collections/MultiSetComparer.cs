using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.Utilities.Collections
{
    public class MultiSetComparer<T> : IEqualityComparer<IEnumerable<T>>
    {
        private readonly IEqualityComparer<T> m_comparer;
        public MultiSetComparer(IEqualityComparer<T> comparer = null) => m_comparer = comparer ?? EqualityComparer<T>.Default;

        public bool Equals(IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first == null)
                return second == null;

            if (second == null)
                return false;

            if (ReferenceEquals(first, second))
                return true;

            if (first is ICollection<T> firstCollection && second is ICollection<T> secondCollection)
            {
                if (firstCollection.Count != secondCollection.Count)
                    return false;

                if (firstCollection.Count == 0)
                    return true;
            }

            return !HaveMismatchedElement(first, second);
        }

        private bool HaveMismatchedElement(IEnumerable<T> first, IEnumerable<T> second)
        {
            var firstElementCounts = GetElementCounts(first, out int firstNullCount);
            var secondElementCounts = GetElementCounts(second, out int secondNullCount);

            if (firstNullCount != secondNullCount || firstElementCounts.Count != secondElementCounts.Count)
                return true;

            foreach (var kvp in firstElementCounts)
            {
                var firstElementCount = kvp.Value;
                secondElementCounts.TryGetValue(kvp.Key, out int secondElementCount);

                if (firstElementCount != secondElementCount)
                    return true;
            }

            return false;
        }

        private Dictionary<T, int> GetElementCounts(IEnumerable<T> enumerable, out int nullCount)
        {
            var dictionary = new Dictionary<T, int>(m_comparer);
            nullCount = 0;

            foreach (T element in enumerable)
            {
                if (element == null)
                    nullCount++;
                else
                {
                    dictionary.TryGetValue(element, out int num);
                    num++;
                    dictionary[element] = num;
                }
            }
            return dictionary;
        }

        public int GetHashCode(IEnumerable<T> enumerable)
        {
            if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));

            int hash = 17;

            foreach (T val in enumerable.OrderBy(x => x))
                hash = hash * 23 + (val?.GetHashCode() ?? 42);

            return hash;
        }
    }
}
