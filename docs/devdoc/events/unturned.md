# Unturned events

## Animal events
These events are stored within the `OpenMod.Unturned.Animals.Events` namespace.

| **Event**                          | **Fired when**                                                               | **Cancellable** |
|------------------------------------|------------------------------------------------------------------------------|-----------------|
| UnturnedAnimalSpawnedEvent         | After an animal spawns (is revived or new instance added)                    | No              |
| UnturnedAnimalAddedEvent           | After a new animal instance is added (triggers spawned event)                | No              |
| UnturnedAnimalRevivedEvent         | After an animal is revived (triggers spawned event)                          | No              |
| UnturnedAnimalAlertingEvent        | Before an animal is alerted                                                  | Yes             |
| UnturnedAnimalAttackingEvent       | Before an animal targets a player/point for attack (triggers alerting event) | Yes             |
| UnturnedAnimalAttackingPlayerEvent | Before an animal targets a player for attack (triggers attacking event)      | Yes             |
| UnturnedAnimalAttackingPointEvent  | Before an animal targets a point for attack (triggers attacking event)       | Yes             |
| UnturnedAnimalFleeingEvent         | Before an animal flees                                                       | Yes             |
| UnturnedAnimalDamagingEvent        | Before an animal is damaged                                                  | Yes             |
| UnturnedAnimalDyingEvent           | Before an animal takes a fatal amount of damage                              | Yes             |
| UnturnedAnimalDeadEvent            | After an animal dies                                                         | No              |

## Building events
These events are stored within the `OpenMod.Unturned.Building.Events` namespace.

Barricade and structure events derive from their buildable counterparts. Whenever a barricade or structure event is emitted, the buildable version will be emitted as well.

| **Event**                        | **Fired when**                                         | **Cancellable** |
|----------------------------------|--------------------------------------------------------|-----------------|
| UnturnedBuildableDeployedEvent   | After a buildable is deployed                          | No              |
| UnturnedBuildableSalvagingEvent  | Before a buildable is salvaged                         | Yes             |
| UnturnedBuildableDamagingEvent   | Before a buildable is damaged                          | Yes             |
| UnturnedBuildableDestroyingEvent | Before a buildable takes enough damage to be destroyed | Yes             |
| UnturnedBuildableDestroyedEvent  | After a buildable is destroyed                         | No              |
| IUnturnedBarricadeEvent          | Any barricade event is fired                           |                 |
| UnturnedBarricadeDeployedEvent   | After a barricade is deployed                          | No              |
| UnturnedBarricadeSalvagingEvent  | Before a barricade is salvaged                         | Yes             |
| UnturnedBarricadeDamagingEvent   | Before a barricade is damaged                          | Yes             |
| UnturnedBarricadeDestroyingEvent | Before a barricade takes enough damage to be destroyed | Yes             |
| UnturnedBarricadeDestroyedEvent  | After a barricade is destroyed                         | No              |
| IUnturnedStructureEvent          | Any structure event is fired                           |                 |
| UnturnedStructureDeployedEvent   | After a structure is deployed                          | No              |
| UnturnedStructureSalvagingEvent  | Before a structure is salvaged                         | Yes             |
| UnturnedStructureDamagingEvent   | Before a structure is damaged                          | Yes             |
| UnturnedStructureDestroyingEvent | Before a structure takes enough damage to be destroyed | Yes             |

## Environment events
These events are stored within the `OpenMod.Unturned.Environment.Events` namespace.

| **Event**                    | **Fired when**                    | **Cancellable** |
|------------------------------|-----------------------------------|-----------------|
| UnturnedDayNightUpdatedEvent | After the day/night cycle updates | No              |
| UnturnedWeatherUpdatedEvent  | After the weather updates         | No              |

## Player events

### Bans
These events are stored within the `OpenMod.Unturned.Players.Bans.Events` namespace.

