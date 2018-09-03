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
using HSBot.Helpers;
using System.Reflection;

namespace HSBot.Modules
{
    public partial class UserRoleCommands : ModuleBase<SocketCommandContext>
    {

        [Command("TestRole")]
        public async Task TestRole(string rolename)
        {
            var role = Task.Run(async () => { return await Context.Guild.CreateRoleAsync(rolename); }).Result;
            var socketRoles = Context.Guild.Roles;
            SocketRole srole = RolesFromName(role.Name).FirstOrDefault();
            foreach (SocketRole r in socketRoles)
            {
                Utilities.Log(MethodBase.GetCurrentMethod(), $"{r.Name} : {r.Id} looking for {role.Id}", LogSeverity.Verbose);
                if (r.Id == role.Id)
                {
                    srole = r;
                    Utilities.Log(MethodBase.GetCurrentMethod(), $"Found it!", LogSeverity.Verbose);
                    return;
                }
                
            }

            await SendClassicEmbed($"RestRole: {role.Id}\nSocketRole: {srole.Id}", "hi");
            await role.DeleteAsync();
        }


        [Command("Stats")]
        public async Task Stats([Remainder]string arg = "")
        {
            SocketUser target = null;
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();

            target = mentionedUser ?? Context.User;

            var account = UserAccounts.GetAccount(target.Id);
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle($"{target.Username} / {account.Xp} xp / {account.Points} points.")
                .WithColor(Color.Blue);
            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("addXP")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public void AddXp(uint xp)
        {
            var account = UserAccounts.GetAccount(Context.User.Id);
            account.Xp += xp;
            account.Save();
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



}

    // Methods
    public sealed partial class UserRoleCommands : ModuleBase<SocketCommandContext>
    {
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
            }.Build();
            await Context.Channel.SendMessageAsync("", false, embed);
        }
        public async Task SendErrorEmbed(string title, string desc, Exception ex = null)
        {
            var embed = new EmbedBuilder
            {
                Title = title,
                Description = desc,
                Color = new Color(219, 57, 75),
                Footer = new EmbedFooterBuilder()
                    .WithText(" -Alex https://discord.gg/DVSjvGa")
                    .WithIconUrl("https://i.imgur.com/HAI5vMj.png"),
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
            }.Build();
            await Context.Channel.SendMessageAsync("", false, embed);
        }
        private bool UserHasRole(SocketGuildUser user, ulong roleId)
        {
            return user.Roles.Contains(user.Guild.GetRole(roleId));
        }
        private List<SocketRole> RolesFromName(string targetRoleName)
        {
            var result = from r in Context.Guild.Roles
                         where r.Name == targetRoleName
                         select r;

            return result.ToList();
        }


    }
}
