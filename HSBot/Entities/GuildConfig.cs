using System.Collections.Generic;
using System;

namespace SchoolDiscordBot.Entities
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
        public enum LabelIDS : ulong
        {
            Role_Manager,
            Hosting_Manager,
            Channel_Manager,
            Code_Manager,
            Webhook_Manager,
            Emoji_Manager,
            Nickname_Manager,
            Group_Manager,
            Emissary_Manager,
            Bot_Manager,
            Voice_Manager,
            Event_Manager,
            Showcase_Manager,
            Scheme_Coordinator,
            Agent_Employer
        };
        public double TimeCreated { get; set; }
        /// <summary>
        /// The times this configuration has been written.
        /// </summary>
        public List<double> TimesWritten { get; set; }
    }
}
