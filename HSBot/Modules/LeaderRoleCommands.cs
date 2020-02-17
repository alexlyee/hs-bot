//using Discord;
//using Discord.Commands;
//using Discord.Rest;
//using Discord.WebSocket;
//using Discord.Addons.Interactive;
//using HSBot.Entities;
//using HSBot.Helpers;
//using HSBot.Modules.References;
//using HSBot.Persistent;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Threading.Tasks;


//namespace HSBot.Modules
//{
//    public sealed partial class LeaderRoleCommands : InteractiveBase
//    {
//        /*
//        [Command("t")]
//        public async Task TestCommand()
//        {
//            var g = await CreateGroupClass();
//            await SendClassicEmbed("Class test info..", 
//                g.name + '|' + g.id + '|' + g.hours + '|' + g.subject + '|' + g.teacher);
//        }
//        */
//        [RequireContext(ContextType.Guild)]
//        [Command("group", RunMode = RunMode.Async)]
//        public async Task Group([Remainder]string message)
//        {
//            string groupfile = GuildsData.GuildsFolder + "/" + Context.Guild.Id + "/Groups";
//            if (!DataStorage.LocalFolderExists(groupfile, true))
//                await Utilities.Log(MethodBase.GetCurrentMethod(), "SubjectFile Created", LogSeverity.Verbose);
//            GuildConfig guildconfig = GuildsData.FindOrCreateGuildConfig(Context.Guild);
//            SocketGuildUser user = (SocketGuildUser)Context.User;
//            if (!(UserIsGroupLeader(guildconfig)))
//            {
//                try
//                {
//                    await SendClassicEmbed("You don't have permissions to use this!", $"Contact a member with one of these roles: "
//                        + $"\n - **{RoleFromID(guildconfig.ChannelManagerRoleID).Name}** "
//                        + $"\n - **{RoleFromID(guildconfig.DirectorRoleID).Name}** "
//                        + $"\n - **{RoleFromID(guildconfig.GroupManagerRoleID).Name}** "
//                        + $"\n - **{RoleFromID(guildconfig.VoiceManagerRoleID).Name}** ");
//                    return;
//                }
//                catch (Exception ex)
//                {
//                    await SendClassicEmbed("You don't have permissions to use this!", "Contact server owner, RoleIDs are not properly setup.");
//                    await Utilities.Log(MethodBase.GetCurrentMethod(), "Failure with RoleIDs", ex, LogSeverity.Warning);
//                    return;
//                }
//            }

//            var firstWord = message.IndexOf(" ") > -1
//                  ? message.Substring(0, message.IndexOf(" "))
//                  : message;

//            switch (firstWord.ToLower())
//            {
//                case "overseen":
//                    break;
//                case "inherent":
//                    break;
//                case "idiomatic":
//                    break;
//                case "functional":
//                    break;
//                case "class":
//                    if (firstWord == message)
//                    {
//                        await SendClassicEmbed("Syntax problem", "It seems no perameters were provided, check how to use the specified group type! :smiley:");
//                        return;
//                    }
//                    string scan = message.Substring(message.IndexOf(" "));
//                    string[] arguments = ParseArguments(scan);
//                    MethodInfo Command = this.GetType().GetMethod($"Group_{firstWord}");
//                    try
//                    {
//                        Command.Invoke(this, new object[] { Context, arguments });
//                    }
//                    catch (Exception ex)
//                    {
//                        await Utilities.Log(MethodBase.GetCurrentMethod(), "Error invoking command.", ex, LogSeverity.Error);
//                    }
//                    break;
//                default:
//                    await SendClassicEmbed("Syntax problem", "No group type found! Check the syntax for this command! :smiley:");
//                    break;
//            } // Foreward to corresponding Task.
//            return;
//        }


//        public async Task Group_Class(SocketCommandContext context, params string[] list)
//        {
//            var embed = new EmbedBuilder();
//            string classfolder = GuildsData.GuildsFolder + "/" + Context.Guild.Id + "/GroupClasses";
//            string subjectfolder = GuildsData.GuildsFolder + "/" + Context.Guild.Id + "/ClassSubjects";
//            string teacherfolder = GuildsData.GuildsFolder + "/" + Context.Guild.Id + "/ClassTeachers";
//            string hourfolder = GuildsData.GuildsFolder + "/" + Context.Guild.Id + "/ClassHours";
//            if (!DataStorage.LocalFolderExists(subjectfolder, true))
//                await Utilities.Log(MethodBase.GetCurrentMethod(), "SubjectFolder Created for " + context.Guild.Name, LogSeverity.Verbose);
//            if (!DataStorage.LocalFolderExists(teacherfolder, true))
//                await Utilities.Log(MethodBase.GetCurrentMethod(), "TeacherFolder Created for " + context.Guild.Name, LogSeverity.Verbose);
//            if (!DataStorage.LocalFolderExists(hourfolder, true))
//                await Utilities.Log(MethodBase.GetCurrentMethod(), "HourFolder Created for " + context.Guild.Name, LogSeverity.Verbose);
//            if (!DataStorage.LocalFolderExists(classfolder, true))
//                await Utilities.Log(MethodBase.GetCurrentMethod(), "ClassFolder Created for " + context.Guild.Name, LogSeverity.Verbose);
//            var ClassName = list[1].Replace("\"", "");
            

//            List<ulong> classids;
//            string classname;
//            List<GroupClass> Classes;
//            References.GroupClass Class;
            
//            switch (ClassName) // Class Functions
//            {
//                case "New":
//                    classname = list[2].Replace("\"", "");
//                    classids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(classfolder), 0);
//                    if (classids == null || !classids.Any())
//                    {
//                        Class = await CreateGroupClass(classname, hourfolder, teacherfolder, classfolder, subjectfolder);

//                        DataStorage.StoreObject(Class, classfolder + "/" + Class.id + ".json", true);
//                    }
//                    else
//                    {
//                        Classes = DataStorage.GetObjectsInFolder<GroupClass>(classfolder);
//                        GroupClass classtoremove = Classes.Find(x => x.name == classname);
//                        if (classtoremove != null && classtoremove.name == classname)
//                        {
//                            await SendClassicEmbed($"class with the name \"{classtoremove.name}\" is already in the list.",
//                                "If you want to see the full list, use class.Display! :smiley:");
//                            return;
//                        }
//                        Class = await CreateGroupClass(classname, hourfolder, teacherfolder, classfolder, subjectfolder);

//                        DataStorage.StoreObject(Class, classfolder + "/" + Class.id + ".json", true);
//                    }
//                    return;
//                case "Remove":
//                    classname = list[2].Replace("\"", "");
//                    classids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(classfolder), 0);
//                    if (classids == null || !classids.Any())
//                    {
//                        await SendErrorEmbed("**Hey Bud!**", "There are no classes to remove. Busted.");
//                        return;
//                    }
//                    else
//                    {
//                        Classes = DataStorage.GetObjectsInFolder<GroupClass>(classfolder);
//                        GroupClass classtoremove = Classes.Find(x => x.name == classname);
//                        if (classtoremove != null && classtoremove.name == classname)
//                        {
//                            RemoveGroupClass(classtoremove, hourfolder);
//                            await DeleteGroupClass(classtoremove, true);
//                            await SendClassicEmbed($"class with the name \"{classtoremove.name}\" deleted.",
//                                "If you want to see the full list, use All! :smiley:");
//                            return;
//                        }
//                        else await SendErrorEmbed("**Hey buddy!**", $"Failed to find a class with the name {classname}.");
//                    }
//                    return;
//                case "Edit":
//                    classname = list[2].Replace("\"", "");
//                    classids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(classfolder), 0);
//                    if (classids == null || !classids.Any())
//                    {
//                        await SendErrorEmbed("**Hey Bud!**", "There are no classes to remove. Busted.");
//                        return;
//                    }
//                    else
//                    {
//                        Classes = DataStorage.GetObjectsInFolder<GroupClass>(classfolder);
//                        GroupClass classtoedit = Classes.Find(x => x.name == classname);
//                        if (classtoedit != null && classtoedit.name == classname)
//                        {
//                            await EditGroupClass(classtoedit, classfolder, hourfolder, teacherfolder, subjectfolder);
//                            await SendClassicEmbed($"class with the name \"{classtoedit.name}\" deleted.",
//                                "If you want to see the full list, use All! :smiley:");
//                            return;
//                        }
//                        else await SendErrorEmbed("**Hey buddy!**", $"Failed to find a class with the name {classname}.");
//                    }
//                    return;
//                    /*
//                case "All":
//                    hourids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(hourfolder), 0);
//                    if (hourids == null || !hourids.Any())
//                        await SendClassicEmbed($"No hours added to {Context.Guild.Name}.", "Go ahead and add one with hour.Add!");
//                    else
//                    {
//                        hours = DataStorage.GetObjectsInFolder<Hour>(hourfolder);
//                        int count = 0;
//                        embed.WithTitle("**hours in " + Context.Guild.Name + "**")
//                            .WithDescription("Add with hour.Add, remove with hour.Remove! :smiley:")
//                            .WithColor(new Color(60, 176, 222));
//                        foreach (Hour h in hours)
//                        {
//                            count++;
//                            embed.AddField($"{count} | ReferenceID: {h.id}", h.title);
//                            if (IsDivisible(count, 24))
//                            {
//                                await Context.Channel.SendMessageAsync("", false, embed.Build());
//                                embed = new EmbedBuilder();
//                                embed.WithTitle("Some more hours in " + Context.Guild.Name + "...")
//                                    .WithColor(new Color(60, 176, 222));
//                            }
//                        }
//                        embed.WithFooter(count + " hours in this school.", "https://i.imgur.com/HAI5vMj.png");
//                        await Context.Channel.SendMessageAsync("", false, embed.Build());
//                    }
//                    return;
//                    */

