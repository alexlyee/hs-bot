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
            Directors = 0,
            Managers = 0,
            Contrivers = 0
        };
        public List<ulong> TokenIDs { get; set; }
        public enum AgentIDs : ulong
        {
            Judges = 0,
            Administrators = 0,
            Moderators = 0
        };
        public enum StatusIDs : ulong
        {
            Graduated = 0,
            Students = 0,
            Visitors = 0
        };
        public enum LabelIds : ulong
        {
            RoleManager = 0,
            HostingManager = 0,
            ChannelManager = 0,
            CodeManager = 0,
            WebhookManager = 0,
            EmojiManager = 0,
            NicknameManager = 0,
            GroupManager = 0,
            EmissaryManager = 0,
            BotManager = 0,
            VoiceManager = 0,
            EventManager = 0,
            ShowcaseManager = 0,
            SchemeCoordinator = 0,
            AgentEmployer = 0
        };
        public double TimeCreated { get; set; }
        /// <summary>
        /// The times this configuration has been written.
        /// </summary>
        public List<double> TimesWritten { get; set; }
    }
}
