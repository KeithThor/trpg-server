﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.EventArgs
{
    public class EndOfBattleEventArgs
    {
        public IReadOnlyList<string> ParticipantIds { get; set; }
    }
}