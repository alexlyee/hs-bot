﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HSBot.Modules.References
{
    public struct Subject
    {
        public ulong id;
        public string name;
        public ulong textchannelid;
        public ulong roleid;

        public ulong? voicechannelid;
    }
    public struct Teacher
    {
        public ulong id;
        public string name;
    }
    public struct Hour
    {
        public ulong id;
        public string title;
    }
}
