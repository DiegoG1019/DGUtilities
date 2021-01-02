using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.CLI
{
    [Serializable]
    public class InvalidCommandException : Exception
    {
        public InvalidCommandException(string message) : base(message) { }
        public InvalidCommandException(string message, Exception inner) : base(message, inner) { }
        protected InvalidCommandException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class InvalidCommandArgumentException : Exception
    {
        public InvalidCommandArgumentException(string message) : base(message) { }
        public InvalidCommandArgumentException(string message, Exception inner) : base(message, inner) { }
        protected InvalidCommandArgumentException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class CommandProcessException : Exception
    {
        public CommandProcessException() { }
        public CommandProcessException(string message) : base(message) { }
        public CommandProcessException(string message, Exception inner) : base(message, inner) { }
        protected CommandProcessException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
