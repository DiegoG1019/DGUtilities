using System;

namespace DiegoG.Utilities.Collections
{
    public class ReadOnlyIndexedProperty<TIndex, TValue>
    {
        readonly Func<TIndex, TValue> GetFunc;
        public ReadOnlyIndexedProperty(Func<TIndex, TValue> getFunc) => GetFunc = getFunc;
        public TValue this[TIndex i] => GetFunc(i);
    }
    public class ReadOnlyIndexedProperty<TIndex1, TIndex2, TValue>
    {
        readonly Func<TIndex1, TIndex2, TValue> GetFunc;
        public ReadOnlyIndexedProperty(Func<TIndex1, TIndex2, TValue> getFunc) => GetFunc = getFunc;
        public TValue this[TIndex1 i1, TIndex2 i2] => GetFunc(i1, i2);
    }
    public class ReadOnlyIndexedProperty<TIndex1, TIndex2, TIndex3, TValue>
    {
        readonly Func<TIndex1, TIndex2, TIndex3, TValue> GetFunc;
        public ReadOnlyIndexedProperty(Func<TIndex1, TIndex2, TIndex3, TValue> getFunc) => GetFunc = getFunc;
        public TValue this[TIndex1 i1, TIndex2 i2, TIndex3 i3] => GetFunc(i1, i2, i3);
    }
}
