using System;
using System.Collections.Generic;
using System.Text;

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
}
