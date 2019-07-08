using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TRPGGame.Entities;

namespace TRPGGame.Services
{
    /// <summary>
    /// Static class responsible for translating target positions from one point to another.
    /// </summary>
    public static class FormationTargeter
    {
        /// <summary>
        /// Returns an IEnumerable of CombatEntities who are the targets of the given ability.
        /// </summary>
        /// <param name="ability">The ability to return the targets of.</param>
        /// <param name="targetPosition">The target position of the ability.</param>
        /// <param name="targetFormation">The formation to get the targets from.</param>
        /// <returns></returns>
        public static IEnumerable<CombatEntity> GetTargets(Ability ability,
                                                           int targetPosition,
                                                           Formation targetFormation)
        {
            var targets = new List<CombatEntity>();
            if (ability.IsPositionStatic)
            {
                for (int i = 0; i < targetFormation.Positions.Length; i++)
                {
                    for (int j = 0; j < targetFormation.Positions[i].Length; j++)
                    {
                        if (ability.Targets.Contains(i * GameplayConstants.MaxFormationColumns + j + 1))
                        {
                            targets.Add(targetFormation.Positions[i][j]);
                        }
                    }
                }
            }
            else if (ability.CanTargetBeBlocked)
            {
                // Check column position, if first in column, can't be blocked. If not, check all positions in front
                // of row to see if another entity exists. If so, attack is blocked.
                var column = GetColumn(targetPosition);
                if (column != 1)
                {
                    var row = GetRow(targetPosition);
                    var entityRow = targetFormation.Positions[row - 1];
                    for (int i = 0; i < column - 1; i++)
                    {
                        if (entityRow[i] != null) return null;
                    }
                }
            }
            var translatedTargets = TranslateTargets(ability.Targets, ability.CenterOfTargets, targetPosition);
            for (int i = 0; i < targetFormation.Positions.Length; i++)
            {
                for (int j = 0; j < targetFormation.Positions[i].Length; j++)
                {
                    if (translatedTargets.Contains(i * GameplayConstants.MaxFormationRows + j + 1))
                    {
                        if (targetFormation.Positions[i][j] != null) targets.Add(targetFormation.Positions[i][j]);
                    }
                }
            }
            return targets;
        }

        /// <summary>
        /// Translates a set of targets from one position to another.
        /// </summary>
        /// <param name="targets">An IEnumerable containing the target positions of a formation.</param>
        /// <param name="centerOfPosition">The original center of targets position.</param>
        /// <param name="newCenter">The new center of targets position to translate to.</param>
        /// <returns></returns>
        public static IEnumerable<int> TranslateTargets(IEnumerable<int> targets,
                                                  int centerOfPosition,
                                                  int newCenter)
        {
            var newTargets = targets.ToList();
            int change = centerOfPosition - newCenter;
            int rowDifference = GetRowDifference(centerOfPosition, newCenter);
            int columnDifference = GetColumnDifference(centerOfPosition, newCenter);

            var toRemove = new List<int>();
            foreach (var position in newTargets)
            {
                var valRowDiff = GetRowDifference(position, position - change);
                var valRowPosition = GetRow(position) - valRowDiff;
                var valColDiff = GetColumnDifference(position, position - change);
                var valColPosition = GetColumn(position) - valColDiff;

                // If new row doesn't exist, remove this position
                if (valRowPosition < 1 || valRowPosition > GameplayConstants.MaxFormationRows) toRemove.Add(position);
                // If the change in rows is not the same as the center of targets, this position is removed
                else if (rowDifference != valRowDiff) toRemove.Add(position);

                else if (valColPosition < 1 || valColPosition > GameplayConstants.MaxFormationColumns) toRemove.Add(position);
                else if (columnDifference != valColDiff) toRemove.Add(position);
            }

            newTargets.RemoveAll(target => toRemove.Contains(target));

            newTargets = newTargets.Select(target => target - change).ToList();

            return newTargets;
        }

        /// <summary>
        /// Gets the difference in rows from one position to another position.
        /// </summary>
        /// <param name="position">The original position.</param>
        /// <param name="newPosition">The new position.</param>
        /// <returns></returns>
        public static int GetRowDifference(int position, int newPosition)
        {
            int initialRow = GetRow(position);
            int newRow = GetRow(newPosition);

            return initialRow - newRow;
        }

        /// <summary>
        /// Gets the difference in columns from one position to another position.
        /// </summary>
        /// <param name="position">The original position.</param>
        /// <param name="newPosition">The new position.</param>
        /// <returns></returns>
        public static int GetColumnDifference(int position, int newPosition)
        {
            int initialColumn = GetColumn(position);
            int newColumn = GetColumn(newPosition);

            return initialColumn - newColumn;
        }

        /// <summary>
        /// Gets an integer that represents the row number a position exists in.
        /// </summary>
        /// <param name="position">The position to get the row number for.</param>
        /// <returns></returns>
        public static int GetRow(int position)
        {
            return (position - 1) / GameplayConstants.MaxFormationColumns + 1;
        }

        /// <summary>
        /// Gets an integer that represents the column number a position exists in.
        /// </summary>
        /// <param name="position">The position to get the column number for.</param>
        /// <returns></returns>
        public static int GetColumn(int position)
        {
            return (position - 1) % GameplayConstants.MaxFormationRows + 1;
        }
    }
}
