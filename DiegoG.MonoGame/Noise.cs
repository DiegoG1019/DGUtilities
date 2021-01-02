using System;
using DiegoG.Utilities;
using DiegoG.Utilities.Measures;

namespace DiegoG.MonoGame
{
    public class Noise
    {
        public enum Units
        {
            Decibel
        }
        float _decibels;
        public float Decibels
        {
            get
            {
                return _decibels;
            }
            set
            {
                _decibels = value;
                AudibleRange = new Length(((decimal)(1 / DiegoGMath.NthRoot(Math.Pow(10, -Decibels), 20))), Length.Units.Meter);
            }
        }
        public Length AudibleRange { get; set; }
        public double GetLoudness(Length distance)
        {
            return Decibels + (20 * Math.Log10((double)(1 / distance.Meter)));
        }
        /// <summary>
        /// Set the value in Decibels to be the sound intensity at 1m distance from the source
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        public Noise(float value, Units unit){
            switch (unit)
            {
                case Units.Decibel:
                    Decibels = value;
                    return;
            }
            throw new InvalidOperationException("Invalid Unit");
        }

        public static readonly Noise Zero = new Noise(0, Units.Decibel);
    }
}
