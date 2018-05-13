using System;
using System.Collections.Generic;
using System.Text;
using Discord.Commands;

namespace HSBot.Modules.References
{
    internal class ModuleUtilities
    {
        public T GetCommandPeramaters<T>(string toParse, T commandPerametersStructure, SocketCommandContext context)
        {
            return commandPerametersStructure;
        }

    }
}
