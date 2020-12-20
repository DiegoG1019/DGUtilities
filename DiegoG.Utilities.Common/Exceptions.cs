using System;

namespace DiegoG.Utilities.Exceptions
{
    [System.Serializable]
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
        public static void Assert(this object obj, Func<bool> func, string message)
        {
            if (!func())
                throw obj switch
                {
                    null => new AssertionException(message),
                    _ => new AssertionException($"Thrown by object type: {obj.GetType()} | {message}"),
                };
        }
        public static void Assert(this object obj, Func<bool> func) => Assert(obj, func, "");
        public static void Assert(this object obj, bool value, string message) => Assert(obj, () => value, message);
        public static void Assert(this object obj, bool value) => Assert(obj, value, "");
    }
}
