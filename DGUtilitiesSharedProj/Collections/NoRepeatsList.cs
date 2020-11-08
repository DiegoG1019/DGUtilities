using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DiegoG.Utilities.Collections
{
   public class NoRepeatsList<T> : IList<T>
    {
        protected bool IsStruct { get; private set; }
        private List<T> @internal { get; set; }
        public T this[int index]
        {
            get => @internal[index];
            set
            {
                //If it's not simply setting over the same value, it checks if it already contains and sets if it doesn't throw.
                //If it's simply setting over the same value, it skips the check (and sets it just because)
                if (Contains(value) && index == IndexOf(value))
                    goto Set;
                ThrowIfRepeat(value);
                Set:;
                @internal[index] = value;
            }
        }

        public int Count => @internal.Count;
        public bool IsReadOnly => false;

        protected void ThrowIfRepeat(T item) { if (Contains(item)){ throw new InvalidOperationException("This list already contains this item"); }}

        public void Add(T item)
        {
            ThrowIfRepeat(item);
            @internal.Add(item);
        }
        public void Clear() => @internal.Clear();
        public bool Contains(T item) => @internal.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => @internal.CopyTo(array, arrayIndex);
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var i in this)
                yield return i;
        }

        public int IndexOf(T item) => @internal.IndexOf(item);
        public virtual void Insert(int index, T item)
        {
            ThrowIfRepeat(item);
            @internal.Insert(index, item);
        }
        public bool Remove(T item) => @internal.Remove(item);
        public void RemoveAt(int index) => @internal.RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