//            } // class functions.
            
            
//            List<ulong> hourids;
//            string hourname;
//            List<Hour> hours;
//            Hour hour;
//            switch (ClassName) // Teacher Functions
//            {
//                case "Hour.Add":
//                    hourname = list[2].Replace("\"", "");
//                    hourids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(hourfolder), 0);

//                    if (hourids == null || !hourids.Any())
//                    {
//                        hour = new Hour
//                        {
//                            id = GenerateID(hourfolder),
//                            title = hourname,
//                        };
//                        hourids = new List<ulong>
//                        {
//                            hour.id
//                        };
//                        DataStorage.StoreObject(hour, hourfolder + "/" + hour.id + ".json", false);
//                    }
//                    else
//                    {
//                        hours = DataStorage.GetObjectsInFolder<Hour>(hourfolder);
//                        Hour hourtoremove = hours.Find(x => x.title == hourname);
//                        if (hourtoremove.title == hourname)
//                        {
//                            await SendClassicEmbed($"Hour with the name \"{hourtoremove.title}\" is already in the list.",
//                                "If you want to see the full list, use hour.Display! :smiley:");
//                            return;
//                        }
//                        hour = new Hour
//                        {
//                            id = GenerateID(hourfolder),
//                            title = hourname,
//                        };
//                        hours.Add(hour);
//                        DataStorage.StoreObject(hour, hourfolder + "/" + hour.id + ".json", false);
//                    }


//                    hours = DataStorage.GetObjectsInFolder<Hour>(hourfolder);
//                    embed = new EmbedBuilder();
//                    embed.WithTitle($"Hour with the name \"{hour.title}\" added to {Context.Guild.Name}.")
//                        .WithDescription("View them all with Hour.Display! :smiley:" +
//                            (hours.Count < 2 ? "" : $" *We're now up to {hours.Count} hours*"))
//                        .AddField("**Data**", $":notepad_spiral: ReferenceID: `{hour.id}`")
//                        .WithColor(new Color(60, 176, 222))
//                        .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png");
//                    await Context.Channel.SendMessageAsync("", false, embed.Build());
//                    return;

//                case "Hour.Remove":
//                    hourname = list[2].Replace("\"", "");
//                    hourids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(hourfolder), 0);
//                    if (hourids == null || !hourids.Any())
//                        await SendClassicEmbed($"No hours found on {Context.Guild.Name}.",
//                            $"Use Hour.Add to add the first one! :smiley:");
//                    else
//                    {
//                        hours = DataStorage.GetObjectsInFolder<Hour>(hourfolder);
//                        Hour hourtoremove = hours.Find(x => x.title == hourname);
//                        if (!(hourtoremove.title == hourname))
//                            await SendClassicEmbed($"{hourname} not found on {Context.Guild.Name}.",
//                                $"Try to use Hour.Display to see it's exact name! :smiley:");
//                        else
//                        {
//                            hours.Remove(hourtoremove);
//                            DataStorage.DeleteFile(hourfolder + "/" + hourtoremove.id + ".json");
//                            await SendClassicEmbed($"hour with the name \"{hourtoremove.title}\" removed from {Context.Guild.Name}.",
//                                "View them all with hour.Display! :smiley:" +
//                                (hours.Count < 2 ? "" : $" *We're now down to {hours.Count} hours*"));
//                            try
//                            {
//                                //attempt capture of related classes.
//                            }
//                            catch (Exception ex)
//                            {
//                                await SendErrorEmbed("Failure attempting to delete some roles and channels.",
//                                    "Exception during deletion of associated voice and text channels, and role.", ex);
//                            }
//                        }
//                    }
//                    return;

//                case "Hours":
//                    hourids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(hourfolder), 0);
//                    if (hourids == null || !hourids.Any())
//                        await SendClassicEmbed($"No hours added to {Context.Guild.Name}.", "Go ahead and add one with hour.Add!");
//                    else
//                    {
//                        hours = DataStorage.GetObjectsInFolder<Hour>(hourfolder);
//                        int count = 0;
//                        embed.WithTitle("**hours in " + Context.Guild.Name + "**")
//                            .WithDescription("Add with hour.Add, remove with hour.Remove! :smiley:")
//                            .WithColor(new Color(60, 176, 222));
//                        foreach (Hour h in hours)
//                        {
//                            count++;
//                            embed.AddField($"{count} | ReferenceID: {h.id}", h.title);
//                            if (IsDivisible(count, 24))
//                            {
//                                await Context.Channel.SendMessageAsync("", false, embed.Build());
//                                embed = new EmbedBuilder();
//                                embed.WithTitle("Some more hours in " + Context.Guild.Name + "...")
//                                    .WithColor(new Color(60, 176, 222));
//                            }
//                        }
//                        embed.WithFooter(count + " hours in this school.", "https://i.imgur.com/HAI5vMj.png");
//                        await Context.Channel.SendMessageAsync("", false, embed.Build());
//                    }
//                    return;

//            } // hour functions.

//            List<ulong> teacherids;
//            string teachername;
//            List<Teacher> teachers;
//            Teacher teacher;
//            switch (ClassName) // Teacher Functions
//            {
//                case "Teacher.Add":
//                    teachername = list[2].Replace("\"", "");
//                    teacherids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(subjectfolder), 0);

//                    if (teacherids == null || !teacherids.Any())
//                    {
//                        teacher = new Teacher
//                        {
//                            id = GenerateID(teacherfolder),
//                            name = teachername,
//                        };
//                        teacherids = new List<ulong>
//                        {
//                            teacher.id
//                        };
//                        DataStorage.StoreObject(teacher, teacherfolder + "/" + teacher.id + ".json", false);
//                    }
//                    else
//                    {
//                        teachers = DataStorage.GetObjectsInFolder<Teacher>(teacherfolder);
//                        Teacher teachertoremove = teachers.Find(x => x.name == teachername);
//                        if (teachertoremove.name == teachername)
//                        {
//                            await SendClassicEmbed($"Teacher with the name \"{teachername}\" is already in the list.",
//                                "If you want to see the full list, use teacher.Display! :smiley:");
//                            return;
//                        }
//                        teacher = new Teacher
//                        {
//                            id = GenerateID(teacherfolder),
//                            name = teachername,
//                        };
//                        teachers.Add(teacher);
//                        DataStorage.StoreObject(teacher, teacherfolder + "/" + teacher.id + ".json", false);
//                    }


//                    teachers = DataStorage.GetObjectsInFolder<Teacher>(teacherfolder);
//                    embed = new EmbedBuilder();
//                    embed.WithTitle($"Teacher with the name \"{teacher.name}\" added to {Context.Guild.Name}.")
//                        .WithDescription("View them all with Teacher.Display! :smiley:" +
//                            (teachers.Count < 2 ? "" : $" *We're now up to {teachers.Count} teachers*"))
//                        .AddField("**Data**", $":notepad_spiral: ReferenceID: `{teacher.id}`")
//                        .WithColor(new Color(60, 176, 222))
//                        .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png");
//                    await Context.Channel.SendMessageAsync("", false, embed.Build());
//                    return;

