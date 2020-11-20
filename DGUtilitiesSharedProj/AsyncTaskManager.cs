using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace DiegoG.Utilities
{
#warning Improve this, as it is now it risks flooding the threadpool with tasks. Limit the maximum amount of parallel tasks per manager
    public class AsyncTaskManager<T> : IEnumerable<Task<T>>
    {
        private List<Task<T>> Ts { get; } = new List<Task<T>>();
        public Task<T> this[int index] => Ts[index];
        private int AddToList(Task<T> tsk)
        {
            Ts.Add(tsk);
            WhenAllFieldReload = true;
            return Ts.Count - 1;
        }
        public Task<T>[] AllTasks => Ts.ToArray();
        public T[] AllResults => WhenAll.Result;
        public bool AllTasksCompleted => WhenAll.IsCompleted;
        public bool AllTasksCompletedSuccesfully => WhenAll.IsCompletedSuccessfully;
        public bool AnyTasksFaulted => WhenAll.IsFaulted;
        public IEnumerable<(int index, AggregateException Exception)> Exceptions
            => from task in this where task.IsFaulted select (Ts.IndexOf(task), task.Exception);
        public void Clear() => Ts.Clear();
        public int Run(Func<T> func) => AddToList(Task.Run(func));
        public int Run(Task<T> task) => AddToList(task);
        public int Run(Func<Task<T>> func) => AddToList(Task.Run(func));

        private Task<T[]> WhenAllField;
        private bool WhenAllFieldReload = true;
        public Task<T[]> WhenAll
        {
            get
            {
                if (WhenAllFieldReload)
                {
                    WhenAllField = Task.WhenAll(AllTasks);
                    WhenAllFieldReload = false;
                }
                return WhenAllField;
            }
        }
        public Task<Task<T>> WhenAny => Task.WhenAny(AllTasks);
        public void WaitAll() => Task.WaitAll(AllTasks);
        public void WaitAny() => Task.WaitAny(AllTasks);
        public IEnumerator<Task<T>> GetEnumerator() => Ts.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public static implicit operator Task<T[]>(AsyncTaskManager<T> obj) => obj.WhenAll;
    }
    public class AsyncTaskManager : IEnumerable<Task>
    {
        private List<Task> Ts { get; } = new List<Task>();
        public Task this[int index] => Ts[index];
        private int AddToList(Task tsk)
        {
            Ts.Add(tsk);
            WhenAllFieldReload = true;
            return Ts.Count - 1;
        }
        public Task[] AllTasks => Ts.ToArray();
        public bool AllTasksCompleted => WhenAll.IsCompleted;
        public bool AllTasksCompletedSuccesfully => WhenAll.IsCompletedSuccessfully;
        public bool AnyTasksFaulted => WhenAll.IsFaulted;
        public IEnumerable<(int index, AggregateException Exception)> Exceptions
            => from task in this where task.IsFaulted select (Ts.IndexOf(task), task.Exception);
        public void Clear() => Ts.Clear();
        public int Run(Action func) => AddToList(Task.Run(func));
        public int Run(Task task) => AddToList(task);
        public int Run(Func<Task> func) => AddToList(Task.Run(func));

        private Task WhenAllField;
        private bool WhenAllFieldReload = true;
        public Task WhenAll
        {
            get
            {
                if (WhenAllFieldReload)
                {
                    WhenAllField = Task.WhenAll(AllTasks);
                    WhenAllFieldReload = false;
                }
                return WhenAllField;
            }
        }

        private Task WhenAnyField;
        private bool WhenAnyFieldReload = true;
        public Task WhenAny
        {
            get
            {
                if (WhenAnyFieldReload)
                {
                    WhenAnyField = Task.WhenAny(AllTasks);
                    WhenAnyFieldReload = false;
                }
                return WhenAnyField;
            }
        }
        public void WaitAll() => Task.WaitAll(AllTasks);
        public void WaitAny() => Task.WaitAny(AllTasks);
        public IEnumerator<Task> GetEnumerator() => Ts.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public static implicit operator Task(AsyncTaskManager obj) => obj.WhenAll;
    }
}
