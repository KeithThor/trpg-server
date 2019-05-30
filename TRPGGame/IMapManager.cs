using System;
using System.Collections.Generic;
using TRPGGame.Entities;
using TRPGGame.EventArgs;
using TRPGShared;

namespace TRPGGame
{
    /// <summary>
    /// Manager responsible for managing events and interactions between entities in a given map.
    /// </summary>
    public interface IMapManager
    {
        /// <summary>
        /// The map that this MapManager is responsible for managing.
        /// </summary>
        IReadOnlyMap Map { get; }

        /// <summary>
        /// Event invoked every game update if the map state has changed.
        /// </summary>
        event EventHandler<MapStateChangedArgs> MapStateChanged;

        /// <summary>
        /// Event invoked whenever one or more WorldEntities are added to the map.
        /// </summary>
        event EventHandler<WorldEntityAddedArgs> WorldEntityAdded;

        /// <summary>
        /// Event invoked whenever one or more WorldEntities are removed from the map.
        /// </summary>
        event EventHandler<WorldEntityRemovedArgs> WorldEntityRemoved;

        /// <summary>
        /// Event invoked on every game tick.
        /// </summary>
        event EventHandler<GameTickEventArgs> GameTick;
        
        /// <summary>
        /// Removes the entity with the given id from this map.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        void RemoveEntity(WorldEntity entity);

        /// <summary>
        /// Tries to add an entity to the current map to the requested position. Will return true if adding succeeded.
        /// </summary>
        /// <param name="entity">The entity to add to the map.</param>
        /// <param name="location">The location to spawn the entity at.</param>
        bool TryAddEntity(WorldEntity entity, Coordinate location);
        
        /// <summary>
        /// Tries to move an entity to a given location; returns true if the move is successful. Also returns an IEnumerable of
        /// IReadOnlyWorldEntities if any hostile targets were touched.
        /// </summary>
        /// <param name="entity">The entity to try to move.</param>
        /// <param name="newLocation">The new location to try to move to.</param>
        /// <param name="contacts">Will be populated with a List of WorldEntities any contact was made.</param>
        /// <returns>Returns whether the move was successful.</returns>
        bool TryMoveEntity(WorldEntity entity, Coordinate newLocation, out IEnumerable<WorldEntity> hostileContacts);

        /// <summary>
        /// Returns true if a given position is valid for the current map.
        /// </summary>
        /// <param name="position">The coordinate to check.</param>
        /// <returns></returns>
        bool IsValidLocation(Coordinate position);
    }
}