| **Event**                      | **Fired when**                          | **Cancellable** |
|--------------------------------|-----------------------------------------|-----------------|
| UnturnedPlayerBanningEvent     | Before a player is banned               | Yes             |
| UnturnedPlayerBannedEvent      | After a player is banned                | No              |
| UnturnedPlayerUnbanningEvent   | Before a player is unbanned             | Yes             |
| UnturnedPlayerUnbannedEvent    | After a player is unbanned              | No              |
| UnturnedPlayerCheckingBanEvent | Before a player's ban status is checked | Yes             |

### Chat
These events are stored within the `OpenMod.Unturned.Players.Chat.Events` namespace.

| **Event**                         | **Fired when**                                        | **Cancellable**                 |
|-----------------------------------|-------------------------------------------------------|---------------------------------|
| UnturnedPlayerChattingEvent       | Before a player chats                                 | Yes                             |
| UnturnedServerSendingMessageEvent | Before the server displays a chat message to a player | No, but there modifiable fields |

### Clothing
These events are stored within the `OpenMod.Unturned.Players.Clothing.Events` namespace.

| **Event**                              | **Fired when**                             | **Cancellable** |
|----------------------------------------|--------------------------------------------|-----------------|
| UnturnedPlayerClothingEquippingEvent   | Before any piece of clothing is equipped   | Yes             |
| UnturnedPlayerClothingEquippedEvent    | After any piece of clothing is equipped    | No              |
| UnturnedPlayerClothingUnequippingEvent | Before any piece of clothing is unequipped | Yes             |
| UnturnedPlayerClothingUnequippedEvent  | After any piece of clothing is unequipped  | No              |

### Connections
These events are stored within the `OpenMod.Unturned.Players.Connections.Events` namespace.

| **Event**                       | **Fired when**             | **Cancellable** |
|---------------------------------|----------------------------|-----------------|
| UnturnedPlayerConnectedEvent    | After a player connects    | No              |
| UnturnedPlayerDisconnectedEvent | After a player disconnects | No              |

### Crafting
These events are stored within the `OpenMod.Unturned.Players.Crafting.Events` namespace.

| **Event**                   | **Fired when**         | **Cancellable** |
|-----------------------------|------------------------|-----------------|
| UnturnedPlayerCraftingEvent | Before a player crafts | Yes             |

### Equipment
These events are stored within the `OpenMod.Unturned.Players.Equipment.Events` namespace.

| **Event**                          | **Fired when**                    | **Cancellable** |
|------------------------------------|-----------------------------------|-----------------|
| UnturnedPlayerItemEquippingEvent   | Before a player equips anything   | Yes             |
| UnturnedPlayerItemEquippedEvent    | After a player equips anything    | No              |
| UnturnedPlayerItemUnequippingEvent | Before a player unequips anything | Yes             |
| UnturnedPlayerItemUnequippedEvent  | After a player unequips anything  | No              |

### Inventory
These events are stored within the `OpenMod.Unturned.Players.Inventory.Events` namespace.

| **Event**                           | **Fired when**                                     | **Cancellable** |
|-------------------------------------|----------------------------------------------------|-----------------|
| UnturnedPlayerOpenedStorageEvent    | After a player opens a storage                     | No              |
| UnturnedPlayerClosedStorageEvent    | After a player closes a storage                    | No              |
| UnturnedPlayerTakingItemEvent       | Before a player takes an item from the ground      | Yes             |
| UnturnedPlayerDroppedItemEvent      | After a player drops an item                       | No              |
| UnturnedPlayerInventoryResizedEvent | After a player's inventory is resized              | No              |
| UnturnedPlayerInventoryUpdatedEvent | After a player's inventory updates                 | No              |
| UnturnedPlayerItemAddedEvent        | After an item is added to a player's inventory     | No              |
| UnturnedPlayerItemRemovedEvent      | After an item is removed from a player's inventory | No              |
| UnturnedPlayerItemUpdatedEvent      | After an item is updated in a player's inventory   | No              |

### Life
These events are stored within the `OpenMod.Unturned.Players.Life.Events` namespace.

