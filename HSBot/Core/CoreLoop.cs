using System;
using System.Threading.Tasks;
using HSBot.Helpers;
using System.Timers;
using HSBot.Persistent;
using Discord.WebSocket;
using HSBot.Entities;

namespace HSBot.Core
{
    internal static class CoreLoop
    {
        private static Timer loopingTimer;

        private static async void OnTimerTicked(object args, ElapsedEventArgs e)
        {
            await Utilities.Log("Timer Event", "Tick! " + Global.Client.ConnectionState);
            await Global.Client.SetGameAsync(Config.config.Playing);
            await Global.Client.SetStatusAsync(Config.config.Status);
            foreach (GuildConfig c in GuildsData.GetConfigs())
            {
                SocketTextChannel channel = Global.Client.GetGuild(c.Id).GetTextChannel(c.LogChannelID);
            }

        }

        internal static Task StartTimer()
        {
            try
            {
                loopingTimer = new Timer()
                {
                    Interval = Config.config.UpdateRate,
                    AutoReset = true,
                    Enabled = true
                };
                loopingTimer.Elapsed += OnTimerTicked;
            }
            catch (Exception ex)
            {
                Utilities.Log("CoreLoop.StartTimer", "Failure in initializing timer", ex);
            }
            return Task.CompletedTask;
        }
    }
}
