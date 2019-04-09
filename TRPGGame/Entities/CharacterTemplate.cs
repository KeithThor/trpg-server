using System;
using TRPGGame.Entities.Data;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents a model template that will be used to create a character entity for the player.
    /// </summary>
    public class CharacterTemplate
    {
        public int EntityId { get; set; }
        public string Name { get; set; }
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; }
        public int BaseId { get; set; }
        public int HairId { get; set; }
        public int? ClassTemplateId { get; set; }
        public int? GroupId { get; set; }
        public CharacterStats AllocatedStats { get; set; }

        public const int MaxAllocatedStats = 25;
    }
}
