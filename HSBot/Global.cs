using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Discord.WebSocket;
using HSBot.Helpers;

namespace HSBot
{
    internal static class Global
    {
        internal static DiscordSocketClient Client { get; set; }
        internal static Random R { get; set; } = new Random();
        internal static ulong MessageIdToTrack { get; set; }

        // Global Helper methods
        static Global()
        {
            Utilities.Log("Global", "Global client created", Discord.LogSeverity.Verbose);
        }
    }
}
