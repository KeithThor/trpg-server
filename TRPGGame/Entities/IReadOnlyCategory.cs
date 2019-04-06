using System.Collections.Generic;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Interface providing read-only references to a Category object.
    /// </summary>
    public interface IReadOnlyCategory
    {
        /// <summary>
        /// Gets the description of the category.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the id of the category.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets the name of the category.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the string paths that point to the uris of the icons that represent this category.
        /// </summary>
        IEnumerable<string> IconUris { get; }
    }
}