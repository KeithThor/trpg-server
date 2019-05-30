using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Managers;

namespace TRPGGame.EventArgs
{
    public class CreatedBattleEventArgs
    {
        public CreatedBattleEventArgs(IBattleManager battleManager,
                                      HashSet<int> aiEntitiesInBattle,
                                      HashSet<Guid> playersInBattle)
        {
            BattleManager = battleManager;
            PlayersInBattle = playersInBattle;
            AiEntitiesInBattle = aiEntitiesInBattle;
        }

        public HashSet<Guid> PlayersInBattle { get; }
        public HashSet<int> AiEntitiesInBattle { get; }
        public IBattleManager BattleManager { get; }
    }
}
