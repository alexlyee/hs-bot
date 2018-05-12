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
    public sealed class StatusRoleCommands : ModuleBase<SocketCommandContext>
    {



        private bool UserHasRole(SocketGuildUser user, ulong roleID)
        {
            return user.Roles.Contains(user.Guild.GetRole(roleID));
        }
        private ulong RoleIDFromName(SocketGuildUser user, string targetRoleName)
        {
            var result = from r in user.Guild.Roles
                         where r.Name == targetRoleName
                         select r.Id;

            return result.FirstOrDefault();
        }
        

    }
}
