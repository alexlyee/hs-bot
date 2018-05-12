namespace SchoolDiscordBot.Entities
{
    public struct BotConfig
    {
        public string Token { get; set; }
        public int UpdateRate { get; set; }
        public string Playing { get; set; }
        public Discord.UserStatus Status { get; set; }
            public byte BotThemeColor_R { get; set; }
            public byte BotThemeColor_G { get; set; }
            public byte BotThemeColor_B { get; set; }
        public string ConsoleTitle { get; set; }
    }
}
