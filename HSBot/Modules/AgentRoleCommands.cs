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
    public sealed class AgentRoleCommands : ModuleBase<SocketCommandContext>
    {
        private enum RoleIDs : ulong
        {
            Administrators = 416439617466793994,
            Moderators = 416439633442897921
        };





    }
}
