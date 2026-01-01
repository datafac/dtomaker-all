using System.Buffers;

namespace DTOMaker.Runtime.MemBlocks;

/// <summary>
/// Defines a factory for creating instances of type T from a sequence of bytes.
/// </summary>
public interface IMemBlocksEntityFactory<T>
{
    /// <summary>
    /// Creates an instance of type T by deserializing the provided sequence of bytes.
    /// </summary>
    T CreateInstance(ReadOnlySequence<byte> buffers);
}
