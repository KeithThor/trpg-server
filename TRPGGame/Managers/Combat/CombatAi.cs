using System;
using System.Collections.Generic;
using System.Linq;
using TRPGGame.Entities;
using TRPGGame.Managers.Combat.Interfaces;
using TRPGGame.Services;
using TRPGShared;

namespace TRPGGame.Managers.Combat
{
    /// <summary>
    /// Basic combat ai that evaluates all possible choices of actions and returns the best choice
    /// modified by the AiRandomnessFactor of the provided Formation.
    /// </summary>
    public class CombatAi: ICombatAi
    {
        /// <summary>
        /// Evaluation data for a Formation.
        /// </summary>
        private class FormationEvaluation
        {
            /// <summary>
            /// Contains the integer-converted value of the Coordinate for this evaluation.
            /// </summary>
            public int TargetLocation { get; set; }

            /// <summary>
            /// The total value of the TargetLocation for a given Formation.
            /// </summary>
            public int TotalValue { get; set; }
        }

        /// <summary>
        /// Evaluation data for an Ability.
        /// </summary>
        private class AbilityEvaluation
        {
            /// <summary>
            /// Contains the highest-value target location for the Ability and Formation in this evaluation.
            /// </summary>
            public FormationEvaluation FormationEvaluation { get; set; }

            /// <summary>
            /// The ability that was evaluated.
            /// </summary>
            public Ability Ability { get; set; }

            /// <summary>
            /// The target Formation used for this evaluation.
            /// </summary>
            public Formation TargetFormation { get; set; }
        }

        /// <summary>
        /// Comparer used to allow SortedList to store duplicate keys.
        /// </summary>
        private class DuplicateKeyComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                int result = x.CompareTo(y);

