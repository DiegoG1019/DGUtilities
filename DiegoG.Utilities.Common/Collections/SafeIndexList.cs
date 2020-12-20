using System.Collections.Generic;

namespace DiegoG.Utilities.Collections
{
    /// <summary>
    /// Usable only in very specific and controlled scenarios, due to the possibility of false positives and hard to diagnose bugs.
    /// Both the set and get accesors are safe to use with any index >= 0
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SafeIndexList<T> : List<T>, IList<T>, IEnumerable<T>
    {
        /// <summary>
        /// Beware that this simply checks against the default of the given type. For value types, this might result in strange behaviour. Such as a expected "0" returning false
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryGet(int index, out T item)
        {
            item = this[index];
            return index >= Count;
        }

        public bool TrySet(int index, T item)
        {
            this[index] = item;
            return index >= Count;
        }

        new public T this[int index]
        {
            get
            {
                if (index >= Count)
                    return default;
                return base[index];
            }
            set
            {
                if (index >= Count)
                    return;
                base[index] = value;
            }
        }
        public SafeIndexList() : base() { }
        public SafeIndexList(int capacity) : base(capacity) { }
        public SafeIndexList(IEnumerable<T> collection) : base(collection) { }
    }
}
