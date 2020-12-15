using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DiegoG.Utilities
{
    public class TimedSequence<T>
    {
        private Timer TimerField;
        public Timer Timer
        {
            get => TimerField;
            set
            {
                TimerField = value;
                if (value is null)
                    Timer.Elapsed += (s, e) => Step();
            }
        }

        private bool AutonomousField = true;
        public bool Autonomous
        {
            get => AutonomousField;
            set
            {
                if (Autonomous == value)
                    return;
                AutonomousField = value;
                ResetTimer();
            }
        }

        public int PrevIndex { get; private set; }
        public int Index { get; private set; }
        public int NextIndex
        {
            get
            {
                var nst = NextStep();
                return nst < 0 ? 1 - nst : nst;
            }
        }

        public TimedSequence(TimeSpan interval, bool autonomous = false) : this(interval.TotalMilliseconds, autonomous) { }
        public TimedSequence(double interval, bool autonomous = false)
        {
            Autonomous = autonomous;
            Interval = interval;
        }

        private void ResetTimer()
        {
            Timer.TryDispose();
            if (Autonomous)
                Timer = new(Interval);
            else
                Timer = null;
        }

        public bool Reverse { get; set; }
        public bool IsReversing { get; private set; }

        private (int index, bool reverse, bool stopreverse) NextStep()
        {
            if (Index == TArray.Length)
                if (Reverse)
                    return (Index - 1, true, false);
                else
                    return (0, false, true);
            if (IsReversing && !(Index == 0))
                return (Index - 1, false, false);
            return ;
        }
        public void Step()
        {
            PrevIndex = Index;
            var nst = NextStep();
            if (nst < 0)
            {
                Index = 1 - nst;
                IsReversing = true;
            }
            Index = nst;
        }

        private double StepMs { get; set; }
        public void Update(double Ms)
        {
            StepMs += Ms;
            if (StepMs >= Interval)
            {
                StepMs = 0;
                Step();
            }
        }

        private void TrySetTimer()
        {
            if (Autonomous)
                Timer.Interval = Interval;
        }

        private double IntervalField;

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

        public T CurrentElement => TArray[Index];

        public ReadOnlyCollection<T> Items { get; private set; }
        public void SetItems(IEnumerable<T> items)
        {
            Items = new(items.ToList());
            Index = PrevIndex = 0;
        }
        private T[] TArrayField;
        private T[] TArray
        {
            get => TArrayField;
            set
            {
                TArrayField = value;
                SetItems(value);
            }
        }
    }
}