                if (result == 0)
                    return 1;   // Handle equality as beeing greater
                else
                    return result;
            }
        }

        public CombatAi()
        {
            _rand = new Random();
        }

        private Random _rand;
        private Formation _myFormation;
        private CombatEntity _myActiveEntity;
        private IEnumerable<Formation> _myAllies;
        private IEnumerable<Formation> _myEnemies;

        private const int UnderParThreshold = 10;

        /// <summary>
        /// Provided the state of combat, makes a decision for the provided CombatEntity on which Ability it should
        /// use and where.
        /// </summary>
        /// <param name="myFormation">The Formation the active entity belongs to.</param>
        /// <param name="myActiveEntity">The CombatEntity to make a decision for.</param>
        /// <param name="myAllies">The Formations that are allies with the CombatEntity provided.</param>
        /// <param name="myEnemies">The Formations that are enemies with the CombatEntity provided.</param>
        /// <returns></returns>
        public CombatAiDecision MakeDecision(Formation myFormation,
                                             CombatEntity myActiveEntity,
                                             IEnumerable<Formation> myAllies,
                                             IEnumerable<Formation> myEnemies)
        {
            _myFormation = myFormation;
            _myActiveEntity = myActiveEntity;
            _myAllies = myAllies;
            _myEnemies = myEnemies;

            return EvaluateEntity(myActiveEntity);
        }

        /// <summary>
        /// Makes a CombatAiDecision for the provided CombatEntity.
        /// <para>Will choose the best Ability, target Formation, and target Coordinate unless the AiRandomnessFactor
        /// for the provided CombatEntity's Formation is not 0.</para>
        /// </summary>
        /// <param name="activeEntity">The CombatEntity to make a decision for.</param>
        /// <returns>A decision on what Ability to use and where in combat.</returns>
        private CombatAiDecision EvaluateEntity(CombatEntity activeEntity)
        {
            var comparer = new DuplicateKeyComparer();
            var evaluations = new SortedList<int, AbilityEvaluation>(comparer);

            // Get any abilities not in the CombatEntity's cooldowns list
            var useableAbilities = activeEntity.Abilities.Where(ability =>
            {
                return !activeEntity.AbilitiesOnCooldown.Any(kvp => kvp.Key.Id == ability.Id);
            });

            foreach (var ability in useableAbilities)
            {
                var abilityEvaluation = EvaluateAbility(activeEntity, ability);
                if (abilityEvaluation == null) continue;

                evaluations.Add(abilityEvaluation.FormationEvaluation.TotalValue, abilityEvaluation);
            }

            // Checks if any abilities are under the desired value
            bool areAbilitiesUnderPar = evaluations.Keys.Any(value => value <= UnderParThreshold);

            // If there are no valid abilities or all abilities are under par, perform a defend action instead
            if (evaluations.Count() == 0 || areAbilitiesUnderPar)
            {
                return new CombatAiDecision
                {
                    IsDefending = true
                };
            }

            // If no ai randomness, choose highest value AbilityEvaluation
            if (_myFormation.AiRandomness <= 0)
            {
                var evaluation = evaluations.Last().Value;
                var coordinate = new Coordinate
                {
                    PositionX = evaluation.FormationEvaluation.TargetLocation % GameplayConstants.MaxFormationColumns,
                    PositionY = evaluation.FormationEvaluation.TargetLocation / GameplayConstants.MaxFormationRows
                };

                return new CombatAiDecision
                {
                    Ability = evaluation.Ability,
                    Actor = activeEntity,
                    TargetCoordinate = coordinate,
                    TargetFormation = evaluation.TargetFormation
                };
            }
            // Randomly choose from the highest value AbilityEvaluation if any ai randomness
            else
            {
                var highestValues = evaluations.TakeLast(_myFormation.AiRandomness + 1)
                                               .Select(kvp => kvp.Value);

                var random = _rand.Next(highestValues.Count());

                var chosenEvaluation = highestValues.ElementAt(random);
                var coordinate = new Coordinate
                {
                    PositionX = chosenEvaluation.FormationEvaluation.TargetLocation % GameplayConstants.MaxFormationColumns,
                    PositionY = chosenEvaluation.FormationEvaluation.TargetLocation / GameplayConstants.MaxFormationRows
                };

                return new CombatAiDecision
                {
                    Ability = chosenEvaluation.Ability,
                    Actor = activeEntity,
                    TargetCoordinate = coordinate,
                    TargetFormation = chosenEvaluation.TargetFormation
                };
            }
        }

        /// <summary>
        /// Evaluates the given Ability being used by the provided CombatEntity against all of the
        /// Formations currently in the battle.
        /// <para>Returns the best target location and target Formation altered by the AiRandomnessFactor
        /// of this Ai's Formation.</para>
        /// </summary>
        /// <param name="activeEntity">The CombatEntity who's ability is being evaluated.</param>
        /// <param name="ability">The Ability being evaluated.</param>
        /// <returns></returns>
        private AbilityEvaluation EvaluateAbility(CombatEntity activeEntity, Ability ability)
        {
            var comparer = new DuplicateKeyComparer();
            var evaluations = new SortedList<int, AbilityEvaluation>(comparer);

            foreach (var enemy in _myEnemies)
            {
                var evaluation = EvaluateFormation(activeEntity, ability, enemy, true);
                if (evaluation == null) continue;

                evaluations.Add(evaluation.TotalValue, new AbilityEvaluation
                {
                    Ability = ability,
                    FormationEvaluation = evaluation,
                    TargetFormation = enemy
                });
            }

            foreach (var ally in _myAllies)
            {
                var evaluation = EvaluateFormation(activeEntity, ability, ally, false);
                if (evaluation == null) continue;

                evaluations.Add(evaluation.TotalValue, new AbilityEvaluation
                {
                    Ability = ability,
                    FormationEvaluation = evaluation,
                    TargetFormation = ally
                });
            }

            if (evaluations.Count == 0) return null;

            // If no ai randomness, choose highest value FormationEvaluation
            if (_myFormation.AiRandomness <= 0)
            {
                return evaluations.Last().Value;
            }
            // Randomly choose from the highest value FormationEvaluation if any ai randomness
            else
            {
                var highestValues = evaluations.TakeLast(_myFormation.AiRandomness + 1)
                                               .Select(kvp => kvp.Value);

                var random = _rand.Next(highestValues.Count());

                return highestValues.ElementAt(random);
            }
        }

        /// <summary>
        /// Returns a FormationEvaluation for the given CombatEntity which will use the given Ability on
        /// the provided Formation.
        /// <para>The FormationEvaluation will contain the best target location for the given Ability depending
        /// on the AiRandomnesFactor for the ai Formation.</para>
        /// </summary>
        /// <param name="activeEntity">The CombatEntity that will be acting.</param>
        /// <param name="ability">The Ability being evaluated.</param>
        /// <param name="formation">The Formation being targeted.</param>
        /// <param name="isEnemyFormation">True if the provided Formation is an enemy Formation. False if allied.</param>
        /// <returns></returns>
        private FormationEvaluation EvaluateFormation(CombatEntity activeEntity, 
                                                      Ability ability, 
                                                      Formation formation,
                                                      bool isEnemyFormation)
        {
            bool isOffensive = ability.IsOffensive.GetValueOrDefault();

            // Only allows offensive abilities against enemy formations and defensive abilities on allied formations
            if ((!isOffensive && isEnemyFormation) || (isOffensive && !isEnemyFormation)) return null;

            var comparer = new DuplicateKeyComparer();
            var evaluations = new SortedList<int, FormationEvaluation>(comparer);

            // Calculate total ai weight modifier for this ability
            int aiWeightModifier = ability.SelfAppliedStatusEffects.Concat(ability.AppliedStatusEffects)
                                                                   .Select(status => status.AiWeightModifier)
                                                                   .Sum();
            aiWeightModifier += ability.AiWeightModifier;

            // Evaluate every target in the formation and record the evaluations in the SortedList
            for (int i = 0; i < formation.Positions.Length; i++)
            {
                for (int j = 0; j < formation.Positions[i].Length; j++)
                {
                    int newCenter = i * GameplayConstants.MaxFormationRows + j + 1;
                    var targets = FormationTargeter.GetTargets(ability, newCenter, formation);

                    // No valid targets, skip this position
                    if (targets == null || targets.Count() <= 0) continue;

                    int totalThreat = targets.Select(entity => entity.Threat)
                                             .Sum();

                    // Calculate total value using damage if using the ability offensively
                    if (isEnemyFormation && isOffensive)
                    {
                        // Ignore dead entities
                        targets = targets.Where(entity => entity.Resources.CurrentHealth > 0).ToList();
                        if (targets.Count() == 0) continue;

                        // Gets total potential damage dealt to each target as a percentage
                        int damagePercentage = targets.Select(entity =>
                        {
                            int damage = DamageCalculator.GetTotalDamageAsInt(activeEntity, entity, ability);
                            int damageAsPercent = damage * 100 / entity.Resources.MaxHealth;

                            return damageAsPercent + DamageCalculator.GetPercentageDamageAsInt(entity, ability);
                        }).Sum();

                        int totalValue = totalThreat + damagePercentage + aiWeightModifier;

                        evaluations.Add(totalValue, new FormationEvaluation
                        {
                            TargetLocation = newCenter,
                            TotalValue = totalValue
                        });
                    }

                    // Calculate total value using healing if using the ability defensively
                    else if (!isEnemyFormation && !isOffensive)
                    {
                        // Ignore dead entities, update later when adding resurrection spells
                        targets = targets.Where(entity => entity.Resources.CurrentHealth > 0).ToList();
                        if (targets.Count() == 0) continue;

                        int healPercentage = targets.Select(entity =>
                        {
                            // Value cannot exceed max heal amount, prevents ai from wanting to overheal
                            int maxHealPercent = 100 - (entity.Resources.CurrentHealth * 100 / entity.Resources.MaxHealth);
                            int healPercent = DamageCalculator.GetHeal(activeEntity, ability) * 100 / entity.Resources.MaxHealth;
                            healPercent += ability.PercentHeal;

                            return (healPercent > maxHealPercent) ? maxHealPercent : healPercent;
                        }).Sum();

                        int totalValue = totalThreat + healPercentage + aiWeightModifier;

                        evaluations.Add(totalValue, new FormationEvaluation
                        {
                            TargetLocation = newCenter,
                            TotalValue = totalValue
                        });
                    }
                }
            }

            if (evaluations.Count == 0) return null;

            // Returns best target evaluation if no randomness
            if (_myFormation.AiRandomness <= 0) return evaluations.Last().Value;

            // If any ai randomness, choose randomly between the best targets
            else
            {
                var highestValues = evaluations.TakeLast(_myFormation.AiRandomness + 1)
                                               .Select(kvp => kvp.Value);

                var random = _rand.Next(highestValues.Count());

                return highestValues.ElementAt(random);
            }
        }
    }
}
