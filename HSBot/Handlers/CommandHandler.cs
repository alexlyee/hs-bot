using Discord.Commands;
using Discord.WebSocket;
using HSBot.Persistent;
using HSBot.Helpers;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using System.IO;
using System;

namespace HSBot.Handlers
{
    internal class CommandHandler
    {
        private volatile DiscordSocketClient _client;
        private volatile CommandService _cmdService;
        private volatile SocketCommandContext context;
        private readonly IServiceProvider _serviceProvider;

        public async Task InitializeAsync()
        {
            await _cmdService.AddModulesAsync(Assembly.GetEntryAssembly());
            _client.MessageReceived += HandleCommandAsync;
            _client.UserJoined += _client_UserJoined;
            _client.UserLeft += _client_UserLeft;
            Global.Client = _client;
            await Utilities.Log(MethodBase.GetCurrentMethod(), "CommandHandler initialized.", LogSeverity.Verbose);
        }

        public CommandHandler(DiscordSocketClient client, CommandService cmdService, IServiceProvider serviceProvider)
        {
            _client = client;
            _cmdService = cmdService;
            _serviceProvider = serviceProvider;
            Utilities.Log(MethodBase.GetCurrentMethod(), "CommandHandler constructed.", LogSeverity.Verbose);
        }


        private static async Task _client_UserJoined(SocketGuildUser user)
        {
            var dmChannel = await user.GetOrCreateDMChannelAsync();
            await dmChannel.SendMessageAsync($"Hey {user.Mention}! Welcome to **{user.Guild.Name}**. try using ``@high school#9905 help`` for all the commands!");
        }

        private async Task _client_UserLeft(SocketGuildUser user)
        {
            if (user.Guild.Name == "IA Central Discord")
            {
                if (_client.GetChannel(GuildsData.FindGuildConfig(user.Guild.Id).LogChannelID) is SocketTextChannel discordBotTutorialGeneral)
                    await discordBotTutorialGeneral.SendMessageAsync(
                        $"{user.Username} ({user.Id}) left **{user.Guild.Name}**!");
            }
        }

        public async Task HandleCommandAsync(SocketMessage s)
        {
            if (!(s is SocketUserMessage msg)) return;
            context = new SocketCommandContext(_client, msg);
            int argPos = 0;
            if (msg.HasStringPrefix(GuildsData.FindOrCreateGuildConfig(context.Guild).Prefix, ref argPos)
                || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                await Utilities.Log(MethodBase.GetCurrentMethod(), "Command detected.", Discord.LogSeverity.Verbose);
                /*
                await Utilities.Log(MethodBase.GetCurrentMethod(), UserAccounts.GetAccount(_client.CurrentUser.Id, true).ToString());
                if (UserAccounts.GetAccount(_client.CurrentUser.Id, true).Equals(null))
                {
                    await Utilities.Log("HandleCommandAsync",
                        $"Welcome the new user by the name of {_client.CurrentUser.Username}!");
                    var embed = new EmbedBuilder();
                    embed.WithTitle($"This is your first time using our bot {_client.CurrentUser.Username}!")
                        .WithDescription("Welcome!!")
                        .WithColor(new Color(Config.BotConfig.BotThemeColorR, Config.BotConfig.BotThemeColorG, Config.BotConfig.BotThemeColorB)); // Should reduce it to a color class.
                    await context.Channel.SendMessageAsync("", false, embed);
                }
                */

                var result = await _cmdService.ExecuteAsync(context, argPos);
                if(!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }
                else
                {
                    // ------------------ create statistics machine. Class DatabaseHandler
                    // ------------------ create 
                    context.Guild.GetTextChannel(GuildsData.FindGuildConfig(context.Guild.Id).LogChannelID);
                }
            }
        }
    }
}
