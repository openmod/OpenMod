extern alias JetBrainsAnnotations;
using System;
using HarmonyLib;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.UnityEngine.Extensions;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Players.Movement.Events
{
    [UsedImplicitly]
    internal class PlayerMovementEventsListener : UnturnedPlayerEventsListener
    {
        public PlayerMovementEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            PlayerAnimator.OnGestureChanged_Global += PlayerAnimator_OnGestureChanged_Global;
            OnTeleporting += Events_OnTeleporting;
        }

        public override void Unsubscribe()
        {
            PlayerAnimator.OnGestureChanged_Global -= PlayerAnimator_OnGestureChanged_Global;
            OnTeleporting -= Events_OnTeleporting;
        }

        public override void SubscribePlayer(Player player)
        {
            player.stance.onStanceUpdated += () => OnStanceUpdated(player);
            player.movement.onSafetyUpdated += (safe) => OnSafetyUpdated(player, safe); 
        }

        public override void UnsubscribePlayer(Player player)
        {
            player.stance.onStanceUpdated -= () => OnStanceUpdated(player);
            player.movement.onSafetyUpdated -= (safe) => OnSafetyUpdated(player, safe);
        }

        private void PlayerAnimator_OnGestureChanged_Global(PlayerAnimator animator, EPlayerGesture gesture)
        {
            var player = GetUnturnedPlayer(animator.player)!;

            var @event = new UnturnedPlayerGestureUpdatedEvent(player, gesture);

            Emit(@event);
        }

        private void Events_OnTeleporting(Player nativePlayer, ref Vector3 position, ref float yaw, ref bool cancel) // lgtm [cs/too-many-ref-parameters]
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            var @event = new UnturnedPlayerTeleportingEvent(player, position.ToSystemVector(), yaw)
            {
                IsCancelled = cancel
            };

            Emit(@event);

            position = @event.Position.ToUnityVector();
            yaw = @event.Yaw;
            cancel = @event.IsCancelled;
        }

        private void OnStanceUpdated(Player nativePlayer)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            var @event = new UnturnedPlayerStanceUpdatedEvent(player);

            Emit(@event);
        }

        private void OnSafetyUpdated(Player nativePlayer, bool safe)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            var @event = new UnturnedPlayerSafetyUpdatedEvent(player, safe);

            Emit(@event);
        }

        private delegate void Teleporting(Player player, ref Vector3 position, ref float yaw, ref bool cancel);
        private static event Teleporting? OnTeleporting;

        [UsedImplicitly]
        [HarmonyPatch]
        internal static class Patches
        {
            [UsedImplicitly]
            [HarmonyPatch(typeof(Player), "teleportToLocationUnsafe")]
            [HarmonyPrefix]
            public static bool TeleportToLocationUnsafe(Player __instance, ref Vector3 position, ref float yaw)
            {
                var cancel = false;

                OnTeleporting?.Invoke(__instance, ref position, ref yaw, ref cancel);

                return !cancel;
            }
        }
    }
}