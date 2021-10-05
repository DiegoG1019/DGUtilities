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
            Gradian,
            Cycle
        }

        public const double PI = 3.1415926535897932384626433;

        public const double RD = 180d / PI;
        public const double RG = 200d / PI;
        public const double DC = 1 / 360d;

        public const double CD = 360d;
        public const double DR = PI / 180d;
        public const double GR = PI / 200d;

        static Angle()
        {
            ShortUnitsDict.Add(Units.Radian, "rad");
            ShortUnitsDict.Add(Units.Degree, "º");
            ShortUnitsDict.Add(Units.Gradian, "\u1D4D");
            ShortUnitsDict.Add(Units.Cycle, "cyc");
            DefaultUnit = Units.Radian;
        }

        public enum Types
        {
            Zero = 2,
            Acute = 4,
            Right = 8,
            Obtuse = 16,
            Straight = 32,
            Reflex = 64,
            Full = 128
        }

        public bool Right => this == RightAngleLocal;
        public bool Straight => this == StraightAngleLocal;
        public bool FullOrPerigon => this == MaxValue;

        public bool AcuteOrSharp => this < RightAngleLocal;
        public bool ObtuseOrBlunt => this > RightAngleLocal && this < StraightAngleLocal;
        public bool Reflex => this > StraightAngleLocal && this < MaxValue;

        public Types Type
        {
            get
            {
                if (TypeField is null)
                {
                    TypeField =
                        !NotZero ? Types.Zero :
                        AcuteOrSharp ? Types.Acute :
                        Right ? Types.Right :
                        ObtuseOrBlunt ? Types.Obtuse :
                        Straight ? Types.Straight :
                        Reflex ? Types.Reflex : Types.Full;
                }
                return (Types)TypeField;
            }
        }
        public Types? TypeField;

        [UnitProperty(nameof(Units.Radian))]
        public double Radian
        {
            get => DefaultValue;
            init { DefaultValue = value; TypeField = null; }
        }
        public double RadianD => (double)Radian;
        public float RadianF => (float)Radian;

        [UnitProperty(nameof(Units.Cycle)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public double Cycle
        {
            get => Degree * DC;
            init => Degree = value * CD;
        }
        public double CycleD => (double)Cycle;
        public float CycleF => (float)Cycle;

        [UnitProperty(nameof(Units.Degree)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public double Degree
        {
            get => Radian * RD;
            init => Radian = value * DR;
        }
        public double DegreeD => (double)Degree;
        public float DegreeF => (float)Degree;

        [UnitProperty(nameof(Units.Gradian)), IgnoreDataMember, JsonIgnore, XmlIgnore]
        public double Gradian
        {
            get => Radian * RG;
            init => Radian = value * GR;
        }
        public double GradianD => (double)Gradian;
        public float GradianF => (float)Gradian;

        public Angle() : base() { }
        public Angle(double V, Units i = Units.Degree) : base(V, i) { }
        public Angle(Angle angle) : base(angle) { }

        public new static Angle Parse(string str) => Measure<Units, Angle>.Parse(str);

        public new static bool TryParse(string str, out Angle angle) => Measure<Units, Angle>.TryParse(str, out angle);

        public const double RightAngleValue = PI / 2d;
        public static Angle RightAngle => new Angle(RightAngleValue, Units.Radian);
        static readonly Angle RightAngleLocal = RightAngle;

        public static Angle ZeroAngle => new(0, Units.Radian);
        public static readonly Angle MinValue = ZeroAngle;

        public const double StraightAngleValue = PI;
        public static Angle StraightAngle => new(StraightAngleValue, Units.Radian);
        static readonly Angle StraightAngleLocal = StraightAngle;

        public const double FullAngleValue = 2 * PI;
        public static Angle FullAngle => new(FullAngleValue, Units.Radian);
        public static readonly Angle MaxValue = FullAngle;
    }
}
