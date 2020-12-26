using System;
using System.Threading.Tasks;

namespace DiegoG.CLI.CLICommands
{
    [CLICommand]
    class Print : ICommand
    {
        public string Trigger => "print";
        public string Alias => "p";
        public string HelpExplanation => "Prints all of the arguments";
        public string HelpUsage => "print [everything else]";

        public async Task<string> Action(string[] args)
        {
            return await Task.Run
                (() =>
                {
                    string result = String.Empty;
                    for (int i = 0; i < args.Length; i++)
                        result += $" [{args[i]}]";
                    Console.WriteLine(result);
                    return result;
                });
        }
    }
}

//