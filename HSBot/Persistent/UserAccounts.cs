using Discord.WebSocket;
using HSBot.Entities;
using System.Collections.Generic;
using System.Linq;
using HSBot.Persistent.Implementations;

namespace HSBot.Persistent
{
    /// <summary>
    /// Methods for each UserAccount.
    /// </summary>
    public static class UserAccounts
    {
        private static List<UserAccount> _accounts;

        private static string _accountsFile = "accounts.json";


        static UserAccounts()
        {
            if (DataStorage.LocalFileExists(_accountsFile))
            {
                _accounts = DataStorage.LoadEnumeratedObject<UserAccount>(_accountsFile).ToList();
            }
            else
            {
                _accounts = new List<UserAccount>();
                SaveAccounts();
            }
        }

        public static void SaveAccounts()
        {
            DataStorage.SaveEnumeratedObject<UserAccount>(_accounts, _accountsFile, true);
        }

        public static UserAccount GetAccount(SocketUser user)
        {
            return GetOrCreateAccount(user.Id);
        }
        
        private static UserAccount GetOrCreateAccount(ulong id)
        {
            var result = from a in _accounts
                         where a.ID == id
                         select a;

            var account = result.FirstOrDefault();
            if (account == null) account = CreateUserAccount(id);
            return account;
        }

        private static UserAccount CreateUserAccount(ulong id)
        {
            var newAccount = new UserAccount(new InMemoryStorage())
            {
                ID = id,
                Points = 0,
                Xp = 0
            };
            
            _accounts.Add(newAccount);
            SaveAccounts();
            return newAccount;
        }
    }
}
