using OpenMod.Unturned.Players;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Zombies.Events
{
    /// <summary>
    /// The event that is triggered when a zombie is dying.
    /// </summary>
    public class UnturnedZombieDyingEvent : UnturnedZombieDamagingEvent
    {
        public UnturnedZombieDyingEvent(UnturnedZombie zombie, UnturnedPlayer? player, ushort damageAmount,
            Vector3 ragdoll, ERagdollEffect ragdollEffect, EZombieStunOverride stunOverride, ELimb limb)
                : base(zombie, player, damageAmount, ragdoll, ragdollEffect, stunOverride, limb)
        {
        }
    }
}
