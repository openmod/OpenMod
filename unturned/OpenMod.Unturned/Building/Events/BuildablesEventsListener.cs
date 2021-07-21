extern alias JetBrainsAnnotations;
using System;
using HarmonyLib;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace OpenMod.Unturned.Building.Events
{
    [UsedImplicitly]
    internal class BuildablesEventsListener : UnturnedEventsListener
    {
        public BuildablesEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            BarricadeManager.onDamageBarricadeRequested += OnDamageBarricadeRequested;
            StructureManager.onDamageStructureRequested += OnDamageStructureRequested;

            BarricadeManager.onDeployBarricadeRequested += OnDeployBarricadeRequested;
            StructureManager.onDeployStructureRequested += OnDeployStructureRequested;

            BarricadeManager.onHarvestPlantRequested += OnHarvestPlantRequested;

            BarricadeManager.onOpenStorageRequested += OnOpenStorageRequested;

            BarricadeManager.onModifySignRequested += OnModifySignRequested;

            BarricadeManager.onBarricadeSpawned += Events_OnBarricadeDeployed;
            StructureManager.onStructureSpawned += Events_OnStructureDeployed;

            BarricadeManager.onSalvageBarricadeRequested += OnSalvageBarricadeRequested;
            StructureManager.onSalvageStructureRequested += OnSalvageStructureRequested;

            OnBarricadeDestroying += Events_OnBarricadeDestroyed;
            OnStructureDestroying += Events_OnStructureDestroyed;

            BarricadeManager.onTransformRequested += OnTransformBarricadeRequested;
            StructureManager.onTransformRequested += OnTransformStructureRequested;

            OnBarricadeTransformed += Events_OnBarricadeTransformed;
            OnStructureTransformed += Events_OnStructureTransformed;
        }

        public override void Unsubscribe()
        {
            BarricadeManager.onDamageBarricadeRequested -= OnDamageBarricadeRequested;
            StructureManager.onDamageStructureRequested -= OnDamageStructureRequested;

            BarricadeManager.onDeployBarricadeRequested -= OnDeployBarricadeRequested;
            StructureManager.onDeployStructureRequested -= OnDeployStructureRequested;

            BarricadeManager.onHarvestPlantRequested -= OnHarvestPlantRequested;

            BarricadeManager.onOpenStorageRequested -= OnOpenStorageRequested;

            BarricadeManager.onModifySignRequested -= OnModifySignRequested;

            BarricadeManager.onBarricadeSpawned -= Events_OnBarricadeDeployed;
            StructureManager.onStructureSpawned -= Events_OnStructureDeployed;

            BarricadeManager.onSalvageBarricadeRequested -= OnSalvageBarricadeRequested;
            StructureManager.onSalvageStructureRequested -= OnSalvageStructureRequested;

            OnBarricadeDestroying -= Events_OnBarricadeDestroyed;
            OnStructureDestroying -= Events_OnStructureDestroyed;

            BarricadeManager.onTransformRequested -= OnTransformBarricadeRequested;
            StructureManager.onTransformRequested -= OnTransformStructureRequested;

            OnBarricadeTransformed -= Events_OnBarricadeTransformed;
            OnStructureTransformed -= Events_OnStructureTransformed;
        }

        private void OnDamageBarricadeRequested(CSteamID instigatorSteamId, Transform barricadeTransform,
            ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            if (!BarricadeManager.tryGetInfo(barricadeTransform, out _, out _, out _,
                out var index, out var region, out var drop))
            {
                return;
            }

            var data = region.barricades[index];
            var buildable = new UnturnedBarricadeBuildable(data, drop);

            var nativePlayer = PlayerTool.getPlayer(instigatorSteamId);
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = pendingTotalDamage >= buildable.State.Health
                ? (UnturnedBuildableDamagingEvent)new UnturnedBarricadeDestroyingEvent(buildable, pendingTotalDamage,
                    damageOrigin, player, instigatorSteamId)
                : new UnturnedBarricadeDamagingEvent(buildable, pendingTotalDamage, damageOrigin, player, instigatorSteamId);

            @event.IsCancelled = !shouldAllow;

            Emit(@event);

            pendingTotalDamage = @event.DamageAmount;
            shouldAllow = !@event.IsCancelled;
        }

        private void OnDamageStructureRequested(CSteamID instigatorSteamId, Transform structureTransform,
            ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            var drop = StructureManager.FindStructureByRootTransform(structureTransform);
            if (drop == null)
            {
                return;
            }

            var buildable = new UnturnedStructureBuildable(drop);

            var nativePlayer = PlayerTool.getPlayer(instigatorSteamId);
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = pendingTotalDamage >= buildable.State.Health
                ? (UnturnedBuildableDamagingEvent)new UnturnedStructureDestroyingEvent(buildable,
                    pendingTotalDamage, damageOrigin, player, instigatorSteamId)
                : new UnturnedStructureDamagingEvent(buildable, pendingTotalDamage, damageOrigin, player, instigatorSteamId);

            @event.IsCancelled = !shouldAllow;

            Emit(@event);

            pendingTotalDamage = @event.DamageAmount;
            shouldAllow = !@event.IsCancelled;
        }

        private void OnDeployBarricadeRequested(Barricade barricade, ItemBarricadeAsset asset, Transform hit, ref Vector3 point,
            ref float angleX, ref float angleY, ref float angleZ, ref ulong owner, ref ulong @group, ref bool shouldAllow)
        {
            var rot = Quaternion.Euler(angleX, angleY, angleZ);

            var @event = new UnturnedBarricadeDeployingEvent(asset, hit, point, rot, owner, group)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            point = @event.Point;

            var eulerAngles = @event.Rotation.eulerAngles;

            angleX = eulerAngles.x;
            angleY = eulerAngles.y;
            angleZ = eulerAngles.z;

            owner = @event.Owner;
            group = @event.Group;

            shouldAllow = !@event.IsCancelled;
        }

        private void OnDeployStructureRequested(Structure structure, ItemStructureAsset asset, // lgtm [cs/too-many-ref-parameters]
            ref Vector3 point, ref float angleX, ref float angleY, ref float angleZ, ref ulong owner, ref ulong @group, ref bool shouldAllow)
        {
            var rot = Quaternion.Euler(angleX, angleY, angleZ);

            var @event = new UnturnedStructureDeployingEvent(asset, point, rot, owner, group)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            point = @event.Point;

            var eulerAngles = @event.Rotation.eulerAngles;

            angleX = eulerAngles.x;
            angleY = eulerAngles.y;
            angleZ = eulerAngles.z;

            owner = @event.Owner;
            group = @event.Group;

            shouldAllow = !@event.IsCancelled;
        }

        private void OnHarvestPlantRequested(CSteamID steamId, byte x, byte y, ushort plant, ushort index, ref bool shouldAllow)
        {
            if (!BarricadeManager.tryGetRegion(x, y, plant, out var region))
            {
                return;
            }

            var data = region.barricades[index];
            var drop = region.drops[index];

            var nativePlayer = PlayerTool.getPlayer(steamId);
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedPlantHarvestingEvent(new UnturnedBarricadeBuildable(data, drop), player!, steamId)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnOpenStorageRequested(CSteamID steamId, InteractableStorage storage, ref bool shouldAllow)
        {
            UnturnedBarricadeBuildable? buildable = GetUnturnedBarricadeBuildableByInteractable(storage);
            if (buildable == null)
            {
                return;
            }

            var nativePlayer = PlayerTool.getPlayer(steamId);
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedStorageOpeningEvent(buildable, player!, steamId)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnModifySignRequested(CSteamID steamId, InteractableSign sign,  // lgtm [cs/too-many-ref-parameters]
            ref string text, ref bool shouldAllow)
        {
            UnturnedBarricadeBuildable? buildable = GetUnturnedBarricadeBuildableByInteractable(sign);
            if (buildable == null)
            {
                return;
            }

            var nativePlayer = PlayerTool.getPlayer(steamId);
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedSignModifyingEvent(buildable, player!, steamId, text)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            text = @event.Text;

            shouldAllow = !@event.IsCancelled;
        }

        private static UnturnedBarricadeBuildable? GetUnturnedBarricadeBuildableByInteractable(Interactable interactable)
        {
            BarricadeData? data = null;
            BarricadeDrop? drop = null;
            foreach (BarricadeRegion region in BarricadeManager.regions)
            {
                int index = region.drops.FindIndex(x => x.interactable == interactable);
                if (index != -1)
                {
                    data = region.barricades[index];
                    drop = region.drops[index];
                    break;
                }
            }

            if (data == null || drop == null)
            {
                return null;
            }

            return new UnturnedBarricadeBuildable(data, drop);
        }

        private void Events_OnBarricadeDeployed(BarricadeRegion region, BarricadeDrop drop)
        {
            var data = region.barricades.TailOrDefault();
            if (data == null)
            {
                return;
            }

            var @event = new UnturnedBarricadeDeployedEvent(new UnturnedBarricadeBuildable(data, drop));

            Emit(@event);
        }

        private void Events_OnStructureDeployed(StructureRegion region, StructureDrop drop)
        {
            var @event = new UnturnedStructureDeployedEvent(new UnturnedStructureBuildable(drop));

            Emit(@event);
        }

        private void OnSalvageBarricadeRequested(CSteamID steamId, byte x, byte y, ushort plant, ushort index, ref bool shouldAllow)
        {
            if (!BarricadeManager.tryGetRegion(x, y, plant, out var region))
            {
                return;
            }

            var data = region.barricades[index];
            var drop = region.drops[index];

            var nativePlayer = PlayerTool.getPlayer(steamId);
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedBarricadeSalvagingEvent(new UnturnedBarricadeBuildable(data, drop), player!)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnSalvageStructureRequested(CSteamID steamId, byte x, byte y, ushort index, ref bool shouldAllow)
        {
            if (!StructureManager.tryGetRegion(x, y, out var region))
            {
                return;
            }

            var drop = region.drops[index];

            var nativePlayer = PlayerTool.getPlayer(steamId);
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedStructureSalvagingEvent(new UnturnedStructureBuildable(drop), player!)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void Events_OnBarricadeDestroyed(BarricadeData data, BarricadeDrop drop)
        {
            var @event = new UnturnedBarricadeDestroyedEvent(new UnturnedBarricadeBuildable(data, drop));

            Emit(@event);
        }

        private void Events_OnStructureDestroyed(StructureDrop drop)
        {
            var @event = new UnturnedStructureDestroyedEvent(new UnturnedStructureBuildable(drop));

            Emit(@event);
        }

        private void OnTransformBarricadeRequested(CSteamID instigator, byte x, byte y, ushort plant, uint instanceId, // lgtm [cs/too-many-ref-parameters]
            ref Vector3 point, ref byte angleX, ref byte angleY, ref byte angleZ, ref bool shouldAllow)
        {
            if (!BarricadeManager.tryGetRegion(x, y, plant, out var region))
            {
                return;
            }

            var index = region.barricades.FindIndex(k => k.instanceID == instanceId);
            var data = region.barricades[index];
            var drop = region.drops[index];

            var nativePlayer = PlayerTool.getPlayer(instigator);
            var player = GetUnturnedPlayer(nativePlayer);

            var rot = Quaternion.Euler(angleX * 2, angleY * 2, angleZ * 2); // lgtm [cs/loss-of-precision]

            var @event = new UnturnedBarricadeTransformingEvent(
                new UnturnedBarricadeBuildable(data, drop), player!, point, rot)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
            point = @event.Point;

            var eulerAngles = @event.Rotation.eulerAngles;

            angleX = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.x / 2f) * 2); // lgtm [cs/loss-of-precision]
            angleY = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.y / 2f) * 2); // lgtm [cs/loss-of-precision]
            angleZ = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.z / 2f) * 2); // lgtm [cs/loss-of-precision]
        }

        private void OnTransformStructureRequested(CSteamID instigator, byte x, byte y, uint instanceId, // lgtm [cs/too-many-ref-parameters]
            ref Vector3 point, ref byte angleX, ref byte angleY, ref byte angleZ, ref bool shouldAllow)
        {
            if (!StructureManager.tryGetRegion(x, y, out var region))
            {
                return;
            }

            var index = region.drops.FindIndex(k => k.instanceID == instanceId);
            var drop = region.drops[index];

            var nativePlayer = PlayerTool.getPlayer(instigator);
            var player = GetUnturnedPlayer(nativePlayer);

            var rot = Quaternion.Euler(angleX * 2, angleY * 2, angleZ * 2); // lgtm [cs/loss-of-precision]

            var @event = new UnturnedStructureTransformingEvent(
                new UnturnedStructureBuildable(drop), player!, point, rot)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
            point = @event.Point;

            var eulerAngles = @event.Rotation.eulerAngles;

            angleX = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.x / 2f) * 2); // lgtm [cs/loss-of-precision]
            angleY = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.y / 2f) * 2); // lgtm [cs/loss-of-precision]
            angleZ = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.z / 2f) * 2); // lgtm [cs/loss-of-precision]
        }

        private void Events_OnBarricadeTransformed(BarricadeData data, BarricadeDrop drop)
        {
            var player = GetUnturnedPlayer(s_CurrentTransformingPlayer);

            var @event = new UnturnedBarricadeTransformedEvent(new UnturnedBarricadeBuildable(data, drop), player);

            Emit(@event);
        }

        private void Events_OnStructureTransformed(StructureDrop drop)
        {
            var player = GetUnturnedPlayer(s_CurrentTransformingPlayer);

            var @event = new UnturnedStructureTransformedEvent(new UnturnedStructureBuildable(drop), player);

            Emit(@event);
        }

        private delegate void BarricadeDestroyed(BarricadeData data, BarricadeDrop drop);
        private static event BarricadeDestroyed? OnBarricadeDestroying;

        private delegate void StructureDestroyed(StructureDrop drop);
        private static event StructureDestroyed? OnStructureDestroying;

        private delegate void BarricadeTransformed(BarricadeData data, BarricadeDrop drop);
        private static event BarricadeTransformed? OnBarricadeTransformed;

        private delegate void StructureTransformed(StructureDrop drop);
        private static event StructureTransformed? OnStructureTransformed;

        private static Player? s_CurrentTransformingPlayer = null;

        [UsedImplicitly]
        [HarmonyPatch]
        private static class Patches
        {
            [UsedImplicitly]
            [HarmonyPatch(typeof(BarricadeManager), nameof(BarricadeManager.destroyBarricade))]
            [HarmonyPrefix]
            private static void BarricadeDestroyed(BarricadeRegion region, ushort index)
            {
                if (index >= region.drops.Count)
                {
                    return;
                }

                var data = region.barricades[index];
                var drop = region.drops[index];

                OnBarricadeDestroying?.Invoke(data, drop);
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(StructureManager), nameof(StructureManager.ReceiveDestroyStructure))]
            [HarmonyPrefix]
            private static void StructureDestroyed(NetId netId)
            {
                var structure = NetIdRegistry.Get<StructureDrop>(netId);
                if (structure == null)
                {
                    return;
                }

                OnStructureDestroying?.Invoke(structure);
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(BarricadeManager), nameof(BarricadeManager.ReceiveTransformBarricadeRequest))]
            [HarmonyPrefix]
            private static void PreAskTransformBarricadeRequest(in ServerInvocationContext context)
            {
                s_CurrentTransformingPlayer = context.GetPlayer();
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(BarricadeManager), nameof(BarricadeManager.ReceiveTransformBarricadeRequest))]
            [HarmonyPostfix]
            private static void PostAskTransformBarricadeRequest()
            {
                s_CurrentTransformingPlayer = null;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(StructureDrop), nameof(StructureDrop.ReceiveTransformRequest))]
            [HarmonyPrefix]
            private static void PreAskTransformStructureRequest(in ServerInvocationContext context)
            {
                s_CurrentTransformingPlayer = context.GetPlayer();
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(StructureDrop), nameof(StructureDrop.ReceiveTransformRequest))]
            [HarmonyPostfix]
            private static void PostAskTransformStructureRequest()
            {
                s_CurrentTransformingPlayer = null;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(BarricadeManager), nameof(BarricadeManager.ReceiveTransformBarricade))]
            [HarmonyPostfix]
            private static void ReceiveTransformBarricade(byte x, byte y, ushort plant, uint instanceID)
            {
                if (!BarricadeManager.tryGetRegion(x, y, plant, out var region))
                    return;

                var index = region.barricades.FindIndex(k => k.instanceID == instanceID);

                if (index < 0)
                    return;

                var data = region.barricades[index];
                var drop = region.drops[index];

                OnBarricadeTransformed?.Invoke(data, drop);
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(StructureDrop), nameof(StructureDrop.ReceiveTransform))]
            [HarmonyPostfix]
            private static void ReceiveTransformBarricade(StructureDrop __instance)
            {
                OnStructureTransformed?.Invoke(__instance);
            }
        }
    }
}