//                case "Teacher.Remove":
//                    teachername = list[2].Replace("\"", "");
//                    teacherids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(teacherfolder), 0);
//                    if (teacherids == null || !teacherids.Any())
//                        await SendClassicEmbed($"No teachers found on {Context.Guild.Name}.",
//                            $"Use Teacher.Add to add the first one! :smiley:");
//                    else
//                    {
//                        teachers = DataStorage.GetObjectsInFolder<Teacher>(teacherfolder);
//                        Teacher teachertoremove = teachers.Find(x => x.name == teachername);
//                        if (!(teachertoremove.name == teachername))
//                            await SendClassicEmbed($"{teachername} not found on {Context.Guild.Name}.",
//                                $"Try to use Teacher.Display to see it's exact name! :smiley:");
//                        else
//                        {
//                            teachers.Remove(teachertoremove);
//                            DataStorage.DeleteFile(teacherfolder + "/" + teachertoremove.id + ".json");
//                            await SendClassicEmbed($"Teacher with the name \"{teachertoremove.name}\" removed from {Context.Guild.Name}.",
//                                "View them all with Teacher.Display! :smiley:" +
//                                (teachers.Count < 2 ? "" : $" *We're now down to {teachers.Count} teachers*"));
//                            try
//                            {
//                                //attempt capture of related classes.
//                            }
//                            catch (Exception ex)
//                            {
//                                await SendErrorEmbed("Failure attempting to delete some roles and channels.",
//                                    "Exception during deletion of associated voice and text channels, and role.", ex);
//                            }
//                        }
//                    }
//                    return;

//                case "Teachers":
//                    teacherids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(teacherfolder), 0);
//                    if (teacherids == null || !teacherids.Any())
//                        await SendClassicEmbed($"No teachers added to {Context.Guild.Name}.", "Go ahead and add one with teacher.Add!");
//                    else
//                    {
//                        teachers = DataStorage.GetObjectsInFolder<Teacher>(teacherfolder);
//                        int count = 0;
//                        embed.WithTitle("**Teachers in " + Context.Guild.Name + "**")
//                            .WithDescription("Add with teacher.Add, remove with teacher.Remove! :smiley:")
//                            .WithColor(new Color(60, 176, 222));
//                        foreach (Teacher t in teachers)
//                        {
//                            count++;
//                            embed.AddField($"{count} | ReferenceID: {t.id}", t.name);
//                            if (IsDivisible(count, 24))
//                            {
//                                await Context.Channel.SendMessageAsync("", false, embed.Build());
//                                embed = new EmbedBuilder();
//                                embed.WithTitle("Some more teachers in " + Context.Guild.Name + "...")
//                                    .WithColor(new Color(60, 176, 222));
//                            }
//                        }
//                        embed.WithFooter(count + " teachers in this school.", "https://i.imgur.com/HAI5vMj.png");
//                        await Context.Channel.SendMessageAsync("", false, embed.Build());
//                    }
//                    return;

//            } // Teacher functions.

//            List<ulong> subjectids;
//            string subjectname;
//            List<Subject> subjects;
//            Subject subject;
//            switch (ClassName) // Subject functions
//            {

//                case "Subject.Add":
//                    subjectname = list[2].Replace("\"", "");
//                    subjectids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(subjectfolder), 0);


//                    if (subjectids == null || !subjectids.Any())
//                    {
//                        subject = CreateSubject(subjectname, subjectfolder).Result;
//                        subjectids = new List<ulong>
//                        {
//                            subject.id
//                        };
//                        DataStorage.StoreObject(subject, subjectfolder + "/" + subject.id + ".json", false);
//                    }
//                    else
//                    {
//                        subjects = DataStorage.GetObjectsInFolder<Subject>(subjectfolder);
//                        Subject subjecttoremove = subjects.Find(x => x.name == subjectname);
//                        if (subjecttoremove.name == subjectname)
//                        {
//                            await SendClassicEmbed($"Subject with the name \"{subjectname}\" is already in the list.",
//                                "If you want to see the full list, use Subject.Display! :smiley:");
//                            return;
//                        }
//                        subject = CreateSubject(subjectname, subjectfolder).Result;
//                        subjects.Add(subject);
//                        DataStorage.StoreObject(subject, subjectfolder + "/" + subject.id + ".json", false);
//                    }


//                    subjects = DataStorage.GetObjectsInFolder<Subject>(subjectfolder);
//                    embed = new EmbedBuilder();
//                    embed.WithTitle($"Subject with the name \"{subject.name}\" added to {Context.Guild.Name}.")
//                        .WithDescription("View them all with Subject.Display! :smiley:" +
//                            (subjects.Count < 2 ? "" : $" *We're now up to {subjects.Count} subjects*"))
//                        .AddField("**Data**", $":notepad_spiral: ReferenceID: `{subject.id}`\n\n" +
//                        $":mag_right: VoiceChannelID `{subject.voicechannelid}`\n" +
//                        $":mag_right: TextChannelID `{subject.textchannelid}`\n" +
//                        $":mag_right: RoleID `{subject.roleid}`")
//                        .WithColor(new Color(60, 176, 222))
//                        .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png");
//                    await Context.Channel.SendMessageAsync("", false, embed.Build());
//                    return;

//                case "Subject.Edit":
//                    return;

//                case "Subject.Remove":
//                    subjectname = list[2].Replace("\"", "");
//                    subjectids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(subjectfolder), 0);
//                    if (subjectids == null || !subjectids.Any())
//                        await SendClassicEmbed($"No subjects found on {Context.Guild.Name}.",
//                            $"Use Subject.Add to add the first one! :smiley:");
//                    else
//                    {
//                        subjects = DataStorage.GetObjectsInFolder<Subject>(subjectfolder);
//                        Subject subjecttoremove = subjects.Find(x => x.name == subjectname);
//                        if (!(subjecttoremove.name == subjectname))
//                            await SendClassicEmbed($"{subjectname} not found on {Context.Guild.Name}.",
//                                $"Try to use Subject.Display to see it's exact name! :smiley:");
//                        else
//                        {
//                            subjects.Remove(subjecttoremove);
//                            DataStorage.DeleteFile(subjectfolder + "/" + subjecttoremove.id + ".json");
//                            await SendClassicEmbed($"Subject with the name \"{subjecttoremove.name}\" removed from {Context.Guild.Name}.",
//                                "View them all with Subject.Display! :smiley:" +
//                                (subjects.Count < 2 ? "" : $" *We're now down to {subjects.Count} subjects*"));
//                            try
//                            {
//                                await Context.Guild.GetChannel(subjecttoremove.textchannelid).DeleteAsync();
//                                if (subjecttoremove.voicechannelid.HasValue)
//                                    await Context.Guild.GetChannel(subjecttoremove.voicechannelid.Value).DeleteAsync();
//                                await Context.Guild.GetRole(subjecttoremove.roleid).DeleteAsync();

//                                //attempt capture of related classes.

//                            }
//                            catch (Exception ex)
//                            {
//                                await SendErrorEmbed("Failure attempting to delete some roles and channels.",
//                                    "Exception during deletion of associated voice and text channels, and role.", ex);
//                            }
//                        }
//                    }
//                    return;

//                case "Subjects":
//                    subjectids = SafeConversion<ulong>(DataStorage.GetFilesInFolder(subjectfolder), 0);
//                    if (subjectids == null || !subjectids.Any())
//                        await SendClassicEmbed($"No subjects added to {Context.Guild.Name}.", "Go ahead and add one with Subject.Add!");
//                    else
//                    {
//                        subjects = DataStorage.GetObjectsInFolder<Subject>(subjectfolder);
//                        int count = 0;
//                        embed.WithTitle("**Subjects in " + Context.Guild.Name + "**")
//                            .WithDescription("Add with Subject.Add, remove with Subject.Remove! :smiley:")
//                            .WithColor(new Color(60, 176, 222));
//                        foreach (Subject s in subjects)
//                        {
//                            count++;
//                            embed.AddField($"{count} | ReferenceID: {s.id}", s.name);
//                            if (IsDivisible(count, 24))
//                            {
//                                await Context.Channel.SendMessageAsync("", false, embed.Build());
//                                embed = new EmbedBuilder();
//                                embed.WithTitle("Some more subjects in " + Context.Guild.Name + "...")
//                                    .WithColor(new Color(60, 176, 222));
//                            }
//                        }
//                        embed.WithFooter(count + " subjects in this school.", "https://i.imgur.com/HAI5vMj.png");
//                        await Context.Channel.SendMessageAsync("", false, embed.Build());
//                    }
//                    return;

//            } // Subject functions.
            
//            // SocketGuildChannel channel = Context.Guild.GetChannel(400434742790586378);
//            subjects = DataStorage.GetObjectsInFolder<Subject>(subjectfolder);
//            string[] Hours; string Teacher;
//            try
//            {
//                Teacher = list[2].Replace("\"", "");

