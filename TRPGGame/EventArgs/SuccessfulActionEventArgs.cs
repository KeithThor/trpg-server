using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities;
using TRPGGame.Entities.Combat;

namespace TRPGGame.EventArgs
{
    public class SuccessfulActionEventArgs
    {
        public IEnumerable<IReadOnlyCombatEntity> AffectedEntities { get; set; }
        public IReadOnlyAbility Ability { get; set; }
        public BattleAction Action { get; set; }
        public IReadOnlyCombatEntity Actor { get; set; }
        public int NextActiveEntityId { get; set; }
        public List<string> ParticipantIds { get; set; }
    }
}
