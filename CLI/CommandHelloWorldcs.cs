namespace DiegoG.CLI
{
    public class HelloWorld : Cmd
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
