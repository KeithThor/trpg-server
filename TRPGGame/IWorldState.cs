using System;
using System.Collections.Generic;
using TRPGGame.Entities;
using TRPGGame.EventArgs;
using TRPGGame.Managers;

namespace TRPGGame
{
    /// <summary>
    /// Interface that is responsible for keeping track of and handling changes within the game
    /// world state.
    /// </summary>
    public interface IWorldState
    {
        /// <summary>
        /// Gets a dictionary containing all of the map managers this WorldState is handling.
        /// <para>Key is the id of the map the MapManager is managing.</para>
        /// </summary>
        IReadOnlyDictionary<int, MapManager> MapManagers { get; }

        /// <summary>
        /// Gets a dictionary containing all of the MapEntityManagers this WorldState is handling.
        /// <para>Key is the id of the map the MapEntityManager is managing.</para>
        /// </summary>
        IReadOnlyDictionary<int, MapEntityManager> MapEntityManagers { get; }

        /// <summary>
        /// Checks all maps for updates on their states.
        /// </summary>
        void CheckMapStates();
    }
}