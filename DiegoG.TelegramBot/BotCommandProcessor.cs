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
    public static class BotCommandProcessor
    {
        public record Config() { }

        const string q = "\"";

        public static event EventHandler<BotCommandArguments>? CommandCalled;

        public static SendTextMessage SendMessageCallback { get; private set; }

        public static async void Bot_OnMessage(object? sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var user = e.Message.From;
            var client = sender as TelegramBotClient;
            Log.Debug($"Message from user {user}, processing");
            var command = e.Message.Text;
            try
            {
                var cr = await Call(command, user);
                SendMessageCallback(e.Message.Chat.Id, cr, ParseMode.Default, false, false, e.Message.MessageId);
                Log.Debug($"Command {command} from user {user} succesfully processed.");
            }
            catch (InvalidBotCommandException exc)
            {
                if (command.StartsWith("/"))
                {
                    SendMessageCallback(e.Message.Chat.Id, $"Invalid Command: {exc.Message}", ParseMode.Default, false, false, e.Message.MessageId);
                    Log.Debug($"Invalid Command {command} from user {user}");
                }
            }
            catch (InvalidBotCommandArgumentsException exc)
            {
                if (command.StartsWith("/"))
                {
                    SendMessageCallback(e.Message.Chat.Id, $"Invalid Command Argument: {exc.Message}", ParseMode.Default, false, false, e.Message.MessageId);
                    Log.Debug($"Invalid Command Arguments {command} from user {user}");
                }
            }
            catch (Exception exc)
            {
                Log.Fatal(exc, "Unhalded Exception thrown:");
            }
        }

        private static bool IsInit;
        /// <summary>
        /// Initializes the BotCommandProcessor
        /// </summary>
        /// <param name="sendMessageCallback">The function to call when sending a text message</param>
        /// <param name="bots">A bot to subscribe onto, if you decide to leave blank, please make sure to manually subscribe <see cref="Bot_OnMessage(object?, Telegram.Bot.Args.MessageEventArgs)"/> to your bots' OnMessage event </param>
        /// <param name="config"></param>
        public static void Initialize(SendTextMessage sendMessageCallback, TelegramBotClient? bot = null, Config? config = null)
        {
            if (IsInit)
                throw new InvalidOperationException("Cannot Initialize Twice");
            IsInit = true;

            _cfg = config ?? new();

            SendMessageCallback = sendMessageCallback;

            foreach (var c in TypeLoader.InstanceTypesWithAttribute<IBotCommand>(typeof(BotCommandAttribute), Assembly.GetExecutingAssembly()))
                CommandList.Add(c);

            if (!CommandList.HasCommand("help") && !CommandList.HasCommand("h"))
                CommandList.Add(new Help());
            if (!CommandList.HasCommand("start"))
                CommandList.Add(new Start());

            if (bot is not null)
                bot.OnMessage += Bot_OnMessage;
        }

        /// <summary>
        /// Searches through the provided assemblies, or the executing assembly, in search of new commands to instantiate. Automatically excludes already registered command types, but does not extend exclusion for duplicated names
        /// </summary>
        /// <param name="assemblies">The assemblies to search. Leave null for none</param>
        /// <exception cref="InvalidBotCommandException">Thrown if one of the commands contains an invalid name</exception>
        /// <exception cref="InvalidOperationException">Thrown if one of the commands is found to be a duplicate</exception>
        public static void LoadNewCommands(params Assembly[] assemblies)
        {
            foreach (var c in TypeLoader.InstanceTypesWithAttribute<IBotCommand>(typeof(BotCommandAttribute), CommandList.Select(d => d.GetType()), assemblies))
                CommandList.Add(c);
        }

        private static Config _cfg;
        private static Config Cfg => IsInit ? _cfg : throw new InvalidOperationException("Please call Initialize First");

        private static BotCommandList _Cl = new();
        public static BotCommandList CommandList => IsInit ? _Cl : throw new InvalidOperationException("Please call Initialize First");

        public static string[] SeparateArgs(string input) => Regex.Split(input, $@"{q}([^{q}]*){q}|(\S+)").Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

        private static Dictionary<User, IBotCommand> HeldCommands { get; } = new();

        public static Task<string> Call(string input, User sender) => Call(SeparateArgs(input), sender);
        private static async Task<string> ReplyCall(BotCommandArguments args)
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

        public static Task<string> Call(string[] input, User sender)
            => Call(new(input, sender));
        public static async Task<string> Call(BotCommandArguments args)
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
