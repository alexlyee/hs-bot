using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HSBot.Entities;
using HSBot.Helpers;
using HSBot.Modules.References;
using HSBot.Persistent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Rest;

namespace HSBot.Modules
{
    public sealed class LeaderRoleCommands : ModuleBase<SocketCommandContext>
    {
        [RequireContext(ContextType.Guild)]
        [Command("group")]
        public async Task Group([Remainder]string message)
        {
            string groupfile = GuildsData.GuildsFolder + "/" + Context.Guild.Id + "/Groups";
            if (!DataStorage.LocalFolderExists(groupfile, true))
                Utilities.Log(MethodBase.GetCurrentMethod(), "SubjectFile Created", LogSeverity.Verbose);
            GuildConfig guildconfig = GuildsData.FindOrCreateGuildConfig(Context.Guild);
            SocketGuildUser user = (SocketGuildUser)Context.User;
            if (!(UserIsGroupLeader(user, guildconfig)))
            {
                try
                {
                    await SendClassicEmbed("You don't have permissions to use this!", $"Contact a member with one of these roles: "
                        + $"\n - **{RoleFromID(user, guildconfig.ChannelManagerRoleID).Name}** "
                        + $"\n - **{RoleFromID(user, guildconfig.DirectorRoleID).Name}** "
                        + $"\n - **{RoleFromID(user, guildconfig.GroupManagerRoleID).Name}** "
                        + $"\n - **{RoleFromID(user, guildconfig.VoiceManagerRoleID).Name}** ");
                    return;
                }
                catch (Exception ex)
                {
                    await SendClassicEmbed("You don't have permissions to use this!", "Contact server owner, RoleIDs are not properly setup.");
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
                        await SendClassicEmbed("Syntax problem", "It seems no perameters were provided, check how to use the specified group type! :smiley:");
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
                    await SendClassicEmbed("Syntax problem", "No group type found! Check the syntax for this command! :smiley:");
                    break;
            } // Foreward to corresponding Task.
            return;
        }


        public async Task Group_Class(SocketCommandContext context, params string[] list)
        {
            EmbedBuilder embed = new EmbedBuilder();
            string subjectfolder = GuildsData.GuildsFolder + "/" + Context.Guild.Id + "/ClassSubjects";
            if (!DataStorage.LocalFolderExists(subjectfolder, true))
                Utilities.Log(MethodBase.GetCurrentMethod(), "SubjectFile Created", LogSeverity.Verbose);
            var ClassName = list[1].Replace("\"", "");
            List<ulong> subjectids;
            string subjectname;


            switch (ClassName) // Subject functions
            {

                case "Subject.Add":
                    subjectname = list[2].Replace("\"", "");
                    subjectids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(subjectfolder), 0);
                    List<Subject> subjects;
                    RestVoiceChannel subjectVC = Context.Guild.CreateVoiceChannelAsync(subjectname).Result;
                    RestTextChannel subjectTC = Context.Guild.CreateTextChannelAsync(subjectname).Result;
                    RestRole subjectR = Context.Guild.CreateRoleAsync(subjectname).Result;
                    Subject subject = new Subject
                    {
                        id = GenerateID(subjectfolder),
                        name = subjectname,
                        voicechannelid = subjectVC.Id,
                        textchannelid = subjectTC.Id,
                        roleid = subjectR.Id,
                    };
                    // Add voice channel perms.
                    await subjectVC.AddPermissionOverwriteAsync(GetEveryoneRole(), new OverwritePermissions
                    {
                        
                    });

                    
                    //
                    if (subjectids == null || !subjectids.Any())
                    {
                        subjectids = new List<ulong>
                        {
                            subject.id
                        };
                        DataStorage.StoreObject(subject, subjectfolder + "/" + subject.id + ".json", false);
                    }
                    else
                    {
                        subjects = DataStorage.GetObjectsInFolder<Subject>(subjectfolder);
                        Subject subjecttoremove = subjects.Find(x => String.Compare(x.name, subject.name) == 0);
                        if (!subject.Equals(null))
                        {
                            await SendClassicEmbed($"Subject with the name \"{subject.name}\" is already in the list.",
                                "If you want to see the full list, use Subject.Display! :smiley:");
                            return;
                        }
                        subjects.Add(subject);
                        DataStorage.StoreObject(subject, subjectfolder + "/" + subject.id + ".json", false);
                    }
                    subjects = DataStorage.GetObjectsInFolder<Subject>(subjectfolder);
                    await SendClassicEmbed($"Subject with the name \"{subject.name}\" added to {Context.Guild.Name}.",
                        "View them all with Subject.Display! :smiley:" +
                            (subjects.Count < 2 ? "" : $" *We're now up to {subjects.Count} subjects*"));
                    return;

                case "Subject.Remove":
                    subjectname = list[2].Replace("\"", "");
                    subjectids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(subjectfolder), 0);
                    if (subjectids == null || !subjectids.Any())
                        await SendClassicEmbed($"No subjects found on {Context.Guild.Name}.",
                            $"Use Subject.Add to add the first one! :smiley:");
                    else
                    {
                        subjects = DataStorage.GetObjectsInFolder<Subject>(subjectfolder);
                        Subject subjecttoremove = subjects.Find(x => String.Compare(x.name, subjectname) == 0);
                        if (subjecttoremove.Equals(null))
                            await SendClassicEmbed($"{subjectname} not found on {Context.Guild.Name}.",
                                $"Try to use Subject.Display to see it's exact name! :smiley:");
                        else
                        {
                            subjects.Remove(subjecttoremove);
                            DataStorage.DeleteFile(subjectfolder + "/" + subjecttoremove.id + ".json");
                            await SendClassicEmbed($"Subject with the name \"{subjecttoremove.name}\" removed from {Context.Guild.Name}.",
                                "View them all with Subject.Display! :smiley:" +
                                (subjects.Count < 2 ? "" : $" *We're now down to {subjects.Count} subjects*"));
                        }
                    }
                    return;

                case "Subject.Display":
                    subjectids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(subjectfolder), 0);
                    if (subjectids == null || !subjectids.Any())
                        await SendClassicEmbed($"No subjects added to {Context.Guild.Name}.", "Go ahead and add one with Subject.Add!");
                    else
                    {
                        subjects = DataStorage.GetObjectsInFolder<Subject>(subjectfolder);
                        int count = 0;
                        embed.WithTitle("**Subjects in " + Context.Guild.Name + "**")
                            .WithDescription("Add with Subject.Add, remove with Subject.Remove! :smiley:")
                            .WithColor(new Color(60, 176, 222));
                        foreach (Subject s in subjects)
                        {
                            count++;
                            embed.AddField($"{count} | ReferenceID: {s.id}", s.name);
                            if (IsDivisible(count, 24))
                            {
                                await Context.Channel.SendMessageAsync("", false, embed);
                                embed = new EmbedBuilder();
                                embed.WithTitle("Some more subjects in " + Context.Guild.Name + "...")
                                    .WithColor(new Color(60, 176, 222));
                            }
                        }
                        embed.WithFooter(count + " subjects in this school.", "https://i.imgur.com/HAI5vMj.png");
                        await Context.Channel.SendMessageAsync("", false, embed);
                    }
                    return;

            } // Subject functions.

