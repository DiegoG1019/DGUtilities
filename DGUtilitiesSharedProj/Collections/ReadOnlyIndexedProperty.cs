using System;

namespace DiegoG.Utilities.Collections
{
    public class ReadOnlyIndexedProperty<TIndex, TValue>
    {
        readonly Func<TIndex, TValue> GetFunc;
        public ReadOnlyIndexedProperty(Func<TIndex, TValue> getFunc) => GetFunc = getFunc;
        public TValue this[TIndex i] => GetFunc(i);
    }
}
