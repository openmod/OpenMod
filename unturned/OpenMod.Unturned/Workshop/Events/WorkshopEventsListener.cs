extern alias JetBrainsAnnotations;
using System;

using HarmonyLib;

using JetBrainsAnnotations::JetBrains.Annotations;

using OpenMod.API;
using OpenMod.Unturned.Events;

using SDG.Unturned;

namespace OpenMod.Unturned.Workshop.Events
{
    [OpenModInternal]
    [UsedImplicitly]
    internal class WorkshopEventsListener : UnturnedEventsListener
    {

        private delegate void WorkshopLoaded();
        private static event WorkshopLoaded? OnWorkshopLoaded;

        public WorkshopEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            OnWorkshopLoaded += WorkshopLoadedEventEmitter;
        }

        public override void Unsubscribe()
        {
            OnWorkshopLoaded -= WorkshopLoadedEventEmitter;
        }

        private void WorkshopLoadedEventEmitter()
        {
            var @event = new UnturnedWorkshopLoadedEvent();

            Emit(@event);
        }

        [UsedImplicitly]
        [HarmonyPatch(typeof(Provider), nameof(Provider.launch))]
        [HarmonyPrefix]
        public static void PreProviderLaunch()
        {
            OnWorkshopLoaded?.Invoke();
        }
    }
}
