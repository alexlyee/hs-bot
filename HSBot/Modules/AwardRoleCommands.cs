
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System.Linq;
using HSBot.Persistent;
using System.IO;
using System.Web;

namespace HSBot.Modules
{
    public sealed class AwardRoleCommands : ModuleBase<SocketCommandContext>
    {
        private enum RoleIDs : ulong
        {
            BotDevelopers = 416439081086484480,
            AlphaTesters = 416439091333169154,
            BetaTesters = 416439102683086850
        };




    }
}
