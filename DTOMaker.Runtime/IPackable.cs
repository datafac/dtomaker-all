using DataFac.Storage;
using DTOMaker.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DTOMaker.Runtime;

/// <summary>
/// Defines a contract for an entity that supports incremental serialization and deserialization 
/// through a data store, called packing and unpacking, allowing for efficient handling of 
/// large entities with potentially deep object graphs.
/// </summary>
public interface IPackable : IEntityBase
{
    /// <summary>
    /// Returns true if the entity is packed and ready for serialization, otherwise false. 
    /// A packed entity is also frozen and immutable.
    /// </summary>
    bool IsPacked { get; }

    /// <summary>
    /// Prepares the entity for serialization, which includes packing and emitting any large 
    /// strings, binary blobs (Octets) and any referenced entities to the data store.
    /// </summary>
    ValueTask Pack(IBlobStore blobStore, CancellationToken cancellation);

    /// <summary>
    /// Returns true if the entity has been unpacked from the data store, otherwise false. 
    /// </summary>
    bool IsUnpacked { get; }

    /// <summary>
    /// Performs a shallow restore of the entity's state from the data store to the depth 
    /// specified. The default depth is 0, which means only the entity and its local properties
    /// will be restored. To access deeper levels of the entity's state, the caller can specify 
    /// a greater depth, or call UnpackAll to restore the entire state, or make additional 
    /// calls to Unpack with increasing depth as needed.
    /// </summary>
    ValueTask Unpack(IBlobStore blobStore, int depth, CancellationToken cancellation);

    /// <summary>
    /// Performs a full restore of the entity's state from the data store.
    /// </summary>
    ValueTask UnpackAll(IBlobStore blobStore, CancellationToken cancellation);
}
