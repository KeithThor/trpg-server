using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TRPGGame.Repository;

namespace TRPGGame
{
    /// <summary>
    /// Class that provides gameplay configuration data from a json file.
    /// </summary>
    public class GameConfig
    {
        public GameConfig()
        {
        }

        public readonly int GameTicksPerSecond = 10;
        public const int MaxFormationRows = 3;
        public const int MaxFormationColumns = 3;
        public const int MaxFormationSize = 9;
        public const int MaxFormationsInCombat = 2;
        public const int MaxFormationsPerSide = 3;
        public const int MaxActionsPerTurn = 5;
        public const int ActionPointsPerTurn = 20;
        public const int MaxTileMoveDistance = 1;
        public const double SecondsPerTurn = 15;
        public const double EndOfTurnDelayInSeconds = 1;
        public const int DefendingStatusEffectId = 3;
        public const int FleeingStatusEffectId = 4;
    }
}
