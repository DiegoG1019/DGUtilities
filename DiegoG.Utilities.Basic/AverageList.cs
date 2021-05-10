using DiegoG.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.Utilities.Basic
{
    public class AverageList
    {
        private LoopbackIndexArray<double> ts { get; set; }
        public int SampleSize { get; private set; }
        public AverageList(int samples) => ts = new(samples);

        public void Resize(int newsize)
        {
            if (newsize < SampleSize)
                throw new ArgumentException("New sample size cannot be less than previous sample size", nameof(newsize));
            if (newsize == SampleSize)
                return;
            var newlia = new LoopbackIndexArray<double>(newsize);
            ts.CopyTo(newlia, 0);
            ts = newlia;
        }

        /// <summary>
        /// If the amount of samples exceed the sample capacity, it will simply loop back and replace previous samples
        /// </summary>
        /// <param name="sample"></param>
        public void AddSample(double sample) => ts.SetNext(sample);

        public override string ToString()
            => $"Peak: {Peak:.####}, Average: {Average:.####}, Valley: {Valley:.####}";

        public double Peak => ts.Max();
        public double Valley => ts.Min();
        public double Average => ts.Sum() / ts.NonDefault;
    }
}
