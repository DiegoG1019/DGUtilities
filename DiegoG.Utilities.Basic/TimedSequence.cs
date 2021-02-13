using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;

namespace DiegoG.Utilities
{
    public class TimedSequence<T>
    {
        public bool Reverse { get; set; }
        public int PrevIndex { get; private set; }
        public int Index { get; private set; }
        public int NextIndex { get; private set; }
        public bool IsReversing { get; private set; }
        public ReadOnlyCollection<T> Items { get; private set; }

        private double Buffer { get; set; }

        public T CurrentElement => TArray[Index];
        public double IntervalsPerSecond => 1d / Interval;

        /// <summary>
        /// Expressed in milliseconds
        /// </summary>
        public double Interval
        {
            get => IntervalField;
            set
            {
                IntervalField = value;
                TrySetTimer();
            }
        }
        private double IntervalField;

        private T[] TArray
        {
            get => TArrayField;
            set
            {
                TArrayField = value;
                SetItems(value);
            }
        }
        private T[] TArrayField;

        public Timer Timer
        {
            get => TimerField;
            set
            {
                TimerField = value;
                if (value is null)
                {
                    Timer.Elapsed += (s, e) => Step();
                }
            }
        }
        private Timer TimerField;

        public bool Autonomous
        {
            get => AutonomousField;
            set
            {
                if (Autonomous == value)
                {
                    return;
                }

                AutonomousField = value;
                ResetTimer();
            }
        }
        private bool AutonomousField = true;

        public TimedSequence(int intervalsPerSecond, bool autonomous = true) : this(1d / (intervalsPerSecond * 1000), autonomous) { }
        public TimedSequence(TimeSpan interval, bool autonomous = true) : this(interval.TotalMilliseconds, autonomous) { }
        public TimedSequence(double interval, bool autonomous = true)
        {
            Autonomous = autonomous;
            Interval = interval;
        }

        private void ResetTimer()
        {
            Timer.TryDispose();
            if (Autonomous)
            {
                Timer = new(Interval);
            }
            else
            {
                Timer = null;
            }
        }

        private (int index, bool reverse) NextStep()
        {
            if (!IsReversing)
            {
                if (Index == TArray.Length - 1)
                {
                    return (Index + 1, false);
                }
                else if (Reverse)
                {
                    return (Index, true);
                }
                else
                {
                    return (0, false);
                }
            }

            if (Index > 0 && IsReversing)
            {
                return (Index - 1, true);
            }

            return (Index + 1, false);
        }
        public void Step()
        {
            var (ind, reverse) = NextStep();
            Index = ind; IsReversing = reverse;
            NextIndex = NextStep().index;
        }

        public void Update(double Ms)
        {
            Buffer += Ms;
            if (Buffer >= Interval)
            {
                Buffer = 0;
                Step();
            }
        }
        private void TrySetTimer()
        {
            if (Autonomous)
            {
                Timer.Interval = Interval;
            }
        }
        public void SetItems(IEnumerable<T> items)
        {
            Items = new(items.ToList());
            Index = PrevIndex = 0;
            Buffer = 0;
        }
    }
}
