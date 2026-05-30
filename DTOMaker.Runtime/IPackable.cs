using DataFac.Storage;
using System;
using System.Threading.Tasks;

namespace DTOMaker.Runtime;

/// <summary>
/// Defines the contract for an entity that manages memory blocks and supports packing and 
/// unpacking operations to and from a data store.
/// </summary>
public interface IPackable
{
    /// <summary>
    /// Returns the serialized state of the entity. This will usually fail if the 
    /// entity has not been packed yet.
    /// </summary>
    ReadOnlyMemory<byte> GetPacked();

    /// <summary>
    /// Prepares the entity for serialization, which includes packing and emitting any large 
    /// strings, binary blobs (Octets) and any referenced entities to the data store.
    /// </summary>
    ValueTask Pack(IDataStore dataStore);

    /// <summary>
    /// Performs a shallow restore of the entity's state from the data store to the depth 
    /// specified. The default depth is 0, which means only the entity and its local properties
    /// will be restored. To access deeper levels of the entity's state, the caller can specify 
    /// a greater depth, or call UnpackAll to restore the entire state, or make additional 
    /// calls to Unpack with increasing depth as needed.
    /// </summary>
    ValueTask Unpack(IDataStore dataStore, int depth = 0);

    /// <summary>
    /// Performs a full restore of the entity's state from the data store.
    /// </summary>
    ValueTask UnpackAll(IDataStore dataStore);
}

/// <summary>
/// Defines the contract for a strongly-typed entity that supports packing and unpacking 
/// operations to and from a data store.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPackable<T> : IPackable where T : class, IPackable<T>
{
    /// <summary>
    /// Deserializes an instance of the entity from the provided packed buffer.
    /// </summary>
    /// <param name="buffer"></param>
#if NET6_0_OR_GREATER
    static abstract T Deserialize(ReadOnlyMemory<byte> buffer);
#endif
}