| **Event**                   | **Fired when**                                 | **Cancellable** |
|-----------------------------|------------------------------------------------|-----------------|
| UnturnedPlayerDamagingEvent | Before a player is damaged                     | Yes             |
| UnturnedPlayerDamagedEvent  | After a player is damaged                      | No              |
| UnturnedPlayerDyingEvent    | Before a player takes a fatal amount of damage | Yes             |
| UnturnedPlayerDeadEvent     | After a player dies                            | No              |
| UnturnedPlayerSpawnedEvent  | After a player spawns (connects or is revived) | No              |
| UnturnedPlayerRevivedEvent  | After a player is revived                      | No              |

### Movement
These events are stored within the `OpenMod.Unturned.Players.Movement.Events` namespace.

| **Event**                         | **Fired when**                   | **Cancellable** |
|-----------------------------------|----------------------------------|-----------------|
| UnturnedPlayerGestureUpdatedEvent | After a player's gesture updates | No              |
| UnturnedPlayerStanceUpdatedEvent  | After a player's stance updates  | No              |
| UnturnedPlayerTeleportingEvent    | Before a player teleports        | Yes             |
| UnturnedPlayerSafetyUpdatedEvent  | After a player safezone updated  | No              |

### Skills
These events are stored within the `OpenMod.Unturned.Players.Skills.Events` namespace.

| **Event**                            | **Fired when**                      | **Cancellable** |
|--------------------------------------|-------------------------------------|-----------------|
| UnturnedPlayerExperienceUpdatedEvent | After a player's experience updates | No              |
| UnturnedPlayerReputationUpdatedEvent | After a player's reputation updates | No              |

### Stats
These events are stored within the `OpenMod.Unturned.Players.Stats.Events` namespace.

| **Event**                             | **Fired when**                               | **Cancellable** |
|---------------------------------------|----------------------------------------------|-----------------|
| UnturnedPlayerStatUpdatedEvent        | After any of a player's stat is updated      | No              |
| UnturnedPlayerBleedingUpdatedEvent    | After a player's bleeding stat is updated    | No              |
| UnturnedPlayerBrokenUpdatedEvent      | After a player's broken stat is updated      | No              |
| UnturnedPlayerFoodUpdatedEvent        | After a player's food stat is updated        | No              |
| UnturnedPlayerHealthUpdatedEvent      | After a player's health stat is updated      | No              |
| UnturnedPlayerLifeUpdatedEvent        | After a player's living state is updated     | No              |
| UnturnedPlayerOxygenUpdatedEvent      | After a player's oxygen stat is updated      | No              |
| UnturnedPlayerStaminaUpdatedEvent     | After a player's stamina is updated          | No              |
| UnturnedPlayerTemperatureUpdatedEvent | After a player's temperature stat is updated | No              |
| UnturnedPlayerVirusUpdatedEvent       | After a player's virus stat is updated       | No              |
| UnturnedPlayerVisionUpdatedEvent      | After a player's vision is updated           | No              |
| UnturnedPlayerWaterUpdatedEvent       | After a player's water stat is updated       | No              |

### Useables
These events are stored within the `OpenMod.Unturned.Players.Useables.Events` namespace.

| **Event**                        | **Fired when**                                 | **Cancellable** |
|----------------------------------|------------------------------------------------|-----------------|
| UnturnedPlayerPerformingAidEvent | Before a player performs aid on another player | Yes             |

### UI
These events are stored within the `OpenMod.Unturned.Players.UI.Events` namespace.

| **Event**                        | **Fired when**                                    | **Cancellable** |
|----------------------------------|---------------------------------------------------|-----------------|
| UnturnedPlayerButtonClickedEvent | After a player has clicked a UI button            | No              |
| UnturnedPlayerTextInputtedEvent  | After a player has inputted a text into a textbox | No              |

### Voice
These events are stored within the `OpenMod.Unturned.Players.Voice.Events` namespace.

| **Event**                         | **Fired when**                                    | **Cancellable** |
|-----------------------------------|---------------------------------------------------|-----------------|
| UnturnedPlayerTalkingUpdatedEvent | After a player has started or stoped talking      | No              |
| UnturnedPlayerRelayingVoiceEvent  | Before a player has started talking               | Yes             |
