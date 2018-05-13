using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Discord.WebSocket;
using HSBot.Helpers;
using HSBot.Entities;
using System.Reflection;

namespace HSBot.Persistent
{
    internal static class Config
    {
        internal static BotConfig BotConfig;

        private static readonly string ConfigFile = "config.json";

        static Config()
        {
            if (DataStorage.LocalFileExists(ConfigFile))
            {
                BotConfig = DataStorage.RestoreObject<BotConfig>(ConfigFile);
                SaveSettings();
                Utilities.Log("Config", "Configuration file restored and saved.");
            }
            else
            {
                BotConfig = new BotConfig();
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
                DataStorage.StoreObject(BotConfig, ConfigFile, useIndentations: true);
            }
            catch
            {
                result.AddAlert(new Alert("Settings error", "Could not save the Settings", LevelEnum.Exception));
            }
            return result;
        }
        
    }
    

}
