using System.Threading.Tasks;
using TRPGGame.Entities;
using TRPGGame.Entities.Data;

namespace TRPGGame.Services
{
    /// <summary>
    /// Factory responsible for creating and populating Formations.
    /// </summary>
    public interface IFormationFactory
    {
        /// <summary>
        /// Creates a Formation from a given FormationTemplate.
        /// </summary>
        /// <param name="template">The template to use to create the Formation.</param>
        /// <returns></returns>
        Formation Create(FormationTemplate template);

        /// <summary>
        /// Creates a Formation from an EnemyFormationTemplate asynchronously.
        /// </summary>
        /// <param name="template">The EnemyFormationTemplate used to create the Formation.</param>
        /// <returns></returns>
        Task<Formation> CreateAsync(EnemyFormationTemplate template);
    }
}