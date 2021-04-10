using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.API.Users
{
    /// <summary>
    /// Service for managing users.
    /// </summary>
    [Service]
    public interface IUserManager
    {
        /// <summary>
        /// Gets the user providers.
        /// </summary>
        IReadOnlyCollection<IUserProvider> UserProviders { get; }

        /// <summary>
        /// Gets all users of the given type.
        /// </summary>
        /// <param name="userType">The type of the users to look for.</param>
        /// <returns>All users of the given type.</returns>
        Task<IReadOnlyCollection<IUser>> GetUsersAsync(string userType);

        /// <summary>
        /// Searches for an user.
        /// </summary>
        /// <param name="userType">The user type.</param>
        /// <param name="searchString">The user ID or name depending on the search mode.</param>
        /// <param name="searchMode">The search mode.</param>
        /// <returns><b>The user</b> if found; otherwise, null.</returns>
        Task<IUser?> FindUserAsync(string userType, string searchString, UserSearchMode searchMode);

        /// <summary>
        /// Broadcasts a message to all users.
        /// </summary>
        /// <param name="message">The message to broadcast.</param>
        /// <param name="color">The message color. May not be supported on all platforms.</param>
        Task BroadcastAsync(string message, Color? color = null);

        /// <summary>
        /// Broadcasts a message to all users of the given type.
        /// </summary>
        /// <param name="userType">The user type to broadcast to.</param>
        /// <param name="message">The message to broadcast.</param>
        /// <param name="color">The message color. May not be supported on all platforms.</param>
        Task BroadcastAsync(string userType, string message, Color? color = null);
        
        /// <summary>
        /// Ban a user.
        /// </summary>
        /// <param name="user">The user to ban.</param>
        /// <param name="reason">The reason of the user ban.</param>
        /// <param name="endTime">The time when the ban will expire.</param>
        Task<bool> BanAsync(IUser user, string? reason = null, DateTime? endTime = null);

        /// <summary>
        /// Ban a user.
        /// </summary>
        /// <param name="user">The user to ban.</param>
        /// <param name="instigator">The user that ban.</param>
        /// <param name="reason">The reason of the user ban.</param>
        /// <param name="endTime">The time when the ban will expire.</param>
        Task<bool> BanAsync(IUser user, IUser? instigator = null, string? reason = null, DateTime? endTime = null);

        /// <summary>
        /// KIck a user.
        /// </summary>
        /// <param name="user">The user to kick.</param>
        /// <param name="reason">The reason of the user kick.</param>
        Task<bool> KickAsync(IUser user, string? reason = null);
    }
}
