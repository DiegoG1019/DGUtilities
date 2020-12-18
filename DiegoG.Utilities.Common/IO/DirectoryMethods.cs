using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Concurrent;
using Serilog;

namespace DiegoG.Utilities.IO
{
    public static partial class DirectoryMethods
    {
        private static string TC(byte b) => new string(Convert.ToChar(b), 1);

        public static ReadOnlyCollection<string> IllegalFileNames { get; }
            = new(new string[] { "prn", "con", "aux", "nul", "com1", "com2", "com3", "com4", "com5", "com6", "com7", "com8", "com9", "lpt1", "lpt2", "lpt3", "lpt4", "lpt5", "lpt6", "lpt7", "lpt8", "lpt9" });

        public static ReadOnlyCollection<string> IllegalCharacters { get; }
            = new(new string[] { "..", "/", "\\", "<", ">", ":", "\"", "|", "?", "*", TC(0), TC(1), TC(2), TC(3), TC(4), TC(5), TC(6), TC(7), TC(8), TC(9), TC(10), TC(11), TC(12), TC(13), TC(14), TC(15), TC(16), TC(17), TC(18), TC(19), TC(20), TC(21), TC(22), TC(23), TC(24), TC(25), TC(26), TC(27), TC(28), TC(29), TC(30), TC(31) });

        public static ReadOnlyCollection<string> IllegalStarts { get; }
            = new(new string[] { " " });

        public static ReadOnlyCollection<string> IllegalEndings { get; }
            = new(new string[] { " " });

        public static bool CheckFileName(string filename)
        {
            AsyncTaskManager<bool> tasks = new();
            tasks.Run(() => IllegalCharacters.Any(d => filename.Contains(d)));
            tasks.Run(() => IllegalCharacters.Any(d => filename.StartsWith(d)));
            tasks.Run(() => IllegalCharacters.Any(d => filename.EndsWith(d)));
            tasks.Run(() => IllegalCharacters.Any(d => filename == d));
            tasks.WaitAll();
            return tasks.AllResults.AllTrue();
        }

        public static bool FixFileName(string filename, out string newfilename)
        {
            if (CheckFileName(filename))
                goto Success;
            if (IllegalFileNames.Any(d => filename == d))
                goto Failure;

            filename = filename.Trim();
            foreach (string s in IllegalCharacters)
                filename = filename.Replace(s, "_");
            try
            {
                foreach (string s in IllegalStarts)
                    while (filename.StartsWith(s))
                        filename = filename[(s.Length - 1)..(filename.Length - 1)];
                foreach (string s in IllegalEndings)
                    while (filename.EndsWith(s))
                        filename = filename[0..(filename.Length - s.Length - 1)];
            }
            catch (IndexOutOfRangeException)
            {
                goto Failure;
            }

            Success:
            newfilename = filename;
            return true;

            Failure:
            newfilename = null;
            return false;
        }

        public static bool CheckAccess(string directory)
        {
            try
            {
                Log.Verbose(dirstr + $"Checking access for directory {directory}");
                Directory.GetFiles(directory);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
    }
}
