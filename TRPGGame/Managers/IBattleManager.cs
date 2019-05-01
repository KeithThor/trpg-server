using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TRPGGame.Entities;
using TRPGGame.Entities.Combat;
using TRPGGame.EventArgs;

namespace TRPGGame.Managers
{
    /// <summary>
    /// Manager responsible for handling Battle-related functions for a single battle instance.
    /// </summary>
    public interface IBattleManager
    {
        /// <summary>
        /// Event handler called to signal that the battle is over.
        /// </summary>
        event EventHandler<EndOfBattleEventArgs> EndOfBattleEvent;

        /// <summary>
        /// Event handler called after the end of turn events happen.
        /// </summary>
        event EventHandler<EndOfTurnEventArgs> EndOfTurnEvent;

        /// <summary>
        /// Event handler called after the start of turn events happen.
        /// </summary>
        event EventHandler<StartOfTurnEventArgs> StartOfTurnEvent;

        /// <summary>
        /// Event handler called whenever a new Formation joins the battle.
        /// </summary>
        event EventHandler<JoinBattleEventArgs> JoinBattleEvent;

        /// <summary>
        /// Event handler called whenever an action is successfully executed in battle.
        /// </summary>
        event EventHandler<SuccessfulActionEventArgs> SuccessfulActionEvent;

        /// <summary>
        /// Adds a participant to the battle and returns the battle instance.
        /// <para>Returns null if no battles were found or the battle group is full.</para>
        /// </summary>
        /// <param name="participant">The Formation to add to the battle.</param>
        /// <param name="isAttacker">If true, will join the side of the attackers. If false, will join the defenders.</param>
        /// <returns></returns>
        IReadOnlyBattle JoinBattle(Formation participant, bool isAttacker);

        /// <summary>
        /// Tries to perform an action, returning true if the action was successfully performed.
        /// </summary>
        /// <param name="action">The action to perform, containing data about the actor and the abilities used.</param>
        /// <returns></returns>
        Task<bool> PerformActionAsync(BattleAction action);

        /// <summary>
        /// Starts a Battle instance between the attackers and defenders.
        /// <para>Returns null if a Battle instance already exists for this manager.</para>
        /// </summary>
        /// <param name="attackers">The Formations who initiated combat.</param>
        /// <param name="defenders">The Formations who are being initiated on.</param>
        /// <returns></returns>
        IReadOnlyBattle StartBattle(List<Formation> attackers, List<Formation> defenders);

        /// <summary>
        /// Gets the current Battle instance.
        /// </summary>
        /// <returns></returns>
        IReadOnlyBattle GetBattle();

        /// <summary>
        /// Returns the ids of all players participating in battle.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetParticipantIds();
    }
}