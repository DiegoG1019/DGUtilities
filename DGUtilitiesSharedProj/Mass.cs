using System.Collections.Immutable;

namespace DiegoG.Utilities
{
    public class Mass
    {
        public enum Units
        {
            Kilogram, Pound
        }
        public static ImmutableDictionary<Units, string> ShortUnits { get; }
        static Mass()
        {
            var builder = ImmutableDictionary.CreateBuilder<Units, string>();
            builder.Add(Units.Kilogram, "Kg.");
            builder.Add(Units.Pound, "Lbs.");
            ShortUnits = builder.ToImmutable();
        }
        public const decimal KgLb = 2.20462M;
        public const decimal LbKg = 0.453592M;
        public decimal Kilogram { get; set; }
        public decimal Pound
        {
            get => Kilogram * KgLb;
            set => Kilogram = value * LbKg;
        }
        public decimal this[Units index]
        {
            get
            {
                switch (index)
                {
                    case Units.Kilogram:
                        return Kilogram;
                    case Units.Pound:
                        return Pound;
                    default:
                        return Kilogram;
                }
            }
            set
            {
                switch (index)
                {
                    case Units.Kilogram:
                        Kilogram = value;
                        break;
                    case Units.Pound:
                        Pound = value;
                        break;
                }
            }
        }
        public Mass(decimal V, Units i)
        {
            switch (i)
            {
                case Units.Kilogram:
                    Kilogram = V;
                    break;
                case Units.Pound:
                    Pound = V;
                    break;
            }
        }
        public static bool operator >(Mass A, Mass B) => A.Kilogram > B.Kilogram;
        public static bool operator <(Mass A, Mass B) => !(A > B);
        public static bool operator >=(Mass A, Mass B) => A.Kilogram >= B.Kilogram;
        public static bool operator <=(Mass A, Mass B) => !(A >= B);
        public static bool operator ==(Mass A, Mass B) => A.Kilogram == B.Kilogram;
        public static bool operator !=(Mass A, Mass B) => !(A == B);
        public static Mass operator +(Mass A, Mass B) => new Mass(A.Kilogram + B.Kilogram, Units.Kilogram);
        public static Mass operator -(Mass A, Mass B) => new Mass(A.Kilogram - B.Kilogram, Units.Kilogram);
        public static Mass operator /(Mass A, Mass B) => new Mass(A.Kilogram / B.Kilogram, Units.Kilogram);
        public static Mass operator *(Mass A, Mass B) => new Mass(A.Kilogram * B.Kilogram, Units.Kilogram);
        public static Mass operator %(Mass A, Mass B) => new Mass(A.Kilogram % B.Kilogram, Units.Kilogram);
        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
    }
}
