using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents a character's in-game secondary combat stats.
    /// </summary>
    public class SecondaryStat : IReadOnlySecondaryStat
    {
        /// <summary>
        /// Amount of bonus applied to abilities that do more than 0 points of healing.
        /// </summary>
        public int HealBonus { get; set; }

        /// <summary>
        /// Amount of bonus in percentage applied to abilities that do more than 0 points of healing.
        /// <para>Only applies to flat heal amounts (not percentage heals).</para>
        /// </summary>
        public int HealPercentageBonus { get; set; }

        /// <summary>
        /// Chance in percentage that an ability has of critically striking, applying the crit damage bonus
        /// to any heal and damage dealt (except percentage heals).
        /// </summary>
        public int CritChance { get; set; }

        /// <summary>
        /// Increases the amount of crit chance by a percentage.
        /// </summary>
        public int CritChancePercentage { get; set; }

        /// <summary>
        /// The increase in percentage in damage and healing done by an ability when critically striking.
        /// </summary>
        public int CritDamage { get; set; }

        /// <summary>
        /// Increases the crit damage by a percentage.
        /// </summary>
        public int CritDamagePercentage { get; set; }

        /// <summary>
        /// The amount of increase in damage to each damage type.
        /// </summary>
        public DamageTypes Damage { get; set; } = new DamageTypes();

        /// <summary>
        /// The amount of increase in damage in percentage of each damage type.
        /// </summary>
        public DamageTypes DamagePercentage { get; set; } = new DamageTypes();

        /// <summary>
        /// The amount of armor against each damage type.
        /// </summary>
        public DamageTypes Armor { get; set; } = new DamageTypes();

        /// <summary>
        /// The amount of increase in armor in percentage against each damage type.
        /// </summary>
        public DamageTypes ArmorPercentage { get; set; } = new DamageTypes();

        /// <summary>
        /// The amount of damage reduced or increased against each damage type.
        /// <para>0 is no chance in damage taken. -15 is 85% damage taken. 15 is 115% damage taken.</para>
        /// </summary>
        public DamageTypes Resistances { get; set; } = new DamageTypes();

        /// <summary>
        /// The amount of extra action points gained per turn.
        /// </summary>
        public int BonusActionPoints { get; set; }

        /// <summary>
        /// The amount of extra action points gained as a percentage per turn.
        /// </summary>
        public int BonusActionPointsPercentage { get; set; }

        /// <summary>
        /// The amount of action point cost reduced.
        /// </summary>
        public int ActionPointCostReduction { get; set; }

        /// <summary>
        /// The amount of action point cost reduced as a percentage.
        /// </summary>
        public int ActionPointCostReductionPercentage { get; set; }

        /// <summary>
        /// The amount of mana cost reduced.
        /// </summary>
        public int ManaCostReduction { get; set; }

        /// <summary>
        /// The amount of mana cost reduced as a percentage.
        /// </summary>
        public int ManaCostReductionPercentage { get; set; }

        public static SecondaryStat operator + (SecondaryStat first, SecondaryStat second)
        {
            return new SecondaryStat
            {
                HealBonus = first.HealBonus + second.HealBonus,
                HealPercentageBonus = first.HealPercentageBonus + second.HealPercentageBonus,
                CritChance = first.CritChance + second.CritChance,
                CritChancePercentage = first.CritChancePercentage + second.CritChancePercentage,
                CritDamage = first.CritDamage + second.CritDamage,
                CritDamagePercentage = first.CritDamagePercentage + second.CritDamagePercentage,
                Damage = first.Damage + second.Damage,
                DamagePercentage = first.DamagePercentage + second.DamagePercentage,
                Armor = first.Armor + second.Armor,
                ArmorPercentage = first.ArmorPercentage + second.ArmorPercentage,
                Resistances = first.Resistances + second.Resistances,
                BonusActionPoints = first.BonusActionPoints + second.BonusActionPoints,
                BonusActionPointsPercentage = first.BonusActionPointsPercentage + second.BonusActionPointsPercentage,
                ActionPointCostReduction = first.ActionPointCostReduction + second.ActionPointCostReduction,
                ActionPointCostReductionPercentage = first.ActionPointCostReductionPercentage + second.ActionPointCostReductionPercentage,
                ManaCostReduction = first.ManaCostReduction + second.ManaCostReduction,
                ManaCostReductionPercentage = first.ManaCostReductionPercentage + second.ManaCostReductionPercentage
            };
        }

        public static SecondaryStat operator -(SecondaryStat first, SecondaryStat second)
        {
            return new SecondaryStat
            {
                HealBonus = first.HealBonus - second.HealBonus,
                HealPercentageBonus = first.HealPercentageBonus - second.HealPercentageBonus,
                CritChance = first.CritChance - second.CritChance,
                CritChancePercentage = first.CritChancePercentage - second.CritChancePercentage,
                CritDamage = first.CritDamage - second.CritDamage,
                CritDamagePercentage = first.CritDamagePercentage - second.CritDamagePercentage,
                Damage = first.Damage - second.Damage,
                DamagePercentage = first.DamagePercentage - second.DamagePercentage,
                Armor = first.Armor - second.Armor,
                ArmorPercentage = first.ArmorPercentage - second.ArmorPercentage,
                Resistances = first.Resistances - second.Resistances,
                BonusActionPoints = first.BonusActionPoints - second.BonusActionPoints,
                BonusActionPointsPercentage = first.BonusActionPointsPercentage - second.BonusActionPointsPercentage,
                ActionPointCostReduction = first.ActionPointCostReduction - second.ActionPointCostReduction,
                ActionPointCostReductionPercentage = first.ActionPointCostReductionPercentage - second.ActionPointCostReductionPercentage,
                ManaCostReduction = first.ManaCostReduction - second.ManaCostReduction,
                ManaCostReductionPercentage = first.ManaCostReductionPercentage - second.ManaCostReductionPercentage
            };
        }

        IReadOnlyDamageTypes IReadOnlySecondaryStat.Damage => Damage;

        IReadOnlyDamageTypes IReadOnlySecondaryStat.DamagePercentage => DamagePercentage;

        IReadOnlyDamageTypes IReadOnlySecondaryStat.Armor => Armor;

        IReadOnlyDamageTypes IReadOnlySecondaryStat.ArmorPercentage => ArmorPercentage;

        IReadOnlyDamageTypes IReadOnlySecondaryStat.Resistances => Resistances;
    }
}
