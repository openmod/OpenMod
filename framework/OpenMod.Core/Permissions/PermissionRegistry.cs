﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Permissions
{
    [UsedImplicitly]
    [ServiceImplementation(Priority = Priority.Lowest, Lifetime = ServiceLifetime.Singleton)]
    public class PermissionRegistry : IPermissionRegistry
    {
        private readonly Dictionary<IOpenModComponent, List<PermissionRegistration>> m_PermissionRegistrations;

        public PermissionRegistry()
        {
            m_PermissionRegistrations = new Dictionary<IOpenModComponent, List<PermissionRegistration>>();
        }

        public void RegisterPermission(
            IOpenModComponent component,
            string permission,
            string description = null,
            PermissionGrantResult? defaultGrant = null)
        {
            if (!m_PermissionRegistrations.ContainsKey(component))
            {
                m_PermissionRegistrations.Add(component, new List<PermissionRegistration>());
            }

            var list = m_PermissionRegistrations[component];
            list.RemoveAll(d => d.Permission.Equals(permission, StringComparison.OrdinalIgnoreCase));
            list.Add(new PermissionRegistration
            {
                Owner = component,
                DefaultGrant = defaultGrant ?? PermissionGrantResult.Default,
                Description = string.IsNullOrEmpty(description) ? null : description,
                Permission = permission
            });
        }

        public IReadOnlyCollection<IPermissionRegistration> GetPermissions(IOpenModComponent component)
        {
            if (!component.IsComponentAlive || !m_PermissionRegistrations.ContainsKey(component))
            {
                return new List<IPermissionRegistration>();
            }

            return m_PermissionRegistrations[component].AsReadOnly();
        }

        public IPermissionRegistration FindPermission(string permission)
        {
            return m_PermissionRegistrations.Values
                .SelectMany(d => d)
                .FirstOrDefault(d => d.Owner.IsComponentAlive 
                                     && permission.Equals($"{d.Owner.OpenModComponentId}.{d.Permission}", StringComparison.OrdinalIgnoreCase));
        }

        public IPermissionRegistration FindPermission(IOpenModComponent component, string permission)
        {
            if (!component.IsComponentAlive || !m_PermissionRegistrations.ContainsKey(component))
            {
                return null;
            }

            return m_PermissionRegistrations[component].FirstOrDefault(d => d.Permission.Equals(permission, StringComparison.OrdinalIgnoreCase));
        }
    }
}