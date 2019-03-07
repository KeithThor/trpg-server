using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TRPGGame;
using TRPGGame.Entities;

namespace TRPGServer.Services
{
    public class MapDataHandler
    {
        private readonly IWorldState _worldState;

        public MapDataHandler(IWorldState worldState)
        {
            _worldState = worldState;
        }

        /// <summary>
        /// Gets the map data for a given map id.
        /// </summary>
        /// <param name="mapId">The id of the map to get map data for.</param>
        /// <returns>A 2d Ienumerable containing the the ids of map tiles for the given map.</returns>
        public IEnumerable<IEnumerable<int>> GetMapData(int mapId)
        {
            if (!_worldState.MapManagers.ContainsKey(mapId))
            {
                return null;
            }
            // Create 2d Ienumerable of maptile ids
            return _worldState.MapManagers[mapId].Map.MapData.Select(mapTileRow => 
                                                             mapTileRow.Select(mapTile => mapTile.Id));
        }

        /// <summary>
        /// Gets unique map tiles for a given map id.
        /// </summary>
        /// <param name="mapId">The map id to get unique map tile symbols for.</param>
        /// <returns></returns>
        public IEnumerable<IReadOnlyMapTile> GetUniqueMapTiles(int mapId)
        {
            if (!_worldState.MapManagers.ContainsKey(mapId))
            {
                return null;
            }
            return _worldState.MapManagers[mapId].Map.UniqueTiles;
        }
    }
}
