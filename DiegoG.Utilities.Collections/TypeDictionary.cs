using System;
using System.Threading;

/// <summary>
/// Code inspired by https://github.com/AsynkronIT/protoactor-dotnet/blob/dev/src/Proto.Actor/Utils/TypedDictionary.cs
/// </summary>
namespace DiegoG.Utilities.Collections
{
    public class TypeDictionary<TValue>
    {
        private readonly double _growthFactor;

        private static int typeIndex = -1;
        private readonly object _lockObject = new();

        private TValue[] _values;

        public TypeDictionary(int initialSize = 100, double growthFactor = 2)
        {
            _values = new TValue[initialSize];
            _growthFactor = growthFactor >= 1 ? growthFactor : 1;
        }

        public void Add<TKey>(TValue value)
        {
            lock (_lockObject)
            {
                var id = TypeKey<TKey>.Id;
                if (id >= _values.Length) Array.Resize(ref _values, (int)(id * _growthFactor));

                _values[id] = value;
            }
        }

#nullable enable
        public TValue? Get<TKey>()
        {
            var id = TypeKey<TKey>.Id;
            return id >= _values.Length ? default : _values[id];
        }

        public void Remove<TKey>()
        {
            var id = TypeKey<TKey>.Id;
            if (id >= _values.Length) return;

            _values[id] = default!;
        }

        private static class TypeKey<TKey>
        {
            internal static readonly int Id = Interlocked.Increment(ref typeIndex);
        }
    }
}