using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DiegoG.Utilities
{
    [Serializable]
    public struct Percentage
    {
        private static Random Rand { get; } = new Random();
        public const double Max = 100;
        public const double Min = 0;
        private double v;
        public double Value
        {
            get => v;
            set => v = value;
        }
        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public double Percent => Value / 100d;

        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public double AddPercent => Value / 100d + 1;

        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public string String => $"{v}%";
        public static bool ThrowChance(double chance) => new Percentage(chance).ThrowChance();

        public bool ThrowChance() => Value >= 100d || Rand.Next(0, 100) <= Value;

        public Percentage(double v) : this() => Value = v;

        public static implicit operator Percentage(double d) => new(d);

        public static implicit operator CappedPercentage(Percentage p) => new(p.Value);

        public static Percentage operator +(Percentage a, Percentage b) => new(a.Value + b.Value);

        public static Percentage operator -(Percentage a, Percentage b) => new(a.Value + b.Value);
    }
    [Serializable]
    public struct CappedPercentage
    {
        private static Random Rand { get; } = new Random();
        public const double Max = 100;
        public const double Min = 0;
        private double v;
        public double Value
        {
            get => v;
            set
            {
                v = value;
                v.Cap(0, 100);
            }
        }
        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public double Percent => Value / 100d;

        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public string String => $"{v}%";
        public static bool ThrowChance(double chance) => new Percentage(chance).ThrowChance();

        public bool ThrowChance() => Value >= 100d || Rand.Next(0, 100) <= Value;

        public CappedPercentage(double v) : this() => Value = v;

        public static implicit operator CappedPercentage(double d) => new(d);

        public static implicit operator Percentage(CappedPercentage p) => new(p.Value);

        public static CappedPercentage operator +(CappedPercentage a, CappedPercentage b) => new(a.Value + b.Value);

        public static CappedPercentage operator -(CappedPercentage a, CappedPercentage b) => new(a.Value + b.Value);
    }
}
