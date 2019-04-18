using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities;
using TRPGGame.Entities.Combat;

namespace TRPGGame.EventArgs
{
    public class EndOfTurnEventArgs
    {
        public List<IReadOnlyAbility> DelayedAbilities { get; set; }
        public List<IReadOnlyCombatEntity> AffectedEntities { get; set; }
        public IReadOnlyList<string> ParticipantIds { get; set; }
    }
}
