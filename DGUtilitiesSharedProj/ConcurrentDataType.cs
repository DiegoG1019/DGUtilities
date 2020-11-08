using System;
using DiegoG.Utilities.Delegates;

namespace DiegoG.Utilities
{
    public class ConcurrentDataType<T>
    {
        private object Key { get; } = new object();
        private T _Data = default;
        public T Data
        {
            get
            {
                //Since it's the same key, this allows the object to wait for a write operation to finish to get the latest value
                //And as for the downside, returning data really doesn't take any time at all
                lock (Key)
                    return _Data;
            }
            set
            {
                lock (Key)
                    _Data = value;
            }
        }
        /// <summary>
        /// Locks the object and performs a function on it
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public T Operate(Func<T,T> action)
        {
            lock (Key)
            {
                _Data = action(_Data);
                return _Data;
            }
        }
        public ConcurrentDataType() { }
        public ConcurrentDataType(T data) => _Data = data;
        public static implicit operator T(ConcurrentDataType<T> c) => c._Data;
        public static implicit operator ConcurrentDataType<T>(T c) => new ConcurrentDataType<T>(c);
    }
}
