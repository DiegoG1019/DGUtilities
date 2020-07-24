using System;

namespace DiegoG.Utilities
{
    [Serializable]
    public struct CUInt64
    {
        private ulong _uint64;
        private const byte _lower = 0;
        private const ulong _upper = UInt64.MaxValue;
        public ulong value
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
            value = v;
        }

        public static implicit operator ulong(CUInt64 d)
        {
            return d.value;
        }

        public static implicit operator int(CUInt64 d)
        {
            return (int)d.value;
        }

        public static implicit operator CUInt64(ulong d)
        {
            return new CUInt64(d);
        }

        public static implicit operator CUInt64(int d)
        {
            return new CUInt64((ulong)d);
        }

        public static bool operator <(CUInt64 a, CUInt64 b)
        {
            return a.value < b.value;
        }

        public static bool operator >(CUInt64 a, CUInt64 b)
        {
            return a.value > b.value;
        }

        public static bool operator ==(CUInt64 a, CUInt64 b)
        {
            return a.value == b.value;
        }

        public static bool operator !=(CUInt64 a, CUInt64 b)
        {
            return a.value != b.value;
        }

        public static bool operator <=(CUInt64 a, CUInt64 b)
        {
            return a.value <= b.value;
        }

        public static bool operator >=(CUInt64 a, CUInt64 b)
        {
            return a.value >= b.value;
        }

        public override bool Equals(object a)
        {
            return !(a is CUInt64) ? false : this == (CUInt64)a;
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
