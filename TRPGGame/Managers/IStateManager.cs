using System;

namespace TRPGGame.Managers
{
    /// <summary>
    /// Manager responsible for getting and setting player state.
    /// </summary>
    public interface IStateManager
    {
        /// <summary>
        /// Gets the state of the player.
        /// </summary>
        /// <param name="playerId">The id of the player to get the state of.</param>
        /// <returns></returns>
        string GetPlayerState(Guid playerId);

        /// <summary>
        /// Sets the state of a player to "Free".
        /// </summary>
        /// <param name="playerId">The id of the player to set to "Free".</param>
        void SetPlayerFree(Guid playerId);

        /// <summary>
        /// Sets the state of the player to "InCombat".
        /// </summary>
        /// <param name="playerId">The id of the player to set to "InCombat".</param>
        void SetPlayerInCombat(Guid playerId);

        /// <summary>
        /// Sets the state of the player to "MakeCharacter".
        /// </summary>
        /// <param name="playerId">The id of the player to set to "MakeCharacter".</param>
        void SetPlayerMakeCharacter(Guid playerId);

        /// <summary>
        /// Sets the state of the player to "MakeFormation".
        /// </summary>
        /// <param name="playerId">The id of the player to set to "MakeFormation".</param>
        void SetPlayerMakeFormation(Guid playerId);
    }
}