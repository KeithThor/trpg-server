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
        /// Retrieves a Formation by id.
        /// </summary>
        /// <param name="id">The unique identifier of the Formation to retrieve.</param>
        /// <returns>Returns a read-only copy of the found Formation.</returns>
        IReadOnlyFormation GetFormation(int id);

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
        /// Retrieves a set of Formations that satisfy a given predicate.
        /// </summary>
        /// <param name="predicate">The predicate used to filter the Formations.</param>
        /// <returns>An IEnumerable of read-only references to the Formations that match the predicate.</returns>
        IEnumerable<IReadOnlyFormation> GetFormations(Func<IReadOnlyFormation, bool> predicate);

        /// <summary>
        /// Retrieves a set of Formations belonging to the user with the provided id.
        /// </summary>
        /// <param name="ownerId">The id of the user of whom to retrieve Formations of.</param>
        /// <returns>An IEnumerable of read-only references to the Formations that belong to the user.</returns>
        IEnumerable<IReadOnlyFormation> GetFormations(Guid ownerId);
    }
}