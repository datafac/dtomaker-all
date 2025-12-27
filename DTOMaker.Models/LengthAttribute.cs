using System;

namespace DTOMaker.Models;

/// <summary>
/// Defines the length (in bytes) of the memory block containing a UTF8-encoded
/// string or byte array, or entity. The length must be a power of 2. For properties,
/// this can be up to 1K. For entities this can be up to 8K.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Interface, AllowMultiple = false)]
public class LengthAttribute : Attribute
{
    public readonly int Length;

    public LengthAttribute(int length)
    {
        Length = length;
    }
}
