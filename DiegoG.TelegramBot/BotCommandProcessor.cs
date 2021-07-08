using DiegoG.TelegramBot.Types;
using DiegoG.Utilities.Collections;
using DiegoG.Utilities.Reflection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DiegoG.TelegramBot
{
    public class BotCommandProcessor
    {
        const string q = "\"";

        public record Config(bool ProcessNormalMessages = true) { }

        public static string[] SeparateArgs(string input) => Regex.Split(input, $@"{q}([^{q}]*){q}|(\S+)").Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();


        private BotKey BotKey { get; init; }
        private Config Cfg;
        private Dictionary<User, IBotCommand> HeldCommands { get; } = new();

        public event EventHandler<BotCommandArguments>? CommandCalled;
        public MessageQueue MessageQueue { get; init; }
        public BotCommandList CommandList { get; init; }

        /// <summary>
        /// Initializes the BotCommandProcessor
        /// </summary>
        /// <param name="bots">A bot to subscribe onto, if you decide to leave blank, please make sure to manually subscribe <see cref="Bot_OnMessage(object?, Telegram.Bot.Args.MessageEventArgs)"/> to your bots' OnMessage event </param>
        /// <param name="config"></param>
        public BotCommandProcessor(TelegramBotClient bot, BotKey key = BotKey.Any, Config? config = null)
        {
            Cfg = config ?? new();
            BotKey = key;

            CommandList = new();

            foreach (var c in TypeLoader.InstanceTypesWithAttribute<IBotCommand>(typeof(BotCommandAttribute), 
                ValidateCommandAttribute,
                AppDomain.CurrentDomain.GetAssemblies()))
            {
                c.Processor = this;
                CommandList.Add(c);
            }

            if (!CommandList.HasCommand("help") && !CommandList.HasCommand("h"))
                CommandList.Add(new Help());
            if (!CommandList.HasCommand("start"))
                CommandList.Add(new Start());

            MessageQueue = new(bot);

            bot.OnMessage += Bot_OnMessage;
        }
        
        private bool ValidateCommandAttribute(Type type, Attribute[] attributes)
        {
            if (BotKey is BotKey.Any)
                return true;

            var b = ((BotCommandAttribute)attributes.First(y => y is BotCommandAttribute)).BotKey;
            return (BotKey & b) is not BotKey.Any; //x & x will always return x, and x & y will return whichever values are shared among them. So if we already know it's not 0 (Any), then if it returns anything other than that, we know it contains at least one of the desired flags
        }

        /// <summary>
        /// Searches through the provided assemblies, or the executing assembly, in search of new commands to instantiate. Automatically excludes already registered command types, but does not extend exclusion for duplicated names
        /// </summary>
        /// <param name="assemblies">The assemblies to search. Leave null for none</param>
        /// <exception cref="InvalidBotCommandException">Thrown if one of the commands contains an invalid name</exception>
        /// <exception cref="InvalidOperationException">Thrown if one of the commands is found to be a duplicate</exception>
        public void LoadNewCommands(params Assembly[] assemblies)
        {
            foreach (var c in TypeLoader.InstanceTypesWithAttribute<IBotCommand>(typeof(BotCommandAttribute), CommandList.Select(d => d.GetType()), assemblies))
                CommandList.Add(c);
        }

        public async void Bot_OnMessage(object? sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var user = e.Message.From;
            var client = sender as TelegramBotClient;
            Log.Debug($"Message from user {user}, processing");
            var command = e.Message.Text;
            try
            {
                if (Cfg.ProcessNormalMessages || command.StartsWith("/"))
                {
                    var cr = await Call(command, user);
                    MessageQueue.EnqueueAction(b => b.SendTextMessageAsync(e.Message.Chat.Id, cr, ParseMode.Default, null, false, false, e.Message.MessageId));
                    Log.Debug($"Command {command} from user {user} succesfully processed.");
                }
            }
            catch (InvalidBotCommandException exc)
            {
                if (command.StartsWith("/"))
                {
                    MessageQueue.EnqueueAction(b => b.SendTextMessageAsync(e.Message.Chat.Id, $"Invalid Command: {exc.Message}", ParseMode.Default, null, false, false, e.Message.MessageId));
                    Log.Debug($"Invalid Command {command} from user {user}");
                }
            }
            catch (InvalidBotCommandArgumentsException exc)
            {
                if (command.StartsWith("/"))
                {
                    MessageQueue.EnqueueAction(b => b.SendTextMessageAsync(e.Message.Chat.Id, $"Invalid Command Argument: {exc.Message}", ParseMode.Default, null, false, false, e.Message.MessageId));
                    Log.Debug($"Invalid Command Arguments {command} from user {user}");
                }
            }
            catch (Exception exc)
            {
                Log.Fatal(exc, "Unhalded Exception thrown:");
            }
        }

        public Task<string> Call(string input, User sender) => Call(SeparateArgs(input), sender);
        private async Task<string> ReplyCall(BotCommandArguments args)
        {
            try
            {
                var (result, hold) = await HeldCommands[args.User].ActionReply(args);
                if (!hold)
                    HeldCommands.Remove(args.User);
                return result;
            }
            catch (InvalidBotCommandException e) { Log.Error(e, args.ToString()); throw; }
            catch (InvalidBotCommandArgumentsException e) { Log.Error(e, args.ToString()); throw; }
            catch (BotCommandProcessException e) { Log.Error(e, args.ToString()); throw; }
            catch (Exception e)
            {
                var ex = new InvalidBotCommandException(args.ToString(), "threw an unspecified exception", e);
                Log.Fatal(e, args.ToString());
                throw ex;
            }
        }

        public Task<string> Call(string[] input, User sender)
            => Call(new(input, sender));
        public async Task<string> Call(BotCommandArguments args)
        {
            try
            {
                if (HeldCommands.ContainsKey(args.User))
                    return await ReplyCall(args);

                if (!CommandList.HasCommand(args.Arguments[0]))
                    return "Unknown Command";

                var cmd = CommandList[args.Arguments[0]];
                var t = cmd.Action(args);
                CommandCalled?.Invoke(null, args);

                var (result, hold) = await t;

                if (hold)
                    HeldCommands.Add(args.User, cmd);

                return result;
            }
            catch (InvalidBotCommandException e) { Log.Error(e, args.ToString()); throw; }
            catch (InvalidBotCommandArgumentsException e) { Log.Error(e, args.ToString()); throw; }
            catch (BotCommandProcessException e) { Log.Error(e, args.ToString()); throw; }
            catch (Exception e)
            {
                var ex = new InvalidBotCommandException(args.ToString(), "threw an unspecified exception", e);
                Log.Fatal(e, args.ToString());
                throw ex;
            }
        }

    }
}
