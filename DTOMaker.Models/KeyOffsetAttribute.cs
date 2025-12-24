using System;

namespace DTOMaker.Models;

[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
public class KeyOffsetAttribute : Attribute
{
    public readonly int KeyOffset;

    public KeyOffsetAttribute(int keyOffset)
    {
        KeyOffset = keyOffset;
    }
}
