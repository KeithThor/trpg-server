using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Managers;

namespace TRPGGame.EventArgs
{
    public class JoinBattleSuccessEventArgs
    {
        public IBattleManager BattleManager { get; set; }
    }
}
