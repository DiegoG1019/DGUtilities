using DiegoG.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DiegoG.Benchmarks
{
    public static class Collections
    {
        public static BenchmarkResults TestCollection(Func<ICollection<long>> factory, int repeats = 10)
        {
            var obj = factory();
            var addrange = Enumerable.Repeat(0, 50000);
            var containsrange = Enumerable.Range(-10000, 40000);
            var removerange = Enumerable.Range(15000, 30000);

            List<BenchmarkResults.Run> runs = new(repeats);

            Stopwatch stopwatch = new();
            for (int reps = 0; reps < repeats; reps++)
            {
                List<(TimeSpan total, string comment)> results = new();

                stopwatch.Start();
                foreach (var i in addrange)
                    obj.Add(i);
                stopwatch.Stop();
                results.Add((stopwatch.Elapsed, "Added 50 000 Int64's"));

                stopwatch.Restart();
                foreach (var i in containsrange)
                    obj.Contains(i);
                stopwatch.Stop();
                results.Add((stopwatch.Elapsed, "Checking whether or not the collection contains some of the 60 000 subjects"));

                stopwatch.Restart();
                foreach (var i in removerange)
                    obj.Remove(i);
                stopwatch.Stop();
                results.Add((stopwatch.Elapsed, "Finding and Removing 15 000 elements"));

                var count = 0;
                stopwatch.Restart();
                count = obj.Count;
                obj.Clear();
                stopwatch.Stop();

                results.Add((stopwatch.Elapsed, "Counting and Clearing members"));
                runs.Add(new(results.Sum(s => s.total), results));
            }
            return new()
            {
                Title = "ICollection Benchmark",
                Runs = runs,
                TotalTimeTaken = runs.Sum(s => s.TotalTimeTaken),
            };
        }
    }
}


