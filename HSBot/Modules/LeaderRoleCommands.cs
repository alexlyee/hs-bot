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
    public sealed class LeaderRoleCommands : ModuleBase<SocketCommandContext>
    {
        private enum RoleIDs : ulong
        {
            Directors = 416437607090880515,
            Managers = 416437658093617162,
            Contrivers = 416437665676787712
        };



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
