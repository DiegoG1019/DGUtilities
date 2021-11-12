using DiegoG.Utilities.Collections;
using DiegoG.Utilities.IO;
using DiegoG.Utilities.Reflection;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using static DiegoG.Utilities.IO.Serialization;

namespace DiegoG.Utilities.Settings
{
    /// <summary>
    /// The default is represented by what the properties are initialized to. It's not required to instatiate anything, Settings will do that for you.
    /// For example: "Settings`ApplicationSettings`.Current" will work after Settings`ApplicationSettings`.Initialize() is called.
    /// A separate "Settings" will be created by each type you call, for example, Settings`ApplicationSettings` is different from Settings`YourSettings`.
    /// By virtue of C# creating a new static type with its own state for every generic type used with it
    /// </summary>
    /// <typeparam name="T">The class that represents the settings</typeparam>
    public static class Settings<T> where T : class, ISettings, new()
    {
        public static event PropertyChangedEventHandler? SettingsChanged;
        private readonly static Type TType = typeof(T);
        private static T Default { get; } = new T();
        private static Dictionary<PropertyInfo, object> ChangesDict { get; } = new();
        public static bool HasChanged { get; private set; }

        /// <summary>
        /// Returns null if the property has not changed
        /// </summary>
        public static ReadOnlyIndexedProperty<PropertyInfo, object?> Changes { get; } = new(i => ChangesDict.ContainsKey(i) ? ChangesDict[i] : null);
        public static bool HasPropertyChanged(PropertyInfo i) => ChangesDict.ContainsKey(i);
        public static IEnumerable<(PropertyInfo Property, object Value)> GetChanges() => from i in ChangesDict select (i.Key, i.Value);
        public static PropertyInfo? GetProperty(string propertyName) => TType.GetProperty(propertyName);

        /// <summary>
        /// Returns a copy of current settings for modification, that can then be applied using Apply(T)
        /// </summary>
        /// <returns></returns>
        public static T? GetModifiable() => Current.CopyByBinarySerialization() as T;

        /// <summary>
        /// Replace the current settings with the given object
        /// </summary>
        /// <param name="current"></param>
        public static void Apply(T current) => Current = current;

        public static T Current
        {
            get
            {
                lock (CurrentSettingsKey)
                    return CurrentField;
            }
            private set //I'm really worried about concurrency here
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value), "Current settings cannot be null");

