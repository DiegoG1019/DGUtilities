using System;
namespace DiegoG.Utilities
{
    [Serializable]
    public struct CFloat
    {

        private float __float;

        public float UpperLimit { get; private set; }
        public float LowerLimit { get; private set; }
        private float Value
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
            Value = v;
            UpperLimit = upper;
            LowerLimit = lower;
        }

        public static implicit operator float(CFloat d) => d.Value;

        public static bool operator <(CFloat a, CFloat b) => a.Value < b.Value;

        public static bool operator >(CFloat a, CFloat b) => a.Value > b.Value;

        public static bool operator ==(CFloat a, CFloat b) => a.Value == b.Value;

        public static bool operator !=(CFloat a, CFloat b) => a.Value != b.Value;

        public static bool operator <=(CFloat a, CFloat b) => a.Value <= b.Value;

        public static bool operator >=(CFloat a, CFloat b) => a.Value >= b.Value;

        public override bool Equals(object a) => a is CFloat @float && this == @float;

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString();
    }
}
