extern alias JetBrainsAnnotations;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Zombies.Events
{
    /// <summary>
    /// The event that is triggered when damage is dealt to a zombie.
    /// </summary>
    public class UnturnedZombieDamagingEvent : UnturnedZombieEvent, ICancellableEvent
    {
        /// <summary>
        /// Gets or sets the amount of damage to deal.
        /// </summary>
        public ushort DamageAmount { get; set; }

        /// <summary>
        /// Gets or sets the ragdoll position.
        /// </summary>
        public Vector3 Ragdoll { get; set; }

        /// <summary>
        /// Gets or sets the player dealing the damage.
        /// </summary>
        public UnturnedPlayer? Instigator { get; set; }

        /// <summary>
        /// Gets or sets the ragdoll effect.
        /// </summary>
        public ERagdollEffect RagdollEffect { get; set; }

        /// <summary>
        /// Gets or sets the stun override.
        /// </summary>
        public EZombieStunOverride StunOverride { get; set; }

        /// <summary>
        /// Gets or sets the ELimb.
        /// </summary>
        public ELimb Limb { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedZombieDamagingEvent(UnturnedZombie zombie, UnturnedPlayer? user, ushort damageAmount,
            Vector3 ragdoll, ERagdollEffect ragdollEffect, EZombieStunOverride stunOverride, ELimb limb) : base(zombie)
        {
            DamageAmount = damageAmount;
            Instigator = user;
            Ragdoll = ragdoll;
            RagdollEffect = ragdollEffect;
            StunOverride = stunOverride;
            Limb = limb;
        }
    }
}
