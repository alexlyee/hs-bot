using Discord.WebSocket;
using HSBot.Entities;
using HSBot.Helpers;
using System;
using System.Collections.Generic;

namespace HSBot.Persistent
{
    internal static class GuildsData
    {
        private static List<GuildConfig> Guilds;
        private static GuildsDataStruct GuildGlobalSettings;

        private static readonly string configFile = "guilds.json";
        private static readonly string guildsFolder = "guilds";

        static GuildsData()
        {
            Guilds = new List<GuildConfig>();
            GuildGlobalSettings = new GuildsDataStruct();
            if (DataStorage.LocalFileExists(configFile))
            {
                GuildGlobalSettings = DataStorage.RestoreObject<GuildsDataStruct>(configFile);
                Utilities.Log("GuildData", "Global guild configuration file restored.");
            }
            else
            {
                GuildGlobalSettings.UseNiceFormatting = true;
                DataStorage.StoreObject(GuildGlobalSettings, configFile, GuildGlobalSettings.UseNiceFormatting);
                Utilities.Log("GuildsData", "GlobalGuildSettings created and saved.");
                GuildGlobalSettings = DataStorage.RestoreObject<GuildsDataStruct>(configFile);
                Utilities.Log("GuildsData", "GlobalGuildSettings restored.");
            }
            if (!(DataStorage.LocalFolderExists(guildsFolder, true))) Utilities.Log("GuildData", guildsFolder + " folder created.");
            Utilities.Log("GuildsData", "Writing Guilds...", Discord.LogSeverity.Verbose);
            WriteGuilds();
            Utilities.Log("GuildsData", "Reastoring Guilds...", Discord.LogSeverity.Verbose);
            RestoreGuilds();
        }

        internal static GuildConfig[] GetConfigs()
        {
            Utilities.Log("GuildsData", "Getting Guild Configurations...", Discord.LogSeverity.Debug);
            return Guilds.ToArray();
        }

        private static void WriteGuilds()
        {
            try
            {
                foreach (Discord.WebSocket.SocketGuild Guild in Global.Client.Guilds)
                {
                    if (!(DataStorage.LocalFileExists(guildsFolder + "/" + Guild.Id + ".json")))
                    {
                        InitializeGuild(Guild);
                        Utilities.Log("GuildData.WriteGuilds", "Guild" + Guild.Name + " saved.", Discord.LogSeverity.Verbose);
                    }
                }
                Utilities.Log("GuildData.WriteGuilds", Global.Client.Guilds.Count + " Guild(s) saved.");
            }
            catch (Exception ex)
            {
                Utilities.Log("GuildData.WriteGuilds", "Error in initializing guilds.", ex);
            }
        }

        private static void RestoreGuilds()
        {
            string[] files = DataStorage.GetFilesInFolder(guildsFolder);
            try
            {
                foreach (string file in files)
                {
                    LoadGuild(file);
                    Utilities.Log("GuildData.RestoreGuilds", file + " Loaded.", Discord.LogSeverity.Verbose);
                }
                Utilities.Log("GuildData.RestoreGuilds", Global.Client.Guilds.Count + " Guild(s) restored.");
            }
            catch (Exception ex)
            {
                Utilities.Log("GuildData.RestoreGuilds", "Error loading guilds.", ex);
            }
        }

        internal static ActionResult LoadGuild(string file)
        {
            var result = new ActionResult();
            GuildConfig config = DataStorage.RestoreObject<GuildConfig>(guildsFolder + "/" + file);
            Guilds.Remove(FindGuildConfig(config.Id));
            Guilds.Add(config);
            return result;
        }

        internal static ActionResult InitializeGuild(Discord.WebSocket.SocketGuild Guild)
        {
            var result = new ActionResult();
            var guildconfig = new GuildConfig()
            {
                Prefix = "!",
                Id = Guild.Id
            };
            Guilds.Add(guildconfig);
            DataStorage.StoreObject(guildconfig, $"{guildsFolder}/{guildconfig.Id}.json", GuildGlobalSettings.UseNiceFormatting);
            Utilities.Log("GuildsData.InitializeGuild", Guild.Name + "Initialized in GuildDataSystem.");
            return result;
        }

        internal static ActionResult RemoveGuild(Discord.WebSocket.SocketGuild Guild)
        {
            var result = new ActionResult();
            Guilds.Remove(Guilds.Find(g => g.Id == Guild.Id));
            DataStorage.DeleteFile(guildsFolder + Guild.Id + ".json");
            Utilities.Log("GuildsData.RemoveGuild", Guild.Name + "Removed from GuildDataSystem.");
            return result;
        }

        internal static ActionResult StoreGuild(ulong Id)
        {
            var result = new ActionResult();
            try
            {
                var Selection = FindGuildConfig(Id);
                DataStorage.StoreObject(Selection, guildsFolder + "/" + Id + ".json", GuildGlobalSettings.UseNiceFormatting);
            }
            catch
            {
                result.AddAlert(new Alert("SaveGuild", "Could not save guild", LevelEnum.Exception));
            }

            return result;
        }

        internal static GuildConfig FindOrCreateGuildConfig(Discord.WebSocket.SocketGuild Guild)
        {
            GuildConfig guild = Guilds.Find(g => g.Id == Guild.Id);
            if (guild.Equals(null))
            {
                InitializeGuild(Guild);
            }
            guild = Guilds.Find(g => g.Id == Guild.Id);
            Utilities.Log("FindOrCreateGuildConfig", $"Guild found with id {Guild.Id}", Discord.LogSeverity.Debug);
            return guild;
        }

        internal static GuildConfig FindGuildConfig(ulong Id)
        {
            GuildConfig guild = GuildsData.Guilds.Find(g => g.Id == Id);
            Utilities.Log("FildGuildConfig", $"Guild found with id {Id}, {guild.Prefix}", Discord.LogSeverity.Debug);
            return guild;
        }

    }

    internal struct GuildsDataStruct
    {
        public bool UseNiceFormatting;
    }
}
