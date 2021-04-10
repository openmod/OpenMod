using JetBrains.Annotations;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Players.Voice.Events
{
    [UsedImplicitly]
    internal class PlayerVoiceEventsListener : UnturnedPlayerEventsListener
    {
        public PlayerVoiceEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            PlayerVoice.onRelayVoice += PlayerVoice_onRelayVoice;
        }

        private void PlayerVoice_onRelayVoice(PlayerVoice speaker, bool wantsToUseWalkieTalkie, ref bool shouldAllow, ref bool shouldBroadcastOverRadio, ref PlayerVoice.RelayVoiceCullingHandler cullingHandler)
        {
            var player = GetUnturnedPlayer(speaker.player)!;

            var @event = new UnturnedPlayerRelayingVoiceEvent(player, wantsToUseWalkieTalkie, shouldBroadcastOverRadio)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldBroadcastOverRadio = @event.ShouldBroadcastOverRadio;
            shouldAllow = !@event.IsCancelled;
        }

        public override void Unsubscribe()
        {
            PlayerVoice.onRelayVoice -= PlayerVoice_onRelayVoice;
        }

        public override void SubscribePlayer(Player player)
        {
            player.voice.onTalkingChanged += (isTalking) => OnTalkingChanged(player, isTalking);
        }

        private void OnTalkingChanged(Player nativePlayer, bool isTalking)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;

            var @event = new UnturnedPlayerTalkingUpdatedEvent(player, isTalking);

            Emit(@event);
        }

        public override void UnsubscribePlayer(Player player)
        {
            player.voice.onTalkingChanged -= (isTalking) => OnTalkingChanged(player, isTalking);
        }
    }
}
