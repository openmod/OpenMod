using OpenMod.Unturned.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Players.Voice.Events
{
    public class UnturnedPlayerTalkingUpdatedEvent : UnturnedPlayerEvent
    {
        public bool IsTalking { get; }
        public UnturnedPlayerTalkingUpdatedEvent(UnturnedPlayer player, bool isTalking) : base(player)
        {
            IsTalking = isTalking;
        }
    }
}
