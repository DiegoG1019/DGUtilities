using System;

namespace DiegoG.Utilities
{
    public class ConcurrentDataType<T>
    {
        private object Key { get; } = new object();
        private T _Data = default;
        public T Data { get; set; }

        /// <summary>
        /// Locks the object and performs a function on it
        /// </summary>
        public T Operate(Func<T, T> action)
        {
            lock (Key)
            {
                _Data = action(_Data);
                return _Data;
            }
        }
        /// <summary>
        /// Locks the object and performs a function on it
        /// </summary>
        public void Operate(Action<T> action)
        {
            lock (Key)
            {
                action(_Data);
            }
        }
        public ConcurrentDataType() { }
        public ConcurrentDataType(T data) => _Data = data;

        public static implicit operator T(ConcurrentDataType<T> c) => c._Data;

        public static implicit operator ConcurrentDataType<T>(T c) => new ConcurrentDataType<T>(c);
    }
}
