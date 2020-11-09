using DiegoG.Utilities.Enumerations;
using System.IO;
using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using static DiegoG.Utilities.IO.Serialization;
using DiegoG.Utilities.Collections;
using Serilog;
using System.Text.Json;
using DiegoG.Utilities.IO;

namespace DiegoG.Utilities.Settings
{
    public interface ISettings
    {
        public ulong Version { get; }
    }
    [Serializable]
    public abstract class ApplicationSettings : ISettings
    {
        public abstract ulong Version { get; }
        public bool VerbosityIsVerbose => Verbosity == Verbosity.Verbose;
        public bool VerbosityIsDebug => VerbosityIsVerbose || Verbosity == Verbosity.Debug;

        public bool Console { get; set; } = false;
        public Verbosity Verbosity { get; set; } = Verbosity.Normal;
        public bool PauseOnWindowUnfocus { get; set; } = false;
#if DEBUG
        public ApplicationSettings()
        {
            Console = true;
            Verbosity = Verbosity.Verbose;
        }
#endif
    }
    public static class Settings<T> where T : ISettings, new()
    {
        /// <summary>
        /// This shouldn't be used constantly, as it produces a new object each time
        /// </summary>
        public static T Default { get; set; } = new T();
        public static T Current { get; set; }

        public static string Directory { get; private set; }
        public static string FileName { get; private set; }

        /// <summary>
        /// => Path.Combine(Directory, File);
        /// </summary>
        public static string FullPath => Path.Combine(Directory, FileName);
        public static bool SettingsFileExist => File.Exists(FullPath + JsonExtension);

        //It would normally be better to place these in the class, but then we woulnd't have access to T and we'd have to override it
        private static TypeInfo typeinfo = (TypeInfo)typeof(T);
        public static IEnumerable<Pair<string, object>> CurrentProperties
            => from item in typeinfo.GetProperties() select new Pair<string, object>(item.Name, item.GetValue(Current));
        public static IEnumerable<Pair<string, object>> DefaultProperties
            => from item in typeinfo.GetProperties() select new Pair<string, object>(item.Name, item.GetValue(Default));

        public static void SaveSettings() => Serialize.Json(Current, Directory, FileName);
        public static async Task SaveSettingsAsync() => await Serialize.JsonAsync(Current, Directory, FileName);
        public static void LoadSettings(string directory, string fileName)
        {
            FileName = fileName;
            Directory = directory;
            Current = Deserialize<T>.Json(Directory, FileName);
        }
        public static async Task LoadSettingsAsync(string directory, string fileName)
        {
            var tsk = Deserialize<T>.JsonAsync(Directory, FileName);
            FileName = fileName;
            Directory = directory;
            Current = await tsk;
        }

        public static bool CheckVersion(string directory, string filename)
            => Default.Version == Parse.Json(directory, filename).RootElement.GetProperty("Version").GetUInt64();

        public static void RestoreToDefault() => Current = Default;
        public static void Initialize(string directory, string fileName)
        {
            FileName = fileName;
            Directory = directory;
            Log.Debug($"Checking for existence of {fileName} settings in {directory}");
            if (SettingsFileExist)
            {
                Log.Debug($"Existence of {fileName} settings in {directory} verified, verifying version");
                
                if(!CheckVersion(directory, fileName))
                {
                    Log.Debug($"Version verified, unequal, restoring to default and creating a new file asynchronously.");
                    File.Move(Path.Combine(directory, fileName), Path.Combine(directory, fileName + "_old"), true);
                    goto RestoreToDefault;
                }
#if DEBUG
                try
                {
                    LoadSettings(directory, fileName);
                    return;
                }catch(JsonException e)
                {
                    Log.Debug($"Exception caught: {e.Message}");
                    Log.Debug($"{fileName} in {directory} is invalid, restoring to default and creating a new file asynchronously.");
                }
#else
                LoadSettings(directory, fileName);
                return;
#endif
            }
            Log.Debug($"{fileName} in {directory} could not be found, restoring to default and creating a new file asynchronously.");
            RestoreToDefault:;
            RestoreToDefault();
#if !DEBUG
                _ = SaveSettingsAsync();
#else
            SaveSettings();
#endif
        }
    }
}
