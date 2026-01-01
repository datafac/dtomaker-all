using System.Buffers;

namespace DTOMaker.Runtime.MemBlocks;

/// <summary>
/// Defines a contract for entities that can be created from a sequence of bytes and provide access to a factory for
/// creating instances of type T.
/// </summary>
public interface IMemBlocksEntity<T> : IMemBlocksEntityBase
{
#if NET8_0_OR_GREATER
    /// <summary>
    /// Creates a new instance of type T from the specified sequence of bytes.
    /// </summary>
    static abstract T CreateInstance(ReadOnlySequence<byte> buffers);
#endif

    /// <summary>
    /// Gets the factory instance used to create entities of type T.
    /// </summary>
    IMemBlocksEntityFactory<T> GetFactory();
}
