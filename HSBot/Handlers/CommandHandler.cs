﻿using Discord.Commands;
using Discord.WebSocket;
using HSBot.Persistent;
using HSBot.Helpers;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using System.IO;
using System;
using HSBot.Modules;

namespace HSBot.Handlers
{
    internal class CommandHandler
    {
        private DiscordSocketClient _client;
        private CommandService _service;

        public async Task InitializeAsync(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            _client.MessageReceived += HandleCommandAsync;
            _client.UserJoined += _client_UserJoined;
            _client.UserLeft += _client_UserLeft;
            Global.Client = client;
            await Utilities.Log(MethodBase.GetCurrentMethod(), "CommandHandler Initialized.");
        }

        /*
        private async Task HandleTagsAsync(SocketMessage s)
        {


            var msg = s as SocketUserMessage;
            var context = new SocketCommandContext(_client, msg);
            int argPos = 0;
            if (msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {

            }
        }
        */
        private static async Task _client_UserJoined(SocketGuildUser user)
        {
            var dmChannel = await user.GetOrCreateDMChannelAsync();
            await dmChannel.SendMessageAsync($"Hey {user.Mention}! Welcome to **{user.Guild.Name}**. try using ``@high school#9905 help`` for all the commands!");
        }

        private async Task _client_UserLeft(SocketGuildUser user)
        {
            if (user.Guild.Name == "Discord-BOT-Tutorial")
            {
                if (_client.GetChannel(GuildsData.FindGuildConfig(user.Guild.Id).LogChannelID) is SocketTextChannel discordBotTutorialGeneral)
                    await discordBotTutorialGeneral.SendMessageAsync(
                        $"{user.Username} ({user.Id}) left **{user.Guild.Name}**!");
            }
        }

        public async Task HandleCommandAsync(SocketMessage s)
        {
            if (!(s is SocketUserMessage msg)) return;
            var context = new SocketCommandContext(_client, msg);
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
                }DF
                */
                var result = await _service.ExecuteAsync(context, argPos, null);
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
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
