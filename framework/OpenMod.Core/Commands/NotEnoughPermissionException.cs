using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Localization;

namespace OpenMod.Core.Commands
{
    public class NotEnoughPermissionException : UserFriendlyException
    {
        [OpenModInternal]
        public NotEnoughPermissionException(IOpenModStringLocalizer stringLocalizer, string permission)
            : base(stringLocalizer["commands:errors:not_enough_permission", new { Permission = permission }])
        {
        }

        public NotEnoughPermissionException(ICommandContext context, string permission)
            : this(context.ServiceProvider.GetRequiredService<IOpenModStringLocalizer>(), GetPermission(context, permission))
        {

        }

        private static string GetPermission(ICommandContext context, string permission)
        {
            var permissionBuilder = context.ServiceProvider.GetRequiredService<ICommandPermissionBuilder>();
            return $"{permissionBuilder.GetPermission(context.CommandRegistration!)}.{permission}";
        }
    }
}