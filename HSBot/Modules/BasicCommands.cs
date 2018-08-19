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
            embed.WithTitle("Got it!")
                .WithDescription(id)
                .WithColor(new Color(60, 176, 222))
                .WithFooter(" -Alex https://discord.gg/emFQ6s4", "https://i.imgur.com/HAI5vMj.png");
            await Context.Channel.SendMessageAsync("", embed: embed);
        }

        [Command("getroleids")]
        public async Task getroleids()
        {
            var embed = new EmbedBuilder();
            int count = 0;
            embed.WithTitle("Captured Roles in " + Context.Guild.Name)
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
                .WithFooter(dataObject.results[0].email.ToString() + " -Alex https://discord.gg/emFQ6s4", "https://i.imgur.com/HAI5vMj.png");

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
                .WithFooter(" -Alex https://discord.gg/emFQ6s4", "https://i.imgur.com/HAI5vMj.png");
            await ReplyAsync("", false, builder.Build());
        }

        [Command("echo")]
        public async Task Echo([Remainder]string message)
        {
            var embed = new EmbedBuilder();
            string r = Utilities.GetFormattedAlert("WELCOME_&NAME", Context.User.Username);
            embed.WithTitle("Echoed message")
                .WithDescription(r)
                .WithColor(new Color(60, 176, 222))
                .WithFooter(" -Alex https://discord.gg/emFQ6s4", "https://i.imgur.com/HAI5vMj.png");


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
