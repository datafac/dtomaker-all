namespace DTOMaker.Models;

/// <summary>
/// Represents an entity, a freezable type which is modifiable until frozen.
/// </summary>
public interface IEntityBase
{
    /// <summary>
    /// Returns true if the entity is frozen, otherwise false.
    /// </summary>
    bool IsFrozen { get; }

    /// <summary>
    /// If not already frozen, recursively freezes the entity, making it immutable.
    /// </summary>
    void Freeze();

    /// <summary>
    /// Returns an unfrozen, shallow copy of the entity.
    /// </summary>
    IEntityBase PartCopy();
}
