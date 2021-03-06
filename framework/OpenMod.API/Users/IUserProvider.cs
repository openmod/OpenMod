﻿using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace OpenMod.API.Users
{
    /// <summary>
    /// Provides users.
    /// </summary>
    public interface IUserProvider
    {
        /// <summary>
        /// Checks if the user provider supports the given user type.
        /// </summary>
        /// <param name="userType">The user type to check for.</param>
        /// <returns><b>True</b> if the user type is supported; otherwise, <b>false</b>.</returns>
        bool SupportsUserType(string userType);

        /// <summary>
        /// Searches for an user.
        /// </summary>
        /// <param name="userType">The user type.</param>
        /// <param name="searchString">The user ID or name depending on the search mode.</param>
        /// <param name="searchMode">The search mode.</param>
        /// <returns><b>The user</b> if found; otherwise, null.</returns>
        Task<IUser?> FindUserAsync(string userType, string searchString, UserSearchMode searchMode);

        /// <summary>
        /// Gets all users of the given type.
        /// </summary>
        /// <param name="userType">The type of the users to look for.</param>
        /// <returns>All users of the given type.</returns>
        Task<IReadOnlyCollection<IUser>> GetUsersAsync(string userType);

        /// <summary>
        /// Broadcasts a message to all users.
        /// </summary>
        /// <param name="message">The message to broadcast.</param>
        /// <param name="color">The message color. May not be supported on all platforms.</param>
        Task BroadcastAsync(string message, Color? color = null);

        /// <summary>
        /// Broadcasts a message to all users.
        /// </summary>
        /// <param name="message">The message to broadcast.</param>
        /// <param name="iconUrl">The icon URL to broadcast. May not be supported on all platforms.</param>
        /// <param name="color">The message color. May not be supported on all platforms.</param>
        Task BroadcastWithIconAsync(string message, string? iconUrl = null, Color? color = null);

        /// <summary>
        /// Broadcasts a message to all users of the given type.
        /// </summary>
        /// <param name="userType">The user type to broadcast to.</param>
        /// <param name="message">The message to broadcast.</param>
        /// <param name="color">The message color. May not be supported on all platforms.</param>
        Task BroadcastAsync(string userType, string message, Color? color = null);

        /// <summary>
        /// Broadcasts a message to all users of the given type.
        /// </summary>
        /// <param name="userType">The user type to broadcast to.</param>
        /// <param name="message">The message to broadcast.</param>
        /// <param name="iconUrl">The icon URL to broadcast. May not be supported on all platforms.</param>
        /// <param name="color">The message color. May not be supported on all platforms.</param>
        Task BroadcastWithIconAsync(string userType, string message, string? iconUrl = null, Color? color = null);
    }
}