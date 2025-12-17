using System;

namespace DTOMaker.Models;

/// <summary>
/// Defines the length (in bytes) of the memory block containing a UTF8-encoded
/// string or byte array. The length must be a power of 2 up to 8K. The first
/// 1-2 bytes of the block contain the actual number of bytes encoded, and the 
/// remainder contains the encoded characters.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class FixedLengthAttribute : Attribute
{
    public readonly int Length;

    public FixedLengthAttribute(int length)
    {
        Length = length;
    }
}
