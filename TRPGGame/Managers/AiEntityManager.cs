using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IMapBattleManager _mapBattleManager;
        private IBattleManager _battleManager;
        private readonly Random _rand;
        private readonly SpawnEntityData _spawnEntityData;
        private bool _isMovementDisabled = false;

        public AiEntityManager(WorldEntity entity,
                               IMapManager mapManager,
                               IMapBattleManager mapBattleManager,
                               SpawnEntityData spawnEntityData)
        {
            _entity = entity;
            _mapManager = mapManager;
            _mapBattleManager = mapBattleManager;
            _spawnEntityData = spawnEntityData;
            _rand = new Random();
            _mapBattleManager.OnCreatedBattle += OnCreatedBattle;

            AddToMap();
        }

        /// <summary>
        /// Event invoked whenever the WorldEntity this AiEntityManager controls is removed from the map.
        /// </summary>
        public event EventHandler<RemovedFromMapEventArgs> RemovedFromMap;

        /// <summary>
        /// Handler invoked whenever a battle is created for any entities on the current map.
        /// <para>Checks if the WorldEntity being managed is in the newly created battle.</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnCreatedBattle(object sender, CreatedBattleEventArgs args)
        {
            if (args.AiEntitiesInBattle.Contains(_entity.Id))
            {
                if (_battleManager != null)
                    throw new Exception($"A battle already exists for ai WorldEntity of id {_entity.Id} on map id {_mapManager.Map.Id}!");

                _isMovementDisabled = true;
                _battleManager = args.BattleManager;
                _battleManager.EndOfBattleEvent += OnEndOfBattle;
            }
        }

        /// <summary>
        /// Handler invoked at the end of a battle. If all CombatEntities in the formation this AiEntityManager controls
        /// are dead, queues this manager for destruction.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnEndOfBattle(object sender, EndOfBattleEventArgs args)
        {
            var areAllEntitiesDead = _entity.ActiveFormation.Positions
                                            .AnyTwoD(entity => entity != null && entity.Resources.CurrentHealth > 0);

            if (areAllEntitiesDead)
            {
                _mapManager.RemoveEnemyEntity(_entity);
                _mapBattleManager.OnCreatedBattle -= OnCreatedBattle;
                RemovedFromMap?.Invoke(this, new RemovedFromMapEventArgs
                {
                    SpawnData = _spawnEntityData
                });
            }

            _battleManager.EndOfBattleEvent -= OnEndOfBattle;
        }

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
            if (_isMovementDisabled) return;

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
                bool success = _mapManager.TryMoveEnemyEntity(_entity, moveCoordinate, out IEnumerable<WorldEntity> contacts);
                if (success) _entity.Position = moveCoordinate;

                if (contacts != null && contacts.Count() > 0)
                {
                    var hostiles = contacts.Where(entity => entity.OwnerGuid != GameplayConstants.AiId);
                    _mapBattleManager.CreateBattle(hostiles.Take(1), new List<WorldEntity> { _entity });
                }
            }
        }
    }
}
