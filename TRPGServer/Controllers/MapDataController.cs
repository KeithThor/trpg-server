using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TRPGGame;
using TRPGServer.Services;

namespace TRPGServer.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class MapDataController : Controller
    {
        private readonly MapDataHandler _mapDataHandler;
        private readonly WorldEntityManager _entityManager;

        public MapDataController(MapDataHandler mapDataHandler,
                                WorldEntityManager entityManager)
        {
            _mapDataHandler = mapDataHandler;
            _entityManager = entityManager;
        }

        [HttpGet("Map")]
        public async Task<IActionResult> GetMap(int? mapId)
        {
            if (mapId == null)
            {
                var userGuid = Guid.Parse(User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
                var manager = _entityManager.GetPlayerEntityManager(userGuid);

                try
                {
                    mapId = manager.GetCurrentMap().Id;
                }
                catch (Exception ex)
                {
                    return BadRequest("Player has no assigned entity.");
                }
            }
            return Json(new
            {
                MapData = _mapDataHandler.GetMapData(mapId.Value),
                UniqueTiles = _mapDataHandler.GetUniqueMapTiles(mapId.Value),
                MapId = mapId
            });
        }
    }
}
