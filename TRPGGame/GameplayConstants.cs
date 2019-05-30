using System;
using System.Collections.Generic;
using System.Text;
using TRPGShared;

namespace TRPGGame
{
    public static class GameplayConstants
    {
        public const int GameTicksPerSecond = 10;
        public const int MaxFormationRows = 3;
        public const int MaxFormationColumns = 3;
        public const int MaxFormationSize = 9;
        public const int MaxFormationsInCombat = 2;
        public const int MaxFormationsPerSide = 3;
        public const int MaxActionsPerTurn = 5;
        public const int ActionPointsPerTurn = 20;
        public const int MaxTileMoveDistance = 1;
        public const double SecondsPerTurn = 40;
        public const double EndOfTurnDelayInSeconds = 1;
        public const int DefendingStatusEffectId = 3;
        public const int FleeingStatusEffectId = 4;

        public const int StartingMapId = 1;
        public static readonly Coordinate StartingPosition = new Coordinate { PositionX = 1, PositionY = 1 };

        /// <summary>
        /// The time in minutes until a player is logged off due to inactivity.
        /// </summary>
        public const int InactiveTimeoutDuration = 10;

        /// <summary>
        /// The guid that fills in the place of any spot that requires an ownerId for an AI owned entity.
        /// </summary>
        public static readonly Guid AiId = new Guid("6a89b96c-f3fc-4cc9-a80a-b45354189405");
    }
}
