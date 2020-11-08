using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DiegoG.Utilities.Collections
{
    public class LimitedMemberList<T> : IList<T>
    {
        private T[] Array { get; set; }
        public int Capacity { get; private set; }
        public T this[int index]
        {
            get => Array[index];
            set
            {
                if (value == null || value.Equals(default))
                    Count--;
                if (IsEmpty(index))
                    Count++;
                Array[index] = value;
            }
        }

        public LimitedMemberList(int capacity) => Capacity = capacity;
        public int Count { get; private set; }

        public bool IsFull => Count < Capacity;
        public bool IsReadOnly => false;

        protected void ThrowIfFull() { if (IsFull) { throw new InvalidOperationException("This LimitedMemberList is full"); } }
        protected bool IsEmpty(int index) => this[index] == null || this[index].Equals(default);

        public void Add(T item)
        {
            ThrowIfFull();
            Array[Count] = item;
            Count++;
        }

        public void Clear()
        {
            Array = new T[Capacity];
            Count = 0;
        }

        public bool Contains(T item) => IndexOf(item) >= 0;

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (arrayIndex >= Capacity)
                throw new ArgumentOutOfRangeException($"Index = {arrayIndex}; Capacity = {Capacity}");
            System.Array.Copy(Array, 0, array, arrayIndex, Capacity);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var i in Array)
                yield return i;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int IndexOf(T item) => Array.FindIndex((o) => o.Equals(item));

        public void Insert(int index, T item)
        {
            ThrowIfFull();
            for (int i = index; i < Capacity - 1; i++)
                this[i + 1] = this[i];
            this[index] = item;
            Count++;
        }

        public bool Remove(T item)
        {
            int indexfoundat;
            for (int i = 0; i < Capacity; i++)
            {
                if (this[i].Equals(item))
                {
                    this[i] = default;
                    indexfoundat = i;
                    goto Found;
                }
            }
            return false;
            Found:;
            for (int i = indexfoundat; i < Capacity - 1; i++)
                this[i] = this[i + 1];
            Count--;
            this[Capacity - 1] = default;
            return true;
        }

        public void RemoveAt(int index)
        {
            this[index] = default;
            for (int i = index; i < Capacity - 1; i++)
                this[i] = this[i + 1];
            Count--;
            this[Capacity - 1] = default;
        }

    }
}
