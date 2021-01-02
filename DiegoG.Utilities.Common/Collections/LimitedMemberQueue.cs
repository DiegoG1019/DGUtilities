using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.Utilities.Collections
{
    /// <summary>
    /// Represents a first in, first out (FIFO) concurrent, thread-safe collection, with a given amount of members
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LimitedMemberQueue<T> : IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>
    {
        protected readonly LinkedList<T> LocalList = new();

        public int Count => ((IReadOnlyCollection<T>)LocalList).Count;
        public bool IsSynchronized => true;
        public bool IsFull => Count >= Capacity;
        
        private object SyncRoot { get; } = new object();

        public void CopyTo(Array array, int index) => ((ICollection)LocalList).CopyTo(array, index);

        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)LocalList).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)LocalList).GetEnumerator();

        public int Capacity
        {
            get
            {
                lock(SyncRoot)
                    return CapacityField;
            }

            set
            {
                lock (SyncRoot)
                {
                    if (value < Capacity)
                        while (Count > value)
                            LocalList.RemoveLast();
                    CapacityField = value;
                }
            }
        }
        private int CapacityField;

        /// <summary>
        /// Enqueues the given item, if Count is larger or equal to Capacity, pushes out the last members
        /// </summary>
        /// <param name="item"></param>
        public void ForceEnqueue(T item)
        {
            lock (SyncRoot)
            {
                while (IsFull)
                    LocalList.RemoveLast();
                LocalList.AddFirst(item);
            }
        }

        /// <summary>
        /// Attemps to Enqueue the given item, if the Queue is full, will return false
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True if enqueueing the item was succesful</returns>
        public bool TryEnqueue(T item)
        {
            lock (SyncRoot)
            {
                if (IsFull)
                    return false;
                LocalList.AddFirst(item);
                return true;
            }
        }
        public T Dequeue()
        {
            lock (SyncRoot)
            {
                var f = LocalList.First.Value;
                LocalList.RemoveFirst();
                return f;
            }
        }
        public T Peek() => LocalList.First.Value;
        public bool TryDequeue([MaybeNullWhen(false)]out T item)
        {
            lock (SyncRoot)
            {
                item = default;
                if (LocalList.Count > 0)
                {
                    item = Dequeue();
                    return true;
                }
                return false;
            }
        }
        public bool TryPeek([MaybeNullWhen(false)]out T item)
        {
            lock (SyncRoot)
            {
                item = default;
                if (LocalList.Count > 0)
                {
                    item = Peek();
                    return true;
                }
                return false;
            }
        }
    }
}
