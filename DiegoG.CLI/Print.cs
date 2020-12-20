using System;

namespace DiegoG.CLI
{
    [CLICommand]
    class Print : ICommand
    {
        public string Trigger => "print";
        public string HelpExplanation => "Prints all of the arguments";
        public string HelpUsage => "print [everything else]";

        public string Action(string[] args)
        {
            string result = String.Empty;
            for (int i = 0; i < args.Length; i++)
                result += $" [{args[i]}]";
            Console.WriteLine(result);
            return result;
        }
    }
}

//