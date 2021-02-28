using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.CLI.CLICommands
{
    /// <summary>
    /// The base class for ChainTestCommand A, B and C
    /// Explanation: "Entirely for test purposes. Call them in order of A >> B >> C, along with the arguments, verbatim."
    /// Usage: "ChainA A B C >> ChainB >> ChainC C"
    /// </summary>
    public abstract class ChainTestCommand : ICommand
    {
        public abstract byte Link { get; }
        public string HelpExplanation => "Entirely for test purposes. Call them in order of A >> B >> C, along with the arguments, verbatim.";
        public string HelpUsage => "ChainA [\"A\"] [\"B\"] [\"C\"] >> ChainB >> ChainC [\"C\"]";
        public abstract string Trigger { get; }
        public string Alias => null;
        public IEnumerable<(string, string)> HelpOptions => null;
        void ICommand.ClearData() { return; }
        public abstract Task<string> Action(CommandArguments args);
        protected Task<string> Action(CommandArguments args, int expectedLinkInd)
        {
            return byte.TryParse(args.Original[expectedLinkInd], out byte b) && b > Link
                ? Task.FromResult(Link.ToString())
                : throw new InvalidCommandArgumentException($"Chain call failed or out of order.");
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

    /// <summary>
    /// A Command to Test Command Concatenation
    /// Explanation: "Entirely for test purposes. Call them in order of A >> B >> C, along with the arguments, verbatim."
    /// Usage: "ChainA A B C >> ChainB >> ChainC C"
    /// </summary>
    public class ChainLinkACommand : ChainTestCommand
    {
        public override byte Link => 0;
        public override string Trigger => "ChainA";

        private readonly string[] Expected = new string[] { "ChainA", "A", "B", "C", "1" };
        public override Task<string> Action(CommandArguments args)
        {
            if (!args.Original.SequenceEqual(Expected))
                InvalidThrow(Expected, args.Original);
            return Action(args, 4);
        }
    }

    /// <summary>
    /// A Command to Test Command Concatenation
    /// Explanation: "Entirely for test purposes. Call them in order of A >> B >> C, along with the arguments, verbatim."
    /// Usage: "ChainA A B C >> ChainB >> ChainC C"
    /// </summary>
    public class ChainLinkBCommand : ChainTestCommand
    {
        public override byte Link => 1;
        public override string Trigger => "ChainB";

        private readonly string[] Expected = new string[] { "ChainB", "2" };
        public override Task<string> Action(CommandArguments args)
        {
            if (!args.Original.SequenceEqual(Expected))
                InvalidThrow(Expected, args.Original);
            return Action(args, 1);
        }
    }

    /// <summary>
    /// A Command to Test Command Concatenation
    /// Explanation: "Entirely for test purposes. Call them in order of A >> B >> C, along with the arguments, verbatim."
    /// Usage: "ChainA A B C >> ChainB >> ChainC C"
    /// Trigger ChainC
    /// </summary>
    public class ChainLinkCCommand : ChainTestCommand
    {
        public override byte Link => 2;
        public override string Trigger => "ChainC";

        private readonly string[] Expected = new string[] { "ChainC", "C" };
        public override Task<string> Action(CommandArguments args)
        {
            if (!args.Original.SequenceEqual(Expected))
                InvalidThrow(Expected, args.Original);
            return Task.FromResult(Link.ToString());
        }
    }
}
