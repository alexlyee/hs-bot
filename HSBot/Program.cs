using Discord;
using Discord.Commands; /* Namespace of discord command tools.
https://www.nuget.org/packages/Discord.Net.Commands/ */
using Discord.WebSocket; /* Namespace of discord web tools.
https://www.nuget.org/packages/Discord.Net.WebSocket/ */
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks; // Async Task.
using System.Collections.Generic;
using SchoolDiscordBot.Entities;
using SchoolDiscordBot.Handlers;
using SchoolDiscordBot.Helpers;
using SchoolDiscordBot.Persistent;
using SchoolDiscordBot.Core;

namespace SchoolDiscordBot
{
    internal class Program
    {
        private DiscordSocketClient _client;
        private IServiceProvider _services; 
        protected internal bool Online = true;
        private CommandService _commands;
        private readonly string _version = Assembly.GetExecutingAssembly().GetName().Version.ToString();


        private static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        public async Task RunBotAsync()
        {
            Console.SetWindowSize(200, 50);
            await Utilities.Log(MethodBase.GetCurrentMethod(), $"Application started. V{_version}.");
            Console.Title = Config.config.ConsoleTitle;

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 10000
            });
            _commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Debug
            });
            _services = new ServiceCollection() // Microsoft.Extensions.DependencyInjection
                .AddSingleton(_client) // Singleton means to a static like class.
                .AddSingleton(_commands)
                .BuildServiceProvider();
            Global.Client = _client;
            var commandhandler = new CommandHandler();
            await Utilities.Log(MethodBase.GetCurrentMethod(), "Objects created.", LogSeverity.Verbose);

            await commandhandler.InitializeAsync(_client);

            _client.JoinedGuild += JoinedGuildHandler.Announce;
            _client.Log += ClientHandler.Log;
            _commands.Log += ClientHandler.Log;
            _client.Ready += CoreLoop.StartTimer;


            await _client.LoginAsync(TokenType.Bot, Config.config.Token);
            await _client.StartAsync();
            await Task.Delay(-1);
        }

    }
}
