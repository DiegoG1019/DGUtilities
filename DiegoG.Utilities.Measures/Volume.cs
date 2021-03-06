﻿using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DiegoG.Utilities.Measures
{
    [Serializable]
    public record Volume : Measure<Volume.Units, Volume>
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

        public const decimal cM3L = 1000M;
        public const decimal mLL = 0.001M;
        public const decimal GalL = 3.785411768M;
        public const decimal PtL = 0.473176473M;
        public const decimal LOz = 0.0284131M;

        public const decimal LcM3 = 1 / 1000M;
        public const decimal LmL = 1 / 0.001M;
        public const decimal LGal = 1 / 3.785411768M;
        public const decimal LPt = 1 / 0.473176473M;
        public const decimal OzL = 1 / 0.0284131M;

        [UnitProperty(nameof(Units.Liter))]
        public decimal Liter
        {
            get => DefaultValue;
            init => DefaultValue = value;
        }
        public double LiterD => (double)Liter;
        public float LiterF => (float)Liter;

        [UnitProperty(nameof(Units.Cubicmeter)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal Cubicmeter
        {
            get => Liter * LcM3;
            init => Liter = value * cM3L;
        }
        public double CubicmeterD => (double)Cubicmeter;
        public float CubicmeterF => (float)Cubicmeter;

        [UnitProperty(nameof(Units.Milliliter)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal Milliliter
        {
            get => Liter * LmL;
            init => Liter = value * mLL;
        }
        public double MilliliterD => (double)Milliliter;
        public float MilliliterF => (float)Milliliter;

        [UnitProperty(nameof(Units.Gallon)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal Gallon
        {
            get => Liter * LGal;
            init => Liter = value * GalL;
        }
        public double GallonD => (double)Gallon;
        public float GallonF => (float)Gallon;

        [UnitProperty(nameof(Units.Pint)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal Pint
        {
            get => Liter * LPt;
            init => Liter = value * PtL;
        }
        public double PintD => (double)Pint;
        public float PintF => (float)Pint;

        [UnitProperty(nameof(Units.Ounce)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal Ounce
        {
            get => Liter * LOz;
            init => Liter = value * OzL;
        }
        public double OunceD => (double)Ounce;
        public float OunceF => (float)Ounce;

        public Volume() => Liter = 0;

        public Volume(decimal V, Units i) : this() => this[i] = V;

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
