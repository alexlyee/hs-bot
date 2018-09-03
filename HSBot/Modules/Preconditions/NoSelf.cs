using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSBot.Modules.Preconditions
{
    public class NoSelf : ParameterPreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services)
        {
            var user = value is IUser ? (IUser)value : null;
            if ((user != null) && (context.User.Id == user.Id))
                return Task.FromResult(PreconditionResult.FromError("You can't use this command on yourself"));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}