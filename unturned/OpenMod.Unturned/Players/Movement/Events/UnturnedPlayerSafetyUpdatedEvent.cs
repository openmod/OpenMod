using OpenMod.Unturned.Events;

namespace OpenMod.Unturned.Players.Movement.Events
{
    public class UnturnedPlayerSafetyUpdatedEvent : UnturnedPlayerEvent
    {
        public bool IsSafe { get; }
        public UnturnedPlayerSafetyUpdatedEvent(UnturnedPlayer player, bool safe) : base(player)
        {
            IsSafe = safe;
        }
    }
}
