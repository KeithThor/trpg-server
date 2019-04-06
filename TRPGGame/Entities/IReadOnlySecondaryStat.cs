namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents a read-only reference to a SecondaryStat object.
    /// </summary>
    public interface IReadOnlySecondaryStat
    {
        /// <summary>
        /// Amount of bonus applied to abilities that do more than 0 points of healing.
        /// </summary>
        int HealBonus { get; }

        /// <summary>
        /// Amount of bonus in percentage applied to abilities that do more than 0 points of healing.
        /// <para>Only applies to flat heal amounts (not percentage heals).</para>
        /// </summary>
        int HealPercentageBonus { get; }

        /// <summary>
        /// The amount of extra action points gained per turn.
        /// </summary>
        int BonusActionPoints { get; }

        /// <summary>
        /// The amount of extra action points gained as a percentage per turn.
        /// </summary>
        int BonusActionPointsPercentage { get; }

        /// <summary>
        /// The amount of action point cost reduced.
        /// </summary>
        int ActionPointCostReduction { get; }

        /// <summary>
        /// The amount of action point cost reduced as a percentage.
        /// </summary>
        int ActionPointCostReductionPercentage { get; }

        /// <summary>
        /// The amount of mana cost reduced.
        /// </summary>
        int ManaCostReduction { get; }

        /// <summary>
        /// The amount of mana cost reduced as a percentage.
        /// </summary>
        int ManaCostReductionPercentage { get; }

        /// <summary>
        /// Chance in percentage that an ability has of critically striking, applying the crit damage bonus
        /// to any heal and damage dealt (except percentage heals).
        /// </summary>
        int CritChance { get; }

        /// <summary>
        /// Increases the amount of crit chance by a percentage.
        /// </summary>
        int CritChancePercentage { get; }

        /// <summary>
        /// The increase in percentage in damage and healing done by an ability when critically striking.
        /// </summary>
        int CritDamage { get; }

        /// <summary>
        /// Increases the crit damage by a percentage.
        /// </summary>
        int CritDamagePercentage { get; }

        /// <summary>
        /// The amount of increase in damage to each damage type.
        /// </summary>
        IReadOnlyDamageTypes Damage { get; }

        /// <summary>
        /// The amount of increase in damage in percentage of each damage type.
        /// </summary>
        IReadOnlyDamageTypes DamagePercentage { get; }

        /// <summary>
        /// The amount of armor against each damage type.
        /// </summary>
        IReadOnlyDamageTypes Armor { get; }

        /// <summary>
        /// The amount of increase in armor in percentage against each damage type.
        /// </summary>
        IReadOnlyDamageTypes ArmorPercentage { get; }

        /// <summary>
        /// The amount of damage reduced or increased against each damage type.
        /// <para>0 is no chance in damage taken. -15 is 85% damage taken. 15 is 115% damage taken.</para>
        /// </summary>
        IReadOnlyDamageTypes Resistances { get; }
    }
}