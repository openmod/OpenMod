using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedStructureTransformedEvent : UnturnedBuildableTransformedEvent, IUnturnedStructureEvent
    {
        public new UnturnedStructureBuildable Buildable => (UnturnedStructureBuildable)base.Buildable;

        public UnturnedStructureTransformedEvent(UnturnedBuildable buildable, UnturnedPlayer? instigator) : base(buildable, instigator)
        {
        }
    }
}
