using System.Threading.Tasks;

namespace DiegoG.CLI.CLICommands
{
    [CLICommand]
    public class HelloWorld : ICommand
    {
        public string Alias => null;
        public string Trigger => "helloworld";
        public string HelpExplanation => "Salute the World!";
        public string HelpUsage => "";
        public Task<string> Action(string[] args) => Task.FromResult("Hello, World!");
    }
}
