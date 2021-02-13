using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace DiegoG.Utilities.Collections
{
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>,
        INotifyCollectionChanged
    {
        /// <summary>
        /// Handles objects of type KeyValuePair`TKey, TValue`
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private static KeyValuePair<TKey, TValue> NKVP(TKey key, TValue value) => new(key, value);

        private readonly Dictionary<TKey, TValue> Dict = new();
        public TValue this[TKey key]
        {
            get => Dict[key];
            set
            {
                var prevv = Dict[key];
                Dict[key] = value;

                if (ContainsKey(key))
                {
                    CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Replace, NKVP(key, value), NKVP(key, prevv)));
                }
                else if (value is null)
                {
                    Remove(key);
                }
                else
                {
                    CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Add, NKVP(key, value)));
                }
            }
        }

        public int Count => Dict.Count;
        public bool IsReadOnly => false;
        public ICollection<TKey> Keys => Dict.Keys;
        public ICollection<TValue> Values => Dict.Values;

        public void Add(TKey key, TValue value)
        {
            Dict.Add(key, value);
            CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Add, NKVP(key, value)));
        }
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)Dict).Add(item);
            CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Add, item));
        }
        public bool TryAdd(TKey key, TValue value)
        {
            if (Dict.TryAdd(key, value))
            {
                CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Add, NKVP(key, value)));
                return true;
            }
            return false;
        }

        public bool Remove(TKey key)
        {
            KeyValuePair<TKey, TValue> previtem = default;
            if (ContainsKey(key))
            {
                previtem = new(key, this[key]);
            }

            if (Dict.Remove(key))
            {
                CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, previtem));
                return true;
            }
            return false;
        }
        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

        public void Clear()
        {
            Dict.Clear();
            CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Reset));
        }

        public bool ContainsKey(TKey key) => Dict.ContainsKey(key);

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => Dict.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => Dict.GetEnumerator();

        public bool Contains(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)Dict).Contains(item);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)Dict).CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, TValue>>)Dict).GetEnumerator();
    }
}
