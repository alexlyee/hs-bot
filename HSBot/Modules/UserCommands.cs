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
using SchoolDiscordBot.Helpers;

namespace SchoolDiscordBot.Modules
{
    public sealed class UserCommands : ModuleBase<SocketCommandContext>
    {

        [Command("Stats")]
        public async Task Stats([Remainder]string arg = "")
        {
            SocketUser target = null;
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();

            target = mentionedUser ?? Context.User;

            var account = UserAccounts.GetAccount(target);
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle($"{target.Username} / {account.XP} xp / {account.Points} points.")
                .WithColor(Color.Blue);
            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("addXP")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public void AddXP(uint xp)
        {
            var account = UserAccounts.GetAccount(Context.User);
            account.XP += xp;
            UserAccounts.SaveAccounts();
        }


                /* Depricated.
                 * 
        [Command("profile")]
        public async Task Profile()
        {
            var html = String.Format("<body>Hello world: {0}</body>", DateTime.Now);
            var htmlToImageConv = new NReco.ImageGenerator.HtmlToImageConverter();
            var jpegBytes = htmlToImageConv.GenerateImage(html, NReco.ImageGenerator.ImageFormat.Jpeg);
            await Context.Channel.SendFileAsync(new MemoryStream(jpegBytes), "test.jpg");
        }

        [Command("poll")]
        public async Task Poll([Remainder]String message)
        {
            var emoji1 = new Emoji("✅");
            var emoji2 = new Emoji("❌");
            var emoji3 = new Emoji("🤔");
            await Context.Channel.SendMessageAsync(message);
            await Context.Message.AddReactionAsync(emoji1);
            await Context.Message.AddReactionAsync(emoji2);
            await Context.Message.AddReactionAsync(emoji3);
        }
        [Command("say")]
        public async Task Say([Remainder] string message)
        {
            await ReplyAsync(message);
        }
        [Command("o"), RequireOwner]
        public async Task O()
        {
            EmbedBuilder builder = new EmbedBuilder();
            string message = "Sent! Right?";
            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
            await dmChannel.SendMessageAsync("Hello! I am the High School bot, and this is a test!");
            builder.WithTitle("Owner Quick Command")
                .WithDescription("For quickly testing stuff.")
                .WithColor(Color.Blue)
                .AddField("Part 1", message);
            await ReplyAsync("", false, builder.Build());
        }
        [Command("myroles")]
        public async Task MyRoles()
        {
            SocketGuildUser suser = (SocketGuildUser)Context.User;
            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
            foreach (var role in suser.Roles)
            {
                EmbedBuilder rolebuilder = new EmbedBuilder();
                rolebuilder.WithTitle(role.Name)
                .WithDescription(role.Id.ToString())
                .WithColor(role.Color)
                .AddField("Position", role.Position)
                .AddField("Permissions", role.Permissions);
                await dmChannel.SendMessageAsync("", false, rolebuilder.Build());
                await Task.Delay(3000);
                rolebuilder = null;
            }
        }
        [Command("t"), RequireUserPermission(GuildPermission.ReadMessages)]
        public async Task T([Remainder] string roleSelection)
        {
            SocketGuildUser suser = (SocketGuildUser)Context.User;


            EmbedBuilder builder = new EmbedBuilder();

            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
            await dmChannel.SendMessageAsync("Hello! I am the High School bot, and this is a test!");
            bool role = UserHasRole(suser, RoleIDFromName(suser, roleSelection));

            builder.WithTitle("Test Quick Command")
                .WithDescription("You can search for a role, and it will respond true or false if you have it or not.")
                .WithColor(Color.Blue)
                .AddField("Role ID", RoleIDFromName(suser, roleSelection))
                .AddField("Role", role);

            await ReplyAsync("", false, builder.Build());
        }

    */

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
