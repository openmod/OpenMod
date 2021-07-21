using OpenMod.Unturned.Players;
using UnityEngine;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedStructureTransformingEvent : UnturnedBuildableTransformingEvent, IUnturnedStructureEvent
    {
        public new UnturnedStructureBuildable Buildable => (UnturnedStructureBuildable)base.Buildable;

        public UnturnedStructureTransformingEvent(UnturnedStructureBuildable buildable, UnturnedPlayer instigator,
            Vector3 point, Quaternion rotation) : base(buildable, instigator, point, rotation)
        {
        }
    }
}
