﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.API.Persistence;
using OpenMod.API.Plugins;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;
using OpenMod.Core.Ioc;
using OpenMod.Core.Ioc.Extensions;
using OpenMod.Core.Localization;
using OpenMod.Core.Permissions;
using OpenMod.Core.Plugins.Events;
using OpenMod.Core.Prioritization;

namespace OpenMod.Core.Plugins
{
    [OpenModInternal]
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class PluginActivator : IPluginActivator, IAsyncDisposable
    {
        private readonly IRuntime m_Runtime;
        private readonly ILogger<PluginActivator> m_Logger;
        private readonly IStringLocalizerFactory m_StringLocalizerFactory;
        private readonly ILifetimeScope m_LifetimeScope;
        private readonly IDataStoreFactory m_DataStoreFactory;
        private readonly List<WeakReference> m_ActivatedPlugins;
        private readonly IEventBus m_EventBus;

        private bool m_IsDisposing;

        public PluginActivator(
            IRuntime runtime,
            ILogger<PluginActivator> logger,
            IStringLocalizerFactory stringLocalizerFactory,
            ILifetimeScope lifetimeScope,
            IDataStoreFactory dataStoreFactory, IEventBus eventBus)
        {
            m_Runtime = runtime;
            m_Logger = logger;
            m_StringLocalizerFactory = stringLocalizerFactory;
            m_LifetimeScope = lifetimeScope;
            m_DataStoreFactory = dataStoreFactory;
            m_EventBus = eventBus;
            m_ActivatedPlugins = new List<WeakReference>();
        }

        public IReadOnlyCollection<IOpenModPlugin> ActivatedPlugins
        {
            get
            {
                if (m_IsDisposing)
                {
                    throw new ObjectDisposedException(nameof(PluginActivator));
                }

                return m_ActivatedPlugins.Where(d => d.IsAlive).Select(d => d.Target).Cast<IOpenModPlugin>().ToList();
            }
        }

        [CanBeNull]
        public async Task<IOpenModPlugin> TryActivatePluginAsync(Assembly assembly)
        {
            try
            {
                if (m_IsDisposing)
                {
                    throw new ObjectDisposedException(nameof(PluginActivator));
                }

                var pluginMetadata = assembly.GetCustomAttribute<PluginMetadataAttribute>();
                if (pluginMetadata == null)
                {
                    m_Logger.LogError(
                        $"Failed to load plugin from assembly {assembly}: couldn't find any plugin metadata");
                    return null;
                }

                var pluginTypes = assembly.FindTypes<IOpenModPlugin>(false).ToList();
                if (pluginTypes.Count == 0)
                {
                    m_Logger.LogError(
                        $"Failed to load plugin from assembly {assembly}: couldn't find any IOpenModPlugin implementation");
                    return null;
                }

                if (pluginTypes.Count > 1)
                {
                    m_Logger.LogError(
                        $"Failed to load plugin from assembly {assembly}: assembly has multiple IOpenModPlugin instances");
                    return null;
                }

                var pluginType = pluginTypes.Single();
                IOpenModPlugin pluginInstance;
                try
                {
                    var serviceProvider = m_LifetimeScope.Resolve<IServiceProvider>();
                    var lifetimeScope = m_LifetimeScope.BeginLifetimeScope(containerBuilder =>
                    {
                        var workingDirectory = PluginHelper.GetWorkingDirectory(m_Runtime, pluginMetadata.Id);

                        var assemblyName = assembly.GetName().Name;
                        var resourcesNameSpace = assembly.GetManifestResourceNames()
                            .FirstOrDefault(rs =>
                                rs.EndsWith($"{assemblyName}.config.yaml") ||
                                rs.EndsWith($"{assemblyName}.translations.yaml"));

                        //Normal case:  <namespeace>.config.yaml -> generate config.yaml
                        //RootName case: RG.Plugin.<namespeace>.config.yaml -> generate RG.Plugin.config.yaml
                        if (!string.IsNullOrEmpty(resourcesNameSpace))
                        {
                            // ReSharper disable StringIndexOfIsCultureSpecific.1
                            var indexToRemove = resourcesNameSpace.EndsWith($"{assemblyName}.config.yaml")
                                ? resourcesNameSpace.IndexOf($"{assemblyName}.config.yaml")
                                : resourcesNameSpace.IndexOf($"{assemblyName}.translations.yaml");
                            // ReSharper restore StringIndexOfIsCultureSpecific.1        

                            resourcesNameSpace = resourcesNameSpace.Remove(indexToRemove);
                        }

                        var configurationBuilder = new ConfigurationBuilder();
                        if (Directory.Exists(workingDirectory))
                        {
                            configurationBuilder
                                .SetBasePath(workingDirectory)
                                .AddYamlFile($"{resourcesNameSpace}config.yaml", optional: true, reloadOnChange: true);
                        }

                        var configuration = configurationBuilder
                            .AddEnvironmentVariables(pluginMetadata.Id.Replace(".", "_") + "_")
                            .Build();

                        containerBuilder.Register(context => configuration)
                            .As<IConfiguration>()
                            .As<IConfigurationRoot>()
                            .SingleInstance()
                            .OwnedByLifetimeScope();

                        containerBuilder.RegisterType(pluginType)
                            .As(pluginType)
                            .As<IOpenModComponent>()
                            .As<IOpenModPlugin>()
                            .SingleInstance()
                            .ExternallyOwned();

                        containerBuilder.RegisterType<ScopedPermissionChecker>()
                            .As<IPermissionChecker>()
                            .InstancePerLifetimeScope()
                            .OwnedByLifetimeScope();

                        containerBuilder.Register(context => m_DataStoreFactory.CreateDataStore(null, workingDirectory))
                            .As<IDataStore>()
                            .SingleInstance()
                            .OwnedByLifetimeScope();

                        var stringLocalizer = Directory.Exists(workingDirectory)
                            ? m_StringLocalizerFactory.Create($"{resourcesNameSpace}translations", workingDirectory)
                            : NullStringLocalizer.Instance;

                        containerBuilder.Register(context => stringLocalizer)
                            .As<IStringLocalizer>()
                            .SingleInstance()
                            .OwnedByLifetimeScope();

                        var services =
                            ServiceRegistrationHelper.FindFromAssembly<PluginServiceImplementationAttribute>(assembly,
                                m_Logger);

                        var servicesRegistrations = services.OrderBy(d => d.Priority,
                            new PriorityComparer(PriortyComparisonMode.LowestFirst));

                        foreach (var servicesRegistration in servicesRegistrations)
                        {
                            var implementationType = servicesRegistration.ServiceImplementationType;
                            containerBuilder.RegisterType(implementationType)
                                .As(implementationType)
                                .WithLifetime(servicesRegistration.Lifetime)
                                .OwnedByLifetimeScope();

                            foreach (var service in servicesRegistration.ServiceTypes)
                            {
                                containerBuilder.Register(c => c.Resolve(implementationType))
                                    .As(service)
                                    .WithLifetime(servicesRegistration.Lifetime)
                                    .OwnedByLifetimeScope();
                            }
                        }

                        foreach (var type in pluginType.Assembly.FindTypes<IPluginContainerConfigurator>())
                        {
                            var configurator =
                                (IPluginContainerConfigurator) ActivatorUtilities.CreateInstance(serviceProvider, type);
                            configurator.ConfigureContainer(
                                new PluginServiceConfigurationContext(m_LifetimeScope, configuration,
                                    containerBuilder));
                        }

                        var configurationEvent = new PluginContainerConfiguringEvent(pluginMetadata, pluginType,
                            configuration, containerBuilder, workingDirectory);
                        AsyncHelper.RunSync(() => m_EventBus.EmitAsync(m_Runtime, this, configurationEvent));
                    });

                    pluginInstance = (IOpenModPlugin) lifetimeScope.Resolve(pluginType);
                    var pluginActivateEvent = new PluginActivatingEvent(pluginInstance);
                    await m_EventBus.EmitAsync(m_Runtime, this, pluginActivateEvent);

                    if (pluginActivateEvent.IsCancelled)
                    {
                        await lifetimeScope.DisposeAsync();
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    m_Logger.LogError(ex,
                        $"Failed to load plugin from type: {pluginType.FullName} in assembly: {assembly.FullName}");
                    return null;
                }

                try
                {
                    await pluginInstance.LoadAsync();
                    var serviceProvider = pluginInstance.LifetimeScope.Resolve<IServiceProvider>();
                    var pluginHelpWriter = ActivatorUtilities.CreateInstance<PluginHelpWriter>(serviceProvider);
                    await pluginHelpWriter.WriteHelpFileAsync();
                }
                catch (Exception ex)
                {
                    m_Logger.LogError(ex,
                        $"Failed to load plugin: {pluginInstance.DisplayName} v{pluginInstance.Version}");
                    return null;
                }

                m_ActivatedPlugins.Add(new WeakReference(pluginInstance));
                return pluginInstance;
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, $"Failed to load plugin from assembly: {assembly.FullName}");
                return null;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (m_IsDisposing)
            {
                return;
            }

            m_IsDisposing = true;
            m_Logger.LogInformation("Unloading all plugins...");

            var i = 0;
            foreach (var instance in from plugin in m_ActivatedPlugins
                where plugin.IsAlive
                select (IOpenModPlugin) plugin.Target)
            {
                try
                {
                    await instance.UnloadAsync();
                    i++;
                }
                catch (Exception ex)
                {
                    m_Logger.LogError(ex, $"An exception occured while unloading {instance.DisplayName}");
                }
            }

            m_ActivatedPlugins.Clear();
            m_Logger.LogInformation($"> {i} plugins unloaded.");
        }
    }
}