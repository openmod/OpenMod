extern alias JetBrainsAnnotations;
using System;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.Unturned.Events;
using SDG.Unturned;

// ReSharper disable InconsistentNaming

namespace OpenMod.Unturned.Players.Skills.Events
{
    [UsedImplicitly]
    internal class PlayerSkillsEventsListener : UnturnedEventsListener
    {
        public PlayerSkillsEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            PlayerSkills.OnExperienceChanged_Global += PlayerSkills_OnExperienceChanged_Global;
            PlayerSkills.OnReputationChanged_Global += PlayerSkills_OnReputationChanged_Global;
        }

        public override void Unsubscribe()
        {
            PlayerSkills.OnExperienceChanged_Global -= PlayerSkills_OnExperienceChanged_Global;
            PlayerSkills.OnReputationChanged_Global -= PlayerSkills_OnReputationChanged_Global;
        }

        private void PlayerSkills_OnExperienceChanged_Global(PlayerSkills skills, uint experience)
        {
            var player = GetUnturnedPlayer(skills.player)!;
            var @event = new UnturnedPlayerExperienceUpdatedEvent(player, experience);

            Emit(@event);
        }

        private void PlayerSkills_OnReputationChanged_Global(PlayerSkills skills, int reputation)
        {
            var player = GetUnturnedPlayer(skills.player)!;

            var @event = new UnturnedPlayerReputationUpdatedEvent(player, reputation);

            Emit(@event);
        }
    }
}