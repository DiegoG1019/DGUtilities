using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiegoG.Utilities
{
    /// <summary>
    /// Provides a collection of tracked items where each one is timer-tracked. Cleaning needs to be done manually with <see cref="Clean"/>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class TemporaryCache<TKey, TValue> : IDictionary<TKey, TValue> where TKey : notnull
    {
        private readonly ConcurrentDictionary<TKey, TrackedValue> Store_ = new();
        private ConcurrentDictionary<TKey, TrackedValue> Store
        {
            get 
            {
                bool lockWasTaken = false;
                Monitor.Enter(Store_, ref lockWasTaken);
                if (lockWasTaken)
                    Monitor.Exit(Store_);
                return Store_;
            }
        }

        public TimeSpan Timeout { get; init; }

        public TValue this[TKey key]
        {
            get
            {
                var x = Store[key];
                x.Added = DateTime.Now;
                return x.Value;
            }
            set => Store[key] = new(value, DateTime.Now);
        }

        public ICollection<TKey> Keys => Store.Keys;

        public ICollection<TValue> Values => Store.Values.Select(s => s.Value).ToArray();

        public int Count => Store.Count;

        public bool IsReadOnly => false;

        public virtual void Clean()
        {
            var l = new List<TKey>(Store.Count);
            foreach (var i in Store)
                if (DateTime.Now >= Store[i.Key].Added + Timeout)
                    l.Add(i.Key);
            foreach (var k in l)
                Store.TryRemove(k, out _);
        }

        public virtual Task CleanAsync()
            => Task.Run(Clean);

        public bool ContainsKey(TKey key)
            => Store.ContainsKey(key);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            => Store.Select(s => new KeyValuePair<TKey, TValue>(s.Key, s.Value.Value)).GetEnumerator();

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if(Store.TryGetValue(key, out var svalue))
            {
                value = svalue.Value;
                return true;
            }
            value = default;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(TKey key, TValue value)
            => Store.AddOrUpdate(key, k => new(value, DateTime.Now), (k, v) =>
            {
                v.Added = DateTime.Now;
                return v;
            });

        public bool Remove(TKey key)
            => Store.Remove(key, out _);

        public void Add(KeyValuePair<TKey, TValue> item)
            => Add(item.Key, item.Value);

        public void Clear()
            => Store.Clear();

        public bool Contains(KeyValuePair<TKey, TValue> item)
            => Store.ContainsKey(item.Key) && (Store[item.Key].Value?.Equals(item.Value) ?? false);

        /// <summary>
        /// NOT SUPPORTED
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        [Obsolete("Not Supported")]
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            => throw new NotSupportedException();

        public bool Remove(KeyValuePair<TKey, TValue> item)
            => Contains(item) && Store.TryRemove(item.Key, out _);

        public TemporaryCache(TimeSpan timeout)
        {
            Timeout = timeout;
        }

        class TrackedValue
        {
            public TValue Value { get; set; }
            public DateTime Added { get; set; }
            public TrackedValue(TValue value, DateTime added)
            {
                Added = added;
                Value = value;
            }
        }
    }
}
