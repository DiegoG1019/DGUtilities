﻿using DiegoG.Utilities.Collections;
using DiegoG.Utilities.IO;
using PropertyChanged;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using static DiegoG.Utilities.IO.Serialization;

namespace DiegoG.Utilities.Settings
{
    public interface ISettings : INotifyPropertyChanged
    {
        /// <summary>
        /// Changing this in the .json file will result in that specific file being invalid
        /// </summary>
        public string Version_Comment => "";

        /// <summary>
        /// Specifies what type of settings this file is, prevents incorrect loading of other Settings files
        /// </summary>
        public string SettingsType { get; }
        /// <summary>
        /// Specifies what version this settings file is, prevents incorrect loading of files targeting modified classes
        /// </summary>
        public ulong Version { get; }
    }
    public interface ICommentedSettings : ISettings
    {
        public string[]? _Comments { get; }
        public Dictionary<string, string>? _Usage { get; }
    }
    /// <summary>
    /// You're not actually supposed to use this one, inherit this in your own class and use that
    /// </summary>
    [Serializable, AddINotifyPropertyChangedInterface]
    public abstract class ApplicationSettings : ISettings
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPC([CallerMemberName] string propertyName = "") => PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        public virtual ulong Version => 3;

        public abstract string SettingsType { get; }

        public virtual bool Console { get; set; }

        public virtual LogEventLevel Verbosity { get; set; }
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
    public static class Settings<T> where T : class, ISettings, new()
    {
        public static event PropertyChangedEventHandler SettingsChanged;
        private readonly static Type TType = typeof(T);
        private static T Default { get; } = new T();
        private static Dictionary<PropertyInfo, object> ChangesDict { get; } = new();
        public static bool HasChanged { get; private set; }

        /// <summary>
        /// Returns null if the property has not changed
        /// </summary>
        public static ReadOnlyIndexedProperty<PropertyInfo, object> Changes { get; } = new(i => ChangesDict.ContainsKey(i) ? ChangesDict[i] : null);
        public static bool HasPropertyChanged(PropertyInfo i) => ChangesDict.ContainsKey(i);
        public static IEnumerable<(PropertyInfo Property, object Value)> GetChanges() => from i in ChangesDict select (i.Key, i.Value);
        public static PropertyInfo GetProperty(string propertyName) => TType.GetProperty(propertyName);

        /// <summary>
        /// Returns a copy of current settings for modification, that can then be applied using Apply(T)
        /// </summary>
        /// <returns></returns>
        public static T GetModifiable() => Current.CopyByBinarySerialization() as T;

