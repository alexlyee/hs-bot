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
        private static Timer _loopingTimer;

        private static async void OnTimerTicked(object args, ElapsedEventArgs e)
        {
            await Utilities.Log("Timer Event", "Tick! " + Global.Client.ConnectionState);
            await Global.Client.SetGameAsync(Config.Config.Playing);
            await Global.Client.SetStatusAsync(Config.Config.Status);
            foreach (GuildConfig c in GuildsData.GetConfigs())
            {
                SocketTextChannel channel = Global.Client.GetGuild(c.Id).GetTextChannel(c.LogChannelId);
            }

        }

        internal static Task StartTimer()
        {
            try
            {
                _loopingTimer = new Timer()
                {
                    Interval = Config.Config.UpdateRate,
                    AutoReset = true,
                    Enabled = true
                };
                _loopingTimer.Elapsed += OnTimerTicked;
            }
            catch (Exception ex)
            {
                Utilities.Log("CoreLoop.StartTimer", "Failure in initializing timer", ex);
            }
            return Task.CompletedTask;
        }
    }
}
