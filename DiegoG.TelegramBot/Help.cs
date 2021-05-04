using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using DiegoG.Utilities.Collections;
using System.Linq;
using DiegoG.Utilities;
using DiegoG.TelegramBot.Types;
using Telegram.Bot.Types;

namespace DiegoG.TelegramBot
{
    public class Help : IBotCommand
    {
        public string Trigger => "/help";
        public string Alias => "/h";
        public string HelpExplanation => "Returns a string explaining the uses of a specific command.";
        public string HelpUsage => "[Command]";
        public IEnumerable<(string, string)>? HelpOptions => null;

        private static string GetAlias(IBotCommand cmd) => cmd.Alias is not null ? $" ({cmd.Alias})" : "";

        private const string HelpExplanationFormat = "\n\tAvailable Options:\n\t\t";
        private static string GetHelpExplanation(IBotCommand cmd)
        {
            if(cmd.HelpOptions is not null)
            {
                int padding = cmd.HelpOptions.Max(s => s.Option.Length);
                return HelpExplanationFormat + cmd.HelpOptions.Select(s => $"{s.Option.PadLeft(padding)}: {s.Explanation}").Flatten("\n\t\t");
            }
            return "";
        }

        //0 : trigger | 1 : alias | 2 : HelpExplanation | 3 : HelpUsage | 4 : HelpOptions (if available)
        private const string HelpFormat = " > {0}{1} | {2}\n >> {3}{4}";
        public Task<(string Result, bool Hold)> Action(BotCommandArguments args)
        {
            if (args.Arguments.Length <= 1)
            {
                string str = "CommandName (Argument) [OptionalArgument]\n";

                foreach (var command in BotCommandProcessor.CommandList)
                    str += $"{command.Trigger}{GetAlias(command)} | {command.HelpUsage}\n";
                return Task.FromResult((str[0..^1], false));
            }

            var clist = BotCommandProcessor.CommandList;
            var cmd = args.Arguments[1];

            IBotCommand c;
            if (clist.HasCommand(cmd))
                c = clist[cmd];
            else if (clist.HasCommand("/" + cmd))
                c = clist["/" + cmd];
            else
                throw new InvalidBotCommandArgumentsException(args.ToString(), "Unknown Command");
            return Task.FromResult((string.Format(HelpFormat, c.Trigger, GetAlias(c), c.HelpExplanation, c.HelpUsage, GetHelpExplanation(c)), false));
        }

        public Task<(string Result, bool Hold)> ActionReply(BotCommandArguments args)
        {
            throw new NotImplementedException();
        }

        public void Cancel(User user)
        {
            throw new NotImplementedException();
        }
    }
}