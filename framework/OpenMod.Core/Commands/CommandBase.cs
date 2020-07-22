﻿using System;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Commands;
using OpenMod.API.Permissions;
using OpenMod.Core.Ioc;

namespace OpenMod.Core.Commands
{
    [DontAutoRegister]
    public abstract class CommandBase: ICommand
    {
        public ICommandContext Context { get; }
        private readonly IPermissionChecker m_PermissionChecker;
        private readonly string m_CommandPermission;

        protected CommandBase(IServiceProvider serviceProvider)
        {
            var contextAccessor = serviceProvider.GetRequiredService<ICurrentCommandContextAccessor>();
            Context = contextAccessor.Context;
            m_PermissionChecker = serviceProvider.GetRequiredService<IPermissionChecker>();
            m_CommandPermission = serviceProvider.GetRequiredService<ICommandPermissionBuilder>().GetPermission(Context.CommandRegistration);
        }

        public abstract Task ExecuteAsync();

        public virtual Task PrintAsync(string message)
        {
            return Context.Actor.PrintMessageAsync(message, Color.White);
        }

        public virtual Task PrintAsync(string message, Color color)
        {
            return Context.Actor.PrintMessageAsync(message, color);
        }

        public virtual Task<PermissionGrantResult> CheckPermissionAsync(string permission)
        {
            string prefix = "";
            if (!string.IsNullOrEmpty(m_CommandPermission))
            {
                prefix = m_CommandPermission + ".";
            }

            return m_PermissionChecker.CheckPermissionAsync(Context.Actor, prefix + permission);
        }
    }
}