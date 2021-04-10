using OpenMod.Unturned.Events;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Players.Skills.Events
{
    public class UnturnedPlayerReputationUpdatedEvent : UnturnedPlayerEvent
    {
        public int Reputation { get; }
        public UnturnedPlayerReputationUpdatedEvent(UnturnedPlayer player, int reputation) : base(player)
        {
            Reputation = reputation;
        }
    }
}
