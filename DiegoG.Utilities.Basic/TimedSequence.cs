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
                ResetTimer();
            }
        }
        private double IntervalField;

        private T[] TArray
        {
            get => TArrayField;
            set => SetItems(value);
        }
        private T[] TArrayField;

        public Timer Timer
        {
            get => TimerField;
            set
            {
                if (ReferenceEquals(TimerField, value))
                    return;
                if(TimerField is not null)
                {
                    TimerField.Elapsed -= Timer_Elapsed;
                    TimerField.Dispose();
                }
                TimerField = value;
                if (value is not null)
                {
                    TimerField.Elapsed += Timer_Elapsed;
                    TimerField.Interval = Interval;
                    ResetTimer();
                }
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e) { Step(); ResetTimer(); }

        private Timer TimerField;

        public bool Autonomous
        {
            get => AutonomousField;
            set
            {
                if (AutonomousField == value)
                    return;
                else Timer = value ? (new(Interval)) : (Timer)null; //If value is true, then it was previously false, as it's not equal to its previous value
                AutonomousField = value;
            }
        }
        private bool AutonomousField = false;

        private void ResetTimer() 
        {
            if (Timer is null)
                return;
            Timer.Stop();
            Timer.Interval = Interval; 
            Timer.Start(); 
        }

        private (int index, bool reverse) NextStep()
        {
            if (!IsReversing)
                if (Index != TArray.Length - 1)
                    return (Index + 1, false);
                else if (Reverse)
                    return (Index, true);
                else
                    return (0, false);
            else if (Index > 0)
                return (Index - 1, true);
            else
                return (Index + 1, false);
        }

        public void Step()
        {
            var (ind, reverse) = NextStep();
            Index = ind;
            IsReversing = reverse;
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

        public void SetItems(IEnumerable<T> items)
        {
            TArrayField = items.ToArray();
            Items = new(TArrayField);
            Buffer = Index = PrevIndex = 0;
        }

        public TimedSequence(int intervalsPerSecond, IEnumerable<T> items, bool autonomous = true) : this(1d / (intervalsPerSecond * 1000), items, autonomous) { }
        public TimedSequence(TimeSpan interval, IEnumerable<T> items, bool autonomous = true) : this(interval.TotalMilliseconds, items, autonomous) { }
        public TimedSequence(double interval, IEnumerable<T> items, bool autonomous = true)
        {
            SetItems(items);
            Interval = interval;
            Autonomous = autonomous;
        }

    }
}
