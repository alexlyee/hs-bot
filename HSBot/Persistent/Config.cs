using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Discord.WebSocket;
using HSBot.Helpers;
using HSBot.Entities;
using System.Reflection;
using Discord;
using System.Diagnostics;

namespace HSBot.Persistent
{
    /// <summary>
    /// Bot configuration referencepoint.
    /// </summary>
    public static class Config
    {
        internal static BotConfig BotConfig;

        private static readonly string ConfigFile = "config.json";

        static Config()
        {
            if (DataStorage.LocalFileExists(ConfigFile))
            {
                BotConfig = DataStorage.RestoreObject<BotConfig>(ConfigFile);
                if (BotConfig.Equals(null))
                {
                    Process.Start(@"cmd.exe ", @"/c """ + (DataStorage.GetFileStream(ConfigFile).Name) + @"""");
                    Utilities.Log("Settings error", "Could not read the Settings, is your file incomplete? ", LogSeverity.Critical);
                }
                BotConfig = DataStorage.RestoreObject<BotConfig>(ConfigFile);
                SaveSettings();
                Utilities.Log("Config", "Configuration file restored and saved.");
            }
            else
            {
                BotConfig = new BotConfig();
                var file = SaveSettings();
                Process.Start(file.Name);
                Utilities.Log("Config", "Configuration file created. Please fill out the data.", Discord.LogSeverity.Critical);
            }

            foreach (var field in typeof(BotConfig).GetFields(BindingFlags.Instance |
                                                              BindingFlags.NonPublic |
                                                              BindingFlags.Public))
            {
                string fieldName = Utilities.GetBetween(field.Name, "<", ">");
                var fieldValue = field.GetValue(BotConfig);
                if (fieldValue == null)
                    Utilities.Log("Config", fieldName + " is empty, review your config if this is not the intention", LogSeverity.Warning);
                else if (fieldValue as string == "")
                    Utilities.Log("Config", fieldName + " is empty, review your config if this is not the intention", LogSeverity.Verbose);
                else Utilities.Log("Config", fieldName + " is currently " + fieldValue, LogSeverity.Verbose);
            }
        }

        private static FileStream SaveSettings()
        {
            var file = DataStorage.StoreObject(BotConfig, ConfigFile, true, true);
            if (file != null) return file;
            Process.Start(@"cmd.exe ", @"/c """ + (DataStorage.GetFileStream(ConfigFile).Name) + @"""");
            Utilities.Log("Settings error", "Could not save the Settings, is your file incomplete?",
                LogSeverity.Critical);
            return null;
        }
        
    }
}
