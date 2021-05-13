using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DiegoG.Utilities.Reflection
{
    public static class ExtensionLoader
    {
        private static string ExtensionsDir;
        private static AppDomain ExtensionsDomain;
        private readonly static List<string> LoadedExtensionsList = new();
        
        public static IEnumerable<string> LoadedExtensions => LoadedExtensionsList;
        public static bool IsInit { get; private set; } = false;

        private static void ThrowIfNotInit()
        {
            if (!IsInit)
                throw new InvalidOperationException("Initialize the class first");
        }

        public static void Initialize(string extensionDir)
        {
            if (IsInit)
                throw new InvalidOperationException("Cannot initiaize twice");
            IsInit = true;

            ExtensionsDir = extensionDir;
            ExtensionsDomain = AppDomain.CurrentDomain;
        }

        public static IEnumerable<string> EnumerateUnloadedAssemblies()
        {
            ThrowIfNotInit();
            return EnumerateAssemblies().Where(s => !LoadedExtensionsList.Contains(s));
        }

        public static IEnumerable<string> EnumerateAssemblies()
        {
            ThrowIfNotInit();
            Directory.CreateDirectory(ExtensionsDir);
            return Directory.EnumerateFiles(ExtensionsDir, "*.dll", SearchOption.AllDirectories);
        }

        public static void Load(IEnumerable<string> paths)
        {
            var count = 0;
            var ch = new Stopwatch();
            ch.Start();
            Log.Information("Loading Extensions");
            foreach(var file in paths)
            {
                Log.Debug("Loading Assembly: " + file);
                var asm = Assembly.LoadFrom(file);
                Log.Debug("Loading assembly into AppDomain");
                ExtensionsDomain.Load(asm.GetName(false));
                LoadedExtensionsList.Add(file);
                count++;
            }
            Log.Information($"Loaded {count} new assemblies for a total of {LoadedExtensionsList.Count} extension assemblies. Total Time Taken: {ch.Elapsed}");
        }
    }
}
