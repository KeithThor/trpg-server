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
        private readonly List<Formation> _formations;
        private readonly FormationFactory _formationFactory;

        public FormationManager(FormationFactory formationFactory)
        {
            _formations = new List<Formation>();
            _formationFactory = formationFactory;
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

            if (formation != null)
            {
                _formations.Add(formation);
            }
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
            var index = _formations.FindIndex(f => f.Id == id);
            if (index == -1) return false;
            if (_formations[index].OwnerId != ownerId) return false;

            _formations.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Retrieves a formation using the given id.
        /// </summary>
        /// <param name="id">The id of the formation to retrieve.</param>
        /// <returns></returns>
        public IReadOnlyFormation GetFormation(int id)
        {
            return _formations.FirstOrDefault(f => f.Id == id);
        }

        /// <summary>
        /// Gets all the formations owned by the user with the given id.
        /// </summary>
        /// <param name="ownerId">The id of the user to retrieve the formations of.</param>
        /// <returns>Returns a set of formations owned by the given user.</returns>
        public IEnumerable<IReadOnlyFormation> GetFormations(Guid ownerId)
        {
            return _formations.Where(f => f.OwnerId == ownerId);
        }

        /// <summary>
        /// Returns a set of formations that match a given predicate.
        /// </summary>
        /// <param name="predicate">The function to use to filter a set of formations.</param>
        /// <returns>Returns a set of formations that match a predicate.</returns>
        public IEnumerable<IReadOnlyFormation> GetFormations(Func<IReadOnlyFormation, bool> predicate)
        {
            return _formations.Where(predicate);
        }

        /// <summary>
        /// Updates the Formation of the id given in the provided template with one created from the template.
        /// </summary>
        /// <param name="template">The template to use to update.</param>
        /// <returns>A readonly reference to the updated Formation.</returns>
        public IReadOnlyFormation UpdateFormation(FormationTemplate template)
        {
            if (template.Id == null) return null;
            var index = _formations.FindIndex(f => f.Id == template.Id);
            if (index == -1) return null;

            var formation = _formationFactory.Create(template);
            formation.Id = template.Id.Value;
            if (formation == null) return null;

            _formations[index] = formation;
            return _formations[index];
        }
    }
}