//                string Hourslist = list[3].Replace("\"", "");
//                if (Hourslist.Contains(",")) Hours = Hourslist.Split(","); else Hours = new string[1] { Hourslist };

//                subjectname = list[4].Replace("\"", "");
//                subject = subjects.Find(x => String.Compare(x.name, subjectname) == 0);
//                if (subject.Equals(null) || subject.name == "")
//                {
//                    await SendClassicEmbed($"**Subject not found! with name {subject.name}**",
//                        "Use Subject.Add to add the new subject! Or ensure it matches.");
//                    return;
//                }


//                await Utilities.Log(MethodBase.GetCurrentMethod(), "2" + Teacher + "3" + subject, LogSeverity.Verbose);
//            } catch (Exception ex)
//            {
//                await SendClassicEmbed("**Syntax problem!**", "Review how the class command is formatted.");
//                await Utilities.Log(MethodBase.GetCurrentMethod(), "Exception expanding permaters on class command", ex, LogSeverity.Warning);
//                return;
//            } // Assign and try subject.

//            /* TO DO:
//             * Do not allow for subject, hour, or teacher edits to conflict with existing classes.
//             * Check for class congruency? Something like !group Class Review.
//             * Help command responds with instructions if server is not configured properly, "bot owner can add rapidly..."
//             * If it is it responds differently to different roles!
//             * 
//             */

//            await Utilities.Log(MethodBase.GetCurrentMethod(), "SubjectFile Created", LogSeverity.Verbose);
//            await Context.Channel.SendMessageAsync($"{ClassName} // {Teacher} {Hours.ToString()} [{subject.name}]");
//            await (Context.Channel as ITextChannel)?.ModifyAsync(x =>
//            {
//                x.Name = "do-not-enter";
//                x.Position = 0;
//                x.Topic = "topic of the channel";
//            });
//            //DataStorage.StoreObject(selection, GuildsData.GuildsFolder + "/" + Context.Guild.Id + ".json", true);
//            //var Group = await Context.Guild.CreateTextChannelAsync(string.Join(" ", list.Skip(1)));
//            //await Group.TriggerTypingAsync();
//        }


//    }


















    
//    // Methods
//    public sealed partial class LeaderRoleCommands : InteractiveBase
//    {
//        /*
//        public async Task<GroupClass> FindCorrespondingClasses<T>(dynamic criterion)
//        {
            
//        }
//        */
//        /// <summary>
//        /// Sends each property of given object in embed.
//        /// </summary>
//        /// <param name="obj"></param>
//        /// <param name=""></param>
//        /// <returns></returns>
//        public async Task SendClassEmbed<T>(string title, string desc, object obj)
//        {
//            var embed = new EmbedBuilder();
//            embed.WithTitle($"**{title}**")
//                .WithDescription($"*{desc}*")
//                .WithColor(new Color(60, 176, 222))
//                .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png")
//                .WithCurrentTimestamp();
//            PropertyInfo[] properties = typeof(T).GetProperties();
//            foreach (PropertyInfo property in properties)
//                embed.AddField($"**{property.Name}**", $"*{property.GetValue(obj)}*", true);
//            await Context.Channel.SendMessageAsync("", false, embed.Build());
//        }
//        public async Task SendSuccessEmbed(string title, string desc)
//        {
//            var embed = new EmbedBuilder
//            {
//                Title = title,
//                Description = desc,
//                Color = new Color(33, 160, 52),
//                Footer = new EmbedFooterBuilder()
//                    .WithText(" -Alex https://discord.gg/DVSjvGa")
//                    .WithIconUrl("https://i.imgur.com/HAI5vMj.png"),
//                Timestamp = DateTime.UtcNow,
//            }.Build();
//            await Context.Channel.SendMessageAsync("", false, embed);
//        }
//        public async Task SendErrorEmbed(string title, string desc, Exception ex = null)
//        {
//            if (ex == null) ex = new Exception();
//            var embed = new EmbedBuilder
//            {
//                Title = title,
//                Description = desc,
//                Color = new Color(219, 57, 75),
//                Footer = new EmbedFooterBuilder()
//                    .WithText(" -Alex https://discord.gg/DVSjvGa")
//                    .WithIconUrl("https://i.imgur.com/HAI5vMj.png"),
//                Timestamp = DateTime.UtcNow,
//                Fields = new List<EmbedFieldBuilder>
//                {
//                    new EmbedFieldBuilder()
//                        .WithName("`Error content:` ")
//                        .WithValue($"*{ex.ToString()}*")
//                        .WithIsInline(true)
//                    // Repeat with comma for another field here.
//                }
//            }.Build();
//            await Context.Channel.SendMessageAsync("", false, embed);
//        }
//        public async Task SendClassicEmbed(string title, string desc)
//        {
//            var embed = new EmbedBuilder
//            {
//                Title = title,
//                Description = desc,
//                Color = new Color(60, 176, 222),
//                Footer = new EmbedFooterBuilder()
//                    .WithText(" -Alex https://discord.gg/DVSjvGa")
//                    .WithIconUrl("https://i.imgur.com/HAI5vMj.png"),
//                Timestamp = DateTime.UtcNow,
//            }.Build();
//            await Context.Channel.SendMessageAsync("", false, embed);
//        }
//        public async Task SendGroupClassEmbed(GroupClass Class, string desc = "View them all with Class.Display! :smiley:")
//        {
//            var embed = new EmbedBuilder();
//            embed.WithTitle($"Class with the name \"{Class.name}\" from {Context.Guild.Name}.")
//                .WithDescription(desc)
//                .AddField("**Data**", $":notepad_spiral: ReferenceID: `{Class.id}`")
//                .AddField("**Teacher**", $":school: Teacher: `{Class.teacher.name}`");
//            string hourscomment = "";
//            foreach (Hour h in Class.hours) hourscomment = String.Concat(hourscomment, $":clock1: Hour: `{h.title}`\n");
//            embed.AddField("**Hours**", hourscomment, true);
//            string rolescomment = "";
//            foreach (ulong r in Class.roles) rolescomment = String.Concat(rolescomment, $":link: Role: " +
//                $"{Context.Guild.GetRole(r).Name}\n");
//            embed.AddField("**Roles**", rolescomment, true);
//            if (!Class.subject.Equals(null)) embed.AddField("**Subject**", $":beach_umbrella: Subject: `{Class.subject.Value.name}`");
//            string channelscomment = "";
//            if (Class.channels.Count() > 0)
//            {
//                foreach (ulong id in Class.channels)
//                {
//                    var classchannelT = Context.Guild.GetTextChannel(id);
//                    if (!Equals(classchannelT, null))
//                        channelscomment = String.Concat(channelscomment, $":speech_balloon: Channel: {classchannelT.Mention}\n");
//                    else
//                    {
//                        var classchannelV = Context.Guild.GetVoiceChannel(id);
//                        if (!Equals(classchannelV, null))
//                            channelscomment = String.Concat(channelscomment, $":speaker: Channel: {classchannelV.Name}\n");
//                        else
//                        {
//                            var channels = Class.channels.ToList<ulong>();
//                            channels.RemoveAll(delegate (ulong c)
//                            {
//                                return c == id;
//                            });
//                            Class.channels = channels.ToArray();
//                            channelscomment = String.Concat(channelscomment, $":x: Channel could not be found! *Removed from class.* `{id}`\n");
//                        }
//                    }
//                }
//                embed.AddField(Class.channels.Count() > 1 ? "**Channels**" : "**Channel**", channelscomment, true);
//            }
//            embed.WithColor(new Color(60, 176, 222))
//                .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png");
//            await Context.Channel.SendMessageAsync("", false, embed.Build());
//        }
//        public async Task<Subject> CreateSubject(string subjectname, string subjectfolder)
//        {
//            Subject subject = new Subject();
//            try
//            {
//                RestTextChannel subjectTC = Context.Guild.CreateTextChannelAsync(subjectname).Result;
//                GuildConfig guildconfig = GuildsData.FindOrCreateGuildConfig(Context.Guild);
//                var restrole = await Context.Guild.CreateRoleAsync(subjectname);
//                subject = new Subject
//                {
//                    id = GenerateID(subjectfolder),
//                    name = subjectname,
//                    voicechannelid = null,
//                    textchannelid = subjectTC.Id,
//                    roleid = restrole.Id, // Converting to socketroleid
//                };
//                // Modify role settings.
//                await restrole.ModifyAsync(r =>
//                {
//                    r.Color = new Color(119, 118, 158);
//                    r.Hoist = false;
//                    r.Mentionable = true;
//                    r.Permissions = GuildPermissions.None;
//                    //r.Position = Context.Guild.GetRole(guildconfig.VisitorRoleID).Position - 1;
//                });
//                // Add text channel perms.
//                await subjectTC.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, EveryonePermissions());
//                await subjectTC.AddPermissionOverwriteAsync(restrole, ClassPermissions());

