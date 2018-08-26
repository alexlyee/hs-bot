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
using System.Web;
using System.Net.Http;

namespace HSBot.Modules
{
    public sealed class LeaderRoleCommands : ModuleBase<SocketCommandContext>
    {
        [RequireContext(ContextType.Guild)]
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
                        + $"\n - **{RoleFromID(user, guildconfig.ChannelManagerRoleID).Name}** "
                        + $"\n - **{RoleFromID(user, guildconfig.DirectorRoleID).Name}** "
                        + $"\n - **{RoleFromID(user, guildconfig.GroupManagerRoleID).Name}** "
                        + $"\n - **{RoleFromID(user, guildconfig.VoiceManagerRoleID).Name}** ")
                        .WithColor(new Color(60, 176, 222))
                        .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png");
                    await Context.Channel.SendMessageAsync("", false, embed);
                    return;
                }
                catch (Exception ex)
                {
                    var embed = new EmbedBuilder();
                    embed.WithTitle("You don't have permissions to use this!")
                        .WithDescription("Contact server owner, RoleIDs are not properly setup.")
                        .WithColor(new Color(60, 176, 222))
                        .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png");
                    await Context.Channel.SendMessageAsync("", false, embed);
                    Utilities.Log(MethodBase.GetCurrentMethod(), "Failure with RoleIDs", ex, LogSeverity.Warning);
                    return;
                }
            }

            var firstWord = message.IndexOf(" ") > -1
                  ? message.Substring(0, message.IndexOf(" "))
                  : message;

            switch (firstWord)
            {
                case "Overseen":
                case "Inherent":
                case "Idiomatic":
                case "Functional":
                case "Class":
                    if (firstWord == message)
                    {
                        var Embed = new EmbedBuilder();
                        Embed.WithTitle("Syntax problem")
                            .WithDescription("It seems no perameters were provided, check how to use the specified group type! :smiley:")
                            .WithColor(new Color(60, 176, 222))
                            .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png");
                        await Context.Channel.SendMessageAsync("", false, Embed);
                        return;
                    }
                    string scan = message.Substring(message.IndexOf(" "));
                    string[] arguments = ParseArguments(scan);
                    MethodInfo Command = this.GetType().GetMethod($"Group_{firstWord}");
                    try
                    {
                        Command.Invoke(this, new object[] { Context, arguments });
                    }
                    catch (Exception ex)
                    {
                        Utilities.Log(MethodBase.GetCurrentMethod(), "Error invoking command.", ex, LogSeverity.Error);
                    }
                    break;
                default:
                    var embed = new EmbedBuilder();
                    embed.WithTitle("Syntax problem")
                        .WithDescription("No group type found! Check the syntax for this command! :smiley:")
                        .WithColor(new Color(60, 176, 222))
                        .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png");
                    await Context.Channel.SendMessageAsync("", false, embed);
                    return;
            }
        }


        public async Task Group_Class(SocketCommandContext context, params string[] list)
        {
            EmbedBuilder embed = new EmbedBuilder();
            string subjectfile = GuildsData.GuildsFolder + "/" + Context.Guild.Id + "/ClassSubjects";
            if (!DataStorage.LocalFolderExists(subjectfile, true))
                Utilities.Log(MethodBase.GetCurrentMethod(), "SubjectFile Created", LogSeverity.Verbose);
            var ClassName = list[1].Replace("\"", "");

            string subject, file;
            embed = new EmbedBuilder();
            switch (ClassName)
            {
                case "Subject.Add":
                    subject = list[2].Replace("\"", "");
                    file = $"{subjectfile}/Subjects.json";
                    string[] subjects = DataStorage.LoadStringArray(file, true);
                    int subjectscount = 1;
                    if (subjects == null)
                    {
                        subjects = new string[1] { subject };
                        DataStorage.StoreStringArray(subjects, file, true);
                    }
                    else
                    {
                        subjectscount = subjects.Length + 1;
                        string[] newsubjects = new string[subjectscount];
                        for (int i = 0; i < subjects.Length; i++) newsubjects[i] = subjects[i];
                        newsubjects[subjects.Length] = subject;
                        DataStorage.StoreStringArray(newsubjects, file, true);
                    }
                    embed = new EmbedBuilder();
                    embed.WithTitle($"Subject with the name \"{subject}\" added to {Context.Guild.Name}.")
                        .WithDescription($"View them all with Subject.Display! *We're now up to {subjectscount} subjects* :smiley:")
                        .WithColor(new Color(60, 176, 222))
                        .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png");
                    await Context.Channel.SendMessageAsync("", false, embed);
                    return;
                case "Subject.Remove":
                    subject = list[2].Replace("\"", "");
                    file = $"{subjectfile}/Subjects.json";

                    return;
                case "Subject.Display":
                    file = $"{subjectfile}/Subjects.json";
                    subjects = DataStorage.LoadStringArray(file, true);
                    if (subjects == null)
                    {
                        embed = new EmbedBuilder();
                        embed.WithTitle($"No subjects added to {Context.Guild.Name}.")
                            .WithDescription("Go ahead and add one with Subject.Add! :smiley:")
                            .WithColor(new Color(60, 176, 222))
                            .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png");
                        await Context.Channel.SendMessageAsync("", false, embed);
                        return;
                    }
                    else
                    {
                        embed = new EmbedBuilder();
                        int count = 0;
                        embed.WithTitle("**Subjects in " + Context.Guild.Name + "**")
                            .WithColor(new Color(60, 176, 222));
                        foreach (string s in subjects)
                        {
                            embed.AddField(s, "");
                            if (IsDivisible(count, 24))
                            {
                                await Context.Channel.SendMessageAsync("", embed: embed);
                                embed = new EmbedBuilder();
                                embed.WithTitle("Some more subjects in " + Context.Guild.Name + "...")
                                    .WithColor(new Color(60, 176, 222));
                            }
                            count++;
                        }
                        embed.WithFooter(count + " subjects in this school.", "https://i.imgur.com/HAI5vMj.png");
                        await Context.Channel.SendMessageAsync("", embed: embed);
                    }
                    return;
            }

            var Teacher = list[2].Replace("\"", "");
            var Subject = list[3].Replace("\"", "");
            string[] Hours = list[4].Replace("\"", "").Split(",");
            await Context.Channel.SendMessageAsync($"{ClassName} // {Teacher} {Hours.ToString()} [{Subject}]");
            //DataStorage.StoreObject(selection, GuildsData.GuildsFolder + "/" + Context.Guild.Id + ".json", true);
            //var Group = await Context.Guild.CreateTextChannelAsync(string.Join(" ", list.Skip(1)));
            //await Group.TriggerTypingAsync();
        }





        private bool IsDivisible(int x, int n)
        {
            return (x % n) == 0;
        }
        public static string[] SplitArguments(string commandLine)
        {
            var parmChars = commandLine.ToCharArray();
            var inSingleQuote = false;
            var inDoubleQuote = false;
            for (var index = 0; index < parmChars.Length; index++)
            {
                if (parmChars[index] == '"' && !inSingleQuote)
                {
                    inDoubleQuote = !inDoubleQuote;
                    parmChars[index] = '\n';
                }  
                if (parmChars[index] == '\'' && !inDoubleQuote)
                {
                    inSingleQuote = !inSingleQuote;
                    parmChars[index] = '\n';
                }
                if (!inSingleQuote && !inDoubleQuote && parmChars[index] == ' ')
                    parmChars[index] = '\n';
            }
            return (new string(parmChars)).Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }
        public static string[] ParseArguments(string commandLine)
        {
            char[] parmChars = commandLine.ToCharArray();
            bool inQuote = false;
            for (int index = 0; index < parmChars.Length; index++)
            {
                if (parmChars[index] == '"')
                    inQuote = !inQuote;
                if (!inQuote && parmChars[index] == ' ')
                    parmChars[index] = '\n';
            }
            return (new string(parmChars)).Split('\n');
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
            //Utilities.Log(MethodBase.GetCurrentMethod(), "F",LogSeverity.Warning);
            return false;
        }
        private bool UserHasRole(SocketGuildUser user, ulong roleId)
        {
            foreach (SocketRole role in user.Roles)
            {
                // Utilities.Log(MethodBase.GetCurrentMethod(), role.Id + " -- " + roleId);
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
