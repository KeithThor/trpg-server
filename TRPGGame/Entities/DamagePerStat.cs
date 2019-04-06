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
        public DamageTypes Strength { get; set; }
        public DamageTypes Dexterity { get; set; }
        public DamageTypes Agility { get; set; }
        public DamageTypes Intelligence { get; set; }
        public DamageTypes Constitution { get; set; }

        IReadOnlyDamageTypes IReadOnlyDamagePerStat.Agility => Agility;

        IReadOnlyDamageTypes IReadOnlyDamagePerStat.Constitution => Constitution;

        IReadOnlyDamageTypes IReadOnlyDamagePerStat.Dexterity => Dexterity;

        IReadOnlyDamageTypes IReadOnlyDamagePerStat.Intelligence => Intelligence;

        IReadOnlyDamageTypes IReadOnlyDamagePerStat.Strength => Strength;
    }
}
