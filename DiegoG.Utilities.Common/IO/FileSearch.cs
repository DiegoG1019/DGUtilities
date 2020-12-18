using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.Utilities.IO
{
    public static partial class DirectoryMethods
    {
        /// <summary>
        /// Sweeps through all of the accessible directories below the given rootdir and enumerates found elements
        /// </summary>
        /// <param name="searchkey">The file or directory to match</param>
        /// <param name="findDirectory">Whether to try to match directory names</param>
        /// <param name="results">The amount of results to be matched. 0 to keep matching until everything's been checked</param>
        /// <param name="resultList">An optional list to be given if you intend to keep track of results as they become available.</param>
        /// <param name="rootdirpath">The strings defining the root directory from where to start the search</param>
        /// <returns>An enumeration containing all the results that were found</returns>
        public async static Task<IEnumerable<string>> FindFile(string searchkey, bool findDirectory = false, int results = 1, ConcurrentDataType<ObservableCollection<string>> resultList = null, params string[] rootdirpath) => await FindFile(searchkey, findDirectory, results, Path.Combine(rootdirpath), resultList);

        
        private const int ConcurrentTasks = 300;
        private const string dirstr = "DirectoryMethods.FindFile: ";

        /// <summary>
        /// Sweeps through all of the accessible directories below the given rootdir and enumerates found elements
        /// </summary>
        /// <param name="searchkey">The file or directory to match</param>
        /// <param name="findDirectory">Whether to try to match directory names</param>
        /// <param name="results">The amount of results to be matched. 0 to keep matching until everything's been checked</param>
        /// <param name="rootdir">The directory from where to start the search</param>
        /// <param name="resultList">An optional list to be given if you intend to keep track of results as they become available.</param>
        /// <returns>An enumeration containing all the results that were found</returns>
        public async static Task<IEnumerable<string>>
            FindFile(string searchkey, bool findDirectory = false, int results = 1, string rootdir = "C:\\", ConcurrentDataType<ObservableCollection<string>> resultList = null)
        {
            Log.Verbose(dirstr + $"Initializing search for {searchkey} {(findDirectory ? "Directory or File" : "File")} in {rootdir}");

            //Check if the directory exists before instantiating anything
            if (!Directory.Exists(rootdir))
                throw new ArgumentException($"{nameof(rootdir)} \"{rootdir}\" does not exist.");
            
            ConcurrentDataType<int> resultCount = 0;
            if (resultList is null)
                resultList = new();

            //Instantiates necessary objects and Enqueues rootdir
            AsyncTaskManager<(bool found, string path), ConcurrentQueue<string>> taskman = new(new()) { AutoClear = false };
            taskman.CommonData.Enqueue(rootdir);

            Log.Verbose(dirstr + "Initializing Manager Task");


            //The loop responsible for managing the tasks
            for (; ; )
            {
                //Check if any task has yielded results
                foreach(var s in taskman.AllResults.Where(t => t.found).Select(d => d.path))
                {
                    if (results != 0)
                    {
                        resultCount.Operate(d => d + 1);
                        resultList.Operate(d => d.Add(s));
                    }
                    if (resultCount > results)
                        return resultList.Data;
                }

                //Clears all completed tasks, we know it's completed because we await it.
                taskman.Clear();

                //As long as the current amount of running tasks does not exceed 300, we Dequeue the next path to search and, if we have access, check for the file and course through it.
                if (taskman.Count < ConcurrentTasks && taskman.CommonData.TryDequeue(out string pathToSearch) && CheckAccess(pathToSearch))
                {
                    Log.Verbose(dirstr + "Running new task for directory pathToSearch");
                    taskman.Run
                    (
                        () =>
                        {
                            var (found, path) = Check(pathToSearch, searchkey, findDirectory);
                            foreach (var s in Directory.EnumerateDirectories(pathToSearch))
                            {
                                Log.Verbose(dirstr + "Enqueing directory: ");
                                taskman.CommonData.Enqueue(s);
                            }
                            return (found, path);
                        }
                    );
                }
                if (taskman.Count < 1 && taskman.CommonData.IsEmpty)
                    return resultList.Data;
                await taskman.WhenAll;
            }

            static (bool found, string path) Check(string dir, string searchkey, bool findDirectory)
            {
                string spath = Path.Combine(dir, searchkey);

                if (findDirectory && Directory.Exists(spath))
                    return (true, spath);

                if (File.Exists(spath))
                    return (true, spath);
                return (false, "");
            }
        }
    }
}
