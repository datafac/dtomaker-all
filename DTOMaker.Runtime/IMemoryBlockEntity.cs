using DataFac.Storage;
using System.Threading.Tasks;

namespace DTOMaker.Runtime;

/// <summary>
/// Defines the contract for an entity that manages memory blocks and supports packing and unpacking operations to and
/// from a data store.
/// </summary>
public interface IMemoryBlockEntity
{
    /// <summary>
    /// Gets the metadata and readonly buffers of the entity.
    /// </summary>
    EntityContent GetContent();

    /// <summary>
    /// Asynchronously prepares the entity's data for emission, which includes emitting any referenced entities to the data store.
    /// </summary>
    ValueTask Pack(IDataStore dataStore);

    /// <summary>
    /// Asynchonously restores the entity's state from the data store, with an optional depth parameter to control the unpacking level.
    /// </summary>
    ValueTask Unpack(IDataStore dataStore, int depth = 0);

    /// <summary>
    /// Asynchronously restores the entity's full state from the data store.
    /// </summary>
    ValueTask UnpackAll(IDataStore dataStore);
}


