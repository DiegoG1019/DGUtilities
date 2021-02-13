using System;

namespace DiegoG.Utilities.Collections
{
    public class IndexedProperty<TIndex, TValue>
    {
        readonly Action<TIndex, TValue> SetAction;
        readonly Func<TIndex, TValue> GetFunc;
        public IndexedProperty(Func<TIndex, TValue> getFunc, Action<TIndex, TValue> setAction)
        {
            GetFunc = getFunc;
            SetAction = setAction;
        }
        public TValue this[TIndex i]
        {
            get => GetFunc(i);
            set => SetAction(i, value);
        }
    }
    public class IndexedProperty<TIndex1, TIndex2, TValue>
    {
        readonly Action<TIndex1, TIndex2, TValue> SetAction;
        readonly Func<TIndex1, TIndex2, TValue> GetFunc;
        public IndexedProperty(Func<TIndex1, TIndex2, TValue> getFunc, Action<TIndex1, TIndex2, TValue> setAction)
        {
            GetFunc = getFunc;
            SetAction = setAction;
        }
        public TValue this[TIndex1 i1, TIndex2 i2]
        {
            get => GetFunc(i1, i2);
            set => SetAction(i1, i2, value);
        }
    }
    public class IndexedProperty<TIndex1, TIndex2, TIndex3, TValue>
    {
        readonly Action<TIndex1, TIndex2, TIndex3, TValue> SetAction;
        readonly Func<TIndex1, TIndex2, TIndex3, TValue> GetFunc;
        public IndexedProperty(Func<TIndex1, TIndex2, TIndex3, TValue> getFunc, Action<TIndex1, TIndex2, TIndex3, TValue> setAction)
        {
            GetFunc = getFunc;
            SetAction = setAction;
        }
        public TValue this[TIndex1 i1, TIndex2 i2, TIndex3 i3]
        {
            get => GetFunc(i1, i2, i3);
            set => SetAction(i1, i2, i3, value);
        }
    }
}
