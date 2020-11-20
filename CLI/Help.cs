using System;
using System.Collections.Generic;

namespace DiegoG.CLI
{
    public class Help : ICommand
    {
        public string Trigger => "help";
        public string HelpExplanation => "Returns a string explaining the uses of a specific command.";
        public string HelpUsage => "[Command]";
        public string Action(string[] args)
        {
            if (args.Length <= 1)
            {
                string str = String.Empty;
                foreach (var c in Commands.CommandList)
                {
                    str += $" > {c.Trigger} {c.HelpUsage} | {c.HelpExplanation}\n";
                    //$ denota una string que usa {} de forma que, en vez de ser place holders, usa {variable} para hacer un formateo mas directo.
                    //\n es un codigo para newline
                }
                return str;
            }
            return Commands.GetHelp(args[1]);
        }
    }
}
