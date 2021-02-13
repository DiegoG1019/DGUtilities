using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DiegoG.Utilities.Measures
{
    public record Mass : Measure<Mass.Units, Mass>
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

        public static MassMeasureProperty<decimal> UserUnitDefinition { get; set; } = new MassMeasureProperty<decimal>(Units.Kilogram, 1);

        [XmlType(TypeName = "MassUnits")]
        public enum Units { Kilogram, Pound, Gram, UserUnits }
        static Mass()
        {
            ShortUnitsDict.Add(Units.Kilogram, "Kg.");
            ShortUnitsDict.Add(Units.Pound, "Lbs.");
            DefaultUnit = Units.Kilogram;
        }
        public const decimal KgLb = 2.20462M;
        public const decimal LbKg = 0.453592M;
        public const decimal KgG = 1000;
        public const decimal GKg = 0.001M;

        [UnitProperty(nameof(Units.Kilogram))]
        public decimal Kilogram
        {
            get => DefaultValue;
            init => DefaultValue = value;
        }
        public double KilogramD => (double)Kilogram;
        public float KilogramF => (float)Kilogram;

        [UnitProperty(nameof(Units.Pound)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal Pound
        {
            get => Kilogram * KgLb;
            init => Kilogram = value * LbKg;
        }
        public double PoundD => (double)Pound;
        public float PoundF => (float)Pound;

        [UnitProperty(nameof(Units.UserUnits)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal UserUnit
        {
            get => this[UserUnitDefinition.BaseUnit] * UserUnitDefinition.ConversionValue;
            init => this[UserUnitDefinition.BaseUnit] = (value / UserUnitDefinition.ConversionValue);
        }
        public double UserUnitD => (double)UserUnit;
        public float UserUnitF => (float)UserUnit;

        [UnitProperty(nameof(Units.Gram)), JsonIgnore, IgnoreDataMember, XmlIgnore]
        public decimal Gram
        {
            get => Kilogram * KgG;
            init => Kilogram = GKg * value;
        }
        public double GramD => (double)Gram;
        public float GramF => (float)Gram;

        public Mass() => Kilogram = 0;

        public Mass(decimal V, Units i) : this() => this[i] = V;

        public Mass(Mass mass) : base(mass) { }

        public static Mass OneKilogram => new Mass(1, Units.Kilogram);
        public static Mass OnePound => new Mass(1, Units.Pound);
        public static Mass OneGram => new Mass(1, Units.Gram);
        public static Mass OneUU => new Mass(1, Units.UserUnits);
    }
}
