using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks; // Async Task.
using Discord;
using Discord.WebSocket;
using HSBot.Helpers;

namespace HSBot.Handlers
{
    public class ClientHandler
    {
        internal static Task Log(LogMessage arg)
        {
            Utilities.Log(arg.Source, arg.Message, arg.Severity, arg.Exception);
            return Task.CompletedTask;
        }

        internal static Task ChannelCreated(SocketChannel arg)
        {
            throw new NotImplementedException();
        }


    }
}

/*
        public event Func<SocketGuild, SocketGuild, Task> GuildUpdated;
        public event Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionRemoved;
        public event Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, Task> ReactionsCleared;
        public event Func<SocketRole, Task> RoleCreated;
        public event Func<SocketRole, Task> RoleDeleted;
        public event Func<SocketRole, SocketRole, Task> RoleUpdated;
        public event Func<SocketGuild, Task> JoinedGuild;
        public event Func<SocketUser, ISocketMessageChannel, Task> UserIsTyping;
        public event Func<SocketSelfUser, SocketSelfUser, Task> CurrentUserUpdated;
        public event Func<SocketUser, SocketVoiceState, SocketVoiceState, Task> UserVoiceStateUpdated;
        public event Func<SocketGuildUser, SocketGuildUser, Task> GuildMemberUpdated;
        public event Func<SocketUser, SocketUser, Task> UserUpdated;
        public event Func<SocketUser, SocketGuild, Task> UserUnbanned;
        public event Func<SocketUser, SocketGuild, Task> UserBanned;
        public event Func<SocketGuildUser, Task> UserLeft;
        public event Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, Task> ReactionAdded;
        public event Func<SocketGuild, Task> LeftGuild;
        public event Func<SocketGuild, Task> GuildAvailable;
        public event Func<SocketGuild, Task> GuildUnavailable;
        public event Func<SocketGuild, Task> GuildMembersDownloaded;
        public event Func<SocketGuildUser, Task> UserJoined;
        public event Func<Cacheable<IMessage, ulong>, SocketMessage, ISocketMessageChannel, Task> MessageUpdated;
        public event Func<int, int, Task> LatencyUpdated;
        public event Func<SocketMessage, Task> MessageReceived;
        public event Func<Cacheable<IMessage, ulong>, ISocketMessageChannel, Task> MessageDeleted;
        public event Func<Task> Connected;
        public event Func<Exception, Task> Disconnected;
        public event Func<Task> Ready;
        public event Func<SocketGroupUser, Task> RecipientRemoved;
        public event Func<SocketChannel, Task> ChannelCreated;
        public event Func<SocketChannel, Task> ChannelDestroyed;
        public event Func<SocketChannel, SocketChannel, Task> ChannelUpdated;
        public event Func<SocketGroupUser, Task> RecipientAdded;

    */