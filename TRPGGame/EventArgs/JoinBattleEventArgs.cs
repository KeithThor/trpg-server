using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities;
using TRPGGame.Entities.Combat;

namespace TRPGGame.EventArgs
{
    public class JoinBattleEventArgs
    {
        public IReadOnlyFormation JoinedFormation { get; set; }
        public bool IsAttacker { get; set; }
        public IReadOnlyList<string> ParticipantIds { get; set; }
        public List<ActiveEntities> ActiveEntities { get; set; }
    }
}
