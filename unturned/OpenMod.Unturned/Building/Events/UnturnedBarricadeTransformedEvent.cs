using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBarricadeTransformedEvent : UnturnedBuildableTransformedEvent, IUnturnedBarricadeEvent
    {
        public new UnturnedBarricadeBuildable Buildable => (UnturnedBarricadeBuildable)base.Buildable;

        public UnturnedBarricadeTransformedEvent(UnturnedBuildable buildable, UnturnedPlayer? instigator) : base(buildable, instigator)
        {
        }
    }
}
