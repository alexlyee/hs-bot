using System;
using System.Collections.Generic;
using System.Text;
using Discord.Commands;

namespace SchoolDiscordBot.Modules.References
{
    internal class ModuleUtilities
    {
        public T GetCommandPeramaters<T>(string ToParse, T CommandPerametersStructure, SocketCommandContext Context)
        {
            return CommandPerametersStructure;
        }

    }
}
