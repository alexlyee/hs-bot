using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Commands.Builders;
using Discord.Rest;
using Discord.WebSocket;
using System.Linq;
using HSBot.Persistent;
using System.IO;
using System.Web;

namespace HSBot.Modules
{
    public sealed class LeaderRoleCommands : ModuleBase<SocketCommandContext>
    {
        private enum RoleIDs : ulong
        {
            Directors = 416437607090880515,
            Managers = 416437658093617162,
            Contrivers = 416437665676787712
        };


        [Command("group")]
        public async Task Group([Remainder]string message)
        {
            var Group = await Context.Guild.CreateTextChannelAsync("a");
            await Group.TriggerTypingAsync();
            //RestGuildChannel.PermissionOverwrites
        }

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
