using System;
using System.Collections.Generic;
using System.Text;

namespace DiegoG.CLI
{
    class Print : Cmd
    {
        public string Trigger => "print";
        public string HelpExplanation => "Prints all of the arguments";
        public string HelpUsage => "print [everything else]";

        public string Action(string[] args)
        {
            string result = String.Empty;
            for(int i = 0; i < args.Length; i++)
                result += $" [{args[i]}]";
            Console.WriteLine(result);
            return result;
        }
    }
}

//