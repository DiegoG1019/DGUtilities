using System;

namespace DiegoG.Utilities
{
    public struct CUInt32
    {
        private uint _uint32;
        private const byte _lower = 0;
        private const uint _upper = UInt32.MaxValue;
        public uint value
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
            value = d;
        }

        public static implicit operator uint(CUInt32 d)
        {
            return d.value;
        }

        public static implicit operator int(CUInt32 d)
        {
            return (int)d.value;
        }

        public static implicit operator CUInt32(uint d)
        {
            return new CUInt32(d);
        }

        public static implicit operator CUInt32(int d)
        {
            return new CUInt32((uint)d);
        }

        public static bool operator <(CUInt32 a, CUInt32 b)
        {
            return a.value < b.value;
        }

        public static bool operator >(CUInt32 a, CUInt32 b)
        {
            return a.value > b.value;
        }

        public static bool operator ==(CUInt32 a, CUInt32 b)
        {
            return a.value == b.value;
        }

        public static bool operator !=(CUInt32 a, CUInt32 b)
        {
            return a.value != b.value;
        }

        public static bool operator <=(CUInt32 a, CUInt32 b)
        {
            return a.value <= b.value;
        }

        public static bool operator >=(CUInt32 a, CUInt32 b)
        {
            return a.value >= b.value;
        }

        public override bool Equals(object a)
        {
            return a is CUInt32 && this == (CUInt32)a;
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
