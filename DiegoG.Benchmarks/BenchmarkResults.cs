using DiegoG.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.Benchmarks
{
    public sealed record BenchmarkResults
    {
        public string Title { get; init; }
        private static int Count;
        public record Run
        {
            public TimeSpan TotalTimeTaken { get; init; }
            public IEnumerable<(TimeSpan TimeTaken, string Comment)> Results { get; init; }
            public Run(TimeSpan totaltime, IEnumerable<(TimeSpan TimeTaken, string Comment)> results)
            {
                TotalTimeTaken = totaltime;
                Results = results;
            }
        }
        public TimeSpan TotalTimeTaken { get; init; }
        public IEnumerable<Run> Runs { get; init; }
        public TimeSpan AverageTimeTaken
        {
            get
            {
                if(ATT_Cache is null)
                {
                    TimeSpan Total = TimeSpan.Zero;
                    int count = 0;
                    foreach(var run in Runs)
                    {
                        Total += run.TotalTimeTaken;
                        count++;
                    }
                    ATT_Cache = Total / count;
                }
                return (TimeSpan)ATT_Cache;
            }
        }
        public TimeSpan LowestTimeTaken => Runs.Min(r => r.TotalTimeTaken);

        /// <summary>
        /// Takes Run Results Comments as keys
        /// </summary>
        public IReadOnlyDictionary<Run, IReadOnlyDictionary<string, TimeSpan>> Averages
        {
            get
            {
                if(Averages_Cache is null)
                {
                    Averages_Cache = new();
                    foreach(var r in Runs)
                    {
                        var dict = new Dictionary<string, TimeSpan>();
                        var buffer = new Dictionary<string, List<TimeSpan>>();
                        Averages_Cache[r] = dict;
                        foreach (var (ts, cm) in r.Results)
                        {
                            if (!buffer.ContainsKey(cm))
                                buffer.Add(cm, new());
                            buffer[cm].Add(ts);
                        }
                        foreach (var cm in buffer.Keys)
                            dict[cm] = buffer[cm].Sum();
                    }
                }
                return Averages_Cache;
            }
        }

        private readonly static object key = new();
        private int code;
        public BenchmarkResults()
        {
            lock (key)
            {
                code = Count;
                Count++;
            }
        }

        //Caches
        Dictionary<Run, IReadOnlyDictionary<string, TimeSpan>> Averages_Cache;
        TimeSpan? ATT_Cache;
    }
}
