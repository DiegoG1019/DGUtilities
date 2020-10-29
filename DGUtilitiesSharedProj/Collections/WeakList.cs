using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiegoG.Utilities.Collections
{
    public class WeakList<T> : IList<WeakReference<T>>, IEnumerable<WeakReference<T>> where T : class
    {
        private List<WeakReference<T>> list { get; } = new List<WeakReference<T>>();
        public WeakReference<T> this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }
        public int Count => list.Count;
        public bool IsReadOnly => false;
        public void Add(WeakReference<T> item)
        {
            T _;
            if (item.TryGetTarget(out _))
                list.Add(item);
        }
        public void Add(T item) => Add(new WeakReference<T>(item));
        public void Clear() => list.Clear();
        public bool Contains(WeakReference<T> item) => list.Contains(item);
        public void CopyTo(WeakReference<T>[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
        public IEnumerator<WeakReference<T>> GetEnumerator() => list.GetEnumerator();
        public int IndexOf(WeakReference<T> item) => list.IndexOf(item);
        public void Insert(int index, WeakReference<T> item)
        {
            T _;
            if (item.TryGetTarget(out _))
                list.Insert(index, item);
        }
        public bool Remove(WeakReference<T> item) => list.Remove(item);
        public void RemoveAt(int index) => list.RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public async void Clean() => await Task.Run(() =>
        {
            T _;
            var c = from item in this where !item.TryGetTarget(out _) select item;
            foreach(var i in c)
                Remove(i);
        }
        );
    }
}
