﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRPGGame.Entities;
using TRPGGame.Entities.Data;
using TRPGGame.Repository;

namespace TRPGGame.Services
{
    public class PlayerEntityFactory
    {
        private readonly IRepository<CharacterBase> _characterBaseRepo;
        private readonly IRepository<CharacterHair> _characterHairRepo;

        // Todo: remove reference to WEM, save entities in dbset instead of in memory.
        private readonly WorldEntityManager _worldEntityManager;
        private readonly WorldEntityFactory _worldEntityFactory;

        public PlayerEntityFactory(IRepository<CharacterBase> characterBaseRepo,
                                   IRepository<CharacterHair> characterHairRepo,
                                   WorldEntityManager worldEntityManager,
                                   WorldEntityFactory worldEntityFactory)
        {
            _characterBaseRepo = characterBaseRepo;
            _characterHairRepo = characterHairRepo;
            _worldEntityManager = worldEntityManager;
            _worldEntityFactory = worldEntityFactory;
        }

        private static int _id = 0;

        /// <summary>
        /// Creates a player combat entity from a given character template asynchronously.
        /// </summary>
        /// <param name="template">The template containing specifications on how to create the entity.</param>
        /// <returns>Returns the combat entity if the operation was successful, else returns null.</returns>
        public async Task<IReadOnlyCombatEntity> CreateAsync(CharacterTemplate template)
        {
            var hairData = await _characterHairRepo.GetDataAsync();
            var baseData = await _characterBaseRepo.GetDataAsync();
            IEnumerable<string> iconUris;

            try
            {
                // Find the corresponding iconUris and arrange them in the correct order from
                // bottom layer to top
                var hair = hairData.First(h => h.Id == template.HairId).IconUri;
                var body = baseData.First(b => b.Id == template.BaseId).IconUri;

                iconUris = new List<string>
                {
                    body,
                    hair
                };
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

            var character = new CombatEntity
            {
                Id = _id++,
                Name = template.Name,
                OwnerId = template.OwnerId,
                IconUris = iconUris,
                GroupId = template.GroupId,
                OwnerName = template.OwnerName
            };

            _worldEntityManager.SaveCombatEntity(character);
            await _worldEntityFactory.CreateAsync(character);
            // Todo: Add to dbset here after creating EFCore migration

            return character;
        }

        /// <summary>
        /// Updates an existing combat entity with the template given asynchronously.
        /// <para>Will return null if the operation failed.</para>
        /// </summary>
        /// <param name="template">The template to use to update the entity with.</param>
        /// <returns>Returns the modified combat entity or null if no entity was modified.</returns>
        public async Task<IReadOnlyCombatEntity> UpdateAsync(CharacterTemplate template)
        {
            var hairData = await _characterHairRepo.GetDataAsync();
            var baseData = await _characterBaseRepo.GetDataAsync();
            IEnumerable<string> iconUris;

            try
            {
                // Find the corresponding iconUris and arrange them in the correct order
                var hair = hairData.First(h => h.Id == template.HairId).IconUri;
                var body = baseData.First(b => b.Id == template.BaseId).IconUri;

                iconUris = new List<string>
                {
                    body,
                    hair
                };
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

            var character = _worldEntityManager.GetCombatEntities()
                                               .FirstOrDefault(e => e.Id == template.EntityId);

            if (character == null) return null;
            if (character.OwnerId != template.OwnerId) return null;

            character.Name = template.Name;
            character.GroupId = template.GroupId;
            character.IconUris = iconUris;

            return character;
        }
    }
}