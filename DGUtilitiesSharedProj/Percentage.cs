using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DiegoG.Utilities
{
    public struct Percentage
    {
        private static Random Rand { get; } = new Random();
        public const float Max = 100;
        public const float Min = 0;
        private float v;
        public float Value
        {
            get
            {
                return v;
            }
            set
            {
                double a = 0;
                if ((value + a) > Max)
                {
                    v = Max;
                    return;
                }
                if ((value + a) < Min)
                {
                    v = Min;
                    return;
                }
                v += value;
            }
        }
        [IgnoreDataMember]
        [JsonIgnore]
        public double Percent => Value / 100f;

        [IgnoreDataMember]
        [JsonIgnore]
        public string String => $"{v}%";
        public static bool ThrowChance(float chance) => new Percentage(chance).ThrowChance();

        public bool ThrowChance()
        {
            if (Value == 100f)
            {
                return true;
            }

            return Rand.Next(0, 100) <= Value;
        }

        public Percentage(float v) :
            this()
        {
            this.v = v;
        }

    }
}
