using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities;

namespace TRPGGame.Managers
{
    /// <summary>
    /// Manager responsible for getting and setting player state.
    /// </summary>
    public class StateManager : IStateManager
    {
        public StateManager()
        {
            _playerGameState = new ConcurrentDictionary<Guid, string>();
        }

        private ConcurrentDictionary<Guid, string> _playerGameState;

        /// <summary>
        /// Gets the state of the player.
        /// </summary>
        /// <param name="playerId">The id of the player to get the state of.</param>
        /// <returns></returns>
        public string GetPlayerState(Guid playerId)
        {
            _playerGameState.TryGetValue(playerId, out string state);
            return state;
        }

        /// <summary>
        /// Sets the state of the player to "InCombat".
        /// </summary>
        /// <param name="playerId">The id of the player to set to "InCombat".</param>
        public void SetPlayerInCombat(Guid playerId)
        {
            _playerGameState.AddOrUpdate(playerId, PlayerStateConstants.InCombat, (id, state) => PlayerStateConstants.InCombat);
        }

        /// <summary>
        /// Sets the state of a player to "Free".
        /// </summary>
        /// <param name="playerId">The id of the player to set to "Free".</param>
        public void SetPlayerFree(Guid playerId)
        {
            // Fly away...
            _playerGameState.AddOrUpdate(playerId, PlayerStateConstants.Free, (id, state) => PlayerStateConstants.Free);
        }

        /// <summary>
        /// Sets the state of the player to "MakeCharacter".
        /// </summary>
        /// <param name="playerId">The id of the player to set to "MakeCharacter".</param>
        public void SetPlayerMakeCharacter(Guid playerId)
        {
            _playerGameState.AddOrUpdate(playerId, PlayerStateConstants.MakeCharacter, (id, state) => PlayerStateConstants.MakeCharacter);
        }

        /// <summary>
        /// Sets the state of the player to "MakeFormation".
        /// </summary>
        /// <param name="playerId">The id of the player to set to "MakeFormation".</param>
        public void SetPlayerMakeFormation(Guid playerId)
        {
            _playerGameState.AddOrUpdate(playerId, PlayerStateConstants.MakeFormation, (id, state) => PlayerStateConstants.MakeFormation);
        }
    }
}
