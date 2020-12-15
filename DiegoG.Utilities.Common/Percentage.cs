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
        public bool Cap { get; set; }
        public double Value
        {
            get => v;
            set
            {
                v += value;
                if(Cap)
                    v.Cap(0, 100);
            }
        }
        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public double Percent => Value / 100d;

        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public string String => $"{v}%";
        public static bool ThrowChance(double chance) => new Percentage(chance).ThrowChance();
        public bool ThrowChance() => Value >= 100d || Rand.Next(0, 100) <= Value;
        public Percentage(double v) : this() => this.v = v;
        public Percentage(double v, bool cap) : this(v) => Cap = cap;
    }
}