        public static void Apply(T current) => Current = current;

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
                    if(CurrentField is not null)
                        CurrentField.PropertyChanged -= Current_PropertyChanged;
                    CurrentField = value;
                    CurrentField.PropertyChanged += Current_PropertyChanged;
                    CurrentSettingsLocked = false;
                }
            }
        }
        private static T CurrentField;
        private static bool CurrentSettingsLocked = false;
        private static readonly object CurrentSettingsKey = new();

        /// <summary>
        /// Finds the value specified in ordinary C# object resolution format (Object.Property)
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static string CurrentGetString(string address) => CurrentGetString(address.Split('.'));

        /// <summary>
        /// Finds the value specified in ordinary C# object resolution format (Object.Property), each item being a dot division
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static string CurrentGetString(params string[] address) => Other.GetProperty(Current, TType, address) as string;

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

        public static Task SaveSettingsAsync() => Serialize.JsonAsync(Current, Directory, FileName);

        public static Task<string> SerializeSettingsAsync() => Serialize.JsonAsync(Current);
        public static string SerializeSettings() => Serialize.Json(Current);

        public static async Task DeserializeSettingsAsync(string jsonstring) => Current = await Deserialize<T>.JsonAsync(jsonstring);
        public static void DeserializeSettings(string jsonstring) => Current = Deserialize<T>.Json(jsonstring);

        private static void LoadSettings(string directory, string fileName, bool doCheck, Func<T, bool> validation)
        {
            if (doCheck && CheckFile(directory, fileName, out _, out _) != CheckFileCode.ValidFile)
                throw new InvalidDataException("The supplied settings are not valid");
            FileName = fileName;
            Directory = directory;
            var stgs = Deserialize<T>.Json(Directory, FileName);
            if (!(validation?.Invoke(stgs) ?? true))
                throw new InvalidDataException("The loaded settings did not pass validation");
            Current = stgs;
        }
        public static void LoadSettings(string directory, string fileName, Func<T, bool> validation = null)
            => LoadSettings(directory, fileName, true, validation);

        public static async Task LoadSettingsAsync(string directory, string fileName, Func<T, bool> validation = null)
        {
            if (CheckFile(directory, fileName, out _, out _) != CheckFileCode.ValidFile)
                throw new InvalidDataException("The supplied settings are not valid");
            var tsk = Deserialize<T>.JsonAsync(directory, fileName);
            FileName = fileName;
            Directory = directory;
            var stgs = await tsk;
            if (!(validation?.Invoke(stgs) ?? true))
                throw new InvalidDataException("The loaded settings did not pass validation");
            Current = stgs;
        }

        public enum CheckFileCode : byte
        {
            InvalidFile,
            DifferentType,
            DifferentVersion,
            ValidFile
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="filename"></param>
        /// <param name="version"></param>
        public static CheckFileCode CheckFile(string directory, string filename, out ulong version, out string type)
        {
            var jsel = Parse.Json(directory, filename).RootElement;
            try
            {
                version = jsel.GetProperty(nameof(ISettings.Version)).GetUInt64();
                type = jsel.GetProperty(nameof(ISettings.SettingsType)).GetString();
            }
            catch (KeyNotFoundException)
            {
                version = ulong.MaxValue;
                type = "__NO_TYPE_SPECIFIED__";
                return CheckFileCode.InvalidFile;
            }
            return Default.SettingsType == type ? Default.Version == version ? CheckFileCode.ValidFile : CheckFileCode.DifferentVersion : CheckFileCode.DifferentType;
        }

        public static void RestoreToDefault() => Current = new();

        /// <summary>
        /// Serializes all the given setting instances under the specific filename
        /// </summary>
        /// <param name="declaringType">The type where to look. All objects must be static and be marked with DefaultSettingsObjectAttribute</param>
        /// <param name="overwrite">Whether or not to overwrite the file if it already exists</param>
        public static void CreateDefaults(IEnumerable<(T obj, string name)> objects, bool overwrite = false)
        {
            foreach(var(obj, name) in objects)
                if (overwrite || !File.Exists(Path.Combine(Directory, name + JsonExtension)))
                    Serialize.Json(obj, Directory, name);
        }

        /// <summary>
        /// Serializes all the given setting instances under the specific filename. It's recommended to use this one, as each file is serialized in parallel
        /// </summary>
        /// <param name="declaringType">The type where to look. All objects must be static and be marked with DefaultSettingsObjectAttribute</param>
        /// <param name="overwrite">Whether or not to overwrite the file if it already exists</param>
        public static Task CreateDefaultsAsync(IEnumerable<(T obj, string name)> objects, bool overwrite = false) => Task.Run(async () =>
        {
            AsyncTaskManager tasks = new();
            foreach (var (obj, name) in objects)
                if (overwrite || !File.Exists(Path.Combine(Directory, name + JsonExtension)))
                    tasks.Add(Serialize.JsonAsync(obj, Directory, name));
            await tasks;
        });

        public static void Initialize(string directory, string fileName, bool defaultIfFail = true, Func<T, bool> validation = null)
        {
            FileName = fileName;
            Directory = directory;
            Log.Debug($"Checking for existence of {fileName} settings in {directory}");
            System.IO.Directory.CreateDirectory(directory);
            if (SettingsFileExist)
            {
                Log.Debug($"Existence of {fileName} settings in {directory} verified, verifying version");
                string str;
                switch (CheckFile(directory, fileName, out ulong version, out string type)) // This chunk of code is quite rocky. I apologize in advance.
                {
                    case CheckFileCode.InvalidFile: //CASE 1: INVALID FILE------------------------------------------------------------------------------------
                        str = "File is invalid. Unable to parse or serialize.";
                        Log.Error(str);
                        if (defaultIfFail)
                            goto RestoreToDefault;
                        throw new JsonException(str);
                    case CheckFileCode.DifferentType: //CASE 2: DIFFERENT TYPE--------------------------------------------------------------------------------
                        str = $"Cannot serialize a settings file of a different type. File specified: {type}, Settings to Parse: {Default.SettingsType}";
                        Log.Error(str);
                        if (defaultIfFail)
                            goto RestoreToDefault;
                        throw new JsonException(str);
                    case CheckFileCode.DifferentVersion: //CASE 3: DIFFERENT VERSION--------------------------------------------------------------------------
                        Log.Error($"Version verified, unequal: Expected: {Default.Version}; Read: {version}.");
                        File.Move(Path.Combine(directory, fileName + JsonExtension), Path.Combine(directory, fileName + $"_{version}_old" + JsonExtension), true);
                        if (defaultIfFail)
                            goto RestoreToDefault;
                        throw new JsonException($"Cannot serialize settings of a different version. Expected: {Default.Version}; Read: {version}");

                    case CheckFileCode.ValidFile: //CASE 4: VALID FILE----------------------------------------------------------------------------------------
                        try
                        {
                            Log.Debug($"Version verified, equal, loading {fileName}");
                            LoadSettings(directory, fileName, false, validation);
                            return;
                        }
                        catch (JsonException e)
                        {
                            Log.Error($"Exception caught: {e}");
                            Log.Information($"{fileName} in {directory} is invalid, restoring to default and creating a new file asynchronously");
                            if(defaultIfFail)
                                goto RestoreToDefault;
                            throw;
                        }
                }
            }
            Log.Information($"{fileName} in {directory} could not be found");
        RestoreToDefault:;
            Log.Information("Restoring to default and creating a new file");
            RestoreToDefault();
            SaveSettings();
        }

        private static void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var prop = GetProperty(e.PropertyName);
            HasChanged = true;
            if (HasPropertyChanged(prop))
                ChangesDict[prop] = prop.GetValue(Current);
            else
                ChangesDict.Add(prop, prop.GetValue(Current));
            SettingsChanged?.Invoke(sender, e);
        }
    }
}
