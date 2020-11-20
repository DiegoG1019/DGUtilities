using System;

namespace DiegoG.Utilities
{
    [Serializable]
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
        decimal Value { get; set; }

        public UDecimal(decimal Value)
        {
            if (Double.IsNegativeInfinity((double)Value))
                throw new NotSupportedException();
            this.Value = Value < 0 ? 0 : Value;
        }

        public static implicit operator decimal(UDecimal d) => d.Value;
        public static implicit operator UDecimal(decimal d) => new UDecimal(d);
        public static bool operator <(UDecimal a, UDecimal b) => a.Value < b.Value;
        public static bool operator >(UDecimal a, UDecimal b) => a.Value > b.Value;
        public static bool operator ==(UDecimal a, UDecimal b) => a.Value == b.Value;
        public static bool operator !=(UDecimal a, UDecimal b) => a.Value != b.Value;
        public static bool operator <=(UDecimal a, UDecimal b) => a.Value <= b.Value;
        public static bool operator >=(UDecimal a, UDecimal b) => a.Value >= b.Value;
        public override bool Equals(object a) => a is UDecimal @decimal && this == @decimal;
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value.ToString();
    }
}
