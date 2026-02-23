using System;

namespace DTOMaker.Models;

/// <summary>
/// Defines the endian-ness (big or little) in memory of the member.
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

