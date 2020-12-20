using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DiegoG.Utilities.Collections
{
    public class WeakList<T> : IList<WeakReference<T>>, IEnumerable<WeakReference<T>> where T : class
    {
        private List<WeakReference<T>> List { get; } = new List<WeakReference<T>>();
        public WeakReference<T> this[int index]
        {
            get => List[index];
            set => List[index] = value;
        }
        public int Count => List.Count;
        public bool IsReadOnly => false;

        private bool AutoCleanField = false;
        public bool AutoClean
        {
            get => AutoCleanField;
            set
            {
                if (AutoCleanField == value)
                    return;
                AutoCleanField = value;
                if (value)
                {
                    AutoCleanTask = Task.Run
                        (
                            async () =>
                            {
                                lock (this)
                                {
                                    Clean();
                                }
                                await Task.Delay(AutoCleanTimer);
                            }
                        , AutoCleanCancellationToken.Token);
                    return;
                }
                if (!value)
                    AutoCleanCancellationToken.Cancel();
            }
        }
        /// <summary>
        /// In Milliseconds, defaults to 60 000 (1 minute)
        /// </summary>
        public int AutoCleanTimer { get; set; } = 60_000;

        private CancellationTokenSource AutoCleanCancellationToken { get; set; } = new CancellationTokenSource();
        private Task AutoCleanTask { get; set; }

        public void Add(WeakReference<T> item)
        {
            T _;
            if (item.TryGetTarget(out _))
                List.Add(item);
        }
        public void Add(T item) => Add(new WeakReference<T>(item));
        public void Clear() => List.Clear();
        public bool Contains(WeakReference<T> item) => List.Contains(item);
        public bool Contains(T item)
        {
            return List.Find
            (
                d =>
                {
                    if (d.TryGetTarget(out T target))
                        return target.Equals(item);
                    return false;
                }
            ) != null;
        }
        public void CopyTo(WeakReference<T>[] array, int arrayIndex) => List.CopyTo(array, arrayIndex);
        public IEnumerator<WeakReference<T>> GetEnumerator() => List.GetEnumerator();
        public int IndexOf(WeakReference<T> item) => List.IndexOf(item);
        public int IndexOf(T item)
        {
            return List.FindIndex
            (
                d =>
                {
                    if (d.TryGetTarget(out T target))
                        return target.Equals(item);
                    return false;
                }
            );
        }
        public void Insert(int index, WeakReference<T> item)
        {
            T _;
            if (item.TryGetTarget(out _))
                List.Insert(index, item);
        }
        public void Insert(int index, T item) => Insert(index, new WeakReference<T>(item));
        public bool Remove(WeakReference<T> item) => List.Remove(item);
        public bool Remove(T item)
        {
            if (Contains(item))
            {
                RemoveAt(IndexOf(item));
                return true;
            }
            return false;
        }
        public void RemoveAt(int index) => List.RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public async void Clean() => await Task.Run(() =>
        {
            T _;
            var c = from item in this where !item.TryGetTarget(out _) select item;
            foreach (var i in c)
                Remove(i);
        }
        );
    }
}
