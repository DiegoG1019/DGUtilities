using System;

namespace DiegoG.Utilities
{
    public class GenericEventArgs<T> : EventArgs
    {
        public T Argument { get; init; }
        public GenericEventArgs(T arg) => Argument = arg;
    }
}
