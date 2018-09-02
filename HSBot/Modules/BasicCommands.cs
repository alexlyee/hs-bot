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
using NReco;

namespace HSBot.Modules
{
    public sealed class BasicCommands : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task Help()
        {
            // spend time here developing system to display all the commands properly and allow the user to specify depth and detail.

     
        }

        [Command("schoolsettings")]
        public async Task settings([Remainder]string message)
        {
            var embed = new EmbedBuilder();
            var firstWord = message.IndexOf(" ") > -1
                ? message.Substring(0, message.IndexOf(" "))
                : message;
            switch (firstWord)
            {
                case "View":
                    embed.WithTitle("**Config.json**")
                        .WithDescription($"The configuration for {Context.Guild.Name} :smiley:")
                        .WithColor(new Color(60, 176, 222))
                        .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png")
                        .AddField("LogChannelID", GuildsData.FindOrCreateGuildConfig(Context.Guild).LogChannelID)
                        .AddField("Prefix", GuildsData.FindOrCreateGuildConfig(Context.Guild).Prefix)
                        .AddField("Id", GuildsData.FindOrCreateGuildConfig(Context.Guild).Id)
                        .AddField("ActivityChannelID", GuildsData.FindOrCreateGuildConfig(Context.Guild).ActivityChannelID)
                        .AddField("DirectorRoleID", GuildsData.FindOrCreateGuildConfig(Context.Guild).DirectorRoleID)
                        .AddField("ManagerRoleID", GuildsData.FindOrCreateGuildConfig(Context.Guild).ManagerRoleID)
                        .AddField("ContriverRoleID", GuildsData.FindOrCreateGuildConfig(Context.Guild).ContriverRoleID)
                        .AddField("JudgeRoleID", GuildsData.FindOrCreateGuildConfig(Context.Guild).JudgeRoleID)
                        .AddField("AdministratorRoleID", GuildsData.FindOrCreateGuildConfig(Context.Guild).AdministratorRoleID)
                        .AddField("ModeratorRoleID", GuildsData.FindOrCreateGuildConfig(Context.Guild).ModeratorRoleID)
                        .AddField("GraduatedRoleID", GuildsData.FindOrCreateGuildConfig(Context.Guild).GraduatedRoleID)
                        .AddField("StudentRoleID", GuildsData.FindOrCreateGuildConfig(Context.Guild).StudentRoleID)
                        .AddField("VisitorRoleID", GuildsData.FindOrCreateGuildConfig(Context.Guild).VisitorRoleID)
                        .AddField("ChannelManagerRoleID", GuildsData.FindOrCreateGuildConfig(Context.Guild).ChannelManagerRoleID)
                        .AddField("WebhookManagerRoleID", GuildsData.FindOrCreateGuildConfig(Context.Guild).WebhookManagerRoleID)
                        .AddField("GroupManagerRoleID", GuildsData.FindOrCreateGuildConfig(Context.Guild).GroupManagerRoleID)
                        .AddField("VoiceManagerRoleID", GuildsData.FindOrCreateGuildConfig(Context.Guild).VoiceManagerRoleID)
                        .AddField("TimeCreated", GuildsData.FindOrCreateGuildConfig(Context.Guild).TimeCreated);
                    await Context.Channel.SendMessageAsync("", false, embed);
                    break;
                case "Modify":
                    if (Context.User.Id == Context.Guild.OwnerId)
                    {
                        string scan = message.Substring(message.IndexOf(" "));
                        string[] vars = scan.Split('=');
                        try
                        {
                            PropertyInfo tochange = GuildsData.FindOrCreateGuildConfig(Context.Guild).GetType().GetProperty(vars[0]);
                            tochange.SetValue(vars[1], Convert.ChangeType(vars[1], tochange.PropertyType), null);
                            await SendClassicEmbed("**Config.json**", $"Set {vars[0]} to {vars[1]}.");
                        }
                        catch (Exception ex)
                        {
                            await SendClassicEmbed("**Syntax error**", "Check for spaces and a proper value=this format. :smiley:");
                            Utilities.Log(MethodBase.GetCurrentMethod(), $"Error changing GuildData. {vars[0]} = {vars[1]}", ex, LogSeverity.Error);
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
                    await SendClassicEmbed("Syntax problem", "Choose to View, Modify, or Reset! :smiley:");
                    return;
            }
        }

        [Command("getroleid")]
        public async Task getroleid([Remainder]string message)
        {
            string id = "";
            var embed = new EmbedBuilder();
            try
            {
                id = RoleFromName((SocketGuildUser)Context.User, message).Id.ToString();
            }
            catch
            {
                embed.WithTitle("Couldn't find " + message + ", try to be exact!")
                    .WithColor(new Color(255, 0, 0));
                await Context.Channel.SendMessageAsync("", embed: embed);
                return;
            }
            await SendClassicEmbed("**Got it!**", id);
        }

        [Command("getroleids")]
        public async Task getroleids()
        {
            var embed = new EmbedBuilder();
            int count = 0;
            embed.WithTitle("**Captured Roles in " + Context.Guild.Name + "**")
                .WithColor(new Color(60, 176, 222));
            foreach (SocketRole role in Context.Guild.Roles)
            {
                embed.AddField(role.Name, role.Id.ToString());
                if (IsDivisible(count, 24))
                {
                    await Context.Channel.SendMessageAsync("", embed: embed);
                    embed = new EmbedBuilder();
                    embed.WithTitle("Captured Roles in " + Context.Guild.Name)
                        .WithColor(new Color(60, 176, 222));
                }
                count++;
            }
            embed.WithFooter(count + " roles in this server.", "https://i.imgur.com/HAI5vMj.png");
            await Context.Channel.SendMessageAsync("", embed: embed);
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
                .AddInlineField("Name", name)
                .WithDescription($"{gender}, from {nationality} in {dob.TrimEnd()}.")
                .AddInlineField("Location", location)
                .WithAuthor(Context.User)
                .AddInlineField("Login", $"Username : {dataObject.results[0].login.username.ToString()} \nPassword : {dataObject.results[0].login.password.ToString()}")
                .AddInlineField("Phone", $"Home {phone}\nCell {cell}")
                .WithFooter(dataObject.results[0].email.ToString() + " -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png");

            await Context.Channel.SendMessageAsync("", embed: embed);
        }

        [Command("purge")]
        [Cooldown(3600)]
        public async Task Purge([Remainder]string message)
        {

        }

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
            string r = Utilities.GetFormattedAlert("WELCOME_&NAME", Context.User.Username);
            await SendClassicEmbed("Sent by " + Context.User.Mention, r);
        }

        public async Task SendClassicEmbed(string title, string desc)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle(title)
                .WithDescription(desc)
                .WithColor(new Color(60, 176, 222))
                .WithFooter(" -Alex https://discord.gg/DVSjvGa", "https://i.imgur.com/HAI5vMj.png");
            await Context.Channel.SendMessageAsync("", false, embed);
        }
        private bool IsDivisible(int x, int n)
        {
            return (x % n) == 0;
        }
        private SocketRole RoleFromName(SocketGuildUser user, string targetRoleName)
        {
            var result = from r in user.Guild.Roles
                         where r.Name == targetRoleName
                         select r;

            return result.FirstOrDefault();
        }
    }
}
