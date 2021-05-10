using DiegoG.TelegramBot.Types;
using DiegoG.Utilities.Collections;
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

            LoadCommands();
            if (!CommandList.HasCommand("help") && !CommandList.HasCommand("h"))
                CommandList.Add(new Help());
            if (!CommandList.HasCommand("start"))
                CommandList.Add(new Start());

            if (bot is not null)
                bot.OnMessage += Bot_OnMessage;
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

        /// <summary>
        /// USE ONLY FOR LOADING COMMANDS FROM DYNAMICALLY LOADED ASSEMBLIES (i.e. Assembly.Load()). Meant only for extensions
        /// It's also preferrable to NOT call this, and instead load all extensions before loading commands
        /// </summary>
        /// <param name="assemblies"></param>
        public static void LoadCommands(Assembly[] assemblies)
        {
            Type? curtype = null;
            try
            {
                foreach (var ty in ReflectionCollectionMethods.GetAllTypesWithAttributeInAssemblies(typeof(BotCommandAttribute), false, assemblies))
                {
                    curtype = ty;
                    CommandList.Add((Activator.CreateInstance(ty) as IBotCommand)!);
                }
            }
            catch (Exception e)
            {
                throw new TypeLoadException($"All classes attributed with BotCommandAttribute must not be generic, abstract, or static, must have a parameterless constructor, and must implement IBotCommand directly or indirectly. BotCommandAttribute is not inheritable. Check inner exception for more details. Type that caused the exception: {curtype}", e);
            }
        }
        private static void LoadCommands()
        {
            Type? cty = null;
            try
            {
                foreach (var ty in ReflectionCollectionMethods.GetAllTypesWithAttribute(typeof(BotCommandAttribute), false))
                {
                    cty = ty;
                    CommandList.Add((Activator.CreateInstance(ty) as IBotCommand)!);
                }
            }
            catch (Exception e)
            {
                throw new TypeLoadException($"All classes attributed with CLICommandAttribute must not be generic, abstract, or static, must have a parameterless constructor, and must implement ICommand directly or indirectly. CLICommandAttribute is not inheritable. Check inner exception for more details. Type that caused the exception: {cty?.ToString() ?? "Unknown"}", e);
            }
        }
    }
}
