using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Localization;

namespace OpenMod.Core.Commands
{
    public class CommandWrongUsageException : UserFriendlyException
    {
        public CommandWrongUsageException(string message) : base(message)
        {

        }

        [OpenModInternal]
        public CommandWrongUsageException(ICommandContext context, IOpenModStringLocalizer localizer)
            : base(localizer["commands:errors:wrong_usage", new { Command = context.CommandPrefix + context.CommandAlias, Args = string.Join(" ", context.Parameters), context.CommandRegistration!.Syntax }])
        {

        }

        public CommandWrongUsageException(ICommandContext context) : this(context, context.ServiceProvider.GetRequiredService<IOpenModStringLocalizer>())
        {

        }
    }
}