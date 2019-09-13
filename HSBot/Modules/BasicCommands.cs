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
using HSBot.Modules.Preconditions;
using System.Collections.Concurrent;
using HSBot.Helpers;
using System.Reflection;
using HSBot.Modules.References;
using System.Net;
using Newtonsoft.Json;
using HSBot.Entities;
using System.Drawing;
using System.ComponentModel;

/* Ideas:
 * Graph for distrobution of set of roles.
 * Select groups of users by role, count number with certain role/attribute.
 * List all users by role with detailed information (all attributes)
 * Commands for /select, like give role and tie in with class tool.
 * This will have to implement a sort of user-cache.
 */

namespace HSBot.Modules
{
    public sealed partial class BasicCommands : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task Help()
        {
            // spend time here developing system to display all the commands properly and allow the user to specify depth and detail.
            await SendClassicEmbed("**Thanks for showing interest in our bot!**", "All of the official documentation is on our wiki! *and if" +
                " you're interested in developing with us check out the rest of the github!* :smiley: https://github.com/alexlyee/hs-bot/wiki");
            
        }

        [Command("schoolsettings")]
        public async Task Settings([Remainder]string message)
        {
            _ = new EmbedBuilder();
            var firstWord = message.IndexOf(" ") > -1
                ? message.Substring(0, message.IndexOf(" "))
                : message;
            string configfile = $"{GuildsData.GuildsFolder}/{Context.Guild.Id}/config.json";
            GuildConfig config = DataStorage.RestoreObject<GuildConfig>(configfile);
            switch (firstWord)
            {
                case "View":
                    await SendClassEmbed<GuildConfig>("config.json", $"Configuration for {Context.Guild.Name} :smiley:", config);
                    DataStorage.StoreObject(config, configfile);
                    break;
                case "Modify":
                    if (Context.User.Id == Context.Guild.OwnerId || UserHasRole(config.DirectorRoleID))
                    {
                        string scan = message.Substring(message.IndexOf(" ") + 1);
                        string[] vars = scan.Split('=');
                        try
                        {
                            PropertyInfo tochange = GuildsData.FindOrCreateGuildConfig(Context.Guild).GetType().GetProperty(vars[0]);
                            if (tochange.PropertyType.IsEquivalentTo(typeof(string)))
                                tochange.SetValue(vars[1], tochange.GetValue(config, null), null);
                            else if (tochange.PropertyType.IsEquivalentTo(typeof(ulong)))
                            {
                                ulong converted;
                                try
                                {
                                    converted = (UInt64)Parse<ulong>(vars[1]);
                                }
                                catch
                                {
                                    await SendErrorEmbed("**Syntax error**", $"Failure to convert {vars[1]} to a long number.");
                                    break;
                                }
                                tochange.SetValue(converted, tochange.GetValue(config, null), null);
                            }
                        


                            await SendClassicEmbed("**Config.json**", $"Set {vars[0]} to {vars[1]}.");
                        }
                        catch (Exception ex)
                        {
                            await SendErrorEmbed("**Syntax error**", "Check for spaces and a proper value=this format. :smiley:");
                            await Utilities.Log(MethodBase.GetCurrentMethod(), $"Error changing GuildData. {vars[0]} = {vars[1]}", ex, LogSeverity.Error);
                        }
                    }
                    else await SendClassicEmbed("**Only an owner can use this command**", "");
                    break;
                case "Reset":
                    if (Context.User.Id == Context.Guild.OwnerId)
                    {
                        GuildsData.DeleteGuildConfig(Context.Guild.Id);
                        await SendClassicEmbed("**Success!", "To confirm, use View. :smiley:");
                    }
                    else await SendClassicEmbed("**Only an owner can use this command**", "");
                    break;
                default:
                    await SendErrorEmbed("Syntax problem", "Choose to View, Modify, or Reset! :smiley:");
                    return;
            }
        }

        [Command("getroleid")]
        public async Task getroleid([Remainder]string message)
        {
            var embed = new EmbedBuilder();
            try
            {
                List<SocketRole> roles = RolesFromName(message);
                if (roles.Count == 1)
                    await SendClassicEmbed("**Got it!**", roles.FirstOrDefault().Id.ToString());
                if (roles.Count > 1)
                {
                    int count = 0;
                    embed.WithTitle("**Captured Roles with name " + message + "**")
                        .WithColor(new Discord.Color(60, 176, 222));
                    foreach (SocketRole role in roles)
                    {
                        embed.AddField(role.Name, role.Id.ToString());
                        if (IsDivisible(count, 24))
                        {
                            await Context.Channel.SendMessageAsync("", embed: embed.Build());
                            embed = new EmbedBuilder();
                            embed.WithTitle("Captured Roles with name " + message)
                                .WithColor(new Discord.Color(60, 176, 222));
                        }
                        count++;
                    }
                    embed.WithFooter(count + " roles with given name.", "https://i.imgur.com/HAI5vMj.png");
                    await Context.Channel.SendMessageAsync("", embed: embed.Build());
                }

            }
            catch
            {
                embed.WithTitle("Couldn't find " + message + ", try to be exact!")
                    .WithColor(new Discord.Color(255, 0, 0));
                await Context.Channel.SendMessageAsync("", embed: embed.Build());
                return;
            }
        }

