using System;
using HSBot.Handlers;
using System.Net.Http;
using Discord.Commands;
using HSBot.Entities;
using Microsoft.Extensions.DependencyInjection;
using Discord.WebSocket;
using HSBot.Helpers;
// Thank you Charly#7094

namespace HSBot.Modules
{
    public class Context : SocketCommandContext
    {
        public new SocketUser User { get; }
        public new SocketGuild Guild { get; }
        public SecureRandom Random { get; }
        private IServiceProvider _provider;
        private UserAccount _userAccount;


        public new DiscordSocketClient Client { get; }
        public HttpClient HttpClient { get; }
        public new SocketUserMessage Message { get; }
        public new ISocketMessageChannel Channel { get; }

        public Context(DiscordSocketClient client, SocketUserMessage message, IServiceProvider provider) : base(client, message)
        {
            Client = client;
            Message = message;
            _provider = provider;
            User = message.Author;
            Channel = message.Channel;
            Guild = (message.Channel as SocketGuildChannel)?.Guild;
        }
    }
}
