using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities
{
    /// <summary>
    /// An object containing the amount of damage increased per point of stat.
    /// </summary>
    public class DamagePerStat : IReadOnlyDamagePerStat
    {
        public DamageTypes Strength { get; set; } = new DamageTypes();
        public DamageTypes Dexterity { get; set; } = new DamageTypes();
        public DamageTypes Agility { get; set; } = new DamageTypes();
        public DamageTypes Intelligence { get; set; } = new DamageTypes();
        public DamageTypes Constitution { get; set; } = new DamageTypes();

        public static DamagePerStat operator + (DamagePerStat first, DamagePerStat second)
        {
            return new DamagePerStat
            {
                Strength = first.Strength + second.Strength,
                Dexterity = first.Dexterity + second.Dexterity,
                Agility = first.Agility + second.Agility,
                Intelligence = first.Intelligence + second.Intelligence,
                Constitution = first.Constitution + second.Constitution
            };
        }

        IReadOnlyDamageTypes IReadOnlyDamagePerStat.Agility => Agility;

        IReadOnlyDamageTypes IReadOnlyDamagePerStat.Constitution => Constitution;

        IReadOnlyDamageTypes IReadOnlyDamagePerStat.Dexterity => Dexterity;

        IReadOnlyDamageTypes IReadOnlyDamagePerStat.Intelligence => Intelligence;

        IReadOnlyDamageTypes IReadOnlyDamagePerStat.Strength => Strength;
    }
}
