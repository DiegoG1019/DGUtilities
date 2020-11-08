using System;

namespace DiegoG.Utilities.Collections
{
    public class WriteOnlyIndexedProperty<TIndex, TValue>
    {
        readonly Action<TIndex, TValue> SetAction;
        public WriteOnlyIndexedProperty(Action<TIndex, TValue> setAction) => SetAction = setAction;
        public TValue this[TIndex i]
        {
            set => SetAction(i, value);
        }
    }
    public class WriteOnlyIndexedProperty<TIndex1, TIndex2, TValue>
    {
        readonly Action<TIndex1, TIndex2, TValue> SetAction;
        public WriteOnlyIndexedProperty(Action<TIndex1, TIndex2, TValue> setAction) => SetAction = setAction;
        public TValue this[TIndex1 i1, TIndex2 i2]
        {
            set => SetAction(i1, i2, value);
        }
    }
    public class WriteOnlyIndexedProperty<TIndex1, TIndex2, TIndex3, TValue>
    {
        readonly Action<TIndex1, TIndex2, TIndex3, TValue> SetAction;
        public WriteOnlyIndexedProperty(Action<TIndex1, TIndex2, TIndex3, TValue> setAction) => SetAction = setAction;
        public TValue this[TIndex1 i1, TIndex2 i2, TIndex3 i3]
        {
            set => SetAction(i1, i2, i3, value);
        }
    }
}
