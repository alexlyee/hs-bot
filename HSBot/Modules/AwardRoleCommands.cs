using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System.Linq;
using SchoolDiscordBot.Persistent;
using System.IO;
using System.Web;

namespace SchoolDiscordBot.Modules
{
    public sealed class AwardRoleCommands : ModuleBase<SocketCommandContext>
    {
        private enum RoleIDs : ulong
        {
            Bot_Developers = 416439081086484480,
            Alpha_Testers = 416439091333169154,
            Beta_Testers = 416439102683086850
        };




    }
}
