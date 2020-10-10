using System;

namespace DiegoG.Utilities
{
    [Serializable]
    public struct CUInt32
    {
        private uint _uint32;
        private const byte _lower = 0;
        private const uint _upper = UInt32.MaxValue;
        public uint Value
        {
            get
            {
                return _uint32;
            }
            set
            {
                long a = _uint32 + value;
                if (a > _upper)
                {
                    _uint32 = _upper;
                    return;
                }
                else
                {
                    if (a < _lower)
                    {
                        _uint32 = _lower;
                        return;
                    }
                    _uint32 = (uint)a;
                    return;
                }
            }
        }

        public CUInt32(uint d) :
            this()
        {
            Value = d;
        }

        public static implicit operator uint(CUInt32 d) => d.Value;
        public static explicit operator int(CUInt32 d) => (int)d.Value;
        public static implicit operator CUInt32(uint d) => new CUInt32(d);
        public static implicit operator CUInt32(int d) => new CUInt32((uint)d);
        public static CUInt32 operator -(CUInt32 a, CUInt32 b) => a.Value - b.Value;
        public static CUInt32 operator +(CUInt32 a, CUInt32 b) => a.Value + b.Value;
        public static CUInt32 operator *(CUInt32 a, CUInt32 b) => a.Value * b.Value;
        public static CUInt32 operator /(CUInt32 a, CUInt32 b) => a.Value / b.Value;
        public static long operator -(CUInt32 a) => -a.Value;
        public static bool operator <(CUInt32 a, CUInt32 b) => a.Value < b.Value;
        public static bool operator >(CUInt32 a, CUInt32 b) => a.Value > b.Value;
        public static bool operator ==(CUInt32 a, CUInt32 b) => a.Value == b.Value;
        public static bool operator !=(CUInt32 a, CUInt32 b) => !(a == b);
        public static bool operator <=(CUInt32 a, CUInt32 b) => a.Value <= b.Value;
        public static bool operator >=(CUInt32 a, CUInt32 b) => a.Value >= b.Value;
        public override bool Equals(object a) => a is CUInt32 @int && this == @int;
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value.ToString();
    }
}
