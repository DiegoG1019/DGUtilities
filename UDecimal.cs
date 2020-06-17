using System;

namespace DiegoG.Utilities
{
    public struct UDecimal
    {

        /// <summary>
        /// Represents the smallest possible value of <see cref="UDecimal"/> (0).
        /// </summary>
        public static UDecimal MinValue = 0M;

        /// <summary>
        /// Represents the largest possible value of <see cref="UDecimal"/> (equivalent to <see cref="decimal.MaxValue"/>).
        /// </summary>
        public static UDecimal MaxValue = decimal.MaxValue;

        decimal value;

        public UDecimal(decimal Value)
        {
            if (Double.IsNegativeInfinity((double)Value))
                throw new NotSupportedException();

            value = Value < 0 ? 0 : Value;
        }

        public static implicit operator decimal(UDecimal d)
        {
            return d.value;
        }

        public static implicit operator UDecimal(decimal d)
        {
            return new UDecimal(d);
        }

        public static bool operator <(UDecimal a, UDecimal b)
        {
            return a.value < b.value;
        }

        public static bool operator >(UDecimal a, UDecimal b)
        {
            return a.value > b.value;
        }

        public static bool operator ==(UDecimal a, UDecimal b)
        {
            return a.value == b.value;
        }

        public static bool operator !=(UDecimal a, UDecimal b)
        {
            return a.value != b.value;
        }

        public static bool operator <=(UDecimal a, UDecimal b)
        {
            return a.value <= b.value;
        }

        public static bool operator >=(UDecimal a, UDecimal b)
        {
            return a.value >= b.value;
        }

        public override bool Equals(object a)
        {
            return !(a is UDecimal) ? false : this == (UDecimal)a;
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
