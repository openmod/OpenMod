using OpenMod.Extensions.Games.Abstractions.Acl;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Players;
using SDG.Unturned;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Building
{
    public class UnturnedBuildableOwnership : IOwnership
    {
        private readonly StructureData? m_Structure;
        private readonly BarricadeData? m_Barricade;

        public bool HasOwner
        {
            get { return PlayerId != 0 || GroupId != 0; }
        }

        public string OwnerPlayerId
        {
            get { return PlayerId.ToString(); }
        }

        public string OwnerGroupId
        {
            get { return GroupId.ToString(); }
        }

        private ulong PlayerId
        {
            get { return m_Barricade?.owner ?? m_Structure!.owner; }
        }

        private ulong GroupId
        {
            get { return m_Barricade?.group ?? m_Structure!.group; }
        }

        public UnturnedBuildableOwnership(BarricadeData barricade)
        {
            m_Barricade = barricade;
        }

        public UnturnedBuildableOwnership(StructureData structure)
        {
            m_Structure = structure;
        }

        public Task<bool> HasAccessAsync(IPlayer player)
        {
            if (player is not UnturnedPlayer unturnedPlayer)
                return Task.FromResult(false);

            if (!HasOwner)
                return Task.FromResult(true);

            //Unturned Code to check access

            //!this.isLocked || enemyPlayer == this.owner || this.group != CSteamID.Nil && enemyGroup == this.group
            //EnemyPlayer => PlayerId
            //EnemyGroup => Always quests.Group (there is not group for steam and other for quests)
            //This -> barricade obj in example case

            if (PlayerId == unturnedPlayer.SteamId.m_SteamID)
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(GroupId != 0 && GroupId == unturnedPlayer.Player.quests.groupID.m_SteamID);
        }
    }
}