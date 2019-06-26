using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TRPGGame.Entities;
using TRPGGame.Entities.Combat;

namespace TRPGServer.Models
{
    /// <summary>
    /// Represents a more lightweight version of an in-game battle instance.
    /// </summary>
    public class CulledBattle
    {
        public CulledBattle(IReadOnlyBattle battle)
        {
            var activeEntities = new List<ActiveEntities>();
            foreach (var kvp in battle.ActionsLeftPerFormation)
            {
                activeEntities.Add(new ActiveEntities
                {
                    FormationId = kvp.Key.Id,
                    EntityIds = kvp.Value.Select(entity => entity.Id).ToList(),
                    OwnerId = kvp.Key.OwnerId
                });
            }
            ActiveEntities = activeEntities;
            Id = battle.Id;
            Attackers = battle.Attackers;
            Defenders = battle.Defenders;
            IsDefenderTurn = battle.IsDefenderTurn;
            Round = battle.Round;
            SecondsLeftInTurn = (int)(battle.TurnExpiration - DateTime.Now).TotalSeconds;
        }

        public int Id { get; set; }
        public IEnumerable<IReadOnlyFormation> Attackers { get; set; }
        public IEnumerable<IReadOnlyFormation> Defenders { get; set; }
        public IEnumerable<ActiveEntities> ActiveEntities { get; set; }
        public bool IsDefenderTurn { get; set; }
        public int Round { get; set; }
        public int SecondsLeftInTurn { get; set; }
    }
}
