using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRPGGame.Entities;
using TRPGGame.Services;

namespace TRPGGame.Repository
{
    public class AbilityRepository: IRepository<Ability>
    {
        private readonly IRepository<StatusEffect> _statusEffectRepo;
        private readonly IRepository<Category> _categoryRepo;
        private List<Ability> _abilities;

        public AbilityRepository(IRepository<StatusEffect> statusEffectRepo,
                                 IRepository<Category> categoryRepo)
        {
            _statusEffectRepo = statusEffectRepo;
            _categoryRepo = categoryRepo;
        }

        public async Task<IEnumerable<Ability>> GetDataAsync()
        {
            if (_abilities != null) return _abilities;
            else
            {
                await LoadDataAsync();
                return _abilities;
            }
        }

        private async Task LoadDataAsync()
        {
            using (var reader = new StreamReader(DataConstants.AssemblyLocation + "/Data/Abilities.json"))
            {
                _abilities = new List<Ability>();
                var statusEffects = await _statusEffectRepo.GetDataAsync();
                var categories = await _categoryRepo.GetDataAsync();
                JContainer abilitiesAsList = JsonConvert.DeserializeObject<JContainer>(reader.ReadToEnd());
                foreach (var abilityObject in abilitiesAsList)
                {
                    var ability = abilityObject.ToObject<Ability>();
                    if (abilityObject["categoryId"] != null)
                    {
                        var categoryId = abilityObject["categoryId"].ToObject<int>();
                        var foundCategory = categories.FirstOrDefault(c => c.Id == categoryId);
                        if (foundCategory != null) ability.Category = foundCategory;
                    }

                    var appliedStatusEffects = new List<StatusEffect>();
                    List<int> appliedStatusIds = null;
                    if (abilityObject["appliedStatusEffectIds"] != null)
                    {
                        appliedStatusIds = abilityObject["appliedStatusEffectIds"].ToObject<List<int>>();
                        foreach (var id in appliedStatusIds)
                        {
                            var foundStatus = statusEffects.FirstOrDefault(se => se.Id == id);
                            if (foundStatus != null) appliedStatusEffects.Add(foundStatus);
                        }
                        ability.AppliedStatusEffects = appliedStatusEffects;
                    }

                    var selfAppliedStatusEffects = new List<StatusEffect>();
                    List<int> selfAppliedStatusIds = null;
                    if (abilityObject["selfAppliedStatusEffectIds"] != null)
                    {
                        selfAppliedStatusIds = abilityObject["selfAppliedStatusEffectIds"].ToObject<List<int>>();
                        foreach (var id in selfAppliedStatusIds)
                        {
                            var foundStatus = statusEffects.FirstOrDefault(se => se.Id == id);
                            if (foundStatus != null) selfAppliedStatusEffects.Add(foundStatus);
                        }
                        ability.SelfAppliedStatusEffects = selfAppliedStatusEffects;
                    }

                    if (ability.IsOffensive == null) ability.IsOffensive = IsAbilityOffensive(ability);

                    _abilities.Add(ability);
                }
            }
        }

        /// <summary>
        /// Returns true if an ability should be used for offensive purposes rather than defensive.
        /// </summary>
        /// <param name="ability">The ability to check.</param>
        /// <returns></returns>
        private bool IsAbilityOffensive(Ability ability)
        {
            bool isOffensive = ability.IsOffensive.GetValueOrDefault();
            if (ability.IsOffensive == null)
            {
                var initialDamage = DamageCalculator.GetDamageTypesAsInt(ability.Damage);
                var initialHealing = ability.Heal;

                var damagePerStat = DamageCalculator.GetDamagePerStatAsInt(ability.DamagePerStat);
                var healPerStat = ability.HealPerStat.GetTotalStats();

                var percentDamage = DamageCalculator.GetDamageTypesAsInt(ability.PercentDamage);
                var percentHeal = ability.PercentHeal;

                // Compare heal vs damage, whichever is higher makes the ability defensive vs offensive
                // Places priority on percentage over per stat values and per stat values over flat values
                if (percentHeal > percentDamage) return false;
                else if (percentHeal < percentDamage) return true;
                else if (healPerStat > damagePerStat) return false;
                else if (healPerStat < damagePerStat) return true;
                else if (initialHealing > initialDamage) return false;
                else if (initialHealing < initialDamage) return true;

                // Ability deals no damage and heals no health, check status effects
                if (ability.AppliedStatusEffects.Any()) return AreStatusEffectsOffensive(ability.AppliedStatusEffects);
                else return AreStatusEffectsOffensive(ability.AppliedStatusEffects);
            }

            return isOffensive;
        }

        /// <summary>
        /// Goes through all status effects and check if the net effects are offensive or defensive.
        /// </summary>
        /// <param name="statusEffects">An IEnumerable of StatusEffects to loop through.</param>
        /// <returns>Returns true if the status effects are mostly offensive.</returns>
        private bool AreStatusEffectsOffensive(IEnumerable<StatusEffect> statusEffects)
        {
            int seInitialDamage = 0;
            int seInitialHealing = 0;
            int seDamagePerStat = 0;
            int seHealPerStat = 0;
            int sePercentHeal = 0;
            int percentStats = 0;
            int totalStats = 0;

            // Get totals of all status effects being applied
            foreach (var statusEffect in statusEffects)
            {
                seInitialDamage += DamageCalculator.GetDamageTypesAsInt(statusEffect.DamagePerTurn);
                seInitialHealing += statusEffect.HealPerTurn;
                seDamagePerStat += DamageCalculator.GetDamagePerStatAsInt(statusEffect.DamagePerStatPerTurn);
                seHealPerStat += statusEffect.HealPerStatPerTurn.GetTotalStats();
                sePercentHeal += statusEffect.PercentHealPerTurn;
                percentStats += statusEffect.ModifiedStatPercentages.GetTotalStats();
                totalStats += statusEffect.ModifiedStats.GetTotalStats();
            }

            // Goes through the list of comparisons from highest priority to lowest.
            if (sePercentHeal > 0) return false;
            else if (sePercentHeal < 0) return true;
            else if (percentStats > 0) return false;
            else if (percentStats < 0) return true;
            else if (seHealPerStat > seDamagePerStat) return false;
            else if (seHealPerStat < seDamagePerStat) return true;
            else if (totalStats > 0) return false;
            else if (totalStats < 0) return true;
            else if (seInitialHealing > seDamagePerStat) return false;
            else if (seInitialHealing < seDamagePerStat) return true;

            // Status effects have no net effects, use in any way
            else return false;
        }
    }
}
