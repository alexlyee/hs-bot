using System;

namespace HSBot.Entities
{
    public class GlobalUserAccount
    {
        public ulong Id { get; set; }

        public ulong Miunies { get; set; }

        public DateTime LastDaily { get; set; } = DateTime.UtcNow.AddDays(-2);
        /* Add more values to store */
    }
}
