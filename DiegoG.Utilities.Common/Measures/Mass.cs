using System;
using System.Collections.Immutable;
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

        public static MassMeasureProperty<float> UserUnitDefinition { get; set; } = new MassMeasureProperty<float>(Units.Kilogram, 1);

        [XmlType(TypeName = "MassUnits")]
        public enum Units { Kilogram, Pound, Gram, UserUnits }
        static Mass()
        {
            var builder = ImmutableDictionary.CreateBuilder<Units, string>();
            builder.Add(Units.Kilogram, "Kg.");
            builder.Add(Units.Pound, "Lbs.");
            ShortUnits = builder.ToImmutable();
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
            set => DefaultValue = value;
        }

        [UnitProperty(nameof(Units.Pound)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal Pound
        {
            get => Kilogram * KgLb;
            set => Kilogram = value * LbKg;
        }

        [UnitProperty(nameof(Units.UserUnits)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public float UserUnit
        {
            get => (float)this[UserUnitDefinition.BaseUnit] * UserUnitDefinition.ConversionValue;
            set => this[UserUnitDefinition.BaseUnit] = (decimal)(value / UserUnitDefinition.ConversionValue);
        }

        [UnitProperty(nameof(Units.Gram)), JsonIgnore, IgnoreDataMember, XmlIgnore]
        public decimal Gram
        {
            get => Kilogram * KgG;
            set => Kilogram = GKg * value;
        }
        public Mass() => Kilogram = 0;
        public Mass(decimal V, Units i) : this() => this[i] = V;
        public Mass(Mass mass) => Kilogram = mass.Kilogram;

        public static Mass OneKilogram => new Mass(1, Mass.Units.Kilogram);
        public static Mass OnePound => new Mass(1, Mass.Units.Pound);
        public static Mass OneGram => new Mass(1, Mass.Units.Gram);
        public static Mass OneUU => new Mass(1, Mass.Units.UserUnits);
    }
}
