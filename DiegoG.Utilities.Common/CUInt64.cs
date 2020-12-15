using System;

namespace DiegoG.Utilities
{
    [Serializable]
    public struct CUInt64
    {
        private ulong _uint64;
        private const byte _lower = 0;
        private const ulong _upper = UInt64.MaxValue;
        public ulong Value
        {
            get
            {
                return _uint64;
            }
            set
            {
                float a = _uint64 + value;
                if (a > _uint64)
                {
                    _uint64 = _upper;
                    return;
                }
                else
                {
                    if (a < _lower)
                    {
                        _uint64 = _lower;
                        return;
                    }
                    _uint64 = (ulong)a;
                    return;
                }
            }
        }

        public CUInt64(ulong v) :
            this()
        {
            Value = v;
        }

        public static implicit operator ulong(CUInt64 d) => d.Value;
        public static explicit operator long(CUInt64 d) => (long)d.Value;
        public static implicit operator CUInt64(ulong d) => new CUInt64(d);
        public static implicit operator CUInt64(int d) => new CUInt64((ulong)d);
        public static CUInt64 operator -(CUInt64 a, CUInt64 b) => a.Value - b.Value;
        public static CUInt64 operator +(CUInt64 a, CUInt64 b) => a.Value + b.Value;
        public static CUInt64 operator *(CUInt64 a, CUInt64 b) => a.Value * b.Value;
        public static CUInt64 operator /(CUInt64 a, CUInt64 b) => a.Value / b.Value;
        public static bool operator <(CUInt64 a, CUInt64 b) => a.Value < b.Value;
        public static bool operator >(CUInt64 a, CUInt64 b) => a.Value > b.Value;
        public static bool operator ==(CUInt64 a, CUInt64 b) => a.Value == b.Value;
        public static bool operator !=(CUInt64 a, CUInt64 b) => !(a == b);
        public static bool operator <=(CUInt64 a, CUInt64 b) => a.Value <= b.Value;
        public static bool operator >=(CUInt64 a, CUInt64 b) => a.Value >= b.Value;
        public override bool Equals(object a) => a is CUInt64 @int && this == @int;
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value.ToString();
    }
}
