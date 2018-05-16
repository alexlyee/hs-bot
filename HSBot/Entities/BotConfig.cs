using HSBot.Helpers;
using static HSBot.Helpers.Utilities;

namespace HSBot.Entities
{
    public struct BotConfig
    {
        public string Token { get; set; }
        public int UpdateRate { get; set; }
        public string Playing { get; set; }
        public Discord.UserStatus Status { get; set; }
            public byte BotThemeColorR { get; set; }
            public byte BotThemeColorG { get; set; }
            public byte BotThemeColorB { get; set; }
        public string ConsoleTitle { get; set; }
        public int MessageCacheSize { get; set; }
        
    }
}