                lock (CurrentSettingsKey)
                    CurrentField = value;
                SettingsChanged?.Invoke(null, new(nameof(Current)));
            }
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static T CurrentField;
        private static readonly object CurrentSettingsKey = new();

        public static string Directory { get; private set; }
        public static string FileName { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        /// => Path.Combine(Directory, File);
        /// </summary>
        public static string FullPath => Path.GetFullPath(Path.Combine(Directory, FileName));
        public static bool SettingsFileExist => File.Exists(FullPath + JsonExtension);

        //It would normally be better to place these in the class, but then we woulnd't have access to T and we'd have to override it
        private static readonly TypeInfo typeinfo = (TypeInfo)typeof(T);

        public static void SaveSettings() => Serialize.Json(Current, Directory, FileName);

        public static Task SaveSettingsAsync() => Serialize.JsonAsync(Current, Directory, FileName);

        public static Task<string> SerializeSettingsAsync() => Serialize.JsonAsync(Current);
        public static string SerializeSettings() => Serialize.Json(Current);

        public static async Task DeserializeSettingsAsync(string jsonstring) => Current = await Deserialize<T>.JsonAsync(jsonstring);
        public static void DeserializeSettings(string jsonstring) => Current = Deserialize<T>.Json(jsonstring);

        public static void SerializeEmptyFile(string directory, string file) => Serialize.Json(new T(), directory, file);
        public static Task SerializeEmptyFileAsync(string directory, string file) => Serialize.JsonAsync(new T(), directory, file);

        private static void LoadSettings(string directory, string fileName, bool doCheck, Func<T, bool>? validation)
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
        public static void LoadSettings(string directory, string fileName, Func<T, bool>? validation = null)
            => LoadSettings(directory, fileName, true, validation);

        public static async Task LoadSettingsAsync(string directory, string fileName, Func<T, bool>? validation = null)
        {
            if (CheckFile(directory, fileName, out var version, out var type) != CheckFileCode.ValidFile)
                throw new InvalidDataException($"The supplied settings are not valid\nExpected: v.{Default.Version} type: {Default.SettingsType}\nRead: v.{version} type: {type}");
            var tsk = Deserialize<T>.JsonAsync(directory, fileName);
            FileName = fileName;
            Directory = directory;
            var stgs = await tsk;
            if (!(validation?.Invoke(stgs) ?? true))
                throw new InvalidDataException("The loaded settings did not pass validation");
            Current = stgs;
        }

        /// <summary>
        /// Loads user secrets into the locally held instance of the given ISettings class -- DO NOT USE THIS IN PRODUCTION. Only for development purposes, use LoadSettings instead (Initialize is also not recommended, as it relies on default constructor and thus, in-project data to build the file if it doesn't exist, when these settings should already exist in your server)
        /// </summary>
        /// <param name="config"></param>
        /// <param name="section"></param>
        /// <param name="configureOptions"></param>
        public static void LoadUserSecrets(IConfiguration config, string? section = null, Action<BinderOptions>? configureOptions = null, Func<T, bool>? validation = null)
        {
            var c = section is null ? config : config.GetSection(section);
            var stgs = configureOptions is null ? c.Get<T>() : c.Get<T>(configureOptions);
            if (!(validation?.Invoke(stgs) ?? true))
                throw new InvalidDataException("The loaded settings did not pass validation");
            Current = stgs;
        }

        public static Task LoadUserSecretsAsync(IConfiguration config, string? section = null, Action<BinderOptions>? configureOptions = null, Func<T, bool>? validation = null)
            => Task.Run(() => LoadUserSecrets(config, section, configureOptions, validation));

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
                type = jsel.GetProperty(nameof(ISettings.SettingsType)).GetString()!;
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
            foreach (var (obj, name) in objects)
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
            var tasks = new Task[objects.Count()];
            int i = 0;
            foreach (var (obj, name) in objects)
                if (overwrite || !File.Exists(Path.Combine(Directory, name + JsonExtension)))
                    tasks[i++] = Serialize.JsonAsync(obj, Directory, name);
            await Task.WhenAll(tasks);
        });

        public static void ApplyEnvironmentVariables(bool throwIfFail = true)
        {
            Current ??= new();
            Action<PropertyInfo, object, object> set = throwIfFail ? SetValue : TrySetValue;
            var recursivenessSet = new HashSet<ISettingsSection>();
            EnvVarsSet(set, Current, ReflectionCollectionMethods<T>.GetAllInstanceProperties());
        }

        private static void EnvVarsSet(Action<PropertyInfo, object, object> set, object target, IEnumerable<PropertyInfo> dat)
        {
            foreach (var prop in dat)
            {
                if (prop.PropertyType.IsAssignableTo(typeof(ISettingsSection)))
                {
                    var val = prop.GetValue(target);
                    EnvVarsSet(set,
                               prop.GetValue(target) ?? throw new ArgumentNullException(nameof(target), "A Settings Section can never be null"), 
                               ReflectionCollectionMethods.GetAllInstanceProperties(prop.PropertyType));
                    continue;
                }

                var at = prop.GetCustomAttribute<FromEnvironmentVariable>();
                if (at is null || at.Precedence is false && prop.GetValue(target) is not null)
                    continue;

                var x = at.FetchValue(prop.PropertyType);
                if (x is null && at.Required && prop.GetValue(target) is null)
                    throw new InvalidOperationException($"Could not obtain a value for required settings property {prop.Name}");
                set(prop, target, x);
            }
        }

        private static void SetValue(PropertyInfo prop, object target, object value)
            => prop.SetValue(target, value);

        private static void TrySetValue(PropertyInfo prop, object target, object value)
        {
            try
            {
                SetValue(prop, target, value);
            }
            catch (Exception e) 
            { 
                Log.Error(e.Message); 
            }
        }

        /// <summary>
        /// Validates, loads and initializes the given settings file, if it doesn't exist, creates one with default values. Environment Variables are loaded AFTER the file or default
        /// </summary>
        /// <param name="directory">The directory where the settings file resides</param>
        /// <param name="fileName">The file name of the settings file</param>
        /// <param name="defaultIfFail">Whether to throw an exception, or use a default settings file upon failure to deserialize</param>
        /// <param name="validation">A method that will validate the settings file for special cases</param>
        /// <param name="update">A method that will update the settings file in case it's an outdated version</param>
        public static void Initialize(string directory, string fileName, bool defaultIfFail = true, Func<T, bool>? validation = null, Action<T>? update = null)
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
                        if (update is not null)
                            goto case CheckFileCode.ValidFile;
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
                            ApplyEnvironmentVariables(false);
                            update?.Invoke(Current);
                            return;
                        }
                        catch (JsonException e)
                        {
                            Log.Error($"Exception caught: {e}");
                            Log.Information($"{fileName} in {directory} is invalid, restoring to default and creating a new file asynchronously");
                            if (defaultIfFail)
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
            ApplyEnvironmentVariables(false);
            if (!(validation?.Invoke(Current) ?? true))
                throw new InvalidDataException("Settings file did not pass validation");
        }
    }
}