        [Command("getroleids")]
        public async Task getroleids()
        {
            var embed = new EmbedBuilder();
            int count = 0;
            embed.WithTitle("**Captured Roles in " + Context.Guild.Name + "**")
                .WithColor(new Discord.Color(60, 176, 222));
            foreach (SocketRole role in Context.Guild.Roles)
            {
                embed.AddField(role.Name, role.Id.ToString());
                if (IsDivisible(count, 24))
                {
                    await Context.Channel.SendMessageAsync("", embed: embed.Build());
                    embed = new EmbedBuilder();
                    embed.WithTitle("Captured Roles in " + Context.Guild.Name)
                        .WithColor(new Discord.Color(60, 176, 222));
                }
                count++;
            }
            embed.WithFooter(count + " roles in this server.", "https://i.imgur.com/HAI5vMj.png");
            await Context.Channel.SendMessageAsync("", embed: embed.Build());
        }

        [Command("hello")]
        public async Task Hello()
        {
            string html = File.ReadAllText("Modules/References/hello.html");
            await Context.Channel.SendMessageAsync(html);
            html = string.Format(html, Context.User.Username);
            var api = new HtmlToPdfOrImage.Api("99433eae-569c-470d-a763-0a3d0b28ad21", "vmZ2Qryg");
            var format = new HtmlToPdfOrImage.GenerateSettings()
            {
                OutputType = HtmlToPdfOrImage.OutputType.Image
            };
            var result = api.Convert(html, format);
            await Context.Channel.SendFileAsync(new MemoryStream((byte[])result.model), "hello.png");
        }

        [Command("person")]
        [Cooldown(10)]
        public async Task Person()
        {
            EmbedBuilder embed = new EmbedBuilder();
            string json = "";
            using (WebClient client = new WebClient())
            {
                json = client.DownloadString("https://randomuser.me/api/1.1/");
            }
            var dataObject = JsonConvert.DeserializeObject<dynamic>(json);

            string gender = dataObject.results[0].gender.ToString();
            string portrait = dataObject.results[0].picture.large.ToString();
            string name = $"{dataObject.results[0].name.title.ToString()}. {dataObject.results[0].name.first.ToString()} {dataObject.results[0].name.last.ToString()}";
            string nationality = dataObject.results[0].nat.ToString();
            var locationinfo = dataObject.results[0].location;
            string street = locationinfo.street.ToString();
            string city = locationinfo.city.ToString();
            string state = locationinfo.state.ToString();
            string postcode = locationinfo.postcode.ToString();
            string location = $"{street}, {city} {postcode}, {state}.";
            string dob = dataObject.results[0].dob.ToString();
            string phone = dataObject.results[0].phone.ToString();
            string cell = dataObject.results[0].cell.ToString();

            embed.WithThumbnailUrl(portrait)
                .WithTitle("Random person: ")
                .AddField("Name", name, true)
                .WithDescription($"{gender}, from {nationality} in {dob.TrimEnd()}.")
                .AddField("Location", location, true)
                .WithAuthor(Context.User)
                .AddField("Login", $"Username : {dataObject.results[0].login.username.ToString()} \nPassword : {dataObject.results[0].login.password.ToString()}", true)
                .AddField("Phone", $"Home {phone}\nCell {cell}", true)
                .WithFooter(dataObject.results[0].email.ToString() + " -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png");

            await Context.Channel.SendMessageAsync("", embed: embed.Build());
        }

        /*
        [Command("purge")]
        [Cooldown(3600)]
        public async Task Purge([Remainder]string message)
        {

        }
        */

        [Command("8ball"), Remarks("answers any question with a stunning level of wiseness")]
        public async Task EightBall([Remainder]string message)
        {
            EightBall ball = new EightBall();
            await Context.Channel.SendMessageAsync(ball.GrabRandomAnswer());
        }

        [Command("ping")]
        public async Task Ping()
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle($":ping_pong:  Pong!")
                .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png");
            await ReplyAsync("", false, builder.Build());
        }

        [Command("echo")]
        public async Task Echo([Remainder]string message)
        {
            // string r = Utilities.GetFormattedAlert("WELCOME_&NAME", Context.User.Username);
            await SendClassicEmbed("Sent by " + Context.User.Username, message);
        }

    }

    // Methods
    public sealed partial class BasicCommands : ModuleBase<SocketCommandContext>
    {
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
                .WithColor(new Discord.Color(60, 176, 222))
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
                Color = new Discord.Color(33, 160, 52),
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
                Color = new Discord.Color(219, 57, 75),
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
                Color = new Discord.Color(60, 176, 222),
                Footer = new EmbedFooterBuilder()
                    .WithText(" -Alex https://discord.gg/DVSjvGa")
                    .WithIconUrl("https://i.imgur.com/HAI5vMj.png"),
                Timestamp = DateTime.UtcNow,
            }.Build();
            await Context.Channel.SendMessageAsync("", false, embed);
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
            //Utilities.Log(MethodBase.GetCurrentMethod(), "F", LogSeverity.Warning);
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
        private bool IsDivisible(int x, int n)
        {
            return (x % n) == 0;
        }
        private List<SocketRole> RolesFromName(string targetRoleName)
        {
            var result = from r in Context.Guild.Roles
                         where r.Name == targetRoleName
                         select r;

            return result.ToList();
        }
        public static T Parse<T>(string value)
        {
            // or ConvertFromInvariantString for serialization.
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value);
        }
        public object GetListOfT(Type T) => Activator.CreateInstance(typeof(List<>).MakeGenericType(T));
    }

}
