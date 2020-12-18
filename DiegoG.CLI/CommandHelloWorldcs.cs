namespace DiegoG.CLI
{
    [CLICommand]
    public class HelloWorld : ICommand
    {
        public string Trigger => "helloworld";
        public string HelpExplanation => "Salute the World!";
        public string HelpUsage => "";
        public string Action(string[] args)
        {
            return "Hello, World!";
        }
    }
}
