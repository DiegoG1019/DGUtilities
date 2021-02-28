using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using DiegoG.Utilities.Collections;
using System.Linq;

namespace DiegoG.CLI.CLICommands
{
    public class Help : ICommand
    {
        public string Trigger => "help";
        public string Alias => "h";
        public string HelpExplanation => "Returns a string explaining the uses of a specific command.";
        public string HelpUsage => "[Command]";
        public IEnumerable<(string, string)> HelpOptions => null;
        void ICommand.ClearData() { return; }

        private static string GetAlias(ICommand cmd) => cmd.Alias is not null ? $" ({cmd.Alias})" : "";

        private const string HelpExplanationFormat = "\n\tAvailable Options:\n\t\t";
        private static string GetHelpExplanation(ICommand cmd)
        {
            if(cmd.HelpOptions is not null)
            {
                int padding = cmd.HelpOptions.Max(s => s.Option.Length);
                return HelpExplanationFormat + cmd.HelpOptions.Select(s => $"{s.Option.PadLeft(padding)}: {s.Explanation}").Flatten("\n\t\t");
            }
            return "";
        }

        //0 : trigger | 1 : alias | 2 : HelpExplanation | 3 : HelpUsage | 4 : HelpOptions (if available)
        private const string HelpFormat = " > {0}{1} | {2}\n >> {3}{4}";
        public Task<string> Action(CommandArguments args)
        {
            if (args.Arguments.Length <= 1)
            {
                var processedc = new Dictionary<ICommand, string>(Commands.CommandList.Count + 1);
                string str = "CommandName [Argument] (OptionalArgument)\n   ";
                int mostlines = 0;

                foreach (var command in Commands.CommandList)
                    if (!processedc.ContainsKey(command))
                    {
                        var s = $" > {command.Trigger}{GetAlias(command)}";
                        processedc.Add(command, s);
                        if (mostlines < s.Length)
                            mostlines = s.Length;
                    }
                foreach (var (cmd, s) in processedc.GetKVTuple())
                    str += $"{s.PadRight(mostlines)}| {cmd.HelpExplanation}\n";
                return Task.FromResult(str);
            }

            var c = Commands.CommandList[args.Arguments.ElementAt(1)];
            return Task.FromResult(string.Format(HelpFormat, c.Trigger, GetAlias(c), c.HelpExplanation, c.HelpUsage, GetHelpExplanation(c)));
        }
    }
}
