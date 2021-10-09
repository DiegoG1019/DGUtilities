using Serilog.Events;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DiegoG.Utilities.Settings
{
    /// <summary>
    /// You're not actually supposed to use this one, inherit this in your own class and use that
    /// </summary>
    [Serializable]
    public abstract class ApplicationSettings : ISettings
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void NotifyPC([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected void NotifyPC<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            field = value;
            NotifyPC(propertyName);
        }

        public virtual ulong Version => 3;

        private bool _con;
        private LogEventLevel _logv;

        public abstract string SettingsType { get; }

        public virtual bool Console { get => _con; set => NotifyPC(ref _con, value); }

        public virtual LogEventLevel LogVerbosity { get => _logv; set => NotifyPC(ref _logv, value); }
#if DEBUG
        public ApplicationSettings()
        {
            _con = true;
            _logv = LogEventLevel.Verbose;
        }
#endif
    }
}
