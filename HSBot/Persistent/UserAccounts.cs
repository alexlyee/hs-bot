using Discord.WebSocket;
using HSBot.Entities;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Transactions;
using Discord;
using HSBot.Helpers;
using HSBot.Persistent.Implementations;

namespace HSBot.Persistent
{
    /// <summary>
    /// Methods for each UserAccount.
    /// </summary>
    public static class UserAccounts
    {
        private static readonly List<UserAccount> Accounts = new List<UserAccount>();

        public const string UsersFolder = "users";

        static UserAccounts()
        {
            if (DataStorage.LocalFolderExists(UsersFolder, true))
            {
                foreach (string file in Directory.GetFiles(UsersFolder))
                {
                    Utilities.Log("UserAccounts", file);
                    var newAccount = new UserAccount(GetAccountFromFile(file).ID, new InMemoryStorage());
                    Accounts.Add(newAccount);
                }
                Utilities.Log("UserAccounts", "User accounts initlized.");
            }
            else
            {
                Utilities.Log("UserAccounts", "There are no user accounts created yet! Created folder.");
            }
        }

        public static UserAccount CreateUserAccount(ulong id, bool _override = false)
        {
            if (GetAccount(id).Equals(null))
            {
                switch (_override)
                {
                    case false:
                        Utilities.Log(MethodBase.GetCurrentMethod(),
                            "User attempted to be created when the user already exists.", LogSeverity.Warning);
                        return GetAccount(id);
                    case true:
                        Utilities.Log(MethodBase.GetCurrentMethod(), $"Overwriting user [{id}].", LogSeverity.Verbose);
                        break;
                }
            }
            var newAccount = new UserAccount(id, new InMemoryStorage())
            {
                Points = 0,
                Xp = 0
            };
            
            Accounts.Add(newAccount);
            return newAccount;
        }

        public static UserAccount GetAccountFromFile(ulong id)
        {
            return GetAccountFromFile($"{UsersFolder}/{id}.json");
        }

        public static UserAccount GetAccount(ulong id, bool createifempty = false)
        {
            var result = from a in Accounts
                         where a.ID == id
                         select a;

            var account = result.FirstOrDefault();
            if (account == null && createifempty) account = CreateUserAccount(id);
            return account;
        }

        private static UserAccount GetAccountFromFile(string fullpath)
        {
            return DataStorage.RestoreObject<UserAccount>(fullpath);
        }
    }
}