//                await ReplyAsync("Would you like to create a voice channel for this subject? \"yes\" or anything else.");
//                var q = await AwaitMessage(100, "Well this is an awkward step to fail at.");
//                if (q.Content == "yes")
//                {
//                    RestVoiceChannel subjectVC = Context.Guild.CreateVoiceChannelAsync(subjectname).Result;
//                    subject.voicechannelid = subjectVC.Id;
//                    // Add voice channel perms.-

//                    await subjectVC.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, EveryonePermissions());
//                    await subjectVC.AddPermissionOverwriteAsync(restrole, ClassPermissions());
//                }

//                return subject;
//            }
//            catch (Exception ex)
//            {
//                await SendErrorEmbed("Error generating subject.", $"*Please check syntax or contact server owner.*", ex);
//                await Utilities.Log(MethodBase.GetCurrentMethod(), "Error generating subject.", ex, LogSeverity.Error);
//                return subject;
//            }
//        }

//        public OverwritePermissions DirectorPermissions() => new OverwritePermissions(
//                /*createInstantInvite*/PermValue.Inherit,/*manageChannel*/PermValue.Allow,
//                /*addReactions*/PermValue.Inherit,/*readMessages*/PermValue.Inherit,
//                /*sendMessages*/PermValue.Inherit, /*sendTTSMessages*/PermValue.Allow,
//                /*manageMessages*/PermValue.Allow, /*embedLinks*/PermValue.Inherit,
//                /*attachFiles*/PermValue.Inherit, /*readMessageHistory*/PermValue.Inherit,
//                /*mentionEveryone*/PermValue.Inherit, /*useExternalEmojis*/PermValue.Inherit,
//                /*connect*/PermValue.Inherit, /*speak*/PermValue.Inherit,
//                /*muteMembers*/PermValue.Allow, /*deafenMembers*/PermValue.Allow,
//                /*moveMembers*/PermValue.Allow, /*useVoiceActivation*/PermValue.Inherit,
//                /*manageRoles*/PermValue.Allow, /*manageWebhooks*/PermValue.Allow);
//        public OverwritePermissions AdminPermissions() => new OverwritePermissions(PermValue.Inherit,
//                PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit,
//                PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit,
//                PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit,
//                PermValue.Inherit, PermValue.Allow, PermValue.Allow, PermValue.Inherit,
//                PermValue.Inherit, PermValue.Inherit, PermValue.Inherit);
//        public OverwritePermissions ModPermissions() => new OverwritePermissions(PermValue.Inherit,
//                PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit,
//                PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit,
//                PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit,
//                PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit,
//                PermValue.Inherit, PermValue.Inherit, PermValue.Inherit);
//        public OverwritePermissions CManagerPermissions() => new OverwritePermissions(PermValue.Inherit,
//                PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit,
//                PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit,
//                PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit,
//                PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit,
//                PermValue.Inherit, PermValue.Inherit, PermValue.Inherit);
//        public OverwritePermissions GManagerPermissions() => new OverwritePermissions(PermValue.Inherit,
//                PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit,
//                PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit,
//                PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit,
//                PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit,
//                PermValue.Inherit, PermValue.Allow, PermValue.Allow);
//        public OverwritePermissions WManagerPermissions() => new OverwritePermissions(PermValue.Inherit,
//                PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit,
//                PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit,
//                PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit,
//                PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit,
//                PermValue.Inherit, PermValue.Inherit, PermValue.Allow);
//        public OverwritePermissions VManagerPermissions() => new OverwritePermissions(PermValue.Inherit,
//                PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit,
//                PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit,
//                PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit,
//                PermValue.Inherit, PermValue.Allow, PermValue.Allow, PermValue.Allow,
//                PermValue.Inherit, PermValue.Allow, PermValue.Allow);
//        public OverwritePermissions ClassPermissions() => new OverwritePermissions(PermValue.Inherit,
//                PermValue.Inherit, PermValue.Allow, PermValue.Allow, PermValue.Allow,
//                PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Allow,
//                PermValue.Allow, PermValue.Inherit, PermValue.Allow, PermValue.Allow,
//                PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit,
//                PermValue.Allow, PermValue.Inherit, PermValue.Inherit);
//        public OverwritePermissions EveryonePermissions() => new OverwritePermissions(PermValue.Deny,
//                PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny,
//                PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny,
//                PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny,
//                PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny,
//                PermValue.Deny, PermValue.Deny, PermValue.Deny);


//        public async Task<GroupClass> CreateGroupClass(string name, string hourFolder, string teacherFolder, string classFolder, string subjectFolder = "")
//        {
//            // find hour.
//            await ReplyAsync("Firstly, specify the **hours this class is in**. Format it like " +
//                "`hourname,hourname,hourname` Or simply `hourname` -- No spaces!");
//            var hoursmsg = await AwaitMessage(100, "Try `h!group Class Hour.Add`");
//            if (hoursmsg.Equals(null)) return null;
//            List<Hour> lhours = DataStorage.GetObjectsInFolder<Hour>(hourFolder); List<Hour> shours;
//            if (hoursmsg.Content.Contains(",")) shours = lhours.FindAll(x => hoursmsg.Content.Split(",").Contains(x.title));
//            else shours = lhours.FindAll(h => h.title == hoursmsg.Content);
//            if (shours.Equals(null) || shours.Count == 0)
//            {
//                await ReplyAsync("No hour found, remember to be exact! Try `h!group Class Hour.Add`");
//                return null;
//            }
//            await ReplyAsync(shours.Count + ((shours.Count > 1) ? " hours" : " hour") + $" found! *(first is {shours.First().title})*");
//            // find teacher.
//            await ReplyAsync("Great! Next up, the teacher of this class. Just say the name of the teacher as it is.");
//            var teachermsg = await AwaitMessage(100, "Try `h!group Class Teacher.Add`");
//            if (teachermsg.Equals(null)) return null;
//            List<Teacher> teachers = DataStorage.GetObjectsInFolder<Teacher>(teacherFolder); Teacher teacher;
//            teacher = teachers.Find(t => t.name == teachermsg.Content);
//            if (teacher.Equals(null) || teacher.name != teachermsg.Content)
//            {
//                await ReplyAsync($"No teacher found with name **{teachermsg.Content}**, remember to be exact!" +
//                    $" Try `h!group Class Hour.Add`");
//                return null;
//            }
//            await ReplyAsync($"Teacher found with name {teacher.name}.");
//            // subject?
//            await ReplyAsync("Perfect! Is there a subject in this school that can be applied to this class? If so," +
//                " say the name of the subject. If not, simply say \"no\"");
//            var subjectmsg = await AwaitMessage(100, "Try `h!group Class Subject.Add`");
//            if (subjectmsg.Equals(null)) return null;
//            Subject subject; SocketTextChannel channel;
//            if (subjectmsg.Content != "no")
//            {
//                var subjects = DataStorage.GetObjectsInFolder<Subject>(subjectFolder);
//                subject = subjects.Find(s => s.name == subjectmsg.Content);
//                if (subject.Equals(null))
//                {
//                    await ReplyAsync($"No subject found with name **{subjectmsg.Content}**, remember to be exact!" +
//                        $" Try `h!group Class Subject.Add`");
//                    return null;
//                }
//                await ReplyAsync($"Subject found with name {subject.name}.");
//            }
//            //

//            await ReplyAsync("**All set!** I'll automatically generate the channels and roles for this class, " +
//                "and get back to you once I'm done...");

//            // Generate roles, return array of roles and hours.
//            List<RestRole> roles = new List<RestRole>();
//            ulong[] roleids = new ulong[shours.Count];
//            Hour[] hours = shours.ToArray();
//            int i = 0;
//            foreach (Hour h in shours)
//            {
//                RestRole role = await Context.Guild.CreateRoleAsync($"{h.title}: {teacher.name}, {name}");
//                await role.ModifyAsync(r =>
//                {
//                    r.Color = new Color(89, 88, 133);
//                    r.Hoist = false;
//                    r.Mentionable = true;
//                    r.Permissions = GuildPermissions.None;
//                    //r.Position = Context.Guild.GetRole(guildconfig.VisitorRoleID).Position - 1;
//                });
//                roles.Add(role);
//                roleids[i] = role.Id;
//                i++;
//            }

//            GroupClass Class = new GroupClass(); // Class is created!
//            string newchannelmention = "end me"; // wish i didn't have to do this.

