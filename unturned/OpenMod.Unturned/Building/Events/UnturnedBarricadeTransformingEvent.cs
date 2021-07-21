using OpenMod.Unturned.Players;
using UnityEngine;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBarricadeTransformingEvent : UnturnedBuildableTransformingEvent, IUnturnedBarricadeEvent
    {
        public new UnturnedBarricadeBuildable Buildable => (UnturnedBarricadeBuildable)base.Buildable;

        public UnturnedBarricadeTransformingEvent(UnturnedBarricadeBuildable buildable, UnturnedPlayer instigator,
            Vector3 point, Quaternion rotation) : base(buildable, instigator, point, rotation)
        {
        }
    }
}
