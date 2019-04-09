namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents a read-only reference to an ItemType.
    /// </summary>
    public interface IReadOnlyItemType
    {
        /// <summary>
        /// The description of this ItemType. Might not be used.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The unique identifier of this ItemType.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name of this ItemType.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The name of the position to equip an item who is of this item type.
        /// </summary>
        string EquipPosition { get; }
    }
}