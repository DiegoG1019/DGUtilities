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
        public const float Max = 100;
        public const float Min = 0;
        private float v;
        public float Value
        {
            get => v;
            set
            {
                v += value;
                v.Cap(0, 100);
            }
        }
        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public double Percent => Value / 100f;

        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public string String => $"{v}%";
        public static bool ThrowChance(float chance) => new Percentage(chance).ThrowChance();

        public bool ThrowChance()
        {
            if (Value == 100f)
                return true;
            return Rand.Next(0, 100) <= Value;
        }
        public Percentage(float v) : this() => this.v = v;
    }
}
