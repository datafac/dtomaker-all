using System;

namespace DTOMaker.Models;

/// <summary>
/// Defines the offset within the memory block of the member.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class OffsetAttribute : Attribute
{
    public readonly int FieldOffset;

    public OffsetAttribute(int fieldOffset)
    {
        FieldOffset = fieldOffset;
    }
}

