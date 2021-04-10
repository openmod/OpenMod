extern alias JetBrainsAnnotations;
using HarmonyLib;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.API;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using System;
using System.Linq;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace OpenMod.Unturned.Zombies.Events
{
    [OpenModInternal]
    internal class ZombieEventsListener : UnturnedEventsListener
    {
        public ZombieEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            OnZombieAlertingPlayer += Events_OnZombieAlertingPlayer;
            OnZombieAlertingPosition += Events_OnZombieAlertingPosition;
            DamageTool.damageZombieRequested += DamageTool_damageZombieRequested;
            OnZombieDead += Events_OnZombieDead;
            OnZombieAdded += Events_OnZombieAdded;
            OnZombieRevived += Events_OnZombieRevived;
        }

        public override void Unsubscribe()
        {
            OnZombieAlertingPlayer -= Events_OnZombieAlertingPlayer;
            OnZombieAlertingPosition -= Events_OnZombieAlertingPosition;
            DamageTool.damageZombieRequested -= DamageTool_damageZombieRequested;
            OnZombieDead -= Events_OnZombieDead;
            OnZombieAdded -= Events_OnZombieAdded;
            OnZombieRevived -= Events_OnZombieRevived;
        }

        private void Events_OnZombieAlertingPlayer(Zombie nativeZombie, ref Player? nativePlayer, ref bool cancel)
        {
            var zombie = new UnturnedZombie(nativeZombie);

            var player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedZombieAlertingPlayerEvent(zombie, player!)
            {
                IsCancelled = cancel
            };

            Emit(@event);

            nativePlayer = @event.Player.Player;
            cancel = @event.IsCancelled;
        }

        private void Events_OnZombieAlertingPosition(Zombie nativeZombie, ref Vector3 position, ref bool isStartling, ref bool cancel) // lgtm [cs/too-many-ref-parameters]
        {
            var zombie = new UnturnedZombie(nativeZombie);

            var @event = new UnturnedZombieAlertingPositionEvent(zombie, position, isStartling)
            {
                IsCancelled = cancel
            };

            Emit(@event);

            position = @event.Position;
            isStartling = @event.IsStartling;
            cancel = @event.IsCancelled;
        }

        private void DamageTool_damageZombieRequested(ref DamageZombieParameters parameters, ref bool shouldAllow)
        {
            var zombie = new UnturnedZombie(parameters.zombie);
            var player = GetUnturnedPlayer(parameters.instigator as Player);

            var damageAmount = (ushort)Math.Min(ushort.MaxValue, Math.Floor(parameters.damage * parameters.times));

            var @event = damageAmount >= zombie.Health
                ? new UnturnedZombieDyingEvent(zombie, player, damageAmount, parameters.direction,
                    parameters.ragdollEffect, parameters.zombieStunOverride, parameters.limb)
                : new UnturnedZombieDamagingEvent(zombie, player, damageAmount, parameters.direction,
                    parameters.ragdollEffect, parameters.zombieStunOverride, parameters.limb);

            @event.IsCancelled = !shouldAllow;
            Emit(@event);

            parameters.damage = @event.DamageAmount;
            parameters.direction = @event.Ragdoll;
            parameters.ragdollEffect = @event.RagdollEffect;
            parameters.instigator = @event.Instigator?.Player;
            parameters.zombieStunOverride = @event.StunOverride;
            parameters.limb = @event.Limb;
            shouldAllow = !@event.IsCancelled;
        }

        private void Events_OnZombieDead(Zombie nativeZombie, Vector3 ragdoll, ERagdollEffect ragdollEffect)
        {
            var zombie = new UnturnedZombie(nativeZombie);

            var @event = new UnturnedZombieDeadEvent(zombie, ragdoll, ragdollEffect);

            Emit(@event);
        }

        private void Events_OnZombieAdded(Zombie nativeZombie)
        {
            var zombie = new UnturnedZombie(nativeZombie);

            var @event = new UnturnedZombieAddedEvent(zombie);

            Emit(@event);
        }

        private void Events_OnZombieRevived(Zombie nativeZombie)
        {
            var zombie = new UnturnedZombie(nativeZombie);

            var @event = new UnturnedZombieRevivedEvent(zombie);

            Emit(@event);
        }

        private delegate void ZombieAlertingPlayer(Zombie nativeZombie, ref Player? player, ref bool cancel);
        private static event ZombieAlertingPlayer? OnZombieAlertingPlayer;

        private delegate void ZombieAlertingPosition(Zombie nativeZombie, ref Vector3 position, ref bool isStartling, ref bool cancel);
        private static event ZombieAlertingPosition? OnZombieAlertingPosition;

        private delegate void ZombieSpawned(Zombie nativeZombie);
        private static event ZombieSpawned? OnZombieAdded;
        private static event ZombieSpawned? OnZombieRevived;

        private delegate void ZombieDead(Zombie nativeZombie, Vector3 ragdoll, ERagdollEffect ragdollEffect);
        private static event ZombieDead? OnZombieDead;

        [UsedImplicitly]
        [HarmonyPatch]
        internal static class ZombiePatches
        {
            [UsedImplicitly]
            [HarmonyPatch(typeof(Zombie), "alert", typeof(Player))]
            [HarmonyPrefix]
            public static bool AlertPlayer(Zombie __instance, ref Player? newPlayer)
            {
                if (__instance.isDead)
                {
                    return true;
                }

                var cancel = false;
                OnZombieAlertingPlayer?.Invoke(__instance, ref newPlayer, ref cancel);
                return !cancel;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(Zombie), "alert", typeof(Vector3), typeof(bool))]
            [HarmonyPrefix]
            public static bool AlertPosition(Zombie __instance, ref Vector3 newPosition, ref bool isStartling)
            {
                if (__instance.isDead)
                {
                    return true;
                }

                var cancel = false;
                OnZombieAlertingPosition?.Invoke(__instance, ref newPosition, ref isStartling, ref cancel);
                return !cancel;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(Zombie), "tellDead")]
            [HarmonyPostfix]
            public static void TellDead(Zombie __instance, Vector3 newRagdoll, ERagdollEffect ragdollEffect)
            {
                OnZombieDead?.Invoke(__instance, newRagdoll, ragdollEffect);
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(Zombie), "tellAlive")]
            [HarmonyPostfix]
            public static void TellAlive(Zombie __instance)
            {
                OnZombieRevived?.Invoke(__instance);
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(ZombieManager), "addZombie")]
            [HarmonyPostfix]
            public static void AddZombie(byte bound)
            {
                var zombie = ZombieManager.regions[bound].zombies.LastOrDefault();

                if (zombie != null)
                {
                    OnZombieAdded?.Invoke(zombie);
                }
            }
        }
    }
}
