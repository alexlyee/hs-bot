using Discord.WebSocket;
using HSBot.Entities;
using HSBot.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace HSBot.Persistent
{
    /// <summary>
    /// Referencepoint for Guild accounts.
    /// </summary>
    public static class GuildsData
    {
        private static List<GuildConfig> _guilds;
        private static GuildsDataStruct _guildGlobalSettings;

        public const string ConfigFile = "guilds.json";
        public const string GuildsFolder = "guilds";

        static GuildsData()
        {
            _guilds = new List<GuildConfig>();
            _guildGlobalSettings = new GuildsDataStruct();
            if (DataStorage.LocalFileExists(ConfigFile))
            {
                _guildGlobalSettings = DataStorage.RestoreObject<GuildsDataStruct>(ConfigFile);
                Utilities.Log("GuildData", "Global guild configuration file restored.");
            }
            else
            {
                _guildGlobalSettings.UseNiceFormatting = true;
                DataStorage.StoreObject(_guildGlobalSettings, ConfigFile, _guildGlobalSettings.UseNiceFormatting);
                Utilities.Log("GuildsData", "GlobalGuildSettings created and saved.");
                _guildGlobalSettings = DataStorage.RestoreObject<GuildsDataStruct>(ConfigFile);
                Utilities.Log("GuildsData", "GlobalGuildSettings restored.");
            }
            if (!(DataStorage.LocalFolderExists(GuildsFolder, true))) Utilities.Log("GuildData", GuildsFolder + " folder created.");
            Utilities.Log("GuildsData", "Writing Guilds...", Discord.LogSeverity.Verbose);
            SyncGuilds();
        }

        internal static GuildConfig[] GetConfigs()
        {
            Utilities.Log("GuildsData", "Getting Guild Configurations...", Discord.LogSeverity.Debug);
            return _guilds.ToArray();
        }

        private static void SyncGuilds()
        {
            try
            {
                int guildcount = 0;
                foreach (Discord.WebSocket.SocketGuild guild in Global.Client.Guilds)
                {
                    if (!(DataStorage.LocalFileExists(GuildsFolder + "/" + guild.Id + ".json")))
                    {
                        InitializeGuild(guild);
                        Utilities.Log("GuildData.SyncGuilds", "Guild" + guild.Name + " saved.", Discord.LogSeverity.Verbose);
                        guildcount++;
                    }
                }
                Utilities.Log("GuildData.SyncGuilds", guildcount + "/" + Global.Client.Guilds.Count + " Guild(s) Initialized.");
            }
            catch (Exception ex)
            {
                Utilities.Log("GuildData.SyncGuilds", "Error in initializing guilds.", ex);
            }
            Utilities.Log("GuildsData.SyncGuils", "Reastoring Guilds...", Discord.LogSeverity.Verbose);
            ReinitializeGuilds();
        }

        private static void ReinitializeGuilds()
        {
            string[] files = DataStorage.GetFilesInFolder(GuildsFolder);
            try
            {
                int guildcount = 0;
                foreach (string file in files)
                {
                    LoadGuild(file);
                    Utilities.Log("GuildData.ReinitializeGuilds", file + " Loaded.", Discord.LogSeverity.Verbose);
                    guildcount++;
                }
                Utilities.Log("GuildData.ReinitializeGuilds", guildcount + "/" + Global.Client.Guilds.Count + " Guild(s) reinitialized.");
            }
            catch (Exception ex)
            {
                Utilities.Log("GuildData.ReinitializeGuilds", "Error loading guilds.", ex);
            }
        }

        internal static ActionResult LoadGuild(string file)
        {
            var result = new ActionResult();
            GuildConfig config = DataStorage.RestoreObject<GuildConfig>(GuildsFolder + "/" + file);
            _guilds.Remove(FindGuildConfig(config.Id));
            _guilds.Add(config);
            return result;
        }

        internal static ActionResult InitializeGuild(Discord.WebSocket.SocketGuild guild)
        {
            var result = new ActionResult();
            var guildconfig = new GuildConfig()
            {
                Prefix = "!",
                Id = guild.Id,
                TimeCreated = DateTime.Now.ToString()
            };
            _guilds.Add(guildconfig);
            DataStorage.StoreObject(guildconfig, $"{GuildsFolder}/{guildconfig.Id}.json", _guildGlobalSettings.UseNiceFormatting);
            Utilities.Log("GuildsData.InitializeGuild", guild.Name + "Initialized in GuildDataSystem.");
            return result;
        }

        internal static ActionResult RemoveGuild(Discord.WebSocket.SocketGuild guild)
        {
            var result = new ActionResult();
            _guilds.Remove(_guilds.Find(g => g.Id == guild.Id));
            DataStorage.DeleteFile(GuildsFolder + guild.Id + ".json");
            Utilities.Log("GuildsData.RemoveGuild", guild.Name + "Removed from GuildDataSystem.");
            return result;
        }

        internal static ActionResult StoreGuild(ulong id)
        {
            var result = new ActionResult();
            try
            {
                var selection = FindGuildConfig(id);
                DataStorage.StoreObject(selection, GuildsFolder + "/" + id + ".json", _guildGlobalSettings.UseNiceFormatting);
            }
            catch
            {
                result.AddAlert(new Alert("SaveGuild", "Could not save guild", LevelEnum.Exception));
            }

            return result;
        }

        public static GuildConfig FindOrCreateGuildConfig(Discord.WebSocket.SocketGuild guild)
        {
            GuildConfig GuildConfig = _guilds.Find(g => g.Id == guild.Id);
            if (GuildConfig.Equals(null))
            {
                InitializeGuild(guild);
            }
            GuildConfig = _guilds.Find(g => g.Id == guild.Id);
            Utilities.Log("FindOrCreateGuildConfig", $"Guild found with id {guild.Id}", Discord.LogSeverity.Debug);
            return GuildConfig;
        }
        
        /// <summary>
        /// Not failsafe. Perferably use FindOrCreateGuildConfig
        /// </summary>
        /// <returns>Guild Config</returns>
        internal static GuildConfig FindGuildConfig(ulong id)
        {
            GuildConfig guild = GuildsData._guilds.Find(g => g.Id == id);
            Utilities.Log("FildGuildConfig", $"Guild found with id {id}, {guild.Prefix}", Discord.LogSeverity.Debug);
            return guild;
        }

    }

    internal struct GuildsDataStruct
    {
        public bool UseNiceFormatting;
    }
}
