using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedStructureSalvagingEvent : UnturnedBuildableSalvagingEvent, IUnturnedStructureEvent
    {
        public new UnturnedStructureBuildable Buildable => (UnturnedStructureBuildable)base.Buildable;

        public UnturnedStructureSalvagingEvent(UnturnedBuildable buildable, UnturnedPlayer instigator) : base(buildable, instigator)
        {
        }
    }
}
