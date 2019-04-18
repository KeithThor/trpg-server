using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities.Combat
{
    public class BattleAction
    {
        public int ActorId { get; set; }
        public Guid OwnerId { get; set; }
        public bool IsDefending { get; set; }
        public bool IsUsingItem { get; set; }
        public bool IsFleeing { get; set; }
        public int AbilityId { get; set; }
        public int TargetPosition { get; set; }
        public int TargetFormationId { get; set; }
    }
}
