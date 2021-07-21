using OpenMod.Extensions.Games.Abstractions.Building;
using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBuildableTransformedEvent : UnturnedBuildableEvent, IBuildableTransformedEvent
    {
        public UnturnedPlayer? Instigator { get; }

        public UnturnedBuildableTransformedEvent(UnturnedBuildable buildable, UnturnedPlayer? instigator) : base(buildable)
        {
            Instigator = instigator;
        }
    }
}