//            if (subjectmsg.Content == "no") // Assign values to class. Check if channel is wanted.
//            {
//                await ReplyAsync("Would you like to create a channel for this class? \"yes\" or anything else.");
//                var qchannel = await AwaitMessage(100, "Well this is an awkward step to fail at.");
//                if (qchannel.Equals(null)) return null;
//                if (qchannel.Content == "yes")
//                {
//                    var newchannel = Context.Guild.CreateTextChannelAsync($"{teacher.name}, {name}").Result;
//                    await newchannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, EveryonePermissions());
//                    foreach (RestRole r in roles) await newchannel.AddPermissionOverwriteAsync(r, ClassPermissions());
//                    newchannelmention = newchannel.Mention;
//                    Class = new GroupClass()
//                    {
//                        hours = hours,
//                        teacher = teacher,
//                        roles = roleids,
//                        subject = null,
//                        channels = new ulong[] { newchannel.Id },
//                        name = name,
//                        id = GenerateID(classFolder) // Group values.
//                    }; // Here class is complete. With new channel.
//                }
//                else Class = new GroupClass()
//                {
//                    hours = hours,
//                    teacher = teacher,
//                    roles = roleids,
//                    subject = null,
//                    channels = new ulong[0],
//                    name = name,
//                    id = GenerateID(classFolder) // Group values.
//                }; // Here class is complete. Without new channel.
//            }
//            else
//            {
//                var subjects = DataStorage.GetObjectsInFolder<Subject>(subjectFolder);
//                subject = subjects.Find(s => s.name == subjectmsg.Content); // Have to relocate subject... inefficient I know.
//                channel = Context.Guild.GetTextChannel(subject.textchannelid);
//                foreach (RestRole r in roles) await channel.AddPermissionOverwriteAsync(r, ClassPermissions());
//                Class = new GroupClass()
//                {
//                    hours = hours,
//                    teacher = teacher,
//                    roles = roleids,
//                    subject = subject,
//                    channels = new ulong[0],
//                    name = name,
//                    id = GenerateID(classFolder) // Group values.
//                }; // Here class is complete. With subject.
//            }

//            // Send final embed.
//            var Classes = DataStorage.GetObjectsInFolder<GroupClass>(classFolder);
//            var embed = new EmbedBuilder();
//            embed.WithTitle($"Class with the name \"{Class.name}\" added to {Context.Guild.Name}.")
//                .WithDescription("View them all with Class.Display! :smiley:" +
//                    (Classes.Count < 2 ? "" : $" *We're now up to {Classes.Count} classs*"))
//                .AddField("**Data**", $":notepad_spiral: ReferenceID: `{Class.id}`")
//                .AddField("**Teacher**", $":school: Teacher: `{Class.teacher.name}`");
//            string hourscomment = "";
//            foreach (Hour h in Class.hours) hourscomment = String.Concat(hourscomment, $":clock1: Hour: `{h.title}`\n");
//            embed.AddField("**Hours**", hourscomment, true);
//            string rolescomment = "";
//            foreach (RestRole r in roles) rolescomment = String.Concat(rolescomment, $":link: Role: {r.Mention}\n");
//            embed.AddField("**Roles**", rolescomment, true);
//            if (!Class.subject.Equals(null)) embed.AddField("**Subject**", $":beach_umbrella: Subject: `{Class.subject.Value.name}`");
//            string channelscomment = "";
//            if (Class.channels.Count() > 0)
//            {
//                foreach (ulong id in Class.channels)
//                {
//                    var classchannelT = Context.Guild.GetTextChannel(id);
//                    if (!Equals(classchannelT, null))
//                        channelscomment = String.Concat(channelscomment, $":speech_balloon: Channel: {classchannelT.Mention}\n");
//                    else
//                    {
//                        var classchannelV = Context.Guild.GetVoiceChannel(id);
//                        if (!Equals(classchannelV, null))
//                            channelscomment = String.Concat(channelscomment, $":speaker: Channel: {classchannelV.Name}\n");
//                        else
//                        {
//                            var channels = Class.channels.ToList<ulong>();
//                            channels.RemoveAll(delegate (ulong c)
//                            {
//                                return c == id;
//                            });
//                            Class.channels = channels.ToArray();
//                            channelscomment = String.Concat(channelscomment, $":x: Channel could not be found! *Removed from class.* `{id}`\n");
//                        }
//                    }
//                }
//                embed.AddField(Class.channels.Count() > 1 ? "**Channels**" : "**Channel**", channelscomment, true);
//            }
//            embed.WithColor(new Color(60, 176, 222))
//                .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png");
//            await Context.Channel.SendMessageAsync("", false, embed.Build());


//            return Class;
//        }

