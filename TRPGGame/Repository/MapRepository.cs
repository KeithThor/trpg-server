﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TRPGGame.Entities;
using TRPGGame.Entities.Data;
using TRPGShared;

namespace TRPGGame.Repository
{
    public class MapRepository : IRepository<Map>
    {
        /// <summary>
        /// An object representing a single instance of transportation data for a map.
        /// </summary>
        private class Transport
        {
            /// <summary>
            /// Contains the unique id for the transport in a map.
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// Contains the id of the map this transport will take an entity to.
            /// </summary>
            public int TransportMapId { get; set; }

            /// <summary>
            /// Contains the coordinate the entity will appear at or around when transported.
            /// </summary>
            public Coordinate TransportTo { get; set; }

            /// <summary>
            /// Contains the coordinate of the maptile that uses this transport.
            /// </summary>
            public Coordinate TransportFrom { get; set; }
        }

        /// <summary>
        /// A data object containing the amount of a given enemy formation that exists in the current map.
        /// </summary>
        private class EnemyData
        {
            /// <summary>
            /// The id of the EnemyFormationTemplate that represents this data object.
            /// </summary>
            public int FormationId { get; set; }

            /// <summary>
            /// The amount of enemies that should exist on this map of the EnemyFormationTemplate the class is based off of.
            /// </summary>
            public int Amount { get; set; }

            public int RespawnTime { get; set; }
        }

        private readonly IRepository<MapTile> _mapTileRepo;
        private readonly IRepository<AiFormationTemplate> _enemyFormationTemplate;
        private List<Map> _maps;

        public MapRepository(IRepository<MapTile> mapTileRepo,
                             IRepository<AiFormationTemplate> enemyFormationTemplate)
        {
            _mapTileRepo = mapTileRepo;
            _enemyFormationTemplate = enemyFormationTemplate;
        }

        public async Task<IEnumerable<Map>> GetDataAsync()
        {
            if (_maps == null)
            {
                await LoadDataAsync();
            }

            return _maps;
        }

        private async Task LoadDataAsync()
        {
            using (var reader = new StreamReader(DataConstants.AssemblyLocation + "/Data/Maps.json"))
            {
                _maps = new List<Map>();
                var mapTiles = await _mapTileRepo.GetDataAsync();
                var formations = await _enemyFormationTemplate.GetDataAsync();

                JContainer mapsAsList = JsonConvert.DeserializeObject<JContainer>(reader.ReadToEnd());
                foreach (var mapObject in mapsAsList)
                {
                    var map = mapObject.ToObject<Map>();
                    var mapConnections = new List<int>();
                    List<Transport> transports = null;
                    if (mapObject["transportAssignments"] != null)
                    {
                        transports = mapObject["transportAssignments"].ToObject<List<Transport>>();
                        foreach (var transport in transports)
                        {
                            mapConnections.Add(transport.TransportMapId);
                        }
                        map.MapConnections = mapConnections;
                    }

                    if (mapObject["enemies"] != null)
                    {
                        var spawnData = new List<SpawnEntityData>();
                        var enemyData = mapObject["enemies"].ToObject<List<EnemyData>>();

                        foreach (var enemy in enemyData)
                        {
                            var foundTemplate = formations.FirstOrDefault(f => f.Id == enemy.FormationId);
                            if (foundTemplate != null)
                            {
                                spawnData.Add(new SpawnEntityData
                                {
                                    FormationTemplate = foundTemplate,
                                    MaxEntities = enemy.Amount,
                                    RespawnTime = enemy.RespawnTime
                                });
                            }
                        }
                        map.SpawnData = spawnData;
                    }

                    if (mapObject["mapTileIds"] != null)
                    {
                        var mapTileIds = mapObject["mapTileIds"].ToObject<List<List<int>>>();
                        var uniqueTiles = new List<MapTile>();
                        var mapData = new List<List<MapTile>>();

                        for(int i = 0; i < mapTileIds.Count(); i++)
                        {
                            mapData.Add(new List<MapTile>());
                            for (int j = 0; j < mapTileIds[i].Count(); j++)
                            {
                                var foundTile = mapTiles.First(tile => tile.Id == mapTileIds[i][j]);
                                if (!uniqueTiles.Contains(foundTile))
                                    uniqueTiles.Add(mapTiles.First(tile => tile.Id == mapTileIds[i][j]));

                                if (foundTile.CanTransport && transports != null)
                                {
                                    foundTile = new MapTile(foundTile);
                                    var transport = transports.FirstOrDefault
                                        (tr => tr.TransportFrom.PositionX == i && tr.TransportFrom.PositionY == j);

                                    if (transport != null)
                                    {
                                        foundTile.TransportLocation = transport.TransportTo;
                                        foundTile.TransportMapId = transport.TransportMapId;
                                    }
                                }
                                mapData[i].Add(foundTile);
                            }
                        }

                        map.MapData = mapData;
                        map.UniqueTiles = uniqueTiles;
                    }
                    _maps.Add(map);
                }
            }
        }
    }
}
