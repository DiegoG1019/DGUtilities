using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.CLI.CLICommands
{
    public abstract class ChainTestCommand : ICommand
    {
        public abstract byte Link { get; }
        public string HelpExplanation => "Entirely for test purposes. Call them in order of A >> B >> C, along with the arguments, verbatim.";
        public string HelpUsage => "A [\"A\"] [\"B\"] [\"C\"] >> B >> C [\"C\"]";
        public abstract string Trigger { get; }
        public string Alias => null;
        public abstract Task<string> Action(string[] args);
        protected Task<string> Action(string[] args, int expectedLinkInd)
        {
            if (byte.TryParse(args[expectedLinkInd], out byte b) && b > Link)
                return Task.FromResult(Link.ToString());
            throw new InvalidCommandArgumentException($"Chain call failed or out of order.");
        }
        protected static string InvalidThrow(string[] expected, string[] received)
        {
            string exstr = "Expected: ";
            string restr = "Received: ";
            foreach (var s in expected)
                exstr += $"{s} ";
            foreach (var s in received)
                restr += $"{s} ";
            throw new InvalidCommandArgumentException($"Arguments Invalid. {exstr} {restr}");
        }
    }

    [CLICommand]
    public class ChainLinkACommand : ChainTestCommand
    {
        public override byte Link => 0;
        public override string Trigger => "ChainA";

        private readonly string[] Expected = new string[] { "ChainA", "A", "B", "C", "1" };
        public override Task<string> Action(string[] args)
        {
            if (!args.SequenceEqual(Expected))
                InvalidThrow(Expected, args);
            return Action(args, 4);
        }
    }

    [CLICommand]
    public class ChainLinkBCommand : ChainTestCommand
    {
        public override byte Link => 1;
        public override string Trigger => "ChainB";

        private readonly string[] Expected = new string[] { "ChainB", "2" };
        public override Task<string> Action(string[] args)
        {
            if (!args.SequenceEqual(Expected))
                InvalidThrow(Expected, args);
            return Action(args, 1);
        }
    }

    [CLICommand]
    public class ChainLinkCCommand : ChainTestCommand
    {
        public override byte Link => 2;
        public override string Trigger => "ChainC";

        private readonly string[] Expected = new string[] { "ChainC", "C" };
        public override Task<string> Action(string[] args)
        {
            if (!args.SequenceEqual(Expected))
                InvalidThrow(Expected, args);
            return Task.FromResult(Link.ToString());
        }
    }
}
