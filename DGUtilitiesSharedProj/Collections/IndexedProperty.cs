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
}
