using HSBot.Persistent;

namespace HSBot.Entities
{
    /// <summary>
    /// Represents a discord user in the bot.
    /// Extends the fields of a discord user, but shares a common Id.
    /// </summary>
    public class UserAccount
    {
        public ulong ID { get; set; } // Implement discord user as ID?
        public uint Points { get; set; }
        public uint Xp { get; set; }
        public uint Reputation { get; set; }

        private readonly IDataStorage _storage;

        public UserAccount(IDataStorage storage)
        {
            _storage = storage;

        }

        public void Save()
        {
            _storage.StoreObject(this, ID.ToString());
        }
    }
}
