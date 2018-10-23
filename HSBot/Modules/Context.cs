using System;
using HSBot.Handlers;
using System.Net.Http;
using Discord.Commands;
using HSBot.Entities;
using Microsoft.Extensions.DependencyInjection;
using Discord.WebSocket;
using HSBot.Helpers;
using HSBot.Persistent;
// Thank you Charly#7094

namespace HSBot.Modules
{
    public class Context : SocketCommandContext
    {
        public new SocketUser User { get; }
        public new SocketGuild Guild { get; }
        public SecureRandom Random { get; }
        private IServiceProvider _provider;
        public UserAccount _userAccount { get; }
        public GuildConfig GuildConfig { get; }

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
            GuildConfig = GuildsData.FindOrCreateGuildConfig(Guild);
            _userAccount = UserAccounts.CreateUserAccount(message.Author.Id, true);
        }
    }
}
