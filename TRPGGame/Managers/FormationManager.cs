using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TRPGGame.Entities;
using TRPGGame.Services;

namespace TRPGGame.Managers
{
    /// <summary>
    /// Manager responsible for handling CRUD operations dealing with combat Formations.
    /// </summary>
    public class FormationManager : IFormationManager
    {
        // Todo: Get FormationDbContext instead of in memory store.
        private readonly Dictionary<Guid, List<Formation>> _formations;
        private readonly IFormationFactory _formationFactory;
        private readonly IWorldEntityAssigner _worldEntityAssigner;
        private object _lock = new object();

        public FormationManager(IFormationFactory formationFactory,
                                IWorldEntityAssigner worldEntityAssigner)
        {
            _formations = new Dictionary<Guid, List<Formation>>();
            _formationFactory = formationFactory;
            _worldEntityAssigner = worldEntityAssigner;
        }

        /// <summary>
        /// Creates a formation from the given template and returns a read-only copy of it.
        /// <para>Will return null if the template was invalid.</para>
        /// </summary>
        /// <param name="template">The template used to create the formation.</param>
        /// <returns></returns>
        public IReadOnlyFormation CreateFormation(FormationTemplate template)
        {
            var formation = _formationFactory.Create(template);
            var swap = new List<Formation> { formation };
            var shouldSetWorldEntity = template.MakeActive;

            if (formation != null)
            {
                lock (_lock)
                {
                    if (_formations.ContainsKey(template.OwnerId)) _formations[template.OwnerId].Add(formation);
                    else
                    {
                        _formations.Add(template.OwnerId, swap);
                        shouldSetWorldEntity = true;
                    }
                }
            }

            if (shouldSetWorldEntity) _worldEntityAssigner.AssignWorldEntity(template.OwnerId, formation);

            return formation;
        }

        /// <summary>
        /// Deletes a Formation given the id of the template to delete and the id of the owner of that template.
        /// </summary>
        /// <param name="id">The unique identifier for the Formation to delete.</param>
        /// <param name="ownerId">The unique identifier for the owner of the Formation.</param>
        /// <returns>Returns true if the delete operation was successful.</returns>
        public bool DeleteFormation(int id, Guid ownerId)
        {
            lock (_lock)
            {
                if (_formations.ContainsKey(ownerId))
                {
                    if (_formations[ownerId].Count <= 1) _formations.Remove(ownerId);
                    else _formations[ownerId].RemoveAll(f => f.Id == id);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Retrieves a formation using the given id and owner id.
        /// </summary>
        /// <param name="ownerId">The id of the owner of the formation to retrieve.</param>
        /// <param name="id">The id of the formation to retrieve.</param>
        /// <returns></returns>
        public IReadOnlyFormation GetFormation(Guid ownerId, int id)
        {
            lock (_lock)
            {
                if (_formations.ContainsKey(ownerId))
                {
                    return _formations[ownerId].FirstOrDefault(f => f.Id == id);
                }
            }
            return null;
        }

        /// <summary>
        /// Gets all the formations owned by the user with the given id.
        /// </summary>
        /// <param name="ownerId">The id of the user to retrieve the formations of.</param>
        /// <returns>Returns a set of formations owned by the given user.</returns>
        public IEnumerable<IReadOnlyFormation> GetFormations(Guid ownerId)
        {
            var failed = new List<IReadOnlyFormation>();
            lock (_lock)
            {
                if (_formations.ContainsKey(ownerId)) return _formations[ownerId];
                else return failed;
            }
        }

        /// <summary>
        /// Updates the Formation of the id given in the provided template with one created from the template.
        /// </summary>
        /// <param name="template">The template to use to update.</param>
        /// <returns>A readonly reference to the updated Formation.</returns>
        public IReadOnlyFormation UpdateFormation(FormationTemplate template)
        {
            if (template.Id == null) return null;
            Formation oldFormation = null;
            bool shouldSetWorldEntity = template.MakeActive;

            lock (_lock)
            {
                if (!_formations.ContainsKey(template.OwnerId)) return null;
                oldFormation = _formations[template.OwnerId].FirstOrDefault(f => f.Id == template.Id);
            }

            if (oldFormation == null || oldFormation.OwnerId != template.OwnerId) return null;

            var formation = _formationFactory.Create(template);
            if (formation == null) return null;
            formation.Id = oldFormation.Id;
            var swap = new List<Formation> { formation };

            lock (_lock)
            {
                if (_formations.ContainsKey(template.OwnerId))
                {
                    if (_formations[template.OwnerId].Count >= 1)
                    {
                        _formations[template.OwnerId].Remove(oldFormation);
                        _formations[template.OwnerId].Add(formation);
                        if (_formations[template.OwnerId].Count == 1) shouldSetWorldEntity = true;
                    }
                    else
                    {
                        _formations.Add(template.OwnerId, swap);
                        shouldSetWorldEntity = true;
                    }
                }
                else throw new Exception($"Tried to update the Formation of player {template.OwnerId} when none exists!");
            }

            if (shouldSetWorldEntity) _worldEntityAssigner.AssignWorldEntity(template.OwnerId, formation);

            return formation;
        }
    }
}
