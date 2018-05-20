using System.Collections.Generic;
using System;

namespace HSBot.Entities
{
    public struct GuildConfig
    {
        public ulong LogChannelID { get; set; }
        public string Prefix { get; set; }
        public ulong Id { get; set; }
        public ulong Popularity { get; set; }
        public ulong ActivityChannelID { get; set; }

        public enum ManagmentIDs : ulong
        {
            Directors,
            Managers,
            Contrivers
        };
        public List<ulong> TokenIDs { get; set; }
        public enum AgentIDs : ulong
        {
            Judges,
            Administrators,
            Moderators
        };
        public enum StatusIDs : ulong
        {
            Graduated,
            Students,
            Visitors
        };
        public enum LabelIds : ulong
        {
            RoleManager,
            HostingManager,
            ChannelManager,
            CodeManager,
            WebhookManager,
            EmojiManager,
            NicknameManager,
            GroupManager,
            EmissaryManager,
            BotManager,
            VoiceManager,
            EventManager,
            ShowcaseManager,
            SchemeCoordinator,
            AgentEmployer
        };
        public double TimeCreated { get; set; }
        /// <summary>
        /// The times this configuration has been written.
        /// </summary>
        public List<double> TimesWritten { get; set; }
    }
}
