using Discord.WebSocket;
using SchoolDiscordBot.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SchoolDiscordBot.Persistent
{
    public static class UserAccounts
    {
        private static List<UserAccount> accounts;

        private static string accountsFile = "accounts.json";


        static UserAccounts()
        {
            if (DataStorage.LocalFileExists(accountsFile))
            {
                accounts = DataStorage.LoadEnumeratedObject<UserAccount>(accountsFile).ToList();
            }
            else
            {
                accounts = new List<UserAccount>();
                SaveAccounts();
            }
        }

        public static void SaveAccounts()
        {
            DataStorage.SaveEnumeratedObject<UserAccount>(accounts, accountsFile, true);
        }

        public static UserAccount GetAccount(SocketUser user)
        {
            return GetOrCreateAccount(user.Id);
        }
        
        private static UserAccount GetOrCreateAccount(ulong id)
        {
            var result = from a in accounts
                         where a.ID == id
                         select a;

            var account = result.FirstOrDefault();
            if (account == null) account = CreateUserAccount(id);
            return account;
        }

        private static UserAccount CreateUserAccount(ulong id)
        {
            var newAccount = new UserAccount()
            {
                ID = id,
                Points = 0,
                XP = 0
            };

            accounts.Add(newAccount);
            SaveAccounts();
            return newAccount;
        }
    }
}
