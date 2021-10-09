using System.ComponentModel;

namespace DiegoG.Utilities.Settings
{
    /// <summary>
    /// Enables a class to be used by Settings`T`
    /// </summary>
    public interface ISettings : INotifyPropertyChanged, ISettingsSection
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
}
