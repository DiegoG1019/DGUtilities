using System;
using System.Collections.Immutable;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DiegoG.Utilities.Measures
{
    public class Angle : Measure<Angle.Units, Angle>
    {
        [XmlType(TypeName = "AngleUnits")]
        public enum Units
        {
            Degree,
            Radian,
            Gradian
        }

        public const decimal PI = (decimal)Math.PI;

        public const decimal RD = 180M / PI;
        public const decimal RG = 200M / PI;

        public const decimal DR = PI / 180M;
        public const decimal GR = PI / 200M;

        static Angle()
        {
            var builder = ImmutableDictionary.CreateBuilder<Units, string>();
            builder.Add(Units.Radian, "rad");
            builder.Add(Units.Degree, "º");
            builder.Add(Units.Gradian, "\u1D4D");
            ShortUnits = builder.ToImmutable();
            DefaultUnit = Units.Radian;
        }

        public bool Right => this == RightAngle;

        [UnitProperty(nameof(Units.Radian))]
        public decimal Radian
        {
            get => DefaultValue;
            set => DefaultValue = value;
        }

        [UnitProperty(nameof(Units.Degree)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal Degree
        {
            get => Radian * RD;
            set => Radian = value * DR;
        }

        [UnitProperty(nameof(Units.Gradian)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public decimal Gradian
        {
            get => Radian * RG;
            set => Radian = value * GR;
        }

        public Angle() : base() { }
        public Angle(decimal V, Units i) : base(V, i) { }
        public Angle(Angle angle) : base(angle) { }

        public static Angle RightAngle => new Angle(PI / 2M, Units.Radian);
    }
}
