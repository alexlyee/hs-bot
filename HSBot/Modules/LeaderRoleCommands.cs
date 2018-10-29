using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using HSBot.Entities;
using HSBot.Helpers;
using HSBot.Modules.References;
using HSBot.Persistent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace HSBot.Modules
{
    public sealed partial class LeaderRoleCommands : InteractiveBase
    {
        /*
        [Command("t")]
        public async Task TestCommand()
        {
            var g = await CreateGroupClass();
            await SendClassicEmbed("Class test info..", 
                g.name + '|' + g.id + '|' + g.hours + '|' + g.subject + '|' + g.teacher);
        }
        */
        [RequireContext(ContextType.Guild)]
        [Command("group")]
        public async Task Group([Remainder]string message)
        {
            string groupfile = GuildsData.GuildsFolder + "/" + Context.Guild.Id + "/Groups";
            if (!DataStorage.LocalFolderExists(groupfile, true))
                await Utilities.Log(MethodBase.GetCurrentMethod(), "SubjectFile Created", LogSeverity.Verbose);
            GuildConfig guildconfig = GuildsData.FindOrCreateGuildConfig(Context.Guild);
            SocketGuildUser user = (SocketGuildUser)Context.User;
            if (!(UserIsGroupLeader(guildconfig)))
            {
                try
                {
                    await SendClassicEmbed("You don't have permissions to use this!", $"Contact a member with one of these roles: "
                        + $"\n - **{RoleFromID(guildconfig.ChannelManagerRoleID).Name}** "
                        + $"\n - **{RoleFromID(guildconfig.DirectorRoleID).Name}** "
                        + $"\n - **{RoleFromID(guildconfig.GroupManagerRoleID).Name}** "
                        + $"\n - **{RoleFromID(guildconfig.VoiceManagerRoleID).Name}** ");
                    return;
                }
                catch (Exception ex)
                {
                    await SendClassicEmbed("You don't have permissions to use this!", "Contact server owner, RoleIDs are not properly setup.");
                    await Utilities.Log(MethodBase.GetCurrentMethod(), "Failure with RoleIDs", ex, LogSeverity.Warning);
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
                        await Utilities.Log(MethodBase.GetCurrentMethod(), "Error invoking command.", ex, LogSeverity.Error);
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
            var embed = new EmbedBuilder();
            string classfolder = GuildsData.GuildsFolder + "/" + Context.Guild.Id + "/GroupClasses";
            string subjectfolder = GuildsData.GuildsFolder + "/" + Context.Guild.Id + "/ClassSubjects";
            string teacherfolder = GuildsData.GuildsFolder + "/" + Context.Guild.Id + "/ClassTeachers";
            string hourfolder = GuildsData.GuildsFolder + "/" + Context.Guild.Id + "/ClassHours";
            if (!DataStorage.LocalFolderExists(subjectfolder, true))
                await Utilities.Log(MethodBase.GetCurrentMethod(), "SubjectFolder Created for " + context.Guild.Name, LogSeverity.Verbose);
            if (!DataStorage.LocalFolderExists(teacherfolder, true))
                await Utilities.Log(MethodBase.GetCurrentMethod(), "TeacherFolder Created for " + context.Guild.Name, LogSeverity.Verbose);
            if (!DataStorage.LocalFolderExists(hourfolder, true))
                await Utilities.Log(MethodBase.GetCurrentMethod(), "HourFolder Created for " + context.Guild.Name, LogSeverity.Verbose);
            if (!DataStorage.LocalFolderExists(hourfolder, true))
                await Utilities.Log(MethodBase.GetCurrentMethod(), "ClassFolder Created for " + context.Guild.Name, LogSeverity.Verbose);
            var ClassName = list[1].Replace("\"", "");
            

            List<ulong> classids;
            string classname;
            List<GroupClass> Classes;
            References.GroupClass Class;
            /*
            switch (ClassName) // Class Functions
            {
                case "Class.New":
                    classname = list[2].Replace("\"", "");
                    classids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(classfolder), 0);

                    if (classids == null || !classids.Any())
                    {
                        // if there are no classes.
                        // find selected hours, and/or teachers, and possibly subject.
                        // if there is a failure to find any of these, abort.
                        // if there is a class with matching teacher and (just one) class then remind user of impossibility.
                        //Class = new GroupClass()
                        Class = await CreateGroupClass();
                        DataStorage.StoreObject(Class, classfolder + "/" + Class.id + ".json", true);
                    }
                    else
                    {
                        Classes = DataStorage.GetObjectsInFolder<GroupClass>(classfolder);
                        GroupClass classtoremove = Classes.Find(x => x.name == classname);
                        if (classtoremove.name == classname)
                        {
                            await SendClassicEmbed($"class with the name \"{classtoremove.name}\" is already in the list.",
                                "If you want to see the full list, use class.Display! :smiley:");
                            return;
                        }
                        Class = await CreateGroupClass();

                        DataStorage.StoreObject(Class, classfolder + "/" + Class.id + ".json", true);
                    }


                    Classes = DataStorage.GetObjectsInFolder<GroupClass>(classfolder);
                    embed = new EmbedBuilder();
                    embed.WithTitle($"class with the name \"{Class.title}\" added to {Context.Guild.Name}.")
                        .WithDescription("View them all with class.Display! :smiley:" +
                            (classs.Count < 2 ? "" : $" *We're now up to {classs.Count} classs*"))
                        .AddField("**Data**", $":notepad_spiral: ReferenceID: `{hour.id}`")
                        .WithColor(new Color(60, 176, 222))
                        .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png");
                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                    return;

                case "Class.Remove":
                    hourname = list[2].Replace("\"", "");
                    hourids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(hourfolder), 0);
                    if (hourids == null || !hourids.Any())
                        await SendClassicEmbed($"No hours found on {Context.Guild.Name}.",
                            $"Use Hour.Add to add the first one! :smiley:");
                    else
                    {
                        hours = DataStorage.GetObjectsInFolder<Hour>(hourfolder);
                        Hour hourtoremove = hours.Find(x => x.title == hourname);
                        if (!(hourtoremove.title == hourname))
                            await SendClassicEmbed($"{hourname} not found on {Context.Guild.Name}.",
                                $"Try to use Hour.Display to see it's exact name! :smiley:");
                        else
                        {
                            hours.Remove(hourtoremove);
                            DataStorage.DeleteFile(hourfolder + "/" + hourtoremove.id + ".json");
                            await SendClassicEmbed($"hour with the name \"{hourtoremove.title}\" removed from {Context.Guild.Name}.",
                                "View them all with hour.Display! :smiley:" +
                                (hours.Count < 2 ? "" : $" *We're now down to {hours.Count} hours*"));
                            try
                            {
                                //attempt capture of related classes.
                            }
                            catch (Exception ex)
                            {
                                await SendErrorEmbed("Failure attempting to delete some roles and channels.",
                                    "Exception during deletion of associated voice and text channels, and role.", ex);
                            }
                        }
                    }
                    return;

                case "Class.Display":
                    hourids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(hourfolder), 0);
                    if (hourids == null || !hourids.Any())
                        await SendClassicEmbed($"No hours added to {Context.Guild.Name}.", "Go ahead and add one with hour.Add!");
                    else
                    {
                        hours = DataStorage.GetObjectsInFolder<Hour>(hourfolder);
                        int count = 0;
                        embed.WithTitle("**hours in " + Context.Guild.Name + "**")
                            .WithDescription("Add with hour.Add, remove with hour.Remove! :smiley:")
                            .WithColor(new Color(60, 176, 222));
                        foreach (Hour h in hours)
                        {
                            count++;
                            embed.AddField($"{count} | ReferenceID: {h.id}", h.title);
                            if (IsDivisible(count, 24))
                            {
                                await Context.Channel.SendMessageAsync("", false, embed.Build());
                                embed = new EmbedBuilder();
                                embed.WithTitle("Some more hours in " + Context.Guild.Name + "...")
                                    .WithColor(new Color(60, 176, 222));
                            }
                        }
                        embed.WithFooter(count + " hours in this school.", "https://i.imgur.com/HAI5vMj.png");
                        await Context.Channel.SendMessageAsync("", false, embed.Build());
                    }
                    return;

            } // class functions.
            */
            
            List<ulong> hourids;
            string hourname;
            List<Hour> hours;
            Hour hour;
            switch (ClassName) // Teacher Functions
            {
                case "Hour.Add":
                    hourname = list[2].Replace("\"", "");
                    hourids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(hourfolder), 0);

                    if (hourids == null || !hourids.Any())
                    {
                        hour = new Hour
                        {
                            id = GenerateID(hourfolder),
                            title = hourname,
                        };
                        hourids = new List<ulong>
                        {
                            hour.id
                        };
                        DataStorage.StoreObject(hour, hourfolder + "/" + hour.id + ".json", false);
                    }
                    else
                    {
                        hours = DataStorage.GetObjectsInFolder<Hour>(hourfolder);
                        Hour hourtoremove = hours.Find(x => x.title == hourname);
                        if (hourtoremove.title == hourname)
                        {
                            await SendClassicEmbed($"Hour with the name \"{hourtoremove.title}\" is already in the list.",
                                "If you want to see the full list, use hour.Display! :smiley:");
                            return;
                        }
                        hour = new Hour
                        {
                            id = GenerateID(hourfolder),
                            title = hourname,
                        };
                        hours.Add(hour);
                        DataStorage.StoreObject(hour, hourfolder + "/" + hour.id + ".json", false);
                    }


                    hours = DataStorage.GetObjectsInFolder<Hour>(hourfolder);
                    embed = new EmbedBuilder();
                    embed.WithTitle($"Hour with the name \"{hour.title}\" added to {Context.Guild.Name}.")
                        .WithDescription("View them all with Hour.Display! :smiley:" +
                            (hours.Count < 2 ? "" : $" *We're now up to {hours.Count} hours*"))
                        .AddField("**Data**", $":notepad_spiral: ReferenceID: `{hour.id}`")
                        .WithColor(new Color(60, 176, 222))
                        .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png");
                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                    return;

                case "Hour.Remove":
                    hourname = list[2].Replace("\"", "");
                    hourids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(hourfolder), 0);
                    if (hourids == null || !hourids.Any())
                        await SendClassicEmbed($"No hours found on {Context.Guild.Name}.",
                            $"Use Hour.Add to add the first one! :smiley:");
                    else
                    {
                        hours = DataStorage.GetObjectsInFolder<Hour>(hourfolder);
                        Hour hourtoremove = hours.Find(x => x.title == hourname);
                        if (!(hourtoremove.title == hourname))
                            await SendClassicEmbed($"{hourname} not found on {Context.Guild.Name}.",
                                $"Try to use Hour.Display to see it's exact name! :smiley:");
                        else
                        {
                            hours.Remove(hourtoremove);
                            DataStorage.DeleteFile(hourfolder + "/" + hourtoremove.id + ".json");
                            await SendClassicEmbed($"hour with the name \"{hourtoremove.title}\" removed from {Context.Guild.Name}.",
                                "View them all with hour.Display! :smiley:" +
                                (hours.Count < 2 ? "" : $" *We're now down to {hours.Count} hours*"));
                            try
                            {
                                //attempt capture of related classes.
                            }
                            catch (Exception ex)
                            {
                                await SendErrorEmbed("Failure attempting to delete some roles and channels.",
                                    "Exception during deletion of associated voice and text channels, and role.", ex);
                            }
                        }
                    }
                    return;

                case "Hour.Display":
                    hourids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(hourfolder), 0);
                    if (hourids == null || !hourids.Any())
                        await SendClassicEmbed($"No hours added to {Context.Guild.Name}.", "Go ahead and add one with hour.Add!");
                    else
                    {
                        hours = DataStorage.GetObjectsInFolder<Hour>(hourfolder);
                        int count = 0;
                        embed.WithTitle("**hours in " + Context.Guild.Name + "**")
                            .WithDescription("Add with hour.Add, remove with hour.Remove! :smiley:")
                            .WithColor(new Color(60, 176, 222));
                        foreach (Hour h in hours)
                        {
                            count++;
                            embed.AddField($"{count} | ReferenceID: {h.id}", h.title);
                            if (IsDivisible(count, 24))
                            {
                                await Context.Channel.SendMessageAsync("", false, embed.Build());
                                embed = new EmbedBuilder();
                                embed.WithTitle("Some more hours in " + Context.Guild.Name + "...")
                                    .WithColor(new Color(60, 176, 222));
                            }
                        }
                        embed.WithFooter(count + " hours in this school.", "https://i.imgur.com/HAI5vMj.png");
                        await Context.Channel.SendMessageAsync("", false, embed.Build());
                    }
                    return;

            } // hour functions.

            List<ulong> teacherids;
            string teachername;
            List<Teacher> teachers;
            Teacher teacher;
            switch (ClassName) // Teacher Functions
            {
                case "Teacher.Add":
                    teachername = list[2].Replace("\"", "");
                    teacherids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(subjectfolder), 0);

                    if (teacherids == null || !teacherids.Any())
                    {
                        teacher = new Teacher
                        {
                            id = GenerateID(teacherfolder),
                            name = teachername,
                        };
                        teacherids = new List<ulong>
                        {
                            teacher.id
                        };
                        DataStorage.StoreObject(teacher, teacherfolder + "/" + teacher.id + ".json", false);
                    }
                    else
                    {
                        teachers = DataStorage.GetObjectsInFolder<Teacher>(teacherfolder);
                        Teacher teachertoremove = teachers.Find(x => x.name == teachername);
                        if (teachertoremove.name == teachername)
                        {
                            await SendClassicEmbed($"Teacher with the name \"{teachername}\" is already in the list.",
                                "If you want to see the full list, use teacher.Display! :smiley:");
                            return;
                        }
                        teacher = new Teacher
                        {
                            id = GenerateID(teacherfolder),
                            name = teachername,
                        };
                        teachers.Add(teacher);
                        DataStorage.StoreObject(teacher, teacherfolder + "/" + teacher.id + ".json", false);
                    }


                    teachers = DataStorage.GetObjectsInFolder<Teacher>(teacherfolder);
                    embed = new EmbedBuilder();
                    embed.WithTitle($"Teacher with the name \"{teacher.name}\" added to {Context.Guild.Name}.")
                        .WithDescription("View them all with Teacher.Display! :smiley:" +
                            (teachers.Count < 2 ? "" : $" *We're now up to {teachers.Count} teachers*"))
                        .AddField("**Data**", $":notepad_spiral: ReferenceID: `{teacher.id}`")
                        .WithColor(new Color(60, 176, 222))
                        .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png");
                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                    return;

                case "Teacher.Remove":
                    teachername = list[2].Replace("\"", "");
                    teacherids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(teacherfolder), 0);
                    if (teacherids == null || !teacherids.Any())
                        await SendClassicEmbed($"No teachers found on {Context.Guild.Name}.",
                            $"Use Teacher.Add to add the first one! :smiley:");
                    else
                    {
                        teachers = DataStorage.GetObjectsInFolder<Teacher>(teacherfolder);
                        Teacher teachertoremove = teachers.Find(x => x.name == teachername);
                        if (!(teachertoremove.name == teachername))
                            await SendClassicEmbed($"{teachername} not found on {Context.Guild.Name}.",
                                $"Try to use Teacher.Display to see it's exact name! :smiley:");
                        else
                        {
                            teachers.Remove(teachertoremove);
                            DataStorage.DeleteFile(teacherfolder + "/" + teachertoremove.id + ".json");
                            await SendClassicEmbed($"Teacher with the name \"{teachertoremove.name}\" removed from {Context.Guild.Name}.",
                                "View them all with Teacher.Display! :smiley:" +
                                (teachers.Count < 2 ? "" : $" *We're now down to {teachers.Count} teachers*"));
                            try
                            {
                                //attempt capture of related classes.
                            }
                            catch (Exception ex)
                            {
                                await SendErrorEmbed("Failure attempting to delete some roles and channels.",
                                    "Exception during deletion of associated voice and text channels, and role.", ex);
                            }
                        }
                    }
                    return;

                case "Teacher.Display":
                    teacherids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(teacherfolder), 0);
                    if (teacherids == null || !teacherids.Any())
                        await SendClassicEmbed($"No teachers added to {Context.Guild.Name}.", "Go ahead and add one with teacher.Add!");
                    else
                    {
                        teachers = DataStorage.GetObjectsInFolder<Teacher>(teacherfolder);
                        int count = 0;
                        embed.WithTitle("**teachers in " + Context.Guild.Name + "**")
                            .WithDescription("Add with teacher.Add, remove with teacher.Remove! :smiley:")
                            .WithColor(new Color(60, 176, 222));
                        foreach (Teacher t in teachers)
                        {
                            count++;
                            embed.AddField($"{count} | ReferenceID: {t.id}", t.name);
                            if (IsDivisible(count, 24))
                            {
                                await Context.Channel.SendMessageAsync("", false, embed.Build());
                                embed = new EmbedBuilder();
                                embed.WithTitle("Some more teachers in " + Context.Guild.Name + "...")
                                    .WithColor(new Color(60, 176, 222));
                            }
                        }
                        embed.WithFooter(count + " teachers in this school.", "https://i.imgur.com/HAI5vMj.png");
                        await Context.Channel.SendMessageAsync("", false, embed.Build());
                    }
                    return;

            } // Teacher functions.

            List<ulong> subjectids;
            string subjectname;
            List<Subject> subjects;
            Subject subject;
            switch (ClassName) // Subject functions
            {

                case "Subject.Add":
                    subjectname = list[2].Replace("\"", "");
                    subjectids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(subjectfolder), 0);


                    if (subjectids == null || !subjectids.Any())
                    {
                        subject = CreateSubject(subjectname, subjectfolder).Result;
                        subjectids = new List<ulong>
                        {
                            subject.id
                        };
                        DataStorage.StoreObject(subject, subjectfolder + "/" + subject.id + ".json", false);
                    }
                    else
                    {
                        subjects = DataStorage.GetObjectsInFolder<Subject>(subjectfolder);
                        Subject subjecttoremove = subjects.Find(x => x.name == subjectname);
                        if (subjecttoremove.name == subjectname)
                        {
                            await SendClassicEmbed($"Subject with the name \"{subjectname}\" is already in the list.",
                                "If you want to see the full list, use Subject.Display! :smiley:");
                            return;
                        }
                        subject = CreateSubject(subjectname, subjectfolder).Result;
                        subjects.Add(subject);
                        DataStorage.StoreObject(subject, subjectfolder + "/" + subject.id + ".json", false);
                    }


                    subjects = DataStorage.GetObjectsInFolder<Subject>(subjectfolder);
                    embed = new EmbedBuilder();
                    embed.WithTitle($"Subject with the name \"{subject.name}\" added to {Context.Guild.Name}.")
                        .WithDescription("View them all with Subject.Display! :smiley:" +
                            (subjects.Count < 2 ? "" : $" *We're now up to {subjects.Count} subjects*"))
                        .AddField("**Data**", $":notepad_spiral: ReferenceID: `{subject.id}`\n\n" +
                        $":mag_right: VoiceChannelID `{subject.voicechannelid}`\n" +
                        $":mag_right: TextChannelID `{subject.textchannelid}`\n" +
                        $":mag_right: RoleID `{subject.roleid}`")
                        .WithColor(new Color(60, 176, 222))
                        .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png");
                    await Context.Channel.SendMessageAsync("", false, embed.Build());
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
                        Subject subjecttoremove = subjects.Find(x => x.name == subjectname);
                        if (!(subjecttoremove.name == subjectname))
                            await SendClassicEmbed($"{subjectname} not found on {Context.Guild.Name}.",
                                $"Try to use Subject.Display to see it's exact name! :smiley:");
                        else
                        {
                            subjects.Remove(subjecttoremove);
                            DataStorage.DeleteFile(subjectfolder + "/" + subjecttoremove.id + ".json");
                            await SendClassicEmbed($"Subject with the name \"{subjecttoremove.name}\" removed from {Context.Guild.Name}.",
                                "View them all with Subject.Display! :smiley:" +
                                (subjects.Count < 2 ? "" : $" *We're now down to {subjects.Count} subjects*"));
                            try
                            {
                                await Context.Guild.GetChannel(subjecttoremove.textchannelid).DeleteAsync();
                                await Context.Guild.GetChannel(subjecttoremove.voicechannelid).DeleteAsync();
                                await Context.Guild.GetRole(subjecttoremove.roleid).DeleteAsync();

                                //attempt capture of related classes.

                            }
                            catch (Exception ex)
                            {
                                await SendErrorEmbed("Failure attempting to delete some roles and channels.",
                                    "Exception during deletion of associated voice and text channels, and role.", ex);
                            }
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
                                await Context.Channel.SendMessageAsync("", false, embed.Build());
                                embed = new EmbedBuilder();
                                embed.WithTitle("Some more subjects in " + Context.Guild.Name + "...")
                                    .WithColor(new Color(60, 176, 222));
                            }
                        }
                        embed.WithFooter(count + " subjects in this school.", "https://i.imgur.com/HAI5vMj.png");
                        await Context.Channel.SendMessageAsync("", false, embed.Build());
                    }
                    return;

            } // Subject functions.

            SocketGuildChannel channel = GetChannel(400434742790586378);
            subjects = DataStorage.GetObjectsInFolder<Subject>(subjectfolder);
            string[] Hours; string Teacher;
            try
            {
                Teacher = list[2].Replace("\"", "");

                string Hourslist = list[3].Replace("\"", "");
                if (Hourslist.Contains(",")) Hours = Hourslist.Split(","); else Hours = new string[1] { Hourslist };

                subjectname = list[4].Replace("\"", "");
                subject = subjects.Find(x => String.Compare(x.name, subjectname) == 0);
                if (subject.Equals(null) || subject.name == "")
                {
                    await SendClassicEmbed($"**Subject not found! with name {subject.name}**",
                        "Use Subject.Add to add the new subject! Or ensure it matches.");
                    return;
                }


                await Utilities.Log(MethodBase.GetCurrentMethod(), "2" + Teacher + "3" + subject, LogSeverity.Verbose);
            } catch (Exception ex)
            {
                await SendClassicEmbed("**Syntax problem!**", "Review how the class command is formatted.");
                await Utilities.Log(MethodBase.GetCurrentMethod(), "Exception expanding permaters on class command", ex, LogSeverity.Warning);
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
             * Cannot be two hours with same name.
             */

            await Utilities.Log(MethodBase.GetCurrentMethod(), "SubjectFile Created", LogSeverity.Verbose);
            await Context.Channel.SendMessageAsync($"{ClassName} // {Teacher} {Hours.ToString()} [{subject.name}]");
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


    }
    
    // Methods
    public sealed partial class LeaderRoleCommands : InteractiveBase
    {
        /*
        public async Task<GroupClass> FindCorrespondingClasses<T>(dynamic criterion)
        {
            
        }
        */
        /// <summary>
        /// Sends each property of given object in embed.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task SendClassEmbed<T>(string title, string desc, object obj)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle($"**{title}**")
                .WithDescription($"*{desc}*")
                .WithColor(new Color(60, 176, 222))
                .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png")
                .WithCurrentTimestamp();
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo property in properties)
                embed.AddField($"**{property.Name}**", $"*{property.GetValue(obj)}*", true);
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
        public async Task SendSuccessEmbed(string title, string desc)
        {
            var embed = new EmbedBuilder
            {
                Title = title,
                Description = desc,
                Color = new Color(33, 160, 52),
                Footer = new EmbedFooterBuilder()
                    .WithText(" -Alex https://discord.gg/DVSjvGa")
                    .WithIconUrl("https://i.imgur.com/HAI5vMj.png"),
                Timestamp = DateTime.UtcNow,
            }.Build();
            await Context.Channel.SendMessageAsync("", false, embed);
        }
        public async Task SendErrorEmbed(string title, string desc, Exception ex = null)
        {
            if (ex == null) ex = new Exception();
            var embed = new EmbedBuilder
            {
                Title = title,
                Description = desc,
                Color = new Color(219, 57, 75),
                Footer = new EmbedFooterBuilder()
                    .WithText(" -Alex https://discord.gg/DVSjvGa")
                    .WithIconUrl("https://i.imgur.com/HAI5vMj.png"),
                Timestamp = DateTime.UtcNow,
                Fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder()
                        .WithName("`Error content:` ")
                        .WithValue($"*{ex.ToString()}*")
                        .WithIsInline(true)
                    // Repeat with comma for another field here.
                }
            }.Build();
            await Context.Channel.SendMessageAsync("", false, embed);
        }
        public async Task SendClassicEmbed(string title, string desc)
        {
            var embed = new EmbedBuilder
            {
                Title = title,
                Description = desc,
                Color = new Color(60, 176, 222),
                Footer = new EmbedFooterBuilder()
                    .WithText(" -Alex https://discord.gg/DVSjvGa")
                    .WithIconUrl("https://i.imgur.com/HAI5vMj.png"),
                Timestamp = DateTime.UtcNow,
            }.Build();
            await Context.Channel.SendMessageAsync("", false, embed);
        }
        public async Task<Subject> CreateSubject(string subjectname, string subjectfolder)
        {
            Subject subject = new Subject();
            try
            {
                RestVoiceChannel subjectVC = Context.Guild.CreateVoiceChannelAsync(subjectname).Result;
                RestTextChannel subjectTC = Context.Guild.CreateTextChannelAsync(subjectname).Result;
                GuildConfig guildconfig = GuildsData.FindOrCreateGuildConfig(Context.Guild);
                var restrole = await Context.Guild.CreateRoleAsync(subjectname);
                subject = new Subject
                {
                    id = GenerateID(subjectfolder),
                    name = subjectname,
                    voicechannelid = subjectVC.Id,
                    textchannelid = subjectTC.Id,
                    roleid = restrole.Id, // Converting to socketroleid
                };
                // Modify role settings.
                await restrole.ModifyAsync(r =>
                {
                    r.Color = new Color(119, 118, 158);
                    r.Hoist = false;
                    r.Mentionable = true;
                    r.Permissions = GuildPermissions.None;
                    //r.Position = Context.Guild.GetRole(guildconfig.VisitorRoleID).Position - 1;
                });
                // Add voice channel perms.
                await subjectVC.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, EveryonePermissions());
                await subjectVC.AddPermissionOverwriteAsync(restrole, ClassPermissions());
                // Add text channel perms.
                await subjectTC.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, EveryonePermissions());
                await subjectTC.AddPermissionOverwriteAsync(restrole, ClassPermissions());
                                
                return subject;
            }
            catch (Exception ex)
            {
                await SendErrorEmbed("Error generating subject.", $"*Please check syntax or contact server owner.*", ex);
                await Utilities.Log(MethodBase.GetCurrentMethod(), "Error generating subject.", ex, LogSeverity.Error);
                return subject;
            }
        }
        public OverwritePermissions ClassPermissions() => new OverwritePermissions(PermValue.Inherit,
                PermValue.Inherit, PermValue.Allow, PermValue.Allow, PermValue.Allow,
                PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Allow,
                PermValue.Allow, PermValue.Inherit, PermValue.Allow, PermValue.Allow,
                PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit,
                PermValue.Allow, PermValue.Inherit, PermValue.Inherit);
        public OverwritePermissions EveryonePermissions() => new OverwritePermissions(PermValue.Deny,
                PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny,
                PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny,
                PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny,
                PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny,
                PermValue.Deny, PermValue.Deny, PermValue.Deny);
        
        public async Task<GroupClass> CreateGroupClass(string hourFolder, string teacherFolder, string classFolder, string subjectFolder = "")
        {
            // find class name!
            await ReplyAsync("**Alright, before anything** a group must have a name! What is the name of this class?");
            var classname = await AwaitMessage(100);
            if (classname.Equals(null)) return null;
            // find hour.
            await ReplyAsync("Firstly, specify the **hours this class is in**. Format it like this: \n" +
                "```hourname,hourname,hourname```\nOr simply\n```hourname```");
            var hoursmsg = await AwaitMessage(100, "Try `h!group Class Hour.Add`");
            if (hoursmsg.Equals(null)) return null;
            List<Hour> hours = DataStorage.GetObjectsInFolder<Hour>(hourFolder); List<Hour> shours;
            if (hoursmsg.Content.Contains(",")) shours = hours.FindAll(x => hoursmsg.Content.Split(",").Contains(x.title));
                else shours = hours.FindAll(h => h.title == hoursmsg.Content);
            if (shours.Equals(null) || shours.Count == 0)
            {
                await ReplyAsync("No group found, remember to be exact! Try `h!group Class Hour.Add`");
                return null;
            }
            await ReplyAsync(shours.Count + ((shours.Count > 1) ? " hours" : " hour") + " found!");
            // find teacher.
            await ReplyAsync("Great! Next up, the teacher of this class. Just say the name of the teacher as it is.");
            var teachermsg = await AwaitMessage(100, "Try `h!group Class Teacher.Add`");
            if (teachermsg.Equals(null)) return null;
            List<Teacher> teachers = DataStorage.GetObjectsInFolder<Teacher>(hourFolder); Teacher teacher;
            teacher = teachers.Find(t => t.name == hoursmsg.Content);
            if (teacher.Equals(null))
            {
                await ReplyAsync($"No teacher found with name **{hoursmsg.Content}**, remember to be exact!" +
                    $" Try `h!group Class Hour.Add`");
                return null;
            }
            await ReplyAsync($"Teacher found with name {teacher.name}.");
            // subject?
            await ReplyAsync("Perfect! Is there a subject in this school that can be applied to this class? If so," +
                " say the name of the subject. If not, simply say \"no\"");
            var subjectmsg = await AwaitMessage(100, "Try `h!group Class Subject.Add`");
            if (subjectmsg.Equals(null)) return null;
            Subject subject; SocketTextChannel channel;
            if (subjectmsg.Content != "no")
            {
                var subjects = DataStorage.GetObjectsInFolder<Subject>(subjectFolder);
                subject = subjects.Find(s => s.name == subjectmsg.Content);
                if (subject.Equals(null))
                {
                    await ReplyAsync($"No subject found with name **{subjectmsg.Content}**, remember to be exact!" +
                        $" Try `h!group Class Subject.Add`");
                    return null;
                }
                await ReplyAsync($"Subject found with name {subject.name}.");
            }
            //
            await ReplyAsync("**All set!** I'll automatically generate the channels and roles for this class, " +
                "and get back to you once I'm done...");
            // generate roles...
            List<RestRole> roles = new List<RestRole>();
            foreach (Hour h in shours)
            {
                RestRole role = await Context.Guild.CreateRoleAsync($"{h.title}: {teacher.name}, {classname}");
                await role.ModifyAsync(r =>
                {
                    r.Color = new Color(89, 88, 133);
                    r.Hoist = false;
                    r.Mentionable = true;
                    r.Permissions = GuildPermissions.None;
                    //r.Position = Context.Guild.GetRole(guildconfig.VisitorRoleID).Position - 1;
                });
                roles.Add(role);
            }

            if (subjectmsg.Content == "no")
            {
                await ReplyAsync("Would you like to create a channel for this class? \"yes\" or anything else.");
                var qchannel = await AwaitMessage(100, "Well this is an awkward step to fail at.");
                if (qchannel.Equals(null)) return null;
                List<ulong> roleids = new List<ulong>();
                if (qchannel.Content == "yes")
                {
                    var newchannel = Context.Guild.CreateTextChannelAsync($"{teacher.name}, {classname}").Result;
                    await newchannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, EveryonePermissions());
                    foreach (RestRole r in roles) await newchannel.AddPermissionOverwriteAsync(r, ClassPermissions());
                    // Here class is complete. With new channel.
                    foreach (RestRole r in roles) roleids.Add(r.Id);
                    var finalclass = new GroupClass(hours.ToArray(), teacher, roleids.ToArray(), null, newchannel.Id);
                    return finalclass;
                }
                // Here class is complete. Without new channel.
                foreach (RestRole r in roles) roleids.Add(r.Id);
                var finalclass2 = new GroupClass(hours.ToArray(), teacher, roleids.ToArray(), null, null);
                return finalclass2;
            }
            else
            {
                var subjects = DataStorage.GetObjectsInFolder<Subject>(subjectFolder);
                subject = subjects.Find(s => s.name == subjectmsg.Content); // Have to relocate subject... inefficient I know.
                channel = Context.Guild.GetTextChannel(subject.textchannelid);
                foreach (RestRole r in roles) await channel.AddPermissionOverwriteAsync(r, ClassPermissions());
                // Here class is complete. With subject.
                List<ulong> roleids = new List<ulong>();
                foreach (RestRole r in roles) roleids.Add(r.Id);
                var finalclass3 = new GroupClass(hours.ToArray(), teacher, roleids.ToArray(), subject, null);
                return finalclass3;
            }
        }
        public async Task<SocketMessage> AwaitMessage(int wait, string comment = "")
        {
            var msg = await NextMessageAsync(true, true, new TimeSpan(10000000 * wait));
            if (msg == null)
            {
                await ReplyAsync($"**You did not reply in time {Context.User.Mention}!** Waited {wait} seconds. {comment}");
                return null;
            }
            return msg;
        }
        /// <summary>
        /// Gemerates unique ID ulong.
        /// </summary>
        /// <param name="inUse"></param>
        /// <returns></returns>g
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
        public SocketGuildChannel GetChannel(ulong id) => Context.Guild.Channels.FirstOrDefault(r => r.Id == id);
        public static bool IsDivisible(int x, int n)
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
        public bool UserIsGroupLeader(GuildConfig guildconfig)
        {
            try
            {
                if (UserHasRole(guildconfig.ChannelManagerRoleID)
                    || UserHasRole(guildconfig.DirectorRoleID)
                    || UserHasRole(guildconfig.GroupManagerRoleID)
                    || UserHasRole(guildconfig.VoiceManagerRoleID)) return true;
            }
            catch (Exception ex)
            {
                Utilities.Log(MethodBase.GetCurrentMethod(), "Failure checking roles.", ex, LogSeverity.Warning);
            }
            //Utilities.Log(MethodBase.GetCurrentMethod(), "F",LogSeverity.Warning);
            return false;
        }
        public bool UserHasRole(ulong roleId)
        {
            var user = (SocketGuildUser)Context.User;
            foreach (SocketRole role in user.Roles)
            {
                // Utilities.Log(MethodBase.GetCurrentMethod(), role.Id + " -- " + roleId);
                if (role.Id == roleId) return true;
            }
            return false;
        }
        private List<SocketRole> RolesFromName(string targetRoleName)
        {
            var result = from r in Context.Guild.Roles
                         where r.Name == targetRoleName
                         select r;

            return result.ToList();
        }
        public SocketRole RoleFromID(ulong roleId) => Context.Guild.Roles.FirstOrDefault(r => r.Id == roleId);
    }
}