using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DiegoG.Utilities
{
    public class AsyncTaskManager<T> : IEnumerable<Task<T>>
    {
        private readonly List<Task<T>> Ts = new List<Task<T>>();
        private readonly Dictionary<string, Task<T>> NamedTs = new();
        public TaskAwaiter<T[]> GetAwaiter() => WhenAll.GetAwaiter();

        public Task<T> this[int index] => Ts[index];
        public Task<T> this[string name] => NamedTs[name];
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
        public int Count => Ts.Count;
        public IEnumerable<(int index, AggregateException Exception)> Exceptions
            => from task in this where task.IsFaulted select (Ts.IndexOf(task), task.Exception);
        public void Clear() { Ts.Clear(); NamedTs.Clear(); }

        public int Run(Func<T> func) => AddToList(Task.Run(func));

        public int Run(Func<Task<T>> func) => AddToList(Task.Run(func));

        public int Add(Task<T> task) => AddToList(task);

        public int Add(Task<T> task, string name)
        {
            NamedTs.Add(name, task);
            return AddToList(task);
        }

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

        public IEnumerable<(Task<T> Task, string Name)> GetNamedTasks()
        {
            foreach (var (name, task) in NamedTs)
                yield return (task, name);
        }

        public IEnumerator<Task<T>> GetEnumerator() => Ts.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static implicit operator Task<T[]>(AsyncTaskManager<T> obj) => obj.WhenAll;
    }
    public class AsyncTaskManager<T, T1> : AsyncTaskManager<T>
    {
        public T1 CommonData { get; init; }
        public AsyncTaskManager(T1 commonData, bool autoclear = true) : base() => CommonData = commonData;
        /// <summary>
        /// Defines whether this should call the object's void Clear() method (If it exists)
        /// </summary>
        public bool AutoClear { get; set; }
    }
    public class AsyncTaskManager : IEnumerable<Task>
    {
        private readonly List<Task> Ts = new List<Task>();
        private readonly Dictionary<string, Task> NamedTs = new();

        public TaskAwaiter GetAwaiter()
        {
            Task.Run(async () =>
            {
                await WhenAll;
                Clear();
            });
            return WhenAll.GetAwaiter();
        }

        public Task this[int index] => Ts[index];
        public Task this[string name] => NamedTs[name];
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
        public int Count => Ts.Count;
        public IEnumerable<(int index, AggregateException Exception)> Exceptions
            => from task in this where task.IsFaulted select (Ts.IndexOf(task), task.Exception);
        public void Clear() { Ts.Clear(); NamedTs.Clear(); }

        public int Run(Action func) => AddToList(Task.Run(func));

        public int Run(Func<Task> func) => AddToList(Task.Run(func));

        public int Add(Task task) => AddToList(task);

        public int Add(Task task, string name)
        {
            NamedTs.Add(name, task);
            return AddToList(task);
        }


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

        public IEnumerator<(Task Task, string Name)> GetNamedTasks()
        {
            foreach (var (name, task) in NamedTs)
                yield return (task, name);
        }

        public IEnumerator<Task> GetEnumerator() => Ts.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static implicit operator Task(AsyncTaskManager obj) => obj.WhenAll;
    }
}
