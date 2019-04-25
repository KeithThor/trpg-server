using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities;
using TRPGGame.Entities.Data;
using TRPGGame.EventArgs;
using TRPGShared;

namespace TRPGGame.Managers
{
    /// <summary>
    /// Manager responsible for controlling non-player WorldEntities.
    /// </summary>
    public class AiEntityManager
    {
        private readonly WorldEntity _entity;
        private readonly IMapManager _mapManager;
        private readonly Random _rand;
        private readonly SpawnEntityData _spawnEntityData;

        public AiEntityManager(WorldEntity entity,
                               IMapManager mapManager,
                               SpawnEntityData spawnEntityData)
        {
            _entity = entity;
            _mapManager = mapManager;
            _spawnEntityData = spawnEntityData;
            _rand = new Random();

            AddToMap();
        }

        /// <summary>
        /// Event invoked whenever the WorldEntity this AiEntityManager controls is removed from the map.
        /// </summary>
        public event EventHandler<RemovedFromMapEventArgs> RemovedFromMap;

        /// <summary>
        /// On every second of game time, perform Ai logic.
        /// </summary>
        public void OnGameTick()
        {
            if (_rand.Next(1, 5) <= 1)
            {
                MoveEntity();
            }
        }

        /// <summary>
        /// Adds the WorldEntity this AiEntityManager controls to the map contained by the MapManager.
        /// </summary>
        private void AddToMap()
        {
            var spawnCoordinate = new Coordinate();

            do
            {
                do
                {
                    spawnCoordinate.PositionX = _rand.Next(0, _mapManager.Map.MapData.Count);
                    spawnCoordinate.PositionY = _rand.Next(0, _mapManager.Map.MapData[0].Count);
                } while (!_mapManager.IsValidLocation(spawnCoordinate));
            } while (!_mapManager.TryAddEnemyEntity(_entity, spawnCoordinate));

            _entity.Position = spawnCoordinate;
        }

        /// <summary>
        /// Moves the WorldEntity this AiEntityManager controls into a random location next to said
        /// Entity's current position.
        /// </summary>
        private void MoveEntity()
        {
            int option = _rand.Next(1, 5);
            var moveCoordinate = _entity.Position.Copy();
            switch (option)
            {
                case 1:
                    moveCoordinate.PositionX++;
                    break;
                case 2:
                    moveCoordinate.PositionX--;
                    break;
                case 3:
                    moveCoordinate.PositionY++;
                    break;
                default:
                    moveCoordinate.PositionY--;
                    break;
            }

            if (_mapManager.IsValidLocation(moveCoordinate))
            {
                bool success = _mapManager.TryMoveEnemyEntity(_entity, moveCoordinate, out IEnumerable<WorldEntity> hostiles);
                if (success) _entity.Position = moveCoordinate;
            }
        }
    }
}
