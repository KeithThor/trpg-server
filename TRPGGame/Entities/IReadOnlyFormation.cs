using System;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents a read-only version of a combat Formation.
    /// </summary>
    public interface IReadOnlyFormation
    {
        int Id { get; }
        Guid OwnerId { get; }
        IReadOnlyCombatEntity[][] Positions { get; }
        string Name { get; }
    }
}