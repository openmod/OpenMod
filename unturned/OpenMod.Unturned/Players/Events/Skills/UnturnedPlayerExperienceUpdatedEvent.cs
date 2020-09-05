﻿using OpenMod.Unturned.Events;

namespace OpenMod.Unturned.Players.Events.Skills
{
    public class UnturnedPlayerExperienceUpdatedEvent : UnturnedPlayerEvent
    {
        public uint Experience { get; }

        public UnturnedPlayerExperienceUpdatedEvent(UnturnedPlayer player, uint experience) : base(player)
        {
            Experience = experience;
        }
    }
}
