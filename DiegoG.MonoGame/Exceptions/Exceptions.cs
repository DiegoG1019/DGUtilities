using System;

namespace DiegoG.MonoGame.Exceptions
{
    public class InvalidIDException : Exception
    {
        public InvalidIDException(string msg) : base(msg) { }
    }
}
