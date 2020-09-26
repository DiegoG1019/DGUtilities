using System;

namespace DiegoG.Utilities
{
    [Serializable]
    public sealed class NumberRelation
    {
        public ulong GCD { get; private set; }

        private UDecimal valuea;
        private UDecimal valueb;
        public UDecimal ValueA
        {
            get
            {
                return valuea;
            }
            set
            {
                basea = (value * Quotient) * baseb;
                Adjust();
            }
        }
        public UDecimal ValueB
        {
            get
            {
                return valueb;
            }
            set
            {
                baseb = (value * Quotient) * basea;
                Adjust();
            }
        }

        private UDecimal basea;
        private UDecimal baseb;
        public UDecimal BaseA
        {
            get
            {
                return basea;
            }
            set
            {
                basea = value;
                Adjust();
            }
        }
        public UDecimal BaseB
        {
            get
            {
                return baseb;
            }
            set
            {
                baseb = value;
                Adjust();
            }
        }

        /// <summary>
        /// Another name for BaseA for the sake of readability
        /// </summary>
        public UDecimal Current
        {
            get
            {
                return BaseA;
            }
            set
            {
                BaseA = value;
            }
        }

        /// <summary>
        /// Another name for BaseB for the sake of readability
        /// </summary>
        public UDecimal Limit
        {
            get
            {
                return BaseB;
            }
            set
            {
                BaseB = value;
            }
        }

        public decimal Percentage
        {
            get
            {
                return Quotient * 100;
            }
        }

        public string PercentageString
        {
            get
            {
                return String.Format("{0}%", Math.Round(Percentage, 2, MidpointRounding.ToEven));
            }
        }

        public decimal RatioA
        {
            get
            {
                return (decimal)ValueA;
            }
        }
        public decimal RatioB
        {
            get
            {
                return (decimal)ValueB;
            }
        }

        public string RatioColon
        {
            get
            {
                return String.Format("{0}:{1}", RatioA, RatioB);
            }
        }

        public string RatioFraction
        {
            get
            {
                return String.Format("{0}/{1}", RatioA, RatioB);
            }
        }

        public string RatioText
        {
            get
            {
                return String.Format("{0} to {1}", RatioA, RatioB);
            }
        }

        public decimal Quotient
        {
            get
            {
                return GCD == 0 ? 0 : RatioA / RatioB;
            }
        }

        public decimal Difference
        {
            get
            {
                return BaseA - BaseB;
            }
        }

        public string ValuesString
        {
            get
            {
                return String.Format("{0} out of {1}", BaseA, BaseB);
            }
        }

        private NumberRelation()
        {
            ValueA = 1;
            ValueB = 1;
            GCD = 1;
            basea = 1;
            baseb = 1;
        }

        private void Adjust()
        {
            GCD = MoreMath.GreatestCommonDivisor((ulong)BaseA, (ulong)BaseB);
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
