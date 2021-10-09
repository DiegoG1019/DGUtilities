using System.Collections.Generic;

namespace DiegoG.Utilities.Settings
{
    public interface ICommentedSettings : ISettings
    {
        public string[]? _Comments { get; }
        public Dictionary<string, string>? _Usage { get; }
    }
}