//        /// <summary>
//        /// Returns new designer class, that replaces an existing class.
//        /// Returns null if process was left unfinished.
//        /// </summary>
//        /// <param name="Class"></param>
//        /// <param name="hourFolder"></param>
//        /// <returns></returns>
//        public async Task<GroupClass> EditGroupClass(GroupClass Class, string classFolder, string hourFolder,
//            string teacherFolder, string subjectFolder)
//        {
//            GroupClass NewClass = Class;
//            await SendGroupClassEmbed(Class);
//            await Context.Channel.SendMessageAsync($"**Specify what elements you'd like to change, in a comma seperated " +
//                $"list like: ** \n `hours,teacher,subject,channels,name`");
//            var emsg = await AwaitMessage(100);
//            if (emsg.Equals(null)) return null;
//            List<string> elements = new List<string>(); byte changecount = 0;
//            if (emsg.Content.Contains(",")) elements = emsg.Content.Split(",").ToList();
//            else elements.Add(emsg.Content);
//            Context.Channel.EnterTypingState(RequestOptions.Default);
//            if (elements.Find(x => x.Contains("hour")) != null)
//            {
//                var embed = new EmbedBuilder(); string hourscomment = "";
//                foreach (Hour h in Class.hours) hourscomment = String.Concat(hourscomment, $":clock1: Hour: `{h.title}`\n");
//                embed.WithTitle($"Class with the name \"{Class.name}\" from {Context.Guild.Name}.")
//                    .WithDescription("Respond with hours from this school in another comma seperated list, if they're in " +
//                    "the class already they will be removed, if not, they will be added.")
//                    .AddField("**Hours**", hourscomment, true)
//                    .WithColor(new Color(60, 176, 222))
//                    .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png");
//                await Context.Channel.SendMessageAsync("", false, embed.Build());
//                var hmsg = await AwaitMessage(100); List<string> nhours = new List<string>();
//                if (hmsg.Equals(null)) return Class;
//                List<Hour> lhours = DataStorage.GetObjectsInFolder<Hour>(hourFolder); List<Hour> shours;
//                if (hmsg.Content.Contains(",")) shours = lhours.FindAll(x => hmsg.Content.Split(",").Contains(x.title));
//                else shours = lhours.FindAll(h => h.title == hmsg.Content);
//                if (shours.Count > 0) await ReplyAsync(shours.Count + ((shours.Count > 1) ? " hours" : " hour")
//                    + $" found! *(first is {shours.First().title})*");
//                else await ReplyAndDeleteAsync("Hour not found. Moving on...", timeout: new TimeSpan(10000000 * 10));
//                foreach (Hour h in shours)
//                {
//                    var nh = NewClass.hours.ToList<Hour>();
//                    if (NewClass.hours.Contains(h)) nh.Remove(h); else nh.Add(h);
//                    NewClass.hours = nh.ToArray();
//                }
//                changecount++;
//            }
//            if (elements.Find(x => x.Contains("teacher")) != null)
//            {
//                var embed = new EmbedBuilder();
//                embed.WithTitle($"Class with the name \"{Class.name}\" from {Context.Guild.Name}.")
//                    .WithDescription("Respond with a new teacher from this school to replace the current one!")
//                    .AddField("**Teacher**", $":school: Teacher: `{Class.teacher.name}`")
//                    .WithColor(new Color(60, 176, 222))
//                    .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png");
//                await Context.Channel.SendMessageAsync("", false, embed.Build());
//                var tmsg = await AwaitMessage(100);
//                if (tmsg.Equals(null)) return null;
//                List<Teacher> teachers = DataStorage.GetObjectsInFolder<Teacher>(teacherFolder); Teacher teacher;
//                teacher = teachers.Find(t => t.name == tmsg.Content);
//                if (!Equals(teacher, null))
//                {
//                    await ReplyAsync($"Teacher found with the name {teacher.name}!");
//                    NewClass.teacher = teacher;
//                }
//                else await ReplyAndDeleteAsync("Teacher not found. Moving on...", timeout: new TimeSpan(10000000 * 10));
//                changecount++;
//            }
//            if (elements.Find(x => x.Contains("channel")) != null)
//            {
//                var embed = new EmbedBuilder();
//                embed.WithTitle($"Class with the name \"{Class.name}\" from {Context.Guild.Name}.")
//                    .WithDescription("Respond with the ID of a channel to add it, make it a comma seperated list to add more!" +
//                    (Class.subject.Equals(null) ? "" : " Include an ID of a preexisting channel to remove it from the class."))
//                    .WithColor(new Color(60, 176, 222))
//                    .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png");
//                string channelscomment = "";
//                if (Class.channels.Count() > 0)
//                {
//                    foreach (ulong id in Class.channels)
//                    {
//                        var classchannelT = Context.Guild.GetTextChannel(id);
//                        if (!Equals(classchannelT, null))
//                            channelscomment = String.Concat(channelscomment, $":speech_balloon: Channel: {classchannelT.Mention}\n");
//                        else
//                        {
//                            var classchannelV = Context.Guild.GetVoiceChannel(id);
//                            if (!Equals(classchannelV, null))
//                                channelscomment = String.Concat(channelscomment, $":speaker: Channel: {classchannelV.Name}\n");
//                            else
//                            {
//                                var channels = Class.channels.ToList<ulong>();
//                                channels.RemoveAll(delegate (ulong c)
//                                {
//                                    return c == id;
//                                });
//                                Class.channels = channels.ToArray();
//                                channelscomment = String.Concat(channelscomment, $":x: Channel could not be found! *Removed from class.* `{id}`\n");
//                            }
//                        }
//                    }
//                    embed.AddField(Class.channels.Count() > 1 ? "**Channels**" : "**Channel**", channelscomment, true);
//                }
//                await Context.Channel.SendMessageAsync("", false, embed.Build());
//                var cmsg = await AwaitMessage(100);
//                if (cmsg.Equals(null)) return null;
//                List<string> Cids = new List<string>();
//                if (cmsg.Content.Contains(",")) Cids = cmsg.Content.Split(",").ToList();
//                    else Cids.Add(cmsg.Content);
//                foreach (string msg in Cids)
//                {
//                    var classchannels = NewClass.channels.ToList();
//                    try
//                    {
//                        var id = Convert.ToUInt64(msg);
//                        var channel = Context.Guild.GetTextChannel(id);
//                        var channel2 = Context.Guild.GetVoiceChannel(id);
//                        if (classchannels.Find(x => x == id) == id)
//                        {
//                            // If channel is found that matches 
//                            classchannels.RemoveAll(x => x == id);
//                            NewClass.channels = classchannels.ToArray();
//                        }
//                        else if (!Equals(channel, null) || !Equals(channel2, null))
//                        {
//                            // If new channel is found. Roles will be added in reinitialization.
//                            classchannels.Add(id);
//                            NewClass.channels = classchannels.ToArray();
//                        }
//                        else await ReplyAsync($"No channel found with ID **{cmsg.Content}**, be sure to enable developer mode!");
//                    }
//                    catch
//                    {
//                        await ReplyAndDeleteAsync($"**{cmsg.Content}** is not a valid ID." +
//                            $" I'll look for a channel called it anyways...", timeout: new TimeSpan(10000000 * 3));
//                        var channelsearch = Context.Guild.Channels.ToList().Find(x => x.Name == msg);
//                        if (msg == channelsearch.Name)
//                        {
//                            if (classchannels.Find(x => x == channelsearch.Id) == channelsearch.Id)
//                            {
//                                // If channel is found that matches 
//                                classchannels.RemoveAll(x => x == channelsearch.Id);
//                                NewClass.channels = classchannels.ToArray();
//                            }
//                            else
//                            {
//                                classchannels.Add(channelsearch.Id);
//                                NewClass.channels = classchannels.ToArray();
//                            }
//                        }
//                    }
//                }
//                changecount++;
//            }
//            if (elements.Find(x => x.Contains("subject")) != null)
//            {
//                var embed = new EmbedBuilder();
//                embed.WithTitle($"Class with the name \"{Class.name}\" from {Context.Guild.Name}.")
//                    .WithDescription("Respond with a new subject to add it!" +
//                    (Class.subject.Equals(null) ? "" : " Or say the name of the current channel to remove it."))
//                    .WithColor(new Color(60, 176, 222))
//                    .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png");
//                if (!Class.subject.Equals(null)) embed.AddField("**Subject**", $":beach_umbrella: Subject: `{Class.subject.Value.name}`");
//                await Context.Channel.SendMessageAsync("", false, embed.Build());
//                var smsg = await AwaitMessage(100);
//                if (smsg.Equals(null)) return null;
//                var subjects = DataStorage.GetObjectsInFolder<Subject>(subjectFolder);
//                var subject = subjects.Find(s => s.name == smsg.Content);
//                if (!subject.Equals(null)) NewClass.subject = subject;
//                else await ReplyAsync($"No subject found with name **{smsg.Content}**, remember to be exact!" +
//                        $" Try `h!group Class Subject.Add`");
//                changecount++;
//            }
//            if (elements.Find(x => x.Contains("name")) != null)
//            {
//                await ReplyAsync($"Currently, the class is named **{Class.name}**. What would you like to change it to?");
//                var nmsg = await AwaitMessage(100);
//                if (nmsg.Equals(null)) return null;
//                NewClass.name = nmsg.Content;
//                changecount++;
//            }
//            if (changecount == 0)
//            {
//                await SendErrorEmbed("**No elements selected!**", "Class will not be changed.");
//                return Class;
//            }
//            // No changes have been made except a model new class has been created.
//            await ReplyAndDeleteAsync("**Changes selected!** Let me process them for you real quick...", timeout: new TimeSpan(10000000 * 10));
//            return await ReinitializeGroupClass(Class, NewClass, hourFolder, teacherFolder, classFolder, subjectFolder);
//        }
//        /// <summary>
//        /// Passes back new succeeding groupclass.
//        /// </summary>
//        /// <param name="Original"></param>
//        /// <param name="New"></param>
//        /// <param name="hourFolder"></param>
//        /// <param name="teacherFolder"></param>
//        /// <param name="classFolder"></param>
//        /// <param name="subjectFolder"></param>
//        /// <returns></returns>
//        public async Task<GroupClass> ReinitializeGroupClass(GroupClass Original, GroupClass New, string hourFolder, 
//            string teacherFolder, string classFolder, string subjectFolder)
//        {
//            var Checked = await CheckGroupClass(New, classFolder, hourFolder, teacherFolder, subjectFolder);
//            await ReplyAndDeleteAsync("*Class prepared. Now cleaning up class...*", timeout: new TimeSpan(10000000 * 3));
//            // hours, teacher, subject, channels might be different. Delete roles and channels only if you have to.
//            if (Original.roles != Checked.roles)
//                await Utilities.Log(MethodBase.GetCurrentMethod(), "Roles should not be different!", LogSeverity.Error);
//            if (Original.hours != Checked.hours)
//            {
//                // foreach change in hour change roles. save new roles. 
//                // roles not effected are recreated if there is a change in teacher or name.
//                foreach (Hour ch in Checked.hours)
//                {
//                    foreach (Hour oh in Original.hours)
//                    {
//                        if (Equals(ch, oh))
//                        {

//                        }
//                        else 
//                        {

//                        }
//                    }
//                }
                
//            }
//            if (Original.channels != Checked.channels || !Equals(Original.subject, Checked.subject))
//            {
//                // remove from all existing channels, add to checked channels.
//                // remove from previous subject, add to new.



//            }
//            await ReplyAndDeleteAsync("*Ready for new class! Applying new class...*", timeout: new TimeSpan(10000000 * 3));
//            return Checked;
//            // if subject is changed, remove it from previous if there was one. and add it to the new if there is one.


//        }

//        /// <summary>
//        /// Process to clean class content from guild. <c>interactive</c> will ask user for confirmation.
//        /// <b>Does not modify class in persistence.</b>
//        /// </summary>
//        /// <param name="Class"></param>
//        /// <returns></returns>
//        public async Task DeleteGroupClass(GroupClass Class, bool _interactive = false)
//        {
//            if (_interactive)
//            {

