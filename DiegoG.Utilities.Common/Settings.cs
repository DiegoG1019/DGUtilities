using DiegoG.Utilities.Collections;
using DiegoG.Utilities.Enumerations;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static DiegoG.Utilities.IO.Serialization;
using Serilog.Events;

namespace DiegoG.Utilities.Settings
{
    public interface ISettings
    {
        public string Version_Comment => "Changing this in the .json file will result in that specific file being invalid";
        public ulong Version { get; }
    }
    /// <summary>
    /// You're not actually supposed to use this one, inherit this in your own class and use that
    /// </summary>
    [Serializable]
    public abstract class ApplicationSettings : ISettings
    {
        public virtual ulong Version => 1;

        public bool Console { get; set; } = false;
        public LogEventLevel Verbosity { get; set; } = LogEventLevel.Information;
        public bool PauseOnWindowUnfocus { get; set; } = false;
#if DEBUG
        public ApplicationSettings()
        {
            Console = true;
            Verbosity = LogEventLevel.Verbose;
        }
#endif
    }
    /// <summary>
    /// The default is represented by what the properties are initialized to. It's not required to instatiate anything, Settings will do that for you.
    /// For example: "Settings`ApplicationSettings`.Current" will work after Settings`ApplicationSettings`.Initialize() is called.
    /// A separate "Settings" will be created by each type you call, for example, Settings`ApplicationSettings` is different from Settings`YourSettings`.
    /// By virtue of C# creating a new static type with its own state for every generic type used with it
    /// </summary>
    /// <typeparam name="T">The class that represents the settings</typeparam>
    public static class Settings<T> where T : ISettings, new()
    {
        public static T Default { get; private set; } = new T();
        public static T Current
        {
            get
            {
                //I don't want it to lock and hold up threads when it's not setting
                if (CurrentSettingsLocked)
                    lock (CurrentSettingsKey)
                        return CurrentField;
                return CurrentField;
            }
            private set //I'm really worried about concurrency here
            {
                CurrentSettingsLocked = true; // I think it's better if I set it before locking the object, so that a return doesn't happen right as itssetting the field
                lock (CurrentSettingsKey)
                {
                    CurrentSettingsLocked = true; //If another thread comes through and sets it to false, then the next one hops in and it's already set to false despite waiting for this to be unlocked, so the "get" will flow through freely.
                    CurrentField = value;
                    CurrentSettingsLocked = false;
                }
            }
        }
        private static T CurrentField;
        private static bool CurrentSettingsLocked = false;
        private static readonly object CurrentSettingsKey = new();

        public static string Directory { get; private set; }
        public static string FileName { get; private set; }

        /// <summary>
        /// => Path.Combine(Directory, File);
        /// </summary>
        public static string FullPath => Path.Combine(Directory, FileName);
        public static bool SettingsFileExist => File.Exists(FullPath + JsonExtension);

        //It would normally be better to place these in the class, but then we woulnd't have access to T and we'd have to override it
        private static readonly TypeInfo typeinfo = (TypeInfo)typeof(T);
        public static IEnumerable<Pair<string, object>> CurrentProperties
            => from item in typeinfo.GetProperties() select new Pair<string, object>(item.Name, item.GetValue(Current));
        public static IEnumerable<Pair<string, object>> DefaultProperties
        { get; } = from item in typeinfo.GetProperties() select new Pair<string, object>(item.Name, item.GetValue(Default));

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

#warning This doesn't do any checks as to whether it's the same type of file
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="filename"></param>
        /// <param name="version"></param>
        /// <returns>Whether the current version matches the version of the file</returns>
        public static bool CheckVersion(string directory, string filename, out ulong version)
        {
            var v = Parse.Json(directory, filename).RootElement.GetProperty("Version").GetUInt64();
            version = v;
            return Default.Version == v;
        }

        public static void RestoreToDefault() => Current = new();
        public static void Initialize(string directory, string fileName)
        {
            FileName = fileName;
            Directory = directory;
            Log.Debug($"Checking for existence of {fileName} settings in {directory}");
            if (SettingsFileExist)
            {
                Log.Debug($"Existence of {fileName} settings in {directory} verified, verifying version");

                if (!CheckVersion(directory, fileName, out ulong version))
                {
                    Log.Debug($"Version verified, unequal, restoring to default and creating a new file asynchronously.");
                    File.Move(Path.Combine(directory, fileName + JsonExtension), Path.Combine(directory, fileName + $"_{version}_old" + JsonExtension), true);
                    goto RestoreToDefault;
                }
#if DEBUG
                try
                {
                    Log.Debug($"Version verified, equal, loading {fileName}");
                    LoadSettings(directory, fileName);
                    return;
                }
                catch (JsonException e)
                {
                    Log.Error($"Exception caught: {e}");
                    Log.Debug($"{fileName} in {directory} is invalid, restoring to default and creating a new file asynchronously");
                }
#else
                LoadSettings(directory, fileName);
                return;
#endif
            }
            Log.Debug($"{fileName} in {directory} could not be found, restoring to default and creating a new file asynchronously");
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
