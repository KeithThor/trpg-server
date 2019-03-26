using System;
using TRPGGame.Entities.Data;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents a read only version of a combat entity.
    /// </summary>
    public interface IReadOnlyCombatEntity
    {
        int? GroupId { get; }
        IReadOnlyCharacterIconSet IconUris { get; }
        int Id { get; }
        string Name { get; }
        Guid OwnerId { get; }
        string OwnerName { get; }
    }
}