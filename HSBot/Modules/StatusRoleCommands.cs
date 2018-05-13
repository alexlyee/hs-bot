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
    public sealed class StatusRoleCommands : ModuleBase<SocketCommandContext>
    {



        private bool UserHasRole(SocketGuildUser user, ulong roleId)
        {
            return user.Roles.Contains(user.Guild.GetRole(roleId));
        }
        private ulong RoleIdFromName(SocketGuildUser user, string targetRoleName)
        {
            var result = from r in user.Guild.Roles
                         where r.Name == targetRoleName
                         select r.Id;

            return result.FirstOrDefault();
        }
        

    }
}
