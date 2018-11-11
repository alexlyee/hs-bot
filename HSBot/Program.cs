using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using HSBot;
using HSBot.Core;
using HSBot.Handlers;
using HSBot.Helpers;
using HSBot.Persistent;
using System.Diagnostics;
using System.IO;
using Discord.Addons.Interactive;
/* Namespace of discord command tools.
https://www.nuget.org/packages/Discord.Net.Commands/ */
/* Namespace of discord web tools.
https://www.nuget.org/packages/Discord.Net.WebSocket/ */
// Async Task.


namespace HSBot
{
    internal class Program
    {
        private volatile IServiceCollection _services;
        private volatile IServiceProvider _provider;
        private volatile DiscordSocketClient _client;
        private volatile InteractiveService _interactive;

        protected internal static bool Online = true;
        private CommandService _commands;
        private readonly string _version = Assembly.GetExecutingAssembly().GetName().Version.ToString();


        private static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        public async Task RunBotAsync()
        {
            Console.SetWindowSize(200, 50);
            await Utilities.Log(MethodBase.GetCurrentMethod(), $"Application started. V{_version}.");
            Console.Title = Config.BotConfig.ConsoleTitle;

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = Config.BotConfig.MessageCacheSize
            });
            _commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Debug
            });
            _interactive = new InteractiveService(_client);
            _services = new ServiceCollection() // Microsoft.Extensions.DependencyInjection
                            .AddSingleton(_client) // Singleton means to a static like class. <-- NOT REALLY
                            .AddSingleton(_commands) //<-- what is _commands?
                            .AddSingleton<CommandHandler>()
                            .AddSingleton(_interactive);
            //.BuildServiceProvider(); removed this

            _provider = _services.BuildServiceProvider();
            await _provider.GetRequiredService<CommandHandler>().InitializeAsync();

            await Utilities.Log(MethodBase.GetCurrentMethod(), "Services started.");

            _client.JoinedGuild += JoinedGuildHandler.Announce;
            _client.Log += ClientHandler.Log;
            _commands.Log += ClientHandler.Log;
            _client.Ready += CoreLoop.StartTimer;

            await Utilities.Log(MethodBase.GetCurrentMethod(), "Event handlers formed. Now logging in...", LogSeverity.Verbose);

            await _client.LoginAsync(TokenType.Bot, Config.BotConfig.Token);
            await _client.StartAsync();
            await _client.SetGameAsync(Config.BotConfig.Playing, "https://www.twitch.tv/alexlyee", ActivityType.Streaming);
            await Utilities.Log(MethodBase.GetCurrentMethod(), "Program running! :)");
            await Task.Delay(-1);
        }

        protected static void Shutdown(string Caller, string Reason, Exception ex = null)
        {
            System.Environment.Exit(0);
        }
    }
}
