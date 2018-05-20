using System.Xml.Linq;
using Discord;
using HSBot.Helpers;
using HSBot.Persistent;
using HSBot.Persistent.Implementations;

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

        private readonly InMemoryStorage _storage;

        public UserAccount(ulong id, IDataStorage storage)
        {
            this.ID = id;
            _storage = (InMemoryStorage)storage;
            _storage.Initialize(this, ID.ToString(), UserAccounts.UsersFolder);
            var User = _storage.RestoreObject<UserAccount>(ID.ToString());
            if (!User.Equals(null))
            {
                this.Points = User.Points;
                this.Xp = User.Xp;
                this.Reputation = User.Reputation;
                _storage.StoreObject();
                Utilities.Log($"UserAccount [{ID}]", "User restored from storage.", LogSeverity.Verbose);
            }
            else
            {
                _storage.StoreObject();
                Utilities.Log($"UserAccount [{ID}]", "New user created.", LogSeverity.Verbose);
            }
        }

        /// <summary>
        /// Save to storage.
        /// </summary>
        public void Save() => _storage.StoreObject();

        /// <summary>
        /// Update class with data from storage.
        /// </summary>
        public void Update()
        {
            var User = _storage.RestoreObject<UserAccount>(ID.ToString());
            this.Points = User.Points;
            this.Xp = User.Xp;
            this.Reputation = User.Reputation;
        }

    }
}
