using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TRPGServer.Services
{
    /// <summary>
    /// Class containing all map listeners for each map of the game.
    /// </summary>
    public class MapListenerContainer
    {
        public IEnumerable<MapEntityListener> Listeners { get; set; }

        /// <summary>
        /// Returns the DisplayEntityStore for a given map.
        /// </summary>
        /// <param name="mapId">The id of the map for the DisplayEntityStore.</param>
        /// <returns></returns>
        public IDisplayEntityStore GetDisplayEntityStore(int mapId)
        {
            return Listeners.FirstOrDefault(store => store.MapId == mapId.ToString());
        }
    }
}
