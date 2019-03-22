using System;
using System.Collections.Generic;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents a read only version of a combat entity.
    /// </summary>
    public interface IReadOnlyCombatEntity
    {
        int? GroupId { get; }
        IEnumerable<string> IconUris { get; }
        int Id { get; }
        string Name { get; }
        Guid OwnerId { get; }
        string OwnerName { get; }
    }
}