using System;
namespace DiegoG.Utilities
{
    [Serializable]
    public struct CFloat
    {

        private float __float;

        public float UpperLimit { get; private set; }
        public float LowerLimit { get; private set; }
        private float value
        {
            get => __float;
            set => __float = value > LowerLimit ? (value > UpperLimit ? UpperLimit : __float) : LowerLimit;
        }

        public CFloat(float v) :
            this(v, float.MaxValue, float.MinValue)
        { }

        public CFloat(float v, float upper, float lower) :
            this()
        {
            value = v;
            UpperLimit = upper;
            LowerLimit = lower;
        }

        public static implicit operator float(CFloat d)
        {
            return d.value;
        }

        public static bool operator <(CFloat a, CFloat b)
        {
            return a.value < b.value;
        }

        public static bool operator >(CFloat a, CFloat b)
        {
            return a.value > b.value;
        }

        public static bool operator ==(CFloat a, CFloat b)
        {
            return a.value == b.value;
        }

        public static bool operator !=(CFloat a, CFloat b)
        {
            return a.value != b.value;
        }

        public static bool operator <=(CFloat a, CFloat b)
        {
            return a.value <= b.value;
        }

        public static bool operator >=(CFloat a, CFloat b)
        {
            return a.value >= b.value;
        }

        public override bool Equals(object a)
        {
            return a is CFloat @float && this == @float;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return value.ToString();
        }

    }
}
