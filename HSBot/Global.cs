using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Discord.WebSocket;
using SchoolDiscordBot.Helpers;

namespace SchoolDiscordBot
{
    internal static class Global
    {
        internal static DiscordSocketClient Client { get; set; }
        internal static Random R { get; set; } = new Random();

        // Global Helper methods
        static Global()
        {
            Utilities.Log("Global", "Global client created", Discord.LogSeverity.Verbose);
        }
    }
}
