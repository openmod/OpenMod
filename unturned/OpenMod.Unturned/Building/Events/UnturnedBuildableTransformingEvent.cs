using OpenMod.Extensions.Games.Abstractions.Building;
using OpenMod.Unturned.Players;
using UnityEngine;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBuildableTransformingEvent : UnturnedBuildableEvent, IBuildableTransformingEvent
    {
        public UnturnedPlayer Instigator { get; }

        public Vector3 Point { get; set; }

        public Quaternion Rotation { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedBuildableTransformingEvent(UnturnedBuildable buildable, UnturnedPlayer instigator, Vector3 point, Quaternion rotation) : base(buildable)
        {
            Instigator = instigator;
            Point = point;
            Rotation = rotation;
        }
    }
}
