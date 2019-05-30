using System;
using System.Collections.Generic;
using TRPGGame.Entities;
using TRPGGame.EventArgs;

namespace TRPGGame.Managers
{
    /// <summary>
    /// Manager responsible for creating and alerting listeners to battle events in one map.
    /// </summary>
    public interface IMapBattleManager
    {
        /// <summary>
        /// Event invoked whenever a Battle is created.
        /// </summary>
        event EventHandler<CreatedBattleEventArgs> OnCreatedBattle;

        /// <summary>
        /// Creates a new battle using the given attacking and defending WorldEntities.
        /// 
        /// <para>Will send the resulting IBattleManager instance through the OnCreatedBattle event.</para>
        /// </summary>
        /// <param name="attackers">An IEnumerable containing the attacking WorldEntities.</param>
        /// <param name="defenders">An IEnumerable containing the defending WorldEntities.</param>
        bool CreateBattle(IEnumerable<WorldEntity> attackers, IEnumerable<WorldEntity> defenders);

        /// <summary>
        /// Tries to get the IBattleManager instance for the given WorldEntity.
        /// </summary>
        /// <param name="entity">The WorldEntity to try to retrieve a battle for.</param>
        /// <param name="battleManager">The IBattleManager instance that is managing the player's battle.</param>
        /// <returns>Returns true if the IBattleManager was successfully retrieved.</returns>
        bool TryGetBattle(IReadOnlyWorldEntity entity, out IBattleManager battleManager);
    }
}