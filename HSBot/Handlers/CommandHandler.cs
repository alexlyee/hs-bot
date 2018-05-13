using Discord.Commands;
using Discord.WebSocket;
using HSBot.Persistent;
using HSBot.Helpers;
using System.Reflection;
using System.Threading.Tasks;

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
            await _service.AddModulesAsync(Assembly.GetEntryAssembly());
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
                var discordBotTutorialGeneral = _client.GetChannel(GuildsData.FindGuildConfig(user.Guild.Id).LogChannelId) as SocketTextChannel;
                await discordBotTutorialGeneral.SendMessageAsync($"{user.Username} ({user.Id}) left **{user.Guild.Name}**!");
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
                var result = await _service.ExecuteAsync(context, argPos);
                if(!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }
                else
                {
                    // ------------------ create statistics machine. Class DatabaseHandler
                    // ------------------ create 
                    context.Guild.GetTextChannel(GuildsData.FindGuildConfig(context.Guild.Id).LogChannelId);
                }
            }
        }
    }
}
