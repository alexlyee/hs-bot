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

        public uint ClassChannelPosition { get; set; }
        public uint ClassVoiceChannelPosition { get; set; }

        public string TimeCreated { get; set; }
    }
}
