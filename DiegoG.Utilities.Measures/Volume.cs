using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DiegoG.Utilities.Measures
{
    [Serializable]
    public class Volume : Measure<Volume.Units, Volume>
    {
        public class VolumeMeasureProperty<T> where T : struct, IComparable, IConvertible, IFormattable, IComparable<T>, IEquatable<T>
        {
            public Units BaseUnit { get; set; }
            public T ConversionValue { get; set; }
            public VolumeMeasureProperty(Units unit, T value)
            {
                BaseUnit = unit;
                ConversionValue = value;
            }
        }

        [XmlType(TypeName = "VolumeUnits")]
        public enum Units
        {
            Cubicmeter,
            Liter,
            Milliliter,
            Gallon,
            Pint,
            Ounce,
        }

        static Volume()
        {
            ShortUnitsDict.Add(Units.Cubicmeter, "m^3");
            ShortUnitsDict.Add(Units.Liter, "L");
            ShortUnitsDict.Add(Units.Milliliter, "mL");
            ShortUnitsDict.Add(Units.Gallon, "gal");
            ShortUnitsDict.Add(Units.Pint, "pt");
            ShortUnitsDict.Add(Units.Ounce, "oz");
            DefaultUnit = Units.Liter;
        }

        public const double cM3L = 1000d;
        public const double mLL = 0.001d;
        public const double GalL = 3.785411768d;
        public const double PtL = 0.473176473d;
        public const double LOz = 0.0284131d;

        public const double LcM3 = 1 / 1000d;
        public const double LmL = 1 / 0.001d;
        public const double LGal = 1 / 3.785411768d;
        public const double LPt = 1 / 0.473176473d;
        public const double OzL = 1 / 0.0284131d;

        [UnitProperty(nameof(Units.Liter))]
        public double Liter
        {
            get => DefaultValue;
            init => DefaultValue = value;
        }

        [UnitProperty(nameof(Units.Cubicmeter)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public double Cubicmeter
        {
            get => Liter * LcM3;
            init => Liter = value * cM3L;
        }

        [UnitProperty(nameof(Units.Milliliter)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public double Milliliter
        {
            get => Liter * LmL;
            init => Liter = value * mLL;
        }

        [UnitProperty(nameof(Units.Gallon)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public double Gallon
        {
            get => Liter * LGal;
            init => Liter = value * GalL;
        }

        [UnitProperty(nameof(Units.Pint)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public double Pint
        {
            get => Liter * LPt;
            init => Liter = value * PtL;
        }

        [UnitProperty(nameof(Units.Ounce)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public double Ounce
        {
            get => Liter * LOz;
            init => Liter = value * OzL;
        }

        public Volume() => Liter = 0;

        public Volume(double V, Units i) : this() => this[i] = V;

        public Volume(Length l1, Length l2, Length l3) : this() => Cubicmeter = (l1.Meter * l2.Meter * l3.Meter) / 3;

        public Volume(Volume volume) : base(volume) { }

        public static Density operator /(Mass m, Volume v) => new(m, v);
        public static Volume OneCubicmeter => new Volume(1, Units.Cubicmeter);
        public static Volume OneGallon => new Volume(1, Units.Gallon);
        public static Volume OneLiter => new Volume(1, Units.Liter);
        public static Volume OneMilliliter => new Volume(1, Units.Milliliter);
        public static Volume OneOunce => new Volume(1, Units.Ounce);
        public static Volume OnePint => new Volume(1, Units.Pint);
    }
}