            SocketGuildChannel channel = GetChannel(400434742790586378);
            List<Subject> Fsubjects = DataStorage.GetObjectsInFolder<Subject>(subjectfolder);
            string[] Hours; string Teacher; Subject Fsubject;
            try
            {
                Teacher = list[2].Replace("\"", "");

                string Hourslist = list[3].Replace("\"", "");
                if (Hourslist.Contains(",")) Hours = Hourslist.Split(","); else Hours = new string[1] { Hourslist };

                subjectname = list[4].Replace("\"", "");
                Fsubject = Fsubjects.Find(x => String.Compare(x.name, subjectname) == 0);
                if (Fsubject.Equals(null))
                {
                    await SendClassicEmbed($"**Subject not found! with name {Fsubject.name}**",
                        "Use Subject.Add to add the new subject! Or ensure it matches.");
                    return;
                }


                Utilities.Log(MethodBase.GetCurrentMethod(), "2" + Teacher + "3" + Fsubject, LogSeverity.Verbose);
            } catch (Exception ex)
            {
                await SendClassicEmbed("**Syntax problem!**", "Review how the class command is formatted.");
                Utilities.Log(MethodBase.GetCurrentMethod(), "Exception expanding permaters on class command", ex, LogSeverity.Warning);
                return;
            } // Assign and try subject.

