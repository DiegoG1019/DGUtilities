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
        public static bool IsInit { get; private set; } = false;
        public static void Initialize(string extensionDir)
        {
            if (IsInit)
                throw new InvalidOperationException("Cannot initiaize twice");
            IsInit = true;

            ExtensionsDir = extensionDir;
            ExtensionsDomain = AppDomain.CurrentDomain;
        }
        private static string ExtensionsDir;
        private static AppDomain ExtensionsDomain;
        private readonly static List<string> LoadedExtensionsList = new();
        public static IEnumerable<string> LoadedExtensions => LoadedExtensionsList;
        public static void Load()
        {
            if (!IsInit)
                throw new InvalidOperationException("Initialize the class first");
            var count = 0;
            var ch = new Stopwatch();
            ch.Start();
            Log.Information("Reloading extensions");
            foreach 
            (
                var file in 
                Directory.EnumerateFiles(ExtensionsDir, "*.dll", SearchOption.AllDirectories)
                .Where(s=>!LoadedExtensionsList.Contains(s))
            )
            {
                Log.Debug("Loading Assembly: " + file);
                var asm = Assembly.LoadFrom(file);
                Log.Debug("Loading assembly into " + nameof(ExtensionsDomain));
                ExtensionsDomain.Load(asm.GetName(false));
                LoadedExtensionsList.Add(file);
                count++;
            }
            Log.Information($"Loaded {count} new assemblies for a total of {LoadedExtensionsList.Count} extension assemblies. Total Time Taken: {ch.Elapsed}");
        }
    }
}
