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
        public ulong BotChannelID { get; set; }

<<<<<<< HEAD
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
=======
        public List<ulong> TokenIDs { get; set; }

        public ulong DirectorRoleID { get; set; }
        public ulong ManagerRoleID { get; set; }
        public ulong ContriverRoleID { get; set; }

        public ulong JudgeRoleID { get; set; }
        public ulong AdministratorRoleID { get; set; }
        public ulong ModeratorRoleID { get; set; }

        public ulong GraduatedRoleID { get; set; }
        public ulong StudentRoleID { get; set; }
        public ulong VisitorRoleID { get; set; }

        public ulong ChannelManagerRoleID { get; set; }
        public ulong WebhookManagerRoleID { get; set; }
        public ulong GroupManagerRoleID { get; set; }
        public ulong VoiceManagerRoleID { get; set; }

        public ulong GroupChannelsCategory { get; set; }
        public ulong GroupClassesCategory { get; set; }

        public string TimeCreated { get; set; }
>>>>>>> 0679f222e55694edf33ae5760713306c76d912bf
    }
}
