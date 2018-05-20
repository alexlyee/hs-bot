namespace HSBot.Entities
{
    /// <summary>
    /// Represents a discord user in the bot.
    /// Extends the fields of a discord user, but shares a common Id.
    /// </summary>
    public class UserAccount
    {
        public ulong Id { get; set; }
        public uint Points { get; set; }
        public uint Xp { get; set; }
        public uint Reputation { get; set; }
    }
}
