using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRPGGame.Entities;
using TRPGGame.Entities.Data;
using TRPGGame.Repository;

namespace TRPGGame.Services
{
    public class CombatEntityFactory
    {
        private readonly IRepository<CharacterBase> _characterBaseRepo;
        private readonly IRepository<CharacterHair> _characterHairRepo;
        private readonly IRepository<Ability> _abilityRepo;

        public CombatEntityFactory(IRepository<CharacterBase> characterBaseRepo,
                                   IRepository<CharacterHair> characterHairRepo,
                                   IRepository<Ability> abilityRepo)
        {
            _characterBaseRepo = characterBaseRepo;
            _characterHairRepo = characterHairRepo;
            _abilityRepo = abilityRepo;
        }

        private static int _id = 0;

        /// <summary>
        /// Creates a player combat entity from a given character template asynchronously.
        /// </summary>
        /// <param name="template">The template containing specifications on how to create the entity.</param>
        /// <returns>Returns the combat entity if the operation was successful, else returns null.</returns>
        public async Task<CombatEntity> CreateAsync(CharacterTemplate template)
        {
            if (!await IsValidTemplateAsync(template)) return null;
            var hairData = await _characterHairRepo.GetDataAsync();
            var baseData = await _characterBaseRepo.GetDataAsync();
            var abilities = await _abilityRepo.GetDataAsync();
            CharacterIconSet iconUris;

            try
            {
                // Find the corresponding iconUris and arrange them in the correct order from
                // bottom layer to top
                var hair = hairData.First(h => h.Id == template.HairId).IconUri;
                var cBase = baseData.First(b => b.Id == template.BaseId).IconUri;

                iconUris = new CharacterIconSet
                {
                    BaseIconUri = cBase,
                    HairIconUri = hair
                };
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

            // Todo: Calculate secondary stats
            var character = new CombatEntity
            {
                Id = _id++,
                Name = template.Name,
                OwnerId = template.OwnerId,
                IconUris = iconUris,
                GroupId = template.GroupId,
                OwnerName = template.OwnerName,
                Stats = template.AllocatedStats,
                Abilities = abilities.ToList(),
                SecondaryStats = new SecondaryStat(),
                StatusEffects = new List<StatusEffect>()
            };
            
            // Todo: Add to dbset here after creating EFCore migration

            return character;
        }

        /// <summary>
        /// Updates an existing combat entity with the template given asynchronously.
        /// <para>Will return null if the operation failed.</para>
        /// </summary>
        /// <param name="template">The template to use to update the entity with.</param>
        /// <returns>Returns the modified combat entity or null if no entity was modified.</returns>
        public async Task<CombatEntity> UpdateAsync(CombatEntity entity, CharacterTemplate template)
        {
            if (!await IsValidTemplateAsync(template)) return null;

            var hairData = await _characterHairRepo.GetDataAsync();
            var baseData = await _characterBaseRepo.GetDataAsync();
            CharacterIconSet iconUris;

            try
            {
                // Find the corresponding iconUris and arrange them in the correct order
                var hair = hairData.First(h => h.Id == template.HairId).IconUri;

                iconUris = new CharacterIconSet(entity.IconUris);
                iconUris.HairIconUri = hair;
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

            var modifiedEntity = new CombatEntity();
            modifiedEntity.Name = template.Name;
            modifiedEntity.GroupId = template.GroupId;
            modifiedEntity.IconUris = iconUris;
            modifiedEntity.Id = entity.Id;
            modifiedEntity.OwnerId = entity.OwnerId;
            modifiedEntity.OwnerName = entity.OwnerName;
            modifiedEntity.Stats = template.AllocatedStats;
            modifiedEntity.SecondaryStats = entity.SecondaryStats;
            modifiedEntity.Abilities = entity.Abilities;

            return modifiedEntity;
        }

        /// <summary>
        /// Checks if the given template is valid asynchronously.
        /// </summary>
        /// <param name="template">The template to validate.</param>
        /// <returns></returns>
        private async Task<bool> IsValidTemplateAsync(CharacterTemplate template)
        {
            if (!await AreStatsValidAsync(template)) return false;
            return true;
        }

        /// <summary>
        /// Checks if the given template is valid asynchronously.
        /// </summary>
        /// <param name="template">The template to validate.</param>
        /// <param name="entity">The entity to validate against.</param>
        /// <returns></returns>
        private async Task<bool> IsValidTemplateAsync(CharacterTemplate template, CombatEntity entity)
        {
            var bases = await _characterBaseRepo.GetDataAsync();
            var selectedBase = bases.FirstOrDefault(b => b.Id == template.BaseId);

            if (selectedBase == null) return false;
            if (entity.IconUris.BaseIconUri != selectedBase.IconUri) return false;
            if (entity.OwnerId != template.OwnerId) return false;
            if (!await IsValidTemplateAsync(template)) return false;
            return true;
        }

        /// <summary>
        /// Checks to see if the allocated stats on a template are valid given the selected character base.
        /// </summary>
        /// <param name="template">The template in which the stats to validate are stored.</param>
        /// <returns></returns>
        private async Task<bool> AreStatsValidAsync(CharacterTemplate template)
        {
            var data = await _characterBaseRepo.GetDataAsync();
            var cBase = data.FirstOrDefault(b => b.Id == template.BaseId);
            if (template.AllocatedStats.GetTotalStats() != CharacterTemplate.MaxAllocatedStats) return false;

            var allocatedStats = template.AllocatedStats.AsArray();
            var bonusStats = cBase.BonusStats.AsArray();
            var maxStats = cBase.MaxStats.AsArray();

            for (int i = 0; i < allocatedStats.Length; i++)
            {
                // Check minimum stats
                if ((bonusStats[i] >= 0 && allocatedStats[i] + bonusStats[i] <= 0) ||
                    (bonusStats[i] < 0 && allocatedStats[i] + bonusStats[i] < 0)) return false;

                // Check maximum stats
                if (allocatedStats[i] + bonusStats[i] > maxStats[i]) return false;
            }

            return true;
        }
    }
}
