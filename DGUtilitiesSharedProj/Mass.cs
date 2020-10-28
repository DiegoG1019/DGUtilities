using System.Collections.Immutable;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

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

        [IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal Pound
        {
            get => Kilogram * KgLb;
            set => Kilogram = value * LbKg;
        }
        [JsonIgnore, IgnoreDataMember, XmlIgnore]
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
        public Mass()
        {
            Kilogram = 0;
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
        public string ToString(Units unit) => $"{this[unit]}{ShortUnits[unit]}";
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

        public static Mass Zero => new Mass(0, Units.Kilogram);

    }
}
