using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DiegoG.Utilities.Measures
{
    public class Mass : Measure<Mass.Units, Mass>
    {
        public class MassMeasureProperty<T> where T : struct, IComparable, IConvertible, IFormattable, IComparable<T>, IEquatable<T>
        {
            public Units BaseUnit { get; set; }
            public T ConversionValue { get; set; }
            public MassMeasureProperty(Units unit, T value)
            {
                BaseUnit = unit;
                ConversionValue = value;
            }
        }

        public static MassMeasureProperty<double> UserUnitDefinition { get; set; } = new MassMeasureProperty<double>(Units.Kilogram, 1);

        [XmlType(TypeName = "MassUnits")]
        public enum Units { Kilogram, Pound, Gram, UserUnits }
        static Mass()
        {
            ShortUnitsDict.Add(Units.Kilogram, "Kg.");
            ShortUnitsDict.Add(Units.Pound, "Lbs.");
            DefaultUnit = Units.Kilogram;
        }
        public const double KgLb = 2.20462d;
        public const double LbKg = 0.453592d;
        public const double KgG = 1000d;
        public const double GKg = 0.001d;

        [UnitProperty(nameof(Units.Kilogram))]
        public double Kilogram
        {
            get => DefaultValue;
            init => DefaultValue = value;
        }

        [UnitProperty(nameof(Units.Pound)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public double Pound
        {
            get => Kilogram * KgLb;
            init => Kilogram = value * LbKg;
        }

        [UnitProperty(nameof(Units.UserUnits)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public double UserUnit
        {
            get => this[UserUnitDefinition.BaseUnit] * UserUnitDefinition.ConversionValue;
            init => this[UserUnitDefinition.BaseUnit] = value / UserUnitDefinition.ConversionValue;
        }

        [UnitProperty(nameof(Units.Gram)), JsonIgnore, IgnoreDataMember, XmlIgnore]
        public double Gram
        {
            get => Kilogram * KgG;
            init => Kilogram = GKg * value;
        }

        public Mass() => Kilogram = 0;

        public Mass(double V, Units i) : this() => this[i] = V;

        public Mass(Mass mass) : base(mass) { }

        public static Mass OneKilogram => new Mass(1, Units.Kilogram);
        public static Mass OnePound => new Mass(1, Units.Pound);
        public static Mass OneGram => new Mass(1, Units.Gram);
        public static Mass OneUU => new Mass(1, Units.UserUnits);
    }
}
