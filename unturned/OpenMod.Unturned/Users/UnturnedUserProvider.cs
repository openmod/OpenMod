﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Prioritization;
using OpenMod.API.Users;
using OpenMod.Core.Helpers;
using OpenMod.Core.Users;
using OpenMod.UnityEngine.Extensions;
using OpenMod.Unturned.Users.Events;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Users
{
    [Priority(Priority = Priority.Low)]
    public class UnturnedUserProvider : IUserProvider, IDisposable
    {
        private readonly HashSet<UnturnedUser> m_Users;
        private readonly HashSet<UnturnedPendingUser> m_PendingUsers;

        private readonly IEventBus m_EventBus;
        private readonly IRuntime m_Runtime;
        private readonly IUserDataSeeder m_DataSeeder;

        private readonly IUserDataStore m_UserDataStore;

        public UnturnedUserProvider(
            IEventBus eventBus,
            IUserDataSeeder dataSeeder,
            IUserDataStore userDataStore,
            IRuntime runtime)
        {
            m_EventBus = eventBus;
            m_DataSeeder = dataSeeder;
            m_UserDataStore = userDataStore;
            m_Runtime = runtime;
            m_Users = new HashSet<UnturnedUser>();
            m_PendingUsers = new HashSet<UnturnedPendingUser>();

            // sync users in case of openmod reloads
            if (Provider.clients != null)
            {
                foreach (var client in Provider.clients)
                {
                    m_Users.Add(new UnturnedUser(this, userDataStore, client.player));
                }
            }

            if (Provider.pending != null)
            {
                foreach (var pending in Provider.pending)
                {
                    m_PendingUsers.Add(new UnturnedPendingUser(this, userDataStore, pending));
                }
            }

            Provider.onCheckValidWithExplanation += OnPendingPlayerConnecting;
            Provider.onEnemyConnected += OnPlayerConnected;
            Provider.onEnemyDisconnected += OnPlayerDisconnected;
            Provider.onRejectingPlayer += OnRejectingPlayer;
        }

        protected virtual void OnPlayerConnected(SteamPlayer steamPlayer)
        {
            if (m_Users.Any(d => d.Player.SteamPlayer.Equals(steamPlayer)))
            {
                return;
            }

            var pending = m_PendingUsers.FirstOrDefault(d => d.SteamId == steamPlayer.playerID.steamID);
            if (pending != null)
            {
                FinishSession(pending);
            }

            var user = new UnturnedUser(this, m_UserDataStore, steamPlayer.player, pending);
            m_Users.Add(user);

            var connectedEvent = new UnturnedUserConnectedEvent(user);

            async UniTaskVoid EmitDelayedEvent(UnturnedUserConnectedEvent @event)
            {
                await UniTask.DelayFrame(1);
                await m_EventBus.EmitAsync(m_Runtime, this, @event);
            }

            EmitDelayedEvent(connectedEvent).Forget();
        }

        protected virtual void OnPlayerDisconnected(SteamPlayer steamPlayer)
        {
            var user = GetUser(steamPlayer.playerID.steamID);

            if (user == null)
            {
                return;
            }

            if (user.Session is UnturnedUserSession session)
            {
                session.OnSessionEnd();
            }

            AsyncHelper.RunSync(async () =>
            {
                var disconnectedEvent = new UnturnedUserDisconnectedEvent(user);
                await m_EventBus.EmitAsync(m_Runtime, this, disconnectedEvent);

                m_Users.Remove(user);

                var userData = await m_UserDataStore.GetUserDataAsync(user.Id, user.Type);
                if (userData == null)
                {
                    return;
                }

                userData.LastSeen = DateTime.Now;
                await m_UserDataStore.SetUserDataAsync(userData);
            });
        }

        public virtual UnturnedUser GetUser(Player player)
        {
            return GetUser(player.channel.owner);
        }

        public virtual UnturnedUser GetUser(SteamPlayer player)
        {
            return GetUser(player.playerID.steamID)!;
        }

        public virtual UnturnedUser? GetUser(CSteamID id)
        {
            return m_Users.FirstOrDefault(d => d.SteamId == id);
        }

        protected virtual void OnRejectingPlayer(CSteamID steamId, ESteamRejection rejection, string explanation)
        {
            var pending = m_PendingUsers.FirstOrDefault(d => d.SteamId == steamId);
            if (pending == null)
            {
                return;
            }

            AsyncHelper.RunSync(async () =>
            {
                var disconnectedEvent = new UnturnedPendingUserDisconnectedEvent(pending);
                await m_EventBus.EmitAsync(m_Runtime, this, disconnectedEvent);

                FinishSession(pending);

                var userData = await m_UserDataStore.GetUserDataAsync(pending.Id, pending.Type);
                if (userData == null)
                {
                    return;
                }

                userData.LastSeen = DateTime.Now;
                await m_UserDataStore.SetUserDataAsync(userData);
            });
        }

        protected virtual void OnPendingPlayerConnecting(ValidateAuthTicketResponse_t callback, ref bool isValid, ref string? explanation)
        {
            if (m_PendingUsers.Any(d => d.SteamId == callback.m_SteamID))
            {
                return;
            }

            //todo check if it is working, if not this should be patched
            var isPendingValid = isValid;
            var rejectExplanation = explanation;

            AsyncHelper.RunSync(async () =>
            {
                var steamPending = Provider.pending.FirstOrDefault(d => d.playerID.steamID == callback.m_SteamID);
                if (steamPending == null)
                {
                    return;
                }

                var pendingUser = new UnturnedPendingUser(this, m_UserDataStore, steamPending);
                var userData = await m_UserDataStore.GetUserDataAsync(pendingUser.Id, pendingUser.Type);
                var isFirstConnect = userData == null;

                if (isFirstConnect)
                {
                    await m_DataSeeder.SeedUserDataAsync(pendingUser.Id, pendingUser.Type, pendingUser.DisplayName);
                }
                else
                {
                    userData ??= new();
                    userData.LastSeen = DateTime.Now;
                    userData.LastDisplayName = pendingUser.DisplayName;
                    await m_UserDataStore.SetUserDataAsync(userData);
                }

                m_PendingUsers.Add(pendingUser);

                var userConnectingEvent = isFirstConnect
                    ? new UnturnedUserFirstConnectingEvent(pendingUser)
                    : new UnturnedUserConnectingEvent(pendingUser);

                userConnectingEvent.IsCancelled = !isPendingValid;

                if (rejectExplanation != null)
                    await userConnectingEvent.RejectAsync(rejectExplanation);

                await m_EventBus.EmitAsync(m_Runtime, this, userConnectingEvent);

                if (!string.IsNullOrEmpty(userConnectingEvent.RejectionReason))
                {
                    isPendingValid = false;
                    rejectExplanation = userConnectingEvent.RejectionReason;
                }

                if (userConnectingEvent.IsCancelled)
                {
                    isPendingValid = false;
                }
            });

            isValid = isPendingValid;
            explanation = rejectExplanation;
        }

        protected virtual void FinishSession(UnturnedPendingUser pending)
        {
            if (pending.Session is UnturnedPendingUserSession session)
            {
                session.OnSessionEnd();
            }

            m_PendingUsers.Remove(pending);
        }

        public bool SupportsUserType(string userType)
        {
            return userType.Equals(KnownActorTypes.Player, StringComparison.OrdinalIgnoreCase);
        }

        public Task<IUser?> FindUserAsync(string userType, string searchString, UserSearchMode searchMode)
        {
            var confidence = 0;
            var unturnedUser = (IUser?)null;

            foreach (var user in m_Users)
            {
                switch (searchMode)
                {
                    case UserSearchMode.FindByNameOrId:
                    case UserSearchMode.FindById:
                        if (user.Id.Equals(searchString, StringComparison.OrdinalIgnoreCase))
                            return Task.FromResult((IUser?)user);

                        if (searchMode == UserSearchMode.FindByNameOrId)
                            goto case UserSearchMode.FindByName;
                        break;

                    case UserSearchMode.FindByName:
                        var currentConfidence = NameConfidence(user.DisplayName, searchString, confidence);
                        if (currentConfidence > confidence)
                        {
                            unturnedUser = user;
                            confidence = currentConfidence;
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(searchMode), searchMode, null);
                }
            }

            return Task.FromResult(unturnedUser);
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private int NameConfidence(string userName, string searchName, int currentConfidence = -1)
        {
            switch (currentConfidence)
            {
                case 2:
                    if (userName.Equals(searchName, StringComparison.OrdinalIgnoreCase))
                        return 3;
                    goto case 1;

                case 1:
                    if (userName.StartsWith(searchName, StringComparison.OrdinalIgnoreCase))
                        return 2;
                    goto case 0;

                case 0:
                    if (userName.IndexOf(searchName, StringComparison.OrdinalIgnoreCase) != -1)
                        return 1;
                    break;

                default:
                    goto case 2;
            }

            return -1;
        }

        public Task<IReadOnlyCollection<IUser>> GetUsersAsync(string userType)
        {
            return Task.FromResult<IReadOnlyCollection<IUser>>(m_Users);
        }

        public Task BroadcastAsync(string userType, string message, Color? color)
        {
            return BroadcastWithIconAsync(userType, message, null, color);
        }

        public Task BroadcastWithIconAsync(string userType, string message, string? iconUrl = null, Color? color = null)
        {
            if (!KnownActorTypes.Player.Equals(userType, StringComparison.OrdinalIgnoreCase))
            {
                return Task.CompletedTask;
            }

            return BroadcastWithIconAsync(message, iconUrl, color);
        }

        public Task BroadcastAsync(string message, Color? color)
        {
            return BroadcastWithIconAsync(message, null, color);
        }

        public Task BroadcastWithIconAsync(string message, string? iconUrl = null, Color? color = null)
        {
            return BroadcastAsync(message, color, isRich: true, iconUrl: iconUrl);
        }

        public Task BroadcastAsync(string message, Color? color, bool isRich, string? iconUrl)
        {
            async UniTask BroadcastTask()
            {
                await UniTask.SwitchToMainThread();

                color ??= Color.White;
                iconUrl ??= Provider.configData.Browser.Icon;

                ChatManager.serverSendMessage(text: message, color: color.Value.ToUnityColor(), useRichTextFormatting: isRich, iconURL: iconUrl);
            }

            return BroadcastTask().AsTask();
        }

        public ICollection<UnturnedUser> GetOnlineUsers()
        {
            return m_Users;
        }

        public ICollection<UnturnedPendingUser> GetPendingUsers()
        {
            return m_PendingUsers;
        }

        public void Dispose()
        {
            m_Users.Clear();
            m_PendingUsers.Clear();

            Provider.onCheckValidWithExplanation -= OnPendingPlayerConnecting;
            Provider.onEnemyConnected -= OnPlayerConnected;
            Provider.onEnemyDisconnected -= OnPlayerDisconnected;
            Provider.onRejectingPlayer -= OnRejectingPlayer;
        }
    }
}