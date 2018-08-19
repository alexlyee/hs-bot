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
using HSBot.Entities;
using HSBot.Helpers;
using System.Reflection;

namespace HSBot.Modules
{
    public sealed class LeaderRoleCommands : ModuleBase<SocketCommandContext>
    {
        [Command("group")]
        public async Task Group([Remainder]string message)
        {
            GuildConfig guildconfig = GuildsData.FindOrCreateGuildConfig(Context.Guild);
            SocketGuildUser user = (SocketGuildUser)Context.User;
            if (!(UserIsGroupLeader(user, guildconfig)))
            {
                try
                {
                    var embed = new EmbedBuilder();
                    embed.WithTitle("You don't have permissions to use this!")
                        .WithDescription($"Contact a member with one of these roles: "
                        + $" {RoleFromID(user, guildconfig.ChannelManagerRoleID).Name} "
                        + $" {RoleFromID(user, guildconfig.DirectorRoleID).Name} "
                        + $" {RoleFromID(user, guildconfig.GroupManagerRoleID).Name} "
                        + $" {RoleFromID(user, guildconfig.VoiceManagerRoleID).Name} ")
                        .WithColor(new Color(60, 176, 222))
                        .WithFooter(" -Alex https://discord.gg/emFQ6s4", "https://i.imgur.com/HAI5vMj.png");
                    await Context.Channel.SendMessageAsync("", false, embed);
                    return;
                }
                catch
                {
                    var embed = new EmbedBuilder();
                    embed.WithTitle("You don't have permissions to use this!")
                        .WithDescription("Contact server owner, RoleIDs are not properly setup.")
                        .WithColor(new Color(60, 176, 222))
                        .WithFooter(" -Alex https://discord.gg/emFQ6s4", "https://i.imgur.com/HAI5vMj.png");
                    await Context.Channel.SendMessageAsync("", false, embed);
                    return;
                }
            }
            
            var Group = await Context.Guild.CreateTextChannelAsync("a");
            await Group.TriggerTypingAsync();
            //RestGuildChannel.PermissionOverwrites
        }
        




        private bool UserIsGroupLeader(SocketGuildUser user, GuildConfig guildconfig)
        {
            try
            {
                if (UserHasRole(user, guildconfig.ChannelManagerRoleID)
                    || UserHasRole(user, guildconfig.DirectorRoleID)
                    || UserHasRole(user, guildconfig.GroupManagerRoleID)
                    || UserHasRole(user, guildconfig.ChannelManagerRoleID)
                    || UserHasRole(user, guildconfig.VoiceManagerRoleID)) return true;
            }
            catch (Exception ex)
            {
                Utilities.Log(MethodBase.GetCurrentMethod(), "Failure checking roles.", ex, LogSeverity.Warning);
            }
            Utilities.Log(MethodBase.GetCurrentMethod(), "F",LogSeverity.Warning);
            return false;
        }
        private bool UserHasRole(SocketGuildUser user, ulong roleId)
        {
            foreach (SocketRole role in user.Roles)
            {
                if (role.Id == roleId) return true;
            }
            return false;
        }
        private SocketRole RoleFromName(SocketGuildUser user, string targetRoleName)
        {
            var result = from r in user.Guild.Roles
                         where r.Name == targetRoleName
                         select r;

            return result.FirstOrDefault();
        }
        private SocketRole RoleFromID(SocketGuildUser user, ulong roleId)
        {
            var result = from r in user.Guild.Roles
                         where r.Id == roleId
                         select r;

            return result.FirstOrDefault();
        }

    }
}
