using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.API.Users;
using OpenMod.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using OpenMod.API;
using OpenMod.Core.Ioc;

namespace OpenMod.Core.Users
{
    [OpenModInternal]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class UserManager : IUserManager, IAsyncDisposable
    {
        private bool m_IsDisposing;

        private readonly List<IUserProvider> m_UserProviders;

        public UserManager(IOptions<UserManagerOptions> options, ILifetimeScope lifetimeScope)
        {
            m_UserProviders = new List<IUserProvider>();

            foreach (var provider in options.Value.UserProviderTypes)
            {
                m_UserProviders.Add((IUserProvider)ActivatorUtilitiesEx.CreateInstance(lifetimeScope, provider));
            }
        }

        public IReadOnlyCollection<IUserProvider> UserProviders
        {
            get { return m_UserProviders; }
        }

        public virtual async Task<IReadOnlyCollection<IUser>> GetUsersAsync(string userType)
        {
            if (string.IsNullOrEmpty(userType))
            {
                throw new ArgumentException(nameof(userType));
            }

            var list = new List<IUser>();

            foreach (var userProvider in UserProviders.Where(d => d.SupportsUserType(userType)))
            {
                list.AddRange(await userProvider.GetUsersAsync(userType));
            }

            return list;
        }

        public virtual async Task<IUser?> FindUserAsync(string userType, string searchString, UserSearchMode searchMode)
        {
            if(string.IsNullOrEmpty(userType))
            {
                throw new ArgumentException(nameof(userType));
            }

            if (string.IsNullOrEmpty(searchString))
            {
                throw new ArgumentException(nameof(searchString));
            }

            foreach (var userProvider in UserProviders.Where(d => d.SupportsUserType(userType)))
            {
                var user = await userProvider.FindUserAsync(userType, searchString, searchMode);
                if (user != null)
                {
                    return user;
                }
            }

            return null;
        }

        public virtual async Task BroadcastAsync(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException(nameof(message));
            }

            foreach (var provider in UserProviders)
            {
                await provider.BroadcastAsync(message);
            }
        }

        public virtual async Task BroadcastAsync(string userType, string message)
        {
            if (string.IsNullOrEmpty(userType))
            {
                throw new ArgumentException(nameof(userType));
            }

            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException(nameof(message));
            }

            var provider = UserProviders.FirstOrDefault(d => d.SupportsUserType(userType));
            if (provider == null)
            {
                return;
            }

            await provider.BroadcastAsync(userType, message);
        }

        public virtual async Task BroadcastAsync(string message, Color? color)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException(nameof(message));
            }

            foreach (var provider in UserProviders)
            {
                await provider.BroadcastAsync(message, color);
            }
        }

        public virtual async Task BroadcastAsync(string userType, string message, Color? color)
        {
            if (string.IsNullOrEmpty(userType))
            {
                throw new ArgumentException(nameof(userType));
            }

            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException(nameof(message));
            }

            var provider = UserProviders.FirstOrDefault(d => d.SupportsUserType(userType));
            if (provider == null)
            {
                return;
            }

            await provider.BroadcastAsync(userType, message, color);
        }

        public Task<bool> BanAsync(IUser user, IUser? instigator = null, string? reason = null, DateTime? endTime = null)
        {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (user.Provider == null)
            {
                return Task.FromResult(result: false);
            }

            return user.Provider.BanAsync(user, instigator, reason, endTime);
        }

        public Task<bool> BanAsync(IUser user, string? reason = null, DateTime? endTime = null)
        {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (user.Provider == null)
            {
                return Task.FromResult(result: false);
            }

            return user.Provider.BanAsync(user, reason, endTime);
        }

        public Task<bool> KickAsync(IUser user, string? reason = null)
        {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (user.Provider == null)
            {
                return Task.FromResult(result: false);
            }

            return user.Provider.KickAsync(user, reason);
        }

        public virtual ValueTask DisposeAsync()
        {
            if (m_IsDisposing)
            {
                return new ValueTask(Task.CompletedTask);
            }
            m_IsDisposing = true;

            return new ValueTask(m_UserProviders.DisposeAllAsync());
        }
    }
}
