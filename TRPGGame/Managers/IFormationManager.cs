using System;
using System.Collections.Generic;
using TRPGGame.Entities;

namespace TRPGGame.Managers
{
    /// <summary>
    /// Interface for a manager that handles CRUD operations for Formation entities.
    /// </summary>
    public interface IFormationManager
    {
        /// <summary>
        /// Creates a Formation using a given template.
        /// </summary>
        /// <param name="template">The template used to create a new Formation.</param>
        /// <returns>Returns a read-only copy of the newly created Formation.</returns>
        IReadOnlyFormation CreateFormation(FormationTemplate template);

        /// <summary>
        /// Updates an existing Formation using the template provided.
        /// </summary>
        /// <param name="template">The template to use to update an existing Formation.</param>
        /// <returns>Returns a read-only copy of the updated Formation.</returns>
        IReadOnlyFormation UpdateFormation(FormationTemplate template);

        /// <summary>
        /// Deletes a Formation given its id and the id of its owner.
        /// </summary>
        /// <param name="id">The id of the Formation.</param>
        /// <param name="ownerId">The id of the owner of Formation.</param>
        /// <returns></returns>
        bool DeleteFormation(int id, Guid ownerId);

        /// <summary>
        /// Retrieves a formation using the given id and owner id.
        /// </summary>
        /// <param name="ownerId">The id of the owner of the formation to retrieve.</param>
        /// <param name="id">The id of the formation to retrieve.</param>
        /// <returns></returns>
        IReadOnlyFormation GetFormation(Guid ownerId, int id);

        /// <summary>
        /// Retrieves a set of Formations belonging to the user with the provided id.
        /// </summary>
        /// <param name="ownerId">The id of the user of whom to retrieve Formations of.</param>
        /// <returns>An IEnumerable of read-only references to the Formations that belong to the user.</returns>
        IEnumerable<IReadOnlyFormation> GetFormations(Guid ownerId);
    }
}