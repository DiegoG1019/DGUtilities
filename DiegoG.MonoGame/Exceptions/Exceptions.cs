using System;
using System.Collections.Generic;
using System.Text;

namespace DiegoG.MonoGame.Exceptions
{
    public class InvalidIDException : Exception
    {
        public InvalidIDException(string msg) : base(msg) { }
    }
}
