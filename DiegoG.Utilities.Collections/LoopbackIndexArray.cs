using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.Utilities.Collections
{
    public class LoopbackIndexArray<T> : IEnumerable, ICollection, IEnumerable<T>
    {
        public int Count => Array.Length;
        public bool IsSynchronized => Array.IsSynchronized;
        public object SyncRoot => Array.SyncRoot;
        public void CopyTo(Array array, int index) => Array.CopyTo(array, index);
        public void CopyTo(LoopbackIndexArray<T> other, int index) => Array.CopyTo(other.Array, index);
        public IEnumerator GetEnumerator() => Array.GetEnumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)Array).GetEnumerator();

        public int MinIndex => 0;
        public int MaxIndex => Count - 1;

        int _index = -1;
        /// <summary>
        /// If, at any point, the index is more 
        /// </summary>
        public int Index
        {
            get => _index;
            set
            {
                while (value < 0)
                    value += MaxIndex;
                while (value > MaxIndex)
                    value -= (MaxIndex - 1);
                _index = value;
            }
        }

        public int NonDefault => _SetIndexes.Count;

        public T this[int index] { 

            get 
            {
                if (Index > MaxIndex)
                    throw new ArgumentOutOfRangeException(nameof(index), Index, $"The given index was Outside of Range for {MaxIndex} and 0");
                Index = index;
                return Array[Index];
            }
            internal set
            {
                if (Index > MaxIndex)
                    throw new ArgumentOutOfRangeException(nameof(index), Index, $"The given index was Outside of Range for {MaxIndex} and 0");
                Index = index;
                Array[Index] = value;
            }
        }

        public T Next() => this[Index++];
        public T Previous() => this[Index--];
        public void SetNext(T item) => Set(Index++, item);
        public void SetPrevious(T item) => Set(Index--, item);

        private void Set(int index, T item)
        {
            this[index] = item;
            _SetIndexes.Add(index);
        }

        private T[] Array { get; init; }
        public IEnumerable<int> SetIndexes => _SetIndexes;
        private List<int> _SetIndexes { get; } = new();

        public LoopbackIndexArray(int size) => Array = new T[size];
    }
}
