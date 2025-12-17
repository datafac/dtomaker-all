using System;

namespace DTOMaker.Models;

/// <summary>
/// Defines the endian-ness (big or little) of the memory layout of the member.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class EndianAttribute : Attribute
{
    public readonly bool IsBigEndian;

    public EndianAttribute(bool isBigEndian)
    {
        IsBigEndian = isBigEndian;
    }
}

