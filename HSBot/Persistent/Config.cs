using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Discord.WebSocket;
using SchoolDiscordBot.Helpers;
using SchoolDiscordBot.Entities;
using System.Reflection;

namespace SchoolDiscordBot.Persistent
{
    internal static class Config
    {
        internal static BotConfig config;

        private static readonly string configFile = "config.json";

        static Config()
        {
            if (DataStorage.LocalFileExists(configFile))
            {
                config = DataStorage.RestoreObject<BotConfig>(configFile);
                SaveSettings();
                Utilities.Log("Config", "Configuration file restored and saved.");
            }
            else
            {
                config = new BotConfig();
                SaveSettings();
                Utilities.Log("Config", "Configuration file created. Please fill out the data.", Discord.LogSeverity.Critical);
                Console.ReadKey();
            }
        }

        private static ActionResult SaveSettings()
        {
            var result = new ActionResult();
            try
            {
                DataStorage.StoreObject(config, configFile, useIndentations: true);
            }
            catch
            {
                result.AddAlert(new Alert("Settings error", "Could not save the Settings", LevelEnum.Exception));
            }
            return result;
        }
        
    }
    

}
