using DiegoG.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.CLI.CLICommands
{
#nullable enable
    internal sealed class AliasedCommand : ICommand
    {
        public string HelpExplanation { get; init; }
        public string HelpUsage { get; private init; }
        public string Trigger { get; init; }
        public IEnumerable<(string Option, string Explanation)>? HelpOptions { get; init; } = null;
        public string? Alias => null;

        private CommandArguments Command { get; init; }
        public Task<string> Action(CommandArguments args) => Commands.Call(Command);
        void ICommand.ClearData() { return; }
        public AliasedCommand(string trigger, CommandArguments command, string? explanation, IEnumerable<(string Option, string Explanation)>? helpOptions)
        {
            Trigger = trigger;
            Command = command;
            HelpExplanation = explanation ?? "A shortcut for a command with set arguments";
            HelpOptions = helpOptions;
            HelpUsage = $"[{Command.Original.Flatten()}] ...";
        }
    }
}
