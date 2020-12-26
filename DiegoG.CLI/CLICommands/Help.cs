using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using DiegoG.Utilities;

namespace DiegoG.CLI.CLICommands
{
    [CLICommand]
    public class Help : ICommand
    {
        public string Trigger => "help";
        public string Alias => "h";
        public string HelpExplanation => "Returns a string explaining the uses of a specific command.";
        public string HelpUsage => "[Command]";

        private static string GetAlias(ICommand cmd) => cmd.Alias is not null ? $"({cmd.Alias})" : "";

        public async Task<string> Action(string[] args)
        {
            if (args.Length <= 1)
            {
                return await Task.Run
                    (
                    () =>
                    {
                        var processedc = new Dictionary<ICommand, string>(Commands.CommandList.Count + 1);
                        string str = "CommandName [Argument] (OptionalArgument)\n   ";
                        int mostlines = 0;

                        foreach (var c in Commands.CommandList)
                            if (!processedc.ContainsKey(c))
                            {
                                var s = $" > {c.Trigger}{GetAlias(c)}";
                                processedc.Add(c, s);
                                if (mostlines < s.Length)
                                    mostlines = s.Length;
                            }
                        foreach (var (c, s) in processedc.GetKVTuple())
                            str += $"{s} {new string(' ', mostlines - s.Length)}| {c.HelpExplanation}\n";
                        return str;
                    }
                    );
            }
            var c = Commands.CommandList[args[1]];
            return $" > {c.Trigger}{GetAlias(c)} | {c.HelpExplanation}\n  >> {c.HelpUsage}";
        }
    }
}
