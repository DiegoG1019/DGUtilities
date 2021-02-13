using System;
using System.Numerics;

namespace DiegoG.Utilities
{
    /// <summary>
    /// Arbitrary precision decimal.
    /// All operations are exact, except for division. Division never determines more digits than the given precision.
    /// Source: https://gist.github.com/JcBernack/0b4eef59ca97ee931a2f45542b9ff06d
    /// Based on https://stackoverflow.com/a/4524254
    /// Author: Jan Christoph Bernack (contact: jc.bernack at gmail.com)
    /// Modified by: Diego Garcia
    /// License: public domain
    /// </summary>
    public struct BigDecimal
        : IComparable
        , IComparable<BigDecimal>
    {
        /// <summary>
        /// Specifies whether the significant digits should be truncated to the given precision after each operation.
        /// </summary>
        public static bool AlwaysTruncate { get; set; } = false;

        /// <summary>
        /// Sets the maximum precision of division operations.
        /// If AlwaysTruncate is set to true all operations are affected.
        /// </summary>
        public static int Precision { get; set; } = 50;

        public BigInteger Mantissa { get; set; }
        public int Exponent { get; set; }

        public BigDecimal(BigInteger mantissa, int exponent)
            : this()
        {
            Mantissa = mantissa;
            Exponent = exponent;
            Normalize();
            if (AlwaysTruncate)
            {
                Truncate();
            }
        }

        /// <summary>
        /// Removes trailing zeros on the mantissa
        /// </summary>
        public void Normalize()
        {
            if (Mantissa.IsZero || Exponent == 0)
            {
                Exponent = 0;
            }
            else
            {
                BigInteger remainder = 0;
                while (remainder == 0)
                {
                    var shortened = BigInteger.DivRem(Mantissa, 10, out remainder);
                    if (remainder == 0)
                    {
                        Mantissa = shortened;
                        Exponent++;
                    }
                }
            }
        }

        /// <summary>
        /// Truncate the number to the given precision by removing the least significant digits.
        /// </summary>
        /// <returns>The truncated number</returns>
        public BigDecimal Truncate(int precision)
        {
            // copy this instance (remember it's a struct)
            var shortened = this;
            // save some time because the number of digits is not needed to remove trailing zeros
            shortened.Normalize();
            // remove the least significant digits, as long as the number of digits is higher than the given Precision
            while (NumberOfDigits(shortened.Mantissa) > precision)
            {
                shortened.Mantissa /= 10;
                shortened.Exponent++;
            }
            // normalize again to make sure there are no trailing zeros left
            shortened.Normalize();
            return shortened;
        }

        public BigDecimal Truncate() => Truncate(Precision);

        public BigDecimal Floor() => Truncate(NumberOfDigits(Mantissa) + Exponent);

        public static int NumberOfDigits(BigInteger value) => (int)Math.Ceiling(BigInteger.Log10(value * value.Sign));

        #region Conversions

        public static implicit operator BigDecimal(int value) => new BigDecimal(value, 0);

        public static implicit operator BigDecimal(double value)
        {
            var mantissa = (BigInteger)value;
            var exponent = 0;
            double scaleFactor = 1;
            while (Math.Abs(value * scaleFactor - (double)mantissa) > 0)
            {
                exponent -= 1;
                scaleFactor *= 10;
                mantissa = (BigInteger)(value * scaleFactor);
            }
            return new BigDecimal(mantissa, exponent);
        }

        public static implicit operator BigDecimal(decimal value)
        {
            var mantissa = (BigInteger)value;
            var exponent = 0;
            decimal scaleFactor = 1;
            while ((decimal)mantissa != value * scaleFactor)
            {
                exponent -= 1;
                scaleFactor *= 10;
                mantissa = (BigInteger)(value * scaleFactor);
            }
            return new BigDecimal(mantissa, exponent);
        }

        public static explicit operator double(BigDecimal value) => (double)value.Mantissa * Math.Pow(10, value.Exponent);

        public static explicit operator float(BigDecimal value) => Convert.ToSingle((double)value);

        public static explicit operator decimal(BigDecimal value) => (decimal)value.Mantissa * (decimal)Math.Pow(10, value.Exponent);

        public static explicit operator int(BigDecimal value) => (int)(value.Mantissa * BigInteger.Pow(10, value.Exponent));

        public static explicit operator uint(BigDecimal value) => (uint)(value.Mantissa * BigInteger.Pow(10, value.Exponent));
        #endregion

        #region Operators

        public static BigDecimal operator +(BigDecimal value) => value;

        public static BigDecimal operator -(BigDecimal value)
        {
            value.Mantissa *= -1;
            return value;
        }

        public static BigDecimal operator ++(BigDecimal value) => value + 1;

        public static BigDecimal operator --(BigDecimal value) => value - 1;

        public static BigDecimal operator +(BigDecimal left, BigDecimal right) => Add(left, right);

        public static BigDecimal operator -(BigDecimal left, BigDecimal right) => Add(left, -right);

        private static BigDecimal Add(BigDecimal left, BigDecimal right) => left.Exponent > right.Exponent
                ? new BigDecimal(AlignExponent(left, right) + right.Mantissa, right.Exponent)
                : new BigDecimal(AlignExponent(right, left) + left.Mantissa, left.Exponent);

        public static BigDecimal operator *(BigDecimal left, BigDecimal right) => new BigDecimal(left.Mantissa * right.Mantissa, left.Exponent + right.Exponent);

        public static BigDecimal operator /(BigDecimal dividend, BigDecimal divisor)
        {
            var exponentChange = Precision - (NumberOfDigits(dividend.Mantissa) - NumberOfDigits(divisor.Mantissa));
            if (exponentChange < 0)
            {
                exponentChange = 0;
            }

            dividend.Mantissa *= BigInteger.Pow(10, exponentChange);
            return new BigDecimal(dividend.Mantissa / divisor.Mantissa, dividend.Exponent - divisor.Exponent - exponentChange);
        }

        public static BigDecimal operator %(BigDecimal left, BigDecimal right) => left - right * (left / right).Floor();

        public static bool operator ==(BigDecimal left, BigDecimal right) => left.Exponent == right.Exponent && left.Mantissa == right.Mantissa;

        public static bool operator !=(BigDecimal left, BigDecimal right) => left.Exponent != right.Exponent || left.Mantissa != right.Mantissa;

        public static bool operator <(BigDecimal left, BigDecimal right) => left.Exponent > right.Exponent ? AlignExponent(left, right) < right.Mantissa : left.Mantissa < AlignExponent(right, left);

        public static bool operator >(BigDecimal left, BigDecimal right) => left.Exponent > right.Exponent ? AlignExponent(left, right) > right.Mantissa : left.Mantissa > AlignExponent(right, left);

        public static bool operator <=(BigDecimal left, BigDecimal right) => left.Exponent > right.Exponent ? AlignExponent(left, right) <= right.Mantissa : left.Mantissa <= AlignExponent(right, left);

        public static bool operator >=(BigDecimal left, BigDecimal right) => left.Exponent > right.Exponent ? AlignExponent(left, right) >= right.Mantissa : left.Mantissa >= AlignExponent(right, left);

        /// <summary>
        /// Returns the mantissa of value, aligned to the exponent of reference.
        /// Assumes the exponent of value is larger than of reference.
        /// </summary>
        private static BigInteger AlignExponent(BigDecimal value, BigDecimal reference) => value.Mantissa * BigInteger.Pow(10, value.Exponent - reference.Exponent);
        #endregion

        #region Additional mathematical functions

        public static BigDecimal Exp(double exponent)
        {
            var tmp = (BigDecimal)1;
            while (Math.Abs(exponent) > 100)
            {
                var diff = exponent > 0 ? 100 : -100;
                tmp *= Math.Exp(diff);
                exponent -= diff;
            }
            return tmp * Math.Exp(exponent);
        }

        public static BigDecimal Pow(double basis, double exponent)
        {
            var tmp = (BigDecimal)1;
            while (Math.Abs(exponent) > 100)
            {
                var diff = exponent > 0 ? 100 : -100;
                tmp *= Math.Pow(basis, diff);
                exponent -= diff;
            }
            return tmp * Math.Pow(basis, exponent);
        }

        #endregion

        public override string ToString() => string.Concat(Mantissa.ToString(), "E", Exponent);

        public bool Equals(BigDecimal other) => other.Mantissa.Equals(Mantissa) && other.Exponent == Exponent;

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            return obj is BigDecimal @decimal && Equals(@decimal);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Mantissa.GetHashCode() * 397) ^ Exponent;
            }
        }

        public int CompareTo(object obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (obj is not BigDecimal)
            {
                throw new ArgumentException($"{nameof(obj)} is not a BigDecimal");
            }

            return CompareTo((BigDecimal)obj);
        }

        public int CompareTo(BigDecimal other) => this < other ? -1 : (this > other ? 1 : 0);
    }
}