//            }
//            else
//            {
//                // Delete channels.
//                var channels = Class.channels.ToList();
//                var missing = new List<ulong>();
//                foreach (var id in channels)
//                {
//                    var channel = Context.Guild.GetTextChannel(id);
//                    var channel2 = Context.Guild.GetVoiceChannel(id);
//                    if (!Equals(channel, null)) await channel.DeleteAsync();
//                    else if (!Equals(channel2, null)) await channel2.DeleteAsync();
//                    else missing.Add(id);
//                }
//                if (missing.Count > 0)
//                    await Utilities.Log(MethodBase.GetCurrentMethod(), "Missing channels when clearing class.", LogSeverity.Debug);
//                // Delete roles.
//                var roles = Class.roles.ToList();
//                missing = new List<ulong>();
//                foreach (var id in channels)
//                {
//                    var role = Context.Guild.GetRole(id);
//                    if (!Equals(role, null)) await role.DeleteAsync();
//                    else missing.Add(id);
//                }
//                if (missing.Count > 0)
//                    await Utilities.Log(MethodBase.GetCurrentMethod(), "Missing roles when clearing class.", LogSeverity.Debug);
//            }
//            return;
//        }
//        /// <summary>
//        /// Simply removes class from database.
//        /// </summary>
//        /// <param name="Class"></param>
//        /// <param name="hourFolder"></param>
//        public void RemoveGroupClass(GroupClass Class, string hourFolder) => DataStorage.DeleteFile(hourFolder + "/" + Class.id + ".json");

//        /// <summary>
//        /// Leave corresponding folders empty to not check them. Responds context with embed for conflicts.
//        /// Only deletes class if user approves.
//        /// </summary>
//        public async Task<GroupClass> CheckGroupClass(GroupClass Class, string classFolder,
//            string hourFolder = "", string teacherFolder = "", string subjectFolder = "")
//        {
//            /* 1 Check each property of class for existence.
//             * 2 If one fails, request replacement.
//             * 3 Once done, say it's checked.
//             * This should only find missing roles and channels anything else means the database was tampered with.
//             */
//            // returns class with changes if needed.
//            await ReplyAsync("check process...");

//            var Checked = Class;
//            return Checked;
//        }
//        /// <summary>
//        /// CheckGroupClass() for each class. And after search for classes with same hour AND teacher, or same roles or channel.
//        /// </summary>
//        /// <param name="classFolder"></param>
//        /// <param name="hourFolder"></param>
//        /// <param name="teacherFolder"></param>
//        /// <param name="subjectFolder"></param>
//        /// <returns></returns>
//        public async Task CheckGroupClasses(string classFolder, string hourFolder = "",
//            string teacherFolder = "", string subjectFolder = "")
//        {
//            // Invoked by a command.

//        }
//        /// <summary>
//        /// Leave corresponding folders empty to not check them. Responds context with embed for conflicts.
//        /// By default, will simply send embed of classes with given GroupClass dependency.
//        /// removeDependency will delete them and ask for replacements.
//        /// </summary>
//        /// <remarks>
//        /// Always include a dependency from GroupClass.
//        /// </remarks>
//        public async Task CheckClassesDependency(dynamic dependency, string classFolder, bool removeDependency = false)
//        {
//            // Deals with edited hour, teacher, or subject.

//        }



//        public async Task<SocketMessage> AwaitMessage(int wait, string comment = "")
//        {
//            var msg = await NextMessageAsync(true, true, new TimeSpan(10000000 * wait));
//            if (msg == null)
//            {
//                await ReplyAsync($"**You did not reply in time {Context.User.Mention}!** Waited {wait} seconds. {comment}");
//                return null;
//            }
//            return msg;
//        }
//        /// <summary>
//        /// Gemerates unique ID ulong.
//        /// </summary>
//        /// <param name="inUse"></param>
//        /// <returns></returns>g
//        public ulong GenerateID(ulong[] inUse)
//        {
//            HashSet<ulong> IDs = new HashSet<ulong>(inUse);
//            ulong newID = Utilities.NextUlong();
//            while (IDs.Add(newID) == false) newID = Utilities.NextUlong();
//            return newID;
//        }
//        /// <summary>
//        /// Retrieves file names without extensions and converts to list of ulong.
//        /// Generates unique ID ulong.
//        /// </summary>
//        /// <param name="folder"></param>
//        /// <returns></returns>
//        public ulong GenerateID(string folder) => GenerateID(SafeConversion<ulong>(DataStorage.GetFilesInFolder(folder), 0).ToArray());
//        /// <summary>
//        /// Tries to convert each in list. If a string is empty and skip is not set to true, it will add defaultValue to the list.
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="toConvert"></param>
//        /// <param name="defaultValue"></param>
//        /// <param name="skipEmptyValues"></param>
//        /// <returns></returns>
//        public List<T> SafeConversion<T>(string[] toConvert, T defaultValue, bool skipEmptyValues = true)
//        {
//            List<T> list = new List<T>();
//            foreach (string str in toConvert)
//            {
//                try
//                {
//                    if (!string.IsNullOrEmpty(str))
//                        list.Add((T)Convert.ChangeType(str, typeof(T)));
//                    else if (!skipEmptyValues)
//                        list.Add(defaultValue);
//                }
//                catch (Exception ex)
//                {
//                    Utilities.Log(MethodBase.GetCurrentMethod(), $"Error converting file name to {typeof(T).ToString()} [{str}]", ex, LogSeverity.Warning);
//                }
//            }
//            return list;
//        }
//        public SocketGuildChannel GetChannel(string name) => Context.Guild.Channels.FirstOrDefault(r => r.Name == name);
//        public static bool IsDivisible(int x, int n)
//        {
//            Utilities.Log(MethodBase.GetCurrentMethod(), x + " % " + n + " = " + (x % n), LogSeverity.Debug);
//            return (x % n) == 0;
//        }
//        public static string[] SplitArguments(string commandLine)
//        {
//            var parmChars = commandLine.ToCharArray();
//            var inSingleQuote = false;
//            var inDoubleQuote = false;
//            for (var index = 0; index < parmChars.Length; index++)
//            {
//                if (parmChars[index] == '"' && !inSingleQuote)
//                {
//                    inDoubleQuote = !inDoubleQuote;
//                    parmChars[index] = '\n';
//                }
//                if (parmChars[index] == '\'' && !inDoubleQuote)
//                {
//                    inSingleQuote = !inSingleQuote;
//                    parmChars[index] = '\n';
//                }
//                if (!inSingleQuote && !inDoubleQuote && parmChars[index] == ' ')
//                    parmChars[index] = '\n';
//            }
//            return (new string(parmChars)).Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
//        }
//        public static string[] ParseArguments(string commandLine)
//        {
//            char[] parmChars = commandLine.ToCharArray();
//            bool inQuote = false;
//            for (int index = 0; index < parmChars.Length; index++)
//            {
//                if (parmChars[index] == '"')
//                    inQuote = !inQuote;
//                if (!inQuote && parmChars[index] == ' ')
//                    parmChars[index] = '\n';
//            }
//            return (new string(parmChars)).Split('\n');
//        }
//        public bool UserIsGroupLeader(GuildConfig guildconfig)
//        {
//            try
//            {
//                if (UserHasRole(guildconfig.ChannelManagerRoleID)
//                    || UserHasRole(guildconfig.DirectorRoleID)
//                    || UserHasRole(guildconfig.GroupManagerRoleID)
//                    || UserHasRole(guildconfig.VoiceManagerRoleID)) return true;
//            }
//            catch (Exception ex)
//            {
//                Utilities.Log(MethodBase.GetCurrentMethod(), "Failure checking roles.", ex, LogSeverity.Warning);
//            }
//            //Utilities.Log(MethodBase.GetCurrentMethod(), "F",LogSeverity.Warning);
//            return false;
//        }
//        public bool UserHasRole(ulong roleId)
//        {
//            var user = (SocketGuildUser)Context.User;
//            foreach (SocketRole role in user.Roles)
//            {
//                // Utilities.Log(MethodBase.GetCurrentMethod(), role.Id + " -- " + roleId);
//                if (role.Id == roleId) return true;
//            }
//            return false;
//        }
//        private List<SocketRole> RolesFromName(string targetRoleName)
//        {
//            var result = from r in Context.Guild.Roles
//                         where r.Name == targetRoleName
//                         select r;

//            return result.ToList();
//        }
//        public SocketRole RoleFromID(ulong roleId) => Context.Guild.Roles.FirstOrDefault(r => r.Id == roleId);
//        /// <summary>
//        /// Returns either SocketTextChannel, SocketVoiceChannel, or null.
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        public string GetChannelType(ulong id)
//        {
//            var channel = Context.Guild.GetTextChannel(id);
//            if (!Equals(channel, null)) return "Text";
//            var channel2 = Context.Guild.GetVoiceChannel(id);
//            if (!Equals(channel2, null)) return "Voice";
//            return "";
//        }
//    }
//}