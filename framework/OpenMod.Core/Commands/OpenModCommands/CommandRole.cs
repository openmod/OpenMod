﻿using System;
using System.Threading.Tasks;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("role")]
    [CommandAlias("r")]
    [CommandDescription("Manage permission roles")]
    [CommandSyntax("<add/remove/reload>")]

    public class CommandRole : Command
    {
        public CommandRole(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override Task OnExecuteAsync()
        {
            return Task.FromException(new CommandWrongUsageException(Context));
        }
    }
}