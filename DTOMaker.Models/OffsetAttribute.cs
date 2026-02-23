using System;

namespace DTOMaker.Models;

/// <summary>
/// Defines the offset within the memory block of the member.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class OffsetAttributeNotUsed : Attribute
{
    public readonly int FieldOffset;

    public OffsetAttributeNotUsed(int fieldOffset)
    {
        FieldOffset = fieldOffset;
    }
}
