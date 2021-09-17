using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.MonoGame
{
    public class GenericEventArgs<T> : EventArgs
    {
        public T Data { get; init; }
        public GenericEventArgs(T data) => Data = data;
    }
}
