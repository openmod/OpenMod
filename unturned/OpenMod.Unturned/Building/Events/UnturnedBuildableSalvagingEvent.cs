using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Building.Events
{
    public abstract class UnturnedBuildableSalvagingEvent : UnturnedBuildableEvent, ICancellableEvent
    {
        public UnturnedPlayer Instigator { get; }

        public bool IsCancelled { get; set; }

        protected UnturnedBuildableSalvagingEvent(UnturnedBuildable buildable, UnturnedPlayer instigator) : base(buildable)
        {
            Instigator = instigator;
        }
    }
}
