using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities;

namespace TRPGGame.EventArgs
{
    public class JoinBattleEventArgs
    {
        public IReadOnlyFormation JoinedFormation { get; set; }
        public bool IsAttacker { get; set; }
        public IEnumerable<string> ParticipantIds { get; set; }
    }
}
