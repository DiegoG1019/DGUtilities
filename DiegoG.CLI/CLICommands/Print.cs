using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiegoG.CLI.CLICommands
{
    class Print : ICommand
    {
        public string Trigger => "print";
        public string Alias => "p";
        public string HelpExplanation => "Prints all of the arguments";
        public string HelpUsage => "print [everything else]";
        public IEnumerable<(string, string)> HelpOptions => null;
        void ICommand.ClearData() { return; }

        public async Task<string> Action(CommandArguments args)
        {
            return await Task.Run
                (() =>
                {
                    string result = String.Empty;
                    for (int i = 0; i < args.Arguments.Length; i++)
                        result += $" [{args.Arguments.ElementAt(i)}]";
                    Console.WriteLine(result);
                    return result;
                });
        }
    }
}

//