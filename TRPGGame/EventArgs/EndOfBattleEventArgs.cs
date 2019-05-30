using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.EventArgs
{
    public class EndOfBattleEventArgs
    {
        public IReadOnlyList<string> ParticipantIds { get; set; }
        public IReadOnlyList<int> AiWorldEntityIds { get; set; }
        public bool DidAttackersWin { get; set; }
    }
}
