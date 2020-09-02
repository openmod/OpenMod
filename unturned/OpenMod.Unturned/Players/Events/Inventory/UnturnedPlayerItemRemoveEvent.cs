﻿using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Inventory
{
    public class UnturnedPlayerItemRemoveEvent : UnturnedPlayerEvent
    {
        public byte Page { get; }

        public byte Index { get; }

        public ItemJar ItemJar { get; }

        public UnturnedPlayerItemRemoveEvent(UnturnedPlayer player, byte page, byte index, ItemJar itemJar) : base(player)
        {
            Page = page;
            Index = index;
            ItemJar = itemJar;
        }
    }
}