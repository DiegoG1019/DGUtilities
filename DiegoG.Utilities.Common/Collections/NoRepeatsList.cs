using System;
using System.Collections;
using System.Collections.Generic;

namespace DiegoG.Utilities.Collections
{
    public class NoRepeatsList<T> : IList<T>
    {
        protected bool IsStruct { get; private set; }
        private List<T> Internal { get; set; }
        public T this[int index]
        {
            get => Internal[index];
            set
            {
                //If it's not simply setting over the same value, it checks if it already contains and sets if it doesn't throw.
                //If it's simply setting over the same value, it skips the check (and sets it just because)
                if (Contains(value) && index == IndexOf(value))
                    goto Set;
                ThrowIfRepeat(value);
            Set:;
                Internal[index] = value;
            }
        }

        public int Count => Internal.Count;
        public bool IsReadOnly => false;

        protected void ThrowIfRepeat(T item) { if (Contains(item)) { throw new InvalidOperationException("This list already contains this item"); } }

        public void Add(T item)
        {
            ThrowIfRepeat(item);
            Internal.Add(item);
        }
        public void Clear() => Internal.Clear();
        public bool Contains(T item) => Internal.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => Internal.CopyTo(array, arrayIndex);
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var i in this)
                yield return i;
        }

        public int IndexOf(T item) => Internal.IndexOf(item);
        public virtual void Insert(int index, T item)
        {
            ThrowIfRepeat(item);
            Internal.Insert(index, item);
        }
        public bool Remove(T item) => Internal.Remove(item);
        public void RemoveAt(int index) => Internal.RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
