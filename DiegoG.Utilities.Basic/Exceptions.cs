using System;

namespace DiegoG.Utilities.Debug
{
    [Serializable]
    public class AssertionException : Exception
    {
        public AssertionException() { }
        public AssertionException(string message) : base(message) { }
        public AssertionException(string message, Exception inner) : base(message, inner) { }
        protected AssertionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    public static class Methods
    {
        public static void Assert(this object obj, Func<bool> func, string message = "")
        {
            if (!func())
                throw obj is null ?
                    new AssertionException(message) :
                    new AssertionException($"Thrown by object type: {obj.GetType()}{(string.IsNullOrWhiteSpace(message) ? "" : $" | {message}")}");
        }

        public static void Assert(this object obj, bool value, string message = "") => Assert(obj, () => value, message);
    }
}