            /* TO DO:
             * Duplicate Subject system for Hours and Teacher.
             * Class simply attaches to a teacher and hour, and possibly a subject. Class contains a text channel and a voice channel.
             * Classes have an hour, teacher, and sometimes shared subject. Channels are for subjects and classes without subjects.
             * Roles include each class (used for students) and subjects (used for visitors and graduates)
             * When teachers are removed, system asks for replacement teacher for classes.
             * When subjects are removed, system asks for replacement subject for classes, or individual class channels are distributed.
             * make sure there is an agreed opoun naming scheme for classes. and check for anything ignoring case.
             * 
             * Lastly, MyClasses command for students asks for class for each hour.
             * MyGroups command can add anyone to subjects.
             * 
             * Under ClassSubjects save name.json for subject name and inside store voicechannelid, textchannelid, and roleid.
             * Store teachers.json
             * Store hours.json
             * Under Classes save id.json for class id and inside store name, teacher, hour, roleid, possible subject name or voicechannelid and textchannelid.
             * classes have ids because they can have the same name.
             */

            Utilities.Log(MethodBase.GetCurrentMethod(), "SubjectFile Created", LogSeverity.Verbose);
            await Context.Channel.SendMessageAsync($"{ClassName} // {Teacher} {Hours.ToString()} [{Fsubject.name}]");
            await (Context.Channel as ITextChannel)?.ModifyAsync(x =>
            {
                x.Name = "do-not-enter";
                x.Position = 0;
                x.Topic = "topic of the channel";
            });
            //DataStorage.StoreObject(selection, GuildsData.GuildsFolder + "/" + Context.Guild.Id + ".json", true);
            //var Group = await Context.Guild.CreateTextChannelAsync(string.Join(" ", list.Skip(1)));
            //await Group.TriggerTypingAsync();
        }



        /// <summary>
        /// Gemerates unique ID ulong.
        /// </summary>
        /// <param name="inUse"></param>
        /// <returns></returns>
        public ulong GenerateID(ulong[] inUse)
        {
            HashSet<ulong> IDs = new HashSet<ulong>(inUse);
            ulong newID = Utilities.NextUlong();
            while (IDs.Add(newID) == false) newID = Utilities.NextUlong();
            return newID;
        }
        /// <summary>
        /// Retrieves file names without extensions and converts to list of ulong.
        /// Generates unique ID ulong.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public ulong GenerateID(string folder) => GenerateID(SafeConversion<ulong>(DataStorage.GetFilesInFolder(folder), 0).ToArray());
        /// <summary>
        /// Tries to convert each in list. If a string is empty and skip is not set to true, it will add defaultValue to the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toConvert"></param>
        /// <param name="defaultValue"></param>
        /// <param name="skipEmptyValues"></param>
        /// <returns></returns>
        public List<T> SafeConversion<T>(string[] toConvert, T defaultValue, bool skipEmptyValues = true)
        {
            List<T> list = new List<T>();
            foreach (string str in toConvert)
            {
                try
                {
                    if (!string.IsNullOrEmpty(str))
                        list.Add((T)Convert.ChangeType(str, typeof(T)));
                    else if (!skipEmptyValues)
                        list.Add(defaultValue);
                }
                catch (Exception ex)
                {
                    Utilities.Log(MethodBase.GetCurrentMethod(), $"Error converting file name to {typeof(T).ToString()} [{str}]", ex, LogSeverity.Warning);
                }
            }
            return list;
        }
        public SocketGuildChannel GetChannel(ulong id) => Context.Guild.Channels.FirstOrDefault(x => x.Id == id);
        public SocketRole GetEveryoneRole() => Context.Guild.Roles.FirstOrDefault(x => x.IsEveryone);
        public async Task SendClassicEmbed(string title, string desc)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle(title)
                .WithDescription(desc)
                .WithColor(new Color(60, 176, 222))
                .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png");
            await Context.Channel.SendMessageAsync("", false, embed);
        }
        private static bool IsDivisible(int x, int n)
        {
            Utilities.Log(MethodBase.GetCurrentMethod(), x + " % " + n + " = " + (x % n), LogSeverity.Debug);
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
