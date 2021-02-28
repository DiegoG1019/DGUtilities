using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiegoG.CLI.CLICommands
{
    public class HelloWorld : ICommand
    {
        public string Alias => null;
        public string Trigger => "helloworld";
        public string HelpExplanation => "Salute the World!";
        public string HelpUsage => "";
        public IEnumerable<(string, string)> HelpOptions => null;
        public Task<string> Action(CommandArguments args) => Task.FromResult("Hello, World!");
        void ICommand.ClearData() { return; }
    }
}
