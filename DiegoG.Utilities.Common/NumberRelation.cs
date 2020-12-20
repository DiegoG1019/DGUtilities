using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DiegoG.Utilities
{
    [Serializable]
    public sealed class NumberRelation
    {
        [IgnoreDataMember, JsonIgnore, XmlIgnore]
        public ulong GCD { get; private set; }

        private UDecimal valuea;
        private UDecimal valueb;
        /// <summary>
        /// The minimum expression of BaseA in BaseA/BaseB
        /// </summary>
        [IgnoreDataMember, JsonIgnore, XmlIgnore]
        public UDecimal ValueA
        {
            get => valuea;
            set
            {
                basea = (value * Quotient) * baseb;
                Adjust();
            }
        }
        /// <summary>
        /// The minimum expression of BaseB in BaseA/BaseB
        /// </summary>
        [IgnoreDataMember, JsonIgnore, XmlIgnore]
        public UDecimal ValueB
        {
            get => valueb;
            set
            {
                baseb = (value * Quotient) * basea;
                Adjust();
            }
        }

        private UDecimal basea;
        private UDecimal baseb;
        /// <summary>
        /// The non-reduced expression A
        /// </summary>
        public UDecimal BaseA
        {
            get => basea;
            set
            {
                basea = value;
                Adjust();
            }
        }
        /// <summary>
        /// The non-reduced expression B
        /// </summary>
        public UDecimal BaseB
        {
            get => baseb;
            set
            {
                baseb = value;
                Adjust();
            }
        }

        /// <summary>
        /// Another name for BaseA for the sake of readability
        /// </summary>
        [IgnoreDataMember, JsonIgnore, XmlIgnore]
        public UDecimal Current
        {
            get => BaseA;
            set => BaseA = value;
        }

        /// <summary>
        /// Another name for BaseB for the sake of readability
        /// </summary>
        [IgnoreDataMember, JsonIgnore, XmlIgnore]
        public UDecimal Limit
        {
            get => BaseB;
            set => BaseB = value;
        }

        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public decimal Percentage => Quotient * 100;
        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public string PercentageString => String.Format("{0}%", Math.Round(Percentage, 2, MidpointRounding.ToEven));
        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public decimal RatioA => ValueA;
        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public decimal RatioB => ValueB;
        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public string RatioColon => String.Format("{0}:{1}", RatioA, RatioB);
        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public string RatioFraction => String.Format("{0}/{1}", RatioA, RatioB);
        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public string RatioText => String.Format("{0} to {1}", RatioA, RatioB);
        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public decimal Quotient => GCD == 0 ? 0 : RatioA / RatioB;
        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public decimal Difference => BaseA - BaseB;
        [JsonIgnore, IgnoreDataMember, XmlIgnore]
        public string ValuesString => String.Format("{0} out of {1}", BaseA, BaseB);
        public NumberRelation()
        {
            ValueA = 1;
            ValueB = 1;
            GCD = 1;
            basea = 1;
            baseb = 1;
            Adjust();
        }

        private void Adjust()
        {
            GCD = DiegoGMath.GreatestCommonDivisor((ulong)BaseA, (ulong)BaseB);
            if (GCD > 0)
            {
                valuea = BaseA / GCD;
                valueb = BaseB / GCD;
                return;
            }
            valuea = BaseA;
            valueb = BaseB;
        }

        public NumberRelation(decimal A, decimal B) :
            this()
        {
            BaseA = A;
            BaseB = B;
            Adjust();
        }

        public static readonly NumberRelation OneToOne = new NumberRelation(1, 1);
        public static readonly NumberRelation HundredToZero = new NumberRelation(100, 0);

    }
}
