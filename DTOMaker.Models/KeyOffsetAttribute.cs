using System;

namespace DTOMaker.Models;

/// <summary>
/// Specifies the key offset for MessagePack model members.
/// </summary>
[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
public class KeyOffsetAttribute : Attribute
{
    public readonly int KeyOffset;

    public KeyOffsetAttribute(int keyOffset)
    {
        KeyOffset = keyOffset;
    }
}
